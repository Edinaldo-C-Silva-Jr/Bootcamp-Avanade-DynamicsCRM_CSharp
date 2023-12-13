function CallCustomAction(executionContext) {
	var globalContext = Xrm.Utility.getGlobalContext(); // Gets the global context of the page
	
	var request = new XMLHttpRequest();
	request.open("POST", globalContext.getClientUrl() + "/api/data/v9.2/ecs_CallCustomAction", true); // getClientUrl gets the URL of the environment where this script runs
	
	request.setRequestHeader("Accept", "application/json"); // Sets the standard parameters for HTTP Requests
	request.setRequestHeader("Content.Type", "application/json; charset=utf-8");
	request.setRequestHeader("OData-MaxVersion", "4.0");
	request.setRequestHeader("OData-Version", "4.0");
	
	request.onreadystatechange = function () {
		if (this.readyState == 4) { // 4 means "Done"
			request.onreadystatechange = null;
			
			if (this.status == 200 || this.status == 204) { // 200 and 204 are success
				Xrm.Utility.AlertDialog("Custom Action executada com sucesso!");
			} else {
				var error = JSON.parse(this.response).error;
				Xrm.Utility.AlertDialog("Erro na action: " + error.message);
			}
		}
	}
	
	request.send(JSON.stringify());
}