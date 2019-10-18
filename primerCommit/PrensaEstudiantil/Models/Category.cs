using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PrensaEstudiantil.Models
{
    public class Category
    {
        [Key]
        [Display(Name = "Category")]
        public int CategoryID { get; set; }

        [Display(Name = "Categoría")]
        public string Description { get; set; }

        public virtual ICollection<Publication> Publications { get; set; }
    }
}