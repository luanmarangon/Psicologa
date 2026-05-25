import React, { Component } from 'react';
import LoadingIndicator from '../../../components/LoadingIndicator';

export default class Cadastro extends Component {

    constructor(props) {
        super(props);

        this.state = {
            aguarde: false,
            aguardeSalvar: false,
            aguardeExcluir: false,
            trocarPaciente: false,
            trocarPsicologo: false,
            servicos: [],
            dados: {
                id: 0,
                pacienteId: '',
                pacienteNome: '',
                psicologoId: '',
                psicologoNome: '',
                servicoId: '',
                dataConsulta: '',
                horaInicio: '',
                tempoSessao: 50,
                online: false,
                ativo: true,
                statusAgendamento: '',
                tipoAgendamento: '',
                confirmouAgendamento: false,
                dataConfirmacao: '',
            },
            horariosDisponiveis: []
        };
    }

    // ─── Lifecycle ───────────────────────────────────────────────────────────

    componentDidMount = () => {
        $('[data-toggle="popover"]').popover();

        const promises = [this.obterServicos()];
        if (this.props.idEdicao) {
            promises.push(this.obter(this.props.idEdicao));
        }

        Promise.all(promises).then(() => {
            this.inicializarSelect2Paciente();
            this.inicializarSelect2Psicologo();
        });
    }

    componentDidUpdate = (prevProps, prevState) => {
        const { dados } = this.state;

        // Reinicializa quando paciente é limpo
        if (prevState.dados.pacienteId && !dados.pacienteId) {
            setTimeout(() => this.inicializarSelect2Paciente(), 0);
        }

        // Reinicializa quando psicólogo é limpo
        if (prevState.dados.psicologoId && !dados.psicologoId) {
            setTimeout(() => this.inicializarSelect2Psicologo(), 0);
        }
    }

    // ─── Select2 Paciente ────────────────────────────────────────────────────

    inicializarSelect2Paciente = () => {
        if (!$("#selPaciente").length) return;

        if ($("#selPaciente").hasClass("select2-hidden-accessible")) {
            $("#selPaciente").select2("destroy");
        }

        $("#selPaciente").select2({
            language: "pt-BR",
            placeholder: "Digite para buscar o paciente...",
            minimumInputLength: 2,
            ajax: {
                url: (params) =>
                    resolveClientURL("Administrativo/Pessoa/ConsultarClienteAutoComplete?q=" + encodeURIComponent(params.term)),
                dataType: 'json',
                delay: 300,
                processResults: (data) => ({
                    results: data.map(item => ({
                        id: item.dados.id,
                        text: item.dados.nome.toUpperCase(),
                        nome: item.dados.nome.toUpperCase(),
                        cidade: item.dados.cidade,
                        docIdNro: item.dados.docIdNro,
                        docIdTipoNome: item.dados.docIdTipoNome,
                    }))
                }),
            },
            templateResult: (data) => {
                if (!data.id) return data.text;
                return $(
                    '<div>' +
                    '<div><strong>' + data.nome + '</strong></div>' +
                    '<div class="small text-muted">' + (data.docIdNro || '') + ' (' + (data.docIdTipoNome || '') + ')</div>' +
                    '<div class="small text-muted">' + (data.cidade || '') + '</div>' +
                    '</div>'
                );
            },
            templateSelection: (data) => data.nome || data.text,
        });

        $("#selPaciente").off("select2:select").on("select2:select", (e) => {
            const item = e.params.data;
            this.setState(prev => ({
                trocarPaciente: false,
                dados: {
                    ...prev.dados,
                    pacienteId: item.id,
                    pacienteNome: item.nome,
                }
            }), () => {
                // Esconde o container do Select2 e mostra o input estático
                $("#selPaciente").next(".select2-container").hide();
            });
        });
    }

    trocarPacienteHandler = () => {
        // Mostra o Select2 novamente e limpa o valor
        $("#selPaciente").val(null).trigger("change");
        $("#selPaciente").next(".select2-container").show();

        this.setState(prev => ({
            trocarPaciente: true,
            dados: {
                ...prev.dados,
                pacienteId: '',
                pacienteNome: '',
            }
        }));
    }

    // ─── Select2 Psicólogo ───────────────────────────────────────────────────

    inicializarSelect2Psicologo = () => {
        if (!$("#selPsicologo").length) return;

        if ($("#selPsicologo").hasClass("select2-hidden-accessible")) {
            $("#selPsicologo").select2("destroy");
        }

        $("#selPsicologo").select2({
            language: "pt-BR",
            placeholder: "Digite para buscar o psicólogo...",
            minimumInputLength: 2,
            ajax: {
                url: (params) =>
                    resolveClientURL("Administrativo/Pessoa/ConsultarPsicologoAutoComplete?q=" + encodeURIComponent(params.term)),
                dataType: 'json',
                delay: 300,
                processResults: (data) => ({
                    results: data.map(item => ({
                        id: item.dados.id,
                        text: item.dados.nome.toUpperCase(),
                        nome: item.dados.nome.toUpperCase(),
                        crp: item.dados.crp || '',
                        especialidade: item.dados.especialidade || '',
                    }))
                }),
            },
            templateResult: (data) => {
                if (!data.id) return data.text;
                return $(
                    '<div>' +
                    '<div><strong>' + data.nome + '</strong></div>' +
                    (data.crp ? '<div class="small text-muted">CRP: ' + data.crp + '</div>' : '') +
                    (data.especialidade ? '<div class="small text-muted">' + data.especialidade + '</div>' : '') +
                    '</div>'
                );
            },
            templateSelection: (data) => data.nome || data.text,
        });

        $("#selPsicologo").off("select2:select").on("select2:select", (e) => {
            const item = e.params.data;
            this.setState(prev => ({
                trocarPsicologo: false,
                dados: {
                    ...prev.dados,
                    psicologoId: item.id,
                    psicologoNome: item.nome,
                    dataConsulta: '',
                    horaInicio: '',
                }
            }), () => {
                // Esconde o container do Select2 e mostra o input estático
                $("#selPsicologo").next(".select2-container").hide();
            });
        });
    }

    trocarPsicologoHandler = () => {
        // Mostra o Select2 novamente e limpa o valor
        $("#selPsicologo").val(null).trigger("change");
        $("#selPsicologo").next(".select2-container").show();

        this.setState(prev => ({
            trocarPsicologo: true,
            horariosDisponiveis: [],
            dados: {
                ...prev.dados,
                psicologoId: '',
                psicologoNome: '',
                dataConsulta: '',
                horaInicio: '',
            }
        }));
    }

    // ─── HTTP ────────────────────────────────────────────────────────────────

    obterDisponibilidade = (psicologoId, dataConsulta) => {
        return HTTPClient.get(
            `Administrativo/Agendamento/ObterDisponibilidadeHorario?psicologoId=${psicologoId}&dataConsulta=${dataConsulta}`
        )
            .then(r => r.json())
            .then(r => {
                this.setState({
                    horariosDisponiveis: r.data.horariosDisponiveis || []
                });
            })
            .catch(() => {
                showToastr({ type: "error", text: "Um erro ocorreu ao obter disponibilidade." });
            });
    }

    obter = (id) => {
        this.setState({ aguarde: true });

        return HTTPClient.get(`Administrativo/Agendamento/Obter/${id}`, false)
            .then(r => r.json())
            .then(r => {
                if (!r.success) {
                    showToastr(r.messages);
                    return;
                }
                this.setState({ dados: { ...this.state.dados, ...r.data } });
                return this.obterDisponibilidade(r.data.psicologoId, r.data.dataConsulta);
            })
            .catch(() => {
                showToastr({ type: "error", text: "Erro ao obter o agendamento." });
            })
            .finally(() => {
                this.setState({ aguarde: false });
            });
    }

    obterServicos = () => {
        this.setState({ aguarde: true });

        return HTTPClient.get(`Administrativo/Servico/ObterTodosAtivos`, false)
            .then(r => r.json())
            .then(r => {
                if (!r.success) {
                    showToastr(r.messages);
                    return;
                }
                this.setState({ servicos: r.data });
            })
            .catch(() => {
                showToastr({ type: "error", text: "Erro ao obter os serviços." });
            })
            .finally(() => {
                this.setState({ aguarde: false });
            });
    }

    salvar = () => {
        const { dados: form } = this.state;
 console.log('payload:', form); // ← adicione isso
        if (!form.pacienteId) return showToastr({ type: "warning", text: "Selecione o paciente." });
        if (!form.psicologoId) return showToastr({ type: "warning", text: "Selecione o psicólogo." });
        if (!form.servicoId) return showToastr({ type: "warning", text: "Selecione o serviço." });
        if (!form.dataConsulta) return showToastr({ type: "warning", text: "Informe a data da consulta." });
        if (!form.horaInicio) return showToastr({ type: "warning", text: "Informe o horário da consulta." });
        if (!form.tempoSessao) return showToastr({ type: "warning", text: "Informe a duração da sessão." });

        this.setState({ aguardeSalvar: true });

        HTTPClient.post("Administrativo/Agendamento/Salvar", form, false)
            .then(r => r.json())
            .then(r => {
                if (!r.success) {
                    showToastr(r.messages);
                    return;
                }
                showToastr({ type: "success", text: "Agendamento salvo com sucesso." });
                this.props.onFechar(true);
            })
            .catch(() => {
                showToastr({ type: "error", text: "Erro ao salvar o agendamento." });
            })
            .finally(() => {
                this.setState({ aguardeSalvar: false });
            });
    }

    excluir = () => {
        if (!window.confirm("Deseja realmente excluir este agendamento?")) return;

        this.setState({ aguardeExcluir: true });

        HTTPClient.delete(`Administrativo/Agendamento/Excluir/${this.state.dados.id}`, false)
            .then(r => r.json())
            .then(r => {
                if (!r.success) {
                    showToastr(r.messages);
                    return;
                }
                showToastr({ type: "success", text: "Agendamento excluído com sucesso." });
                this.props.onFechar(true);
            })
            .catch(() => {
                showToastr({ type: "error", text: "Erro ao excluir o agendamento." });
            })
            .finally(() => {
                this.setState({ aguardeExcluir: false });
            });
    }

    handleChange = (campo, valor) => {
        this.setState(prev => ({
            dados: { ...prev.dados, [campo]: valor }
        }));
    }

    // ─── Render ──────────────────────────────────────────────────────────────

    render() {
        const {
            aguarde,
            aguardeSalvar,
            aguardeExcluir,
            trocarPaciente,
            trocarPsicologo,
            dados: form,
            servicos,
            horariosDisponiveis,
        } = this.state;

        const edicao = !!this.props.idEdicao;

        // Determina se deve mostrar o input estático (valor selecionado) ou o Select2
        const mostrarInputPaciente = !!(form.pacienteId && !trocarPaciente);
        const mostrarInputPsicologo = !!(form.psicologoId && !trocarPsicologo);

        return (
            <div className="modal fade show d-block" tabIndex="-1" style={{ backgroundColor: 'rgba(0,0,0,0.5)' }}>
                <div className="modal-dialog modal-lg">
                    <div className="modal-content">

                        {/* Header */}
                        <div className="modal-header">
                            <h5 className="modal-title">
                                <i className="fas fa-calendar-alt mr-2"></i>
                                {edicao ? "Editar Agendamento" : "Novo Agendamento"}
                            </h5>
                            <button type="button" className="close" onClick={() => this.props.onFechar(false)}>
                                <span>&times;</span>
                            </button>
                        </div>

                        {/* Body */}
                        <div className="modal-body">
                            {aguarde ? (
                                <LoadingIndicator />
                            ) : (
                                <div className="row">

                                    {/* PACIENTE */}
                                    <div className="col-md-12 form-group">
                                        <label htmlFor="selPaciente">
                                            Paciente <span className="text-danger">*</span>
                                        </label>

                                        {/* Input estático — visível quando há valor selecionado */}
                                        {mostrarInputPaciente && (
                                            <div className="input-group">
                                                <input
                                                    type="text"
                                                    className="form-control"
                                                    value={form.pacienteNome || ''}
                                                    disabled
                                                />
                                                {!edicao && (
                                                    <div className="input-group-append">
                                                        <button
                                                            type="button"
                                                            className="btn btn-outline-secondary"
                                                            onClick={this.trocarPacienteHandler}
                                                        >
                                                            <i className="fas fa-exchange-alt mr-1"></i>
                                                            Trocar
                                                        </button>
                                                    </div>
                                                )}
                                            </div>
                                        )}

                                        {/* Select2 — sempre no DOM, visível/oculto via display */}
                                        <div style={{ display: mostrarInputPaciente ? 'none' : 'block' }}>
                                            <select
                                                id="selPaciente"
                                                className="form-control"
                                                style={{ width: "100%" }}
                                            />
                                        </div>
                                    </div>

                                    {/* PSICÓLOGO */}
                                    <div className="col-md-12 form-group">
                                        <label htmlFor="selPsicologo">
                                            Psicólogo <span className="text-danger">*</span>
                                        </label>

                                        {/* Input estático — visível quando há valor selecionado */}
                                        {mostrarInputPsicologo && (
                                            <div className="input-group">
                                                <input
                                                    type="text"
                                                    className="form-control"
                                                    value={form.psicologoNome || ''}
                                                    disabled
                                                />
                                                <div className="input-group-append">
                                                    <button
                                                        type="button"
                                                        className="btn btn-outline-secondary"
                                                        onClick={this.trocarPsicologoHandler}
                                                    >
                                                        <i className="fas fa-exchange-alt mr-1"></i>
                                                        Trocar
                                                    </button>
                                                </div>
                                            </div>
                                        )}

                                        {/* Select2 — sempre no DOM, visível/oculto via display */}
                                        <div style={{ display: mostrarInputPsicologo ? 'none' : 'block' }}>
                                            <select
                                                id="selPsicologo"
                                                className="form-control"
                                                style={{ width: "100%" }}
                                            />
                                        </div>
                                    </div>

                                    {/* SERVIÇO */}
                                    <div className="col-md-12 form-group">
                                        <label htmlFor="selServico">
                                            Serviço <span className="text-danger">*</span>
                                        </label>
                                        <select
                                            id="selServico"
                                            className="form-control"
                                            value={form.servicoId || ''}
                                            onChange={(e) => {
                                                const servicoId = e.target.value;
                                                const servicoSelecionado = servicos.find(s => String(s.id) === String(servicoId));
                                                this.setState(prev => ({
                                                    dados: {
                                                        ...prev.dados,
                                                        servicoId,
                                                        tempoSessao: servicoSelecionado
                                                            ? servicoSelecionado.tempoSessaoMinutos
                                                            : prev.dados.tempoSessao,
                                                    }
                                                }));
                                            }}
                                        >
                                            <option value="">Selecione...</option>
                                            {servicos.map((s) => (
                                                <option key={s.id} value={s.id}>
                                                    {s.nome}
                                                </option>
                                            ))}
                                        </select>
                                    </div>

                                    {/* DATA */}
                                    <div className="col-md-4 form-group">
                                        <label>
                                            Data da Consulta <span className="text-danger">*</span>
                                        </label>
                                        <input
                                            type="date"
                                            className="form-control"
                                            value={form.dataConsulta ? form.dataConsulta.split('T')[0] : ''}
                                            disabled={!form.psicologoId}
                                            onChange={(e) => {
                                                const dataConsulta = e.target.value;
                                                this.setState(prev => ({
                                                    dados: { ...prev.dados, dataConsulta, horaInicio: '' }
                                                }), () => {
                                                    this.obterDisponibilidade(this.state.dados.psicologoId, dataConsulta);
                                                });
                                            }}
                                        />
                                    </div>

                                    {/* HORÁRIO */}
                                    <div className="col-md-4 form-group">
                                        <label>
                                            Horário <span className="text-danger">*</span>
                                        </label>
                                        <select
                                            className="form-control"
                                            value={form.horaInicio || ""}
                                            disabled={
                                                !form.dataConsulta ||
                                                (horariosDisponiveis.length === 0 && !form.horaInicio)
                                            }
                                            onChange={(e) => this.handleChange('horaInicio', e.target.value)}
                                        >
                                            <option value="">Selecione um horário</option>

                                            {/* Horário atual (edição) — quando não está na lista disponível */}
                                            {form.horaInicio && !horariosDisponiveis.some(h => h.horaInicio === form.horaInicio) && (
                                                <option value={form.horaInicio}>
                                                    {form.horaInicio} às {form.horaFim} (Atual)
                                                </option>
                                            )}

                                            {horariosDisponiveis.map((horario, index) => (
                                                <option key={index} value={horario.horaInicio}>
                                                    {horario.horaInicio} às {horario.horaFim}
                                                </option>
                                            ))}
                                        </select>

                                        {form.dataConsulta && horariosDisponiveis.length === 0 && !form.horaInicio && (
                                            <small className="text-danger">
                                                Nenhum horário disponível para esta data.
                                            </small>
                                        )}
                                    </div>

                                    {/* DURAÇÃO */}
                                    <div className="col-md-4 form-group">
                                        <label>
                                            Duração (min) <span className="text-danger">*</span>
                                        </label>
                                        <input
                                            type="number"
                                            className="form-control"
                                            value={form.tempoSessao}
                                            min={1}
                                            onChange={(e) => this.handleChange('tempoSessao', e.target.value)}
                                        />
                                    </div>

                                    {/* MODALIDADE */}
                                    <div className="col-md-6 form-group">
                                        <label>Modalidade</label>
                                        <div className="d-flex" style={{ gap: '16px' }}>
                                            <div className="custom-control custom-radio">
                                                <input
                                                    type="radio"
                                                    id="presencial"
                                                    className="custom-control-input"
                                                    checked={!form.online}
                                                    onChange={() => this.handleChange('online', false)}
                                                />
                                                <label className="custom-control-label" htmlFor="presencial">
                                                    <i className="fas fa-user mr-1"></i> Presencial
                                                </label>
                                            </div>
                                            <div className="custom-control custom-radio">
                                                <input
                                                    type="radio"
                                                    id="online"
                                                    className="custom-control-input"
                                                    checked={form.online}
                                                    onChange={() => this.handleChange('online', true)}
                                                />
                                                <label className="custom-control-label" htmlFor="online">
                                                    <i className="fas fa-video mr-1"></i> Online
                                                </label>
                                            </div>
                                        </div>
                                    </div>

                                    {/* STATUS */}
                                    <div className="col-md-6 form-group">
                                        <label>Status</label>
                                        <div className="custom-control custom-switch mt-1">
                                            <input
                                                type="checkbox"
                                                className="custom-control-input"
                                                id="ativo"
                                                checked={form.ativo}
                                                onChange={(e) => this.handleChange('ativo', e.target.checked)}
                                            />
                                            <label className="custom-control-label" htmlFor="ativo">
                                                {form.ativo ? "Ativo" : "Inativo"}
                                            </label>
                                        </div>
                                    </div>

                                    {/* STATUS AGENDAMENTO */}
                                    <div className="col-md-6 form-group">
                                        <label>Status do Agendamento</label>
                                        <select
                                            className="form-control"
                                            value={form.statusAgendamento || ""}
                                            onChange={(e) => this.handleChange('statusAgendamento', e.target.value)}
                                        >
                                            <option value="">Selecione...</option>
                                            <option value="1">Agendado</option>
                                            <option value="2">Cancelado</option>
                                            <option value="3">Realizado</option>
                                            <option value="4">Faltou</option>
                                            <option value="5">Remarcado</option>
                                        </select>
                                    </div>

                                    {/* TIPO AGENDAMENTO */}
                                    <div className="col-md-6 form-group">
                                        <label>Tipo de Agendamento</label>
                                        <select
                                            className="form-control"
                                            value={form.tipoAgendamento || ""}
                                            onChange={(e) => this.handleChange('tipoAgendamento', e.target.value)}
                                        >
                                            <option value="">Selecione...</option>
                                            <option value="1">Consulta</option>
                                            <option value="2">Retorno</option>
                                            <option value="3">Avaliação</option>
                                        </select>
                                    </div>

                                    {/* CONFIRMAÇÃO DO AGENDAMENTO */}

                                    {this.props.idEdicao && (
                                        <div className="col-md-6 form-group">
                                            <label>Confirmação do Agendamento</label>
                                            <div className="custom-control custom-switch mt-1">
                                                <input
                                                    type="checkbox"
                                                    className="custom-control-input"
                                                    id="confirmouAgendamento"
                                                    checked={form.confirmouAgendamento || false}
                                                    onChange={(e) => {
                                                        const confirmou = e.target.checked;
                                                        this.setState(prev => ({
                                                            dados: {
                                                                ...prev.dados,
                                                                confirmouAgendamento: confirmou,
                                                                dataConfirmacao: confirmou ?  new Date().toISOString() : null,
                                                            }
                                                        }));
                                                    }}
                                                />
                                                <label className="custom-control-label" htmlFor="confirmouAgendamento">
                                                    {form.confirmouAgendamento ? "Confirmado" : "Não Confirmado"}
                                                </label>
                                                {form.confirmouAgendamento && form.dataConfirmacao && (
                                                    <small className="text-muted d-block mt-1">
                                                        <i className="fas fa-check-circle text-success mr-1"></i>
                                                        Confirmado em {form.dataConfirmacao}
                                                    </small>
                                                )}
                                            </div>
                                        </div>
                                    )}



                                </div>
                            )}
                        </div>

                        {/* Footer */}
                        <div className="modal-footer">
                            {edicao && (
                                <button
                                    type="button"
                                    className="btn btn-danger mr-auto"
                                    onClick={this.excluir}
                                    disabled={aguardeExcluir}
                                >
                                    {aguardeExcluir
                                        ? <><i className="fas fa-spinner fa-spin mr-1"></i>Excluindo...</>
                                        : <><i className="far fa-trash-alt mr-1"></i>Excluir</>
                                    }
                                </button>
                            )}

                            <button
                                type="button"
                                className="btn btn-secondary"
                                onClick={() => this.props.onFechar(false)}
                                disabled={aguardeSalvar}
                            >
                                Cancelar
                            </button>

                            <button
                                type="button"
                                className="btn btn-primary"
                                onClick={this.salvar}
                                disabled={aguardeSalvar}
                            >
                                {aguardeSalvar
                                    ? <><i className="fas fa-spinner fa-spin mr-1"></i>Salvando...</>
                                    : <><i className="fas fa-save mr-1"></i>Salvar</>
                                }
                            </button>
                        </div>

                    </div>
                </div>
            </div>
        );
    }
}