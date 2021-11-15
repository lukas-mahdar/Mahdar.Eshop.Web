using Mahdar.Eshop.Web.Models.Database;
using Mahdar.Eshop.Web.Models.Entity;
using Mahdar.Eshop.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mahdar.Eshop.Web.Controllers
{
    
    public class CarouselController : Controller
    {
        readonly EshopDbContext eshopDbContext;

        public CarouselController(EshopDbContext eshopDb)
        {
            eshopDbContext = eshopDb;
        }

        public IActionResult Select()
        {
            IndexViewModel indexVM = new IndexViewModel();
            indexVM.CarouselItems = eshopDbContext.CarouselItems.ToList();

            return View(indexVM);            
        }
        public async Task<IActionResult> Create(CarouselItem carouselItem)
        {
            if (String.IsNullOrWhiteSpace(carouselItem.ImageSource) == false
                && String.IsNullOrWhiteSpace(carouselItem.ImageAlt) == false)
            {
                //if (DatabaseFake.CarouselItems != null && DatabaseFake.CarouselItems.Count > 0)
                //{
                //    carouselItem.ID = DatabaseFake.CarouselItems.Last().ID + 1;
                //}

                eshopDbContext.CarouselItems.Add(carouselItem);
                await eshopDbContext.SaveChangesAsync();


                return RedirectToAction(nameof(CarouselController.Select));
            }
            else
            {
                return View(carouselItem);
            }
        }
        public IActionResult Edit(int ID)
        {
            return View();
        }
        public async Task<IActionResult> Delete(int ID)
        {
            CarouselItem carouselItem = eshopDbContext.CarouselItems
                                                    .Where(ci => ci.ID == ID)
                                                    .FirstOrDefault();
            if (carouselItem != null)
            {
                eshopDbContext.CarouselItems.Remove(carouselItem);
                await eshopDbContext.SaveChangesAsync();
            }

            return RedirectToAction(nameof(CarouselController.Select));
        }
    }
}
