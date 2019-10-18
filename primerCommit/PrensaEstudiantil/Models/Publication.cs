using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace PrensaEstudiantil.Models
{
    public class Publication
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "Título")]
        [Required(ErrorMessage = "{0} campo obligatorio")]
        [StringLength(150, ErrorMessage = "El campo {0} debe tener entre {2} y {1} caracteres", MinimumLength = 10)]
        public string Title { get; set; }
        
        [Display(Name = "Encabezado")]
        [Required(ErrorMessage = "{0} campo obligatorio")]
        public string Header { get; set; }

        [Display(Name = "Cuerpo de la publicación")]
        public string Body { get; set; }

        [Display(Name = "Pie de nota")]
        [StringLength(250, ErrorMessage = "El campo {0} debe tener entre {2} y {1} caracteres", MinimumLength = 10)]
        public string Footer { get; set; }

        [Display(Name = "Fecha Publicación")]
        public DateTime? Date { get; set; }

        public DateTime? LastUpdate { get; set; }

        [Display(Name = "Imagen principal")]
        public string ImagePath { get; set; }

        //public HttpPostedFileBase ImageFile{ get; set; }

        [Display(Name = "Descripción Imagen")]
        [StringLength(90, ErrorMessage = "El campo {0} debe tener entre {2} y {1} caracteres", MinimumLength = 10)]
        public string ImageDescription { get; set; }

        // Aplica solo para publicacoines de tipo Opinion
        [Display(Name = "Autor")]
        [StringLength(60, ErrorMessage = "El campo {0} debe tener entre {2} y {1} caracteres", MinimumLength = 3)]
        public string Author { get; set; }

        //[Display(Name = "Estado")]
        //public int Status { get; set; }

        [Display(Name = "Categoría")]
        [Required(ErrorMessage = "{0} campo obligatorio")]
        public int CategoryID { get; set; }

        // Foreing key
        [Display(Name = "Categoría")]
        public virtual Category Category { get; set; }

        public virtual ICollection<PublicationImage> PublicationImages { get; set; }
}
}