# StringToExpression
StringToExpression allows you to create methods that take strings and outputs .NET expressions. It is highly configurable allowing you to define your own language with your own syntax.

Two languages are provided out of the box, an `ArithmeticLanguage` for performing algebra and an `ODataFilterLanguage` for parsing OData filter expressions.

[![NuGet version](https://badge.fury.io/nu/StringToExpression.svg)](https://badge.fury.io/nu/StringToExpression)

## Arithmetic
A basic arithmetic language is provided. It can be used as is, or extended with customer function by extending `ArithmeticLanguage`

```csharp
var language = new ArithmeticLanguage();
Expression<Func<decimal>> expressionFunction = language.Parse("(4 - 2) * 5 + 9 / 3");
Func<decimal> function = expressionFunction.compile();

Assert.Equal(13, function());
```


## OData filter
[OData filtering](http://www.odata.org/documentation/odata-version-2-0/uri-conventions/#FilterSystemQueryOption) is a nice way to pass generic filtering requirements into a WebAPI, although parsing the the filter expression can be cumbersome. StringToExpression can be used as a lightweight parser

```csharp
public async Task<IHttpActionResult> GetDoohickies([FromUri(Name = "$filter")] string filter = "name eq 'discount' and rating gt 18")
{
  var language = new ODataFilterLanguage()
  Expression<Func<Doohicky, bool>> predicate = language.Parse<Doohickey>(filter);
  
  //can either pass this expression into either IQueryable or IEnumerable where clauses
  return await DataContext.Doohickies.Where(predicte).ToListAsync();
}

```

`StringToExpression` has the advantage of being configurable; if the OData parser doesnt support methods you want, (or it supports methods you dont want) it is very easy to extend `ODataFilterLanguage` and modify the configuration

### Supported Operations

| Operators            | Name                  | Example                            |
|----------------------|-----------------------|------------------------------------|
| eq                   | Equal                 | City eq 'Redmond'                  |
| ne                   | Not equal             | City ne 'London'                   |
| gt                   | Greater than          | Price gt 20                        |
| ge                   | Greater than or equal | Price ge 10                        |
| lt                   | Less than             | Price lt 20                        |
| le                   | Less than or equal    | Price le 100                       |
| and                  | Logical and           | Price le 200 and Price gt 3.5      |
| or                   | Logical or            | Price le 3.5 or Price gt 200       |
| not                  | Logical negation      | not endswith(Description,'milk')   |
| add                  | Addition              | Price add 5 gt 10                  |
| sub                  | Subtraction           | Price sub 5 gt 10                  |
| mul                  | Multiplication        | Price mul 2 gt 2000                |
| div                  | Division              | Price div 2 gt 4                   |
| mod                  | Modulo                | Price mod 2 eq 0                   |
| ( )                  | Precedence grouping   | (Price sub 5) gt 10                |
| /                    | Property access       | Address/City eq 'Redmond'          |

### Supported Functions

| String Functions                                          | Example                                           |
|-----------------------------------------------------------|---------------------------------------------------|
| bool substringof(string po, string p1)                    | substringof('day', 'Monday') eq true              |
| bool endswith(string p0, string p1)                       | endswith('Monday', 'day') eq true                 |
| bool startswith(string p0, string p1)                     | startswith('Monday', 'Mon') eq true               |
| int length(string p0)                                     | length('Monday') eq 6 | Price ge 10               |
| int indexof(string p0, string p1)                         | indexof('Monday', 'n') eq 2                       |
| string replace(string p0, string find, string replace)    | replace('Monday', 'Mon', 'Satur') eq 'Saturday'   |
| string substring(string p0, int pos)                      | substring('Monday', 3) eq 'day'                   |
| string substring(string p0, int pos, int length)          | substring('Monday', 3, 2) eq 'da'                 |
| string tolower(string p0)                                 | tolower('Monday') eq 'monday'                     |
| string toupper(string p0)                                 | toupper('Monday') eq 'MONDAY'                     |
| string trim(string p0)                                    | trim('  Monday  ') eq 'Monday'                    |
| string concat(string p0, string p1)                       | concat('Mon', 'day') eq 'Monday'                  |

| Date Functions                                            | Example                                           |
|-----------------------------------------------------------|---------------------------------------------------|
| int day(DateTime p0)	                                    | day(datetime'2000-01-02T03:04:05') eq 2           |
| int hour(DateTime p0)	                                    | hour(datetime'2000-01-02T03:04:05') eq 3          |
| int minute(DateTime p0)                                   | minute(datetime'2000-01-02T03:04:05') eq 4        |
| int month(DateTime p0)                                    | month(datetime'2000-01-02T03:04:05') eq 1         |
| int second(DateTime p0)                                   | second(datetime'2000-01-02T03:04:05') eq 5        |
| int year(DateTime p0)                                     | year(datetime'2000-01-02T03:04:05') eq 2000       |

| Math Functions                                            | Example                                           |
|-----------------------------------------------------------|---------------------------------------------------|
| double round(double p0)	                                | round(10.4) eq 10 <br/> round(10.6) eq 11 </br> round(10.5) eq 10 <br/> round(11.5) eq 12
| double floor(double p0)                                   | floor(10.6) eq 10
| decimal floor(decimal p0)                                 | month(datetime'2000-01-02T03:04:05') eq 1
| double ceiling(double p0)                                 | ceiling(10.4) eq 11



## Custom languages
Languages are defined by a set of `GrammerDefintions`. These define both how the string is broken up into tokens as well as the behaviour of each token. There are many subclasses of `GrammerDefinition` that makes implementing standard language features very easy. 

An example of a very simple arithmetic language is as follows

```csharp
ListDelimiterDefinition delimeter;
BracketOpenDefinition openBracket, sqrt;
language = new Language(new [] {
    new OperandDefinition(
        name:"DECIMAL",
        regex: @"\-?\d+(\.\d+)?",
        expressionBuilder: x => Expression.Constant(decimal.Parse(x))),
    new BinaryOperatorDefinition(
        name:"ADD",
        regex: @"\+",
        orderOfPrecedence: 2,
        expressionBuilder: (left,right) => Expression.Add(left, right)),
    new BinaryOperatorDefinition(
        name:"SUB",
        regex: @"\-",
        orderOfPrecedence: 2,
        expressionBuilder: (left,right) => Expression.Subtract(left, right)),
    new BinaryOperatorDefinition(
        name:"MUL",
        regex: @"\*",
        orderOfPrecedence: 1, //multiply should be done before add/subtract
        expressionBuilder: (left,right) => Expression.Multiply(left, right)),
    new BinaryOperatorDefinition(
        name:"DIV",
        regex: @"\/",
        orderOfPrecedence: 1, //division should be done before add/subtract
        expressionBuilder: (left,right) => Expression.Divide(left, right)),
    sqrt = new FunctionCallDefinition(
        name:"FN_SQRT",
        regex: @"sqrt\(",
        argumentTypes: new[] {typeof(double) },
        expressionBuilder: (parameters) => {
            return Expression.Call(
                null,
                method:typeof(Math).GetMethod("Sqrt"),
                arguments: new [] { parameters[0] });
        }),
    openBracket = new BracketOpenDefinition(
        name: "OPEN_BRACKET",
        regex: @"\("),
    delimeter = new ListDelimiterDefinition(
        name: "COMMA",
        regex: ","),
    new BracketCloseDefinition(
        name: "CLOSE_BRACKET",
        regex: @"\)",
        bracketOpenDefinitions: new[] { openBracket, sqrt },
        listDelimeterDefinition: delimeter)
    new GrammerDefinition(name: "WHITESPACE", regex: @"\s+", ignore: true) //we dont want to process whitespace
  });
```
Some of the out of the box grammer defintions are detailed below

Name | Description | Properties
---|---|---
`GrammerDefintion`|Base class for all defintions. Does not perform any functionality during the parsing | <ul><li>name - A name for this rule</li><li>regex - the regular expression that will match for this token</li></ul>
`OperandDefinition`| Defines the smallest atomic piece in your language, used to represent items like numbers or strings | <ul><li>expressionBuilder - a function that when given the string matched from the regex it produces a .NET expression (usually a `ConstantExpression`)</li></ul>
`BinaryOperatorDefintion` | An operation that takes parameters from the left and right of it. Often represents arithmetic operaitons (`+`, `-`, `*`, `/`) or equality checks (`==`, `!=`, `<` `>`) or boolean logic (`and`, `or`) | <ul><li>orderOfPrecedence - determines when this function should run, lower numbers get run before higher numbers (allowing defining BEDMAS rules)</li><li>expressionBuilder - function that takes in two Expression (the left and right of the operator), and outputs a new expression combining them.</li></ul>
`UnaryOperator` | An operation that takes a single parameter, used for operations such as `not` | <ul><li>orderOfPrecedence - determines when this function should run, lower numbers get run before higher numbers</li><li>parameterPosition - whether the operand is to the left or right of the operator</li></ul>
`BracketOpenDefinition` | Defines an open bracket, functionally does not do much unless paried with a `BracketCloseDefinition` |
`ListDelimeterDefinition` | The seperator to use to denote lists within bracktes (a `,` in most languages) functionally does not do much unless paired with a `BracketCloseDefinition` | 
`BracketCloseDefinition` | The expression between the brackets is evaluated first | <ul><li>bracketOpenDefinitions - list of definitions that would be treated as a start to the bracketing</li><li>listDelimeterDefinition - definition of the `ListDelimeterDefinition`</li></ul>
`FunctionCallDefinition` | Defines a function that takes in a list of operands. Also acts as a bracket `BracketOpenDefinition` definition | <ul><li>argumentTypes - list of types that define the types expected and the number of arguments expected</li><li>expressionBuilder - takes an array of expressions and output a single expression</li></ul>

If your language is more complicated than the provided `GrammerDefinitions` you are able to define your own by extending `GrammerDefintion`. You best read the [Nuts and bolts](#Nuts-and-bolts) section to determine the best way to implement your definition.

## Error handling

All parsing exceptions extend `ParseException`. A `ParseException` will contain both a readable message and a `StringSegment` that represents what token(s) in the original input string caused the error.

The `StringSegment` allows pinpointing of issues such as where operands are missing, which function has too many parameters or where the unexpected character is. It provides a useful feedback for wherever you are getting your original strings from.

## Nuts and bolts
Under the hood `StringToExpression` implements a [shunting-yard alogrithm](https://en.wikipedia.org/wiki/Shunting-yard_algorithm). 

The internal parsing state contains a `Stack<Operand>` and a `Stack<Operator>` that are built up during the parsing
* `Operand` - Represent a .NET expression. This may be a simple `ConstantExpression`, or the root of a complicated Tree of `BinaryExpressions`
* `Operator` - Is a function that can be run. Generally these functions when run will consume one or more operands and produce one operand. Such that run operators reduces the number of operands on the stack.

The parsing is done in roughly three steps

1. *Tokenize* - The string is parsed through a tokenizer which uses the regular expressions defined in the `GrammerDefinitions` to break the strings into `Tokens`. A `Token` knows the `GrammerDefinition` that created it and the string value it represents.

2. *Apply `GrammerDefinitions`* - All `GrammerDefinition` has an `void Apply(Token token, ParseState state)` method. We first read all the `Tokens` in sequentially and run each `GrammerDefinition` `Apply` method. The apply method can make any modifications to the state it wants, this can range from something simple like pushing an Operand on to the stack, to something more complicated like executing operands.

3. *Execute Operators* -Once all the tokens are Applied we will start poping Operators off the stack and executing them. When an `Operator` executes its generally expected that it will consume one or more `Operands` and create one `Operand`. This way by the time we apply all the operators we should only have a single `Operand` on the stack, that is our result.

To customize you can make your own `GrammerDefinition` and implment the `Apply` method to meet your purposes.

## How to use complex object?

To perform an operation accessing a property of a complex object, it is necessary to use a slash
* `/` - Accesses the value of the child property when having a complex object

Example: 
```csharp
//Concrete class
public class Address {

  public string Street { get; set; }

  public string City { get; set; }

  public string Country { get; set; }

  public string ZIPCode { get; set; }

}

//Complex class
public class Custumer {

   public string Name { get; set; }

   public string Email { get; set; }

   public string Phone { get; set; }

   public Address Address;
  
}

```
*  When filtering the customer, this expression can be used to access the data of the "Address" of its child
```csharp
   var filter = new ODataFilterLanguage().Parse<Custumer>("Adress/City ne null and Adress/ZipCode eq '000990-00'");
```