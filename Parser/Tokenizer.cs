namespace Parsing
{
  public class Tokenizer<T>
    where T : ITokenType
  {
    private T[] acceptedTokens;
    private Dictionary<string, T> wordLut;

    public Tokenizer(T[] acceptedTokens)
    {
      this.acceptedTokens = acceptedTokens;
      wordLut = MakeWordLut();
    }

    public Token<T>[] Tokenize(string text)
    {
      var tokens = new List<Token<T>>();
      var textPos = 0;
      string curSubText = text;
      while (textPos != text.Length)
      {
        if (GetNextMatch(curSubText) is Token<T> tok)
        {
          tokens.Append(tok);
          curSubText = curSubText.Substring(tok.TextPos + tok.Length);
        }
      }
      return tokens.ToArray();
    }

    private Dictionary<string, T> MakeWordLut()
    {
      var lut = new Dictionary<string, T>();
      foreach (var token in acceptedTokens)
      {
        lut[token.GetWord()] = token;
      }
      return lut;
    }

    private Token<T>? GetNextMatch(string text)
    {
      int low = text.Length + 1;
      T? tt = default(T);
      foreach (var word in wordLut.Keys)
      {
        var tmp = text.IndexOf(word);
        if (tmp >= 0 && tmp < low)
        {
          low = tmp;
          tt = wordLut[word];
        }
      }

      if (low != text.Length + 1 && tt is not null)
      {
        return new Token<T>(tt, low);
      }

      return null;
    }
  }
}
