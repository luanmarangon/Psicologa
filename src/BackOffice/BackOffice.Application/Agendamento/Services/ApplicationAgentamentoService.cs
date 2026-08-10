using Psicologa.Application.Agendamento.ViewsModel;
using Psicologa.Application.ProntuarioSessao.Services;
using Psicologa.Application.ProntuarioSessao.ViewsModel;
using Psicologa.Domain.Agendamento.Entities;
using Shared.Infra.CrossCutting;
using Shared.Infra.CrossCutting.ValidationResult;
using System;
using System.Collections.Generic;
using System.Linq;
using static Shared.Infra.CrossCutting.PaginacaoDados;

namespace Psicologa.Application.Agendamento.Services
{
    public class ApplicationAgentamentoService : IDisposable
    {
        private readonly Domain.LogAplicacao.Services.LogAplicacaoService _logAplicacaoService;
        private readonly Domain.Agendamento.Services.AgendamentoService _servicoAgendamento;
        private readonly Domain.Configuracao.Services.ConfiguracaoService _configuracaoService;
        private readonly Domain.Prontuario.Services.ProntuarioService _prontuarioService;
        private readonly Domain.ProntuarioSessao.Services.ProntuarioSessaoService _prontuarioSessaoService;

        //private readonly ApplicationProntuarioSessaoService _appProntuarioSessaoService;
        private readonly IAppSettings _appSettings;

        public ApplicationAgentamentoService(Domain.LogAplicacao.Services.LogAplicacaoService logAplicacaoService,
            Domain.Agendamento.Services.AgendamentoService servicoAgendamento,
            Domain.Configuracao.Services.ConfiguracaoService configuracaoService,
                Domain.Prontuario.Services.ProntuarioService prontuarioService,
                Domain.ProntuarioSessao.Services.ProntuarioSessaoService prontuarioSessaoService,
            //ApplicationProntuarioSessaoService appProntuarioSessaoService,
            IAppSettings appSettings)
        {
            _logAplicacaoService = logAplicacaoService;
            _servicoAgendamento = servicoAgendamento;
            _configuracaoService = configuracaoService;
            _prontuarioService = prontuarioService;
            _prontuarioSessaoService = prontuarioSessaoService;
            //_appProntuarioSessaoService = appProntuarioSessaoService;
            _appSettings = appSettings;
        }

        public (bool, ValidationResult) Salvar(AgendamentoViewModel dados, string[] requisicao)
        {
            var dadosExistente = _servicoAgendamento.ObterPorId(dados.Id);

            ValidationResult vr = new ValidationResult();
            bool operacao = false;

            Domain.Agendamento.Entities.Agendamento agendamento = new Domain.Agendamento.Entities.Agendamento
            {
                Id = dados.Id,
                Paciente = new Domain.Pessoa.Entities.Pessoa
                {
                    Id = dados.PacienteId
                },
                Psicologo = new Domain.Pessoa.Entities.Pessoa
                {
                    Id = dados.PsicologoId
                },
                Servico = new Domain.Servico.Entities.Servico
                {
                    Id = dados.ServicoId
                },
                DataConsulta = dados.DataConsulta,
                HoraInicio = TimeSpan.Parse(dados.HoraInicio).ToString(@"hh\:mm"),
                HoraFim = (TimeSpan.Parse(dados.HoraInicio) + TimeSpan.FromMinutes(dados.TempoSessao)).ToString(@"hh\:mm"),
                TempoSessao = dados.TempoSessao,

                Online = dados.Online,
                Presencial = !dados.Online, //dados.Presencial, //Não estou usando o Presencial pois se o Online for verdadeiro o Presencial é falso, se o Online for falso o Presencial é verdadeiro

                StatusAgendamento = (Domain.Agendamento.Entities.Agendamento.TpStatusAgendamento)dados.StatusAgendamento,
                TipoAgendamento = (Domain.Agendamento.Entities.Agendamento.TpTipoAgendamento)dados.TipoAgendamento,
                Ativo = dados.Ativo,
                ConfirmouAgendamento = dados.ConfirmouAgendamento,
                DataConfirmacao = dados.ConfirmouAgendamento ? dados.DataConfirmacao : (DateTime?)null,
            };

            if (agendamento.Validar())
            {
                operacao = _servicoAgendamento.Salvar(agendamento);

                //Criar a Sessão
                if (operacao)
                {
                    dados.Id = agendamento.Id;

                    // Obtém a sessão vinculada ao agendamento ou cria uma nova
                    var prontSessao = _prontuarioSessaoService.ObterPorAgendamento(agendamento.Id)
                                      ?? new Domain.ProntuarioSessao.Entities.ProntuarioSessao();

                    var prontuario = _prontuarioService.ObterProntuarioPorPacienteId(agendamento.Paciente.Id);

                    if (prontuario == null)
                    {
                        throw new Exception("Não foi encontrado um prontuário para o paciente.");
                    }

                    if (prontSessao.Prontuario == null || prontSessao.Prontuario.Id == 0)
                    {
                        prontSessao.Prontuario = new Domain.Prontuario.Entities.Prontuario
                        {
                            Id = prontuario.Id
                        };
                    }

                    prontSessao.Agendamento = new Domain.Agendamento.Entities.Agendamento
                    {
                        Id = agendamento.Id
                    };

                    prontSessao.Psicologa = new Domain.Pessoa.Entities.Pessoa
                    {
                        Id = agendamento.Psicologo.Id
                    };

                    prontSessao.DataSessao = agendamento.DataConsulta;
                    prontSessao.HoraInicio = TimeSpan.Parse(agendamento.HoraInicio);
                    prontSessao.HoraFim = TimeSpan.Parse(agendamento.HoraFim);
                    prontSessao.TipoAtendimento = (Domain.Agendamento.Entities.Agendamento.tpFiltro)(agendamento.Online ? 2 : 1);

                    var operacaoSessao = _prontuarioSessaoService.EvoluirSessao(prontSessao);
                    operacao = operacao && operacaoSessao;

                    if (operacao)
                        _logAplicacaoService.Registrar(prontSessao.Id, requisicao, prontSessao, null, "ProntuarioSessao", "ApplicationAgentamentoService", "Salvar");
                }
            }

            if (operacao)
            {
                _logAplicacaoService.Registrar(dados.Id, requisicao, dadosExistente, agendamento, "Agendamento", "ApplicationAgentamentoService", "Salvar");
            }
            return (operacao, vr);
        }

        public (bool, ValidationResult, ResultadoRecorrenciaViewModel) SalvarRecorrente(AgendamentoViewModel dados, string[] requisicao)
        {
            var resultado = new ResultadoRecorrenciaViewModel();
            var vrGeral = new ValidationResult();

            var datas = GerarDatasRecorrencia(
                dados.DataConsulta,
                dados.TipoRecorrencia,
                dados.QuantidadeOcorrencias,
                dados.DataFimRecorrencia
            );

            if (!datas.Any())
            {
                vrGeral.AddUserMessageError("Não foi possível gerar nenhuma data para a recorrência informada.");
                return (false, vrGeral, resultado);
            }

            foreach (var data in datas)
            {
                // Monta um ViewModel novo por ocorrência, reaproveitando os dados do "molde"
                var itemDados = new AgendamentoViewModel
                {
                    Id = 0, // sempre novo — nunca reaproveita Id de outra ocorrência
                    PacienteId = dados.PacienteId,
                    PacienteNome = dados.PacienteNome,
                    PsicologoId = dados.PsicologoId,
                    PsicologoNome = dados.PsicologoNome,
                    ServicoId = dados.ServicoId,
                    ServicoNome = dados.ServicoNome,
                    DataConsulta = data,
                    HoraInicio = dados.HoraInicio,
                    TempoSessao = dados.TempoSessao,
                    Online = dados.Online,
                    Presencial = dados.Presencial,
                    StatusAgendamento = dados.StatusAgendamento,
                    TipoAgendamento = dados.TipoAgendamento,
                    Ativo = dados.Ativo,
                    ConfirmouAgendamento = false,   // ocorrências futuras nascem não confirmadas
                    DataConfirmacao = null,
                };

                var (sucesso, vrItem) = Salvar(itemDados, requisicao);

                //if (!datas.Any())
                //{
                //    vrGeral.AddUserMessageError("Recorrencia", "Não foi possível gerar nenhuma data para a recorrência informada.");
                //    return (false, vrGeral, resultado);
                //}
                if (sucesso)
                {
                    resultado.TotalCriados++;
                    resultado.IdsGerados.Add(itemDados.Id);
                }
                else
                {
                    resultado.Conflitos.Add($"{data:dd/MM/yyyy}");
                    // TODO: quando tivermos como ler as mensagens do vrItem, anexar aqui
                }
                //if (sucesso)
                //{
                //    resultado.TotalCriados++;
                //    resultado.IdsGerados.Add(itemDados.Id);
                //}
                //else
                //{
                //    var erros = vrItem?.AddUserMessageError != null && vrItem.AddUserMessageError.Any()
                //        ? string.Join("; ", vrItem.AddUserMessageError)
                //        : "erro ao salvar";
                //    resultado.Conflitos.Add($"{data:dd/MM/yyyy} ({erros})");
                //}
            }

            bool operacaoGeral = resultado.TotalCriados > 0;
            return (operacaoGeral, vrGeral, resultado);
        }

        private List<DateTime> GerarDatasRecorrencia(DateTime dataInicial, string tipo, int? quantidade, DateTime? dataFim)
        {
            var datas = new List<DateTime>();
            var atual = dataInicial;

            Func<DateTime, DateTime> proximaData = tipo switch
            {
                "semanal" => d => d.AddDays(7),
                "quinzenal" => d => d.AddDays(15),
                "mensal" => d => d.AddMonths(1),
                _ => d => d.AddDays(7)
            };

            bool temDataFimValida = dataFim.HasValue && dataFim.Value > DateTime.MinValue;

            if (temDataFimValida)
            {
                while (atual <= dataFim.Value)
                {
                    datas.Add(atual);
                    atual = proximaData(atual);
                }
            }
            else
            {
                int qtd = quantidade ?? 1;
                for (int i = 0; i < qtd; i++)
                {
                    datas.Add(atual);
                    atual = proximaData(atual);
                }
            }

            return datas;
        }

        public AgendamentoDisponibilidadeViewModel ObterDisponibilidade(int psicologoId, DateTime dataConsulta)
        {
            AgendamentoDisponibilidadeViewModel disponibilidade =
                new AgendamentoDisponibilidadeViewModel();

            disponibilidade.PsicologaId = psicologoId;
            disponibilidade.DataConsulta = dataConsulta;
            disponibilidade.HorariosDisponiveis =
                new List<AgendamentoHorariosDisponiveisViewModel>();

            var horariosAgendados =
                _servicoAgendamento.ObterDisponibilidade(psicologoId, dataConsulta);

            var configuracao =
                _configuracaoService.ObterFuncionamento();

            int diaSemana = (int)dataConsulta.DayOfWeek + 1;

            if (diaSemana == 8)
                diaSemana = 1;

            var funcionamentoDia =
                configuracao.Funcionamento
                    .FirstOrDefault(x =>
                        x.DiaSemana == diaSemana &&
                        x.Ativo);

            if (funcionamentoDia == null)
                return disponibilidade;

            // horários já ocupados
            var ocupados =
                horariosAgendados?
                    .SelectMany(x => x.HorariosAgendados)
                    .OrderBy(x => x.HoraInicio)
                    .ToList()
                ?? new List<HorarioAgendado>();

            foreach (var periodo in funcionamentoDia.Periodos)
            {
                TimeSpan inicioPeriodo =
                    TimeSpan.Parse(periodo.HoraInicio);

                TimeSpan fimPeriodo =
                    TimeSpan.Parse(periodo.HoraFim);

                // gera slots de 1h
                for (
                    TimeSpan horaAtual = inicioPeriodo;
                    horaAtual < fimPeriodo;
                    //horaAtual = horaAtual.Add(TimeSpan.FromHours(1))
                    horaAtual = horaAtual.Add(TimeSpan.FromMinutes(30))
                )
                {
                    TimeSpan proximaHora =
                        //horaAtual.Add(TimeSpan.FromHours(1));
                        horaAtual.Add(TimeSpan.FromMinutes(30));

                    // não ultrapassa o horário final
                    if (proximaHora > fimPeriodo)
                        break;

                    bool ocupado =
                        ocupados.Any(x =>
                        {
                            TimeSpan inicioOcupado =
                                TimeSpan.Parse(x.HoraInicio);

                            TimeSpan fimOcupado =
                                TimeSpan.Parse(x.HoraFim);

                            return horaAtual < fimOcupado &&
                                   proximaHora > inicioOcupado;
                        });

                    // somente horários livres
                    if (!ocupado)
                    {
                        disponibilidade.HorariosDisponiveis.Add(
                            new AgendamentoHorariosDisponiveisViewModel
                            {
                                HoraInicio =
                                    horaAtual.ToString(@"hh\:mm"),

                                HoraFim =
                                    proximaHora.ToString(@"hh\:mm")
                            });
                    }
                }
            }

            return disponibilidade;
        }

        public IEnumerable<AgendamentoConsultaViewModel> Consultar(string nome, Domain.Agendamento.Entities.Agendamento.tpFiltro filtro, PaginacaoDados paginacao)
        {
            List<AgendamentoConsultaViewModel> retorno = new List<AgendamentoConsultaViewModel>();
            var servicos = _servicoAgendamento.Consultar(nome, filtro, paginacao);

            foreach (var serv in servicos)
            {
                retorno.Add(FormatarRetornoConsulta(serv));
            }

            paginacao.OrdenacaoNome = Utils.ObterDescricaoEnum(paginacao.Ordenacao);
            if (paginacao.Ordenacao == TpOrdenacao.Nome)
            {
                retorno = retorno.OrderBy(o => o.DataConsulta).ToList();
            }

            return retorno;
        }

        public AgendamentoConsultaViewModel ObterPorId(int id)
        {
            var agendamento = _servicoAgendamento.ObterPorId(id);

            return FormatarRetornoConsulta(agendamento);
        }

        public AgendamentoConsultaViewModel ObterAgendamentoPorPaciente(int prontuarioId, int psicologoId, DateTime data)
        {
            var paciente = _prontuarioService.Obter(prontuarioId);
            var agendamento = _servicoAgendamento.ObterAgendamentoPorPaciente(paciente.Paciente.Id, psicologoId, data);
            return FormatarRetornoConsulta(agendamento);
        }

        public bool AtualizarStatusAgendamento(int agendamentoId, int agendamentoStatusId, string[] requisicao)
        {
            bool operacao = false;
            var dadosExistente = _servicoAgendamento.ObterPorId(agendamentoId);
            operacao = _servicoAgendamento.AtualizarStatusAgendamento(agendamentoId, agendamentoStatusId);
            if (operacao)
            {
                var dadosAtualizado = _servicoAgendamento.ObterPorId(agendamentoId);
                _logAplicacaoService.Registrar(agendamentoId, requisicao, dadosExistente, dadosAtualizado, "Agendamento", "ApplicationAgentamentoService", "AtualizarStatusAgendamento");
            }
            return operacao;
        }
        //public bool AtualizarStatusAgendamento(int agendamentoId, int agendamentoStatusId) 
        //{
        //    bool operacao = false;
        //    operacao = _servicoAgendamento.AtualizarStatusAgendamento(agendamentoId, agendamentoStatusId);

        //    if(operacao)


        //    return operacao;
        //}

        public bool Excluir(int agendamentoId, string[] requisicao)
        {
            bool operacao = false;
            var dadosExistente = _servicoAgendamento.ObterPorId(agendamentoId);
            operacao = _servicoAgendamento.Excluir(agendamentoId);

            //Excluir Sessão
            var prontSessao = _prontuarioSessaoService.ObterPorAgendamento(agendamentoId);

            operacao = _prontuarioSessaoService.ExcluirSessao(prontSessao.Id);

            if (operacao)
            {
                _logAplicacaoService.Registrar(agendamentoId, requisicao, dadosExistente, null, "Agendamento", "ApplicationAgentamentoService", "Excluir");
                _logAplicacaoService.Registrar(prontSessao.Id, requisicao, prontSessao, null, "ProntuarioSessao", "ApplicationAgentamentoService", "Excluir");
            }

            return operacao;
        }

        internal AgendamentoConsultaViewModel FormatarRetornoConsulta(Domain.Agendamento.Entities.Agendamento agendamento)
        {
            if (agendamento == null)
                return null;

            return new AgendamentoConsultaViewModel
            {
                Id = agendamento.Id,
                PacienteId = agendamento.Paciente.Id,
                PacienteNome = agendamento.Paciente.Nome,

                PsicologoId = agendamento.Psicologo.Id,
                PsicologoNome = agendamento.Psicologo.Nome,

                ServicoId = agendamento.Servico.Id,
                ServicoNome = agendamento.Servico.Nome,

                DataConsulta = agendamento.DataConsulta,
                HoraInicio = TimeSpan.Parse(agendamento.HoraInicio).ToString(@"hh\:mm"),
                HoraFim = TimeSpan.Parse(agendamento.HoraFim).ToString(@"hh\:mm"),
                TempoSessao = agendamento.TempoSessao,
                Online = agendamento.Online,
                Presencial = agendamento.Presencial,
                DataCriacao = agendamento.DataCriacao,
                DataAtualizacao = agendamento.DataAtualizacao,
                Ativo = agendamento.Ativo,

                ConfirmouAgendamento = agendamento.ConfirmouAgendamento,
                DataConfirmacao = agendamento.DataConfirmacao != null ? agendamento.DataConfirmacao : (DateTime?)null,

                StatusAgendamentoDescricao = agendamento.StatusAgendamento.ToString(),
                TipoAgendamentoDescricao = agendamento.TipoAgendamento.ToString(),

                StatusAgendamento = (int)agendamento.StatusAgendamento,
                TipoAgendamento = (int)agendamento.TipoAgendamento
            };
        }

        public void Dispose()
        {
        }
    }
}