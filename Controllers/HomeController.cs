using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APICrudBasica.Data;
using APICrudBasica.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("load")]
public class HomeController : ControllerBase
{
    private readonly DataContext Contexto;
    public HomeController(DataContext context) => Contexto = context;

    [HttpGet]
    [Route("")]
    public async Task<IActionResult> Get()
    {
        var categories = new [] {
                new Category { Title = "C#" },
                new Category { Title = "FrontEnd" }
        };


        var products = new [] {
            new Product { Title = "Treinamento Asp.Net Core MVC Razor Pages" , Price = 1000, Description = "Formação Completa" , CategoryId = 1},
            new Product { Title = "Treinamento Asp.Net Core Web Api" , Price = 2000, Description = "Formação Completa API" , CategoryId = 1}
        };

        
        var users = new [] {
            new User { Username = "davi.murilo" , Password = "123123" , Role = "TI"},
            new User { Username = "murilo.davi" , Password = "321321" , Role = "Gerente"},
        };

        await Contexto.Categories.AddRangeAsync(categories);
        await Contexto.Products.AddRangeAsync(products);
        await Contexto.Users.AddRangeAsync(users);

        await Contexto.SaveChangesAsync();

        return Ok(new { mensagem = "Carga Inicial Feita Com Sucesso!"});
    }
}