using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using minimal_api.Dominio.Entidades;
using minimal_api.Infraestrutura.Db;
using minimal_api.Infraestrutura.Servicos;

namespace Teste.Domain.Servicos
{   
    [TestClass]
    public class AdministradorServicoTeste
    {   
        private DbContexto CriarContextoTeste()
        {   
            var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var path = Path.GetFullPath(Path.Combine(assemblyPath?? "", "..", "..", ".."));

            var builder = new ConfigurationBuilder()
                              .SetBasePath(path?? Directory.GetCurrentDirectory())
                              .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                              .AddEnvironmentVariables();

            var configuration = builder.Build();

            var connectionString = configuration.GetConnectionString("mysql");                   

            var options = new DbContextOptionsBuilder<DbContexto>()
                              .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                              .Options;  

            
            return new DbContexto(configuration);
        }
        
       
        [TestMethod]
        public void TestandoSalvarAdministradorAndPegarTodos()
        {
            //Arrange 
            var context = CriarContextoTeste();
            context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");

            var adm = new Administrador();
            adm.Email = "teste@teste.com";
            adm.Senha = "teste";
            adm.Perfil = "editor";
            
            var administradorServico = new AdministradorServico(context);
        
            //action 
            
            administradorServico.Incluir(adm);
            
            //assert
            Assert.AreEqual(1, administradorServico.GetAll(1).Count());
            
        }

        [TestMethod]
        public void TestandoBuscarAdministradorPorId()
        {
            //Arrange 
            var context = CriarContextoTeste();
            context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");

            var adm = new Administrador();
            adm.Email = "teste@teste.com";
            adm.Senha = "teste";
            adm.Perfil = "editor";
            
            var administradorServico = new AdministradorServico(context);
        
            //action 
            
            administradorServico.Incluir(adm);
            var admTeste = administradorServico.BuscarPorId(adm.Id);
            
            //assert
            Assert.AreEqual(1, admTeste.Id);
            
        }


    }
}