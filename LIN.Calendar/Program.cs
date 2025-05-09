using Http.Extensions;
using LIN.Access.Auth;
using LIN.Calendar.Persistence.Extensions;

// Constructor.
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddLINHttp();
builder.Services.AddAuthenticationService(app: builder.Configuration["LIN:AppKey"]);
builder.Services.AddPersistence(builder.Configuration);

// Configurar logs.
builder.Host.UseLoggingService(builder.Configuration);

builder.Services.AddScoped<Iam>();

// Construir app.
var app = builder.Build();

app.UseLINHttp();
app.UseHttpsRedirection();
app.UseAuthorization();
app.UsePersistence();

Jwt.Open(builder.Configuration);

app.MapControllers();

app.Run();