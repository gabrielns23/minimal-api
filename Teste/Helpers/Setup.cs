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

namespace Teste.Helpers
{
    public class Setup
    {
        public const string PORT = "5001";
        public static TestContext testContext = default!;
        public static WebApplicationFactory<Startup> http = default!;
        public static HttpClient client = default!;

        public static async Task ExecutaComandoSqlAsync(string sql)
        {
            await new DbContexto().Database.ExecuteSqlRawAsync(sql);
        }
    
    }
}