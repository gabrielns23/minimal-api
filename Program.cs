using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using minimal_api.Dominio.DTOs;
using minimal_api.Dominio.Entidades;
using minimal_api.Dominio.Enuns;
using minimal_api.Dominio.Interfaces;
using minimal_api.Dominio.ModelViews;
using minimal_api.Infraestrutura.Db;
using minimal_api.Infraestrutura.Servicos;
using Swashbuckle.AspNetCore.SwaggerGen;

#region Builder
var builder = WebApplication.CreateBuilder(args);

var key = builder.Configuration.GetSection("Jwt")["Key"].ToString();

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option =>
{
    option.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ValidateIssuer = false, 
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization();

builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();
builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen( options =>{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT dessa maneira: {Seu token}"
    });
   
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
             new OpenApiSecurityScheme{
                
             Reference = new OpenApiReference
             {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
             }
            
             },
            new string[] {}
        }
    });
});


builder.Services.AddDbContext<DbContexto>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
        );
}
);

var app = builder.Build();
#endregion

#region Home

app.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");

#endregion

#region Administradores

string GerarTokenJWT(Administrador administrador){
    
    if(string.IsNullOrEmpty(key)) return string.Empty;
    
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
    
    var claims = new List<Claim>()
    {
        new Claim("Email", administrador.Email),
        new Claim("Perfil", administrador.Perfil),
        new Claim(ClaimTypes.Role, administrador.Perfil)
    };

    var token = new JwtSecurityToken(
        claims: claims,
        expires : DateTime.Now.AddDays(1),
        signingCredentials :  credentials

    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}

app.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico) =>
{
    var adm = administradorServico.Login(loginDTO);

    if (adm != null)
    {
        string token = GerarTokenJWT(adm);
        return Results.Ok(new AdministradorLogado
        {
            Email = adm.Email,
            Perfil = adm.Perfil,
            Token = token
        });
    }
    return Results.Unauthorized();
}).AllowAnonymous().WithTags("Administrador");


app.MapGet("/administradores", ([FromQuery] int? pagina, IAdministradorServico administradorServico) =>
{
    var adms = new List<AdministradorModelView>();
    var administradores = administradorServico.GetAll(pagina);

    foreach (var adm in administradores)
    {
        adms.Add(new AdministradorModelView
        {
            Id = adm.Id,
            Email = adm.Email,
            Perfil = adm.Perfil
        });
    }


    return Results.Ok(adms);
})
.RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute{Roles = "adm"})
.WithTags("Administrador");

app.MapGet("/administradores/{id}", ([FromRoute] int id, IAdministradorServico administradorServico) =>
{

    var administrador = administradorServico.BuscarPorId(id);
    var admView = new AdministradorModelView
    {
        Id = administrador.Id,
        Email = administrador.Email,
        Perfil = administrador.Perfil

    };

    if (administrador == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(admView);

})
.RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute{Roles = "adm"})
.WithTags("Administrador");


app.MapPost("/administradores", ([FromBody] AdministradorDTO administradorDTO, IAdministradorServico administradorServico) =>
{
    var validacao = new ErrosDeValidacao
    {
        Mensagens = new List<string>()
    };

    if (string.IsNullOrEmpty(administradorDTO.Email))
    {
        validacao.Mensagens.Add("O email não pode ser nulo");
    }

    if (string.IsNullOrEmpty(administradorDTO.Senha))
    {
        validacao.Mensagens.Add("A senha não pode ser nula");
    }

    if (administradorDTO.Perfil == null)
    {
        validacao.Mensagens.Add("O perfil não pode ser nulo");
    }

    if (validacao.Mensagens.Count > 0)
    {
        return Results.BadRequest(validacao);
    }

    var administrador = new Administrador
    {
        Email = administradorDTO.Email,
        Senha = administradorDTO.Senha,
        Perfil = administradorDTO.Perfil.ToString() ?? Perfil.editor.ToString()
    };

    administradorServico.Incluir(administrador);

    return Results.Created($"/administrador/{administrador.Id}", new AdministradorModelView
    {
        Id = administrador.Id,
        Email = administrador.Email,
        Perfil = administrador.Perfil
    });
})
.RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute{Roles = "adm"})
.WithTags("Administrador");

#endregion

#region Veiculos

ErrosDeValidacao ValidaDTO(VeiculoDTO veiculoDTO)
{
    var validacao = new ErrosDeValidacao
    {
        Mensagens = new List<string>()
    };

    if (string.IsNullOrEmpty(veiculoDTO.Nome))
    {
        validacao.Mensagens.Add("O nome não pode ser vazio");
    }

    if (string.IsNullOrEmpty(veiculoDTO.Marca))
    {
        validacao.Mensagens.Add("A marca não pode ser vazia");
    }

    if (veiculoDTO.Ano <= 1950)
    {
        validacao.Mensagens.Add("O ano do veículo não pode ser inferior a 1950");
    }

    return validacao;
}


app.MapPost("veiculos/", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
{

    var validacao = ValidaDTO(veiculoDTO);
    if (validacao.Mensagens.Count > 0)
    {
        return Results.BadRequest(validacao);
    }

    var veiculo = new Veiculo
    {
        Nome = veiculoDTO.Nome,
        Marca = veiculoDTO.Marca,
        Ano = veiculoDTO.Ano
    };

    veiculoServico.IncluirVeiculo(veiculo);

    return Results.Created($"/veiculo/{veiculo.Id}", veiculo);

})
.RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute{Roles = "adm,editor"})
.WithTags("Veiculo");

app.MapGet("/veiculos", ([FromQuery] int? pagina, IVeiculoServico veiculoServico) =>
{
    var veiculos = veiculoServico.GetAll();


    return Results.Ok(veiculos);

}).RequireAuthorization().WithTags("Veiculo");

app.MapGet("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscaPorId(id);

    if (veiculo == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(veiculo);

})
.RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute{Roles = "adm,editor"})
.WithTags("Veiculo");

app.MapPut("/veiculos/{id}", ([FromRoute] int id, [FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscaPorId(id);

    if (veiculo == null)
    {
        return Results.NotFound();
    }

    var validacao = ValidaDTO(veiculoDTO);
    if (validacao.Mensagens.Count > 0)
    {
        return Results.BadRequest(validacao);
    }

    veiculo.Nome = veiculoDTO.Nome;
    veiculo.Marca = veiculoDTO.Marca;
    veiculo.Ano = veiculoDTO.Ano;

    veiculoServico.Atualizar(veiculo);

    return Results.Ok(veiculo);

})
.RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute{Roles = "adm"})
.WithTags("Veiculo");

app.MapDelete("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscaPorId(id);

    if (veiculo == null)
    {
        return Results.NotFound();
    }

    veiculoServico.Apagar(veiculo);

    return Results.NoContent();

})
.RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute{Roles = "adm"})
.WithTags("Veiculo");



#endregion

#region App

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Minha API v1"));


app.UseAuthentication();
app.UseAuthorization();

app.Run();

#endregion
