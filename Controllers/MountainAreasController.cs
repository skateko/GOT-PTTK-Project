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
    public class MountainAreasController : Controller
    {
        private readonly GOTDatabaseContext _context;

        public MountainAreasController(GOTDatabaseContext context)
        {
            _context = context;
        }

        // GET: MountainAreas
        public async Task<IActionResult> Index()
        {
            return View(await _context.MountainAreas.ToListAsync());
        }

        // GET: MountainAreas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mountainArea = await _context.MountainAreas
                .FirstOrDefaultAsync(m => m.MountainAreaId == id);
            if (mountainArea == null)
            {
                return NotFound();
            }

            return View(mountainArea);
        }

        // GET: MountainAreas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MountainAreas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MountainAreaId,MoutainAreaName")] MountainArea mountainArea)
        {
            if (ModelState.IsValid)
            {
                _context.Add(mountainArea);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(mountainArea);
        }

        // GET: MountainAreas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mountainArea = await _context.MountainAreas.FindAsync(id);
            if (mountainArea == null)
            {
                return NotFound();
            }
            return View(mountainArea);
        }

        // POST: MountainAreas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MountainAreaId,MoutainAreaName")] MountainArea mountainArea)
        {
            if (id != mountainArea.MountainAreaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mountainArea);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MountainAreaExists(mountainArea.MountainAreaId))
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
            return View(mountainArea);
        }

        // GET: MountainAreas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mountainArea = await _context.MountainAreas
                .FirstOrDefaultAsync(m => m.MountainAreaId == id);
            if (mountainArea == null)
            {
                return NotFound();
            }

            return View(mountainArea);
        }

        // POST: MountainAreas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mountainArea = await _context.MountainAreas.FindAsync(id);
            _context.MountainAreas.Remove(mountainArea);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MountainAreaExists(int id)
        {
            return _context.MountainAreas.Any(e => e.MountainAreaId == id);
        }
    }
}
