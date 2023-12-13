using Microsoft.Xrm.Sdk;
using System;

namespace Plugins
{
    public class PreValidation : IPlugin
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
                tracer.Trace($"Entidade do Contexto: {contextEntity.Attributes.Count}");

                if (contextEntity == null)
                {
                    return;
                }

                if (!contextEntity.Contains("telephone1"))
                {
                    throw new InvalidPluginExecutionException("Campo de Telefone Principal é obrigatório!");
                }
                else if (String.IsNullOrEmpty(contextEntity["telephone1"].ToString()))
                {
                    throw new InvalidPluginExecutionException("Campo de Telefone Principal não pode estar vazio!");
                }

            }
        }
    }
}
