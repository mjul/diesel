(namespace Test.Diesel.Generated 
	(defvaluetype EmployeeNumber)
	(defapplicationservice ImportService
		(defcommand ImportEmployee (int EmployeeNumber, string FirstName, string LastName))))

