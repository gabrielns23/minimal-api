using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.ModelViews;
using Teste.Helpers;

namespace Teste.Request
{   
    [TestClass]
    public class AdministradorRequestTeste
    {   
        [ClassInitialize]
        public static void ClassInit(TestContext testContext)
        {
            Setup.ClassInit(testContext);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Setup.ClassCleanup();
        }


        // [TestMethod]
        // public async Task TestarGetSetPropriedades()
        // {   
        //     //Arrange 
        //     var LoginDTO = new LoginDTO{
        //         Email = "adm@teste.com",
        //         Senha = "F1l0s0f1@"
        //     };

        //     var content = new StringContent(JsonSerializer.Serialize(LoginDTO), Encoding.UTF8, "Apilication/json");

            
        //     //action
        //     var response = await Setup.client.PostAsync("/administradores/login", content);
            
            
            
        //     //assert
        //     Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        //     var result = await response.Content.ReadAsStringAsync();
            
        //     var admLogado = JsonSerializer.Deserialize<AdministradorLogado>(result, new JsonSerializerOptions{
        //         PropertyNameCaseInsensitive = true
        //     });

    
        //     Assert.IsNotNull(admLogado?.Email ?? "");
        //     Assert.IsNotNull(admLogado?.Perfil ?? "");
        //     Assert.IsNotNull(admLogado?.Token ?? "");            

            
        // }
    }
}