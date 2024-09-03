using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Interfaces;
using minimal_api.Infraestrutura.Db;


namespace minimal_api.Infraestrutura.Servicos
{
    public class AdministradorServico : IAdministradorServico
    {
        
        private readonly DbContexto _contexto;
        public AdministradorServico(DbContexto contexto)
        {
            _contexto = contexto;
        }
        public Administrador? Login(LoginDTO loginDTO)
        {   
            var adm = _contexto.Administradores.Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).FirstOrDefault();
            return adm;
            
        }

        public Administrador Incluir(Administrador administrador)
        {
            _contexto.Administradores.Add(administrador);
            _contexto.SaveChanges();
            return administrador;
        }


        public List<Administrador>? GetAll(int? pagina)
        {
            var query = _contexto.Administradores.AsQueryable();
        
            int itensPorPagina = 10;
            
            if(pagina != null)
            {
                query.Skip(((int)pagina-1) * itensPorPagina).Take(itensPorPagina);  
            }
             return query.ToList();
        }

        public Administrador? BuscarPorId(int id)
        {
            return  _contexto.Administradores.Where(a => a.Id == id).FirstOrDefault();
        }
    }
}