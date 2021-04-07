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
    public class GenresController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public GenresController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected override void Dispose(bool disposing)
        {
            _dbContext.Dispose();
        }

        public async Task<IActionResult> GetGenres()
        {
            var genres = await _dbContext.Genres.ToListAsync();

            return View(genres);
        }

        public IActionResult CreateGenreForm()
        {
            return View("CreateGenreForm");
        }

        public async Task<IActionResult> UpdateGenreForm(int id)
        {
            var genreInDb = await _dbContext.Genres.FirstOrDefaultAsync(g => g.Id == id);

            if (genreInDb == null)
            {
                return NotFound();
            }

            return View("UpdateGenreForm", genreInDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrUpdateGenre(Genre genre)
        {
            if (genre.Id == 0)
            {
                if (await _dbContext.Genres.AnyAsync(g => g.GenreName == genre.GenreName))
                {
                    ModelState.AddModelError("GenreName", "Žanr već postoji");

                    return View("CreateGenreForm", genre);
                }

                await _dbContext.Genres.AddAsync(genre);
            }
            else
            {
                var genreInDb = await _dbContext.Genres.FirstOrDefaultAsync(g => g.Id == genre.Id);

                genreInDb.GenreName = genre.GenreName;
            }

            await _dbContext.SaveChangesAsync();

            return RedirectToAction("GetGenres", "Genres", new { genre.Id });
        }

        public async Task<IActionResult> DeleteGenre(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var genre = await _dbContext.Genres.FindAsync(id);

            if (genre == null)
            {
                return NotFound();
            }

            return View("DeleteGenre", genre);
        }

        [HttpPost, ActionName("DeleteGenre")]
        public async Task<IActionResult> DeleteGenreConfirm(int id)
        {
            var genre = await _dbContext.Genres.FindAsync(id);

            _dbContext.Genres.Remove(genre);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("GetGenres", "Genres");
        }
    }
}