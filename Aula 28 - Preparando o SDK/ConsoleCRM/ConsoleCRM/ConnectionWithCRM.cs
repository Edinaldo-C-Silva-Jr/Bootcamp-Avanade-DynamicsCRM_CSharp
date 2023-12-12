using Microsoft.Xrm.Tooling.Connector;
using System.Net;

namespace ConsoleCRM
{
    public class ConnectionWithCRM
    {
        private static CrmServiceClient connectionClient;

        public CrmServiceClient GetConnection()
        {
            if (connectionClient == null)
            {
                // Change these variables to be able to access the desired environment
                string url = "Ambient URL";
                string username = "UserName";
                string password = "Password";

                string connectionString = $@"
                            AuthType = OAuth;
                            Url = {url};
                            UserName = {username};
                            Password = {password};
                            AppId = 51f81489-12ee-4a9e-aaae-a2591f45987d;
                            RedirectUri = app://58145B91-0C36-4500-8554-080854F2AC97;
                            LoginPrompt=Auto;
                            RequireNewInstance = True";

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                connectionClient = new CrmServiceClient(connectionString);
            }

            return connectionClient;
        }
    }
}
