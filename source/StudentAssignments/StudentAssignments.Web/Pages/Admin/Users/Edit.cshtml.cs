using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentAssignments.Web.Identity;
using StudentAssignments.Web.Model;

namespace StudentAssignments.Web.Pages.Admin.Users
{
    public class EditModel : PageModel
    {
        private readonly StudentAssignments.Web.Model.ApplicationContext _context;
        private readonly IUserStore<User> _userStore;
        private readonly IPasswordHasher<User> _passwordHasher;

        public EditModel(StudentAssignments.Web.Model.ApplicationContext context, IUserStore<User> userStore, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _userStore = userStore;
            _passwordHasher = passwordHasher;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public int Id { get; set; }

            [Display(Name = "Логин")]
            public string UserName { get; set; }

            [Display(Name = "Роли")]
            public RoleModel[] Roles { get; set; }

            [Required(ErrorMessage = "Поле обязательно для заполнения.")]
            [Display(Name = "Фамилия")]
            public string LastName { get; set; }

            [Required(ErrorMessage = "Поле обязательно для заполнения.")]
            [Display(Name = "Имя")]
            public string FirstName { get; set; }

            [Display(Name = "Отчество")]
            public string MiddleName { get; set; }

            [Display(Name = "Группа")]
            public int? GroupId { get; set; }

            //[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 4)]
            [DataType(DataType.Password)]
            [Display(Name = "Новый пароль")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Подтвердите новый пароль")]
            [Compare("Password", ErrorMessage = "Пароль и подтверждение пароля не совпадают.")]
            public string ConfirmPassword { get; set; }
        }

        public class RoleModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public bool IsChecked { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);
            var roles = await _context.Roles.Select(r => new RoleModel { Id = r.Id, Name = r.Name }).ToArrayAsync();
            foreach (var userRole in user.UserRoles)
            {
                var role = roles.FirstOrDefault(r => r.Id == userRole.RoleId);
                if (role != null)
                    role.IsChecked = true;
            }

            if (user == null)
            {
                return NotFound();
            }
            Input = new InputModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Roles = roles,
                LastName = user.LastName,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                GroupId = user.GroupId
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == Input.Id);
            if (user == null)
            {
                return NotFound();
            }

            user.LastName = Input.LastName;
            user.FirstName = Input.FirstName;
            user.MiddleName = Input.MiddleName;
            user.GroupId = Input.GroupId;
            if (!string.IsNullOrWhiteSpace(Input.Password))
            {
                var userPasswordStore = _userStore as IUserPasswordStore<User>;
                if (userPasswordStore != null)
                {
                    var hash = _passwordHasher.HashPassword(user, Input.Password);
                    await userPasswordStore.SetPasswordHashAsync(user, hash, CancellationToken.None);
                }
            }
            user.UserRoles.Clear();
            foreach (RoleModel roleModel in Input.Roles)
            {
                if (roleModel.IsChecked)
                    user.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = roleModel.Id });
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(user.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
