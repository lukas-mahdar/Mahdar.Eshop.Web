using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mahdar.Eshop.Web.Models.ApplicationServices.Abstraction;
using Mahdar.Eshop.Web.Models.Database;
using Mahdar.Eshop.Web.Models.Entity;
using Mahdar.Eshop.Web.Models.Identity;

namespace Mahdar.Eshop.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = nameof(Roles.Customer))]
    public class CartOrderCustomerController : Controller
    {

        ISecurityApplicationService iSecure;
        private readonly EshopDbContext EshopDbContext;

        public CartOrderCustomerController(ISecurityApplicationService iSecure, EshopDbContext eshopDBContext)
        {
            this.iSecure = iSecure;
            EshopDbContext = eshopDBContext;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                User currentUser = await iSecure.GetCurrentUser(User);
                if (currentUser != null)
                {
                    IList<Cart> userCart = await this.EshopDbContext.Carts
                                                                        .Where(or => or.CartNumber == currentUser.Id)
                                                                        .Include(o => o.User)
                                                                        .Include(o => o.CartItems)
                                                                        .ThenInclude(oi => oi.Product)
                                                                        .ToListAsync();
                    return View(userCart);
                }
            }
            return View();
        }

        public async Task<IActionResult> Delete(int id)
        {
            var cartI = await EshopDbContext.CartItems
                .Include(o => o.Product)
                .Include(o => o.Cart)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (cartI == null)
            {
                return NotFound();
            }

            User currentUser = await iSecure.GetCurrentUser(User);
            Cart userOrders = EshopDbContext.Carts.Where(or => or.CartNumber == currentUser.Id).FirstOrDefault();

            var cartItem = await EshopDbContext.CartItems.FindAsync(id);
            userOrders.TotalPrice -= cartItem.Price;

            EshopDbContext.Update(userOrders);
            EshopDbContext.CartItems.Remove(cartItem);

            await EshopDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Sub(int id)
        {
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Add(int id)
        {
            return RedirectToAction(nameof(Index));
        }


    }
}
