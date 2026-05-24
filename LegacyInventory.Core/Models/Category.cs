using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LegacyInventory.Core.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        public int? ParentCategoryId { get; set; }

        public virtual Category ParentCategory { get; set; }

        public virtual ICollection<Category> ChildCategories { get; set; } = new List<Category>();

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
