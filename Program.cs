using System;
using FitnessTracker.Data;
using FitnessTracker.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

var connStr = builder.Configuration
    .GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("No DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(opts =>
    opts.UseSqlServer(connStr));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services
    .AddDefaultIdentity<IdentityUser>(opts =>
    {
        opts.SignIn.RequireConfirmedAccount = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IWgerService, WgerService>();

builder.Services.AddHttpClient<ICalorieService, ApiNinjasCalorieService>(c =>
{
    c.BaseAddress = new Uri("https://api.api-ninjas.com/");
    c.DefaultRequestHeaders.Add("X-Api-Key", builder.Configuration["ApiNinjas:Key"]);
});
builder.Services.AddHttpClient<IOpenFoodFactsService, OpenFoodFactsService>(c =>
{
    c.BaseAddress = new Uri("https://world.openfoodfacts.org/");
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Fitness Tracker API",
        Version = "v1"
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Fitness Tracker API");
        c.RoutePrefix = string.Empty;
    });
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
