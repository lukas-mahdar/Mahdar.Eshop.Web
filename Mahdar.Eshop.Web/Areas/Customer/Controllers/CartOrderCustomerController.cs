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
        private readonly EshopDbContext _context;
        ISecurityApplicationService iSecure;


        public CartOrderCustomerController(ISecurityApplicationService iSecure, EshopDbContext context)
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

                    IList<Cart> userCart = await this._context.Carts
                                                                     .Where(or => or.UserId == currentUser.Id)
                                                                     .Include(o => o.User)
                                                                     .Include(o => o.CartItems)
                                                                     .ThenInclude(oi => oi.Product)
                                                                     .ToListAsync();
                    foreach (Cart cart in userCart)
                    {
                        if (cart.TotalPrice == 0)
                        {
                            _context.Remove(cart);
                            await _context.SaveChangesAsync();
                            return RedirectToAction(nameof(Index));
                        }
                    }
                    return View(userCart);
                }
            }
            return View();
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartItem = await _context.CartItems
                .Include(o => o.Cart)
                .Include(o => o.Product)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (cartItem == null)
            {
                return NotFound();
            }

            return View(cartItem);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            User currentUser = await iSecure.GetCurrentUser(User);
            Cart userOrders = _context.Carts.Where(or => or.UserId == currentUser.Id).FirstOrDefault();

            var cartItem = await _context.CartItems.FindAsync(id);
            userOrders.TotalPrice -= cartItem.Price;

            _context.Update(userOrders);
            _context.CartItems.Remove(cartItem);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Sub(int id)
        {
            User currentUser = await iSecure.GetCurrentUser(User);

            Cart userOrders = _context.Carts.Where(or => or.UserId == currentUser.Id).FirstOrDefault();
            var cartItem = await _context.CartItems.FindAsync(id);

            if (cartItem == null)
            {
                return RedirectToAction(nameof(Index));
            }
            if (userOrders == null)
            {
                return NotFound();
            }
            var amount = cartItem.Amount;
            --cartItem.Amount;
            if (cartItem.Amount == 0)
            {
                _context.CartItems.Remove(cartItem);
                userOrders.TotalPrice -= cartItem.Price;
                _context.Update(userOrders);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            userOrders.TotalPrice -= cartItem.Price;
            cartItem.Price -= cartItem.Price / amount;
            userOrders.TotalPrice += cartItem.Price;

            _context.Update(cartItem);
            _context.Update(userOrders);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Add(int id)
        {
            User currentUser = await iSecure.GetCurrentUser(User);


            Cart userOrders = _context.Carts.Where(or => or.UserId == currentUser.Id).FirstOrDefault();
            var cartItem = await _context.CartItems.FindAsync(id);

            if (userOrders == null)
            {
                return NotFound();
            }

            if (cartItem == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var amount = cartItem.Amount;
            ++cartItem.Amount;
            userOrders.TotalPrice -= cartItem.Price;
            cartItem.Price = cartItem.Amount * (cartItem.Price / amount);
            userOrders.TotalPrice += cartItem.Price;

            _context.Update(cartItem);
            _context.Update(userOrders);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteCart(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartItem = await _context.Carts
                .FirstOrDefaultAsync(m => m.ID == id);

            if (cartItem == null)
            {
                return NotFound();
            }

            return View(cartItem);
        }

        [HttpPost, ActionName("DeleteCart")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCartConfirmed(int id)
        {
            User currentUser = await iSecure.GetCurrentUser(User);
            Cart userOrders = _context.Carts.Where(or => or.UserId == currentUser.Id).FirstOrDefault();

            _context.Carts.Remove(userOrders);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
