using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RecipeFinder.Models;

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
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Prevent duplicate favorites
        modelBuilder.Entity<FavoriteRecipe>()
            .HasIndex(f => new { f.CustomerId, f.RecipeId })
            .IsUnique();
            
        // One rating per customer per recipe
        modelBuilder.Entity<RecipeRating>()
            .HasIndex(r => new { r.CustomerId, r.RecipeId })
            .IsUnique();
    }
}