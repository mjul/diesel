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

        (defcommand ImportEmployeeCommand (int EmployeeNumber, string FirstName, string LastName)))

Use the Visual Studio T4 template to generate the code or compile it the model source 
with a single method call: `DieselCompiler.Compile(modelSource)`. 


Now use it


	// value types automatically get a constructor setting their value
    var founderNumber = new EmployeeNumber(1);
	var lateHireNumber = new EmployeeNumber(100);
	
	// You get free value equality and equality operators 
	var isFounder = (employee.EmployeeNumber == founderNumber);

	// Commands are classes and have a constructor that assigns all properties
	var command = new ImportEmployeeCommand(1, "Martin", "Jul");
	
	// Properties exposing these are automatically added to the class:
	var firstName = command.FirstName;


  
# Defining simple value types

## `(defvaluetype <typename> <type?>)`

When you compile the defvaluetype, Diesel generates .NET code in your language,
building a struct with a Value property carrying, constructor to set it, and
equality operator overloads.

The type is optional, it defaults to Int32.

### Example

    (defvaluetype EmployeeNumber)
    (defvaluetype Amount Decimal)


# Defining Commands

## `(defcommand <typename> <properties>)`

This defines a class representing the Command, the properties are assigned via a constructor
and equals and equality operators are implemented with value semantics.

### Example

    (defcommand ImportEmployee (int EmployeeNumber, string FirstName, string LastName))

This generates a class with properties `EmployeeNumber`, `FirstName` and `LastName`.


# Defining Namespaces
## `(namespace <name> <typedeclarations*>)`

A model consists of one or more namespace declarations.


# Using the T4 Template

The Test project contains a Visual Studio T4 template (.tt file) under Generated.
It can be used as a template for adding code generation to your own project.
It depends on the Diesel compiler and its dependencies being available in binary form 
in the project (assemblies).


# License
Copyright (C)2013 Martin Jul (www.mjul.com)

Distributed under the MIT License. See the LICENSE.txt file for details.

