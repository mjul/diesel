# Feature Requests

## Emit verbatim type-name for unknown types
When a named type does not exist, emit the type name as specified
rather than failing in the generation step when the corresponding System type 
has not been found (`CodeDomGenerator`, `SystemTypeMapper`).

## Validate model before generation

### Validate allowed types in DTO types (Commands, Events).
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
