using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Mipa;

public class MipaParser : IMipaParser
{
    public ParseResult Parse(string content)
    {
        // Match all key value pairs
        var memory = content.AsMemory();
        var maxArguments = memory.Span.Count(':');

        if (maxArguments == 0)
        {
            return new ParseResult
            {
                Arguments = Array.Empty<KeyValuePair<ReadOnlyMemory<char>, ReadOnlyMemory<char>>>(),
                Content = content,
            };
        }

        var arguments = new KeyValuePair<ReadOnlyMemory<char>, ReadOnlyMemory<char>>[maxArguments];

        Span<ArgumentPosition> positions = stackalloc ArgumentPosition[maxArguments];
        var positionIndex = 0;
        var contentLength = content.Length;

        while (!memory.IsEmpty)
        {
            // Find the next key value pair
            var index = memory.Span.IndexOf(':');

            if (index == -1)
            {
                break;
            }

            if (index == 0 || !char.IsAsciiLetterOrDigit(memory.Span[index - 1]))
            {
                memory = memory.Slice(index + 1);
                continue;
            }

            // Get the value
            var valueSpan = memory.Slice(index + 1);

            if (valueSpan.Length == 0)
            {
                break;
            }

            var value = GetValue(valueSpan, out var length);

            // Get the key
            var key = GetKey(memory, index - 1);

            // Add the argument
            arguments[positionIndex] = new KeyValuePair<ReadOnlyMemory<char>, ReadOnlyMemory<char>>(key, value);

            // Trim whitespace before the key
            var start = index - key.Length;

            if (start > 0 && char.IsWhiteSpace(memory.Span[start - 1]))
            {
                start--;
            }

            // Trim whitespace after the value
            var end = index + 1 + length;

            if (end < memory.Length && char.IsWhiteSpace(memory.Span[end]))
            {
                end++;
            }

            // Store the position of the argument in the content
            var argumentLength = end - start;
            positions[positionIndex] = new ArgumentPosition(start, argumentLength);
            contentLength -= argumentLength;

            memory = memory.Slice(end);
            positionIndex++;
        }

        return new ParseResult
        {
            Content = CreateContent(content, contentLength, positions.Slice(0, positionIndex)),
            Arguments = arguments.AsMemory(0, positionIndex),
        };
    }

    private static unsafe string CreateContent(string content, int length, ReadOnlySpan<ArgumentPosition> positions)
    {
        if (positions.Length == 0)
        {
            return content;
        }

        var state = new State
        {
            Content = content,
            Positions = (ArgumentPosition*)Unsafe.AsPointer(ref MemoryMarshal.GetReference(positions)),
            PositionLength = positions.Length,
        };

        return string.Create(length, state, static (span, state) =>
        {
            var chars = state.Content.AsSpan();
            var positions = new ReadOnlySpan<ArgumentPosition>(state.Positions, state.PositionLength);

            foreach (ref readonly var position in positions)
            {
                if (position.Start > 0)
                {
                    chars.Slice(0, position.Start).CopyTo(span);
                    span = span.Slice(position.Start);
                }

                chars = chars.Slice(position.Start + position.Length);
            }

            if (!chars.IsEmpty)
            {
                chars.CopyTo(span);
            }
        });
    }

    private unsafe struct State
    {
        public required string Content;
        public required ArgumentPosition* Positions;
        public required int PositionLength;
    }

    private record struct ArgumentPosition(int Start, int Length);

    private static ReadOnlyMemory<char> GetKey(in ReadOnlyMemory<char> memory, int start)
    {
        var index = start - 1;

        while (index >= 0 && char.IsAsciiLetterOrDigit(memory.Span[index]))
        {
            index--;
        }

        return memory.Slice(index + 1, start - index);
    }

    private static ReadOnlyMemory<char> GetValue(in ReadOnlyMemory<char> memory, out int length)
    {
        var quote = memory.Span[0] == '"';

        int index;

        if (quote)
        {
            var search = memory.Slice(1);
            index = search.Span.IndexOf('"');
            length = index + 2;
            return search.Slice(0, index);
        }

        index = memory.Span.IndexOf(' ');

        if (index == -1)
        {
            index = memory.Length;
        }

        length = index;
        return memory.Slice(0, index);
    }
}
