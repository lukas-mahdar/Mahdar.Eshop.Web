using Mahdar.Eshop.Web.Models;
using Mahdar.Eshop.Web.Models.Database;
using Mahdar.Eshop.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Mahdar.Eshop.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        readonly EshopDbContext eshopDbContext;

        public HomeController(EshopDbContext eshopDb, ILogger<HomeController> logger)
        {
            _logger = logger;
            eshopDbContext = eshopDb;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Nacitanie Home Index");
            IndexViewModel indexVM = new IndexViewModel();
            indexVM.CarouselItems = DatabaseFake.CarouselItems;
            indexVM.ProductItems = DatabaseFake.ProductItems;

            return View(indexVM);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
