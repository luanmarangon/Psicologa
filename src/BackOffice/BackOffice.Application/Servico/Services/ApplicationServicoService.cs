using Psicologa.Application.Servico.ViewsModel;
using Psicologa.Domain.Psicologo.Entities;
using Psicologa.Domain.Servico.Entities;
using Shared.Infra.CrossCutting;
using Shared.Infra.CrossCutting.ValidationResult;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using static Shared.Infra.CrossCutting.PaginacaoDados;

namespace Psicologa.Application.Servico.Services
{
    public class ApplicationServicoService : IDisposable
    {
        private readonly Domain.Servico.Services.ServicoService _servicoService;
        private readonly Domain.LogAplicacao.Services.LogAplicacaoService _logAplicacaoService;
        private readonly IAppSettings _appSettings;

        public ApplicationServicoService(Domain.Servico.Services.ServicoService servicoService, Domain.LogAplicacao.Services.LogAplicacaoService logAplicacaoService, IAppSettings appSettings)
        {
            _servicoService = servicoService;
            _logAplicacaoService = logAplicacaoService;
            _appSettings = appSettings;
        }

        public (bool, ValidationResult) Salvar(ServicoViewModel servicoVM, string[] requisicao)
        {
            var dadosExistente = _servicoService.Obter(servicoVM.Id);

            bool operacao = false;
            Domain.Servico.Entities.Servico servico = new Domain.Servico.Entities.Servico();
            servico.Id = servicoVM.Id;
            servico.Nome = servicoVM.Nome;

            if (string.IsNullOrEmpty(servico.Url))
                servico.Url = MontarUrl(servico.Nome);
            else
                servico.Url = servico.Url;
            servico.DescricaoCurta = servicoVM.DescricaoCurta;
            servico.Descricao = servicoVM.Descricao;
            servico.TempoSessaoMinutos = servicoVM.TempoSessaoMinutos;
            servico.ValorSessao = servicoVM.ValorSessao;

            if (!string.IsNullOrEmpty(servicoVM.ImagemCapa))
            {
                var base64 = servicoVM.ImagemCapa;

                // Remove o prefixo data:image/...;base64,
                var commaIndex = base64.IndexOf(',');
                if (commaIndex >= 0)
                    base64 = base64.Substring(commaIndex + 1);

                servico.ImagemCapa = Convert.FromBase64String(base64);
            }
            else
            {
                servico.ImagemCapa = null;
            }
            servico.Online = servicoVM.Online;
            servico.Presencial = servicoVM.Presencial;
            servico.DestaqueHome = servicoVM.DestaqueHome;
            servico.OrdemExibicao = servicoVM.OrdemExibicao;
            servico.Ativo = servicoVM.Ativo;

            if (!string.IsNullOrEmpty(servico.Nome))
            {
                servico.Nome = servico.Nome.ToUpper();
            }


            operacao = servico.Validar();

            if (!operacao)
            {
                return (false, servico.ValidationResult);
            }

            // Validação nome duplicado
            bool nomeJaExiste = _servicoService.ObterPorNome(servico.Nome) != null;

            if (nomeJaExiste && servico.Id == 0)
            {
                servico.ValidationResult.AddUserMessageError("Já existe um serviço cadastrado com esse nome. Por favor verificar.");
                return (false, servico.ValidationResult);
            }

            // Salvar
            operacao = _servicoService.Salvar(servico);

            if (!operacao)
            {
                servico.ValidationResult.AddUserMessageError("Não foi possível salvar o serviço.");
                return (false, servico.ValidationResult);
            }

            // Retorna ID gerado
            servicoVM.Id = servico.Id;

            // Log
            //RegistrarLog(servicoVM.Id, requisicao, dadosExistente, "Servico");

            if (operacao)
            {
                _logAplicacaoService.Registrar(servicoVM.Id, requisicao, dadosExistente, servico, "Servico", "ApplicationServicoService", "Salvar");
            }

            return (true, servico.ValidationResult);
        }
        public IEnumerable<ServicoViewModel> Consultar(string nome, Domain.Servico.Entities.Servico.TpFiltroServico filtro, PaginacaoDados paginacao)
        {
            List<ServicoViewModel> retorno = new List<ServicoViewModel>();
            var servicos = _servicoService.Consultar(nome, filtro, paginacao);

            foreach (var serv in servicos)
            {
                retorno.Add(FormatarRetornoConsulta(serv));
            }

            paginacao.OrdenacaoNome = Utils.ObterDescricaoEnum(paginacao.Ordenacao);
            if (paginacao.Ordenacao == TpOrdenacao.Nome)
            {
                retorno = retorno.OrderBy(o => o.Nome).ToList();
            }

            return retorno;
        }
        public IEnumerable<ServicoViewModel> ObterTodos()
        {
            List<ServicoViewModel> retorno = new List<ServicoViewModel>();
            var servicos = _servicoService.ObterTodos();
            foreach (var servico in servicos)
            {
                retorno.Add(FormatarRetornoConsulta(servico));
            }
            return retorno;
        }
        public IEnumerable<ServicoViewModel> ObterTodosAtivos()
        {
            List<ServicoViewModel> retorno = new List<ServicoViewModel>();
            var servicos = _servicoService.ObterTodosAtivos();
            foreach (var servico in servicos)
            {
                retorno.Add(FormatarRetornoConsulta(servico));
            }
            return retorno;
        }
        public ServicoViewModel Obter(int servicoId)
        {
            ServicoViewModel retorno = new ServicoViewModel();
            retorno = FormatarRetornoConsulta(_servicoService.Obter(servicoId));
            return retorno;
        }
        public bool Excluir(int servicoId, string[] requisicao)
        {
            bool operacao = false;
            var dadosExistente = _servicoService.Obter(servicoId);
            operacao = _servicoService.Excluir(servicoId);

            //            RegistrarLog(servicoId, requisicao, dadosExistente, "Servico");

            if (operacao)
            {
                _logAplicacaoService.Registrar(servicoId, requisicao, dadosExistente, null, "Servico", "ApplicationServicoService", "Excluir");
            }

            return operacao;
        }
        public IEnumerable<ServicoViewModel> ObterDestaquesHome(int limite)
        {
            List<ServicoViewModel> retorno = new List<ServicoViewModel>();
            var servicos = _servicoService.ObterDestaquesHome(limite);
            foreach (var servico in servicos)
            {
                retorno.Add(FormatarRetornoConsulta(servico));
            }
            return retorno;
        }
        public ServicoViewModel ObterPorUrl(string blogUrl)
        {
            ServicoViewModel retorno = new ServicoViewModel();
            retorno = FormatarRetornoConsulta(_servicoService.ObterPorUrl(blogUrl));
            return retorno;
        }
        internal ServicoViewModel FormatarRetornoConsulta(Domain.Servico.Entities.Servico servico)
        {
            var servicoViewModel = new ServicoViewModel
            {
                Id = servico.Id,
                Nome = servico.Nome,
                Url = servico.Url,
                DescricaoCurta = servico.DescricaoCurta,
                Descricao = servico.Descricao,
                TempoSessaoMinutos = servico.TempoSessaoMinutos,
                ValorSessao = servico.ValorSessao,
                ImagemCapa = MontarImagem(servico.ImagemCapa),
                Online = servico.Online,
                Presencial = servico.Presencial,
                DestaqueHome = servico.DestaqueHome,
                OrdemExibicao = servico.OrdemExibicao,
                Ativo = servico.Ativo,
                DataCriacao = servico.DataCriacao,
                DataAtualizacao = servico.DataAtualizacao
            };

            return servicoViewModel;
        }
        
        //Funções Private
        private bool EhBase64(string s)
        {
            Span<byte> buffer = new Span<byte>(new byte[s.Length]);
            return Convert.TryFromBase64String(s, buffer, out _);
        }
        private string MontarImagem(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return null;

            var texto = System.Text.Encoding.UTF8.GetString(bytes);

            // 🔥 CASO 1: já é data:image
            if (texto.StartsWith("data:image"))
                return texto;

            // 🔥 CASO 2: é base64 puro (sem prefixo)
            if (EhBase64(texto))
                return $"data:image/jpeg;base64,{texto}";

            // 🔥 CASO 3: é imagem binária real
            var mime = ObterMimeType(bytes);
            return $"data:{mime};base64,{Convert.ToBase64String(bytes)}";
        }
        private string ObterMimeType(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 4)
                return "image/jpeg";

            // JPEG
            if (bytes[0] == 0xFF && bytes[1] == 0xD8)
                return "image/jpeg";

            // PNG
            if (bytes[0] == 0x89 && bytes[1] == 0x50)
                return "image/png";

            // GIF
            if (bytes[0] == 0x47 && bytes[1] == 0x49)
                return "image/gif";

            // WEBP
            if (bytes[0] == 0x52 && bytes[1] == 0x49)
                return "image/webp";

            return "image/jpeg"; // fallback
        }
        private string MontarUrl(string titulo)
        {
            string url = titulo.ToLower().Trim();
            url = url.Replace(" ", "-");
            url = url.Replace(".", "");
            url = url.Replace(",", "");
            url = url.Replace(";", "");
            url = url.Replace(":", "");
            url = url.Replace("?", "");
            url = url.Replace("!", "");
            return url;
        }
        private string ObterResumo(string conteudo)
        {
            if (string.IsNullOrWhiteSpace(conteudo))
                return string.Empty;

            // Remove tags HTML
            string textoLimpo = Regex.Replace(conteudo, "<.*?>", string.Empty);

            // Remove espaços extras
            textoLimpo = textoLimpo.Trim();

            int maxLength = 100;

            if (textoLimpo.Length <= maxLength)
                return textoLimpo;

            return textoLimpo.Substring(0, maxLength) + "...";
        }
        private void RegistrarLog(int servicoId, string[] requisicao, Domain.Servico.Entities.Servico dadosExistente, string nomeClasse)
        {
            var dadosAtualizado = _servicoService.Obter(servicoId);
            _logAplicacaoService.Registrar(
                entidadeId: servicoId,
                requisicao: requisicao,
                dadosAntes: dadosExistente,
                dadosDepois: dadosAtualizado,
                entidade: nomeClasse,
                aplicacao: MethodBase.GetCurrentMethod()?.DeclaringType?.Name,
                metodo: MethodBase.GetCurrentMethod()?.Name
            );
        }

        public void Dispose()
        {
        }
    }
}