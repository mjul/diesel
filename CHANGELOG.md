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
