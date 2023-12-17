using Microsoft.Xrm.Sdk;
using System;

namespace Plugins
{
    /// <summary>
    /// A Plugin that creates 20 contacts related to the newly created account.
    /// Used only in the Create message, as an asynchronous plugin.
    /// </summary>
    public class AccountAsyncPostOperation : IPlugin
    {
        /// <summary>
        /// Method Execute, which will be called from Dynamics 365 to run the Plugin.
        /// </summary>
        /// <param name="serviceProvider">The service provider that allows passing information to the plugin.</param>
        public void Execute(IServiceProvider serviceProvider)
        {
            // Default implementation of a plugin for Dynamics 365.
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(null);
            ITracingService tracer = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            if (context.InputParameters.Contains("Target"))
            {
                Entity contextEntity = (Entity)context.InputParameters["Target"];

                // Only proceed if the entity passed by the context exists and is the "account" entity.
                if (contextEntity == null || contextEntity.LogicalName != "account")
                {
                    tracer.Trace($"Conta nula ou a entidade não é conta");
                    return;
                }

                tracer.Trace($"Iniciando criação de contatos.");

                // Iterates over the creation of 20 contacts.
                for (int i = 0; i < 20; i++)
                {
                    Entity contact = new Entity("contact");
                    contact.Attributes["firstname"] = $"Contato Assincrono {i + 1}";
                    contact.Attributes["lastname"] = $"Conta: {contextEntity["name"]}";
                    contact.Attributes["parentcustomerid"] = new EntityReference("account", context.PrimaryEntityId);
                    contact.Attributes["ownerid"] = new EntityReference("systemuser", context.UserId);
                    service.Create(contact);

                    tracer.Trace($"Contato criado - Nome: {contact.Attributes["firstname"]}");
                }
            }
        }
    }
}
