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

                if (item.Attributes.Contains("telephone1"))
                {
                    Console.WriteLine(item["telephone1"]);
                }
            }
            Console.ReadLine();
        }

        public void CreateRow(CrmServiceClient client)
        {
            Guid newRecordGuid;
            Entity newEntity = new Entity("account");
            newEntity.Attributes.Add("name", $"Nova Conta - {DateTime.Now}");
            newEntity.Attributes.Add("telephone1", "11233334444");

            newRecordGuid = client.Create(newEntity);

            if (newRecordGuid == Guid.Empty)
            {
                Console.WriteLine("Criação de registro falhou!");
            }
            else
            {
                Console.WriteLine($"Registro criado com sucesso! Id = {newRecordGuid}");
            }
            Console.ReadLine();
        }

        public void UpdateRow(CrmServiceClient client) 
        {
            string fetchXML = @"<fetch version='1.0' mapping='logical' savedqueryid='00000000-0000-0000-00aa-000010001001' no-lock='false' distinct='true'>
                                    <entity name='account'>
                                        <attribute name='name'/>
                                        <attribute name='accountid'/>
                                            <order attribute='name' descending='false'/>
                                            <filter type='and'>
                                                <condition attribute='name' operator='like' value='%Nova Conta%'/>
                                            </filter>
                                    </entity>
                                </fetch>";

            EntityCollection newAccountCollection = client.RetrieveMultiple(new FetchExpression(fetchXML));

            Entity newEntity = new Entity("account", newAccountCollection[0].Id) ;
            newEntity["name"] = $"Conta Alterada - {DateTime.Now}";

            client.Update(newEntity);
            Console.WriteLine($"Update Realizado na conta de Id: {newAccountCollection[0].Id}");
            Console.ReadLine();
        }
    }
}
