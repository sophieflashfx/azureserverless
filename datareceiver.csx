#r "Newtonsoft.Json"
#r "Microsoft.WindowsAzure.Storage"
using Microsoft.WindowsAzure.Storage.Table;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
        public class DataEntry : TableEntity
        {
           
            public double value { get; set; }

        }
public static async Task<IActionResult> Run(HttpRequest  req, CloudTable inputTable, ICollector<DataEntry> outputTable, ICollector<string> outputQueueItem, ILogger log)
{
      log.LogInformation("QUTDemoWebEP Call");

            //3 params (username,cmd,value) are expected they can be passed either in the Query string or the request body

            //Checking Query string
            string cmd = null;
            string username = null;
            string value = null;
            string answer = "NA";
            try
            {
               
                    cmd = req.Query["cmd"];
                    username = req.Query["username"];
                    value = req.Query["value"];    
                           

                //Creating content object                          

                //Checking content object object for params missing in Query string
            
                 
                    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                    dynamic data = JsonConvert.DeserializeObject(requestBody);
                    if (username == null)
                    {
                        username =username ?? data?.username;
                    }
                    if (cmd == null)
                    {
                        cmd = cmd ?? data?.cmd;
                    }
                    if (value == null)
                    {
                        value = value?? data?.value;
                    }
                   
               
                    double dataValue = 0;
                    double.TryParse(value, out dataValue);
                    if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(cmd))
                    {
                        switch (cmd)
                        {
                            case "entry":
                                outputTable.Add(
                                  new DataEntry()
                                  {
                                      PartitionKey = username,
                                      RowKey = Guid.NewGuid().ToString("N"),
                                      value = dataValue
                                  }
                                    );
                                break;
                            case "add":
                                outputQueueItem.Add(username);
                                break;
                            case "answer":
                                TableQuery<DataEntry> rangeQuery = new TableQuery<DataEntry>().Where(
                                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal,
                                  "sum" + username)

                              );
                                long lastTick = 0;
                                foreach (DataEntry entity in await inputTable.ExecuteQuerySegmentedAsync(rangeQuery, null))
                                {

                                    if (entity.Timestamp.Ticks > lastTick)
                                    {
                                        answer = entity.value.ToString();
                                        lastTick = entity.Timestamp.Ticks;

                                    }
                                }

                                break;
                        }

                    }
             
            }
            catch (Exception e)
            {
                log.LogInformation(e.Message);



            }
            return username != null
                    ? (ActionResult)new OkObjectResult("Request Recieved username:" + username + ",cmd:" + cmd + ",value:" + value + " output answer =" + answer)
                    : new BadRequestObjectResult("Please pass a username,cmd and value on the query string or in the request body");



          
}
