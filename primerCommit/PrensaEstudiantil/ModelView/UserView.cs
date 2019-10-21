using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PrensaEstudiantil.ModelView
{
    public class UserView
    {
        public string UserID { get; set; }

        [Display(Name = "Nombre")]
        public string Name { get; set; }

        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public RoleView Role { get; set; }

        [Display(Name = "Roles")]
        public List<RoleView> Roles { get; set; }
    }
}