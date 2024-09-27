using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using WebApi8.Data;
using WebApi8.Dto.Usuario;
using WebApi8.Models;
using WebApi8.Services.Senha;

namespace WebApi8.Services.Auth
{
    public class AuthService : IAuthInterface
    {
        private readonly AppDbContext _context;
        private readonly ISenhaInterface _senhaInterface;
        public AuthService(AppDbContext context, ISenhaInterface senhaInterface)
        {
            _context = context;
            _senhaInterface = senhaInterface;
        }

        public async Task<ResponseModel<UsuarioCriacaoDto>> Registrar(UsuarioCriacaoDto usuarioCriacaoDto)
        {
            ResponseModel<UsuarioCriacaoDto> resposta = new ResponseModel<UsuarioCriacaoDto>();

            try
            {

                if (!VerificaEmailUsuarioExiste(usuarioCriacaoDto))
                {
                    resposta.Dados = null;
                    resposta.Status = false;
                    resposta.Mensagem = "Email/Usuario já existe";
                    return resposta;
                }

                _senhaInterface.CriarSenhaHash(usuarioCriacaoDto.Senha, out byte[] senhaHash, out byte[] senhaSalt);

                UsuarioModel usuario = new UsuarioModel()
                {
                    Usuario = usuarioCriacaoDto.Usuario,
                    Email = usuarioCriacaoDto.Email,
                    SenhaHash = senhaHash,
                    SenhaSalt = senhaSalt
                };

                _context.Add(usuario);
                await _context.SaveChangesAsync();

                resposta.Mensagem = "Usuario criado com sucesso";

            }
            catch (Exception ex)
            {
                resposta.Dados = null;
                resposta.Mensagem = ex.Message;
                resposta.Status = false;
            }

            return resposta;
        }

        public async Task<ResponseModel<string>> Login(UsuarioLoginDto usuarioLoginDto)
        {
            ResponseModel<string> resposta = new ResponseModel<string>();

            try
            {

                var usuario = await _context.Usuarios.FirstOrDefaultAsync(userBanco => userBanco.Email == usuarioLoginDto.Email);

                if (usuario == null)
                {
                    resposta.Mensagem = "Credenciais inválidas";
                    resposta.Status = false;
                    return resposta;
                }

                if (!_senhaInterface.VerificaSenhaHash(usuarioLoginDto.Senha, usuario.SenhaHash, usuario.SenhaSalt))
                {
                    resposta.Mensagem = "Credenciais inválidas";
                    resposta.Status = false;
                    return resposta;
                }

                var token = _senhaInterface.CriarToken(usuario);

                resposta.Dados = token;
                resposta.Mensagem = "Usuario logado com sucesso";
            }
            catch(Exception ex)
            {
                resposta.Dados = null;
                resposta.Mensagem = ex.Message;
                resposta.Status = false;
            }

            return resposta;
        }

        private bool VerificaEmailUsuarioExiste(UsuarioCriacaoDto usuarioCriacaoDto)
        {
            var usuario = _context.Usuarios.FirstOrDefault(userBanco => userBanco.Usuario == usuarioCriacaoDto.Usuario || userBanco.Email == usuarioCriacaoDto.Email);

            if (usuario != null) return false;

            return true;
        }
    }
}
