using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PrensaEstudiantil.Models
{
    public class YoutubeVideo
    {
        [Key]
        public int YoutubeVideoID { get; set; }

        [Display(Name = "URL")]
        [Required(ErrorMessage = "{0} campo obligatorio")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "Este campo debe tener 11 caracteres")]
        public string URL { get; set; }

        [Display(Name="Titulo")]
        [Required(ErrorMessage = "{0} campo obligatorio")]
        public string Name { get; set; }
    }
}