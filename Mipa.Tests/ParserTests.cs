namespace Mipa.Tests;

public record UnparsableType : IParsable<UnparsableType>, ISpanParsable<UnparsableType>
{
    public static UnparsableType Parse(string s, IFormatProvider? provider)
    {
        throw new Exception();
    }

    public static bool TryParse(string value, IFormatProvider? formatProvider, out UnparsableType result)
    {
        result = new UnparsableType();
        return false;
    }

    public static UnparsableType Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        throw new Exception();
    }

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out UnparsableType result)
    {
        result = new UnparsableType();
        return false;
    }
}

public class Tests
{
    [Test]
    public void EmptyText()
    {
        var parser = new MipaParser();
        var result = parser.Parse("");
        
        Assert.That(result.Content, Is.EqualTo(""));
        Assert.That(result.Arguments, Has.Length.Zero);
    }
    
    [Test]
    public void BaseText()
    {
        var parser = new MipaParser();
        var result = parser.Parse("hello world");
        
        Assert.That(result.Content, Is.EqualTo("hello world"));
        Assert.That(result.Arguments, Has.Length.Zero);
    }
    
    [Test]
    public void SingleArgument()
    {
        var parser = new MipaParser();
        var result = parser.Parse("hello world key:value");
        
        Assert.That(result.Content, Is.EqualTo("hello world"));
        Assert.That(result.Arguments, Has.Length.EqualTo(1));
        Assert.That(result.GetArgument("key").Span is "value");
    }

    [Test]
    public void SingleArgumentBetweenContent()
    {
        var parser = new MipaParser();
        var result = parser.Parse("hello key:value world");

        Assert.That(result.Content, Is.EqualTo("hello world"));
        Assert.That(result.Arguments, Has.Length.EqualTo(1));
        Assert.That(result.GetArgument("key").Span is "value");
    }

    [Test]
    public void MultipleArguments()
    {
        var parser = new MipaParser();
        var result = parser.Parse("key2:value2 hello world key:value ");
        
        Assert.That(result.Content, Is.EqualTo("hello world"));
        Assert.That(result.Arguments, Has.Length.EqualTo(2));
        Assert.That(result.GetArgument("key").Span is "value");
        Assert.That(result.GetArgument("key2").Span is "value2");
    }
    
    [Test]
    public void MultipleArgumentsWithQuotes()
    {
        var parser = new MipaParser();
        var result = parser.Parse("key2:\"this is value2\" hello world key:\"value\" ");
        
        Assert.That(result.Content, Is.EqualTo("hello world"));
        Assert.That(result.Arguments, Has.Length.EqualTo(2));
        Assert.That(result.GetArgument("key").Span is "value");
        Assert.That(result.GetArgument("key2").Span is "this is value2");
    }

    [Test]
    public void ParseArguments()
    {
        var parser = new MipaParser();
        var result = parser.Parse("date:08/08/2023 hello world key:23");

        Assert.That(result.Content, Is.EqualTo("hello world"));
        Assert.That(result.Arguments, Has.Length.EqualTo(2));
        Assert.That(result.GetArgument<DateTime>("date"), Is.TypeOf<DateTime>());
        Assert.That(result.GetArgument<DateTime>("date"), Is.EqualTo(new DateTime(2023, 8, 8)));
        Assert.That(result.GetArgument<int>("key"), Is.EqualTo(23));
        
        // failure states
        Assert.That(result.GetArgument<UnparsableType>("date"), Is.Null);
        Assert.That(result.GetArgument<UnparsableType>("non-existant-key"), Is.Null);
    }
}