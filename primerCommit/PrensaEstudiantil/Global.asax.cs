using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PrensaEstudiantil.Context;
using PrensaEstudiantil.Models;
using System;
using System.Data.Entity;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace PrensaEstudiantil
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // Initial config of BD
            // Verify changes in code first
            Database.SetInitializer(
                new MigrateDatabaseToLatestVersion<PrensaContext, Migrations.Configuration>());

            ApplicationDbContext db = new ApplicationDbContext();

            CreateRoles(db);
            CreateSuperUser(db);
            AddPersmissionsToAdmins(db);

            db.Dispose();

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        /// <summary>
        ///  Assign permissions to superAdmins
        ///  1. sevann.radhak@gmail.com
        ///  2. prensasetudiantil@hotmail.com
        /// </summary>
        /// <param name="db"></param>
        private void AddPersmissionsToAdmins(ApplicationDbContext db)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

            var user1 = userManager.FindByName("sevann.radhak@gmail.com");
            var user2 = userManager.FindByName("prensaestudiantil@hotmail.com");

            // Add roles to users
            if (!userManager.IsInRole(user1.Id, "SuperAdmin"))
            {
                userManager.AddToRole(user1.Id, "SuperAdmin");
            }

            if (!userManager.IsInRole(user2.Id, "SuperAdmin"))
            {
                userManager.AddToRole(user2.Id, "SuperAdmin");
            }
        }

        /// <summary>
        ///  Create initial users for SuperAdmin role. Both password: Pr3n$a*20!ñ
        ///  1. sevann.radhak@gmail.com
        ///  2. prensasetudiantil@hotmail.com
        /// </summary>
        /// <param name="db"></param>
        private void CreateSuperUser(ApplicationDbContext db)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

            // Search if superUsers alrady exist
            var user1 = userManager.FindByName("sevann.radhak@gmail.com");
            var user2 = userManager.FindByName("prensaestudiantil@hotmail.com");

            if (user1 == null)
            {
                user1 = new ApplicationUser
                {
                    UserName = "sevann.radhak@gmail.com",
                    Email = "sevann.radhak@gmail.com"
                };

                // Create the user (user, password)
                userManager.Create(user1, "Pr3n$a*20!ñ");
            }

            if(user2 == null)
            {
                user2 = new ApplicationUser
                {
                    UserName = "prensaestudiantil@hotmail.com",
                    Email = "prensaestudiantil@hotmail.com"
                };

                // Create the user (user, password)
                userManager.Create(user2, "Pr3n$a*20!ñ");
            }
        }

        /// <summary>
        ///  Create initial roles for system: 
        ///  1. SuperAdmin
        ///  2. Admin
        ///  3. User
        /// </summary>
        /// <param name="db"></param>
        private void CreateRoles(ApplicationDbContext db)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));

            if (!roleManager.RoleExists("SuperAdmin"))
            {
                roleManager.Create(new IdentityRole("SuperAdmin"));
            }

            if (!roleManager.RoleExists("Admin"))
            {
                roleManager.Create(new IdentityRole("Admin"));
            }

            if (!roleManager.RoleExists("User"))
            {
                roleManager.Create(new IdentityRole("User"));
            }
        }
    }
}
