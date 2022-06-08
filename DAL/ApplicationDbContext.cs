using System;
using System.Linq;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class ApplicationDbContext : DbContext
    {
        private static string ConnectionString = @"
                    Server=barrel.itcollege.ee;
                    User Id=student;
                    Password=Student.Pass.1;
                    Database=student_heveet_BS;
                    MultipleActiveResultSets=true;
                    ";
        
        public DbSet<Coordinate> Coordinates { get; set; } = default!;
        public DbSet<Game> Games { get; set; } = default!;
        public DbSet<GameBoardState> GameBoardStates { get; set; } = default!;
        public DbSet<Player> Players { get; set; } = default!;
        public DbSet<Ship> Ships { get; set; } = default!;
        
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionString);
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder){
            
            base.OnModelCreating(modelBuilder);
            foreach (var relationship in modelBuilder.Model
                .GetEntityTypes()
                .Where(e => !e.IsOwned())
                .SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}