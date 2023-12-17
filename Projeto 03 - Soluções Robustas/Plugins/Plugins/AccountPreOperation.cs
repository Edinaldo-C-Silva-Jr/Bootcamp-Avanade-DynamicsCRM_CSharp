using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace Plugins
{
    /// <summary>
    /// A Plugin that checks if any contact exists in the database with the same telephone as the account.
    /// If the contact exists, then it sets that contact as the account's primary contact.
    /// Works for both Create and Update messages.
    /// </summary>
    public class AccountPreOperation : IPlugin
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

                // Only proceed if the entity passed by the context exists, is the "account" entity and contains the "telephone1" field.
                if (contextEntity == null || contextEntity.LogicalName != "account" || !contextEntity.Attributes.Contains("telephone1"))
                {
                    tracer.Trace($"Conta nula ou a entidade não é conta");
                    return;
                }

                string accountPhone = contextEntity["telephone1"].ToString();
                tracer.Trace($"Telefone da conta: {accountPhone}");

                // Fetches in the database any contact with the same telephone as the account. Returns the contact's name, telephone and id.
                string fetchContact = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' no-lock='false' distinct='false'>
                                                <entity name='contact'>
                                                    <attribute name='fullname'/>
                                                    <attribute name='telephone1'/>
                                                    <attribute name='contactid'/>
                                                        <order attribute='fullname' descending='false'/>
                                                    <filter type='and'>
                                                        <condition attribute='telephone1' operator='eq' value='{accountPhone}'/>
                                                    </filter>
                                                </entity>
                                            </fetch>";

                EntityCollection contactList = service.RetrieveMultiple(new FetchExpression(fetchContact)); // Returns all results of contacts using the above query.

                // If the query returns at least one contact.
                if (contactList.Entities.Count > 0)
                {
                    foreach (Entity returnedContact in contactList.Entities)
                    {
                        tracer.Trace($"Contato: {returnedContact.Id} \nNome do contato: {returnedContact.Attributes["fullname"]}");
                        
                        // Sets the contact returned as the entity's primary contact. (Note: if more than one result is returned, every new iteration replaces the previously set contact.)
                        contextEntity["primarycontactid"] = new EntityReference("contact", returnedContact.Id);
                    }
                }
            }
        }
    }
}