using Azure.Storage.Blobs;
using WebBanVang.Models.Domain;

namespace WebBanVang.Repository
{
    public class FileRepository : IFileRepository
    {
        private readonly BlobServiceClient blobServiceClient;

        public FileRepository(BlobServiceClient blobServiceClient)
        {
            this.blobServiceClient = blobServiceClient;
        }
        public async Task Upload(Files files)
        {
            var Container = blobServiceClient.GetBlobContainerClient("storageimageazure");
            var Blob = Container.GetBlobClient(files.ImageFile.FileName);
            await Blob.UploadAsync(files.ImageFile.OpenReadStream());
        }
        public async Task<Stream> Get(String name)
        {
            var Container = blobServiceClient.GetBlobContainerClient("storageimageazure");
            var Blob = Container.GetBlobClient(name);
            var DownLoad = await Blob.DownloadAsync();
            return DownLoad.Value.Content;
        }

        public BlobContainerClient GetBlobContainerClient(string containerName)
        {
            return blobServiceClient.GetBlobContainerClient(containerName);
        }
    }
}
