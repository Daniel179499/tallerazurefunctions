using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using tallerazurefunctions.Functions.Entities;

namespace tallerazurefunctions.Functions.Functions
{
    public static class SchedulerFunction
    {
        [FunctionName("SchedulerFunction")]
        public static async Task Run([TimerTrigger("0 */2 * * * *")]TimerInfo myTimer,
        [Table("tiempo", Connection = "AzureWebJobsStorage")] CloudTable tiempoTable,
        ILogger log)
        {
            log.LogInformation($"Deleting completed function executed ad: {DateTime.Now}");
            string filter = TableQuery.GenerateFilterConditionForBool("Consolidated", QueryComparisons.Equal, true);
            TableQuery<TiempoEntity> query = new TableQuery<TiempoEntity>().Where(filter);
            TableQuerySegment<TiempoEntity> completedTiempos = await tiempoTable.ExecuteQuerySegmentedAsync(query, null);
            int deleted = 0;
            foreach(TiempoEntity completedTiempo in completedTiempos)
            {
                await tiempoTable.ExecuteAsync(TableOperation.Delete(completedTiempo));
                deleted++;
            }

            log.LogInformation($"Deleted: {deleted}, items at {DateTime.Now}");

        }
    }
}
