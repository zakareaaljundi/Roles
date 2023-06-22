using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Roles.Models.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System.Security.Cryptography;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection;
using Roles.Models;
using Roles.Data;

namespace Roles.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        #region Configuration

        private UserManager<IdentityUser> userManager;
        private SignInManager<IdentityUser> signInManager;
        private RoleManager<IdentityRole> roleManager;
        private readonly finalDbContext dbContext;
        public AccountController(UserManager<IdentityUser> _userManager,
            SignInManager<IdentityUser> _signInManager,
            RoleManager<IdentityRole> _roleManager,
            finalDbContext dbContext)

        {
            userManager = _userManager;
            signInManager = _signInManager;
            roleManager = _roleManager;
            this.dbContext = dbContext;
        }

        #endregion

        #region Users

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = new IdentityUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    PhoneNumber = model.Phone,
                };
                var result = await userManager.CreateAsync(user, model.Password!);
                if (result.Succeeded)
                {
                    return RedirectToAction("Login", "Account");
                }
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError(err.Code, err.Description);
                }
                return View(model);
            }
            return View(model);
        }
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Email!, model.Password!, false, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Invalid user or password");
                return View(model);
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region Roles

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole role = new IdentityRole
                {
                    Name = model.RoleName
                };
                var result = await roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("RolesList");
                }
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError(err.Code, err.Description);
                }
                return View(model);
            }
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult RolesList()
        {
            return View(roleManager.Roles);
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditRole(string id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(RolesList));
            }
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return RedirectToAction("RolesList");
            }
            EditRoleViewModel model = new EditRoleViewModel
            {
                RoleId = role.Id,
                RoleName = role.Name
            };
            foreach (var user in userManager.Users)
            {
                if (await userManager.IsInRoleAsync(user, role.Name!))
                {
                    model.Users!.Add(user.UserName!);
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = await roleManager.FindByIdAsync(model.RoleId!);
                if (role == null)
                {
                    return RedirectToAction(nameof(ErrorPage));

                }
                role.Name = model.RoleName;
                var result = await roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(RolesList));
                }
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError(err.Code, err.Description);
                }
                return View(model);
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ErrorPage()
        {
            return View();
        }  

        [HttpGet]
        public async Task<IActionResult> ModifyUsersInRole(string id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(RolesList));
            }
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return RedirectToAction(nameof(ErrorPage));
            }
            List<UserRoleViewModel> models = new List<UserRoleViewModel>();
            foreach (var user in userManager.Users)
            {
                UserRoleViewModel userRole = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };
                if (await userManager.IsInRoleAsync(user, role.Name!))
                {
                    userRole.IsSelected = true;
                }
                else
                {
                    userRole.IsSelected = false;
                }
                models.Add(userRole);
            }
            return View(models);
        }

        [HttpPost]
        public async Task<IActionResult> ModifyUsersInRole(string id, List<UserRoleViewModel> models)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(RolesList));
            }
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return RedirectToAction(nameof(ErrorPage));
            }
            IdentityResult result = new IdentityResult();
            for (int i = 0; i < models.Count; i++)
            {
                var user = await userManager.FindByIdAsync(models[i].UserId!);
                if (models[i].IsSelected && (!await userManager.IsInRoleAsync(user!, role.Name!)))
                {
                    result = await userManager.AddToRoleAsync(user!, role.Name!);
                }
                else if (!models[i].IsSelected && (await userManager.IsInRoleAsync(user!, role.Name!)))
                {
                    result = await userManager.RemoveFromRoleAsync(user!, role.Name!);
                }
            }
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(RolesList));
            }
            return View(models);

        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Permissions(string id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(RolesList));
            }

            var role = roleManager.FindByIdAsync(id).Result;
            if (role == null)
            {
                return RedirectToAction("RolesList");
            }

            var classes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(type => typeof(Controller).IsAssignableFrom(type))
                .Where(type => type.Name != "AccountController")
                .Where(type => type.Name != "HomeController")
                .Where(type => type.Name != "DashboardController")
                .Select(type => type.Name.Replace("Controller", ""))
                .ToList();

            var permissionsModel = new RolePermissionsViewModel
            {
                RoleId = role.Id,
                RoleName = role.Name,
                Permissions = new Dictionary<string, bool>(),
                ReadPermissions = new Dictionary<string, bool>(),
                AddPermissions = new Dictionary<string, bool>(),
                EditPermissions = new Dictionary<string, bool>(),
                DeletePermissions = new Dictionary<string, bool>()
            };

            foreach (var classType in classes)
            {
                var existingPermission = dbContext.RolePermissions.FirstOrDefault(p => p.RoleId == role.Id && p.TableName == classType);

                if (existingPermission != null)
                {
                    permissionsModel.Permissions[classType] = existingPermission.ReadPermission;
                    permissionsModel.AddPermissions[classType] = existingPermission.AddPermission;
                    permissionsModel.EditPermissions[classType] = existingPermission.EditPermission;
                    permissionsModel.DeletePermissions[classType] = existingPermission.DeletePermission;
                }
                else
                {
                    permissionsModel.Permissions[classType] = false;
                    permissionsModel.AddPermissions[classType] = false;
                    permissionsModel.EditPermissions[classType] = false;
                    permissionsModel.DeletePermissions[classType] = false;
                }
            }

            return View(permissionsModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Permissions(RolePermissionsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var role = await roleManager.FindByIdAsync(model.RoleId!);
            if (role == null)
            {
                return RedirectToAction("RolesList");
            }

            foreach (var tableName in model.Permissions!.Keys)
            {
                var existingPermission = await dbContext.RolePermissions.FirstOrDefaultAsync(p => p.RoleId == model.RoleId && p.TableName == tableName);

                if (existingPermission != null)
                {
                    existingPermission.ReadPermission = model.Permissions[tableName];
                    existingPermission.AddPermission = model.AddPermissions[tableName];
                    existingPermission.EditPermission = model.EditPermissions[tableName];
                    existingPermission.DeletePermission = model.DeletePermissions[tableName];

                    dbContext.RolePermissions.Update(existingPermission);
                }
                else
                {
                    var permission = new RolePermission
                    {
                        RoleId = model.RoleId,
                        TableName = tableName,
                        ReadPermission = model.Permissions[tableName],
                        AddPermission = model.AddPermissions[tableName],
                        EditPermission = model.EditPermissions[tableName],
                        DeletePermission = model.DeletePermissions[tableName]
                    };

                    dbContext.RolePermissions.Add(permission);
                }
            }

            await dbContext.SaveChangesAsync();

            return RedirectToAction("RolesList");
        }
        #endregion
    }
}
