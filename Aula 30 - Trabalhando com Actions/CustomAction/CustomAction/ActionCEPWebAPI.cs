using Microsoft.Xrm.Sdk;
using System;
using System.Net;
using System.Text;

namespace CustomAction
{
    public class ActionCEPWebAPI : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(null);
            ITracingService tracer = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            object cep = context.InputParameters["InputCEP"]; // Input Parameter passed from Dynamics 365
            tracer.Trace("Cep informado: " + cep);

            string URLViaCep = $"https://viacep.com.br/ws/{cep}/json/"; // The URL to access on the request
            string result = string.Empty;

            using (WebClient client = new WebClient()) // Declaring a Web Client to make a HTTP Request
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/json"; // Defining a header to the HTTP Request
                client.Encoding = Encoding.UTF8; // Defining the encoding for the request
                result = client.DownloadString(URLViaCep); // Access the URL with the request
            }

            context.OutputParameters["ResultCEP"] = result; // Output Parameters expected from Dynamics 365
            tracer.Trace($"Resultado: {result}");
        }
    }
}
