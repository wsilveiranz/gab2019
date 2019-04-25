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

 public static async Task<object> Run(HttpRequestMessage req, TraceWriter log) 
{     log.Info($"Webhook was triggered!");  
    //intiliaze     
string imageInfo = string.Empty;      
    //get content     
string jsonContent = await req.Content.ReadAsStringAsync(); 
     log.Info($"Event : {jsonContent}"); 
    //event data is an Json Array 
    JArray data = JArray.Parse(jsonContent); 
     
    //get url from event data     foreach (JObject item in data) 
    {         var blobEventData = item.GetValue("data");         log.Info($"blobEventData : {blobEventData}"); 
         var imageUrl = blobEventData.Value<string>("url");         log.Info($"imageUrl : {imageUrl}"); 
         //read image         var webClient = new WebClient();         byte[] image = webClient.DownloadData(imageUrl);  
        //analyze image         imageInfo = AnalyzeImage(image); 
  
        //write to the console window         log.Info(imageInfo); 
    }      var response = req.CreateResponse(HttpStatusCode.OK);     response.Content = new StringContent(imageInfo, System.Text.Encoding.UTF8, "application/json");     return response; 
}  private static string AnalyzeImage(byte[] imageLocation) {     var client = new HttpClient();     var queryString = HttpUtility.ParseQueryString(string.Empty); 
      client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "your key"); 
      queryString["maxCandidates"] = "1";     var uri = " https://westeurope.api.cognitive.microsoft.com/vision/v1.0/describe? 
" + queryString; 
    HttpResponseMessage response; 
      using (var content = new ByteArrayContent(imageLocation)) {         content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");         response = client.PostAsync(uri, content).Result; 
          string imageInfo = response.Content.ReadAsStringAsync().Result; 
          return imageInfo; 
    } 
}
