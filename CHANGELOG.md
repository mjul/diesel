# Version 1.5
* Parser requires entire source file to be valid (before it unintentionally allowed anything to follow the AST).
* 


# Version 1.4 (June 3rd, 2013)
* Support for nullable types in properties introduced (e.g. `defcommand` and `defvaluetype`).
* Support for System types DateTime and Guid in added to properties in `defcommand` and `defvaluetype`.

# Version 1.3 (June 3rd, 2013)
* `defvaluetype` extended to also accept a list of properties like `defcommand`.

# Version 1.2 (May 28th, 2013)
* `defapplicationservice` added, generating a service interface for the associated commands.

# Version 1.1 (May 27th, 2013)
* `defvaluetype` adds binary serializability.
* `defcommand` adds serializability.
* `defcommand` now adds DataContract and DataMember attributes for XML serialization.

# Version 1.0 (May 24th, 2013)
Initial release.
Includes `namespace`, `defvaluetype` and `defcommand`.
