(defconventions :domainevents {:inherit [Test.Diesel.IDomainEvent]})

(namespace Test.Diesel.Generated 
	(defvaluetype EmployeeNumber)
	(defvaluetype EmailAddress string)
	(defvaluetype EmployeeName (string FirstName, string LastName))
	(defvaluetype EmployeeMetadata (string Source, int? SourceId))
	(defvaluetype EmployeeRatings (int EmployeeNumber, int[] Ratings))
	(defvaluetype EmployeeRoles (int EmployeeNumber, string[] Roles))
	;; Value types can contain other value types:
	(defvaluetype EmployeeInfo (EmployeeNumber Number, EmployeeName Name, EmailAddress Email))
	(defdomainevent EmployeeImported (Guid Id, int EmployeeNumber, string FirstName, string LastName, int? SourceId))
	(defdto Name (string First, string Last))
	(defenum Gender [Female Male])
	(defapplicationservice ImportService
		(defcommand ImportEmployee (Guid CommandId, int EmployeeNumber, string FirstName, string LastName, int? SourceId))
		(defcommand ImportConsultant (string FirstName, string LastName, string Company)))
	;; Verify that we can use nested DTOs and enums in commands:
	(defcommand ImportEmployeeNestedTypes(Guid CommandId, int EmployeeNumber, Name Name, Gender Gender)))

