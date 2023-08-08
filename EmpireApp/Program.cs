using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;
using Microsoft.Net.Http.Headers;
using Empire.Utilities;
using EmpireApp.Services;
using EmpireApp.Services.IServices;
using Empire.DataAccess.Data;
using Empire.DataAccess.DbInitializer;
using Empire.DataAccess.Repository;
using Empire.DataAccess.Repository.IRepository;

var builder = WebApplication.CreateBuilder(args);  

// Add services to the container.
builder.Services.AddControllersWithViews();
var DBhost = Environment.GetEnvironmentVariable("DB_HOST");
var DBName = Environment.GetEnvironmentVariable("DB_NAME");
var DBpw = Environment.GetEnvironmentVariable("SA_PASSWORD");

var connectionString = $"Data Source={DBhost};Initial Catalog={DBName};User ID=sa;Password={DBpw}";
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<PayMobSettings>(builder.Configuration.GetSection("PayMob"));
builder.Services.AddOptions();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddHttpClient<IPayMobService, PayMobService>();
builder.Services.AddScoped<IPayMobService, PayMobService>();


builder.Services.AddScoped<IEmailSender, EmailSender>(); 
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
}); 
builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.SmallestSize;
});
builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

builder.Services.AddAuthentication()
    .AddGoogle(option =>
{
    option.ClientId = "1097535170814-k2nb9kplse0icvl97v05poqc0am96h55.apps.googleusercontent.com";
    option.ClientSecret = "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";
})
    .AddFacebook(options =>
{
    options.AppId = "930739641364678";
    options.AppSecret = "76a03b4648733a57bacbbed1d3fa4de2";
});

builder.Services.AddScoped<IDbInitializer, DbInitializer>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(100);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddIdentity<IdentityUser,IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});
builder.Services.AddRazorPages();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmailSender, EmailSender>();

builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        const int durationInSeconds = 365 * 60 * 60 * 24;
        ctx.Context.Response.Headers[HeaderNames.CacheControl] =
            "public,max-age=" + durationInSeconds;
    }
});
app.UseResponseCompression();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
seedDatabase();
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");
app.Run();


void seedDatabase()
{
    using (var scope = app.Services.CreateScope())
    {
        var DbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        DbInitializer.Initialize();
    }
}