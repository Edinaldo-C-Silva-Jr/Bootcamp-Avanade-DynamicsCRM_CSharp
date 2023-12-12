using Microsoft.Xrm.Tooling.Connector;

namespace ConsoleCRM
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ConnectionWithCRM connection = new ConnectionWithCRM();
            CrmServiceClient connectionService = connection.GetConnection();
            
            DataverseInteraction rows = new DataverseInteraction();
            rows.ReturnRows(connectionService);
        }
    }
}
