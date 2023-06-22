using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Roles.Data;
using Roles.Models;

namespace Roles.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly finalDbContext _context;
        private UserManager<IdentityUser> _userManager;

        public EmployeesController(finalDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

      /*  private async Task<string> GetCurrentRole()
        {
            var user = await userManager.GetUserAsync(User);
            var roles = await userManager.GetRolesAsync(user);
            return roles.FirstOrDefault();
        }*/

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var currentRole = await _userManager.GetRolesAsync(currentUser);

            var controllerName = this.ControllerContext.RouteData.Values["controller"]!.ToString();

            var rolePermissions = await _context.RolePermissions.FirstOrDefaultAsync(p => currentRole.Contains(p.RoleId!) && p.TableName == controllerName);

            bool canAdd = rolePermissions != null && rolePermissions.AddPermission;
            bool canEdit = rolePermissions != null && rolePermissions.EditPermission;
            bool canRead = rolePermissions != null && rolePermissions.ReadPermission;
            bool canDelete = rolePermissions != null && rolePermissions.DeletePermission;

            ViewBag.CanAdd = canAdd;
            ViewBag.CanEdit = canEdit;
            ViewBag.CanRead = canRead;
            ViewBag.CanDelete = canDelete;

            var finalDbContext = _context.Employees.Include(e => e.Department);
            return View(await finalDbContext.ToListAsync());
        }

        /* private async Task<RolePermission> GetPermissionsForCurrentUser(string roleName, string controllerName)
         {
             return await _context.RolePermissions.FirstOrDefaultAsync(rp => rp.RoleId == roleName && rp.TableName == controllerName);
         }
         public async Task<IActionResult> Index()
         {
             var finalDbContext = _context.Employees.Include(e => e.Department);

             var user = await _userManager.GetUserAsync(User);
             var userRoles = await _userManager.GetRolesAsync(user);
             var userRole = userRoles.FirstOrDefault(); 

             var controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();

             var permissions = await GetPermissionsForCurrentUser(userRole, controllerName);

             ViewData["AddPermission"] = permissions?.AddPermission ?? false;

             return View(await finalDbContext.ToListAsync());
         }*/





        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Department)
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "Name");
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "Name", employee.DepartmentId);
            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "Name", employee.DepartmentId);
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee employee)
        {
            if (id != employee.EmployeeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.EmployeeId))
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
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "Name", employee.DepartmentId);
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Department)
                .FirstOrDefaultAsync(m => m.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Employees == null)
            {
                return Problem("Entity set 'finalDbContext.Employees'  is null.");
            }
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return (_context.Employees?.Any(e => e.EmployeeId == id)).GetValueOrDefault();
        }
    }
}
