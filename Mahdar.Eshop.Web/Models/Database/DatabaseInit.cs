using Mahdar.Eshop.Web.Models.Entity;
using Mahdar.Eshop.Web.Models.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Mahdar.Eshop.Web.Models.Database
{
    public class DatabaseInit
    {
        public void Initialization(EshopDbContext eshopDbContext)
        {
            eshopDbContext.Database.EnsureCreated();
            if (eshopDbContext.CarouselItems.Count() == 0)
            {
                IList<CarouselItem> cItems = GenerateCarouselItems();
                foreach (var ci in cItems)
                {
                    eshopDbContext.CarouselItems.Add(ci);
                }
                eshopDbContext.SaveChanges();
            }
        }

        public List<CarouselItem> GenerateCarouselItems()
        {
            List<CarouselItem> carouselItems = new List<CarouselItem>();

            CarouselItem ci1 = new CarouselItem()
            {
                ID = 0,
                ImageSource = "/img/pic1.jpg",
                ImageAlt = "First slide"
            };
            CarouselItem ci2 = new CarouselItem()
            {
                ID = 1,
                ImageSource = "/img/pic2.jpg",
                ImageAlt = "Second slide"
            };
            CarouselItem ci3 = new CarouselItem()
            {
                ID = 2,
                ImageSource = "/img/pic3.jpg",
                ImageAlt = "Third slide"
            };
  
            carouselItems.Add(ci1);
            carouselItems.Add(ci2);
            carouselItems.Add(ci3);

            return carouselItems;
        }

        public List<Product> GenerateProductlItems()
        {
            List<Product> productItems = new List<Product>();

            Product p1 = new Product()
            {
                ID = 0,
                Name = "Product1",
                //Price = 15,
                Description = "Description of product1."
            };

            Product p2 = new Product()
            {
                ID = 1,
                Name = "Product2",
                //Price = 22.3,
                Description = "Description of product2."
            };

            Product p3 = new Product()
            {
                ID = 2,
                Name = "Product3",
                //Price = 99.99,
                Description = "Description of product3."
            };

            Product p4 = new Product()
            {
                ID = 3,
                Name = "Product4",
                //Price = 1.15,
                Description = "Description of product4."
            };

            productItems.Add(p1);
            productItems.Add(p2);
            productItems.Add(p3);
            productItems.Add(p4);



            return productItems;
        }

        public async Task EnsureRoleCreated(RoleManager<Role> roleManager)
        {
            string[] roles = Enum.GetNames(typeof(Roles));

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(new Role(role));
            }
        }

        public async Task EnsureAdminCreated(UserManager<User> userManager)
        {
            User user = new User
            {
                UserName = "admin",
                Email = "admin@admin.cz",
                EmailConfirmed = true,
                FirstName = "Tom",
                LastName = "Vogletanz"
            };
            string password = "abc";

            User adminInDatabase = await userManager.FindByNameAsync(user.UserName);

            if (adminInDatabase == null)
            {

                IdentityResult result = await userManager.CreateAsync(user, password);

                if (result == IdentityResult.Success)
                {
                    string[] roles = Enum.GetNames(typeof(Roles));
                    foreach (var role in roles)
                    {
                        await userManager.AddToRoleAsync(user, role);
                    }
                }
                else if (result != null && result.Errors != null && result.Errors.Count() > 0)
                {
                    foreach (var error in result.Errors)
                    {
                        Debug.WriteLine($"Error during Role creation for Admin: {error.Code}, {error.Description}");
                    }
                }
            }

        }

        public async Task EnsureManagerCreated(UserManager<User> userManager)
        {
            User user = new User
            {
                UserName = "manager",
                Email = "manager@manager.cz",
                EmailConfirmed = true,
                FirstName = "Tom",
                LastName = "Vogletanz"
            };
            string password = "abc";

            User managerInDatabase = await userManager.FindByNameAsync(user.UserName);

            if (managerInDatabase == null)
            {

                IdentityResult result = await userManager.CreateAsync(user, password);

                if (result == IdentityResult.Success)
                {
                    string[] roles = Enum.GetNames(typeof(Roles));
                    foreach (var role in roles)
                    {
                        if (role != Roles.Admin.ToString())
                            await userManager.AddToRoleAsync(user, role);
                    }
                }
                else if (result != null && result.Errors != null && result.Errors.Count() > 0)
                {
                    foreach (var error in result.Errors)
                    {
                        Debug.WriteLine($"Error during Role creation for Manager: {error.Code}, {error.Description}");
                    }
                }
            }

        }
    }
}
