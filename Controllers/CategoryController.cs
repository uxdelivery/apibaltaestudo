using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APICrudBasica.Data;
using APICrudBasica.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("categories")]
public class CategoryController : ControllerBase
{
    private readonly DataContext Contexto;
    public CategoryController(DataContext context) => Contexto = context;

    [HttpGet]
    [Route("")]
    [ResponseCache(VaryByHeader = "User-Agent" , Location = ResponseCacheLocation.Any , Duration = 3600)]
    public async Task<IActionResult> Get()
    {
        List<Category> listaCategoriaRetorno = await Contexto.Categories.ToListAsync();

        if (!listaCategoriaRetorno.Any()) return NotFound();


        return Ok( new { obj = listaCategoriaRetorno , cache = DateTime.Now});
    }

    [HttpGet]
    [Route("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        
        Category retorno = await Contexto.Categories.FirstOrDefaultAsync(x => x.Id == id);

        if (retorno is null) return NotFound();

        return Ok(retorno);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Category category)
    {

        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            await Contexto.Categories.AddAsync(category);
            await Contexto.SaveChangesAsync();
        }
        catch (Exception)
        {

            return BadRequest(new { error = true, message = "Não foi possível cadastrar uma categoria." });
        }

        return Ok(await Contexto.Categories.ToListAsync());
    }

    [HttpPut]
    [Route("{id:int}")]
    public async Task<IActionResult> Put([FromBody] Category category, int id)
    {

        if (!ModelState.IsValid) return BadRequest(ModelState.Select(n => new { campo = n.Key, erros = n.Value.Errors.Select(x => x.ErrorMessage) }));

        var categoriaAtual = await Contexto.Categories.FirstOrDefaultAsync(x => x.Id == id);

        if (categoriaAtual is null) return NotFound();

        categoriaAtual.Title = category.Title;

        try
        {
            Contexto.Categories.Update(categoriaAtual);
            await Contexto.SaveChangesAsync();
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Não foi possível criar uma categoria." });
        }

        return Ok(categoriaAtual);
    }

    [HttpDelete]
    [Route("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {

        if (!Contexto.Categories.Any(x => x.Id == id)) return NotFound();

        try{
        Contexto.Categories.Remove(await Contexto.Categories.AsNoTracking().FirstAsync(x => x.Id == id));
        await Contexto.SaveChangesAsync();
        }catch(DbUpdateConcurrencyException){

            return BadRequest(new{ error = true, message = "Houve concorrência de dados."});
            
        }catch{
                return BadRequest(new { erro = true, message = "Não foi possível remover" });
        }

        return Ok(await Contexto.Categories.ToListAsync());
    }

}