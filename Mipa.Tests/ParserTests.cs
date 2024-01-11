namespace Mipa.Tests;

public class Tests
{
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
}