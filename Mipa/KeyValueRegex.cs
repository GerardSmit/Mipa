using System.Text.RegularExpressions;

namespace Mipa;

internal partial class KeyValueRegex
{
    [GeneratedRegex("(\\w+):((?:\\\"[^\"]*\\\"|\\S+))", RegexOptions.IgnoreCase, "en-US")]
    internal static partial Regex Get();
}