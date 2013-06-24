# Feature Requests

## Declare Bounded Contexts
Commands and Domain Events should then have DataContract namespaces
corresponding to the bounded context's name.

## Support for C# keyword Identifiers
Support for Identifiers like `@event` and other reserved C# keywords.

## Emit DebuggerDisplay for types with more than one field
Currently, this is emitted only for single-field types.

## Configurable DebuggerDisplay on types
Allow a way to configure the DebuggerDisplay on the generated types, e.g.
declaring a way to generate `[DebuggerDisplay("{FirstName} {LastName}")]` 
on a value type, `(defvaluetype StudentName (string FirstName, string LastName))`.

## Validate model before generation

### Validate allowed types in DTO types (Commands, Events, DTOs).
Commands and Domain events should only be able to have properties that are primitive types,
arrays of these or other Data Transfer Objects.

### Emit friendly error when code is not generatable 
E.g. for multi-dimensional arrays in Equality.


## Multi-dimensional array equality
Currently one single-dimensional arrays are supported when generating equality members.
Implement these, too:

		(namespace TestCases.TypeDeclarations
			(defcommand PrintArraySimple2D (int[,] Value))
			(defcommand PrintArraySimpleMulti (int[][,] Value))
