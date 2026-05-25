using Psicologa.Application.ServicoContato.ViewsModel;
using Shared.Infra.CrossCutting;
using Shared.Infra.CrossCutting.ValidationResult;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using static Psicologa.Domain.ServicoContato.Entities.ServicoContato;
using static Shared.Infra.CrossCutting.PaginacaoDados;

namespace Psicologa.Application.ServicoContato.Services
{
    public class ApplicationServicoContatoService : IDisposable
    {
        private readonly Domain.ServicoContato.Services.ServicoContatoService _servicoService;
        private readonly Domain.LogAplicacao.Services.LogAplicacaoService _logAplicacaoService;
        private readonly IAppSettings _appSettings;

        public ApplicationServicoContatoService(Domain.ServicoContato.Services.ServicoContatoService servicoService, Domain.LogAplicacao.Services.LogAplicacaoService logAplicacaoService, IAppSettings appSettings)
        {
            _servicoService = servicoService;
            _logAplicacaoService = logAplicacaoService;
            _appSettings = appSettings;
        }

        public (bool, ValidationResult) Salvar(ServicoContatoViewModel servicoVM, string[] requisicao)
        {
            var dadosExistente = _servicoService.Obter(servicoVM.Id);

            bool operacao = false;
            Domain.ServicoContato.Entities.ServicoContato servico = new Domain.ServicoContato.Entities.ServicoContato();
            servico.Id = servicoVM.Id;

            servico.Servico = new Domain.Servico.Entities.Servico
            {
                Id = servicoVM.ServicoId
            };

            servico.Nome = servicoVM.Nome;
            servico.Contato = servicoVM.Contato;
            servico.Email = servicoVM.Email;
            servico.Mensagem = servicoVM.Mensagem;
            servico.StatusContato = (Domain.ServicoContato.Entities.ServicoContato.TpStatusContato)servicoVM.StatusContato;
            servico.EntrouContato = servicoVM.EntrouContato;
            servico.DataContato = servicoVM.DataContato;
            servico.DataRetorno = servicoVM.DataRetorno;
            servico.ObservacaoInterna = servicoVM.ObservacaoInterna;
            servico.Origem = servicoVM.Origem;
            servico.VirouPaciente = servicoVM.VirouPaciente;
            servico.Prioridade = (Domain.ServicoContato.Entities.ServicoContato.TpPrioridade)servicoVM.Prioridade;
            servico.PreferenciaContato = (Domain.ServicoContato.Entities.ServicoContato.TpPreferenciaContato)servicoVM.PreferenciaContato;
            servico.IP = requisicao[0];
            servico.UserAgent = requisicao[1];

            if (!string.IsNullOrEmpty(servico.Nome))
            {
                servico.Nome = servico.Nome.ToUpper();
            }

            operacao = servico.Validar();

            if (!operacao)
            {
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
            RegistrarLog(servicoVM.Id, requisicao, dadosExistente, "ServicoContato");

            return (true, servico.ValidationResult);
        }

        public (bool, ValidationResult) SalvarSaibaMais(ServicoContatoViewModel servicoVM, string[] requisicao)
        {
            bool operacao = false;
            Domain.ServicoContato.Entities.ServicoContato servico = new Domain.ServicoContato.Entities.ServicoContato();
            servico.Id = servicoVM.Id;

            servico.Servico = new Domain.Servico.Entities.Servico
            {
                Id = servicoVM.ServicoId
            };

            servico.Nome = servicoVM.Nome;
            servico.Contato = servicoVM.Contato;
            servico.Email = servicoVM.Email;
            servico.Mensagem = servicoVM.Mensagem;

            servico.StatusContato = 0;

            servico.EntrouContato = false;
            servico.DataContato = null;
            servico.DataRetorno = null;
            servico.ObservacaoInterna = string.Empty;
            servico.Origem = "Site";
            servico.VirouPaciente = false;

            servico.Prioridade = 0;
            servico.PreferenciaContato = 0;

            servico.IP = requisicao[0];
            servico.UserAgent = requisicao[1];

            if (!string.IsNullOrEmpty(servico.Nome))
            {
                servico.Nome = servico.Nome.ToUpper();
            }

            operacao = servico.Validar();

            if (!operacao)
            {
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

            return (true, servico.ValidationResult);
        }

        public IEnumerable<ServicoContatoConsultaViewModel> Consultar(string nome, PaginacaoDados paginacao)
        {
            List<ServicoContatoConsultaViewModel> retorno = new List<ServicoContatoConsultaViewModel>();
            var servicos = _servicoService.Consultar(nome, paginacao);

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

        //public IEnumerable<ServicoViewModel> ObterTodos()
        //{
        //    List<ServicoViewModel> retorno = new List<ServicoViewModel>();
        //    var servicos = _servicoService.ObterTodos();
        //    foreach (var servico in servicos)
        //    {
        //        retorno.Add(FormatarRetornoConsulta(servico));
        //    }
        //    return retorno;
        //}
        public ServicoContatoConsultaViewModel Obter(int servicoId)
        {
            ServicoContatoConsultaViewModel retorno = new ServicoContatoConsultaViewModel();
            retorno = FormatarRetornoConsulta(_servicoService.Obter(servicoId));
            return retorno;
        }

        //public bool Excluir(int servicoId, string[] requisicao)
        //{
        //    bool operacao = false;
        //    var dadosExistente = _servicoService.Obter(servicoId);
        //    operacao = _servicoService.Excluir(servicoId);

        //    RegistrarLog(servicoId, requisicao, dadosExistente, "Servico");

        //    return operacao;
        //}
        //public IEnumerable<ServicoViewModel> ObterDestaquesHome(int limite)
        //{
        //    List<ServicoViewModel> retorno = new List<ServicoViewModel>();
        //    var servicos = _servicoService.ObterDestaquesHome(limite);
        //    foreach (var servico in servicos)
        //    {
        //        retorno.Add(FormatarRetornoConsulta(servico));
        //    }
        //    return retorno;
        //}
        //public ServicoViewModel ObterPorUrl(string blogUrl)
        //{
        //    ServicoViewModel retorno = new ServicoViewModel();
        //    retorno = FormatarRetornoConsulta(_servicoService.ObterPorUrl(blogUrl));
        //    return retorno;
        //}
        internal ServicoContatoConsultaViewModel FormatarRetornoConsulta(Domain.ServicoContato.Entities.ServicoContato servico)
        {
            var servicoViewModel = new ServicoContatoConsultaViewModel
            {
                Id = servico.Id,
                ServicoId = servico.Servico.Id,
                ServicoNome = servico.Servico.Nome,
                Nome = servico.Nome,
                Contato = servico.Contato,
                Email = servico.Email,
                Mensagem = servico.Mensagem,

                StatusContato = (Domain.ServicoContato.Entities.ServicoContato.TpStatusContato)servico.StatusContato,
                EntrouContato = servico.EntrouContato,
                DataContato = servico.DataContato == null ? null : servico.DataContato,
                DataRetorno = servico.DataRetorno == null ? null : servico.DataRetorno,

                ObservacaoInterna = servico.ObservacaoInterna,
                Origem = servico.Origem,

                VirouPaciente = servico.VirouPaciente,

                Prioridade = (Domain.ServicoContato.Entities.ServicoContato.TpPrioridade)servico.Prioridade,
                PreferenciaContato = (Domain.ServicoContato.Entities.ServicoContato.TpPreferenciaContato)servico.PreferenciaContato,

                IP = servico.IP,
                UserAgent = servico.UserAgent,
                DataCriacao = servico.DataCriacao,
                DataAtualizacao = servico.DataAtualizacao
            };

            return servicoViewModel;
        }

        public object ObterParametros()
        {
            return new
            {
                StatusContato = ObterEnum(typeof(TpStatusContato)),
                PreferenciaContato = ObterEnum(typeof(TpPreferenciaContato)),
                Prioridade = ObterEnum(typeof(TpPrioridade))
            };
        }

        private List<object> ObterEnum(Type enumType)
        {
            return Enum.GetValues(enumType)
                .Cast<Enum>()
                .Select(e => new
                {
                    Valor = Convert.ToInt32(e),
                    Nome = e.ToString(),
                    Descricao = ObterDescricaoEnum(e),
                    Tipo = enumType.Name
                })
                .Cast<object>()
                .ToList();
        }

        private string ObterDescricaoEnum(Enum valor)
        {
            FieldInfo field = valor.GetType().GetField(valor.ToString());

            DescriptionAttribute attribute =
                field.GetCustomAttribute<DescriptionAttribute>();

            return attribute != null
                ? attribute.Description
                : valor.ToString();
        }

        ////Funções Private
        //private bool EhBase64(string s)
        //{
        //    Span<byte> buffer = new Span<byte>(new byte[s.Length]);
        //    return Convert.TryFromBase64String(s, buffer, out _);
        //}
        //private string MontarImagem(byte[] bytes)
        //{
        //    if (bytes == null || bytes.Length == 0)
        //        return null;

        //    var texto = System.Text.Encoding.UTF8.GetString(bytes);

        //    // 🔥 CASO 1: já é data:image
        //    if (texto.StartsWith("data:image"))
        //        return texto;

        //    // 🔥 CASO 2: é base64 puro (sem prefixo)
        //    if (EhBase64(texto))
        //        return $"data:image/jpeg;base64,{texto}";

        //    // 🔥 CASO 3: é imagem binária real
        //    var mime = ObterMimeType(bytes);
        //    return $"data:{mime};base64,{Convert.ToBase64String(bytes)}";
        //}
        //private string ObterMimeType(byte[] bytes)
        //{
        //    if (bytes == null || bytes.Length < 4)
        //        return "image/jpeg";

        //    // JPEG
        //    if (bytes[0] == 0xFF && bytes[1] == 0xD8)
        //        return "image/jpeg";

        //    // PNG
        //    if (bytes[0] == 0x89 && bytes[1] == 0x50)
        //        return "image/png";

        //    // GIF
        //    if (bytes[0] == 0x47 && bytes[1] == 0x49)
        //        return "image/gif";

        //    // WEBP
        //    if (bytes[0] == 0x52 && bytes[1] == 0x49)
        //        return "image/webp";

        //    return "image/jpeg"; // fallback
        //}
        //private string MontarUrl(string titulo)
        //{
        //    string url = titulo.ToLower().Trim();
        //    url = url.Replace(" ", "-");
        //    url = url.Replace(".", "");
        //    url = url.Replace(",", "");
        //    url = url.Replace(";", "");
        //    url = url.Replace(":", "");
        //    url = url.Replace("?", "");
        //    url = url.Replace("!", "");
        //    return url;
        //}
        //private string ObterResumo(string conteudo)
        //{
        //    if (string.IsNullOrWhiteSpace(conteudo))
        //        return string.Empty;

        //    // Remove tags HTML
        //    string textoLimpo = Regex.Replace(conteudo, "<.*?>", string.Empty);

        //    // Remove espaços extras
        //    textoLimpo = textoLimpo.Trim();

        //    int maxLength = 100;

        //    if (textoLimpo.Length <= maxLength)
        //        return textoLimpo;

        //    return textoLimpo.Substring(0, maxLength) + "...";
        //}
        private void RegistrarLog(int servicoId, string[] requisicao, Domain.ServicoContato.Entities.ServicoContato dadosExistente, string nomeClasse)
        {
            var dadosAtualizado = _servicoService.Obter(servicoId);
            var (retorno, dadosAlterados) = _logAplicacaoService.ObterDiferencas(dadosExistente, dadosAtualizado);

            if (dadosAlterados.Any())
            {
                var log = _logAplicacaoService.Criar(
                    requisicao: requisicao,
                    entidade: nomeClasse,
                    entidadeId: servicoId,
                    operacao: retorno,
                    dadosAntes: dadosExistente,
                    dadosDepois: dadosAtualizado,
                    dadosAlterados: dadosAlterados,
                    aplicacao: MethodBase.GetCurrentMethod()?.DeclaringType?.Name,
                    metodo: MethodBase.GetCurrentMethod()?.Name
                );
                _logAplicacaoService.Salvar(log);
            }
        }

        public void Dispose()
        {
        }
    }
}