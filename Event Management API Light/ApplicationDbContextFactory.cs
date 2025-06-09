// ApplicationDbContextFactory.cs

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using System;
// WICHTIG: Stelle sicher, dass der Namespace hier genau zu deinem DbContext passt!
// Gemäß deiner Ausgabe ist es Event_Management_API_Light.Data
using Event_Management_API_Light.Data; // <--- Dies muss exakt der Namespace deines DbContext sein!
using Pomelo.EntityFrameworkCore.MySql.Infrastructure; // Für UseMySql

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // 1. Konfiguration laden (appsettings.json finden)
        // SetBasePath muss auf das Verzeichnis zeigen, in dem appsettings.json ist
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        // 2. Connection String abrufen
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // 3. DbContextOptions konfigurieren (wie in Program.cs, aber manuell hier)
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseMySql(connectionString,
            // WICHTIG: PASSE DEINE MYSQL-SERVER-VERSION AN!
            // Du kannst auch ServerVersion.AutoDetect(connectionString) verwenden,
            // aber eine explizite Version ist oft sicherer, wenn du sie kennst.
            new MySqlServerVersion(new Version(8, 0, 21)), // <-- HIER ANPASSEN (Beispiel: MySQL 8.0.21)
            mysqlOptions => mysqlOptions.EnableRetryOnFailure()
        );

        // 4. Eine Instanz deines DbContext zurückgeben
        return new ApplicationDbContext(optionsBuilder.Options);
    }
}