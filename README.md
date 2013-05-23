# Diesel is a DSL toolkit for .NET code generation for DDD

Diesel provides a declarative language for generating code for your .NET projects.

* Value Types - generate strong types for value types, e.g. EmployeeNumber instead of int.
* Commands - generates classes for the Command DTOs
* 

## Example

Create a model in the DSL language:

    (defvaluetype EmployeeNumber)
    (defvaluetype FirstName string)
    (defvaluetype LastName string)

	(defcommand ImportEmployee (int EmployeeNumber, string FirstName, string LastName))

Now use it

	public Employee(EmployeeNumber number, FirstName first, LastName last)
	{
		
	}


# Defining simple value types

## (defvaluetype <typename> <type>)

When you compile the defvaluetype, Diesel generates .NET code in your language,
building a struct with a Value property carrying, constructor to set it, and
equality operator overloads.


# Defining Commands

## (defcommand <typename> <properties>)

This defines a class representing the Command, the properties are assigned via a constructor
and equals and equality operators are implemented with value semantics.

### Example

	(defcommand ImportEmployee (int EmployeeNumber, string FirstName, string LastName))





# License
Copyright (C)2013 Martin Jul (www.mjul.com)

Distributed under the MIT License. See the LICENSE.txt file for details.

