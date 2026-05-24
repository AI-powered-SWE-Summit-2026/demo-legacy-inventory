using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LegacyInventory.Core.Models
{
    public class Warehouse
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        [StringLength(250)]
        public string Location { get; set; }

        public int Capacity { get; set; }

        [StringLength(100)]
        public string ManagerName { get; set; }

        public bool IsActive { get; set; }

        public virtual ICollection<StockLevel> StockLevels { get; set; } = new List<StockLevel>();
    }
}
