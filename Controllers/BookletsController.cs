#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GOTHelperEng.Data;
using GOTHelperEng.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace GOTHelperEng.Controllers
{
    public class BookletsController : Controller
    {
        private readonly GOTDatabaseContext _context;
        private readonly UserManager<User> _userManager;

        public BookletsController(GOTDatabaseContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Booklets
        [Authorize(Roles = "Admin,Tourist")]
        public async Task<IActionResult> Index()
        {
            var gOTDatabaseContext = _context.Booklets.Include(b => b.Tourist);
            if (!User.IsInRole("Admin"))
            {
                return View(await gOTDatabaseContext
                    .Where(booklet => booklet.Tourist.User.Id == _userManager.GetUserId(User)).ToListAsync());
            }
            else
            {
                return View(await gOTDatabaseContext.ToListAsync());
            }
        }

        // GET: Booklets/Details/5
        [Authorize(Roles = "Admin,Tourist")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booklet = await _context.Booklets
                .Include(b => b.Tourist)
                .FirstOrDefaultAsync(m => m.BookletId == id);
            if (booklet == null)
            {
                return NotFound();
            }

            return View(booklet);
        }

        // GET: Booklets/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["TouristId"] = new SelectList(_context.Tourists, "TouristId", "UserId");
            return View();
        }

        // POST: Booklets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("BookletId,TouristId,CreationDate")] Booklet booklet)
        {
            if (ModelState.IsValid)
            {
                _context.Add(booklet);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TouristId"] = new SelectList(_context.Tourists, "TouristId", "UserId", booklet.TouristId);
            return View(booklet);
        }

        // GET: Booklets/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booklet = await _context.Booklets.FindAsync(id);
            if (booklet == null)
            {
                return NotFound();
            }
            ViewData["TouristId"] = new SelectList(_context.Tourists, "TouristId", "UserId", booklet.TouristId);
            return View(booklet);
        }

        // POST: Booklets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("BookletId,TouristId,CreationDate")] Booklet booklet)
        {
            if (id != booklet.BookletId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booklet);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookletExists(booklet.BookletId))
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
            ViewData["TouristId"] = new SelectList(_context.Tourists, "TouristId", "UserId", booklet.TouristId);
            return View(booklet);
        }

        // GET: Booklets/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booklet = await _context.Booklets
                .Include(b => b.Tourist)
                .FirstOrDefaultAsync(m => m.BookletId == id);
            if (booklet == null)
            {
                return NotFound();
            }

            return View(booklet);
        }

        // POST: Booklets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booklet = await _context.Booklets.FindAsync(id);
            _context.Booklets.Remove(booklet);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookletExists(int id)
        {
            return _context.Booklets.Any(e => e.BookletId == id);
        }
    }
}
