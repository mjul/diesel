# Version 1.10
* Added support for properties that are uni-dimensional arrays of named types (e.g. `EmployeeNumber[]`).

# Version 1.9
* Support for single-line comments added (semicolon to end-of-line)
* `defenum` added to generate enumeration DTOs (Data Transfer Objects).
* Properties can now be any type (generator will emit names of custom/non-system types literally)

# Version 1.8
* `defdto` added to generate DTOs (Data Transfer Objects).

# Version 1.7
* Support for arrays in value types, commands and domain events.
* Grammar for property specification now follows C# grammar more closely.

# Version 1.6
* `defdomainevent` added to generate domain event DTOs.
* `defconventions` added to configure the code-generation. For now, just configurable base classes for Domain Events.

# Version 1.5
* Parser requires entire source file to be valid (before it unintentionally allowed anything to follow the AST).
* Added DataContractSerializer serializability for commands (`defcommand`).

# Version 1.4
* Support for nullable types in properties introduced (e.g. `defcommand` and `defvaluetype`).
* Support for System types DateTime and Guid in added to properties in `defcommand` and `defvaluetype`.

# Version 1.3
* `defvaluetype` extended to also accept a list of properties like `defcommand`.

# Version 1.2
* `defapplicationservice` added, generating a service interface for the associated commands.

# Version 1.1
* `defvaluetype` adds binary serializability.
* `defcommand` adds serializability.
* `defcommand` now adds DataContract and DataMember attributes for XML serialization.

# Version 1.0
Initial release.
Includes `namespace`, `defvaluetype` and `defcommand`.
