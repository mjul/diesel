# Feature Requests

## Support for Enums in DTOs
For example `(defenum Gender (Male, Female))`

## Support for Nested DTOs
Extend TypeNameTypeNode generation to emit fields of non-System / non-primite types
to support tree structured types, and add a DTO type for non-root DTO classes
that can be used from Commands and Domain Events.

### Example

        (defdto Name (string First, string Last))
        (defcommand CreateEmployee (int EmployeeNumber, Name Name))

## Declare Bounded Contexts
Commands and Domain Events should then have DataContract namespaces
corresponding to the bounded context's name.

## Support for C# keyword Identifiers
Support for Identifiers like `@event` and other reserved C# keywords.

## Emit verbatim type-name for unknown types
When a named type does not exist, emit the type name as specified
rather than failing in the generation step when the corresponding System type 
has not been found (`CodeDomGenerator`, `SystemTypeMapper`).

## Emit DebuggerDisplay for types with more than one field
Currently, this is emitted only for single-field types.

## Configurable DebuggerDisplay on types
Allow a way to configure the DebuggerDisplay on the generated types, e.g.
declaring a way to generate `[DebuggerDisplay("{FirstName} {LastName}")]` 
on a value type, `(defvaluetype StudentName (string FirstName, string LastName))`.

## Validate model before generation

### Validate allowed types in DTO types (Commands, Events).
Commands and Domain events should only be able to have properties that are primitive types,
arrays of these or other Data Transfer Objects.

### Emit friendly error when code is not generatable 
E.g. for multi-dimensional arrays in Equality.

## Support for Comments
Add support for comments in the DSL language, e.g. `;; This is a single-line comment`.
        


## Multi-dimensional array equality
Currently one single-dimensional arrays are supported when generating equality members.
Implement these, too:

		(namespace TestCases.TypeDeclarations
			(defcommand PrintArraySimple2D (int[,] Value))
			(defcommand PrintArraySimpleMulti (int[][,] Value))
