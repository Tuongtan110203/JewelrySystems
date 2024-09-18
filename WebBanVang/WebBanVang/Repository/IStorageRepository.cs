namespace WebBanVang.Repository
{
    public interface IStorageRepository
    {
        Task<string> UploadImageAsync(Stream imageStream, string containerName, string folderName, string fileName);

    }
}
