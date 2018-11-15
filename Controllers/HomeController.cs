using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using BeltExam.Models;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BeltExam.Controllers
{
    public class HomeController : Controller
    {

        private YourContext dbContext;
 
        // here we can "inject" our context service into the constructor
        public HomeController(YourContext context)
        {
            dbContext = context;
        }
        // GET: /Home/
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [Route("Register")]
        public IActionResult Register(User adduser)
        {
            if(ModelState.IsValid)
            {
                if (dbContext.User.Any(user => user.Email == adduser.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use!");
                    return View("Index");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                User AddUserinDB = new User
                {
                    FirstName = adduser.FirstName,
                    LastName = adduser.LastName,
                    Email = adduser.Email,
                    Password = adduser.Password,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                AddUserinDB.Password = Hasher.HashPassword(AddUserinDB, AddUserinDB.Password);
                dbContext.Add(AddUserinDB);
                dbContext.SaveChanges();
                AddUserinDB = dbContext.User.SingleOrDefault(user => user.Email == AddUserinDB.Email);

                var activeuser = dbContext.User.SingleOrDefault(user => user.Email ==adduser.Email);
                HttpContext.Session.SetString("Loggedinuser", activeuser.FirstName + " " + activeuser.LastName);
                HttpContext.Session.SetInt32("UserId", AddUserinDB.UserId);
                ViewBag.UserName=HttpContext.Session.GetString("Loggedinuser");
                System.Console.WriteLine(HttpContext.Session.GetString("Loggedinuser"));

                
                return RedirectToAction ("Success");
            }
            else
            {
                return View("Index", adduser);
            }
        }

        [HttpPost("login")]
        public IActionResult Login(LoginUser loginuser)
        {
            if(ModelState.IsValid) {
                var Hasher = new PasswordHasher<User>();
                User usercheck = dbContext.User.SingleOrDefault(user => user.Email == loginuser.loginemail);
                if (usercheck == null || 0 == Hasher.VerifyHashedPassword(usercheck, usercheck.Password, loginuser.loginpw))
                {
                    ViewBag.Message = "You could not be logged in. Please try again.";
                    return View("Login");
                }
                else
                {
                    var activeuser = dbContext.User.SingleOrDefault(user => user.Email ==loginuser.loginemail);
                    HttpContext.Session.SetString("Loggedinuser", activeuser.FirstName + " " + activeuser.LastName);
                    HttpContext.Session.SetInt32("UserId", usercheck.UserId);
                    return RedirectToAction("Success");
                }
            }

            else {
                return View("Login");
            }
            
        }

        [HttpGet("Home")]
        public IActionResult Success()
        {
            var activeuser = dbContext.User.SingleOrDefault(user => user.UserId == HttpContext.Session.GetInt32("UserId"));
            if (HttpContext.Session.GetInt32("UserId")!= null){
                ViewBag.User=activeuser;
                List<Activity> AllActivities = dbContext.Activities.Include(p => p.RSVPs).ThenInclude(p => p.User).Include(p => p.User).Where(p => p.ActivityDate > DateTime.Now).OrderBy(p => p.ActivityDate).ToList();
                List<Activity> Activities = dbContext.Activities.ToList();
                ViewBag.AllActivities=AllActivities;

                return View();
            }
            else {
                return View("Index");
            }
            
        }

        [HttpGet("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return View("Index");
        }

        [HttpGet("New")]
        public IActionResult ActivityForm()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpGet("activity/{ActivityId}")]
        public IActionResult ShowActivity(int ActivityId)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index");
            }
            Activity activity = dbContext.Activities.Include(p => p.RSVPs).ThenInclude(p => p.User).Include(p => p.User).SingleOrDefault(p => p.ActivityId == ActivityId);
            ViewBag.activity = activity;
            var activeuser = dbContext.User.SingleOrDefault(user => user.UserId == HttpContext.Session.GetInt32("UserId"));
            ViewBag.User=activeuser;
        
            return View();
        }

        [HttpPost("New")]
        public IActionResult NewActivity(Activity activity)
        {
            if(ModelState.IsValid) {
                if(activity.ActivityDate < DateTime.Now) {
                    ModelState.AddModelError("ActivityDate", "The activity must be in the future");
                    return View("ActivityForm");
                }
                else {
                    Activity newActivity = new Activity 
                {
                    ActivityTitle = activity.ActivityTitle,
                    ActivityDate = activity.ActivityDate,
                    Duration = activity.Duration,
                    DurationType = activity.DurationType,
                    Description = activity.Description,
                    User = dbContext.User.SingleOrDefault(p => p.UserId == HttpContext.Session.GetInt32("UserId"))
                };

                dbContext.Add(newActivity);
                dbContext.SaveChanges();
                return RedirectToAction("ShowActivity", new { ActivityId = newActivity.ActivityId});
                }

                
            }
            else {
                
                return View("ActivityForm");
            }  
        }

        [HttpGet("rsvp/{ActivityId}")]
        public IActionResult RSVP(int ActivityId)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index");
            }

            int? userId = HttpContext.Session.GetInt32 ("UserId");

            User userToJoin = dbContext.User
                .FirstOrDefault (u => u.UserId == userId);

            Activity activityToJoin = dbContext.Activities
                .Include (g => g.RSVPs)
                .FirstOrDefault (w => w.ActivityId == ActivityId);
            RSVP newGuest = new RSVP
            {
                UserId = (int) userId,
                ActivityId = ActivityId,
                User = userToJoin,
                ActivityAttended = activityToJoin
            };
            dbContext.RSVPs.Add(newGuest);
            dbContext.SaveChanges();
            return RedirectToAction ("Success");
        }

        [HttpGet("unrsvp/{ActivityId}")]
        public IActionResult UNRSVP(int ActivityId)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index");
            }

            int? userId = HttpContext.Session.GetInt32 ("UserId");
            RSVP rsvp = dbContext.RSVPs
                .FirstOrDefault (g => g.ActivityId == ActivityId && g.UserId == userId);

            dbContext.RSVPs.Remove(rsvp);
            dbContext.SaveChanges();
            return RedirectToAction ("Success");
        }

        [HttpGet("delete/{ActivityId}")]
        public IActionResult DeleteWedding(int ActivityId)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index");
            }
            Activity deletedActivity = dbContext.Activities.FirstOrDefault(p => p.ActivityId == ActivityId);
            dbContext.Activities.Remove(deletedActivity);
            dbContext.SaveChanges();
            return RedirectToAction ("Success");
        }
        




    }
}
