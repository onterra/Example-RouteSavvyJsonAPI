using System;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            const string baseUrl = "https://optimizer2.routesavvy.com/RSAPI.svc/GetOptimize?query=";

            if (args.Length != 2)
            {
                Console.WriteLine("Usage: <input json request file> <output json result file>");
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
                return;
            }

            string inFilePath = args[0];
            string outFilePath = args[1];

            string requestStr = File.ReadAllText(inFilePath);
            requestStr = Regex.Replace(requestStr, @"\r\n?|\n|\t", "");
            string requestUrl = $"{baseUrl}{requestStr}";

            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(requestUrl))
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
                        string outputJson = JObject.Parse(token.ToString(Newtonsoft.Json.Formatting.Indented)).ToString();
                        File.WriteAllText(outFilePath, outputJson);
                        Console.WriteLine("Success");
                        Console.WriteLine(outputJson);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred: {ex.Message}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }

        Console.WriteLine("Press any key to exit.");
        Console.ReadKey();
    }
}