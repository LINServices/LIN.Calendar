namespace LIN.Contacts.Data;


public class Context : DbContext
{


    /// <summary>
    /// Tabla de perfiles.
    /// </summary>
    public DbSet<ProfileModel> Profiles { get; set; }


    /// <summary>
    /// Tabla de eventos.
    /// </summary>
    public DbSet<EventModel> Events { get; set; }



    /// <summary>
    /// Tabla de eventos.
    /// </summary>
    public DbSet<EventGuestModel> Guests { get; set; }



    /// <summary>
    /// Nuevo contexto a la base de datos
    /// </summary>
    public Context(DbContextOptions<Context> options) : base(options) { }



    /// <summary>
    /// Naming DB
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        // Indices y identidad
        modelBuilder.Entity<ProfileModel>()
           .HasIndex(e => e.AccountId)
           .IsUnique();


        modelBuilder.Entity<EventGuestModel>()
           .HasOne(t => t.Event)
           .WithMany(t => t.Guests)
           .HasForeignKey(t => t.EventId);


        modelBuilder.Entity<EventGuestModel>()
         .HasOne(t => t.Profile)
         .WithMany()
         .HasForeignKey(t => t.ProfileId);

        modelBuilder.Entity<EventGuestModel>()
            .HasKey(t => new { t.EventId, t.ProfileId });

    }


}