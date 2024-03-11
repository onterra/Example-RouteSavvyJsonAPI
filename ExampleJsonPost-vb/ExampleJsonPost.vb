Imports System.IO
Imports System.Net.Http
Imports Newtonsoft.Json.Linq

Module Program
    Dim POST As Boolean = True

    Async Sub Main(args As String())
        Dim baseUrl As String = "https://optimizer2.routesavvy.com/RSAPI.svc/"

        Try
            If args.Length <> 2 Then
                Console.WriteLine("Usage: <input json request file> <output json result file>")
                Console.WriteLine("Press any key to exit.")
                Console.ReadKey()
                Return
            End If

            Dim inFilePath As String = args(0)
            Dim outFilePath As String = args(1)

            Dim requestStr As String = File.ReadAllText(inFilePath).Replace(vbTab, "")

            Dim endpoint As String = "POSTOptimize"
            Dim requestUrl As String = $"{baseUrl}{endpoint}"

            Using client As New HttpClient()
                Dim content As New StringContent(requestStr)
                content.Headers.ContentType.MediaType = "application/json"

                Dim response As HttpResponseMessage = Await client.PostAsync(requestUrl, content)

                If response.IsSuccessStatusCode Then
                    Dim responseBody As String = Await response.Content.ReadAsStringAsync()
                    Dim token As JToken = JToken.Parse(responseBody)
                    Dim outputJson As JObject = JObject.Parse(token.ToString(Newtonsoft.Json.Formatting.Indented))
                    File.WriteAllText(outFilePath, outputJson.ToString())
                    Console.WriteLine("Success")
                Else
                    Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}")
                End If
            End Using
        Catch ex As Exception
            Console.WriteLine($"Exception: {ex.Message}")
        End Try

        Console.WriteLine("Press any key to exit.")
        Console.ReadKey()
    End Sub
End Module
