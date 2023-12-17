using Microsoft.Xrm.Sdk;
using System;
using System.Net;
using System.Text;

namespace CustomAction
{
    /// <summary>
    /// An action that takes the "address1_postalcode" value (CEP in a brazilian address) and consults the ViaCEP WebAPI to get the address values.
    /// </summary>
    public class ActionCEPWebAPI : IPlugin
    {
        /// <summary>
        /// Method Execute, which will be called from Dynamics 365 to run the Plugin.
        /// </summary>
        /// <param name="serviceProvider">The service provider that allows passing information to the plugin.</param>
        public void Execute(IServiceProvider serviceProvider)
        {
            // Default implementation of a plugin for Dynamics 365, since an action is a plugin that is executed by a process.
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(null);
            ITracingService tracer = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            object cep = context.InputParameters["InputCEP"]; // Input Parameter passed from Dynamics 365, contains the value of the field "address1_postalcode".
            tracer.Trace("Cep informado: " + cep);
            
            // The URL to access on the HTTP request.
            // This URL is from an API that consults a brazilian address through the CEP (postal code). It takes the value of the CEP as a parameter.
            string ViaCepURL = $"https://viacep.com.br/ws/{cep}/json/";
            string result = string.Empty;

            using (WebClient client = new WebClient()) // Declaring a Web Client to make the HTTP Request.
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/json"; // Defining a header to the HTTP Request.
                client.Encoding = Encoding.UTF8; // Defining the encoding for the request.
                result = client.DownloadString(ViaCepURL); // Accessing the URL with the request.
            }

            context.OutputParameters["ResultCEP"] = result; // Output Parameters expected from Dynamics 365, which are the result of the HTTP request.
            tracer.Trace($"Resultado: {result}");
        }
    }
}
