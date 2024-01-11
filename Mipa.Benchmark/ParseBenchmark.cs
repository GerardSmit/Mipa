using BenchmarkDotNet.Attributes;

namespace Mipa.Benchmark;

[MemoryDiagnoser]
public class ParseBenchmark
{
    private static readonly IMipaParser Parser = new MipaParser();

    [Benchmark]
    public void ParseEmpty()
    {
        Parser.Parse("");
    }

    [Benchmark]
    public void ParseHelloWorld()
    {
        Parser.Parse("hello world");
    }

    [Benchmark]
    public void ParseSingleArgument()
    {
        Parser.Parse("hello world key:value");
    }

    [Benchmark]
    public void ParseMultipleArguments()
    {
        Parser.Parse("key2:value2 hello world key:value");
    }

    [Benchmark]
    public void ParseMultipleArgumentsWithQuotes()
    {
        Parser.Parse("key2:\"this is value2\" hello world key:\"value\"");
    }
}