using System.ComponentModel.DataAnnotations;

namespace Forms.Models
{
    public class Product
    {
        [Display(Name = "Urun Id")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Gerekli alan!")]
        [Display(Name = "Urun Adi")]
        public string Name { get; set; } =null!;

        [Required(ErrorMessage = "Gerekli alan!")]
        [Range(0, 100000)]
        [Display(Name = "Fiyat")]
        public decimal? Price { get; set; }
        [Display(Name = "Resim")]
        public string? Image { get; set; }
        public bool IsActive { get; set; }

        [Required(ErrorMessage = "Gerekli alan!")]
        [Display(Name = "Kategori")]
        public int CategoryId { get; set; }
    }
}