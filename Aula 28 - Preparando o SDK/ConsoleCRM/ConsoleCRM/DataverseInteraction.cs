using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;

namespace ConsoleCRM
{
    public class DataverseInteraction
    {
        public void ReturnRows(CrmServiceClient client)
        {
            string fetchXML = @"<fetch version='1.0' mapping='logical' savedqueryid='00000000-0000-0000-00aa-000010001001' no-lock='false' distinct='true'>
                                    <entity name='account'>
                                        <attribute name='name'/>
                                        <attribute name='telephone1'/>
                                            <order attribute='name' descending='false'/>
                                            <filter type='and'>
                                                <condition attribute='statecode' operator='eq' value='0'/>
                                            </filter>
                                    </entity>
                                </fetch>";

            EntityCollection collectionOfEntities = client.RetrieveMultiple(new FetchExpression(fetchXML));

            foreach (var item in collectionOfEntities.Entities)
            {
                Console.WriteLine(item["name"]);
            }
            Console.ReadLine();
        }
    }
}
