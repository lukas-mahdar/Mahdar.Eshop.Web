﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mahdar.Eshop.Web.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Detail(int ID)
        {
            return View();
        }
    }
}
