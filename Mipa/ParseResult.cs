namespace Mipa;

public record ParseResult
{
    /// <summary>
    /// Remainder of the content that is unparsed.
    /// </summary>
    public string Content { get; init; }
    public IReadOnlyList<KeyValuePair<string, string>> Arguments { get; init; }
    
    public string? GetArgument(string key)
    {
        return Arguments.FirstOrDefault(x => x.Key == key).Value;
    }
    public T? GetArgument<T>(string key) where T : IParsable<T>
    {
        var argument = GetArgument(key);
        if (argument == null)
        {
            return default;
        }

        if (!T.TryParse(argument, null, out var result))
        {
            return default;
        }

        return result;
    }
}