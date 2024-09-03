using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimal_api.Dominio.Entidades;

namespace Teste.Domain.Entidades
{   
    [TestClass]
    public class VeiculoTeste
    {
        [TestMethod]
        public void TestarGetSetPropriedades()
        {   
            //Arrange 
            var veiculo = new Veiculo();

            
            //action
            veiculo.Id = 1;
            veiculo.Marca = "chevrolet";
            veiculo.Nome = "Corsa";
            veiculo.Ano = 2010;
            
            
            //assert
            Assert.AreEqual(1, veiculo.Id);
            Assert.AreEqual("chevrolet", veiculo.Marca);
            Assert.AreEqual("Corsa", veiculo.Nome);
            Assert.AreEqual(2010, veiculo.Ano);

            
        }
    }
}