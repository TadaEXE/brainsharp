using LLVMSharp.Interop;
using MVP;

class Program
{
  static void Main()
  {
    string[] args = Environment.GetCommandLineArgs();
    string? inputFile = args.ElementAtOrDefault(1);
    string? outputFile = args.ElementAtOrDefault(2);
    if (inputFile is null)
    {
      Console.WriteLine("Usage: brainsharp [INPUT] (OUTPUT)");
      return;
    }
    if (outputFile is null)
    {
      outputFile = inputFile + ".ll";
    }
    Console.WriteLine($"Reading {inputFile}");
    ParseFile(inputFile);

    // var context = LLVMContextRef.Create();
    // var module = LLVMModuleRef.CreateWithName("brainsharp");
    // var builder = LLVMBuilderRef.Create(context);
    //
    // var i32 = LLVMTypeRef.Int32;
    // var mainTy = LLVMTypeRef.CreateFunction(i32, Array.Empty<LLVMTypeRef>()); // no params
    // var mainFn = module.AddFunction("main", mainTy);
    // var entry = mainFn.AppendBasicBlock("entry");
    //
    // builder.PositionAtEnd(entry);
    // builder.BuildRet(LLVMValueRef.CreateConstInt(i32, 0));
    //
    // module.PrintToFile(outputFile);
  }

  static void ParseFile(string path)
  {
    var comp = new Compiler();
    var nodes = comp.ParseFile(path);

    // foreach (var n in nodes)
    // {
    //   Console.WriteLine($"{n.Value?.Word} at {n.TextPos}");
    // }

    var ast = comp.BuildAst(nodes);
    // Console.WriteLine(ast.ToString());

    comp.CompileAst(ast, path + ".ll");
  }
}
