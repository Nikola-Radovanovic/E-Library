using Library.Data;
using Library.Models.LibraryModels;
using Library.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Controllers
{
    [Authorize]
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public BooksController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected override void Dispose(bool disposing)
        {
            _dbContext.Dispose();
        }

        [AllowAnonymous]
        public async Task<IActionResult> GetBooks()
        {
            var books = await _dbContext.Books.Include(b => b.Author)
                                              .Include(b => b.Publisher)
                                              .Include(b => b.Genre).ToListAsync();

            if (!User.Identity.IsAuthenticated)
            {
                return View("GetBooksAnonymous", books);
            }
            else if (User.IsInRole("Admin") || User.IsInRole("Bibliotekar"))
            {
                return View("GetBooks", books);
            }

            return View("GetBooksAuthenticated", books);
        }

        public async Task<IActionResult> CreateBookForm()
        {
            var authors = await _dbContext.Authors.ToListAsync();
            var publishers = await _dbContext.Publishers.ToListAsync();
            var genres = await _dbContext.Genres.ToListAsync();

            CreateOrUpdateBookViewModel viewModel = new();

            authors.ForEach(a =>
            {
                viewModel.AuthorList.Add(new SelectListItem { Value = a.Id.ToString(), Text = a.AuthorName });
            });

            publishers.ForEach(p =>
            {
                viewModel.PublisherList.Add(new SelectListItem { Value = p.Id.ToString(), Text = p.PublisherName });
            });

            genres.ForEach(g =>
            {
                viewModel.GenreList.Add(new SelectListItem { Value = g.Id.ToString(), Text = g.GenreName });
            });

            return View("CreateBookForm", viewModel);
        }

        public async Task<IActionResult> UpdateBookForm(int id)
        {
            var book = await _dbContext.Books.FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            var authors = await _dbContext.Authors.ToListAsync();
            var publishers = await _dbContext.Publishers.ToListAsync();
            var genres = await _dbContext.Genres.ToListAsync();

            CreateOrUpdateBookViewModel viewModel = new()
            {
                Book = book
            };

            authors.ForEach(a =>
            {
                viewModel.AuthorList.Add(new SelectListItem { Value = a.Id.ToString(), Text = a.AuthorName });
            });

            publishers.ForEach(p =>
            {
                viewModel.PublisherList.Add(new SelectListItem { Value = p.Id.ToString(), Text = p.PublisherName });
            });

            genres.ForEach(g =>
            {
                viewModel.GenreList.Add(new SelectListItem { Value = g.Id.ToString(), Text = g.GenreName });
            });

            return View("UpdateBookForm", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrUpdateBook(Book book)
        {
            CreateOrUpdateBookViewModel viewModel = new()
            {
                Book = new Book()
            };

            var authors = await _dbContext.Authors.ToListAsync();
            var publishers = await _dbContext.Publishers.ToListAsync();
            var genres = await _dbContext.Genres.ToListAsync();

            authors.ForEach(a =>
            {
                viewModel.AuthorList.Add(new SelectListItem { Value = a.Id.ToString(), Text = a.AuthorName });
            });

            publishers.ForEach(p =>
            {
                viewModel.PublisherList.Add(new SelectListItem { Value = p.Id.ToString(), Text = p.PublisherName });
            });

            genres.ForEach(g =>
            {
                viewModel.GenreList.Add(new SelectListItem { Value = g.Id.ToString(), Text = g.GenreName });
            });

            if (book.Id == 0)
            {
                await _dbContext.Books.AddAsync(book);
            }
            else
            {
                if (!ModelState.IsValid)
                {
                    return View("UpdateBookForm", viewModel);
                }

                var bookInDb = await _dbContext.Books.FirstAsync(b => b.Id == book.Id);

                bookInDb.Title = book.Title;
                bookInDb.AuthorId = book.AuthorId;
                bookInDb.PublisherId = book.PublisherId;
                bookInDb.GenreId = book.GenreId;
                bookInDb.NumberOfPages = book.NumberOfPages;
                bookInDb.ISBN = book.ISBN;
                bookInDb.NumberOfCopies = book.NumberOfCopies;
                bookInDb.EntryDate = book.EntryDate;
            }

            await _dbContext.SaveChangesAsync();

            return RedirectToAction("GetBooks", "Books", new { book.Id });
        }

        public async Task<IActionResult> DeleteBook(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var book = await _dbContext.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            return View("DeleteBook", book);
        }

        [HttpPost, ActionName("DeleteBook")]
        public async Task<IActionResult> DeleteBookConfirm(int id)
        {
            var book = await _dbContext.Books.FindAsync(id);

            _dbContext.Books.Remove(book);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("GetBooks", "Books");
        }
    }
}