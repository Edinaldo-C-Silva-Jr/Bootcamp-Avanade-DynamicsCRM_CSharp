function ValidarCPF(executionContext){
	// Only run the function if the execution context is not null
	if (executionContext === null){
		return;
	}
	
	// Context related variables
	var formContext = executionContext.getFormContext();
	var cpf = executionContext.getEventSource().getValue();
	var fieldName = executionContext.getEventSource().getName();
	
	// Starts the CPF validation
	var isValid = true;
	if (cpf !== null && cpf !== ""){
		// Removes all non digit characters from the string
		cpf = cpf.replace(/[\D_]/g , "");
		
		// Checks if the CPF has the correct size
		if (cpf.length === 11){
			
			// Checks for invalid trivial patterns
			if (cpf != "11111111111" && cpf != "22222222222" && cpf != "33333333333" 
			&& cpf != "44444444444" && cpf != "55555555555" && cpf != "66666666666" 
			&& cpf != "77777777777" && cpf != "88888888888" && cpf != "99999999999"){
				
				var sum = 0, remainder;
			
				// Validates the first verification digit
				for(var i = 0; i < 9; i++){
					sum += parseInt(cpf.substring(i, i+1)) * (i+1);
				}
				remainder = sum % 11;
				if (remainder === 10){
					remainder = 0;
				}
				if (remainder != cpf.substring(9, 10)){
					isValid = false;
				}
			
				// Validates the second verification digit
				sum = 0;
				for(var i = 0; i < 10; i++){
					sum += parseInt(cpf.substring(i, i+1)) * i;
				}
				remainder = sum % 11;
				if (remainder === 10){
					remainder = 0;
				}
				if (remainder != cpf.substring(10, 11)){
					isValid = false;
				}
			} else {
				isValid = false;
			}
		} else {
			isValid = false;
		}
	} else {
		isValid = false;
	}
	
	if (isValid) {
		// If CPF id valid, correctly formats it and inserts it into the form field
		cpf = cpf.substring(0, 3) + '.' + cpf.substring(3, 6) + '.' + cpf.substring(6, 9) + '-' + cpf.substring(9);
		cpf = formContext.getAttribute(fieldName).setValue(cpf);
	} else {
		// If CPF is not valid, show a warning and clears the form field
		Xrm.Navigation.openAlertDialog({confirmButtonLabel: "Ok", text: "CPF: " + cpf + "\n Este valor de CPF não é válido."}, {height: 100, width: 300});
		cpf = formContext.getAttribute(fieldName).setValue("");
	}
}