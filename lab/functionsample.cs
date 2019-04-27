#r "Newtonsoft.Json"
#r "System.Web" 

using System; 
using System.Net; 
using Newtonsoft.Json; 
using Newtonsoft.Json.Linq; 
using System.IO; 
using System.Net.Http; 
using System.Net.Http.Headers; 
using System.Web; 

public static void Run(JObject eventGridEvent, TraceWriter log)
{
    log.Info(eventGridEvent.ToString(Formatting.Indented));
        log.Info($"Webhook was triggered!");  
    
    //intiliaze
    string imageInfo = string.Empty;      
      
        var blobEventData = eventGridEvent.GetValue("data");         
        log.Info($"blobEventData : {blobEventData}"); 
         var imageUrl = blobEventData.Value<string>("url");         
         log.Info($"imageUrl : {imageUrl}"); 
        
         //read image         
         var webClient = new WebClient();         
         byte[] image = webClient.DownloadData(imageUrl);  
        
        //analyze image         
        imageInfo = AnalyzeImage(image); 

        //write to the console window         
        log.Info(imageInfo);
        PushToEndpoint(imageInfo);

}

private static string AnalyzeImage(byte[] imageLocation) 
{     
    var client = new HttpClient();     
    var queryString = HttpUtility.ParseQueryString(string.Empty);
    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "51ebb4dace854e4a94da66a843d4175b");
    queryString["maxCandidates"] = "1";     
    var uri = "https://australiaeast.api.cognitive.microsoft.com/vision/v1.0/describe?" + queryString; 
    HttpResponseMessage response; 
    using (var content = new ByteArrayContent(imageLocation)) 
    {         
        content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");         
        response = client.PostAsync(uri, content).Result; 
        string imageInfo = response.Content.ReadAsStringAsync().Result; 
        return imageInfo; 
    } 
}

private static void PushToEndpoint(string imageInfo)
{
    var client = new HttpClient();     
    var queryString = HttpUtility.ParseQueryString(string.Empty);
    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "your key");
    queryString["maxCandidates"] = "1";

    var uri = "https://gib2018requestbin.herokuapp.com/1dzhkir1"; 
    HttpResponseMessage response; 
    using (var content = new StringContent(imageInfo)) 
    {         
        content.Headers.ContentType = new MediaTypeHeaderValue("application/text");         
        response = client.PostAsync(uri, content).Result; 
        string result = response.Content.ReadAsStringAsync().Result; 
    } 
}

