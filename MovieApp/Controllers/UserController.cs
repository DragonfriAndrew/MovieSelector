using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieApp.Models;
using MovieApp.Repositories;

namespace MovieApp.Controllers
{
    public class UserController : Controller
    {
        private IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public JsonResult IsUniqueUserName(string username)
        {
            if (HttpContext.Session.GetString("mode") == "SignUp")
            {
                if (_userRepository.DoesUserNameExist(username))
                {
                    return Json(false);
                }
            }
            return Json(true);
        }
        // GET: User
        public async Task<IActionResult> Index()
        {
            string token = HttpContext.Session.GetString("token");
            if (token == null || !CurrentUser.Users.ContainsKey(token))
            {
                return RedirectToAction("Login");
            }
            ViewBag.token = token;

            return View(await _userRepository.GetAll());
        }

        // GET: User/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            string token = HttpContext.Session.GetString("token");
            if (token == null || !CurrentUser.Users.ContainsKey(token))
            {
                return RedirectToAction("Login");
            }

            if (id == null)
            {
                return NotFound();
            }

            var tblUser = await _userRepository.GetSingle(id.GetValueOrDefault());
            if (tblUser == null)
            {
                return NotFound();
            }

            ViewBag.token = token;

            return View(tblUser);
        }

        // GET: User/SignUp
        public IActionResult SignUp()
        {
            string token = HttpContext.Session.GetString("token");
            if (token != null && CurrentUser.Users.ContainsKey(token))
            {
                TempData["message"] = "Already signed in";
                return RedirectToAction("Index", "Home");
            }
            ViewBag.token = token;
            HttpContext.Session.SetString("mode", "SignUp");
            return View();
        }

        // POST: User/SignUp
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp([Bind("UserId,FirstName,LastName,Email,UserName,Password")] TblUser tblUser)
        {
            if (ModelState.IsValid)
            {
                tblUser.UserId = Guid.NewGuid();

                // Hash password
                string pw = _userRepository.EncryptString(tblUser.Password);
                tblUser.Password = pw;

                await _userRepository.Add(tblUser);
                return RedirectToAction(nameof(Index));
            }
            return View(tblUser);
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            string token = HttpContext.Session.GetString("token");
            if (token == null || !CurrentUser.Users.ContainsKey(token))
            {
                return RedirectToAction("Login");
            }
            else if (CurrentUser.Users[token].UserId != id)
            {
                TempData["message"] = "Not allowed to edit other users";
                return RedirectToAction("Index");
            }

            if (id == null)
            {
                return NotFound();
            }

            var tblUser = await _userRepository.GetSingle(id.GetValueOrDefault());
            if (tblUser == null)
            {
                return NotFound();
            }

            ViewBag.token = token;

            return View(tblUser);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("UserId,FirstName,LastName,Email,UserName,Password")] TblUser tblUser)
        {
            if (id != tblUser.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _userRepository.Update(tblUser);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblUserExists(tblUser.UserId))
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
            return View(tblUser);
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            string token = HttpContext.Session.GetString("token");
            if (token == null || !CurrentUser.Users.ContainsKey(token))
            {
                return RedirectToAction("Login");
            }
            else if (CurrentUser.Users[token].UserId != id)
            {
                TempData["message"] = "Not allowed to delete other users";
                return RedirectToAction("Index");
            }

            if (id == null)
            {
                return NotFound();
            }

            var tblUser = await _userRepository.GetSingle(id.GetValueOrDefault());
            if (tblUser == null)
            {
                return NotFound();
            }

            ViewBag.token = token;

            return View(tblUser);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var tblUser = await _userRepository.GetSingle(id);
            await _userRepository.Delete(id);
            //CurrentUser.Users.Remove(HttpContext.Session.GetString("token"));
            foreach (var user in CurrentUser.Users)
            {
                if (user.Value.UserId == id)
                {
                    CurrentUser.Users.Remove(user.Key);
                    break;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        private bool TblUserExists(Guid id)
        {
            return _userRepository.DoesExist(id);
        }

        // GET: User/Login
        public IActionResult Login()
        {
            string token = HttpContext.Session.GetString("token");
            if (token != null)
            {
                if (CurrentUser.Users.ContainsKey(token))
                {
                    TempData["message"] = "Already signed in";
                    return RedirectToAction("Index", "Home");
                }
            }
            HttpContext.Session.SetString("mode", "SignIn");
            return View();
        }

        // POST: User/Login
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("UserName,Password")] TblUser tblUser)
        {
            if (ModelState.IsValid)
            {
                var user = await _userRepository.GetLoginInfo(tblUser.UserName, tblUser.Password);
                if (user == null)
                {
                    TempData["message"] = "Invalid credential";
                    return View(tblUser);
                }

                Guid guid = Guid.NewGuid();
                string token = guid.ToString();
                CurrentUser.Users.Add(token, user);

                HttpContext.Session.SetString("token", token);
                ViewBag.loggedInUserName = CurrentUser.Users[token].UserName;
                ViewBag.loggedInUserId = CurrentUser.Users[token].UserId;

                TempData["message"] = "Hello, " + CurrentUser.Users[token].UserName;

                return RedirectToAction("Index", "Movie");
            }
            return View(tblUser);
        }

        // GET: User/Logout/5
        [HttpGet]
        public IActionResult Logout(Guid id)
        {
            string token = HttpContext.Session.GetString("token");

            if (token == null || !CurrentUser.Users.ContainsKey(token))
            {
                TempData["message"] = "Please sign in";
                return RedirectToAction("Login");
            }

            if (CurrentUser.Users[token].UserId != id)
            {
                TempData["message"] = "Sign out failed";
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                CurrentUser.Users.Remove(token);
                HttpContext.Session.Remove("token");
                TempData["message"] = "Signed out successfully";
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
