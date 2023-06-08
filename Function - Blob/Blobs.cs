using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text;

public static class Blobs
{
    [FunctionName("SendData")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
        ILogger log)
    {
        BlobClient blobClient = createBlobClient();

        if (!await blobClient.ExistsAsync())
        {
            await blobClient.UploadAsync(new MemoryStream(), true);
        }

        using (MemoryStream existingStream = new MemoryStream())
        {
            await blobClient.DownloadToAsync(existingStream);
            string existingData = Encoding.UTF8.GetString(existingStream.ToArray());

            string newData = await new StreamReader(req.Body).ReadToEndAsync();

            string updatedData = existingData + "\n" + newData;

            using (MemoryStream updatedStream = new MemoryStream(Encoding.UTF8.GetBytes(updatedData)))
            {
                await blobClient.UploadAsync(updatedStream, true);
            }
        }

        return new OkResult();
    }


    [FunctionName("GetData")]
    public static async Task<IActionResult> GetData(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
    ILogger log)
    {
        BlobClient blobClient = createBlobClient();

        if (await blobClient.ExistsAsync())
        {
            using (MemoryStream stream = new MemoryStream())
            {
                await blobClient.DownloadToAsync(stream);

                string data = Encoding.UTF8.GetString(stream.ToArray());

                return new OkObjectResult(data);
            }
        }

        return new NotFoundResult();
    }

    private static BlobClient createBlobClient()
    {
        string connectionString = "DefaultEndpointsProtocol=https;AccountName=function20230529160724st;AccountKey=MIu1/9Pgc4Yv/KQkGeJY+xjZ0aYhZ65JDtHwLpxqQXx0hYViN4+HKL4bpBLIlmD9Wt5+f+VFX93P+AStr/jMOA==;EndpointSuffix=core.windows.net";
        string containerName = "test";
        string blobName = "windData";

        BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        BlobClient blobClient = containerClient.GetBlobClient(blobName);

        return blobClient;
    }
}