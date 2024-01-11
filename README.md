# Mipa 

Mini Parser for text that contains attributed values. Originally designed to work similarily to Discord slash command arguments, but without having to set them as actual variables. Though its use-cases may vary to cases like terminal apps.

## Install

```sh
$ dotnet add package Mipa
```

## How to use it

```csharp
var parser = new MipaParser();

var result = parser.Parse("hello! foo:bar");

result.Content // "hello!"
result.Arguments // [{"foo": "bar"}]
```

## Parsing Input

Any parsed text in Mipa comes with a basic set of parsing for arguments. Any type with the IParsable interface implemented works with Mipa.

```csharp
var result = parser.Parse("welcome to the chat! date:08/18/2018");
result.GetArgument<DateTime>("date") // 8/18/2018 12:00:00 AM
```

## Defining your own argument parsers

The recommended way to implemented argument parsers is to implement IParsable into your data structure. Mipa will pass the raw text into TryParse, and will return a value if the parser succeeds in parsing it. 