using AvaliaFatec.Data;
using AvaliaFatec.Models;
using AvaliaFatec.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Rotativa.AspNetCore;
using AvaliaFatec.Settings;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AvaliaFatecContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AvaliaFatecContext") ?? throw new InvalidOperationException("Connection string 'AvaliaFatecContext' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<ContextMongodb>();

ContextMongodb.ConnectionString = builder.Configuration.GetSection("MongoConnection:ConnectionString").Value;
ContextMongodb.Database = builder.Configuration.GetSection("MongoConnection:Database").Value;
ContextMongodb.IsSSL = Convert.ToBoolean(builder.Configuration.GetSection("MongoConnection:IsSSL").Value);

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>(
    ContextMongodb.ConnectionString, ContextMongodb.Database).AddDefaultTokenProviders();
    

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<EmailService>();

var app = builder.Build();


RotativaConfiguration.Setup(app.Environment.WebRootPath, "Rotativa");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
