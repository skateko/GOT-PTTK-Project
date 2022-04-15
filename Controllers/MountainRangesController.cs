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
using Microsoft.AspNetCore.Authorization;

namespace GOTHelperEng.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MountainRangesController : Controller
    {
        private readonly GOTDatabaseContext _context;

        public MountainRangesController(GOTDatabaseContext context)
        {
            _context = context;
        }

        // GET: MountainRanges
        public async Task<IActionResult> Index()
        {
            var gOTDatabaseContext = _context.MountainRanges.Include(m => m.MountainArea);
            return View(await gOTDatabaseContext.ToListAsync());
        }

        // GET: MountainRanges/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mountainRange = await _context.MountainRanges
                .Include(m => m.MountainArea)
                .FirstOrDefaultAsync(m => m.MountainRangeId == id);
            if (mountainRange == null)
            {
                return NotFound();
            }

            return View(mountainRange);
        }

        // GET: MountainRanges/Create
        public IActionResult Create()
        {
            ViewData["MountainAreaId"] = new SelectList(_context.MountainAreas, "MountainAreaId", "MoutainAreaName");
            return View();
        }

        // POST: MountainRanges/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MountainRangeId,MountainRangeName,MountainRangeAbbr,MountainAreaId")] MountainRange mountainRange)
        {
            if (ModelState.IsValid)
            {
                _context.Add(mountainRange);
                await _context.SaveChangesAsync();
                Console.WriteLine("DODALO");
                return RedirectToAction(nameof(Index));
            }
            ViewData["MountainAreaId"] = new SelectList(_context.MountainAreas, "MountainAreaId", "MoutainAreaName", mountainRange.MountainAreaId);
            Console.WriteLine($"{mountainRange.MountainRangeId} {mountainRange.MountainRangeName} + {mountainRange.MountainRangeAbbr} { mountainRange.MountainAreaId}");
            foreach (var modelState in ViewData.ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    Console.WriteLine(error);
                }
            }
            return View(mountainRange);
        }

        // GET: MountainRanges/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mountainRange = await _context.MountainRanges.FindAsync(id);
            if (mountainRange == null)
            {
                return NotFound();
            }
            ViewData["MountainAreaId"] = new SelectList(_context.MountainAreas, "MountainAreaId", "MoutainAreaName", mountainRange.MountainAreaId);
            return View(mountainRange);
        }

        // POST: MountainRanges/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MountainRangeId,MountainRangeName,MountainRangeAbbr,MountainAreaId")] MountainRange mountainRange)
        {
            if (id != mountainRange.MountainRangeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mountainRange);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MountainRangeExists(mountainRange.MountainRangeId))
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
            ViewData["MountainAreaId"] = new SelectList(_context.MountainAreas, "MountainAreaId", "MoutainAreaName", mountainRange.MountainAreaId);
            return View(mountainRange);
        }

        // GET: MountainRanges/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mountainRange = await _context.MountainRanges
                .Include(m => m.MountainArea)
                .FirstOrDefaultAsync(m => m.MountainRangeId == id);
            if (mountainRange == null)
            {
                return NotFound();
            }

            return View(mountainRange);
        }

        // POST: MountainRanges/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mountainRange = await _context.MountainRanges.FindAsync(id);
            _context.MountainRanges.Remove(mountainRange);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MountainRangeExists(int id)
        {
            return _context.MountainRanges.Any(e => e.MountainRangeId == id);
        }
    }
}
