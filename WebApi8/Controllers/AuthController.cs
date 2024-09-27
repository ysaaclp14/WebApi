using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi8.Dto.Usuario;
using WebApi8.Services.Auth;

namespace WebApi8.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("MyPolicy")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthInterface _authInterface;
        public AuthController(IAuthInterface authInterface)
        {
            _authInterface = authInterface;
        }

        [HttpPost("Login")]
        public async Task<ActionResult> Login(UsuarioLoginDto usuarioLoginDto)
        {
            var resposta = await _authInterface.Login(usuarioLoginDto);
            return Ok(resposta);
        }

        [HttpPost("Register")]
        public async Task<ActionResult> Register(UsuarioCriacaoDto usuarioCriacaoDto)
        {
            var resposta = await _authInterface.Registrar(usuarioCriacaoDto);
            return Ok(resposta);
        }
    }
}
