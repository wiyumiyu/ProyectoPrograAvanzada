using ProyectoPrograAvanzada.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using ProyectoPrograAvanzada.Services;
using ProyectoPrograAvanzada.Models;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ========================================
// SERVICIOS MVC
// ========================================
builder.Services.AddControllersWithViews();

// ========================================
// EF CORE + CONEXIÓN A BD
// ========================================
builder.Services.AddDbContext<DbAlquilerVehiculosContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ========================================
// AUTENTICACIÓN CON COOKIES
// ========================================
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });

// ========================================
// AUTORIZACIÓN
// ========================================
builder.Services.AddAuthorization();

// ========================================
// IDENTITY: Password Hasher
// ========================================
builder.Services.AddScoped<IPasswordHasher<TEmpleado>, PasswordHasher<TEmpleado>>();

// ========================================
// SERVICIO DE AUTENTICACIÓN PERSONALIZADO
// ========================================
builder.Services.AddScoped<AuthService>();

// ========================================
// ACCESO A HttpContext (usuario logueado)
// ========================================
builder.Services.AddHttpContextAccessor();

// ========================================
// QuestPDF: Registro de licencia
// ========================================
QuestPDF.Settings.License = LicenseType.Community;

// ========================================
// REGISTRAR SERVICIO PDF COMO SINGLETON
// ========================================
builder.Services.AddSingleton<PdfGeneratorService>();

// ========================================
// CONSTRUIR APP
// ========================================
var app = builder.Build();

// ========================================
// MIDDLEWARES
// ========================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();  // SIEMPRE ANTES DE Authorization
app.UseAuthorization();

// ========================================
// RUTAS MVC
// ========================================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
