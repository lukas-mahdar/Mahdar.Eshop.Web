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
    public class ItemsCustomerController : Controller
    {
        const string cartPriceString = "CartPrice";
        const string orderPriceString = "OrderPrice";
        const string cartItemsString = "CartItems";
        const string orderItemsString = "OrderItems";

        ISecurityApplicationService iSecure;
        EshopDbContext EshopDbContext;
        public ItemsCustomerController(ISecurityApplicationService iSecure, EshopDbContext eshopDBContext)
        {
            this.iSecure = iSecure;
            EshopDbContext = eshopDBContext;
        }

        [HttpPost]
        public double AddItemsToSession(int? productId)
        {
            double totalPrice = 0;

            if (HttpContext.Session.IsAvailable)
            {
                totalPrice = HttpContext.Session.GetDouble(cartPriceString).GetValueOrDefault();
            }

            Product product = EshopDbContext.ProductItems.Where(prod => prod.ID == productId).FirstOrDefault();

            if (product != null)
            {
                CartItem cartItem = new CartItem()
                {
                    ProductID = product.ID,
                    Product = product,
                    Amount = 1,
                    Price = product.Price   //zde pozor na datový typ -> pokud máte Price v obou případech double nebo decimal, tak je to OK. Mě se bohužel povedlo mít to jednou jako decimal a jednou jako double. Nejlepší je datový typ změnit v databázi/třídě, tak to prosím udělejte.
                };

                OrderItem orderItem = new OrderItem()
                {
                    ProductID = product.ID,
                    Product = product,
                    Amount = 1,
                    Price = product.Price   //zde pozor na datový typ -> pokud máte Price v obou případech double nebo decimal, tak je to OK. Mě se bohužel povedlo mít to jednou jako decimal a jednou jako double. Nejlepší je datový typ změnit v databázi/třídě, tak to prosím udělejte.
                };

                if (HttpContext.Session.IsAvailable)
                {

                    List<CartItem> cartItems = HttpContext.Session.GetObject<List<CartItem>>(cartItemsString);
                    CartItem cartItemInSession = null;
                    if (cartItems != null)
                        cartItemInSession = cartItems.Find(oi => oi.ProductID == cartItem.ProductID);
                    else
                        cartItems = new List<CartItem>();

                    if (cartItemInSession != null)
                    {
                        ++cartItemInSession.Amount;
                        cartItemInSession.Price += cartItem.Product.Price;   //zde pozor na datový typ -> pokud máte Price v obou případech double nebo decimal, tak je to OK. Mě se bohužel povedlo mít to jednou jako decimal a jednou jako double. Nejlepší je datový typ změnit v databázi/třídě, tak to prosím udělejte.
                    }
                    else
                    {
                        cartItems.Add(cartItem);
                    }

                    List<OrderItem> orderItems = HttpContext.Session.GetObject<List<OrderItem>>(orderItemsString);
                    OrderItem orderItemInSession = null;
                    if (orderItems != null)
                        orderItemInSession = orderItems.Find(oi => oi.ProductID == orderItem.ProductID);
                    else
                        orderItems = new List<OrderItem>();


                    if (orderItemInSession != null)
                    {
                        ++orderItemInSession.Amount;
                        orderItemInSession.Price += orderItem.Product.Price;   //zde pozor na datový typ -> pokud máte Price v obou případech double nebo decimal, tak je to OK. Mě se bohužel povedlo mít to jednou jako decimal a jednou jako double. Nejlepší je datový typ změnit v databázi/třídě, tak to prosím udělejte.
                    }
                    else
                    {
                        orderItems.Add(orderItem);
                    }

                    HttpContext.Session.SetObject(orderItemsString, orderItems);
                    HttpContext.Session.SetObject(cartItemsString, cartItems);

                    totalPrice += cartItem.Product.Price;

                    HttpContext.Session.SetDouble(orderPriceString, totalPrice);
                    HttpContext.Session.SetDouble(cartPriceString, totalPrice);
                }
            }

            return totalPrice;
        }
        public async Task<IActionResult> SaveCart()
        {
            User currentUser = await iSecure.GetCurrentUser(User);
            Cart userOrdes = EshopDbContext.Carts.Where(or => or.CartNumber == currentUser.Id).FirstOrDefault();

            if (HttpContext.Session.IsAvailable && userOrdes == null)
            {
                double totalPrice = 0;
                List<CartItem> orderItems = HttpContext.Session.GetObject<List<CartItem>>(cartItemsString);
                if (orderItems != null)
                {
                    foreach (CartItem orderItem in orderItems)
                    {
                        totalPrice += orderItem.Product.Price * orderItem.Amount;
                        orderItem.Product = null;
                    }

                    Cart order = new Cart()
                    {
                        CartNumber = currentUser.Id,
                        TotalPrice = totalPrice,
                        CartItems = orderItems,
                        UserId = currentUser.Id
                    };
                    await EshopDbContext.AddAsync(order);
                    await EshopDbContext.SaveChangesAsync();

                    HttpContext.Session.Remove(cartItemsString);
                    HttpContext.Session.Remove(cartPriceString);
                    return RedirectToAction(nameof(CartCustomerController.Index), nameof(CartOrderCustomerController).Replace("Controller", ""), new { Area = nameof(Customer) });
                }
            }
            else
            {
                double totalPrice = userOrdes.TotalPrice;
                List<CartItem> orderItems = HttpContext.Session.GetObject<List<CartItem>>(cartItemsString);

                if (orderItems != null)
                {
                    foreach (CartItem orderItem in orderItems)
                    {
                        var userItems = EshopDbContext.CartItems.Where(ui => ui.CartID == userOrdes.ID)
                                                                .FirstOrDefault(ui => ui.ProductID == orderItem.ProductID);
                        totalPrice += orderItem.Product.Price * orderItem.Amount;
                        if (userItems != null)
                        { 
                            orderItem.ID = userOrdes.ID;
                            orderItem.Amount += userItems.Amount;
                            EshopDbContext.Remove(userItems);
                        }
                        orderItem.Price = orderItem.Amount * orderItem.Product.Price;
                        orderItem.Product = null;
                    }
                    userOrdes.TotalPrice = totalPrice;
                    userOrdes.CartItems = orderItems;

                    EshopDbContext.Update(userOrdes);
                    await EshopDbContext.SaveChangesAsync();

                    HttpContext.Session.Remove(cartItemsString);
                    HttpContext.Session.Remove(cartPriceString);
                    return RedirectToAction(nameof(CartCustomerController.Index), nameof(CartOrderCustomerController).Replace("Controller", ""), new { Area = nameof(Customer) });
                }
            }
            return RedirectToAction(nameof(HomeController.Index), nameof(HomeController).Replace("Controller", ""), new { Area = ""});
        }

        [HttpPost]
        public double ItemsSubSession(int? productId)
        {
            double totalPrice = 0;
            if (HttpContext.Session.IsAvailable)
            { 
                totalPrice = HttpContext.Session.GetDouble(cartPriceString).GetValueOrDefault();
            }
            Product productItem = EshopDbContext.ProductItems.Where(pi => pi.ID == productId).FirstOrDefault();

            if (productItem != null)
            {
                CartItem cartItem = new CartItem()
                {
                    ProductID = productItem.ID,
                    Product = productItem,
                    Amount = 1,
                    Price = productItem.Price                    
                };

                if (HttpContext.Session.IsAvailable)
                {
                    List<CartItem> cartItems = HttpContext.Session.GetObject<List<CartItem>>(cartItemsString);
                    CartItem cart_itemSession = null;
                    if (cartItems != null)
                    {
                        cart_itemSession = cartItems.Find(ci => ci.ProductID == cartItem.ProductID);
                    }
                    else
                    {
                        cartItems = new List<CartItem>();
                    }

                    if (cart_itemSession != null)
                    {
                        if (cart_itemSession.Amount >= 1)
                        {
                            --cart_itemSession.Amount;
                            cart_itemSession.Price -= cartItem.Product.Price;
                        }
                        if (cart_itemSession.Amount == 0)
                        {
                            cartItems.Remove(cart_itemSession);
                        }
                        totalPrice -= cartItem.Product.Price;
                    }
                    HttpContext.Session.SetObject(cartItemsString, cartItems);
                    HttpContext.Session.SetDouble(cartPriceString, totalPrice);
                }
            }
            return totalPrice;
        }
    }
}
