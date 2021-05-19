using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APICrudBasica.Data;
using APICrudBasica.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Threading;

[Route("products")]
public class ProductController : ControllerBase
{
    private readonly DataContext Contexto;
    public ProductController(DataContext context) => Contexto = context;

    [HttpGet]
    [Route("")]
    //[Authorize]
    public async Task<IActionResult> Get([FromServices]IMemoryCache _cache)
    {
        List<Product> listaProdutoRetorno = await _cache.GetOrCreateAsync("produtos", 
       async (entry) => {
           
           entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30);
           entry.SetPriority(CacheItemPriority.High);
           Thread.Sleep(20000);
           return await Contexto.Products.Include(n => n.Category).ToListAsync();
       });

        if (!listaProdutoRetorno.Any()) return NotFound();

        return Ok(listaProdutoRetorno);
    }

    [HttpGet]
    [Route("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        Product retorno = await Contexto.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

        if (retorno is null) return NotFound();

        return Ok(retorno);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Product Product)
    {

        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            await Contexto.Products.AddAsync(Product);
            await Contexto.SaveChangesAsync();
        }
        catch (Exception)
        {

            return BadRequest(new { error = true, message = "Não foi possível cadastrar um Produto." });
        }

        return Ok(await Contexto.Products.ToListAsync());
    }

    [HttpPut]
    [Route("{id:int}")]
    public async Task<IActionResult> Put([FromBody] Product Product, int id)
    {

        if (!ModelState.IsValid) return BadRequest(ModelState.Select(n => new { campo = n.Key, erros = n.Value.Errors.Select(x => x.ErrorMessage) }));

        var ProdutoAtual = await Contexto.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

        if (ProdutoAtual is null) return NotFound();

        ProdutoAtual.Title = Product.Title;

        try
        {
            Contexto.Products.Update(ProdutoAtual);
            await Contexto.SaveChangesAsync();
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Não foi possível criar um Produto." });
        }

        return Ok(ProdutoAtual);
    }

    [HttpDelete]
    [Route("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {

        if (!Contexto.Products.Any(x => x.Id == id)) return NotFound();

        try{
        Contexto.Products.Remove(await Contexto.Products.FirstAsync(x => x.Id == id));
        await Contexto.SaveChangesAsync();
        }catch(DbUpdateConcurrencyException){

            return BadRequest(new{ error = true, message = "Houve concorrência de dados."});
            
        }catch{
                return BadRequest(new { erro = true, message = "Não foi possível remover" });
        }

        return Ok(await Contexto.Products.ToListAsync());
    }

}