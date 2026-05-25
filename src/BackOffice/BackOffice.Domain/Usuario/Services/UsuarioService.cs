using Psicologa.Domain.Usuario.Interfaces.Repositories;
using Shared.Infra.CrossCutting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Psicologa.Domain.Usuario.Services
{
    public class UsuarioService : ServiceBase<Domain.Usuario.Entities.Usuario>, IServiceBase<Domain.Usuario.Entities.Usuario>
    {
        private readonly Interfaces.Repositories.IUsuarioRepository _repository;
        private readonly Domain.Pessoa.Services.PessoaService _pessoaService;

        public UsuarioService(IUsuarioRepository repository, Pessoa.Services.PessoaService pessoaService)
            : base(repository)
        {
            _repository = repository;
            _pessoaService = pessoaService;
        }

        public bool Salvar(Domain.Usuario.Entities.Usuario usuario)
        {
            throw new Exception("Não usar");
        }

        public bool Salvar(Domain.Usuario.Entities.Usuario usuario, string senhaConfirmacao)
        {
            bool operacao = false;

            bool valido = false;
            if (usuario.Id == 0)
            {
                valido = usuario.Validar(senhaConfirmacao);
            }
            else
            {
                valido = usuario.Validar();
            }

            if (valido)
            {
                if (usuario.Id <= 0)
                {
                    if (ObterPorUsuarioNome(usuario.Nome) != null)
                    {
                        valido = false;
                        usuario.ValidationResult.AddUserMessageInvalidField("Nome de usuário já utilizado por outro usuário.");
                    }
                    else if (ObterPorPessoa(usuario.Pessoa.Id) != null)
                    {
                        usuario.ValidationResult.AddUserMessageInvalidField( "A Pessoa já possui um usuário.");
                        valido = false;
                    }
                }
                else
                {
                    var uAux = Obter(usuario.Id);

                    if (uAux != null && uAux.Nome != usuario.Nome)
                    {
                        if (ObterPorUsuarioNome(usuario.Nome) != null)
                        {
                            valido = false;
                            usuario.ValidationResult.AddUserMessageInvalidField("Nome de usuário já utilizado por outro usuário.");
                        }
                    }
                }

                if (valido)
                {
                    if (usuario.Id == 0)
                    {
                        usuario.DataCadastro = DateTime.Now;
                        usuario.DataSenha = DateTime.Now;
                    }

                    usuario.Nome = usuario.Nome.ToLower().Trim();
                    usuario.Senha = CriptografarSenha(usuario.Senha);

                    operacao = _repository.Salvar(usuario);
                }
            }

            return operacao;
        }

        public Domain.Usuario.Entities.Usuario Obter(string usuarioNome, string senha)
        {
            senha = CriptografarSenha(senha);
            return _repository.Obter(usuarioNome, senha);
        }


        public Domain.Usuario.Entities.Usuario Obter(int id)
        {
            return _repository.Obter(id);
        }

        public bool Excluir(int id)
        {
            return _repository.Excluir(id);
        }

        public bool ExcluirPorPessoa(int id)
        {
            return _repository.Excluir(id);
        }

        public IEnumerable<Domain.Usuario.Entities.Usuario> ObterUltimos(int top)
        {
            return _repository.ObterUltimos(top);
        }

        public IEnumerable<Domain.Usuario.Entities.Usuario> Consultar(string usuarioNome)
        {
            return _repository.Consultar(usuarioNome);
        }

        public Domain.Usuario.Entities.Usuario ObterPorUsuarioNome(string nome)
        {
            return _repository.ObterPorUsuarioNome(nome);   
        }

        public Domain.Usuario.Entities.Usuario ObterPorPessoa(int pessoaId)
        {
            return _repository.ObterPorPessoa(pessoaId);
        }

        public void AtualizarCodigoSeguranca(int id, string codigoSeguranca)
        {
            _repository.AtualizarCodigoSeguranca(id, codigoSeguranca);
        }

        public bool AlterarSenha(int id, string senha)
        {
            senha = CriptografarSenha(senha);
            return _repository.AlterarSenha(id, senha);
        }

        public string CriptografarSenha(string senhaLimpa)
        {
            string key = "<<$$##M4r4ng0n##$$>>";
            senhaLimpa += senhaLimpa + key;
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(senhaLimpa));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }

        public void EnviarEmailRecuperarSenha(Domain.Usuario.Entities.Usuario usuario, Email e, string codigoSeguranca)
        {
            var pessoa = _pessoaService.Obter(usuario.Pessoa.Id);
            var email = pessoa.Contatos.FirstOrDefault(c => c.Tipo == Domain.Pessoa.Entities.PessoaContato.TpContato.Email).Contato;

            string texto =
                $@"<br />
                Olá <b>{pessoa.Nome.Split(" ")[0]}</b>.
                <br />
                <div>Para obter uma nova senha de acesso ao Portal, volte à página e utilize o código de segurança: <b>{codigoSeguranca}</b>.</div> 
                <br/><br/>";

            e.EnviarEmail(email, texto, "Recuperação de senha");
        }

        public int ObterQuantidadeUsuariosCadastrados()
        {
            return _repository.ObterQuantidadeUsuariosCadastrados();
        }

        //Gerar Senhas
        public bool ExisteLogin(string login)
        {
            return ObterPorUsuarioNome(login) != null;
        }

        public string GerarSenhaUsuario()
        {
            const string maiusculas = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string minusculas = "abcdefghijklmnopqrstuvwxyz";
            const string numeros = "0123456789";
            const string todos = maiusculas + minusculas + numeros;

            int tamanhoSenha = 10;
            var rng = RandomNumberGenerator.Create();
            var senha = new char[tamanhoSenha];

            // Garante ao menos 1 de cada tipo
            senha[0] = ObterCaractereAleatorio(maiusculas, rng);
            senha[1] = ObterCaractereAleatorio(minusculas, rng);
            senha[2] = ObterCaractereAleatorio(numeros, rng);

            // Preenche o restante aleatoriamente
            for (int i = 3; i < tamanhoSenha; i++)
                senha[i] = ObterCaractereAleatorio(todos, rng);

            // Embaralha para não fixar os tipos nas primeiras posições
            return new string(senha.OrderBy(_ => ObterByteAleatorio(rng)).ToArray());
        }

        private char ObterCaractereAleatorio(string fonte, RandomNumberGenerator rng)
        {
            byte[] buffer = new byte[1];
            int indice;
            // Evita viés de módulo
            do { rng.GetBytes(buffer); } while (buffer[0] >= 256 - (256 % fonte.Length));
            indice = buffer[0] % fonte.Length;
            return fonte[indice];
        }

        private byte ObterByteAleatorio(RandomNumberGenerator rng)
        {
            byte[] buffer = new byte[1];
            rng.GetBytes(buffer);
            return buffer[0];
        }

        public string GerarLoginUsuario(string nome, Func<string, bool> loginExiste)
        {
            var preposicoes = new HashSet<string> { "da", "de", "do", "dos", "das", "e" };

            // 1. Remove números e pontos
            var nomeLimpo = Regex.Replace(nome, @"[\d.]+", string.Empty);
            // 2. Colapsa espaços múltiplos e remove das bordas
            nomeLimpo = Regex.Replace(nomeLimpo, @"\s{2,}", " ").Trim();

            // Normaliza — remove acentos e converte para minúsculo
            var partes = nomeLimpo.Normalize(NormalizationForm.FormD)
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .Aggregate(string.Empty, (acc, c) => acc + c)
                .ToLower()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Where(p => !preposicoes.Contains(p))
                .ToArray();

            if (partes.Length == 0) return string.Empty;

            var primeiro = partes[0];
            var ultimo = partes.Length > 1 ? partes[^1] : partes[0];
            var loginBase = $"{primeiro}.{ultimo}";

            // Trata colisão
            var login = loginBase;
            int contador = 2;
            while (loginExiste(login))
            {
                login = $"{loginBase}{contador}";
                contador++;
            }

            return login;
        }



    }
}
