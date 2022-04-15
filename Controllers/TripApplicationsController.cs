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
    public class TripApplicationsController : Controller
    {
        private readonly GOTDatabaseContext _context;

        public TripApplicationsController(GOTDatabaseContext context)
        {
            _context = context;
        }

        // GET: TripApplications
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var gOTDatabaseContext = _context.TripApplications.Include(t => t.Ksiazeczka).Include(t => t.Trip);
            return View(await gOTDatabaseContext.ToListAsync());
        }

        // GET: TripApplications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tripApplication = await _context.TripApplications
                .Include(t => t.Ksiazeczka)
                .Include(t => t.Trip)
                .FirstOrDefaultAsync(m => m.TripApplicationId == id);

            if (tripApplication == null)
            {
                return NotFound();
            }

            var positions = _context.Position
                .Include(t => t.Stage)
                .Include(t => t.Trip)
                .Include(t => t.Stage.StartPoint)
                .Include(t => t.Stage.EndPoint)
                .Where(t => t.TripId == tripApplication.TripId)
                .OrderBy(t => t.StageNumber);

            if (positions.Any())
            {
                ViewData["Positions"] = positions.ToList();
            }
            else
            {
                ViewData["Positions"] = new List<Position>();
            }

            HttpContext.Session.SetInt32("TripAppID", id.Value);
            ViewData["ReturnBookId"] = HttpContext.Session.GetInt32("BookletID");
            return View(tripApplication);
        }

        // GET: TripApplications/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["BookletId"] = new SelectList(_context.Booklets, "BookletId", "BookletId");
            ViewData["TripId"] = new SelectList(_context.Trips, "TripId", "EndDate");
            return View();
        }

        // POST: TripApplications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("TripApplicationId,CreationDate,IsApproved,TripId,BookletId")] TripApplication tripApplication)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tripApplication);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BookletId"] = new SelectList(_context.Booklets, "BookletId", "BookletId", tripApplication.BookletId);
            ViewData["TripId"] = new SelectList(_context.Trips, "TripId", "EndDate", tripApplication.TripId);
            return View(tripApplication);
        }

        // GET: TripApplications/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tripApplication = await _context.TripApplications.FindAsync(id);
            if (tripApplication == null)
            {
                return NotFound();
            }
            ViewData["BookletId"] = new SelectList(_context.Booklets, "BookletId", "BookletId", tripApplication.BookletId);
            ViewData["TripId"] = new SelectList(_context.Trips, "TripId", "EndDate", tripApplication.TripId);
            return View(tripApplication);
        }

        // POST: TripApplications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("TripApplicationId,CreationDate,IsApproved,TripId,BookletId")] TripApplication tripApplication)
        {
            if (id != tripApplication.TripApplicationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tripApplication);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TripApplicationExists(tripApplication.TripApplicationId))
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
            ViewData["BookletId"] = new SelectList(_context.Booklets, "BookletId", "BookletId", tripApplication.BookletId);
            ViewData["TripId"] = new SelectList(_context.Trips, "TripId", "EndDate", tripApplication.TripId);
            return View(tripApplication);
        }

        // GET: TripApplications/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tripApplication = await _context.TripApplications
                .Include(t => t.Ksiazeczka)
                .Include(t => t.Trip)
                .FirstOrDefaultAsync(m => m.TripApplicationId == id);
            if (tripApplication == null)
            {
                return NotFound();
            }

            return View(tripApplication);
        }

        // POST: TripApplications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tripApplication = await _context.TripApplications.FindAsync(id);
            _context.TripApplications.Remove(tripApplication);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TripApplicationExists(int id)
        {
            return _context.TripApplications.Any(e => e.TripApplicationId == id);
        }

        // GET: TripApplications/BookletApplications/5
        // This int id is from Booklet
        [Authorize(Roles = "Admin,Tourist")]
        public async Task<IActionResult> BookletApplications(int id)
        {
            HttpContext.Session.SetInt32("BookletID", id);
            ViewData["BookletID"] = id;
            var gOTDatabaseContext = _context.TripApplications.Include(t => t.Ksiazeczka).Include(t => t.Trip).Where(t => t.BookletId == id);
            return View(await gOTDatabaseContext.ToListAsync());
        }

        [Authorize(Roles = "Admin,Tourist")]
        public async Task<IActionResult> CreateApp(int id)
        {
            ViewData["BookletID"] = id;
            return View();
        }

        // POST: TripApplications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Tourist")]
        public async Task<IActionResult> CreateApp([Bind("TripApplicationId,BookletId,TripStartDate,TripEndDate")] TripApplicationViewModel tripApplication)
        {
            
            if (ModelState.IsValid)
            {
                int days = (Convert.ToDateTime(tripApplication.TripEndDate).Date - Convert.ToDateTime(tripApplication.TripStartDate).Date).Days;
                if (days < 0)
                {
                    ViewData["DateError"] = $"End date is not after start date, {days}";
                    return View(tripApplication);
                }
                Trip trip = new Trip()
                {
                    StartDate = tripApplication.TripStartDate,
                    EndDate = tripApplication.TripEndDate,
                    Points = 0,
                };

                TripApplication tripApp = new TripApplication()
                {
                    CreationDate = DateTime.Now.ToString("dd/MM/yyyy"),
                    IsApproved = false,
                    Trip = trip,
                    BookletId = tripApplication.BookletId,
                };
                
                _context.Add(trip);
                _context.Add(tripApp);
                _context.SaveChanges();

                HttpContext.Session.SetInt32("TripAppID", tripApp.TripApplicationId);
                return RedirectToAction("AddStages" , "Trips", new { id = trip.TripId });
            }

            ViewData["BookletID"] = tripApplication.BookletId;
            //ViewData["TripId"] = new SelectList(_context.Trips, "TripId", "EndDate", tripApplication.TripId);
            //Console.WriteLine(tripApplication.TripStartDate);
            //Console.WriteLine(tripApplication.TripEndDate);
            //Console.WriteLine(tripApplication.TripApplicationId);
            //Console.WriteLine(tripApplication.BookletId);
            //Console.WriteLine();
            //Console.WriteLine();
            return View(tripApplication);
        }

        public void SetCookie(string key, string value, int expireTimeDays)
        {
            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(expireTimeDays);
            Response.Cookies.Append(key, value, option);
        }
    }
}
