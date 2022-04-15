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
using GOTHelperEng.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace GOTHelperEng.Controllers
{
    public class TripsController : Controller
    {
        private readonly GOTDatabaseContext _context;

        public TripsController(GOTDatabaseContext context)
        {
            _context = context;
        }

        // GET: Trips
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Trips.ToListAsync());
        }

        // GET: Trips/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trip = await _context.Trips
                .FirstOrDefaultAsync(m => m.TripId == id);
            if (trip == null)
            {
                return NotFound();
            }

            return View(trip);
        }

        // GET: Trips/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Trips/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("TripId,StartDate,EndDate,Points")] Trip trip)
        {
            if (ModelState.IsValid)
            {
                _context.Add(trip);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(trip);
        }

        // GET: Trips/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trip = await _context.Trips.FindAsync(id);
            if (trip == null)
            {
                return NotFound();
            }
            return View(trip);
        }

        // POST: Trips/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("TripId,StartDate,EndDate,Points")] Trip trip)
        {
            if (id != trip.TripId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trip);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TripExists(trip.TripId))
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
            return View(trip);
        }

        // GET: Trips/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trip = await _context.Trips
                .FirstOrDefaultAsync(m => m.TripId == id);
            if (trip == null)
            {
                return NotFound();
            }

            return View(trip);
        }

        // POST: Trips/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trip = await _context.Trips.FindAsync(id);
            _context.Trips.Remove(trip);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TripExists(int id)
        {
            return _context.Trips.Any(e => e.TripId == id);
        }

        //int id is from TripApplication
        [Authorize(Roles = "Admin,Tourist")]
        public async Task<IActionResult> AddStages(int id)
        {
            var trip = await _context.Trips.FindAsync(id);
            if (trip == null)
            {
                return NotFound();
            }

            var positions = _context.Position
                .Include(t => t.Stage)
                .Include(t => t.Trip)
                .Include(t => t.Stage.StartPoint)
                .Include(t => t.Stage.EndPoint)
                .Where(t => t.TripId == id)
                .OrderBy(t => t.StageNumber);

            int? lastStartPtId = null;
            int? lastEndPtId = null;
            int? lastStageNumber = null;
            
            if (positions.Any())
            {
                var lastPosition = positions.LastOrDefault();
                var lastStage = lastPosition.Stage;
                if (lastPosition.Direction.Value == true)
                {
                    lastStartPtId = lastStage.StartPointId.Value;
                    lastEndPtId = lastStage.EndPointId.Value;
                }
                else
                {
                    lastStartPtId = lastStage.EndPointId.Value;
                    lastEndPtId = lastStage.StartPointId.Value;
                }
                lastStageNumber = lastPosition.StageNumber.Value;
                ViewData["Positions"] = positions.ToList();
                Console.WriteLine("LAST DIR: " + lastPosition.Direction.Value);
            }
            else
            {
                ViewData["Positions"] = new List<Position>();
            }

            HttpContext.Session.SetString("LastStartPointID", lastStartPtId.ToString());
            HttpContext.Session.SetString("LastEndPointID", lastEndPtId.ToString());
            HttpContext.Session.SetInt32("TripID", id);
            HttpContext.Session.SetString("LastPosition", lastStageNumber.ToString());
            ViewData["ReturnAppId"] = HttpContext.Session.GetInt32("TripAppID");
            return View(trip);
        }

        // POST: Trips/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Tourist")]
        public async Task<IActionResult> AddStages([Bind("TripId,StartDate,EndDate")] TripViewModel tripView)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction(nameof(TripConfirmed));
            }
            return View(tripView);
        }

        [Authorize(Roles = "Admin,Tourist")]
        public async Task<IActionResult> TripConfirmed()
        {
            return View();
        }
    }
}
