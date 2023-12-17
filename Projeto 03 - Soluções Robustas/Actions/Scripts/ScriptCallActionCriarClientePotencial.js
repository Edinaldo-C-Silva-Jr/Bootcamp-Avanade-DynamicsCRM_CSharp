function CallActionCriarClientePotencial(primaryControl) {
	var globalContext = Xrm.Utility.getGlobalContext(); // Gets the global context of the page that executes the script.
	
	var request = new XMLHttpRequest();
	
	// getClientUrl gets the URL of the environment where this script runs.
	// Creates a POST request to call the process in the URL.
	request.open("POST", globalContext.getClientUrl() + "/api/data/v9.2/ecs_ActionCriarClientePotencial", true); 
	
	// Sets the standard parameters for HTTP Requests.
	request.setRequestHeader("Accept", "application/json");
	request.setRequestHeader("Content-Type", "application/json; charset=utf-8");
	request.setRequestHeader("OData-MaxVersion", "4.0");
	request.setRequestHeader("OData-Version", "4.0");
	
	request.onreadystatechange = function () {
		if (this.readyState == 4) { // Code 4 means the state is "Done".
			request.onreadystatechange = null;
			
			if (this.status == 200 || this.status == 204) { // Status codes 200 (Ok) and 204 (NoContent) mean the request was successful.
				Xrm.Navigation.openAlertDialog("Criação de Cliente Potencial executada com sucesso!");
			} else {
				var error = JSON.parse(this.response).error;
				Xrm.Navigation.openAlertDialog("Erro ao criar Cliente Potencial: " + error.message);
			}
		}
	}
	
	request.send(JSON.stringify());
}