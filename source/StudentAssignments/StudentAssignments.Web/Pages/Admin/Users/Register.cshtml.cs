using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using StudentAssignments.Web.Model;

namespace StudentAssignments.Web.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<RegisterModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Поле обязательно для заполнения.")]
            [Display(Name = "Логин")]
            public string UserName { get; set; }

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

            [Required(ErrorMessage = "Поле обязательно для заполнения.")]
            //[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 4)]
            [DataType(DataType.Password)]
            [Display(Name = "Пароль")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Подтвердите пароль")]
            [Compare("Password", ErrorMessage = "Пароль и подтверждение пароля не совпадают.")]
            public string ConfirmPassword { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = new User { UserName = Input.UserName, LastName = Input.LastName, FirstName = Input.FirstName, MiddleName = Input.MiddleName, GroupId = Input.GroupId };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    try
                    {
                        await _userManager.AddToRoleAsync(user, "User");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Ошибка при попытке назначить пользователю роль.");
                    }
                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //var callbackUrl = Url.Page(
                    //    "/Account/ConfirmEmail",
                    //    pageHandler: null,
                    //    values: new { userId = user.Id, code = code },
                    //    protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    //await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    if (error.Code == "DuplicateUserName")
                        ModelState.AddModelError(string.Empty, "Пользователь с таким логином уже существует.");
                    else if(error.Code == "InvalidUserName")
                        ModelState.AddModelError(string.Empty, "Логин содержит недопустимые символы.");
                    else
                        ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
