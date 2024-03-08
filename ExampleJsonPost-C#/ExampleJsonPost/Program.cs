using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


class Program
{
    static async Task Main(string[] args)
    {
        const string baseUrl = "https://optimizer2.routesavvy.com/RSAPI.svc/";

        try
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: <input json request file> <output json result file>");
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
                return;
            }

            string inFilePath = args[0];
            string outFilePath = args[1];

            string requestStr = File.ReadAllText(inFilePath).Replace("\t", "");

            string endpoint = "POSTOptimize";
            string requestUrl = $"{baseUrl}{endpoint}";

            using (HttpClient client = new HttpClient())
            {
                StringContent content = new StringContent(requestStr);
                content.Headers.ContentType.MediaType = "application/json";

                using (HttpResponseMessage response = await client.PostAsync(requestUrl, content)) 
                {

                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();

                    dynamic jsonObject = JsonConvert.DeserializeObject(responseBody);

                    // Access properties of the dynamic object
                    string message = jsonObject.Message;

                    if (message != null && message != "Success")
                    {
                        Console.WriteLine($"An error occurred: {message}");
                    }
                    else
                    {
                        try
                        {
                            JToken token = JToken.Parse(responseBody);
                            JObject outputJson = JObject.Parse(token.ToString(Newtonsoft.Json.Formatting.Indented));
                            File.WriteAllText(outFilePath, outputJson.ToString());
                            Console.WriteLine("Success");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"An error occurred: {ex.Message}");
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: " + e.Message);
        }

        Console.WriteLine("Press any key to exit.");
        Console.ReadKey();
    }
}


