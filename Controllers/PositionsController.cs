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

namespace GOTPO.Controllers
{
    public class PositionsController : Controller
    {
        private readonly GOTDatabaseContext _context;

        public PositionsController(GOTDatabaseContext context)
        {
            _context = context;
        }

        // GET: Positions
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var gOTDatabaseContext = _context.Position.Include(p => p.Stage).Include(p => p.Trip);
            return View(await gOTDatabaseContext.ToListAsync());
        }

        // GET: Positions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var position = await _context.Position
                .Include(p => p.Stage)
                .Include(p => p.Trip)
                .FirstOrDefaultAsync(m => m.PositionId == id);
            if (position == null)
            {
                return NotFound();
            }

            return View(position);
        }

        // GET: Positions/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["StageId"] = new SelectList(_context.Stage, "StageId", "StageId");
            ViewData["TripId"] = new SelectList(_context.Trips, "TripId", "EndDate");
            return View();
        }

        // POST: Positions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("PositionId,StageNumber,Direction,TripId,StageId")] Position position)
        {
            if (ModelState.IsValid)
            {
                _context.Add(position);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["StageId"] = new SelectList(_context.Stage, "StageId", "StageId", position.StageId);
            ViewData["TripId"] = new SelectList(_context.Trips, "TripId", "EndDate", position.TripId);
            return View(position);
        }

        // GET: Positions/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var position = await _context.Position.FindAsync(id);
            if (position == null)
            {
                return NotFound();
            }
            ViewData["StageId"] = new SelectList(_context.Stage, "StageId", "StageId", position.StageId);
            ViewData["TripId"] = new SelectList(_context.Trips, "TripId", "EndDate", position.TripId);
            return View(position);
        }

        // POST: Positions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("PositionId,StageNumber,Direction,TripId,StageId")] Position position)
        {
            if (id != position.PositionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(position);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PositionExists(position.PositionId))
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
            ViewData["StageId"] = new SelectList(_context.Stage, "StageId", "StageId", position.StageId);
            ViewData["TripId"] = new SelectList(_context.Trips, "TripId", "EndDate", position.TripId);
            return View(position);
        }

        // GET: Positions/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var position = await _context.Position
                .Include(p => p.Stage)
                .Include(p => p.Trip)
                .FirstOrDefaultAsync(m => m.PositionId == id);
            if (position == null)
            {
                return NotFound();
            }

            return View(position);
        }

        // POST: Positions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var position = await _context.Position.FindAsync(id);
            _context.Position.Remove(position);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PositionExists(int id)
        {
            return _context.Position.Any(e => e.PositionId == id);
        }

        [Authorize(Roles = "Admin,Tourist")]
        public IActionResult CreatePos()
        {
            bool success = Int32.TryParse(HttpContext.Session.GetString("LastEndPointID"), out var LastPointID);
            if (success)
            {
                var points = _context.Points.Where(point => point.PointId == LastPointID);
                ViewData["YourLastPoint"] = points.FirstOrDefault().PointName;
                ViewData["StageId"] = new SelectList(_context.Stage.Where(stage => (stage.StartPointId == LastPointID || stage.EndPointId == LastPointID && stage.PointsBackwards != null)), "StageId", "RouteDescription");
            }
            else
            {
                ViewData["StageId"] = new SelectList(_context.Stage, "StageId", "RouteDescription");
            }

            makeReturnData();
            return View();
        }

        // POST: Positions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Tourist")]
        public async Task<IActionResult> CreatePos([Bind("PositionId,Direction,StageId")] PositionViewModel positionVM)
        {
            bool LastParsed = Int32.TryParse(HttpContext.Session.GetString("LastEndPointID"), out var lastEndPtId);
            bool StartParsed = Int32.TryParse(HttpContext.Session.GetString("LastStartPointID"), out var LastStPtId);
            if (ModelState.IsValid)
            {
                var addedStage = _context.Stage.Where(stage => stage.StageId == positionVM.StageId).FirstOrDefault();
                if (LastParsed && StartParsed)
                {
                    if (positionVM.Direction.Value == true)
                    {
                        if (addedStage.StartPointId == LastStPtId && addedStage.EndPointId == lastEndPtId)
                        {
                            ViewData["BackwardPointsErr"] = $"Selected stage has the same both starting and ending point as the previous one, please add another stage";
                            ViewData["StageId"] = new SelectList(_context.Stage.Where(stage => (stage.StartPointId == lastEndPtId || stage.EndPointId == lastEndPtId && stage.PointsBackwards != null)), "StageId", "RouteDescription");
                            makeReturnData();
                            return View(positionVM);
                        }
                        if (addedStage.StartPointId != lastEndPtId)
                        {
                            ViewData["BackwardPointsErr"] = $"Selected stage does not start at the beginning of the last stage, please add another stage";
                            ViewData["StageId"] = new SelectList(_context.Stage.Where(stage => (stage.StartPointId == lastEndPtId || stage.EndPointId == lastEndPtId && stage.PointsBackwards != null)), "StageId", "RouteDescription");
                            makeReturnData();
                            return View(positionVM);
                        }
                    }
                    else
                    {
                        if (addedStage.PointsBackwards == null)
                        {
                            ViewData["BackwardPointsErr"] = "Selected stage does not have backwards points, please add another stage.";
                            ViewData["StageId"] = new SelectList(_context.Stage.Where(stage => (stage.StartPointId == lastEndPtId || stage.EndPointId == lastEndPtId && stage.PointsBackwards != null)), "StageId", "RouteDescription");
                            makeReturnData();
                            return View(positionVM);
                        }
                        if (addedStage.EndPointId != lastEndPtId)
                        {
                            ViewData["BackwardPointsErr"] = $"Selected stage does not start at the beginning of the last stage, please add another stage";
                            ViewData["StageId"] = new SelectList(_context.Stage.Where(stage => (stage.StartPointId == lastEndPtId || stage.EndPointId == lastEndPtId && stage.PointsBackwards != null)), "StageId", "RouteDescription");
                            makeReturnData();
                            return View(positionVM);
                        }
                    }
                }
                else
                {
                    if (positionVM.Direction == false && addedStage.PointsBackwards == null)
                    {
                        ViewData["BackwardPointsErr"] = "Selected stage does not have backwards points, please add another stage";
                        ViewData["StageId"] = new SelectList(_context.Stage, "StageId", "RouteDescription");
                        makeReturnData();
                        return View(positionVM);
                    }
                }

                int tripId = HttpContext.Session.GetInt32("TripID").Value;
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
                    Direction = positionVM.Direction,
                    TripId = tripId,
                    StageId = positionVM.StageId,
                };

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
                foreach (var pos in positions)
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
            if (LastParsed && StartParsed)
            {
                ViewData["StageId"] = new SelectList(_context.Stage.Where(stage => (stage.StartPointId == lastEndPtId || stage.EndPointId == lastEndPtId && stage.PointsBackwards != null)), "StageId", "RouteDescription");
            }
            else
            {
                ViewData["StageId"] = new SelectList(_context.Stage, "StageId", "RouteDescription");
            }
            makeReturnData();
            return View(positionVM);
        }
        private void makeReturnData()
        {
            List<Object> directions = new List<Object>();
            directions.Add(new { Value = true, Text = "Tam" });
            directions.Add(new { Value = false, Text = "Z Powrotem" });
            ViewData["Direction"] = new SelectList(directions, "Value", "Text");
            ViewData["ReturnId"] = HttpContext.Session.GetInt32("TripID").Value;
            return;
        }
        private bool TripExists(int id)
        {
            return _context.Trips.Any(e => e.TripId == id);
        }
    }
}
