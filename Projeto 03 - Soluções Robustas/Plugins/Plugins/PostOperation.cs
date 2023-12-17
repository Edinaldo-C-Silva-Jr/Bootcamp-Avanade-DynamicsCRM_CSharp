using Microsoft.Xrm.Sdk;
using System;

namespace Plugins
{
    public class PostOperation : IPlugin
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

                if (!contextEntity.Contains("websiteurl"))
                {
                    throw new InvalidPluginExecutionException("Campo de Website é obrigatório!");
                }

                Entity task = new Entity("task");
                task.Attributes["ownerid"] = new EntityReference("systemuser", context.UserId);
                task.Attributes["regardingobjectid"] = new EntityReference("account", contextEntity.Id);
                task.Attributes["subject"] = $"Visite nosso site: {contextEntity["websiteurl"]}";
                task.Attributes["description"] = "Task criada via Plugin Post Operation";

                service.Create(task);
            }
        }
    }
}
