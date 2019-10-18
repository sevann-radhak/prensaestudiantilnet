using PrensaEstudiantil.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PrensaEstudiantil.Context
{
    public class PrensaContext : DbContext
    {
        public System.Data.Entity.DbSet<PrensaEstudiantil.Models.Publication> Publications { get; set; }

        public System.Data.Entity.DbSet<PrensaEstudiantil.Models.Category> Categories { get; set; }

        public System.Data.Entity.DbSet<PrensaEstudiantil.Models.YoutubeVideo> YoutubeVideos { get; set; }

        public System.Data.Entity.DbSet<PrensaEstudiantil.Models.PublicationImage> PublicationImages { get; set; }
    }
}