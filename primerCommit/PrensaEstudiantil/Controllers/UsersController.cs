using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PrensaEstudiantil.Models;
using PrensaEstudiantil.ModelView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PrensaEstudiantil.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Users
        public ActionResult Index()
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));

            var users = userManager.Users.ToList();
            var roles = roleManager.Roles.ToList();

            // Array to put users and roles for each user
            var usersView = new List<UserView>();

            // Create a new UserView object for every user of User Model
            foreach (var user in users)
            {
                // Array to put the roles of user
                var rolesUser = new List<RoleView>();

                // Verify roles for user
                foreach (var role in roles)
                {
                    // IF user is in role, add role to rolesUser
                    if (userManager.IsInRole(user.Id, role.Name))
                    {
                        // Construct the RoleView object to add to rolesUser
                        var roleUser = new RoleView {
                            Name = role.Name,
                            RoleID = role.Id
                        };

                        // Add role to rolesUser
                        rolesUser.Add(roleUser);
                    }
                }

                var userView = new UserView
                {
                    Email = user.Email,
                    Name = user.UserName,
                    UserID = user.Id,
                    Roles = rolesUser
                };

                // Add userView in usersView array
                usersView.Add(userView);
            }

            return View(usersView);
        }

        // Get the Roles by UserID
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult Roles(string userID)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));

            // Create an object for user and list for roles by user
            var user = userManager.Users.ToList().Find(u => u.Id == userID);
            var roles = roleManager.Roles.ToList();

            // Array to put the roles of user
            var rolesView = new List<RoleView>();

            // Consturc role objects and put it into rolesView array
            foreach (var item in user.Roles)
            {
                var role = roles.Find(r => r.Id == item.RoleId);

                var roleView = new RoleView
                {
                    RoleID = role.Id,
                    Name = role.Name
                };

                rolesView.Add(roleView);
            }

            // Create the userView object
            var userView = new UserView
            {
                Email = user.Email,
                Name = user.UserName,
                UserID = user.Id,
                Roles = rolesView
            };

            // ViewBag with length of Roles list
            ViewBag.RolesCount = rolesView.Count;

            return View(userView);
        }

        // GET: Users/AddRoleToUser
        // Add role to user
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult AddRoleToUser(string UserID, string Email, string Name)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));

            // Create a userVieww object
            UserView userView = new UserView
            {
                UserID = UserID,
                Email = Email,
                Name = Name
            };

            // Construc the ViewBag with dropdown list of Roles for user
            ViewBag.RoleID = new SelectList(roleManager.Roles.ToList(), "Id", "Name");

            return View(userView);
        }

        // POST: Users/AddRoleToUser
        // Add role to user
        [HttpPost]
        public ActionResult AddRoleToUser(string UserID)
        {
            var roleID = Request["RoleID"];

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));

            // Create a userVieww object
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var users = userManager.Users.ToList();
            var user = users.Find(u => u.Id == UserID);

            UserView userView = new UserView
            {
                UserID = UserID,
                Email = user.Email,
                Name = user.UserName
            };

            // Search current role
            var roles = roleManager.Roles.ToList();
            var role = roles.Find(r => r.Id == roleID);

            //Verify if user alreads has the role
            if (userManager.IsInRole(UserID, role.Name))
            {
                // ViewBag for error message
                ViewBag.Error = "Usuario ya pertenece a Rol seleccionado.";
                // Construc the ViewBag with dropdown list of Roles for user
                ViewBag.RoleID = new SelectList(roleManager.Roles.ToList(), "Id", "Name");

                return View(userView);
            }
            else
            {
                userManager.AddToRole(UserID, role.Name);
            }

            // Array to put the roles of user
            var rolesView = new List<RoleView>();

            // Consturc role objects and put it into rolesView array
            foreach (var item in user.Roles)
            {
                role = roles.Find(r => r.Id == item.RoleId);

                var roleView = new RoleView
                {
                    RoleID = role.Id,
                    Name = role.Name
                };

                rolesView.Add(roleView);
            }

            // Update the userView object
            userView = new UserView
            {
                Email = user.Email,
                Name = user.UserName,
                UserID = user.Id,
                Roles = rolesView
            };

            return RedirectToAction("Roles", userView);
        }

        // DELETE: Users/DeteleRoleToUser
        // Delete a role to user
        [Authorize(Roles = "SuperAdmin")]
        public ActionResult DeteleRoleToUser(string userID, string roleID)
        {
            // Verify userID and roleID are valids
            if (string.IsNullOrEmpty(userID) || string.IsNullOrEmpty(roleID))
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var role = roleManager.Roles.ToList().Find(r => r.Id == roleID);

            // Delete ser ufrom  role
            if (userManager.IsInRole(userID, role.Name))
            {
                userManager.RemoveFromRole(userID, role.Name);
            }

            // Prepare the view to return 
            var user = userManager.Users.ToList().Find(u => u.Id == userID);
            // Array to put the roles of user
            var rolesView = new List<RoleView>();
            var roles = roleManager.Roles.ToList();

            // Consturc role objects and put it into rolesView array
            foreach (var item in user.Roles)
            {
                role = roles.Find(r => r.Id == item.RoleId);

                var roleView = new RoleView
                {
                    RoleID = role.Id,
                    Name = role.Name
                };

                rolesView.Add(roleView);
            }

            // Update the userView object
            var userView = new UserView
            {
                Email = user.Email,
                Name = user.UserName,
                UserID = user.Id,
                Roles = rolesView
            };

            return RedirectToAction("Roles", userView);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}