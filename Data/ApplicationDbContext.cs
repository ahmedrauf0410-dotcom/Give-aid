using Microsoft.EntityFrameworkCore;
using GiveAID.Models;

namespace GiveAID.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Admin> Admins => Set<Admin>();
        public DbSet<DonationCause> DonationCauses => Set<DonationCause>();
        public DbSet<Donation> Donations => Set<Donation>();
        public DbSet<Programme> Programmes => Set<Programme>();
        public DbSet<ProgrammeInterest> ProgrammeInterests => Set<ProgrammeInterest>();
        public DbSet<Partner> Partners => Set<Partner>();
        public DbSet<GalleryItem> GalleryItems => Set<GalleryItem>();
        public DbSet<AboutSection> AboutSections => Set<AboutSection>();
        public DbSet<HelpItem> HelpItems => Set<HelpItem>();
        public DbSet<UserQuery> UserQueries => Set<UserQuery>();
        public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();
        public DbSet<Invitation> Invitations => Set<Invitation>();
        public DbSet<SiteSetting> SiteSettings => Set<SiteSetting>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique constraints
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Admin>()
                .HasIndex(a => a.Username)
                .IsUnique();

            modelBuilder.Entity<Admin>()
                .HasIndex(a => a.Email)
                .IsUnique();

            modelBuilder.Entity<AboutSection>()
                .HasIndex(s => s.SectionKey)
                .IsUnique();

            // Decimal Precision
            modelBuilder.Entity<DonationCause>()
                .Property(c => c.TargetAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<DonationCause>()
                .Property(c => c.RaisedAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Donation>()
                .Property(d => d.Amount)
                .HasPrecision(18, 2);

            // Relationships
            modelBuilder.Entity<Donation>()
                .HasOne(d => d.User)
                .WithMany(u => u.Donations)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Donation>()
                .HasOne(d => d.DonationCause)
                .WithMany(c => c.Donations)
                .HasForeignKey(d => d.DonationCauseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProgrammeInterest>()
                .HasOne(p => p.Programme)
                .WithMany(pr => pr.ProgrammeInterests)
                .HasForeignKey(p => p.ProgrammeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProgrammeInterest>()
                .HasOne(p => p.User)
                .WithMany(u => u.ProgrammeInterests)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<UserQuery>()
                .HasOne(q => q.User)
                .WithMany(u => u.Queries)
                .HasForeignKey(q => q.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Invitation>()
                .HasOne(i => i.SenderUser)
                .WithMany(u => u.Invitations)
                .HasForeignKey(i => i.SenderUserId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
