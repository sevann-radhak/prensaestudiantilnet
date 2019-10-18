using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PrensaEstudiantil.Models
{
    public class PublicationImage
    {
        [Key]
        public int PublicationImagenID { get; set; }

        [Display(Name = "Ruta de la Imagen")]
        public string ImagePath { get; set; }

        [Display(Name = "Descripción")]
        public string Description { get; set; }

        public int ID { get; set; }

        public virtual Publication Publication { get; set; }
    }
}