using Microsoft.EntityFrameworkCore;
using CICertSOAR.Data;
using CICertSOAR.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Database Configuration (Supports both PostgreSQL & InMemory Demo Mode)
var useInMemory = builder.Configuration.GetValue<bool>("ConnectionStrings:UseInMemory", true);
var postgresConnStr = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (useInMemory || string.IsNullOrEmpty(postgresConnStr))
    {
        options.UseInMemoryDatabase("CICertDb");
    }
    else
    {
        options.UseNpgsql(postgresConnStr);
    }
});

// Dependency Injection Services
builder.Services.AddScoped<IIncidentService, IncidentService>();
builder.Services.AddScoped<IAssetService, AssetService>();
builder.Services.AddScoped<IReportingService, ReportingService>();

// Automated Weekly Report Scheduler Service (Runs every Monday at 07:00 AM)
builder.Services.AddHostedService<WeeklyReportSchedulerService>();

var app = builder.Build();

// Initialize DB with realistic Seed Data for CI-CERT / Côte d'Ivoire Ministries
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DbInitializer.Initialize(context);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
