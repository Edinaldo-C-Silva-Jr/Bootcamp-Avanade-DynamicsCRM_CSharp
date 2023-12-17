using Microsoft.Xrm.Sdk;
using System;

namespace CustomAction
{
    /// <summary>
    /// An action that creates a record of the "lead" entity.
    /// This action is activated by a Ribbon Workbench button.
    /// </summary>
    public class ActionCreateLead : IPlugin
    {
        /// <summary>
        /// Method Execute, which will be called from Dynamics 365 to run the Plugin.
        /// </summary>
        /// <param name="serviceProvider">The service provider that allows passing information to the plugin.</param>
        public void Execute (IServiceProvider serviceProvider)
        {
            // Default implementation of a plugin for Dynamics 365, since an action is a plugin that is executed by a process.
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(null);
            ITracingService tracer = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            
            tracer.Trace("Iniciando a criação do Cliente Potencial através de Action.");

            // Creating a record of the "lead" entity.
            // Since actions are not tied to any entity, there's no need to validate the target.
            Entity entityLead = new Entity("lead");
            entityLead["subject"] = "Cliente Potencial Criado por Action";
            entityLead["firstname"] = "Primeiro Nome";
            entityLead["lastname"] = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:ff");
            entityLead["ownerid"] = new EntityReference("systemuser", context.UserId);

            // Creates the lead and returns the record's Guid to be added to the log
            Guid guidLead = service.Create(entityLead);
            tracer.Trace($"Cliente Potencial criado: {guidLead}");
        }
    }
}
