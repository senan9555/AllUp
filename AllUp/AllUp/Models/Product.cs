using System.ComponentModel.DataAnnotations.Schema;

namespace AllUp.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public int Rate { get; set; }
        public List<ProductImage> ProductImages { get; set; }
        public ProductDetail ProductDetail { get; set; }
        public virtual ICollection<ProductCategory> ProductCategories { get; set; }

        [NotMapped]
        public IFormFile[] Photos { get; set; }
    }
}
