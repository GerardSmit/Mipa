namespace Mipa.Tests;

public record UnparsableType : IParsable<UnparsableType>
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
}

public class Tests
{
    [Test]
    public void EmptyText()
    {
        var parser = new MipaParser();
        var result = parser.Parse("");
        
        Assert.That(result.Content, Is.EqualTo(""));
        Assert.That(result.Arguments, Is.Empty);
    }
    
    [Test]
    public void BaseText()
    {
        var parser = new MipaParser();
        var result = parser.Parse("hello world");
        
        Assert.That(result.Content, Is.EqualTo("hello world"));
        Assert.That(result.Arguments, Is.Empty);
    }
    
    [Test]
    public void SingleArgument()
    {
        var parser = new MipaParser();
        var result = parser.Parse("hello world key:value");
        
        Assert.That(result.Content, Is.EqualTo("hello world"));
        Assert.That(result.Arguments, Has.Count.EqualTo(1));
        Assert.That(result.GetArgument("key"), Is.EqualTo("value"));
    }
    
    [Test]
    public void MultipleArguments()
    {
        var parser = new MipaParser();
        var result = parser.Parse("key2:value2 hello world key:value ");
        
        Assert.That(result.Content, Is.EqualTo("hello world"));
        Assert.That(result.Arguments, Has.Count.EqualTo(2));
        Assert.That(result.GetArgument("key"), Is.EqualTo("value"));
        Assert.That(result.GetArgument("key2"), Is.EqualTo("value2"));
    }
    
    [Test]
    public void MultipleArgumentsWithQuotes()
    {
        var parser = new MipaParser();
        var result = parser.Parse("key2:\"this is value2\" hello world key:\"value\" ");
        
        Assert.That(result.Content, Is.EqualTo("hello world"));
        Assert.That(result.Arguments, Has.Count.EqualTo(2));
        Assert.That(result.GetArgument("key"), Is.EqualTo("value"));
        Assert.That(result.GetArgument("key2"), Is.EqualTo("this is value2"));
    }

    [Test]
    public void ParseArguments()
    {
        var parser = new MipaParser();
        var result = parser.Parse("date:08/08/2023 hello world key:23");

        Assert.That(result.Content, Is.EqualTo("hello world"));
        Assert.That(result.Arguments, Has.Count.EqualTo(2));
        Assert.That(result.GetArgument<DateTime>("date"), Is.TypeOf<DateTime>());
        Assert.That(result.GetArgument<DateTime>("date"), Is.EqualTo(new DateTime(2023, 8, 8)));
        Assert.That(result.GetArgument<int>("key"), Is.EqualTo(23));
        
        // failure states
        Assert.That(result.GetArgument<UnparsableType>("date"), Is.Null);
        Assert.That(result.GetArgument<UnparsableType>("non-existant-key"), Is.Null);
    }
}