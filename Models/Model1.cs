using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace QuanLySinhVien_EntityFrameWork.Models
{
    public partial class Model1 : DbContext
    {
        public Model1()
            : base("name=Models")
        {
        }

        public virtual DbSet<Faculty> Faculties { get; set; }
        public virtual DbSet<SinhVien> SinhViens { get; set; } 

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
