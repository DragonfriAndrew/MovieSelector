using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MovieApp.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieApp.Models;

namespace MovieApp.Controllers
{
    public class CommentController : Controller
    {
        private ICommentRepository _commentRepository;

        public CommentController(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        // POST: Comment/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormCollection formField, Guid movieId)
        {
            TblComment tblComment = new TblComment();
            tblComment.CommentId = Guid.NewGuid();
            tblComment.MovieId = movieId;
            tblComment.UserId = CurrentUser.Users[HttpContext.Session.GetString("token")].UserId;
            tblComment.Comment = formField["txtComment"].ToString();
            //tblComment.User = CurrentUser.Users[HttpContext.Session.GetString("token")];
            if (tblComment.Comment.Trim() == "")
            {
                return RedirectToAction("Details", "Movie", new { id = tblComment.MovieId });
            }
            tblComment.Date = DateTime.Now;
            await _commentRepository.Add(tblComment);
            return RedirectToAction("Details", "Movie", new { id = tblComment.MovieId } );
        }

        // POST: Comment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var tblComment = await _commentRepository.GetSingle(id);
            await _commentRepository.Delete(id);
            return RedirectToAction("Details", "Movie", new { id = tblComment.MovieId });
        }

        private bool TblCommentExists(Guid id)
        {
            return _commentRepository.DoesExist(id);
        }
    }
}
