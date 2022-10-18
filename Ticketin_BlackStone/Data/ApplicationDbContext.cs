using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Ticketin_BlackStone.Models;

namespace Ticketin_BlackStone.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }



        //Add DBContext for TicketingSystem DB
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<File> Files { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<Ticket> Tickets { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            //using Fluent API to  create one to many relationship between tables
            modelBuilder.Entity<Ticket>()
                   .HasMany(b => b.Files)
                   .WithOne()
                   .HasForeignKey(p => p.TicketId)
                   .HasConstraintName("FK_Files_Tickets_TicketId");

            modelBuilder.Entity<Ticket>()
                .Navigation(b => b.Files)
                .UsePropertyAccessMode(PropertyAccessMode.Property);

            modelBuilder.Entity<File>()
                .HasOne(p => p.Ticket)
                .WithMany(b => b.Files)
                .HasForeignKey(p => p.TicketId)
                .HasConstraintName("FK_Files_Tickets_TicketId"); ;
        }

    }
}
