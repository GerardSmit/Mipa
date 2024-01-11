using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Mipa;

public readonly struct ParseResult
{
    /// <summary>
    /// Remainder of the content that is unparsed.
    /// </summary>
    public string Content { get; init; }
    public ReadOnlyMemory<KeyValuePair<ReadOnlyMemory<char>, ReadOnlyMemory<char>>> Arguments { get; init; }
    
    public ReadOnlyMemory<char> GetArgument(string key)
    {
        foreach (var argument in Arguments.Span)
        {
            if (argument.Key.Span.SequenceEqual(key))
            {
                return argument.Value;
            }
        }

        return default;
    }

    public T? GetArgument<T>(string key) where T : ISpanParsable<T>
    {
        var argument = GetArgument(key);
        if (argument.IsEmpty)
        {
            return default;
        }

        if (!T.TryParse(argument.Span, null, out var result))
        {
            return default;
        }

        return result;
    }
}