using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieApp.Models;
using MovieApp.Repositories;

namespace MovieApp.Controllers
{
    public class AdminController : Controller
    {
        private IAdminRepository _adminRepository;
        private readonly string adminSecret = "jamdSecret";

        public AdminController(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        private bool IsAdmin()
        {
            string token = HttpContext.Session.GetString("token");
            if (token == null)
            {
                return false;
            }
            else if (!_adminRepository.IsAdmin(CurrentUser.Users[token].UserId.ToString()))
            {
                return false;
            }
            return true;
        }

        // GET: Admin
        public async Task<IActionResult> Index()
        {
            return View(await _adminRepository.GetAll());
        }

        // GET: Admin/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (!IsAdmin())
            {
                TempData["message"] = "Access denied";
                return RedirectToAction("Index");
            }
            if (id == null)
            {
                return NotFound();
            }

            var tblAdmin = await _adminRepository.GetSingle(id.GetValueOrDefault());
            if (tblAdmin == null)
            {
                return NotFound();
            }

            return View(tblAdmin);
        }

        // GET: Admin/Authenticate
        public IActionResult Authenticate()
        {
            if (IsAdmin())
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        // POST: Admin/Authenticate
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Authenticate([Bind("Code")] AdminCode adminCode)
        {
            if (ModelState.IsValid)
            {
                if (adminCode.Code == adminSecret)
                {
                    return RedirectToAction("Index");
                }
            }

            TempData["message"] = "Invalid admin code. Access denied";
            return View(adminCode);
        }

        // GET: Admin/Create
        public IActionResult Create()
        {
            SelectList list = new SelectList(_adminRepository.GetAllUsers(), "UserId", "Email");
            string email = CurrentUser.Users[HttpContext.Session.GetString("token")].Email;
            foreach (var item in list)
            {
                if (item.Text == email)
                {
                    item.Selected = true;
                }
            }
            ViewData["UserId"] = list;
            return View();
        }

        // POST: Admin/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AdminId,UserId")] TblAdmin tblAdmin)
        {
            if (ModelState.IsValid)
            {
                if (_adminRepository.IsAdmin(tblAdmin.UserId.ToString()))
                {
                    TempData["message"] = "Selected user is already an admin";
                    return RedirectToAction("Index");
                }

                tblAdmin.AdminId = Guid.NewGuid();
                await _adminRepository.Add(tblAdmin);
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_adminRepository.GetAllUsers(), "UserId", "Email", tblAdmin.UserId);
            return View(tblAdmin);
        }

        // GET: Admin/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (!IsAdmin())
            {
                TempData["message"] = "Access denied";
                return RedirectToAction("Index");
            }
            if (id == null)
            {
                return NotFound();
            }

            var tblAdmin = await _adminRepository.GetSingle(id.GetValueOrDefault());
            if (tblAdmin == null)
            {
                return NotFound();
            }

            return View(tblAdmin);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var tblAdmin = await _adminRepository.GetSingle(id);
            await _adminRepository.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        private bool TblAdminExists(Guid id)
        {
            return _adminRepository.DoesExist(id);
        }
    }
}
