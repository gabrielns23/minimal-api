using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using minimal_api;
using minimal_api.Infraestrutura.Db;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Interfaces;
using Teste.Mocks;

namespace Teste.Helpers
{
    public class Setup
    {
        public const string PORT = "5100";
        public static TestContext testContext = default!;
        public static WebApplicationFactory<Startup> http = default!;
        public static HttpClient client = default!;

        public static void ClassInit(TestContext testContext)
        {
            Setup.testContext = testContext;
            Setup.http = new WebApplicationFactory<Startup>();

            Setup.http = Setup.http.WithWebHostBuilder( builder => 
            {
                builder.UseSetting("http_port", Setup.PORT).UseEnvironment("Testing");

                builder.ConfigureServices(services => 
                {
                    services.AddScoped<IAdministradorServico, AdministradorServicoMock>();

                    //todo
                    services.AddScoped<IVeiculoServico, VeiculoServicoMock>();
                });
            });
           
            Setup.client = Setup.http.CreateClient();
         }

         public static void ClassCleanup()
         {
            Setup.http.Dispose();
         }
    
    }
}