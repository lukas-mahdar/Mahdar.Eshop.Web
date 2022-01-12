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
        const string totalcartPriceString = "CartPrice";
        const string cartItemsString = "CartItems";
        const string totalorderPriceString = "OrderPrice";
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
                totalPrice = HttpContext.Session.GetDouble(totalcartPriceString).GetValueOrDefault();
            }

            Product product = EshopDbContext.ProductItems.Where(prod => prod.ID == productId).FirstOrDefault();

            if (product != null)
            {
                OrderItem orderItem = new OrderItem()
                {
                    ProductID = product.ID,
                    Product = product,
                    Amount = 1,
                    Price = product.Price   //zde pozor na datový typ -> pokud máte Price v obou případech double nebo decimal, tak je to OK. Mě se bohužel povedlo mít to jednou jako decimal a jednou jako double. Nejlepší je datový typ změnit v databázi/třídě, tak to prosím udělejte.
                };
                CartItem cartItem = new CartItem()
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
                        cartItemInSession = cartItems.Find(oi => oi.ProductID == orderItem.ProductID);
                    else
                        cartItems = new List<CartItem>();


                    if (cartItemInSession != null)
                    {
                        ++cartItemInSession.Amount;
                        cartItemInSession.Price += orderItem.Product.Price;   //zde pozor na datový typ -> pokud máte Price v obou případech double nebo decimal, tak je to OK. Mě se bohužel povedlo mít to jednou jako decimal a jednou jako double. Nejlepší je datový typ změnit v databázi/třídě, tak to prosím udělejte.
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
                    HttpContext.Session.SetObject(orderItemsString, cartItems);

                    totalPrice += cartItem.Product.Price;

                    HttpContext.Session.SetDouble(totalorderPriceString, totalPrice);
                    HttpContext.Session.SetObject(totalcartPriceString, totalPrice);
                }
            }
            return totalPrice;
        }


    }
}
