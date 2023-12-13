using Microsoft.Xrm.Sdk;
using System;

namespace Plugins
{
    public class AsyncPostOperation : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(null);
            ITracingService tracer = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            if (context.InputParameters.Contains("Target"))
            {
                Entity contextEntity = (Entity)context.InputParameters["Target"];

                for (int i = 0; i < 10; i++)
                {
                    Entity contact = new Entity("contact");

                    contact.Attributes["firstname"] = "Contato Assincrono Vinculado a Conta";
                    contact.Attributes["lastname"] = contextEntity["name"] + " - " + DateTime.Now.ToString();
                    contact.Attributes["parentcustomerid"] = new EntityReference("account", context.PrimaryEntityId);
                    contact.Attributes["ownerid"] = new EntityReference("systemuser", context.UserId);

                    tracer.Trace($"firstname: {contact.Attributes["firstname"]}");

                    service.Create(contact);
                }
            }
        }
    }
}
