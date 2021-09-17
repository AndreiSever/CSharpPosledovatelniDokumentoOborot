using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentAssignments.Web.Model
{
    public class DbInitializer
    {
        public static async Task SeedAsync(ApplicationContext context, IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
            var logger = serviceProvider.GetRequiredService<ILogger<DbInitializer>>();

            var adminUser = new User { UserName = "admin", LastName = "Иванов", FirstName = "Иван", MiddleName = "Иванович" };
            await userManager.CreateAsync(adminUser, "admin");

            var adminRole = new Role { Name = "Admin" };
            var userRole = new Role { Name = "User" };
            await roleManager.CreateAsync(adminRole);
            await roleManager.CreateAsync(userRole);

            await userManager.AddToRolesAsync(adminUser, new[] { adminRole.Name, userRole.Name });

            var fileDocumentType = new DocumentType { Name = "Файл" };
            var textDocumentType = new DocumentType { Name = "Текст" };
            context.DocumentTypes.Add(fileDocumentType);
            context.DocumentTypes.Add(textDocumentType);

            context.SaveChanges();
        }
    }
}
