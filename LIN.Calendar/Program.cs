// Servicio de errores.
using Http.Extensions;
using LIN.Access.Auth;
using LIN.Calendar.Data;

// Constructor.
var builder = WebApplication.CreateBuilder(args);

// Obtiene el string de conexión SQL.
#if DEBUG
var sqlConnection = builder.Configuration["ConnectionStrings:local"] ?? string.Empty;
#elif RELEASE
var sqlConnection = builder.Configuration["ConnectionStrings:somee"] ?? string.Empty;
#endif
// Servicio de BD
builder.Services.AddDbContext<Context>(options =>
{
    options.UseSqlServer(sqlConnection);
});

// Add services to the container.
builder.Services.AddLINHttp();
builder.Services.AddAuthenticationService();


var app = builder.Build();


try
{
    // Si la base de datos no existe.
    using var scope = app.Services.CreateScope();
    var dataContext = scope.ServiceProvider.GetRequiredService<Context>();
    var res = dataContext.Database.EnsureCreated();
}
catch (Exception ex)
{
}


app.UseLINHttp();

app.UseHttpsRedirection();

app.UseAuthorization();

Conexión.SetStringConnection(sqlConnection);
Jwt.Open();
App.Open();

app.MapControllers();

app.Run();