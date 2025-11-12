namespace Parsing
{
  public interface ITokenType
  {
    public Type GetEnumType();
    public Enum GetEnumValue();
    public string GetWord();
  }

  public struct TokenType<T> : ITokenType
    where T : Enum
  {
    private readonly string word;
    private readonly T value;

    public TokenType(T value, string word)
    {
      this.word = word;
      this.value = value;
    }

    public string GetWord() => word;

    public Type GetEnumType() => typeof(T);

    public Enum GetEnumValue() => value;

    public int? GetNextMatch(string text)
    {
      var idx = text.IndexOf(word);
      return idx >= 0 ? (int)idx : null;
    }
  }
}
