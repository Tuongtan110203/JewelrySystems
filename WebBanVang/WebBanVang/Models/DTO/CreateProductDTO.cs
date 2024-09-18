using WebBanVang.Models.Domain;

namespace WebBanVang.Models.DTO
{
    public class CreateProductDTO : AddProductBlob

    {
        public List<IFormFile>? Images { get; set; }

    }
}
