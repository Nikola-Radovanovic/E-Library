using Library.Areas.Identity.Data;
using Library.Data;
using Library.Models.LibraryModels;
using Library.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Library.Controllers
{
    [Authorize]
    public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ReservationsController(ApplicationDbContext dbContext,
                                      IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override void Dispose(bool disposing)
        {
            _dbContext.Dispose();
        }

        public async Task<IActionResult> GetReservations()
        {
            var reservations = await _dbContext.Reservations.Include(r => r.ApplicationUser)
                                                            .Include(r => r.Book)
                                                            .Include(r => r.Book.Author).ToListAsync();

            if (User.IsInRole("Admin") || User.IsInRole("Bibliotekar"))
            {
                return View("GetReservations", reservations);
            }

            return View("GetReservationsUser", reservations);
        }

        public async Task<IActionResult> CreateReservationForm()
        {
            var viewModel = new CreateOrUpdateReservationViewModel();

            var books = await _dbContext.Books.ToListAsync();
            var users = await _dbContext.Users.ToListAsync();

            books.ForEach(b =>
            {
                viewModel.BookList.Add(new SelectListItem { Value = b.Id.ToString(), Text = b.Title });
            });

            users.ForEach(u =>
            {
                viewModel.UserList.Add(new SelectListItem { Value = u.Id, Text = u.UserName });
            });

            return View("CreateReservationForm", viewModel);
        }

        public async Task<IActionResult> CreateReservationFormUser(int id)
        {
            var books = await _dbContext.Books.Include(b => b.Author)
                                              .Include(b => b.Publisher)
                                              .Include(b => b.Genre).ToListAsync();

            var book = books.Where(b => b.Id == id).First();

            var viewModel = new CreateReservationUserByBookIdViewModel()
            {
                Reservation = new Reservation(),
                Book = book
            };

            return View("CreateReservationFormUser", viewModel);
        }

        public async Task<IActionResult> UpdateReservationForm(int id)
        {
            var reservation = await _dbContext.Reservations.FirstOrDefaultAsync(r => r.Id == id);
            var books = await _dbContext.Books.ToListAsync();

            if (reservation == null)
            {
                return NotFound();
            }

            var viewModel = new CreateOrUpdateReservationViewModel()
            {
                Reservation = reservation
            };

            books.ForEach(b =>
            {
                viewModel.BookList.Add(new SelectListItem { Value = b.Id.ToString(), Text = b.Title });
            });

            return View("UpdateReservationForm", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrUpdateReservation(Reservation reservation)
        {
            var books = await _dbContext.Books.ToListAsync();
            var users = await _dbContext.Users.ToListAsync();

            var viewModel = new CreateOrUpdateReservationViewModel()
            {
                Reservation = new Reservation()
            };

            books.ForEach(b =>
            {
                viewModel.BookList.Add(new SelectListItem { Value = b.Id.ToString(), Text = b.Title });
            });

            users.ForEach(u =>
            {
                viewModel.UserList.Add(new SelectListItem { Value = u.Id, Text = u.UserName });
            });

            if (reservation.Id == 0)
            {
                var bookFromReservation = books.Where(b => b.Id == reservation.BookId).First();

                if (reservation.RentedBooksNumber > bookFromReservation.NumberOfCopies)
                {
                    if (bookFromReservation.NumberOfCopies == 0)
                    {
                        ModelState.AddModelError("Reservation.RentedBooksNumber", "Nema nijednog primerka");
                        return View("CreateReservationForm", viewModel);
                    }

                    ModelState.AddModelError("Reservation.RentedBooksNumber", "Izabrali ste više knjiga nego što je dostupno");
                    return View("CreateReservationForm", viewModel);
                }

                await _dbContext.Reservations.AddAsync(reservation);
            }
            else
            {
                var reservationInDb = await _dbContext.Reservations.FirstAsync(r => r.Id == reservation.Id);

                reservationInDb.BookId = reservation.BookId;
                reservationInDb.ReservationDate = reservation.ReservationDate;
                reservationInDb.IsAccepted = reservation.IsAccepted;

                if (reservation.IsAccepted == true)
                {
                    DateTime rentDate = Convert.ToDateTime(reservation.RentDate);
                    DateTime returnDate = Convert.ToDateTime(reservation.ReturnDate);
                    rentDate = DateTime.Now;
                    returnDate = rentDate.AddDays(20);
                    reservation.RentDate = rentDate;
                    reservation.ReturnDate = returnDate;
                    reservationInDb.RentDate = reservation.RentDate;
                    reservationInDb.ReturnDate = reservation.ReturnDate;
                }
            }

            await _dbContext.SaveChangesAsync();

            return RedirectToAction("GetReservations", "Reservations", new { reservation.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateReservationUser(Reservation reservation)
        {
            var sessionUserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            CreateReservationUserByBookIdViewModel viewModel = new();

            viewModel.Reservation = new Reservation();
            
            if (reservation.Id == 0)
            {
                reservation.ApplicationUserId = sessionUserId;

                await _dbContext.Reservations.AddAsync(reservation);
            }

            await _dbContext.SaveChangesAsync();

            return RedirectToAction("GetReservations", "Reservations", new { reservation.Id });
        }

        public async Task<IActionResult> ConfirmReservation(Reservation reservation)
        {
            var reservationInDb = await _dbContext.Reservations.FirstOrDefaultAsync(r => r.Id == reservation.Id);
            var bookFromReservation = await _dbContext.Books.FirstAsync(b => b.Id == reservationInDb.BookId);

            if (reservationInDb == null)
            {
                return NotFound();
            }

            reservation.IsAccepted = true;

            if (reservation.IsAccepted == true)
            {
                reservationInDb.RentDate = DateTime.Now;
                DateTime rentDate = Convert.ToDateTime(reservationInDb.RentDate);
                reservationInDb.ReturnDate = rentDate.AddDays(20);

                bookFromReservation.NumberOfCopies -= reservationInDb.RentedBooksNumber;

                if (bookFromReservation.NumberOfCopies < 0)
                {
                    bookFromReservation.NumberOfCopies = 0;
                }
            }

            await _dbContext.SaveChangesAsync();

            return RedirectToAction("GetReservations", "Reservations", new { reservation.Id });
        }

        public async Task<IActionResult> ReturnBook(Reservation reservation)
        {
            var reservationInDb = await _dbContext.Reservations.FirstOrDefaultAsync(r => r.Id == reservation.Id);
            var bookFromReservation = await _dbContext.Books.FirstAsync(b => b.Id == reservationInDb.BookId);

            if (reservationInDb == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(reservationInDb.ReturnDate.ToString()))
            {
                reservation.IsReturned = false;
                return RedirectToAction("GetReservations", "Reservations", new { reservation.Id });
            }

            reservation.IsReturned = true;

            if (reservation.IsReturned == true)
            {
                bookFromReservation.NumberOfCopies += reservationInDb.RentedBooksNumber;
            }

            _dbContext.Remove(reservationInDb);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("GetReservations", "Reservations", new { reservation.Id });
        }

        public async Task<IActionResult> DeleteReservation(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var reservationDelete = await _dbContext.Reservations.Include(r => r.ApplicationUser)
                                                                 .Include(r => r.Book)
                                                                 .FirstOrDefaultAsync(r => r.Id == id);

            if (reservationDelete == null)
            {
                return NotFound();
            }

            return View("DeleteReservation", reservationDelete);
        }

        [HttpPost, ActionName("DeleteReservation")]
        public async Task<IActionResult> DeleteReservationConfirm(Reservation reservation)
        {
            var reservationInDb = await _dbContext.Reservations.FirstOrDefaultAsync(r => r.Id == reservation.Id);
            var bookFromReservation = await _dbContext.Books.FirstAsync(b => b.Id == reservationInDb.BookId);

            reservation.IsReturned = true;

            if (reservation.IsReturned == true)
            {
                if (!string.IsNullOrEmpty(reservationInDb.ReturnDate.ToString()))
                {
                    bookFromReservation.NumberOfCopies += reservationInDb.RentedBooksNumber;
                }
            }

            _dbContext.Remove(reservationInDb);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("GetReservations", "Reservations");
        }
    }
}