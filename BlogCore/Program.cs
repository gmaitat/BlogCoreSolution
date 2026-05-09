using BlogCoreSolution.AccesoDatos.Data;
using BlogCoreSolution.AccesoDatos.Data.Repository;
using BlogCoreSolution.AccesoDatos.Data.Repository.IRepository;
using BlogCoreSolution.AccesoDatos.Inicializador;
using BlogCoreSolution.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("ConexionSQL") ?? throw new InvalidOperationException("Connection string 'ConexionSQL' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => 
options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultUI();
builder.Services.AddControllersWithViews();

// Agregar contenedor de trabajo al contenedor IoC de inyecci�n de dependencias
builder.Services.AddScoped<IContenedorTrabajo, ContenedorTrabajo>();

//Siembra de datos - Paso 1
builder.Services.AddScoped<IInicializadorBD, InicializadorBD>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

//Método que ejecuta la siembra de datos
SiembraDatos();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Cliente}/{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();

//Funcionalidad método SiembraDeDatos();
void SiembraDatos()
{
    using (var scope = app.Services.CreateScope())
    {
        var inicilizadorBD = scope.ServiceProvider.GetRequiredService<IInicializadorBD>();
        inicilizadorBD.Inicializar();
    }
}
