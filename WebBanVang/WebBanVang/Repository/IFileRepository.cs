using Azure.Storage.Blobs;
using WebBanVang.Models.Domain;

namespace WebBanVang.Repository
{
    public interface IFileRepository
    {
        Task Upload(Files files);
        Task<Stream> Get(string name);
        BlobContainerClient GetBlobContainerClient(string containerName);

    }
}
