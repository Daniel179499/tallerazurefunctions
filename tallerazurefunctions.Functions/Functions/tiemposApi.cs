using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using tallerazurefunctions.Common.Models;
using tallerazurefunctions.Common.Models.Responses;
using tallerazurefunctions.Functions.Entities;

namespace tallerazurefunctions.Functions.Functions
{
    public static class tiemposApi
    {
        [FunctionName(nameof(CreateTiempo))]
        public static async Task<IActionResult> CreateTiempo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "tiempo")] HttpRequest req,
            [Table("tiempo", Connection = "AzureWebJobsStorage")] CloudTable tiempoTable,
            ILogger log)
        {
            log.LogInformation("Recieved a new tiempo.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            tiempo tiempo = JsonConvert.DeserializeObject<tiempo>(requestBody);

            if (string.IsNullOrEmpty(tiempo?.Type))
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "The request must have a type."
                });

            }

            TiempoEntity tiempoEntity = new TiempoEntity
            {
                DateHour = DateTime.UtcNow,
                ETag = "*",
                Consolidated = false,
                PartitionKey = "TIEMPO",
                RowKey = Guid.NewGuid().ToString(),
                Type = tiempo.Type,
                IdEmployee = tiempo.IdEmployee
            };

            TableOperation addOperation = TableOperation.Insert(tiempoEntity);
            await tiempoTable.ExecuteAsync(addOperation);

            string message = "New tiempo stored in table";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = tiempoEntity
            }
            );

        }

        [FunctionName(nameof(UpdateTiempo))]
        public static async Task<IActionResult> UpdateTiempo(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "tiempo/{id}")] HttpRequest req,
        [Table("tiempo", Connection = "AzureWebJobsStorage")] CloudTable tiempoTable,
        string id,
        ILogger log)
        {
            log.LogInformation($"Update for tiempo: {id}, received.");


            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            tiempo tiempo = JsonConvert.DeserializeObject<tiempo>(requestBody);

            //Validate tiempo id
            TableOperation findOperation = TableOperation.Retrieve<TiempoEntity>("TIEMPO", id);
            TableResult findResult = await tiempoTable.ExecuteAsync(findOperation);
            if(findResult==null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "Tiempo no found."
                });
            }

            //update todo
            TiempoEntity tiempoEntity = (TiempoEntity)findResult.Result;
            tiempoEntity.Consolidated = tiempo.Consolidated;

            if(!string.IsNullOrEmpty(tiempo.IdEmployee.ToString()))
            {
                tiempoEntity.IdEmployee = tiempo.IdEmployee;
            }

            TableOperation addOperation = TableOperation.Replace(tiempoEntity);
            await tiempoTable.ExecuteAsync(addOperation);

            string message = $"tiempo: {id}, update in table.";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = tiempoEntity
            }
            );

        }

    }
}
