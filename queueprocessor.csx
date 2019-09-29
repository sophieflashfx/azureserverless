#r "Microsoft.WindowsAzure.Storage"
using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;

       public class DataEntry: TableEntity
        {
          

            public double value { get; set; }


        }
public static async Task Run(  CloudTable inputTable,ICollector<DataEntry> outputTable,string myQueueItem, ILogger log)
{
    log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        //query for last total value

            TableQuery<DataEntry> rangeQuery = new TableQuery<DataEntry>().Where(
                          TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal,
                        "sum" + myQueueItem)

                    );
            long lastTick = 0;
            double lastValue = 0;
            foreach (DataEntry entity in await inputTable.ExecuteQuerySegmentedAsync(rangeQuery, null))
            {

                if (entity.Timestamp.Ticks > lastTick)
                {
                    lastTick = entity.Timestamp.Ticks;
                    lastValue = entity.value;
                }
            }

            TableQuery<DataEntry> rangeQuery2 = new TableQuery<DataEntry>().Where(
                            TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal,
                                    myQueueItem)

                                    );

            double A = lastValue;

            // Execute the query and loop through the results
            foreach (DataEntry entity in
                await inputTable.ExecuteQuerySegmentedAsync(rangeQuery2, null))
            {
                if (entity.Timestamp.Ticks > lastTick)
                {
                    A = A + entity.value;
                }
                //  log.LogInformation(entity.RowKey.ToString());
            }
            log.LogInformation(A.ToString());
            outputTable.Add(
                       new DataEntry()
                       {
                           PartitionKey = "sum" + myQueueItem,
                           RowKey = Guid.NewGuid().ToString("N"),
                           value = A
                       }
                       );
}
