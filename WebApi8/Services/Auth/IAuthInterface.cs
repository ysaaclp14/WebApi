using WebApi8.Dto.Usuario;
using WebApi8.Models;

namespace WebApi8.Services.Auth
{
    public interface IAuthInterface
    {
        Task<ResponseModel<UsuarioCriacaoDto>> Registrar(UsuarioCriacaoDto usuarioCriacaoDto);
        Task<ResponseModel<string>> Login(UsuarioLoginDto usuarioLoginDto);
    }
}
