namespace CardsAgainstMySanity.Infrastructure.Data.EntityFramework;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
{
    public DataContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DataContext>();

        optionsBuilder.UseNpgsql("Host=localhost;Database=cards_db;Username=docker;Password=docker;port=5001");

        return new DataContext(optionsBuilder.Options);
    }
}
