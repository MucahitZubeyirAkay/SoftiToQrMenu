using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QrMenuApi.Data.Context;
using QrMenuApi.Data.Models;

namespace QrMenuApi.Data
{
	public class DbInitializer
	{
        public DbInitializer(QrMenuApiContext? context, RoleManager<IdentityRole>? roleManager, UserManager<ApplicationUser>? userManager)
        {
            State state;
            IdentityRole identityRole;
            ApplicationUser applicationUser;
            Company? company = null;

            if (context != null)
            {
                context.Database.Migrate();
                if (context.States!.Count() == 0)
                {
                    state = new State();
                    state.Id = 0;
                    state.Name = "Deleted";
                    context.States!.Add(state);
                    state = new State();
                    state.Id = 1;
                    state.Name = "Active";
                    context.States.Add(state);
                    state = new State();
                    state.Id = 2;
                    state.Name = "Passive";
                    context.States.Add(state);
                }
                if (context.Companies!.Count() == 0)
                {
                    company = new Company();
                    company.Address = "İstanbul";
                    company.EMail = "tatgida.com";
                    company.Name = "Tat Gida";
                    company.PhoneNumber = "2102101111";
                    company.PostalCode = "34210";
                    company.RegisterDate = DateTime.Today;
                    company.StateId = 1;
                    company.ParentCompanyId = null;
                    company.TaxNumber = "1234567890";
                    context.Companies!.Add(company);
                }
                context.SaveChanges();
                if (roleManager != null)
                {
                    if (roleManager.Roles.Count() == 0)
                    {
                        identityRole = new IdentityRole("Administrator");
                        roleManager.CreateAsync(identityRole).Wait();
                        identityRole = new IdentityRole("CompanyAdministrator");
                        roleManager.CreateAsync(identityRole).Wait();
                        identityRole = new IdentityRole("RestaurantAdministrator");
                        roleManager.CreateAsync(identityRole).Wait();
                    }
                }
                if (userManager != null)
                {
                    if (userManager.Users.Count() == 0)
                    {
                        if (company != null)
                        {
                            applicationUser = new ApplicationUser();
                            applicationUser.UserName = "mucahitzubeyir";
                            applicationUser.CompanyId = company.Id;
                            applicationUser.Name = "Mücahit Zübeyir";
                            applicationUser.Email = "abc@def.com";
                            applicationUser.PhoneNumber = "1112223344";
                            applicationUser.RegisterDate = DateTime.Today;
                            applicationUser.StateId = 1;
                            applicationUser.SurName = "Akay";
                            userManager.CreateAsync(applicationUser, "Admin123!").Wait();
                            userManager.AddToRoleAsync(applicationUser, "Administrator").Wait();
                        }
                    }
                }
            }
        }
    }
}