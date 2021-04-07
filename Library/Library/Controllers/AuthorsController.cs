using Library.Data;
using Library.Models.LibraryModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Controllers
{
    [Authorize(Roles = "Admin, Bibliotekar")]
    public class AuthorsController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public AuthorsController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected override void Dispose(bool disposing)
        {
            _dbContext.Dispose();
        }

        public async Task<IActionResult> GetAuthors()
        {
            var authors = await _dbContext.Authors.ToListAsync();

            return View(authors);
        }

        public IActionResult CreateAuthorForm()
        {
            return View("CreateAuthorForm");
        }

        public async Task<IActionResult> UpdateAuthorForm(int id)
        {
            var authorInDb = await _dbContext.Authors.FirstOrDefaultAsync(a => a.Id == id);

            if (authorInDb == null)
            {
                return NotFound();
            }

            return View("UpdateAuthorForm", authorInDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrUpdateAuthor(Author author)
        {
            if (author.Id == 0)
            {
                if (await _dbContext.Authors.AnyAsync(a => a.AuthorName == author.AuthorName))
                {
                    ModelState.AddModelError("AuthorName", "Autor već postoji");

                    return View("CreateAuthorForm", author);
                }

                await _dbContext.Authors.AddAsync(author);
            }
            else
            {
                var authorInDb = await _dbContext.Authors.FirstAsync(a => a.Id == author.Id);

                authorInDb.AuthorName = author.AuthorName;
            }

            await _dbContext.SaveChangesAsync();

            return RedirectToAction("GetAuthors", "Authors", new { author.Id });
        }

        public async Task<IActionResult> DeleteAuthor(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var author = await _dbContext.Authors.FindAsync(id);

            if (author == null)
            {
                return NotFound();
            }

            return View(author);
        }

        [HttpPost, ActionName("DeleteAuthor")]
        public async Task<IActionResult> DeleteAuthorConfirm(int id)
        {
            var author = await _dbContext.Authors.FindAsync(id);

            _dbContext.Remove(author);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("GetAuthors", "Authors");
        }
    }
}