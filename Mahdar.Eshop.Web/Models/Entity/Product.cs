using Mahdar.Eshop.Web.Models.Validations;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Mahdar.Eshop.Web.Models.Entity
{
    [Table(nameof(Product))]
    public class Product
    {
        [Key]
        [Required]
        public int ID { get; set; }

        [StringLength(64)]
        [Required]
        public string Name { get; set; }

        [StringLength(255)]
        [Required]
        public string Description { get; set; }
        public double Price { get; set; }

        [NotMapped]
        [FileContentValidation("image")]
        public IFormFile Image { get; set; }

        [NotMapped]
        [FileContentValidation("image")]
        public IFormFile Image2 { get; set; }

        [StringLength(255)]
        [Required]
        public string ImageSource450x300 { get; set; }

        [StringLength(255)]
        [Required]
        public string ImageSource600x700 { get; set; }

    }
}
