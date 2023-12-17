using Microsoft.Xrm.Sdk;
using System;

namespace Plugins
{
    /// <summary>
    /// A Plugin to validate the integrity of data in the "telephone1" field of the "account" entity
    /// Works for both Create and Update messages.
    /// </summary>
    public class AccountPreValidation : IPlugin
    {
        /// <summary>
        /// Method Execute, which will be called from Dynamics 365 to run the Plugin.
        /// </summary>
        /// <param name="serviceProvider">The service provider that allows passing information to the plugin.</param>
        /// <exception cref="InvalidPluginExecutionException">If the "telephone1" field of the "account" entity is empty.</exception>
        public void Execute(IServiceProvider serviceProvider)
        {
            // Default implementation of a plugin for Dynamics 365.
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext)); // The information about the context the plugin is executed on, such as the record it is called from.
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory)); // A factory for creating organization services.
            IOrganizationService service = serviceFactory.CreateOrganizationService(null); // The organization service, which allows accessing data and metadata from the organization via the SDK, allowing the execution of methods such as CRUD operations.
            ITracingService tracer = (ITracingService)serviceProvider.GetService(typeof(ITracingService)); // Tracing service for logging information during the plugin execution.

            // Checks if the Input Parameters contain the "Target" property, which defines the entity passed to the plugin by the execution context.
            if (context.InputParameters.Contains("Target"))
            {
                Entity contextEntity = (Entity)context.InputParameters["Target"];

                // Only proceed if the entity passed by the context exists and is the "account" entity.
                if (contextEntity == null || contextEntity.LogicalName != "account")
                {
                    tracer.Trace($"Conta nula ou a entidade não é conta");
                    return;
                }

                if (!contextEntity.Contains("telephone1")) // Checks if the "telephone1" field has been filled, used for the Create message.
                {
                    tracer.Trace($"Campo de telefone não foi preenchido!");
                    throw new InvalidPluginExecutionException("Campo de Telefone Principal é obrigatório!");
                }
                else if (String.IsNullOrEmpty(contextEntity["telephone1"].ToString())) // Checks if the "telephone1" field is empty, used for the Update message.
                {
                    tracer.Trace($"Campo de telefone vazio!");
                    throw new InvalidPluginExecutionException("Campo de Telefone Principal não pode estar vazio!");
                }

                tracer.Trace($"Plugin executado com sucesso! Conta criada/alterada com telefone!");
            }
        }
    }
}
