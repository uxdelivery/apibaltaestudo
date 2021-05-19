using System.Threading.Tasks;
using APICrudBasica.Data;
using APICrudBasica.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICrudBasica.Controllers{

    [Route("users")]
    public class UserController : ControllerBase {

        private readonly DataContext contextoGeral;
        public UserController(DataContext _contextoGeral) => contextoGeral = _contextoGeral;

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Post([FromBody]User user)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);

            if(await contextoGeral.Users.AsNoTracking().AnyAsync(n => n.Username == user.Username)) return BadRequest(new { error = true, message = "Usuário já existente"});

            try{    
                    await contextoGeral.Users.AddAsync(user);
                    await contextoGeral.SaveChangesAsync();
            }catch{
                    return BadRequest(new { error = true, message = "Não foi possível cadastrar o usuário"});
            }

            return Ok(user);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public async Task<IActionResult> Autenticar([FromBody]User user)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);

            var usuario = await contextoGeral.Users.AsNoTracking().FirstOrDefaultAsync(n => n.Username == user.Username && n.Password == user.Password);

            if(usuario is null) return NotFound(new { error = true, message = "Usuário inexistente"});

            var token = TokenServices.GenarateToken(usuario);

            usuario.Password = null;

            return Ok(new{ usuario , token});
        }

    }
}