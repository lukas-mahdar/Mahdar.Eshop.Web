using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mahdar.Eshop.Web.Controllers;
using Mahdar.Eshop.Web.Models.ApplicationServices.Abstraction;
using Mahdar.Eshop.Web.Models.Database;
using Mahdar.Eshop.Web.Models.Entity;
using Mahdar.Eshop.Web.Models.Identity;

namespace Mahdar.Eshop.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = nameof(Roles.Customer))]
    public class CartCustomerController : Controller
    {
        const string totalPriceString = "TotalPrice";
        const string cartItemsString = "CartItems";


        ISecurityApplicationService iSecure;
        private readonly EshopDbContext _context;
        public CartCustomerController(ISecurityApplicationService iSecure, EshopDbContext context)
        {
            this.iSecure = iSecure;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                User currentUser = await iSecure.GetCurrentUser(User);
                if (currentUser != null)
                {
                    if (HttpContext.Session.IsAvailable)
                    {
                        double totalPrice = 0;
                        List<CartItem> orderItems = HttpContext.Session.GetObject<List<CartItem>>(cartItemsString);
                        List<CartItem> emptyCart = new List<CartItem>();
                        if (orderItems != null)
                        {
                            foreach (CartItem orderItem in orderItems)
                            {
                                totalPrice += orderItem.Product.Price * orderItem.Amount;
                            }

                            return View(orderItems);
                        }
                        else
                        {
                            return View(emptyCart);
                        }
                    }
                }
            }
            return View();
        }
    }
}