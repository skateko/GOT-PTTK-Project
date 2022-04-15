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
using GOTHelperEng.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace GOTHelperEng.Controllers
{
    public class StagesController : Controller
    {
        private readonly GOTDatabaseContext _context;
        private readonly UserManager<User> _userManager;

        public StagesController(GOTDatabaseContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Stages
        [AllowAnonymous]
        public async Task<IActionResult> Index(string? pointName)
        {
            ViewData["pointName"] = pointName;
            var gOTDatabaseContext = _context.Stage.Include(s => s.EndPoint).Include(s => s.MountainRange).Include(s => s.StartPoint).Include(s => s.Tourist).AsQueryable();

            if (pointName != null)
            {
                gOTDatabaseContext = gOTDatabaseContext
                    .Where(s => s.EndPoint.PointName.Contains(pointName) || s.StartPoint.PointName.Contains(pointName));
            }

            if (!User.IsInRole("Admin"))
            {
                return View(await gOTDatabaseContext.Where(s => s.TouristId == null).ToListAsync());
            }
            else
            {
                return View(await gOTDatabaseContext.ToListAsync());
            }
        }

        // GET: Stages/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stage = await _context.Stage
                .Include(s => s.EndPoint)
                .Include(s => s.MountainRange)
                .Include(s => s.StartPoint)
                .Include(s => s.Tourist)
                .FirstOrDefaultAsync(m => m.StageId == id);
            if (stage == null)
            {
                return NotFound();
            }

            return View(stage);
        }

        // GET: Stages/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["EndPointId"] = new SelectList(_context.Points, "PointId", "PointName");
            ViewData["MountainRangeId"] = new SelectList(_context.MountainRanges, "MountainRangeId", "MountainRangeName");
            ViewData["StartPointId"] = new SelectList(_context.Points, "PointId", "PointName");
            ViewData["TouristId"] = new SelectList(_context.Tourists, "TouristId", "UserId");
            return View();
        }

        // POST: Stages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("StageId,Length,PointsForwards,PointsBackwards,MountainRangeId,StartPointId,EndPointId")] Stage stage)
        {
            if (ModelState.IsValid)
            {
                var descr1 = _context.Points.Where(pt => pt.PointId == stage.StartPointId).FirstOrDefault().PointName;
                var descr2 = _context.Points.Where(pt => pt.PointId == stage.EndPointId).FirstOrDefault().PointName;
                Stage stageToSave = new Stage()
                {
                    Length = stage.Length,
                    PointsForwards = stage.PointsForwards,
                    PointsBackwards = stage.PointsBackwards,
                    RouteDescription = $"{descr1} -> {descr2} PF: {stage.PointsForwards}, PB: {stage.PointsBackwards}",
                    MountainRangeId = stage.MountainRangeId,
                    StartPointId = stage.StartPointId,
                    EndPointId = stage.EndPointId,
                };
                _context.Add(stageToSave);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EndPointId"] = new SelectList(_context.Points, "PointId", "PointName", stage.EndPointId);
            ViewData["MountainRangeId"] = new SelectList(_context.MountainRanges, "MountainRangeId", "MountainRangeName", stage.MountainRangeId);
            ViewData["StartPointId"] = new SelectList(_context.Points, "PointId", "PointName", stage.StartPointId);
            ViewData["TouristId"] = new SelectList(_context.Tourists, "TouristId", "UserId", stage.TouristId);
            return View(stage);
        }

        // GET: Stages/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stage = await _context.Stage.FindAsync(id);
            if (stage == null)
            {
                return NotFound();
            }
            ViewData["EndPointId"] = new SelectList(_context.Points, "PointId", "PointName", stage.EndPointId);
            ViewData["MountainRangeId"] = new SelectList(_context.MountainRanges, "MountainRangeId", "MountainRangeName", stage.MountainRangeId);
            ViewData["StartPointId"] = new SelectList(_context.Points, "PointId", "PointName", stage.StartPointId);
            ViewData["TouristId"] = new SelectList(_context.Tourists, "TouristId", "UserId", stage.TouristId);
            return View(stage);
        }

        // POST: Stages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("StageId,Length,PointsForwards,PointsBackwards,RouteDescription,MountainRangeId,StartPointId,EndPointId,TouristId")] Stage stage)
        {
            if (id != stage.StageId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(stage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StageExists(stage.StageId))
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
            ViewData["EndPointId"] = new SelectList(_context.Points, "PointId", "PointName", stage.EndPointId);
            ViewData["MountainRangeId"] = new SelectList(_context.MountainRanges, "MountainRangeId", "MountainRangeName", stage.MountainRangeId);
            ViewData["StartPointId"] = new SelectList(_context.Points, "PointId", "PointName", stage.StartPointId);
            ViewData["TouristId"] = new SelectList(_context.Tourists, "TouristId", "UserId", stage.TouristId);
            return View(stage);
        }

        // GET: Stages/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stage = await _context.Stage
                .Include(s => s.EndPoint)
                .Include(s => s.MountainRange)
                .Include(s => s.StartPoint)
                .Include(s => s.Tourist)
                .FirstOrDefaultAsync(m => m.StageId == id);
            if (stage == null)
            {
                return NotFound();
            }

            return View(stage);
        }

        // POST: Stages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var stage = await _context.Stage.FindAsync(id);
            _context.Stage.Remove(stage);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StageExists(int id)
        {
            return _context.Stage.Any(e => e.StageId == id);
        }
        //TouristId = _context.Tourists.Where(t => t.UserId == userId).Select(t => t.TouristId).ToList().FirstOrDefault()

        // GET: Stages/Create/5
        [Authorize(Roles = "Admin,Tourist")]
        public IActionResult CreateOwn(int? id)
        {
            bool parsed = Int32.TryParse(HttpContext.Session.GetString("LastEndPointID"), out var lastEndPtId);
            if (parsed)
            {
                var points = _context.Points.Where(point => point.PointId == lastEndPtId);
                ViewData["StartPointId"] = new SelectList(points, "PointId", "PointName");
                ViewData["YourLastPoint"] = points.FirstOrDefault().PointName;
            }
            else
            {
                ViewData["StartPointId"] = new SelectList(_context.Points, "PointId", "PointName");
            }

            ViewData["EndPointId"] = new SelectList(_context.Points, "PointId", "PointName");
            ViewData["MountainRangeId"] = new SelectList(_context.MountainRanges, "MountainRangeId", "MountainRangeName");
            ViewData["ReturnId"] = HttpContext.Session.GetInt32("TripID").Value;
            return View();
        }

        // POST: Stages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Tourist")]
        public async Task<IActionResult> CreateOwn([Bind("StageId,LengthInMeters,HeightDifference,MountainRangeId,StartPointId,EndPointId")] StageViewModel stageVM)
        {
            var touristId = _context.Tourists.Where(t => t.UserId == _userManager.GetUserId(User)).Select(t => t.TouristId).ToList().FirstOrDefault();
            if (ModelState.IsValid)
            {
                int tripId = HttpContext.Session.GetInt32("TripID").Value;
                double length = stageVM.LengthInMeters.Value;
                double height = stageVM.HeightDifference.Value;
                double points = Math.Round(length / 1000, MidpointRounding.AwayFromZero) + Math.Round(height / 100, MidpointRounding.AwayFromZero);
                var descr1 = _context.Points.Where(pt => pt.PointId == stageVM.StartPointId).FirstOrDefault().PointName;
                var descr2 = _context.Points.Where(pt => pt.PointId == stageVM.EndPointId).FirstOrDefault().PointName;

                Stage stage = new Stage()
                {
                    Length = stageVM.LengthInMeters,
                    PointsForwards = (int)points,
                    PointsBackwards = 0,
                    RouteDescription = $"{descr1} -> {descr2} P: {(int)points}",
                    MountainRangeId = stageVM.MountainRangeId,
                    StartPointId = stageVM.StartPointId,
                    EndPointId = stageVM.EndPointId,
                    TouristId = touristId,
                };

                int stageposition;
                bool success = Int32.TryParse(HttpContext.Session.GetString("LastPosition"), out var lastPosition);
                if (success)
                {
                    stageposition = lastPosition + 1;
                }
                else
                {
                    stageposition = 1;
                }

                Position position = new Position()
                {
                    StageNumber = stageposition,
                    Direction = true,
                    TripId = tripId,
                    Stage = stage,
                };

                _context.Add(stage);
                _context.Add(position);
                _context.SaveChanges();

                var trip = _context.Trips.Find(tripId);
                if (trip == null)
                {
                    return NotFound();
                }

                var positions = _context.Position
                        .Include(t => t.Stage)
                        .Where(t => t.TripId == tripId);

                int? pointsForTrip = 0;

                foreach(var pos in positions)
                {
                    if (pos.Direction.Value)
                    {
                        pointsForTrip += pos.Stage.PointsForwards;
                    }
                    else
                    {
                        pointsForTrip += pos.Stage.PointsBackwards;
                    }
                }

                trip.Points = pointsForTrip;

                try
                {
                    _context.Update(trip);
                    _context.SaveChanges();
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
                return RedirectToAction("AddStages", "Trips", new { id = tripId });
            }
            ViewData["EndPointId"] = new SelectList(_context.Points, "PointId", "PointName", stageVM.EndPointId);
            ViewData["MountainRangeId"] = new SelectList(_context.MountainRanges, "MountainRangeId", "MountainRangeName", stageVM.MountainRangeId);
            ViewData["StartPointId"] = new SelectList(_context.Points, "PointId", "PointName", stageVM.StartPointId);
            return View(stageVM);
        }

        private bool TripExists(int id)
        {
            return _context.Trips.Any(e => e.TripId == id);
        }
    }
}
