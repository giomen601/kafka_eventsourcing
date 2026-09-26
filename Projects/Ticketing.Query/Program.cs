using Ticketing.Query.Application;
using Ticketing.Query.Application.Extensions;
using Ticketing.Query.Infraestructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.RegisterAppicationServices();
builder.Services.RegisterInfraestructureServices(builder.Configuration);

var app = builder.Build();

app.MapControllers();

await app.ApplyMigration();

app.Run();
