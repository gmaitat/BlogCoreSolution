using BlogCoreSolution.AccesoDatos.Data;
using BlogCoreSolution.Models;
using BlogCoreSolution.Utilidades;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlogCoreSolution.AccesoDatos.Inicializador
{
    public class InicializadorBD : IInicializadorBD
    {
        private readonly ApplicationDbContext _bd;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public InicializadorBD(ApplicationDbContext bd,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _bd = bd;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        public void Inicializar()
        {
            try
            {
                if (_bd.Database.GetPendingMigrations().Count() > 0)
                {
                    _bd.Database.Migrate();
                }
            }
            catch (Exception)
            {                
            }

            if (_bd.Roles.Any(ro => ro.Name == CNT.Administrador)) return;

            //Creación de roles
            _roleManager.CreateAsync(new IdentityRole(CNT.Administrador)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(CNT.Registrado)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(CNT.Cliente)).GetAwaiter().GetResult();

            //Creación del usuario inicial
            var resultado = _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "joseandresmontoya@hotmail.com",
                Email = "joseandresmontoya@hotmail.com",
                EmailConfirmed = true,
                Nombre = "render2web",
                Direccion = "Calle 24",
                Ciudad = "Pereira",
                Pais = "Colombia"
            }, "Admin123*").GetAwaiter().GetResult();

            var usuario = _userManager.FindByEmailAsync("joseandresmontoya@hotmail.com")
                .GetAwaiter().GetResult();

            if (!resultado.Succeeded)
            {
                // Aquí puedes lanzar excepción o loguear errores
                throw new Exception(
                    string.Join(" | ", resultado.Errors.Select(e => e.Description))
                );
            }

            if (usuario != null)
            {
                _userManager.AddToRoleAsync(usuario, CNT.Administrador)
                    .GetAwaiter().GetResult();
            }
        }
    }
}
