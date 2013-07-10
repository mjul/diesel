# Diesel is a DSL toolkit for .NET code generation for DDD

Diesel provides a declarative language for generating code for your .NET projects.

* __Value Types__ - strong types for value types, e.g. struct EmployeeNumber instead of int. 
* __Commands__ - generates classes for the Command DTOs.
* __Domain Events__ - generates classes for the Domain Event DTOs.
* __DTOs__ - generate other Data Transfer Objects: classes and enums tagged for serialization.
* __Application Services__ - generate an interface for all the commands accepted by the service.

Planned features include declarations for Value Objects, Aggregate Roots and Projections.


## Example

Create a model in the Diesel DSL language:

```
    (namespace Employees
        (defvaluetype EmployeeNumber)
        (defvaluetype EmployeeName (string FirstName, string LastName))
        (defdto Name (string FirstName, string LastName))
        (defapplicationService ImportService
            (defcommand ImportEmployee (int EmployeeNumber, Name Name))
        (defdomainevent EmployeeImported (Guid Id, int EmployeeNumber, Name Name)))
```

Use the Visual Studio T4 template to automatically generate the code (see below) or compile it the model source 
with a single method call: `DieselCompiler.Compile(modelSource)`. 

Now use it

```csharp

    // value types automatically get a constructor setting their value
    var founderNumber = new EmployeeNumber(1);
    var lateHireNumber = new EmployeeNumber(100);
            
    // value types can have multiple fields 
    var name = new EmployeeName("Martin", "Jul");

    // You get free value equality and equality operators 
    var isFounder = (employee.EmployeeNumber == founderNumber);
         
    // Commands are classes and have a constructor that assigns all properties
    var command = new ImportEmployeeCommand(1, new Name("Martin", "Jul"));
           
    // The corresponding properties are automatically added to the classes:
    var firstName = command.Name.FirstName;
     
```

# Benefits

Diesel saves you from writing a lot of trivial code that no humans should write by hand,
so you can focus on building the essential parts of your application:

* Get your Value Objects at huge discounts - no more excuses for [Primitive Obsession](http://c2.com/cgi/wiki?PrimitiveObsession)
* DTOs are on sale, too: all you need to do is writing the constructor signature.
* Get consistent serializability of Commands, Domain Events and other DTOs.

  
# Defining Simple Value Types

    (defvaluetype <typename>)
    (defvaluetype <typename> <type>)
    (defvaluetype <typename> <properties>)

When you compile the `defvaluetype`, Diesel generates .NET code in your language, 
emitting a  value type (`struct` in C#) with the declared properties, 
a constructor to set them and equality operator overloads to give it full value equality semantics.

In the first two cases, where properties are not declared the property name is always `Value`.
The type is optional, it defaults to Int32.

If multiple properties are desired, the list of properties and their types must be declared.


## Examples

    (defvaluetype EmployeeNumber)
    (defvaluetype Amount Decimal)
    (defvaluetype EmployeeName (string First, string Last))

The single-property declaration can also be written in the property list format, i.e. the `Amount` 
type above can also be declared as `(defvaluetype Amount (Decimal Value))`.

The code generation is straight-forward, for example, the declaration 
`(defvaluetype EmployeeNumber)` yields a type with value semantics and
equality operator overloads like this:

```csharp

    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerDisplayAttribute("{Value}")]
    public partial struct EmployeeNumber : System.IEquatable<EmployeeNumber> {
        
        private int _value;
        
        public EmployeeNumber(int value) {
            this._value = value;
        }
        
        public int Value {
            get {
                return this._value;
            }
        }
        
        public static bool operator ==(EmployeeNumber left, EmployeeNumber right) {
            return left.Equals(right);
        }
        
        public static bool operator !=(EmployeeNumber left, EmployeeNumber right) {
            return (false == left.Equals(right));
        }
        
        public override int GetHashCode() {
            return (0 + this.Value.GetHashCode());
        }
        
        public bool Equals(EmployeeNumber other) {
            return (true 
                        && (this.Value == other.Value));
        }
        
        public override bool Equals(object obj) {
            if (object.ReferenceEquals(null, obj)) {
                return false;
            }
            return (typeof(EmployeeNumber).IsAssignableFrom(obj.GetType()) && this.Equals(((EmployeeNumber)(obj))));
        }
    }

```

# Defining Commands

    (defcommand <typename> <properties>)

This defines a class representing the Command, the properties are assigned via a constructor
and equals and equality operators are implemented with value semantics.

Commands are DTOs (Data Transfer Objects), so they are also decorated with attributes
to allow them to be serializable with the BinarySerializer and the DataContractSerializer.

Command can be generated with base types, see `defconventions` for a description of 
how to configure this. 

Do note, however, that since Commands are contracts it is generally best to declare base
 __interfaces__ only so that the risk of breaking contracts by modifying a base is 
minimal.


## Example

    (defcommand ImportEmployee (int EmployeeNumber, string FirstName, string LastName, int? SourceId))

This generates a class with properties `EmployeeNumber`, `FirstName` and `LastName` and `SourceId`.
Nullable types are supported in properties with C# short syntax, i.e. "int?" denotes a nullable Int32.


## Adding Base Classes or Interfaces to Domain Events 

The code generator reads conventions from the optional `defconventions` declaration
at the top of the Diesel source file.

You can use this to add base types to the generated Commands. 
For example, to have all Commands derive from the `GreatApp.ICommand` interface, 
just add this declaration:

    (defconventions :commands {:inherit [GreatApp.IDomainEvent]})

Note that since Commands are contracts it is generally best to 
declare base __interfaces__ only so that the risk of breaking contracts by modifying a base is 
minimal.

See the section on defining conventions for a full description of `defconventions`.





# Defining Domain Events

    (defdomainevent <typename> <properties>)

This defines a class representing the Domain Event, a constructor to assign its properties
and equals and equality operators are implemented with value semantics.

Like Commands, Domain Events are DTOs, so they are also decorated with attributes
to allow them to be serializable with the BinarySerializer and the DataContractSerializer.

## Example

    (defdomainevent EmployeeImported (Guid Id, int EmployeeNumber, 
                                      string FirstName, string LastName, int? SourceId))

This generates a class with properties `Id`, `EmployeeNumber`, `FirstName` and `LastName` and `SourceId`.


## Adding Base Classes or Interfaces to Domain Events 

The code generator reads conventions from the optional `defconventions` declaration
at the top of the Diesel source file.

You can use this to add base types to the generated Domain Events. 
For example, to have all Domain Events derive from the `GreatApp.IDomainEvent` interface, 
just add this declaration:

    (defconventions :domainevents {:inherit [GreatApp.IDomainEvent]})

Note that since Domain Events are contracts it is generally best to 
declare base __interfaces__ only so that the risk of breaking contracts by modifying a base is 
minimal.

See the section on defining conventions for a full description of `defconventions`.


# Defining Data Transfer Objects (DTOs)

    (defdto <typename> <properties>)

This defines a class representing the DTO with the specified properties.
It adds a constructor to set all properties.
Equals and equality operators are implemented with value semantics.

Being a DTO (Data Transfer Objects), the type is decorated with attributes
to make it serializable with the BinarySerializer and the DataContractSerializer.

The properties are specified in the C# constructor parameter syntax. 

## Example

    (defdto Name (string FirstName, string LastName))

This generates a class `Name` with properties `FirstName` and `LastName`.


# Defining Enum DTOs

    (defenum <typename> <values>)

This defines an enum with the specified values.

Being a DTO (Data Transfer Objects), the type is decorated with attributes
to make it serializable with the BinarySerializer and the DataContractSerializer.

## Example

    (defenum Gender [Female Male])

This generates an enum `Gender` with values `Female` and `Male`
and the corresponding DataContract attributes.


# Defining Application Services

    (defapplicationservice <name> <commands+>)

This defines an Application Service of the given name. 

The code generator emits an interface with Execute overloads for each of the commands
defined in the scope of the application service.

### Example

Given the following declaration

    (defapplicationservice ImportService
        (defcommand ImportEmployee (Guid CommandId, int EmployeeNumber, string FirstName, string LastName, int? SourceId))
        (defcommand ImportConsultant (string FirstName, string LastName, string Company)))

This declares the `ImportEmployee` class as defined by the nested `defcommand` and the following
interface for the service:

```csharp
    public partial interface IImportService {
         void Execute(ImportEmployee command);
         void Execute(ImportConsultant command);
    }
```

# Defining Namespaces

    (namespace <name> <typedeclarations*>)

A model consists of one or more namespace declarations. Namespaces work like in .NET, controlling 
the .NET namespace of the types declared inside.

## Example

```
    (namespace Employees
        (defvaluetype EmployeeNumber)
        (defapplicationService ImportService
            (defcommand ImportEmployee (int EmployeeNumber, string FirstName, string LastName))
        (defdomainevent EmployeeImported (Guid Id, int EmployeeNumber, string FirstName, string LastName)))
```

This declares types `Employees.EmployeeNumber`, `Employees.IImportService`, `Employees.ImportEmployee`, 
and `Employees.EmployeeImported`.


# Defining Conventions for Code-Generation

    (defconventions :domainevents {:inherit <list-of-base-types>}
                    :commands {:inherit <list-of-base-types>})

The code-generation conventions can be controlled through the `defconventions` declaration.
For now, it only controls the list of interfaces and base classes for the Domain Events and Commands.

The declaration must be placed first in the source file. The `:domainevents` and `:commands`
declarations inside are both optional and can be in any order, but they can not occur more than
once.

## Example

    (defconventions :domainevents {:inherit [Test.Diesel.IDomainEvent]}
                    :commands {:inherit [Test.Diesel.ICommand]})

This causes the code generator to add the `Test.Diesel.IDomainEvent` interface 
as a base on all Domain Events, and the `Test.Diesel.ICommand` interface to all Commands.


## Example: Adding interfaces to Domain Events

    (defconventions :domainevents {:inherit [Test.Diesel.IDomainEvent]})

This adds the `Test.Diesel.IDomainEvent` as a base on all generated Domain Events.

## Example: Adding interfaces to Commands

    (defconventions :commands {:inherit [Test.Diesel.ICommand]})

This adds the `Test.Diesel.ICommand` as a base on all generated Domain Events.


# Comments

Single-line comments are supported. They begin with one (or more) semicolons and 
continue to the end of the line.

Comments are allowed between the high-level syntactic elements, such as namespaces,
services, convetions and type declarations such as DTOs, enums, Domain Events, Services and Commands.

Comments are generally not allowed _inside_ the elements, _e.g._ between parameters in a parameter declaration.
The reason for this is that the inside is typically a C# syntax property list, which has a different 
comment syntax than Diesel.

## Example

```
    ;; Declare Employees namespace
    (namespace Employees
        ;; This a comment
        (defvaluetype EmployeeNumber)
        (defapplicationService ImportService
            ;; Comments can also be nested
            (defcommand ImportEmployee (int EmployeeNumber, string FirstName, string LastName))
        (defdomainevent EmployeeImported (Guid Id, int EmployeeNumber, string FirstName, string LastName)))
```


# Using the T4 Template

The Test project contains a Visual Studio T4 template (.tt file) under Generated.
It can be used as a template for adding code generation to your own project.
It depends on the Diesel compiler and its dependencies being available in binary form 
in the project (assemblies).

To use it, you just need to add two files to your project: the model generator, and the 
model source code in the Diesel language.

Here is the `GenerateModel.tt`  T4 template from one project, you need to adapt the name 
of your model file `MetaModel.diesel` and the absolute paths to `Sprache` and `Diesel`.
After this, you just "Run Custom Tool" on the T4 file to regenerate your model from the 
Diesel specification in `MetaModel.diesel`.

    <#@ template debug="false" hostspecific="true" language="C#" #>
    <#@ assembly name="System.Core" #>
    <#@ import namespace="System.IO" #>
    <#@ import namespace="System.Linq" #>
    <#@ import namespace="System.Text" #>
    <#@ import namespace="System.Collections.Generic" #>
    <#@ assembly name="EnvDTE" #>
    <#@ import namespace="EnvDTE" #>
    <#@ assembly name="$(SolutionDir)Packages\Sprache.1.10.0.28\lib\net40\Sprache.dll" #>
    <#@ assembly name="$(SolutionDir)Packages\Diesel.1.2.0.0\lib\net45\Diesel.dll" #>
    <#@ import namespace="Diesel" #>
    <#@ output extension=".cs" #>
    <# 
        var model = this.Host.ResolvePath("MetaModel.diesel");
        var source = File.ReadAllText(model);
        var output = DieselCompiler.Compile(source);
    #>
    <#= output #>

# NuGet Package

The library is available on [NuGet](https://nuget.org/packages/Diesel/) under the package name "Diesel".

To install: 

        PM> Install-Package Diesel    

# License
Copyright (C)2013 Martin Jul (www.mjul.com)

Distributed under the MIT License. See the LICENSE.txt file for details.

## Dependencies

### Sprache Parser Combinator
This library uses the amazing [Sprache](https://github.com/sprache/Sprache) parser combinator to easily 
parse the model language. It rocks! 

Sprache has the following license:

    The MIT License

    Copyright (c) 2011 Nicholas Blumhardt

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in
    all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    THE SOFTWARE.


