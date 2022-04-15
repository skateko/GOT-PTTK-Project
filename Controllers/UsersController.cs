#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GOTHelperEng.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GOTHelperEng.Data;
using GOTHelperEng.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using Microsoft.AspNetCore.Identity.UI.Services;
using GOTHelperEng.Areas.Identity.Pages.Account;
using System.Text.Encodings.Web;

namespace GOTHelperEng.Controllers
{

    public class UsersController : Controller
    {
        private readonly GOTDatabaseContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IUserStore<User> _userStore;
        private readonly IUserEmailStore<User> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<User> _signInManager;

        public UsersController(GOTDatabaseContext context, UserManager<User> userManager, IUserStore<User> userStore, ILogger<RegisterModel> logger, IEmailSender emailSender, SignInManager<User> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _logger = logger;
            _emailSender = emailSender;
            _signInManager = signInManager;
        }


        // GET: Tourists
        public async Task<IActionResult> Index()
        {
            var gOTDatabaseContext = await _context.Users.Include(t => t.Gender).ToListAsync();
            var userViewDatabaseContext = gOTDatabaseContext.Select(user =>
            {
                return new UserViewModel()
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Surname = user.Surname,
                    GenderId = user.GenderId,
                    RoleName = _userManager.GetRolesAsync(user).Result.FirstOrDefault()
                };
            }).ToList();
            return View(userViewDatabaseContext);
        }

        // GET: Tourists/Details/5
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tourist = await _context.Users
                .Include(t => t.Gender)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tourist == null)
            {
                return NotFound();
            }
            ViewData["Role"] = (await _userManager.GetRolesAsync(await _userManager.FindByIdAsync(id))).FirstOrDefault();
            return View(tourist);
        }

        // GET: Tourists/Create
        public IActionResult Create()
        {
            ViewData["GenderId"] = new SelectList(_context.Set<Gender>(), "GenderId", "GenderName");
            ViewData["RoleName"] = new SelectList(_context.Roles, "Name", "Name");
            return View();
        }

        // POST: Tourists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name, Surname, Email, Password, GenderId,RoleName")]UserViewModel userViewModel)
        {
            if (ModelState.IsValid)
            {
                var  user= CreateUser();
                user.Name = userViewModel.Name;
                user.Surname = userViewModel.Surname;
                user.GenderId = userViewModel.GenderId;
                await _userStore.SetUserNameAsync(user, userViewModel.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, userViewModel.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, userViewModel.Password);

                if (result.Succeeded)
                {
                    var role = userViewModel.RoleName;
                    _logger.LogInformation("User created a new account with password.");
                    var userId = await _userManager.GetUserIdAsync(user);
                    _userManager.AddToRoleAsync(user, userViewModel.RoleName).Wait();
                    if (role == "Tourist")
                    {
                        _context.Add(new Tourist()
                        {
                            TouristNumber = _context.Tourists.Select(t => t.TouristNumber).ToList().LastOrDefault(0) + 1,
                            User = user,
                            UserId = userId,
                        });
                        _context.SaveChanges();
                        _context.Add(new Booklet()
                        {
                            TouristId = _context.Tourists.Where(t => t.UserId == userId).Select(t => t.TouristId).ToList().FirstOrDefault(),
                            CreationDate = DateTime.Now.ToString("dd/MM/yyyy"),
                        });
                        _context.SaveChanges();                    }
                    if (role == "Admin")
                    {
                        _context.Add(new Administrator()
                        {
                            User = user,
                            UserId = userId
                        });
                        _context.SaveChanges();
                    }
                    if (role == "Leader")
                    {
                        _context.Add(new Leader()
                        {
                            User = user,
                            UserId = userId,
                            LeaderNumber = _context.Leaders.Select(l => l.LeaderNumber).ToList().LastOrDefault(0) + 1,
                            HireDate = DateTime.Now.ToString("dd/MM/yyyy")
                        });
                        _context.SaveChanges();
                    }
                    return RedirectToAction(nameof(Index));


                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            ViewData["RoleName"] = new SelectList(_context.Roles, "Name", "Name", userViewModel.RoleName);
            ViewData["GenderId"] = new SelectList(_context.Set<Gender>(), "GenderId", "GenderName", userViewModel.GenderId);
            return View(userViewModel);
        }

        // GET: Tourists/Edit/5
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["GenderId"] = new SelectList(_context.Set<Gender>(), "GenderId", "GenderName", user.GenderId);
            ViewData["RoleName"] = new SelectList(_context.Roles, "Name", "Name", (await _userManager.GetRolesAsync(user)).FirstOrDefault());

            return View(new UserEditViewModel()
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Surname = user.Surname,
                GenderId = user.GenderId,
                RoleName = _userManager.GetRolesAsync(user).Result.FirstOrDefault()
            });
        }

        // POST: Tourists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Email, Name, Surname, Password, GenderId,RoleName")] UserEditViewModel userViewModel)
        {
          
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.FindByIdAsync(id);
                    user.Email = userViewModel.Email;
                    user.Name = userViewModel.Name;
                    user.Surname = userViewModel.Surname;
                    user.GenderId = userViewModel.GenderId;
                    if (userViewModel.Password != null)
                    {
                        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                        await _userManager.ResetPasswordAsync(user, token, userViewModel.Password);
                    }
                    var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
                    if (userViewModel.RoleName != role)
                    {
                        if (role != null)
                        {
                            await _userManager.RemoveFromRoleAsync(user, role);
                        }
                        await _userManager.AddToRoleAsync(user, userViewModel.RoleName);
                    }
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                
               
            }
            ViewData["RoleName"] = new SelectList(_context.Roles, "Name", "Name", userViewModel.RoleName);
            ViewData["GenderId"] = new SelectList(_context.Set<Gender>(), "GenderId", "GenderName", userViewModel.GenderId);

            return View(userViewModel);            
        }
      

        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
        private User CreateUser()
        {
            try
            {
                return Activator.CreateInstance<User>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(User)}'. " +
                    $"Ensure that '{nameof(User)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<User> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<User>)_userStore;
        }
    }

}

