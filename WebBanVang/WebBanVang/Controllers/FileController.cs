//using Microsoft.AspNetCore.Cors;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using WebBanVang.Models.Domain;
//using WebBanVang.Repository;

//namespace WebBanVang.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    [EnableCors("AllowLocalhost3000And15723035249")]

//    public class FileController : ControllerBase
//    {
//        private readonly IFileRepository fileRepository;

//        public FileController(IFileRepository fileRepository)
//        {
//            this.fileRepository = fileRepository;
//        }
//        [HttpPost]
//        [Route("Upload")]
//        public async Task<IActionResult> Upload([FromForm] Files files)
//        {
//            await fileRepository.Upload(files);
//            return Ok("Success");
//        }
//        [HttpGet]
//        [Route("GetImage")]
//        public async Task<IActionResult> GetImage(string name)
//        {
//            var imageFileStream = await fileRepository.Get(name);
//            string fileType = "jpg";
//            if (name.Contains("png"))
//            {
//                fileType = "png";
//            }
//            return File(imageFileStream, $"image/{fileType}");

//        }
//        [HttpGet]
//        [Route("GetLink")]
//        public async Task<IActionResult> GetLink(string name)
//        {
//            var container = fileRepository.GetBlobContainerClient("storageimageazure");
//            var blob = container.GetBlobClient(name);
//            var url = blob.Uri.AbsoluteUri;
//            return Ok(new { Url = url });
//        }


//        [HttpGet]
//        [Route("DownLoad")]
//        public async Task<IActionResult> DownLoad(string name)
//        {
//            var imageFileStream = await fileRepository.Get(name);
//            string fileType = "jpg";
//            if (name.Contains("png"))
//            {
//                fileType = "png";
//            }
//            return File(imageFileStream, $"image/{fileType}", $"blobfile.{fileType}");
//        }
//    }
//}
