(namespace Test.Diesel.Generated 
	(defvaluetype EmployeeNumber)
	(defvaluetype EmailAddress string)
	(defvaluetype EmployeeName (string FirstName, string LastName))
	(defapplicationservice ImportService
		(defcommand ImportEmployee (int EmployeeNumber, string FirstName, string LastName))))

