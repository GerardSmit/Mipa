using System.Text;

namespace Mipa;

public class MipaParser : IMipaParser
{
    public ParseResult Parse(string content)
    {
        if(string.IsNullOrWhiteSpace(content))
        {
            return new ParseResult
            {
                Arguments = [],
                Content = content,
            };
        }

        // match all key value pairs
        var matches = KeyValueRegex.Get().Matches(content);
        var arguments = new List<KeyValuePair<string, string>>();
        var builder = new StringBuilder(content);
        for (var i = matches.Count - 1; i >= 0; i--)
        {
            var match = matches[i];
            var key = match.Groups[1].Value;
            var value = match.Groups[2].Value.Trim('"');
            arguments.Add(new KeyValuePair<string, string>(key, value));
            builder.Remove(match.Index, match.Length);
        }
        
        return new ParseResult
        {
            Content = builder.ToString().Trim(),
            Arguments = arguments,
        };
    }
}
