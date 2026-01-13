using Microsoft.EntityFrameworkCore;
using RecipeFinder.Models;

namespace RecipeFinder.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<DietaryInfo> DietaryInfos { get; set; }
}