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
        // Create a BlobClient to interact with the blob storage
        BlobClient blobClient = createBlobClient();

        // Check if the blob exists
        if (!await blobClient.ExistsAsync())
        {
            // If the blob does not exist, upload an empty stream to create it
            await blobClient.UploadAsync(new MemoryStream(), true);
        }

        // Read the existing data from the blob
        using (MemoryStream existingStream = new MemoryStream())
        {
            await blobClient.DownloadToAsync(existingStream);
            string existingData = Encoding.UTF8.GetString(existingStream.ToArray());

            // Read the new data from the request body
            string newData = await new StreamReader(req.Body).ReadToEndAsync();

            // Concatenate the existing data with the new data
            string updatedData = existingData + "\n" + newData;

            // Upload the updated data to the blob
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
            // Download the data from the blob
            using (MemoryStream stream = new MemoryStream())
            {
                await blobClient.DownloadToAsync(stream);

                // Convert the stream to a string
                string data = Encoding.UTF8.GetString(stream.ToArray());

                return new OkObjectResult(data);
            }
        }

        return new NotFoundResult();
    }

    // Helper function to create a BlobClient
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