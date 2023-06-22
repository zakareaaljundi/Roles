using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Roles.Data;
using Roles.Models;

namespace Roles.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly finalDbContext _context;
        private UserManager<IdentityUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        public DepartmentsController(finalDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Departments
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var currentRoles = await _userManager.GetRolesAsync(currentUser);
            var roleId = "";
            if (currentRoles.Count > 0)
            {
                var roleName = currentRoles[0];
                var role = await _roleManager.FindByNameAsync(roleName);

                if (role != null)
                {
                    roleId = role.Id;
                }
            }

            var controllerName = this.ControllerContext.RouteData.Values["controller"]!.ToString();

            var rolePermissions = await _context.RolePermissions.FirstOrDefaultAsync(p => p.RoleId! == roleId && p.TableName == controllerName);

            bool canAdd = rolePermissions != null && rolePermissions.AddPermission;
            bool canEdit = rolePermissions != null && rolePermissions.EditPermission;
            bool canRead = rolePermissions != null && rolePermissions.ReadPermission;
            bool canDelete = rolePermissions != null && rolePermissions.DeletePermission;

            ViewBag.CanAdd = canAdd;
            ViewBag.CanEdit = canEdit;
            ViewBag.CanRead = canRead;
            ViewBag.CanDelete = canDelete;

            return _context.Departments != null ? 
                          View(await _context.Departments.ToListAsync()) :
                          Problem("Entity set 'finalDbContext.Departments'  is null.");
        }

        // GET: Departments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var currentRoles = await _userManager.GetRolesAsync(currentUser);
            var roleId = "";
            if (currentRoles.Count > 0)
            {
                var roleName = currentRoles[0];
                var role = await _roleManager.FindByNameAsync(roleName);

                if (role != null)
                {
                    roleId = role.Id;
                }
            }
            var controllerName = this.ControllerContext.RouteData.Values["controller"]!.ToString();
            var rolePermissions = await _context.RolePermissions.FirstOrDefaultAsync(p => p.RoleId! == roleId && p.TableName == controllerName);
            bool canEdit = rolePermissions != null && rolePermissions.EditPermission;
            bool canRead = rolePermissions != null && rolePermissions.ReadPermission;
            ViewBag.CanEdit = canEdit;

            if (id == null || _context.Departments == null || canRead == false)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .FirstOrDefaultAsync(m => m.DepartmentId == id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // GET: Departments/Create
        public async Task<IActionResult> Create()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var currentRoles = await _userManager.GetRolesAsync(currentUser);
            var roleId = "";
            if (currentRoles.Count > 0)
            {
                var roleName = currentRoles[0];
                var role = await _roleManager.FindByNameAsync(roleName);

                if (role != null)
                {
                    roleId = role.Id;
                }
            }
            var controllerName = this.ControllerContext.RouteData.Values["controller"]!.ToString();
            var rolePermissions = await _context.RolePermissions.FirstOrDefaultAsync(p => p.RoleId! == roleId && p.TableName == controllerName);
            bool canAdd = rolePermissions != null && rolePermissions.AddPermission;
            if (!canAdd)
            {
                return Unauthorized();
            }
            return View();
        }

        // POST: Departments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( Department department)
        {
            if (ModelState.IsValid)
            {
                _context.Add(department);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(department);
        }

        // GET: Departments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var currentRoles = await _userManager.GetRolesAsync(currentUser);
            var roleId = "";
            if (currentRoles.Count > 0)
            {
                var roleName = currentRoles[0];
                var role = await _roleManager.FindByNameAsync(roleName);

                if (role != null)
                {
                    roleId = role.Id;
                }
            }
            var controllerName = this.ControllerContext.RouteData.Values["controller"]!.ToString();
            var rolePermissions = await _context.RolePermissions.FirstOrDefaultAsync(p => p.RoleId! == roleId && p.TableName == controllerName);
            bool canEdit = rolePermissions != null && rolePermissions.EditPermission;

            if (id == null || _context.Departments == null || canEdit == false)
            {
                return NotFound();
            }

            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }
            return View(department);
        }

        // POST: Departments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Department department)
        {
            if (id != department.DepartmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(department);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartmentExists(department.DepartmentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(department);
        }

        // GET: Departments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var currentRoles = await _userManager.GetRolesAsync(currentUser);
            var roleId = "";
            if (currentRoles.Count > 0)
            {
                var roleName = currentRoles[0];
                var role = await _roleManager.FindByNameAsync(roleName);

                if (role != null)
                {
                    roleId = role.Id;
                }
            }
            var controllerName = this.ControllerContext.RouteData.Values["controller"]!.ToString();
            var rolePermissions = await _context.RolePermissions.FirstOrDefaultAsync(p => p.RoleId! == roleId && p.TableName == controllerName);
            bool canDelete = rolePermissions != null && rolePermissions.DeletePermission;

            if (id == null || _context.Departments == null || canDelete == false)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .FirstOrDefaultAsync(m => m.DepartmentId == id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Departments == null)
            {
                return Problem("Entity set 'finalDbContext.Departments'  is null.");
            }
            var department = await _context.Departments.FindAsync(id);
            if (department != null)
            {
                _context.Departments.Remove(department);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DepartmentExists(int id)
        {
          return (_context.Departments?.Any(e => e.DepartmentId == id)).GetValueOrDefault();
        }
    }
}
