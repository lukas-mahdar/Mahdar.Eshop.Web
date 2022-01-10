using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Mahdar.Eshop.Web.Models.Identity;
using Mahdar.Eshop.Web.Models.Entity;

namespace Mahdar.Eshop.Web.Models.Entity
{
    [Table(nameof(Cart))]
    public class Cart
    {

        [Key]
        [Required]
        public int ID { get; set; }

        [ForeignKey(nameof(User))]
        [Required]
        public int CartNumber { get; set; }

        [Required]
        public double TotalPrice { get; set; }

        public User User { get; set; }

        //[ForeignKey(nameof(OrderStatus))]
        //public int OrderStatusId { get; set; }
        //public OrderStatus OrderStatus { get; set; }

        public IList<CartItem> CartItems { get; set; }

    }
}
