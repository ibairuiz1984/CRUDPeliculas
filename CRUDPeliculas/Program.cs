using CRUDPeliculas;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var politicaUsuariosAutenticados = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();

builder.Services.AddControllersWithViews(opciones =>
{
    opciones.Filters.Add(new AuthorizeFilter(politicaUsuariosAutenticados));
});

// Configurar el contexto de base de datos utilizando SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(opciones => opciones.UseSqlServer("name=DefaultConnection"));

// Agregar servicios de autenticaci�n
builder.Services.AddAuthentication();

// Configurar la identidad de usuario y roles
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    // Configuraci�n espec�fica para requerir cuenta confirmada al iniciar sesi�n
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Postconfigurar las opciones de autenticaci�n con cookies
builder.Services.PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme, opciones =>
{
    // Configurar las rutas de inicio de sesi�n y acceso denegado
    opciones.LoginPath = "/usuarios/login";
    opciones.AccessDeniedPath = "/usuarios/login";
});

var app = builder.Build();

// Configurar el pipeline de solicitudes HTTP
if (!app.Environment.IsDevelopment())
{
    // Manejar las excepciones y redirigirlas a la p�gina de error
    app.UseExceptionHandler("/Home/Error");

    // Establecer la directiva de seguridad HTTP Strict Transport Security (HSTS)
    // Se puede ajustar el valor predeterminado de 30 d�as para escenarios de producci�n
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Habilitar la autenticaci�n
app.UseAuthentication();

// Habilitar la autorizaci�n
app.UseAuthorization();

// Configurar la ruta predeterminada del controlador
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Ejecutar la aplicaci�n
app.Run();
