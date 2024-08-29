using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimal_api.Dominio.Entidades;

namespace minimal_api.Dominio.Interfaces
{
    public interface IVeiculoServico
    {   
        List<Veiculo> GetAll(int? pagina = 1, string? nome = null, string? marca = null);
        void IncluirVeiculo(Veiculo veiculo);
        Veiculo? BuscaPorId(int id);
        void Atualizar(Veiculo veiculo);
        void Apagar(Veiculo veiculo);


    }
}