using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

public class DatabaseContext : DbContext
{
    public DbSet<Api_Key> Api_Keys { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<Inventory> Inventories { get; set; }
    public DbSet<Item_Group> Item_Groups { get; set; }
    public DbSet<Item_Line> Item_Lines { get; set; }
    public DbSet<Item_Type> Item_Types { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Shipment> Shipments { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Transfer> Transfers { get; set; }
    public DbSet<Warehouse> Warehouses { get; set; }

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Item>()
            .HasKey(i => i.UId);

        var contactsConverter = new ValueConverter<Dictionary<string, string>, string>(
            v => JsonConvert.SerializeObject(v),
            v => JsonConvert.DeserializeObject<Dictionary<string, string>>(v)
        );

        modelBuilder.Entity<Warehouse>()
            .Property(a => a.Contact)
            .HasConversion(contactsConverter);
        
        var itemsConverter = new ValueConverter<List<PropertyItem>, string>(
            v => JsonConvert.SerializeObject(v),
            v => JsonConvert.DeserializeObject<List<PropertyItem>>(v)
        );

        modelBuilder.Entity<Shipment>()
            .Property(a => a.Items)
            .HasConversion(itemsConverter);
        
        modelBuilder.Entity<Order>()
            .Property(a => a.Items)
            .HasConversion(itemsConverter);
        
        modelBuilder.Entity<Transfer>()
            .Property(a => a.Items)
            .HasConversion(itemsConverter);

        modelBuilder.Entity<Api_Key>()
            .HasIndex(k => k.Id).IsUnique();

        // GENERAL API_KEY THAT IS USED FOR ALL KINDS OF TESTS
        modelBuilder.Entity<Api_Key>()
            .HasData(new Api_Key 
            { Id = 1, ApiKey = ApiKeyService.HashApiKey("f3f0efb1-917d-4457-a279-90280da97439"), Warehouse_Id = 100, App = "dashboard" });
        
        // ONLY ENABLE THESE FOUR API_KEYS WHEN YOU ARE MIGRATING AND UPDATING Cargohub_test_database.db
        modelBuilder.Entity<Api_Key>()
            .HasData(
                new Api_Key { Id = 2, ApiKey = ApiKeyService.HashApiKey("104e99b1-64c4-4026-bc71-6cd6cb854de6"), Warehouse_Id = 1, App = "dashboard" },
                new Api_Key { Id = 3, ApiKey = ApiKeyService.HashApiKey("c28469f3-0d63-4567-906f-34d927cbcb1c"), Warehouse_Id = 1, App = "scanner" },
                new Api_Key { Id = 4, ApiKey = ApiKeyService.HashApiKey("dce8f448-23f1-4296-89a5-7b56b2fa9ea6"), Warehouse_Id = 2, App = "dashboard" },
                new Api_Key { Id = 5, ApiKey = ApiKeyService.HashApiKey("98c04070-9b8d-4c63-a052-3e2457740975"), Warehouse_Id = 2, App = "scanner" }
            );
    }
}