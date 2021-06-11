using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReportAspNetCore.Convertors;
using ReportAspNetCore.Data;
using ReportAspNetCore.DTOs;
using ReportAspNetCore.Models;
using ReportAspNetCore.Services.Interfaces;
using Stimulsoft.Base;
using Stimulsoft.Base.Licenses;
using Stimulsoft.Report;
using Stimulsoft.Report.Mvc;

namespace ReportAspNetCore.Controllers
{
    
    public class PeopleController : Controller
    {


        #region Constructor & License Stimulsoft

        private readonly ReportAspNetCoreContext _context;

        private readonly IUserService _userService;

        public PeopleController(ReportAspNetCoreContext context, IUserService userService)
        {
            StiLicense.LoadFromString("6vJhGtLLLz2GNviWmUTrhSqnOItdDwjBylQzQcAOiHl2AD0gPVknKsaW0un+3PuM6TTcPMUAWEURKXNso0e5OJN40hxJjK5JbrxU+NrJ3E0OUAve6MDSIxK3504G4vSTqZezogz9ehm+xS8zUyh3tFhCWSvIoPFEEuqZTyO744uk+ezyGDj7C5jJQQjndNuSYeM+UdsAZVREEuyNFHLm7gD9OuR2dWjf8ldIO6Goh3h52+uMZxbUNal/0uomgpx5NklQZwVfjTBOg0xKBLJqZTDKbdtUrnFeTZLQXPhrQA5D+hCvqsj+DE0n6uAvCB2kNOvqlDealr9mE3y978bJuoq1l4UNE3EzDk+UqlPo8KwL1XM+o1oxqZAZWsRmNv4Rr2EXqg/RNUQId47/4JO0ymIF5V4UMeQcPXs9DicCBJO2qz1Y+MIpmMDbSETtJWksDF5ns6+B0R7BsNPX+rw8nvVtKI1OTJ2GmcYBeRkIyCB7f8VefTSOkq5ZeZkI8loPcLsR4fC4TXjJu2loGgy4avJVXk32bt4FFp9ikWocI9OQ7CakMKyAF6Zx7dJF1nZw");
            _context = context;
            _userService = userService;
        }

        #endregion


        #region Register

        [Route("Register")]
        public IActionResult Register()
        {
            return View();
        }


        [Route("Register")]
        [HttpPost]
        public IActionResult Register(RegisterViewModel register)
        {
            if (!ModelState.IsValid)
            {
                return View(register);
            }

            if (_userService.IsExistEmail(FixedText.FixedEmail(register.Email)))
            {
                ModelState.AddModelError("Email", "ایمیل وارد شده معتبر نمیباشد");
                return View(register);
            }

            Models.User user = new User()
            {
                UserName = register.UserName,
                Email = FixedText.FixedEmail(register.Email),
                Password = register.Password,
                RePassword = register.RePassword
            };

            _userService.addUser(user);

            return View("SuccessRegister", user);
        }

        #endregion


        #region Login

        [Route("LoginUserResult")]
        public IActionResult LoginUserResult()
        {
            return View();
        }

        [HttpPost]
        [Route("LoginUser")]
        public IActionResult LoginUserResult(LoginViewModel login)
        {
            if (!ModelState.IsValid)
            {
                return View(login);
            }


            var user = _userService.LoginUser(login);

            if (user != null)
            {

                var claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.NameIdentifier,user.UserId.ToString()),
                        new Claim(ClaimTypes.Name,user.UserName)
                    };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                var properties = new AuthenticationProperties
                {
                    IsPersistent = login.RememberMe
                };
                HttpContext.SignInAsync(principal, properties);

                ViewBag.IsSuccess = true;
                return View();

                
            }

            if (user == null)
            {
                ModelState.AddModelError("Email", "کاربری با مشخصات وارد شده یافت نشد ");
            }




            return View(login);
        }

        #endregion


        #region Logout

        [Route("LogoutUserResult")]
        public IActionResult LogoutUserResult()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/LoginUserResult");
        }

        #endregion


        #region PrintPage

        // GET: People
        public async Task<IActionResult> Index()
        {
            return View(await _context.Person.ToListAsync());
        }

        public IActionResult PrintPage()
        {
            return View("PrintPage");
        }

        public IActionResult Print()
        {
            StiReport report = new StiReport();

            report.Load(StiNetCoreHelper.MapPath(this, "wwwroot/Reports/Report.mrt"));

            var persons = _context.Person.ToList();

            report.RegData("dt", persons);

            return StiNetCoreViewer.GetReportResult(this, report);
        }

        public IActionResult ViewEvent()
        {
            return StiNetCoreViewer.ViewerEventResult(this);
        }

        #endregion


        #region Create New Information

        // GET: People/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _context.Person
                .FirstOrDefaultAsync(m => m.Id == id);
            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        // GET: People/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: People/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FullName,Email,Mobile")] Person person)
        {
            if (ModelState.IsValid)
            {
                _context.Add(person);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(person);
        }

        // GET: People/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _context.Person.FindAsync(id);
            if (person == null)
            {
                return NotFound();
            }
            return View(person);
        }

        // POST: People/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,Email,Mobile")] Person person)
        {
            if (id != person.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(person);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonExists(person.Id))
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
            return View(person);
        }

        // GET: People/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _context.Person
                .FirstOrDefaultAsync(m => m.Id == id);
            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        // POST: People/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var person = await _context.Person.FindAsync(id);
            _context.Person.Remove(person);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PersonExists(int id)
        {
            return _context.Person.Any(e => e.Id == id);
        }

        #endregion


    }
}
