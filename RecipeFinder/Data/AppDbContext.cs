using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RecipeFinder.Models;
using RecipeFinder.Models.FourmModel;

namespace RecipeFinder.Data;

public class AppDbContext : IdentityDbContext<Customer, IdentityRole<int>, int>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<DietaryInfo> DietaryInfos { get; set; }
    public DbSet<FavoriteRecipe> FavoriteRecipes { get; set; }
    public DbSet<RecipeRating> RecipeRatings { get; set; }
    public DbSet<RecipeNote> RecipeNotes { get; set; }
    public DbSet<RecipeInstructions> RecipeInstructions { get; set; }

    // ── Forum ──────────────────────────────────────────────────
    public DbSet<ForumPost> ForumPosts { get; set; }
    public DbSet<ForumComment> ForumComments { get; set; }
    public DbSet<ForumPostVote> ForumPostVotes { get; set; }
    public DbSet<ForumCommentVote> ForumCommentVotes { get; set; }
    public DbSet<ForumFlair> ForumFlairs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // ── Flair seed data ────────────────────────────────────
        modelBuilder.Entity<ForumFlair>().HasData(
            new ForumFlair { Id = 1, Name = "🍳 General Cooking",    ColorHex = "#e85d26" },
            new ForumFlair { Id = 2, Name = "📖 Recipe Share",        ColorHex = "#2d7d46" },
            new ForumFlair { Id = 3, Name = "🙋 Recipe Requests",     ColorHex = "#2563eb" },
            new ForumFlair { Id = 4, Name = "🔪 Tips & Techniques",   ColorHex = "#b5550a" },
            new ForumFlair { Id = 5, Name = "🌱 Vegetarian & Vegan",  ColorHex = "#16a34a" },
            new ForumFlair { Id = 6, Name = "🌍 World Cuisines",      ColorHex = "#7c3aed" },
            new ForumFlair { Id = 7, Name = "📸 Food Photography",    ColorHex = "#db2777" },
            new ForumFlair { Id = 8, Name = "💬 Site Feedback",       ColorHex = "#0891b2" }
        );

        // ── Existing constraints ───────────────────────────────
        modelBuilder.Entity<FavoriteRecipe>()
            .HasIndex(f => new { f.CustomerId, f.RecipeId })
            .IsUnique();

        modelBuilder.Entity<RecipeRating>()
            .HasIndex(r => new { r.CustomerId, r.RecipeId })
            .IsUnique();

        // ── One vote per user per post/comment ─────────────────
        modelBuilder.Entity<ForumPostVote>()
            .HasIndex(v => new { v.CustomerId, v.ForumPostId })
            .IsUnique();

        modelBuilder.Entity<ForumCommentVote>()
            .HasIndex(v => new { v.CustomerId, v.ForumCommentId })
            .IsUnique();

        // ── Prevent EF cascade delete cycles ──────────────────
        // (SQL Server will complain about multiple cascade paths
        //  from Customer → ForumPost → ForumComment etc.)
        modelBuilder.Entity<ForumPost>()
            .HasOne(p => p.Customer)
            .WithMany()
            .HasForeignKey(p => p.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ForumComment>()
            .HasOne(c => c.Customer)
            .WithMany()
            .HasForeignKey(c => c.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ForumPostVote>()
            .HasOne(v => v.Customer)
            .WithMany()
            .HasForeignKey(v => v.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ForumCommentVote>()
            .HasOne(v => v.Customer)
            .WithMany()
            .HasForeignKey(v => v.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        // ── Self-referencing replies (no cascade needed) ───────
        modelBuilder.Entity<ForumComment>()
            .HasOne(c => c.ParentComment)
            .WithMany(c => c.Replies)
            .HasForeignKey(c => c.ParentCommentId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}