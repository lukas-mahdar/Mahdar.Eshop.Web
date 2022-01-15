using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Mahdar.Eshop.Web.Models.Database;
using Mahdar.Eshop.Web.Models.Identity;
using Mahdar.Eshop.Web.Models.Entity;

namespace Mahdar.Eshop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = nameof(Roles.Admin) + ", " + nameof(Roles.Manager))]
    public class CartsController : Controller
    {
        private readonly EshopDbContext _context;

        public CartsController(EshopDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Carts
        public async Task<IActionResult> Index()
        {

            IList<Cart> userCarts = await this._context.Carts
                                                 .Include(o => o.User)
                                                 .Include(o => o.CartItems)
                                                 .ThenInclude(oi => oi.Product)
                                                 .ToListAsync();
            return View(userCarts);
        }

    }
}
