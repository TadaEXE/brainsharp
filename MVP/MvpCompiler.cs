using LLVMSharp.Interop;
using LTR = LLVMSharp.Interop.LLVMTypeRef;
using LVR = LLVMSharp.Interop.LLVMValueRef;

namespace MVP
{
  public enum TokenEnum
  {
    Left,
    Right,
    Increment,
    Decrement,
    LoopStart,
    LoopEnd,
    GetInput,
    Print,
    FileStart,
    FileEnd,
    Loop,
    GetPos,
  }

  public struct Token
  {
    public readonly TokenEnum Type;
    public readonly string Word;

    public Token(TokenEnum type) => (Type, Word) = (type, "");

    public Token(TokenEnum type, string word) => (Type, Word) = (type, word);
  }

  public static class Syntax
  {
    public static List<Token> AcceptedTokens =
    [
      new(TokenEnum.Left, "<"),
      new(TokenEnum.Right, ">"),
      new(TokenEnum.Increment, "+"),
      new(TokenEnum.Decrement, "-"),
      new(TokenEnum.LoopStart, "["),
      new(TokenEnum.LoopEnd, "]"),
      new(TokenEnum.GetInput, ","),
      new(TokenEnum.Print, "."),
      new(TokenEnum.GetPos, "$"),
    ];
  }

  public class AstNode
  {
    public Token? Value;
    public AstNode? ParentNode;
    public List<AstNode> ChildNodes = new List<AstNode>();
    public int TextPos;

    public static AstNode RootNode() =>
      new AstNode(new Token(TokenEnum.FileStart, "%ROOT%"), -1);

    public static AstNode EOFNode() =>
      new AstNode(new Token(TokenEnum.FileEnd, "%EOF%"), -2);

    public static AstNode LoopNode() =>
      new AstNode(new Token(TokenEnum.Loop, "$LOOP$"));

    public AstNode(Token value) => (Value, TextPos) = (value, -3);

    public AstNode(Token value, int textPos) =>
      (Value, TextPos) = (value, textPos);

    public AstNode(Token value, List<AstNode> subNodes) =>
      (Value, ChildNodes) = (value, subNodes);

    public AstNode AddChild(AstNode node)
    {
      node.ParentNode = this;
      ChildNodes.Add(node);
      return node;
    }

    public override string ToString()
    {
      if (ChildNodes.Count() == 0)
      {
        return $"({(Value?.Word ?? "Err")})";
      }
      else
      {
        var nodeStrs = ChildNodes.Select(n => n.ToString(0));
        if (nodeStrs is null)
          return (Value?.Word ?? "Err") + "CNE";

        var tmp = "\t" + string.Join("\n\t", nodeStrs);
        return (Value?.Word ?? "Err") + $"({ChildNodes.Count()})\n" + tmp;
      }
    }

    public bool IsLeaf() => ChildNodes.Count() == 0;

    public string ToString(int indent)
    {
      string ind = "";
      for (int i = 0; i < indent; i++)
      {
        ind += "\t";
      }
      if (ChildNodes.Count() == 0)
      {
        return ind + (Value?.Word ?? "Err");
      }
      else
      {
        var nodeStrs = ChildNodes.Select(n => n.ToString(indent + 1));
        if (nodeStrs is null)
          return ind + (Value?.Word ?? "Err") + "CNE";

        var tmp = "\t" + string.Join("\n\t", nodeStrs);
        return ind + (Value?.Word ?? "Err") + $"({ChildNodes.Count()})\n" + tmp;
      }
    }
  }

  public sealed class LLVMHolder : IDisposable
  {
    public LLVMContextRef ctx;
    public LLVMModuleRef mod;
    public LLVMBuilderRef bld;

    public LVR tape;
    public LTR tapeTy;

    public LVR tapeIdx;
    public LTR tapeIdxTy;

    public LVR printfNumFmt;
    public LVR printfCharFmt;
    public LVR printfFn;
    public LTR printfTy;

    private bool disposed = false;

    public LLVMHolder(string name)
    {
      ctx = LLVMContextRef.Create();
      mod = LLVMModuleRef.CreateWithName(name);
      bld = LLVMBuilderRef.Create(ctx);
      (printfFn, printfTy) = declarePrintf();
      (tape, tapeTy, tapeIdx, tapeIdxTy) = makeTape();
    }

    private (LVR printfFn, LTR printfTy) declarePrintf()
    {
      var ptr = LLVMTypeRef.CreatePointer(LTR.Int8, 0);

      var printfTy = LLVMTypeRef.CreateFunction(LTR.Int32, [ptr], true);
      var printfFn = mod.AddFunction("printf", printfTy);
      return (printfFn, printfTy);
    }

    private (LVR tape, LTR typeTy, LVR tapePtr, LTR tapePtrTy) makeTape()
    {
      var tapeTy = LTR.CreateArray2(LTR.Int8, UInt16.MaxValue);
      var tape = mod.AddGlobal(tapeTy, "tape");
      tape.Initializer = LVR.CreateConstNull(tapeTy);

      var tapeIdxTy = LTR.CreateInt(16);
      var tapeIdx = mod.AddGlobal(tapeIdxTy, "tape_idx");
      tapeIdx.Initializer = LVR.CreateConstNull(tapeIdxTy);

      return (tape, tapeTy, tapeIdx, tapeIdxTy);
    }

    public void Dispose()
    {
      if (disposed)
        return;

      ctx.Dispose();
      mod.Dispose();
      bld.Dispose();

      disposed = true;
      GC.SuppressFinalize(this);
    }
  }

  public class Compiler
  {
    private Dictionary<TokenEnum, string> lut;
    private Dictionary<string, TokenEnum> rlut;

    public Compiler()
    {
      lut = new Dictionary<TokenEnum, string>();
      rlut = new Dictionary<string, TokenEnum>();

      foreach (var t in Syntax.AcceptedTokens)
      {
        lut[t.Type] = t.Word;
        rlut[t.Word] = t.Type;
      }
    }

    public List<AstNode> ParseFile(string path)
    {
      var text =
        File.ReadAllText(path)
        ?? throw new Exception($"{path} does not exist!");

      var nodes = new List<AstNode>();
      var curSubText = text;
      var textPos = 0;

      while (textPos < text.Length)
      {
        if (getNextMatch(curSubText) is AstNode node)
        {
          textPos += node.TextPos;
          node.TextPos = textPos;
          textPos += (node.Value?.Word ?? "").Length;

          nodes.Add(node);
          curSubText = text.Substring(textPos);
        }
        else
        {
          break;
        }
      }

      return nodes;
    }

    private AstNode? getNextMatch(string text)
    {
      int low = text.Length + 1;
      TokenEnum? te = null;

      foreach (var v in lut.Values)
      {
        var tmp = text.IndexOf(v);
        if (tmp >= 0 && tmp < low)
        {
          low = tmp;
          te = rlut[v];
        }
      }

      if (low != text.Length + 1 && te is TokenEnum t)
      {
        return new AstNode(new Token(t, lut[t]), low);
      }

      return null;
    }

    public AstNode BuildAst(List<AstNode> nodes)
    {
      var root = AstNode.RootNode();
      var current = root;

      foreach (var node in nodes)
      {
        if (current is AstNode cur)
        {
          if (node.Value?.Type == TokenEnum.LoopStart)
          {
            current = cur.AddChild(AstNode.LoopNode());
          }
          else if (node.Value?.Type == TokenEnum.LoopEnd)
          {
            current =
              cur.ParentNode
              ?? throw new Exception(
                $"Syntax Error: Closed loop without opening. At pos: {cur.TextPos}"
              );
          }
          else
          {
            cur.AddChild(node);
          }
        }
      }

      return root;
    }

    public void CompileAst(AstNode root, string outPath)
    {
      using var hld = new LLVMHolder("brainsharp");

      var mainTy = LTR.CreateFunction(LTR.Int32, Array.Empty<LTR>());
      var mainFn = hld.mod.AddFunction("main", mainTy);
      var entry = mainFn.AppendBasicBlock("entry");
      hld.bld.PositionAtEnd(entry);

      // If we do this earlier, llvm get's mighty upset and won't do jackshit.
      hld.printfNumFmt = hld.bld.BuildGlobalStringPtr("\n$%d\n", "fmt_num");
      hld.printfCharFmt = hld.bld.BuildGlobalStringPtr("%c", "fmt_char");

      var vtable = new Dictionary<TokenEnum, (LVR fn, LTR ty)>()
      {
        { TokenEnum.GetPos, makeGetPos(hld) },
        { TokenEnum.Print, makePrint(hld) },
        { TokenEnum.Increment, makeAdd(hld) },
        { TokenEnum.Decrement, makeSub(hld) },
        { TokenEnum.Right, makeMoveRight(hld) },
        { TokenEnum.Left, makeMoveLeft(hld) },
      };

      hld.bld.PositionAtEnd(entry);
      CompileChildren(root, hld, vtable);

      hld.bld.BuildRet(LVR.CreateConstInt(LTR.Int32, 0));
      hld.mod.PrintToFile(outPath);

      Console.WriteLine($"Wrote to {outPath}");
    }

    private void CompileChildren(
      AstNode parent,
      LLVMHolder hld,
      Dictionary<TokenEnum, (LVR fn, LTR ty)> vtable
    )
    {
      foreach (var node in parent.ChildNodes)
      {
        if (node.IsLeaf())
        {
          if (node.Value is Token tok)
          {
            var (fn, ty) = vtable[tok.Type];
            hld.bld.BuildCall2(ty, fn, []);
          }
        }
        else
        {
          CompileChildren(node, hld, vtable);
        }
      }
    }

    private (LVR fn, LTR fnTy) makeMoveRight(LLVMHolder hld)
    {
      var fnTy = LTR.CreateFunction(LTR.Void, []);
      var fn = hld.mod.AddFunction("move_right", fnTy);
      var entry = fn.AppendBasicBlock("entry");
      hld.bld.PositionAtEnd(entry);

      var idx = hld.bld.BuildLoad2(hld.tapeIdxTy, hld.tapeIdx);
      var one = LVR.CreateConstInt(LTR.Int16, 1);
      var res = hld.bld.BuildAdd(idx, one);
      hld.bld.BuildStore(res, hld.tapeIdx);

      hld.bld.BuildRetVoid();
      return (fn, fnTy);
    }

    private (LVR fn, LTR fnTy) makeMoveLeft(LLVMHolder hld)
    {
      var fnTy = LTR.CreateFunction(LTR.Void, []);
      var fn = hld.mod.AddFunction("move_left", fnTy);
      var entry = fn.AppendBasicBlock("entry");
      hld.bld.PositionAtEnd(entry);

      var idx = hld.bld.BuildLoad2(hld.tapeIdxTy, hld.tapeIdx);
      var one = LVR.CreateConstInt(LTR.Int16, 1);
      var res = hld.bld.BuildSub(idx, one);
      hld.bld.BuildStore(res, hld.tapeIdx);

      hld.bld.BuildRetVoid();
      return (fn, fnTy);
    }

    private (LVR fn, LTR fnTy) makeAdd(LLVMHolder hld)
    {
      var fnTy = LTR.CreateFunction(LTR.Void, []);
      var fn = hld.mod.AddFunction("add", fnTy);
      var entry = fn.AppendBasicBlock("entry");
      hld.bld.PositionAtEnd(entry);

      var idx = hld.bld.BuildLoad2(hld.tapeIdxTy, hld.tapeIdx);
      var elmPtr = hld.bld.BuildGEP2(hld.tapeTy, hld.tape, [idx]);
      var elm = hld.bld.BuildLoad2(LTR.Int8, elmPtr);
      var one = LVR.CreateConstInt(LTR.Int8, 1);
      var res = hld.bld.BuildAdd(elm, one);
      hld.bld.BuildStore(res, elmPtr);

      hld.bld.BuildRetVoid();
      return (fn, fnTy);
    }

    private (LVR fn, LTR fnTy) makeSub(LLVMHolder hld)
    {
      var fnTy = LTR.CreateFunction(LTR.Void, []);
      var fn = hld.mod.AddFunction("sub", fnTy);
      var entry = fn.AppendBasicBlock("entry");
      hld.bld.PositionAtEnd(entry);

      var idx = hld.bld.BuildLoad2(hld.tapeIdxTy, hld.tapeIdx);
      var elmPtr = hld.bld.BuildGEP2(hld.tapeTy, hld.tape, [idx]);
      var elm = hld.bld.BuildLoad2(LTR.Int8, elmPtr);
      var one = LVR.CreateConstInt(LTR.Int8, 1);
      var res = hld.bld.BuildSub(elm, one);
      hld.bld.BuildStore(res, elmPtr);

      hld.bld.BuildRetVoid();
      return (fn, fnTy);
    }

    private (LVR fn, LTR fnTy) makePrint(LLVMHolder hld)
    {
      var fnTy = LTR.CreateFunction(LTR.Void, []);
      var fn = hld.mod.AddFunction("print", fnTy);
      var entry = fn.AppendBasicBlock("entry");
      hld.bld.PositionAtEnd(entry);

      var idx = hld.bld.BuildLoad2(hld.tapeIdxTy, hld.tapeIdx);
      var tape = hld.bld.BuildGEP2(LTR.Int8, hld.tape, [idx]);
      var val = hld.bld.BuildLoad2(LTR.Int8, tape);
      hld.bld.BuildCall2(hld.printfTy, hld.printfFn, [hld.printfCharFmt, val]);

      hld.bld.BuildRetVoid();
      return (fn, fnTy);
    }

    private (LVR fn, LTR fnTy) makeGetPos(LLVMHolder hld)
    {
      var fnTy = LTR.CreateFunction(LTR.Void, []);
      var fn = hld.mod.AddFunction("get_pos", fnTy);
      var entry = fn.AppendBasicBlock("entry");
      hld.bld.PositionAtEnd(entry);

      var idx = hld.bld.BuildLoad2(hld.tapeIdxTy, hld.tapeIdx);
      hld.bld.BuildCall2(hld.printfTy, hld.printfFn, [hld.printfNumFmt, idx]);

      hld.bld.BuildRetVoid();
      return (fn, fnTy);
    }
  }
}
