namespace Parsing
{
  public struct Token<T>
    where T : ITokenType
  {
    public readonly T Type;
    public readonly int TextPos;
    public Token<T>[]? SubTokens { get; set; }
    public int Length => Type.GetWord().Length;

    public Token(T type, int pos)
    {
      this.Type = type;
      this.TextPos = pos;
    }
  }
}
