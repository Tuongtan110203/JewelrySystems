namespace WebBanVang.Models.DTO
{
    public class UpdateStoreInfo
    {
        public IFormFile? Avatar { get; set; }
        public IFormFile? LogoFile { get; set; }
        public string? Slogan { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string? NumberPhone { get; set; }
        public string? TaxNumber { get; set; }
        public string? Footer { get; set; }
    }
}
