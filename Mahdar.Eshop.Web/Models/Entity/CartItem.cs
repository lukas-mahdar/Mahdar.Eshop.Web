using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Mahdar.Eshop.Web.Models.Entity
{
    [Table(nameof(CartItem))]
    public class CartItem
    {

        [Key]
        [Required]
        public int ID { get; set; }

        [ForeignKey(nameof(Cart))]
        public int CartID { get; set; }

        [ForeignKey(nameof(Product))]
        public int ProductID { get; set; }

        public int Amount { get; set; }
        public double Price { get; set; }

        public Cart Cart { get; set; }
        public Product Product { get; set; }
    }
}
