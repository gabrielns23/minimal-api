using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Interfaces;

namespace Teste.Mocks
{
    public class AdministradorServicoMock : IAdministradorServico
    {
        private static List<Administrador> administradores = new List<Administrador>(){
            new Administrador{
                Id = 1,
                Email = "adm@teste.com",
                Senha = "123456",
                Perfil = "adm"
            },
            new Administrador{
                Id = 2,
                Email = "editor@teste.com",
                Senha = "123456",
                Perfil = "editor"
            }
        };

        public Administrador? BuscarPorId(int id)
        {
            return administradores.Find(a => a.Id == id);
        }

        public List<Administrador>? GetAll(int? pagina)
        {
            return administradores;
        }

        public Administrador Incluir(Administrador administrador)
        {   
            administrador.Id = administradores.Count() + 1;
            administradores.Add(administrador);

            return administrador;
        }

        public Administrador? Login(LoginDTO loginDTO)
        {   
            
            return administradores.Find(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha);
        }
    }
}