using Psicologa.Application.Usuario.ViewsModel;
using Shared.Infra.CrossCutting.ValidationResult;
using Shared.Infra.CrossCutting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Psicologa.Application.Usuario.Services
{
    public class ApplicationUsuarioService : IDisposable
    {
        private readonly Domain.Usuario.Services.UsuarioService _usuarioService;
        private readonly Domain.Pessoa.Services.PessoaService _pessoaService;
        private readonly IAppSettings _appSettings;

        public ApplicationUsuarioService(Domain.Usuario.Services.UsuarioService usuarioService, Domain.Pessoa.Services.PessoaService pessoaService, IAppSettings appSettings)
        {
            _usuarioService = usuarioService;
            _pessoaService = pessoaService;
            _appSettings = appSettings;
        }

        public (bool, ValidationResult) Salvar(UsuarioViewModel uVM)
        {
            bool operacao = false;

            Domain.Usuario.Entities.Usuario u = new Domain.Usuario.Entities.Usuario();

            u.Id = uVM.Id;
            u.Nome = uVM.Nome.Trim().ToLower();
            u.Senha = uVM.Senha;
            u.Perfil = (Domain.Usuario.Entities.PerfilUsuario.TpPerfil)uVM.PerfilId;
            //u.Perfil = uVM.PerfilNome;
            u.Pessoa = new Domain.Pessoa.Entities.Pessoa()
            {
                Id = uVM.PessoaId
            };

            operacao = _usuarioService.Salvar(u, uVM.SenhaConfirmacao);

            if (operacao)
                uVM.Id = u.Id;

            return (operacao, u.ValidationResult);
        }

        public UsuarioConsultaViewModel Obter(int id)
        {
            var u = _usuarioService.Obter(id);

            return FormatarRetornoConsulta(u);
        }

        public UsuarioConsultaViewModel Obter(string usuarioNome, string senha)
        {
            var u = _usuarioService.Obter(usuarioNome, senha);

            return FormatarRetornoConsulta(u);
        }

        public bool Excluir(int id)
        {
            return _usuarioService.Excluir(id);
        }

        public IEnumerable<UsuarioConsultaViewModel> Consultar(string nome)
        {
            var us = _usuarioService.Consultar(nome);
            List<UsuarioConsultaViewModel> retorno = new List<UsuarioConsultaViewModel>();
            PaginacaoDados paginacao = new PaginacaoDados(0, 30, (PaginacaoDados.TpOrdenacao)1);
            var pessoasSemUsuario = _pessoaService.Consultar(nome, paginacao);

            foreach (var cc in us)
            {
                retorno.Add(FormatarRetornoConsulta(cc));
            }

            //incluindo pessoas sem usuários.
            foreach (var c in pessoasSemUsuario)
            {
                if (retorno.Where(r => r.PessoaId == c.Id).Count() == 0)
                {
                    retorno.Add(new UsuarioConsultaViewModel()
                    {
                        PessoaId = c.Id,
                        PessoaNome = c.Nome
                    });
                }
            }

            retorno.OrderBy(o => o.PessoaNome);

            return retorno;
        }

        public IEnumerable<UsuarioConsultaViewModel> ObterUltimos(int top)
        {
            var us = _usuarioService.ObterUltimos(top);
            List<UsuarioConsultaViewModel> retorno = new List<UsuarioConsultaViewModel>();

            foreach (var u in us)
            {
                retorno.Add(FormatarRetornoConsulta(u));
            }

            return retorno;
        }


        internal UsuarioConsultaViewModel FormatarRetornoConsulta(Domain.Usuario.Entities.Usuario u)
        {
            if (u == null)
                return null;

            var ccRetorno = new UsuarioConsultaViewModel()
            {
                Id = u.Id,
                Nome = u.Nome,
                PerfilId = (int)u.Perfil,
                PerfilNome = Utils.ObterDescricaoEnum(u.Perfil),
                PessoaId = u.Pessoa.Id,
                PessoaNome = u.Pessoa.Nome
            };

            return ccRetorno;
        }

        public (bool, ValidationResult) EnviarEmailRecuperarSenha(string nome)
        {
            bool ok = false;
            ValidationResult vr = new ValidationResult();


            var usuario = _usuarioService.ObterPorUsuarioNome(nome.ToLower());

            if (usuario == null)
            {
                vr.Add(Message.TypeMessage.Error, "Usuário não encontrado.");
            }
            else
            {
                var pessoa = _pessoaService.Obter(usuario.Pessoa.Id);
                var contato = pessoa.Contatos.Where(c => c.Tipo == Domain.Pessoa.Entities.PessoaContato.TpContato.Email).FirstOrDefault();

                if (contato == null)
                {
                    vr.Add(Message.TypeMessage.Error, "Usuário não possui um e-mail cadastrado. Por favor, entrar em contato.");
                }
                else
                {
                    Random random = new Random();
                    string codigoSeguranca = random.Next(10000, 1000000).ToString();
                    _usuarioService.AtualizarCodigoSeguranca(usuario.Id, codigoSeguranca);

                    Email e = new Email(_appSettings.Email.Email, _appSettings.Email.Email, _appSettings.Email.Senha, _appSettings.Email.Remetente, _appSettings.Email.SMTP, _appSettings.Email.Porta);
                    ok = true;
                    _usuarioService.EnviarEmailRecuperarSenha(usuario, e, codigoSeguranca);
                    //vr.Add(Message.TypeMessage.Success, $"Foi enviada uma mensagem para \"{contato.Contato}\" com o código de segurança necessário para continuar.");
                    vr.Add(Message.TypeMessage.Success, $"Código de recuperação enviado para {contato.Contato}");
                }
            }

            return (ok, vr);
        }


        public (bool, ValidationResult) AlterarSenha(string nome, string codigoSeguranca, string novaSenha, string novaSenhaConfirmar)
        {
            ValidationResult vr = new ValidationResult();
            nome = nome.Trim().ToLower();

            var usuario = _usuarioService.ObterPorUsuarioNome(nome);

            if (usuario == null)
            {
                vr.Add(Message.TypeMessage.InvalidField, "Forneça o usuário.");
                return (false, vr);
            }

            if (string.IsNullOrEmpty(codigoSeguranca))
            {
                vr.Add(Message.TypeMessage.InvalidField, "Forneça o código de segurança.");
                return (false, vr);
            }

            codigoSeguranca = codigoSeguranca.Trim();

            if (usuario.CodigoSeguranca.Trim() != codigoSeguranca)
            {
                vr.Add(Message.TypeMessage.InvalidField, "Código de Segurança inválido.");
                return (false, vr);
            }

            if (string.IsNullOrEmpty(novaSenha) || string.IsNullOrEmpty(novaSenhaConfirmar))
            {
                vr.Add(Message.TypeMessage.InvalidField, "Forneça todas as senhas.");
                return (false, vr);
            }
            else if (novaSenha.Trim() != novaSenhaConfirmar.Trim())
            {
                vr.Add(Message.TypeMessage.InvalidField, "A nova senha é diferente da sua confirmação.");
                return (false, vr);
            }
            else
            {
                novaSenha = novaSenha.Trim();
                bool ok = _usuarioService.AlterarSenha(usuario.Id, novaSenha);
                return (ok, vr);
            }

        }

        public int ObterQuantidadeUsuariosCadastrados()
        {
            return _usuarioService.ObterQuantidadeUsuariosCadastrados();
        }

        public void Dispose()
        {

        }
    }

}
