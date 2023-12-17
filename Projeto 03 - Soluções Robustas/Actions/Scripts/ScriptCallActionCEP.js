function CallActionCEP(executionContext) {
	var globalContext = Xrm.Utility.getGlobalContext(); // Gets the global context of the page
	
	var formContext = executionContext.getFormContext(); // Gets the form context from the execution context
	var cep = formContext.getControl("address1_postalcode").getAttribute().getValue(); // Gets the value of the CEP field
	
	console.log("address1_postalcode: " + cep); // Logs the message on the browser console
	
	if (cep) {
		cep = cep.replace(/\D/g, ''); // Finds all non digit characters and replaces them with empty spaces
		
		var parameters = { // Defines the parameters that will be sent to the process
			"InputCEP": cep
		}
		
		var request = new XMLHttpRequest();
		request.open("POST", globalContext.getClientUrl() + "/api/data/v9.2/ecs_CallCEPAction", true); // getClientUrl gets the URL of the environment where this script runs
	
		request.setRequestHeader("Accept", "application/json"); // Sets the standard parameters for HTTP Requests
		request.setRequestHeader("Content-Type", "application/json; charset=utf-8");
		request.setRequestHeader("OData-MaxVersion", "4.0");
		request.setRequestHeader("OData-Version", "4.0");
		
		request.onreadystatechange = function () {
		if (this.readyState == 4) { // 4 means "Done"
			request.onreadystatechange = null;
			
			if (this.status == 200 || this.status == 204) { // 200 and 204 are success
				Xrm.Navigation.openAlertDialog("Custom Action executada com sucesso!");
				
				var result = JSON.parse(this.response); // Gets the result of the request
				const CEPJson = JSON.parse(result.ResultCEP); // Gets the Output Parameter from the process
				console.log(CEPJson);
				
				// Sets the values returned from the process into the Dynamics 365 entity
				formContext.getAttribute("address1_line1").setValue(CEPJson.logradouro);
				formContext.getAttribute("address1_line2").setValue(CEPJson.bairro);
				formContext.getAttribute("address1_city").setValue(CEPJson.localidade);
				formContext.getAttribute("address1_stateorprovince").setValue(CEPJson.uf);
				
			} else {
				var error = JSON.parse(this.response).error;
				Xrm.Navigation.openAlertDialog("Erro na action: " + error.message);
			}
		}
	}
	
	request.send(JSON.stringify(parameters));
	}
}