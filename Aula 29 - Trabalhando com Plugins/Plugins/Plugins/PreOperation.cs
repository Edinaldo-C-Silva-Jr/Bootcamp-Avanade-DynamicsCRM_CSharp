using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace Plugins
{
    public class PreOperation : IPlugin
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

                if (contextEntity.LogicalName == "account" && contextEntity.Attributes.Contains("telephone1"))
                {
                    string phone1 = contextEntity["telephone1"].ToString();

                    string fetchContact = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' savedqueryid='00000000-0000-0000-00aa-000010001003' no-lock='false' distinct='false'>
                                                <entity name='contact'>
                                                    <attribute name='fullname'/>
                                                    <attribute name='telephone1'/>
                                                    <attribute name='contactid'/>
                                                        <order attribute='fullname' descending='false'/>
                                                    <filter type='and'>
                                                        <condition attribute='telephone1' operator='eq' value='{phone1}'/>
                                                    </filter>
                                                </entity>
                                            </fetch>";

                    tracer.Trace($"FetchContact: {fetchContact}");

                    EntityCollection contactList = service.RetrieveMultiple(new FetchExpression(fetchContact));

                    if (contactList.Entities.Count > 0)
                    {
                        foreach (Entity entity in contactList.Entities)
                        {
                            contextEntity["primarycontactid"] = new EntityReference("contact", entity.Id);
                        }
                    }
                }
            }
        }
    }
}
