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
    public class PublishersController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public PublishersController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected override void Dispose(bool disposing)
        {
            _dbContext.Dispose();
        }

        public async Task<IActionResult> GetPublishers()
        {
            var publishers = await _dbContext.Publishers.ToListAsync();

            return View(publishers);
        }

        public IActionResult CreatePublisherForm()
        {
            return View("CreatePublisherForm");
        }

        public async Task<IActionResult> UpdatePublisherForm(int id)
        {
            var publisherInDb = await _dbContext.Publishers.FirstOrDefaultAsync(p => p.Id == id);

            if (publisherInDb == null)
            {
                return NotFound();
            }

            return View("UpdatePublisherForm", publisherInDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrUpdatePublisher(Publisher publisher)
        {
            if (publisher.Id == 0)
            {
                if (await _dbContext.Publishers.AnyAsync(p => p.PublisherName == publisher.PublisherName))
                {
                    ModelState.AddModelError("PublisherName", "Izdavač već postoji");

                    return View("CreatePublisherForm", publisher);
                }

                await _dbContext.Publishers.AddAsync(publisher);
            }
            else
            {
                var publisherInDb = await _dbContext.Publishers.FirstAsync(p => p.Id == publisher.Id);

                publisherInDb.PublisherName = publisher.PublisherName;
            }

            await _dbContext.SaveChangesAsync();

            return RedirectToAction("GetPublishers", "Publishers", new { publisher.Id });
        }

        public async Task<IActionResult> DeletePublisher(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var publisher = await _dbContext.Publishers.FindAsync(id);

            if (publisher == null)
            {
                return NotFound();
            }

            return View(publisher);
        }

        [HttpPost, ActionName("DeletePublisher")]
        public async Task<IActionResult> DeletePublisherConfirm(int id)
        {
            var publisher = await _dbContext.Publishers.FindAsync(id);

            _dbContext.Publishers.Remove(publisher);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("GetPublishers", "Publishers");
        }
    }
}