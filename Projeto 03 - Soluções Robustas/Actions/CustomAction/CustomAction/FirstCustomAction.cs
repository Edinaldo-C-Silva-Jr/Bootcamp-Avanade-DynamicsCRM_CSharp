using Microsoft.Xrm.Sdk;
using System;

namespace CustomAction
{
    public class FirstCustomAction : IPlugin
    {
        public void Execute (IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(null);
            ITracingService tracer = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            tracer.Trace("A Primeira Action foi executada com sucesso! Criando Cliente Potencial no Dataverse.");

            Entity entityLead = new Entity("lead");
            entityLead["subject"] = "Cliente Potencial por Action";
            entityLead["firstname"] = "Primeiro Nome";
            entityLead["lastname"] = DateTime.Now.ToString();
            entityLead["mobilephone"] = "11912341234";
            entityLead["ownerid"] = new EntityReference("systemuser", context.UserId);

            Guid guidLead = service.Create(entityLead);
            tracer.Trace($"Lead criado: {guidLead}");
        }
    }
}
