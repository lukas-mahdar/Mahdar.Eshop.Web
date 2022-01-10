using Mahdar.Eshop.Web.Models.Database;
using Mahdar.Eshop.Web.Models.Entity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mahdar.Eshop.Web.Controllers
{
    public class ProductController : Controller
    {
        readonly EshopDbContext eshopDbContext;
        IWebHostEnvironment env;

        public ProductController(EshopDbContext eshopDb, IWebHostEnvironment env)
        {
            this.env = env;
            eshopDbContext = eshopDb;
        }
        public IActionResult Detail(int ID)
        {
            Product productItem = eshopDbContext.ProductItems.FirstOrDefault(ci => ci.ID == ID);

            if (productItem != null)
            {
                return View();
            }
            return NotFound();
        }
    }
}
