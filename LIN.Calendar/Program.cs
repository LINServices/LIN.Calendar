// Servicio de errores.
global using LIN.Access.Logger;
using LIN.Calendar.Data;
using Http.Extensions;

// Constructor.
var builder = WebApplication.CreateBuilder(args);

// App en logger.
LIN.Access.Logger.Logger.AppName = "LIN.CALENDAR";

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
    _ = Logger.Log(ex, 3);
}


app.UseLINHttp();

app.UseHttpsRedirection();

app.UseAuthorization();

Conexión.SetStringConnection(sqlConnection);
Jwt.Open();
App.Open();

LIN.Access.Auth.Build.Init();

app.MapControllers();

app.Run();