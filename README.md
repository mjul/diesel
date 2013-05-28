# Diesel is a DSL toolkit for .NET code generation for DDD

Diesel provides a declarative language for generating code for your .NET projects.

* Value Types - generate strong types for value types, e.g. EmployeeNumber instead of int.
* Commands - generates classes for the Command DTOs

## Example

Create a model in the DSL language:

    (namespace Employees
        (defvaluetype EmployeeNumber)
        (defvaluetype FirstName string)
        (defvaluetype LastName string)
        (defapplicationService ImportService
          (defcommand ImportEmployeeCommand (int EmployeeNumber, string FirstName, string LastName))))

Use the Visual Studio T4 template to automatically generate the code or compile it the model source 
with a single method call: `DieselCompiler.Compile(modelSource)`. 

Now use it

```csharp
    // value types automatically get a constructor setting their value
    var founderNumber = new EmployeeNumber(1);
    var lateHireNumber = new EmployeeNumber(100);
    
    // You get free value equality and equality operators 
    var isFounder = (employee.EmployeeNumber == founderNumber);

    // Commands are classes and have a constructor that assigns all properties
    var command = new ImportEmployeeCommand(1, "Martin", "Jul");
    
    // Properties exposing these are automatically added to the class:
    var firstName = command.FirstName;
```

  
# Defining simple value types

    (defvaluetype <typename> <type?>)

When you compile the defvaluetype, Diesel generates .NET code in your language,
building a struct with a Value property carrying, constructor to set it, and
equality operator overloads.

The type is optional, it defaults to Int32.

### Example

    (defvaluetype EmployeeNumber)
    (defvaluetype Amount Decimal)

The first of these declarations yields a type with value semantics and equality operator overloads:

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

### Example

    (defcommand ImportEmployee (int EmployeeNumber, string FirstName, string LastName))

This generates a class with properties `EmployeeNumber`, `FirstName` and `LastName`.


# Defining Application Services

    (defapplicationservice <name> <commands+>)

This defines an Application Service of the given name. 

The code generator emits an interface with Execute overloads for each of the commands
defined in the scope of the application service.

### Example

Given the following declaration

    (defapplicationService ImportService
      (defcommand ImportEmployee (int EmployeeNumber, string FirstName, string LastName))

This declares the `ImportEmployee` class as defined by the nested `defcommand` and the following
interface for the service:

```csharp
     public partial interface IImportService {
        
        void Execute(ImportEmployee command);
    }
```



# Defining Namespaces
`(namespace <name> <typedeclarations*>)`

A model consists of one or more namespace declarations.


# Using the T4 Template

The Test project contains a Visual Studio T4 template (.tt file) under Generated.
It can be used as a template for adding code generation to your own project.
It depends on the Diesel compiler and its dependencies being available in binary form 
in the project (assemblies).


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


