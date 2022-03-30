using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SoMeSimulator.Data.Models;

namespace SoMeSimulator.Data
{
    public class SoMeContext : DbContext
    {
        public SoMeContext(DbContextOptions<SoMeContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usr>()
                .HasOne(u => u.ActiveSessionGroup)
                .WithOne(s => s.Usr)
                .HasForeignKey<SessionGroup>(s => s.UsrId);
            
            modelBuilder.Entity<PhaseComment>()
                .HasKey(x => new { x.PhaseId, x.CommentId });

            modelBuilder.Entity<PhaseComment>()
                .HasOne(pp => pp.Phase)
                .WithMany(p => p.CommentLink)
                .HasForeignKey(pp => pp.PhaseId);

            modelBuilder.Entity<PhaseComment>()
                .HasOne(pp => pp.Comment)
                .WithMany(p => p.PhaseLink)
                .HasForeignKey(pp => pp.CommentId);

            modelBuilder.Entity<PhasePost>()
                .HasKey(x => new { x.PhaseId, x.PostId });

            modelBuilder.Entity<PhasePost>()
                .HasOne(pp => pp.Phase)
                .WithMany(p => p.PostLink)
                .HasForeignKey(pp => pp.PhaseId);

            modelBuilder.Entity<PhasePost>()
                .HasOne(pp => pp.Post)
                .WithMany(p => p.PhaseLink)
                .HasForeignKey(pp => pp.PostId);

            
            modelBuilder.Entity<SessionLog>()
                .HasOne( s => s.Parent)
                .WithMany(s => s.Children)
                .HasForeignKey(s => s.ParentSessionLogId);

            modelBuilder.Entity<Session>()
                .HasMany(s => s.SessionLogs)
                .WithOne(s => s.Session)
                .HasForeignKey(s => s.SessionId);

        }

        public DbSet<Session> Sessions { get; set; }
        public DbSet<ScenarioEvent> ScenarioEvents { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Scenario> Scenarios { get; set; }
        public DbSet<SessionLog> SessionLogs { get; set; }
        public DbSet<Phase> Phases { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<PhaseComment> PhaseComments { get; set; }
        public DbSet<PhasePost> PhasePosts { get; set; }
        public DbSet<SessionGroup> SessionGroups { get; set; }
        public DbSet<Usr> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
    }
}