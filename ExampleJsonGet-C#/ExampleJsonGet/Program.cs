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
            Requestobject request = JsonConvert.DeserializeObject<Requestobject>(requestStr);

            requestStr = Regex.Replace(requestStr, @"\r\n?|\n|\t", "");
            string requestUrl = $"{baseUrl}{requestStr}";

            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client.GetAsync(requestUrl))
                {
                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();
                    Resultobject Resultobject = JsonConvert.DeserializeObject<Resultobject>(responseBody);

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
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }

        Console.WriteLine("Press any key to exit.");
        Console.ReadKey();
    }


    public class Requestobject
    {
        public Location[] Locations { get; set; }
        public Optimizeparameters OptimizeParameters { get; set; }
    }

    public class Optimizeparameters
    {
        public string AppId { get; set; }
        public string OptimizeType { get; set; }
        public string RouteType { get; set; }
        public string Avoid { get; set; }
        public DateTime Departure { get; set; }
    }

    public class Location
    {
        public string Name { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public int VisitDurationInMinutes { get; set; }
    }



    public class Resultobject
    {
        public string Message { get; set; }
        public Optimizedstop[] OptimizedStops { get; set; }
        public Route Route { get; set; }
    }

    public class Route
    {
        public float DriveDistance { get; set; }
        public string DriveDistanceUnit { get; set; }
        public int DriveTime { get; set; }
        public string DriveTimeUnit { get; set; }
        public Routeleg[] RouteLegs { get; set; }
        public float[][] RoutePath { get; set; }
    }

    public class Routeleg
    {
        public string[] Directions { get; set; }
        public float DriveDistance { get; set; }
        public int DriveTime { get; set; }
        public Legbegin LegBegin { get; set; }
        public Legend LegEnd { get; set; }
    }

    public class Legbegin
    {
        public string Name { get; set; }
        public Routelocation RouteLocation { get; set; }
    }

    public class Routelocation
    {
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }

    public class Legend
    {
        public string Name { get; set; }
        public Routelocation1 RouteLocation { get; set; }
    }

    public class Routelocation1
    {
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }

    public class Optimizedstop
    {
        public string Arrival { get; set; }
        public string Departure { get; set; }
        public string Distance { get; set; }
        public string Duration { get; set; }
        public bool IsDuplicate { get; set; }
        public string Name { get; set; }
        public Routelocation2 RouteLocation { get; set; }
        public int StopTimeMinutes { get; set; }
    }

    public class Routelocation2
    {
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }
}