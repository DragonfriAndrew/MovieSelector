using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieApp.Models;
using MovieApp.Repositories;

namespace MovieApp.Controllers
{
    public class MovieController : Controller
    {
        private IMovieRepository _movieRepository;
        
        public MovieController(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;
        }

        private bool IsAdmin()
        {
            string token = HttpContext.Session.GetString("token");
            return _movieRepository.IsAdmin(token);
        }

        private bool IsSignedIn()
        {
            string token = HttpContext.Session.GetString("token");
            if (token == null || !CurrentUser.Users.ContainsKey(token))
            {
                return false;
            }

            return true;
        }

        // GET: Movie
        public async Task<IActionResult> Index()
        {
            if (!IsSignedIn())
            {
                TempData["message"] = "Please sign in";
                return RedirectToAction("Login", "User");
            }
            return View(await _movieRepository.GetAll());
        }

        // GET: Movie/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (!IsSignedIn())
            {
                TempData["message"] = "Please sign in";
                return RedirectToAction("Login", "User");
            }
            if (IsAdmin())
            {
                ViewBag.isAdmin = "true";
            }
            if (id == null)
            {
                return NotFound();
            }

            var tblMovie = await _movieRepository.GetSingle(id.GetValueOrDefault());
            if (tblMovie == null)
            {
                return NotFound();
            }

            // Get Comments
            List<TblComment> comments = _movieRepository.GetComments(id.GetValueOrDefault());
            ViewBag.comments = comments;

            if (tblMovie.Likes == null) tblMovie.Likes = 0;
            if (tblMovie.Dislikes == null) tblMovie.Dislikes = 0;
            
            return View(tblMovie);
        }

        public async Task<IActionResult> AddLikesDislikes(Guid id, bool up)
        {
            TblUser user = CurrentUser.Users[HttpContext.Session.GetString("token")];
            TblMovie movie = await _movieRepository.GetSingle(id);
            Guid[] movieUser = new Guid[] { user.UserId, movie.MovieId };
            foreach (var item in CurrentUser.MovieUser)
            {
                if (item[0] == movieUser[0] && item[1] == movieUser[1])
                {
                    TempData["message"] = "Already voted";
                    return RedirectToAction("Details", new { id = movie.MovieId });
                }
            }
            CountThumbs(movie, up);
            await _movieRepository.Update(movie);
            CurrentUser.MovieUser.Add(movieUser);
            TempData["message"] = "You successfully voted";
            return RedirectToAction("Details", new { id = movie.MovieId });
        }

        private void CountThumbs(TblMovie movie, bool up)
        {
            if (up)
            {
                if (movie.Likes == null)
                {
                    movie.Likes = 1;
                }
                else
                {
                    movie.Likes++;
                }
            }
            else
            {
                if (movie.Dislikes == null)
                {
                    movie.Dislikes = 1;
                }
                else
                {
                    movie.Dislikes++;
                }
            }
        }

        // GET: Movie/Create
        public IActionResult Create()
        {
            if (!IsAdmin())
            {
                TempData["message"] = "Access denied";
                return RedirectToAction("Index");
            }
            return View();
        }

        // POST: Movie/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MovieId,MovieName,Director,Year,Synopsis,PosterLink,TrailerLink,Likes,Dislikes")] TblMovie tblMovie)
        {
            if (ModelState.IsValid)
            {
                tblMovie.MovieId = Guid.NewGuid();
                //tblMovie.Year = Convert.ToDateTime(Convert.ToDateTime(tblMovie.Year).ToString("YYYY-MM-DD"));
                await _movieRepository.Add(tblMovie);
                return RedirectToAction(nameof(Index));
            }
            return View(tblMovie);
        }

        // GET: Movie/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
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

            var tblMovie = await _movieRepository.GetSingle(id.GetValueOrDefault());
            if (tblMovie == null)
            {
                return NotFound();
            }
            return View(tblMovie);
        }

        // POST: Movie/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("MovieId,MovieName,Director,Year,Synopsis,PosterLink,TrailerLink,Likes,Dislikes")] TblMovie tblMovie)
        {
            if (id != tblMovie.MovieId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _movieRepository.Update(tblMovie);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblMovieExists(tblMovie.MovieId))
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
            return View(tblMovie);
        }

        // GET: Movie/Delete/5
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

            var tblMovie = await _movieRepository.GetSingle(id.GetValueOrDefault());
            if (tblMovie == null)
            {
                return NotFound();
            }

            return View(tblMovie);
        }

        // POST: Movie/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _movieRepository.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        private bool TblMovieExists(Guid id)
        {
            return _movieRepository.DoesExist(id);
        }
    }
}
