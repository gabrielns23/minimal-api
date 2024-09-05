using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Interfaces;

namespace Teste.Mocks
{
    public class VeiculoServicoMock : IVeiculoServico
    {   
        private static List<Veiculo> veiculos = new List<Veiculo>();

        public void Apagar(Veiculo veiculo)
        {
            veiculos.Remove(veiculo);
        }

        public void Atualizar(Veiculo veiculo)
        {   
            var veiculoAtt = veiculos.Find(v => v == veiculo);
            
            if(veiculoAtt == null )
            {
                throw new Exception("O veículo não está cadastrado");
            }
            else
            {
                veiculos.Remove(veiculoAtt);
                veiculos.Add(veiculo);
            }
            
        }

        public Veiculo? BuscaPorId(int id)
        {
            return veiculos.Find(v => v.Id == id);
        }

        public List<Veiculo> GetAll(int? pagina = 1, string? nome = null, string? marca = null)
        {
            return veiculos;
        }

        public void IncluirVeiculo(Veiculo veiculo)
        {   
            veiculo.Id = veiculos.Count() + 1;
            veiculos.Add(veiculo);
        }
    }
}