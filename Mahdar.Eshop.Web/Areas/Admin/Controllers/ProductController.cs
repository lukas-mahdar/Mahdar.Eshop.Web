using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mahdar.Eshop.Web.Models.Database;
using Mahdar.Eshop.Web.Models.Entity;
using Mahdar.Eshop.Web.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using Mahdar.Eshop.Web.Models.Identity;
using Mahdar.Eshop.Web.Models.Implementation;

namespace Mahdar.Eshop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = nameof(Roles.Admin) + ", " + nameof(Roles.Manager))]
    public class ProductController : Controller
    {
        readonly EshopDbContext eshopDbContext;
        IWebHostEnvironment env;

        public ProductController(EshopDbContext eshopDb, IWebHostEnvironment env)
        {
            eshopDbContext = eshopDb;
            this.env = env;
        }

        public IActionResult Select()
        {
            IndexViewModel indexVM = new IndexViewModel();
            indexVM.ProductItems = eshopDbContext.ProductItems.ToList();
            return View(indexVM);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Product productItem)
        {
            FileUpload fileUpload = new FileUpload(env.WebRootPath, "img/Products", "image");

            if ((fileUpload.CheckFileContent(productItem.Image)
               && fileUpload.CheckFileLength(productItem.Image)) && (fileUpload.CheckFileContent(productItem.Image2)
               && fileUpload.CheckFileLength(productItem.Image2)))
            {
                productItem.ImageSource450x300 = await fileUpload.FileUploadAsync(productItem.Image);
                productItem.ImageSource600x700 = await fileUpload.FileUploadAsync(productItem.Image2);

                ModelState.Clear();
                TryValidateModel(productItem);
                if (ModelState.IsValid)
                {
                    eshopDbContext.ProductItems.Add(productItem);

                    await eshopDbContext.SaveChangesAsync();

                    return RedirectToAction(nameof(ProductController.Select));
                }
            }
            return View(productItem);
        }

        public IActionResult Edit(int ID)
        {
            Product productItem = eshopDbContext.ProductItems.FirstOrDefault(ci => ci.ID == ID);
            if (productItem != null)
            {
                return View(productItem);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Product pItem)
        {
            Product productItem = eshopDbContext.ProductItems.FirstOrDefault(ci => ci.ID == pItem.ID);

            if (productItem != null)
            {
                if (pItem.Image != null && pItem.Image2 != null)
                {
                    FileUpload fileUpload = new FileUpload(env.WebRootPath, "img/Products", "image");

                    if ((fileUpload.CheckFileContent(pItem.Image)
                       && fileUpload.CheckFileLength(pItem.Image)) || (fileUpload.CheckFileContent(pItem.Image2)
                       && fileUpload.CheckFileLength(pItem.Image2)))
                    {
                        pItem.ImageSource450x300 = await fileUpload.FileUploadAsync(pItem.Image);
                        productItem.ImageSource450x300 = pItem.ImageSource450x300;
                        pItem.ImageSource600x700 = await fileUpload.FileUploadAsync(pItem.Image2);
                        productItem.ImageSource600x700 = pItem.ImageSource600x700;
                    }
                }
                else
                {
                    pItem.ImageSource450x300 = "-";
                    pItem.ImageSource600x700 = "-";
                }
                ModelState.Clear();
                TryValidateModel(pItem);
                if (ModelState.IsValid)
                {
                    productItem.Name = pItem.Name;
                    productItem.Description = pItem.Description;
                    productItem.Price = pItem.Price;

                    await eshopDbContext.SaveChangesAsync();

                    return RedirectToAction(nameof(ProductController.Select));
                }
            }
            return View(productItem);
        }
        public async Task<IActionResult> Delete(int ID)
        {
            Product productItem = eshopDbContext.ProductItems.Where(ci => ci.ID == ID).FirstOrDefault();

            if (productItem != null)
            {
                eshopDbContext.ProductItems.Remove(productItem);
                await eshopDbContext.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ProductController.Select));
        }
    }
}
