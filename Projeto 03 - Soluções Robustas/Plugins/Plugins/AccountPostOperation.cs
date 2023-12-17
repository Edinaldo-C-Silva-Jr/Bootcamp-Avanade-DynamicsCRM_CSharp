using Microsoft.Xrm.Sdk;
using System;

namespace Plugins
{
    /// <summary>
    /// A Plugin that creates a Task for the account, which says to visit that account's website.
    /// It is required that the account has the "websiteurl" field filled.
    /// Used only for the Create message.
    /// </summary>
    public class AccountPostOperation : IPlugin
    {
        /// <summary>
        /// Method Execute, which will be called from Dynamics 365 to run the Plugin.
        /// </summary>
        /// <param name="serviceProvider">The service provider that allows passing information to the plugin.</param>
        /// <exception cref="InvalidPluginExecutionException">If the "websiteurl" field in the "account" entity is empty.</exception>
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

                // Checks if the "websiteurl" field has been filled 
                if (!contextEntity.Contains("websiteurl"))
                {
                    tracer.Trace($"Campo de Website não foi preenchido.");
                    throw new InvalidPluginExecutionException("Campo de Website é obrigatório!");
                }

                tracer.Trace($"Website da Conta: {contextEntity["websiteurl"]}");

                // Creates a new task that says to access the company's website
                Entity task = new Entity("task");
                task.Attributes["ownerid"] = new EntityReference("systemuser", context.UserId);
                task.Attributes["regardingobjectid"] = new EntityReference("account", contextEntity.Id);
                task.Attributes["subject"] = $"Visite nosso site: {contextEntity["websiteurl"]}";
                task.Attributes["description"] = "Task criada via Plugin Post Operation";
                service.Create(task);

                tracer.Trace("Task criada com sucesso!");
            }
        }
    }
}
