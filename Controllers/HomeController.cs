﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using college_assignment_mvc_project.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace college_assignment_mvc_project.Controllers
{

    public class HomeController : Controller
    {
        private readonly college_assignment_mvc_projectContext _context;

        public HomeController(college_assignment_mvc_projectContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("UserFirstName") == null)
            {
                HttpContext.Session.SetString("UserFirstName", "Guest");
                HttpContext.Session.SetString("IsUserLoggedIn", "UserNotConnected");
            }
            
            return View(await _context.Track.ToListAsync());
        }

        public IActionResult Search(string search)
        {
            IQueryable<Track> result = null;
            result = _context.Track.Where(t => t.Name.Contains(search));

            ViewData["Message"] = search;
            ViewData["Result"] = result;

            return View();
        }

        public IActionResult Login()
        {
            ViewData["Message"] = "Your login page.";

            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login([Bind(include: "Email,Password")] User user)
        {
            User usr = null;
            var password = user.Password;
            var email = user.Email;
            try
            {
                usr = _context.User.Single(u => u.Email.Equals(email) && u.Password.Equals(password));
                HttpContext.Session.SetString("UserFirstName", usr.FirstName);
                HttpContext.Session.SetString("Role", usr.Role.ToString());
                HttpContext.Session.SetString("IsUserLoggedIn", "UserConnected");
               

                return RedirectToAction("Index", "Home");
            }
            //catch (ArgumentNullException)
            //{
                //should return to error page
                //return RedirectToAction("Login", "Home");
            //}
            catch (InvalidOperationException)
            {
                TempData["login-failure"] = "could not log you in.. wrong email or password";
                return RedirectToAction("Login", "Home");
            }
        }
    }
}
