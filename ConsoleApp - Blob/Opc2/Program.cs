using System;
using System.Threading;
using TitaniumAS.Opc.Client.Common;
using TitaniumAS.Opc.Client.Da;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Opc2
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            TitaniumAS.Opc.Client.Bootstrap.Initialize();
            Uri url = UrlBuilder.Build("Matrikon.OPC.Simulation.1");

            using (var server = new OpcDaServer(url))
            {
                server.Connect();

                OpcDaGroup group = server.AddGroup("Group1");
                group.IsActive = true;


                var itemValue1 = new OpcDaItemDefinition
                {

                    ItemId = "CC2028:.tINTF_General_Visu.tWind.tWindSpeed.rAnalogSignal",
                    IsActive = true,

                };

                /*var itemValue2 = new OpcDaItemDefinition
                {

                    ItemId = "CC2028:.tINTF_General_Visu.tWind.tWindDirectionSensor.rAnalogSignal",
                    IsActive = true,

                };*/

                OpcDaItemDefinition[] opcDaItems = { itemValue1 };//, itemValue2 };

                OpcDaItemResult[] results = group.AddItems(opcDaItems);

                while (true)
                {
                    OpcDaItemValue[] values = group.Read(group.Items, OpcDaDataSource.Device);
                    foreach (OpcDaItemValue value in values)
                    {
                        try
                        {
                            Console.WriteLine($"The Value for item {value.Item.ItemId} is {value.Value}");

                            int newInt = int.Parse(value.Value.ToString());
                            await SendValueToServerless(value.Item.ItemId, newInt, DateTime.Now.ToString());
                        }

                        catch (Exception ex)
                        {
                            Console.WriteLine("An error occurred while sending the value: " + ex.Message);
                        }
                    }

                    Thread.Sleep(5000);
                }
            }
        }

        private static async Task SendValueToServerless(string stringValue, int intValue, string date)
        {
            //string functionUrl = "http://localhost:7260/api/SendData";
            string functionUrl = "https://serverlessfunctionjd.azurewebsites.net/api/SendData";
        
            // Create HttpClient instance
            HttpClient httpClient = new HttpClient();

            // Prepare the data to send
            var requestData = new { Name = stringValue, Value = intValue, Date = date};
            string json = JsonConvert.SerializeObject(requestData);

            // Create the request content
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            // Make the HTTP POST request to the Azure website
            HttpResponseMessage response = await httpClient.PostAsync(functionUrl, content);

            // Read the response content as string
            string responseContent = await response.Content.ReadAsStringAsync();

            // Check the HTTP response status code
            if (response.IsSuccessStatusCode)
            {
                // Display a simplified success message
                Console.WriteLine("Values sent successfully.");
            }
            else
            {
                // Display a simplified error message
                Console.WriteLine("Failed to send the values. StatusCode: " + response.StatusCode);
            }
        }
    }
}

