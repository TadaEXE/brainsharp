using Parsing;

namespace BrainSharp
{
  public static class Syntax
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
    }

    public static Type EnumType() => typeof(TokenEnum);

    public static Type TokenTypeType() => typeof(TokenType<TokenEnum>);

    public static TokenType<TokenEnum>[] TokenTypes =
    [
      new(TokenEnum.Left, "<"),
      new(TokenEnum.Right, ">"),
      new(TokenEnum.Increment, "+"),
      new(TokenEnum.Decrement, "-"),
      new(TokenEnum.LoopStart, "["),
      new(TokenEnum.LoopEnd, "]"),
      new(TokenEnum.GetInput, ","),
      new(TokenEnum.Print, "."),
    ];
  }
}
