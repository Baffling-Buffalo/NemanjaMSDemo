using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace API2.Models
{
    public partial class Api2Context : DbContext
    {
        public Api2Context()
        {
        }

        public Api2Context(DbContextOptions<Api2Context> options)
            : base(options)
        {
        }

        public virtual DbSet<Api2Data> Api2Data { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Data Source=DEVMASINA\\DEVBAZA;Initial Catalog=IdentityServerAsp;Persist Security Info=True;User ID=kestrel;Password=Algotech237!");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.0-rtm-35687");

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}