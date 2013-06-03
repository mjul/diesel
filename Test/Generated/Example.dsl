(namespace Test.Diesel.Generated 
	(defvaluetype EmployeeNumber)
	(defvaluetype EmailAddress string)
	(defvaluetype EmployeeName (string FirstName, string LastName))
	(defvaluetype EmployeeMetadata (string Source, int? SourceId))
	(defapplicationservice ImportService
		(defcommand ImportEmployee (Guid CommandId, int EmployeeNumber, string FirstName, string LastName, int? SourceId))
		(defcommand ImportConsultant (string FirstName, string LastName, string Company))))

