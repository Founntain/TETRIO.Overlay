using System.Globalization;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using TetraLeague.Overlay.Network.Api.Discord;
using TetraLeague.Overlay.Network.Api.Tetrio;
using Tetrio.Overlay.Database;

Console.WriteLine("Starting Tetrio.Overlay Backend...");

var builder = WebApplication.CreateBuilder(args);

// Services
Console.WriteLine("Adding services...");
builder.Services.AddSwaggerGen();
builder.Services.AddControllers().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
builder.Services.AddScoped<TetrioApi>();
builder.Services.AddScoped<DiscordApi>();
builder.Services.AddScoped<EncryptionService>();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<TetrioContext>();

// CORS config
Console.WriteLine("Adding CORS Rules...");
builder.Services.AddCors(options =>
{
#if DEBUG
    Console.WriteLine("Adding CORS Policy: AllowDev");

    options.AddPolicy("AllowDev", b =>
    {
        b.WithOrigins("http://localhost:8080")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
#else
    Console.WriteLine("Adding CORS Policy: AllowFounntainDev");

    options.AddPolicy("AllowFounntainDev", policy =>
    {
        policy.SetIsOriginAllowed(origin =>
                origin.EndsWith(".founntain.dev") || origin == "https://founntain.dev")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
#endif
});

Console.WriteLine("Building app...");
var app = builder.Build();

// Culture
Console.WriteLine("Setting culture...");
var cultureInfo = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

// Swagger for dev
if (app.Environment.IsDevelopment())
{
    Console.WriteLine("Adding Swagger...");
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TetrioContext>();
    await db.Database.MigrateAsync();
}

#if DEBUG
Console.WriteLine("Applying CORS Policy: AllowDev");
app.UseCors("AllowDev");
#else
Console.WriteLine("Applying CORS Policy: AllowFounntainDev");
app.UseCors("AllowFounntainDev");
#endif

Console.WriteLine("Adding middleware...");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapControllers();
app.Run();