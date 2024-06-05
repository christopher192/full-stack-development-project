using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WD_ERECORD_CORE.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Newtonsoft.Json.Converters;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.FileProviders;
using WD_ERECORD_CORE.Service;
using Microsoft.AspNetCore.Mvc;
using WD_ERECORD_CORE.Function;

var builder = WebApplication.CreateBuilder(args);
var connectionString = String.Format("Data Source={0};User Id={1};Password={2};", builder.Configuration.GetConnectionString("Data Source"), builder.Configuration.GetConnectionString("User Id"), EncryptDecrypt.Decrypt(builder.Configuration.GetConnectionString("Password")));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseOracle(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>().AddRoles<IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
    // SignIn setting
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;

    // Password setting
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 1;

    // User setting
    options.User.RequireUniqueEmail = false;
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = ".AspNetCore.Identity.Application";
    options.ExpireTimeSpan = TimeSpan.FromDays(36500);
});

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"bin\debug\configuration"))
    .ProtectKeysWithDpapi()
    .SetDefaultKeyLifetime(TimeSpan.FromDays(10));

var mvcBuilder = builder.Services.AddRazorPages();

/*mvcBuilder.AddRazorPagesOptions(o =>
{
    o.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute());
});*/

if (builder.Environment.IsDevelopment())
{
    mvcBuilder.AddRazorRuntimeCompilation();
}

builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.Converters.Add(new StringEnumConverter());
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    });

// email service
if (builder.Environment.IsProduction())
{
    builder.Services.AddHostedService<MasterFormEmailService>();
    builder.Services.AddHostedService<FormEmailService>();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles(
    new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(builder.Environment.WebRootPath)
    }
);

app.UseStaticFiles(
    new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(
               Path.Combine(builder.Environment.ContentRootPath, "GuidelineFiles")),
        RequestPath = "/GuidelineFiles"
    }
);

app.UseStaticFiles(
    new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(
               Path.Combine(builder.Environment.ContentRootPath, "UserManualFiles")),
        RequestPath = "/UserManualFiles"
    }
);

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
