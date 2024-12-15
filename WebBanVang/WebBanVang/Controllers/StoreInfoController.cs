using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.Json;
using Newtonsoft.Json.Linq;
using WebBanVang.Models.Domain;
using WebBanVang.Models.DTO;

namespace WebBanVang.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowLocalhost3000And15723035249")]
    //[AllowAnonymous]
    public class StoreInfoController : ControllerBase
    {
        private readonly IConfigurationRoot _configurationRoot;
        private readonly string _jsonFilePath;
        private readonly string _blobConnectionString;

        public StoreInfoController(IConfiguration configuration)
        {
            _configurationRoot = (IConfigurationRoot)configuration;
            _jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            _blobConnectionString = configuration.GetConnectionString("AzureBlobStore");
        }

        [HttpGet]
        public IActionResult GetStoreInfo()
        {
            var storeInfo = new StoreInfo
            {
                Avatar = _configurationRoot["StoreInfo:Avatar"],
                Logo = _configurationRoot["StoreInfo:Logo"],
                Slogan = _configurationRoot["StoreInfo:Slogan"],
                Address = _configurationRoot["StoreInfo:Address"],
                Email = _configurationRoot["StoreInfo:Email"],
                NumberPhone = _configurationRoot["StoreInfo:NumberPhone"],
                TaxNumber = _configurationRoot["StoreInfo:TaxNumber"],
                Footer = _configurationRoot["StoreInfo:Footer"]
            };

            return Ok(storeInfo);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateStoreInfo([FromForm] UpdateStoreInfo storeInfo)
        {
            string logoUrl = _configurationRoot["StoreInfo:Logo"];
            string avatarUrl = _configurationRoot["StoreInfo:Avatar"];

            var blobServiceClient = new BlobServiceClient(_blobConnectionString);

            // Xử lý tải lên logo mới
            if (storeInfo.LogoFile != null)
            {
                var logoContainerClient = blobServiceClient.GetBlobContainerClient("logo");
                var logoBlobClient = logoContainerClient.GetBlobClient(storeInfo.LogoFile.FileName);

                // Xóa logo cũ nếu có
                await DeleteBlobIfExistsAsync(logoContainerClient, Path.GetFileName(logoUrl));

                await using (var stream = storeInfo.LogoFile.OpenReadStream())
                {
                    await logoBlobClient.UploadAsync(stream, overwrite: true);
                }

                logoUrl = logoBlobClient.Uri.ToString();
            }

            // Xử lý tải lên avatar mới
            if (storeInfo.Avatar != null)
            {
                var avatarContainerClient = blobServiceClient.GetBlobContainerClient("avatar");
                var avatarBlobClient = avatarContainerClient.GetBlobClient(storeInfo.Avatar.FileName);

                // Xóa avatar cũ nếu có
                await DeleteBlobIfExistsAsync(avatarContainerClient, Path.GetFileName(avatarUrl));

                await using (var stream = storeInfo.Avatar.OpenReadStream())
                {
                    await avatarBlobClient.UploadAsync(stream, overwrite: true);
                }

                avatarUrl = avatarBlobClient.Uri.ToString();
            }

            // Đọc và cập nhật tệp cấu hình
            var json = await System.IO.File.ReadAllTextAsync(_jsonFilePath);
            var jsonObj = JObject.Parse(json);

            jsonObj["StoreInfo"]["Avatar"] = avatarUrl;
            jsonObj["StoreInfo"]["Logo"] = logoUrl;
            jsonObj["StoreInfo"]["Slogan"] = storeInfo.Slogan;
            jsonObj["StoreInfo"]["Address"] = storeInfo.Address;
            jsonObj["StoreInfo"]["Email"] = storeInfo.Email;
            jsonObj["StoreInfo"]["NumberPhone"] = storeInfo.NumberPhone;
            jsonObj["StoreInfo"]["TaxNumber"] = storeInfo.TaxNumber;
            jsonObj["StoreInfo"]["Footer"] = storeInfo.Footer;

            string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
            await System.IO.File.WriteAllTextAsync(_jsonFilePath, output);

            ReloadConfiguration();

            return Ok(new { message = "Store information updated successfully" });
        }

        private async Task DeleteBlobIfExistsAsync(BlobContainerClient containerClient, string blobName)
        {
            var blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.DeleteIfExistsAsync();
        }



        private void ReloadConfiguration()
        {
            foreach (var provider in _configurationRoot.Providers)
            {
                if (provider is JsonConfigurationProvider jsonProvider)
                {
                    jsonProvider.Load();
                }
            }
        }



    }
}
