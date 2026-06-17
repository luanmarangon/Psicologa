using Psicologa.Application.Pessoa.ViewsModel;
using Psicologa.Domain.Paciente.Entities;
using Psicologa.Domain.Pessoa.Entities;
using Psicologa.Domain.Usuario.Entities;
using Shared.Infra.CrossCutting;
using Shared.Infra.CrossCutting.ValidationResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

//using static Psicologa.Application.Pessoa.ViewsModel.ClienteEGestorViewModel;
using static Shared.Infra.CrossCutting.PaginacaoDados;

namespace Psicologa.Application.Pessoa.Services
{
    public class ApplicationPessoaService : IDisposable
    {
        private readonly Domain.Pessoa.Services.PessoaService _pessoaService;
        private readonly Shared.Domain.Cidade.Services.CidadeService _cidadeService;
        private readonly Domain.LogAplicacao.Services.LogAplicacaoService _logAplicacaoService;
        private readonly Domain.Usuario.Services.UsuarioService _usuarioService;

        private readonly IAppSettings _appSettings;

        private readonly Paciente.Services.ApplicationPacienteService _applicationPacienteService;
        private readonly Psicologo.Services.ApplicationPsicologoService _applicationPsicologoService;

        public ApplicationPessoaService(Domain.Pessoa.Services.PessoaService pessoaService, Shared.Domain.Cidade.Services.CidadeService cidadeService, Domain.LogAplicacao.Services.LogAplicacaoService logAplicacaoService,
            IAppSettings appSettings, Domain.Usuario.Services.UsuarioService usuarioService, 
            Paciente.Services.ApplicationPacienteService applicationPacienteService,
            Psicologo.Services.ApplicationPsicologoService applicationPsicologoService)
        {
            _pessoaService = pessoaService;
            _cidadeService = cidadeService;
            _logAplicacaoService = logAplicacaoService;
            _appSettings = appSettings;
            _applicationPacienteService = applicationPacienteService;
            _applicationPsicologoService = applicationPsicologoService;
            _usuarioService = usuarioService;
        }

        public (bool, ValidationResult) Salvar(PessoaViewModel pessoaVM, string[] requisicao)
        {
            var dadosExistente = _pessoaService.Obter(pessoaVM.Dados.Id);

            bool operacao = false;
            Domain.Pessoa.Entities.Pessoa pessoa = new Domain.Pessoa.Entities.Pessoa();
            pessoa.Id = pessoaVM.Dados.Id;
            pessoa.Nome = pessoaVM.Dados.Nome;
            pessoa.DocIdTipo = (Domain.Pessoa.Entities.Pessoa.TpDoc)Convert.ToInt32(pessoaVM.Dados.DocIdTipo);
            pessoa.DocIdNro = pessoaVM.Dados.DocIdNro;
            pessoa.Ativo = pessoaVM.Dados.Ativo;
            pessoa.Endereco = new Endereco();
            pessoa.Endereco.Id = pessoaVM.Endereco.Id;
            pessoa.Endereco.Logradouro = pessoaVM.Endereco.Logradouro;
            pessoa.Endereco.Numero = pessoaVM.Endereco.Numero;
            pessoa.Endereco.Bairro = pessoaVM.Endereco.Bairro;
            pessoa.Endereco.CEP = pessoaVM.Endereco.CEP;
            pessoa.Endereco.Complemento = pessoaVM.Endereco.Complemento;
            pessoa.Endereco.PontoReferencia = pessoaVM.Endereco.PontoReferencia;
            pessoa.Endereco.Latitude = pessoaVM.Endereco.Latitude;
            pessoa.Endereco.Longitude = pessoaVM.Endereco.Longitude;
            pessoa.Endereco.Cidade = pessoaVM.Endereco.Cidade;
            pessoa.Endereco.UF = pessoaVM.Endereco.UF;

            pessoa.Contatos = new List<PessoaContato>();

            foreach (var c in pessoaVM.Contatos)
            {
                pessoa.Contatos.Add(new PessoaContato()
                {
                    Id = c.Id,
                    Tipo = (PessoaContato.TpContato)c.Tipo,
                    Contato = c.Contato,
                    Observacao = c.Observacao
                });
            }

            pessoa.Tipos = new List<PessoaTipo>();

            foreach (var t in pessoaVM.Tipos)
            {
                pessoa.Tipos.Add(new PessoaTipo()
                {
                    Id = t.Id,
                    Tipo = (PessoaTipo.TpPessoa)t.Tipo
                });
            }

            //Dados específicos
            if (pessoa.DocIdTipo == Domain.Pessoa.Entities.Pessoa.TpDoc.CNPJ)
            {
                var pj = new PessoaJuridica(pessoa);
                pessoa = pj;
                pj.RazaoSocial = pessoaVM.Dados.RazaoSocial;
            }
            else
            {
                var pf = new PessoaFisica(pessoa);
                pessoa = pf;

                if (pessoaVM.Dados.DataNascimento.ToString() != "")
                {
                    try
                    {
                        pf.DataNascimento = Convert.ToDateTime(pessoaVM.Dados.DataNascimento);
                    }
                    catch
                    {
                        pessoa.ValidationResult.Add(Message.TypeMessage.InvalidField, "Data de Nascimento inválida.");
                    }
                }

                pf.Sexo = (PessoaFisica.TpSexo)pessoaVM.Dados.Sexo;
            }

            if (pessoa.ValidationResult.Count == 0)
            {
                operacao = _pessoaService.Salvar(pessoa);

                if (operacao)
                {
                    pessoaVM.Dados.Id = pessoa.Id;

                    //Criar o Paciente

                    if (pessoa.Tipos.Any(t => t.Tipo == PessoaTipo.TpPessoa.Paciente))
                    {
                        var paciente = _applicationPacienteService.ObterPorPessoaId(pessoa.Id);

                        var pacienteVM = new Application.Paciente.ViewsModel.PacienteViewModel()
                        {
                            Id = paciente.Id,
                            Ativo = pessoa.Ativo,
                            PessoaId = pessoa.Id,
                            ContatoEmergenciaNome = pessoaVM.Paciente?.ContatoEmergenciaNome,
                            ContatoEmergenciaTelefone = pessoaVM.Paciente?.ContatoEmergenciaTelefone,
                            ResponsavelId = pessoaVM.Paciente?.ResponsavelId,
                        };
                        (operacao, _) = _applicationPacienteService.Salvar(pacienteVM, requisicao);
                    }

                    //Criar o Psicologo
                    if (pessoa.Tipos.Any(t => t.Tipo == PessoaTipo.TpPessoa.Psicologo))
                    {

                        var psicologoVM = new Application.Psicologo.ViewsModel.PsicologoViewModel()
                        {
                            Id = 0,
                            PessoaId = pessoa.Id,
                            Ativo = pessoa.Ativo, 
                            Crp = pessoaVM.Psicologo?.Crp,
                            CrpUf = pessoaVM.Psicologo?.CrpUf,
                            DataEmissaoCrp = pessoaVM.Psicologo?.DataEmissaoCrp ?? DateTime.MinValue
                        };
                        (operacao, _) = _applicationPsicologoService.Salvar(psicologoVM, requisicao);
                    }
                }
            }

            if (operacao)
            {
                _logAplicacaoService.Registrar(pessoa.Id, requisicao, dadosExistente, pessoa, "Pessoa", MethodBase.GetCurrentMethod()?.DeclaringType?.Name, MethodBase.GetCurrentMethod()?.Name);
            }


            return (operacao, pessoa.ValidationResult);
        }

        //public (bool, ValidationResult) SalvarCadastroInicial(PessoaCadastroInicialViewModel pessoaVM, string[] requisicao)
        //{
        //    var dadosExistente = _pessoaService.Obter(pessoaVM.Id);
        //    List<string> usuarioEmail = new List<string>();

        //    bool operacao = false;
        //    Domain.Pessoa.Entities.Pessoa pessoa = new Domain.Pessoa.Entities.Pessoa();
        //    pessoa.Id = pessoaVM.Id;
        //    pessoa.Nome = pessoaVM.Nome.ToUpper();
        //    pessoa.DocIdTipo = (Domain.Pessoa.Entities.Pessoa.TpDoc)Convert.ToInt32(pessoaVM.DocIdTipo);
        //    pessoa.DocIdNro = pessoaVM.DocIdNro;
        //    pessoa.Ativo = true;
        //    pessoa.Endereco = new Endereco();
        //    pessoa.Endereco.Id = pessoaVM.Endereco.Id;
        //    pessoa.Endereco.Logradouro = pessoaVM.Endereco.Logradouro;
        //    pessoa.Endereco.Numero = pessoaVM.Endereco.Numero;
        //    pessoa.Endereco.Bairro = pessoaVM.Endereco.Bairro;
        //    pessoa.Endereco.CEP = pessoaVM.Endereco.CEP;
        //    pessoa.Endereco.Complemento = pessoaVM.Endereco.Complemento;
        //    pessoa.Endereco.PontoReferencia = pessoaVM.Endereco.PontoReferencia;
        //    pessoa.Endereco.Latitude = pessoaVM.Endereco.Latitude;
        //    pessoa.Endereco.Longitude = pessoaVM.Endereco.Longitude;
        //    pessoa.Endereco.Cidade = pessoaVM.Endereco.Cidade;
        //    pessoa.Endereco.UF = pessoaVM.Endereco.UF;

        //    pessoa.Contatos = new List<PessoaContato>();

        //    foreach (var c in pessoaVM.Contatos)
        //    {
        //        pessoa.Contatos.Add(new PessoaContato()
        //        {
        //            Id = c.Id,
        //            Tipo = (PessoaContato.TpContato)c.Tipo,
        //            Contato = c.Contato,
        //        });
        //    }

        //    pessoa.Tipos = new List<PessoaTipo>();

        //    foreach (var t in pessoaVM.Tipos)
        //    {
        //        pessoa.Tipos.Add(new PessoaTipo()
        //        {
        //            Id = t.Id,
        //            Tipo = (PessoaTipo.TpPessoa)t.Tipo
        //        });
        //    }

        //    //Dados específicos
        //    if (pessoa.DocIdTipo == Domain.Pessoa.Entities.Pessoa.TpDoc.CNPJ)
        //    {
        //        var pj = new PessoaJuridica(pessoa);
        //        pessoa = pj;
        //        pj.RazaoSocial = pessoaVM.RazaoSocial.ToUpper();
        //    }
        //    else
        //    {
        //        var pf = new PessoaFisica(pessoa);
        //        pessoa = pf;

        //        if (pessoaVM.DataNascimento.ToString() != "")
        //        {
        //            try
        //            {
        //                pf.DataNascimento = Convert.ToDateTime(pessoaVM.DataNascimento);
        //            }
        //            catch
        //            {
        //                pessoa.ValidationResult.Add(Message.TypeMessage.InvalidField, "Data de Nascimento inválida.");
        //            }
        //        }
        //        else
        //            pf.DataNascimento = DateTime.MinValue;

        //        pf.Sexo = (PessoaFisica.TpSexo)pessoaVM.Sexo;
        //    }

        //    if (pessoa.ValidationResult.Count == 0)
        //    {
        //        operacao = _pessoaService.Salvar(pessoa);

        //        if (operacao)
        //        {
        //            pessoaVM.Id = pessoa.Id;

        //            string emailCriado = null;

        //            //var contatoEmail = pessoa.Contatos.FirstOrDefault(c => c.Tipo == 4);
        //            //emailCriado = contatoEmail?.Contato;

        //            if (pessoa.Contatos != null && pessoa.Contatos.Any())
        //            {
        //                var contatoEmail = pessoa.Contatos.FirstOrDefault(c => c.Tipo == PessoaContato.TpContato.Email);
        //                emailCriado = contatoEmail?.Contato;
        //            }

        //            //Criar o usuário
        //            (operacao, usuarioEmail) = CriarUsuarioCadastroInicial(pessoa);

        //            if (operacao)
        //            {
        //                Shared.Infra.CrossCutting.Email email = new Shared.Infra.CrossCutting.Email(_appSettings.Email.Email, _appSettings.Email.Email, _appSettings.Email.Senha, _appSettings.Email.Remetente, _appSettings.Email.SMTP, _appSettings.Email.Porta);

        //                string texto =
        //                        $@"
        //                        <p style='margin:0 0 12px 0;'>
        //                            Olá, <strong style='color:#111827;'>{pessoaVM.Nome}</strong>!
        //                        </p>
        //                        <p style='margin:0 0 24px 0;color:#6b7280;font-size:14px;'>
        //                            Seu cadastro foi realizado com sucesso. Abaixo estão suas credenciais de acesso:
        //                        </p>
        //                        <table cellpadding='0' cellspacing='0' border='0' style='background:#1e293b;border-radius:8px;padding:20px 24px;width:100%;margin-bottom:24px;'>
        //                            <tr>
        //                                <td style='font-size:11px;color:#94a3b8;text-transform:uppercase;letter-spacing:.08em;padding-bottom:4px;'>Usuário</td>
        //                            </tr>
        //                            <tr>
        //                                <td style='font-size:16px;font-weight:bold;color:#f1f5f9;padding-bottom:16px;'>{usuarioEmail[0]}</td>
        //                            </tr>
        //                            <tr>
        //                                <td style='font-size:11px;color:#94a3b8;text-transform:uppercase;letter-spacing:.08em;padding-bottom:4px;'>Senha</td>
        //                            </tr>
        //                            <tr>
        //                                <td style='font-size:16px;font-weight:bold;color:#f1f5f9;'>{usuarioEmail[1]}</td>
        //                            </tr>
        //                        </table>
        //                        <p style='margin:0;font-size:13px;color:#9ca3af;'>(Enviado a partir do MultiServiços)</p>
        //                        ";

        //                string msgAux;
        //                (operacao, msgAux) = email.EnviarEmail(emailCriado, texto, "Psicologa - Cadastro");
        //            }
        //        }
        //    }

        //    if (operacao)
        //    {
        //        var dadosAtualizado = _pessoaService.Obter(pessoaVM.Id);
        //        var (retorno, dadosAlterados) = _logAplicacaoService.ObterDiferencas(dadosExistente, dadosAtualizado);

        //        if (dadosAlterados.Any())
        //        {
        //            var log = _logAplicacaoService.Criar(
        //                requisicao: requisicao,
        //                entidade: nameof(Domain.Pessoa.Entities.Pessoa),
        //                entidadeId: pessoaVM.Id,
        //                operacao: retorno,
        //                dadosAntes: dadosExistente,
        //                dadosDepois: dadosAtualizado,
        //                dadosAlterados: dadosAlterados,
        //                aplicacao: MethodBase.GetCurrentMethod()?.DeclaringType?.Name,
        //                metodo: MethodBase.GetCurrentMethod()?.Name
        //            );
        //            _logAplicacaoService.Salvar(log);
        //        }
        //    }

        //    return (operacao, pessoa.ValidationResult);
        //}

        private (bool, List<string>) CriarUsuarioCadastroInicial(Domain.Pessoa.Entities.Pessoa pessoa)
        {
            bool operacao = false;

            Domain.Usuario.Entities.Usuario usuarioCriar = new Domain.Usuario.Entities.Usuario();

            usuarioCriar.Id = 0;
            //usuarioCriar.Nome = pessoa.Nome;

            usuarioCriar.Nome = _usuarioService.GerarLoginUsuario(
                pessoa.Nome,
                login => _usuarioService.ExisteLogin(login)
            );

            usuarioCriar.Senha = _usuarioService.GerarSenhaUsuario();

            usuarioCriar.Perfil = pessoa.Tipos[0].Tipo == PessoaTipo.TpPessoa.Paciente
                                                        ? PerfilUsuario.TpPerfil.Administrativo    // substituir pelo valor correto do enum
                                                        : PerfilUsuario.TpPerfil.Psicologo; // substituir pelo valor correto do enum
            usuarioCriar.Pessoa = new Domain.Pessoa.Entities.Pessoa()
            {
                Id = pessoa.Id
            };

            var usuarioEmail = new List<string>
            {
                usuarioCriar.Nome.ToLower(), usuarioCriar.Senha
            };

            operacao = _usuarioService.Salvar(usuarioCriar, usuarioCriar.Senha);

            return (operacao, usuarioEmail);
        }

        public PessoaViewModel Obter(int id)
        {
            var pessoa = _pessoaService.Obter(id);

            if (pessoa == null)
                return null;

            var dados = new PessoaViewModel.PessoaDados()
            {
                Id = pessoa.Id,
                Nome = pessoa.Nome,
                DocIdNro = PessoaUtils.FormatarCPFCPNJ(pessoa.DocIdNro),
                DocIdTipo = (int)pessoa.DocIdTipo,
                Ativo = pessoa.Ativo
            };

            PessoaFisica pf = new PessoaFisica(pessoa);
            PessoaJuridica pj = new PessoaJuridica(pessoa);
            dados.DataNascimento = pf.DataNascimento;
            dados.RazaoSocial = pj.RazaoSocial;
            dados.Sexo = (int)pf.Sexo;

            var endereco = new PessoaViewModel.PessoaEndereco()
            {
                Id = pessoa.Endereco.Id,
                Logradouro = pessoa.Endereco.Logradouro,
                Numero = pessoa.Endereco.Numero,
                Bairro = pessoa.Endereco.Bairro,
                CEP = pessoa.Endereco.CEP,
                Complemento = pessoa.Endereco.Complemento,
                UF = pessoa.Endereco.UF,
                Cidade = pessoa.Endereco.Cidade,
                PontoReferencia = pessoa.Endereco.PontoReferencia,
                Latitude = pessoa.Endereco.Latitude,
                Longitude = pessoa.Endereco.Longitude
            };

            var contatos = new List<PessoaViewModel.PessoaContato>();

            foreach (var c in pessoa.Contatos)
            {
                contatos.Add(new PessoaViewModel.PessoaContato
                {
                    Id = c.Id,
                    Tipo = (int)c.Tipo,
                    TipoNome = Utils.ObterDescricaoEnum(c.Tipo),
                    Contato = c.Contato,
                    Observacao = c.Observacao,
                });
            }

            var tipos = new List<PessoaViewModel.PessoaTipo>();

            foreach (var t in pessoa.Tipos)
            {
                tipos.Add(new PessoaViewModel.PessoaTipo
                {
                    Id = t.Id,
                    Tipo = (int)t.Tipo,
                    TipoNome = Utils.ObterDescricaoEnum(t.Tipo)
                });
            }

            var pacienteVM = (PessoaViewModel.PessoaPaciente?)null;

            if (pessoa.Tipos.Any(t => t.Tipo == PessoaTipo.TpPessoa.Paciente))
            {
                var paciente = _applicationPacienteService.ObterPorPessoaId(pessoa.Id);
                if (paciente != null)
                {
                    pacienteVM = new PessoaViewModel.PessoaPaciente()
                    {
                        ContatoEmergenciaNome = paciente.ContatoEmergenciaNome,
                        ContatoEmergenciaTelefone = paciente.ContatoEmergenciaTelefone,
                        ResponsavelId = paciente.ResponsavelId,
                        ResponsavelNome = paciente.ResponsavelNome
                    };
                }
            }

            var psicologoVM = (PessoaViewModel.PessoaPsicologo?)null;
            if(pessoa.Tipos.Any(t => t.Tipo == PessoaTipo.TpPessoa.Psicologo))
            {
                var psicologo = _applicationPsicologoService.ObterPorPessoaId(pessoa.Id);
                if(psicologo != null)
                {
                    psicologoVM = new PessoaViewModel.PessoaPsicologo()
                    {
                        Crp = psicologo.Crp,
                        CrpUf = psicologo.CrpUf,
                        DataEmissaoCrp = psicologo.DataEmissaoCrp
                    };
                }
            }


            var pessoaRetorno = new PessoaViewModel()
            {
                DataAlteracao = pessoa.DataAlteracao,
                Dados = dados,
                Endereco = endereco,
                Contatos = contatos,
                Tipos = tipos,
                Paciente = pacienteVM, 
                Psicologo = psicologoVM
            };

            return pessoaRetorno;
        }

        public PessoaViewModel ObterPorDocumentoIdentificacao(string documento)
        {
            var pessoa = _pessoaService.ObterPorDocumentoIdentificacao(documento);

            if (pessoa == null)
                return null;

            var dados = new PessoaViewModel.PessoaDados()
            {
                Id = pessoa.Id,
                Nome = pessoa.Nome,
                DocIdNro = PessoaUtils.FormatarCPFCPNJ(pessoa.DocIdNro),
                DocIdTipo = (int)pessoa.DocIdTipo,
            };

            PessoaFisica pf = new PessoaFisica(pessoa);
            PessoaJuridica pj = new PessoaJuridica(pessoa);
            dados.DataNascimento = pf.DataNascimento;
            dados.RazaoSocial = pj.RazaoSocial;
            dados.Sexo = (int)pf.Sexo;

            var endereco = new PessoaViewModel.PessoaEndereco()
            {
                Id = pessoa.Endereco.Id,
                Logradouro = pessoa.Endereco.Logradouro,
                Numero = pessoa.Endereco.Numero,
                Bairro = pessoa.Endereco.Bairro,
                CEP = pessoa.Endereco.CEP,
                Complemento = pessoa.Endereco.Complemento,
                UF = pessoa.Endereco.UF,
                Cidade = pessoa.Endereco.Cidade,
                PontoReferencia = pessoa.Endereco.PontoReferencia,
                Latitude = pessoa.Endereco.Latitude,
                Longitude = pessoa.Endereco.Longitude
            };

            var contatos = new List<PessoaViewModel.PessoaContato>();

            foreach (var c in pessoa.Contatos)
            {
                contatos.Add(new PessoaViewModel.PessoaContato
                {
                    Id = c.Id,
                    Tipo = (int)c.Tipo,
                    TipoNome = Utils.ObterDescricaoEnum(c.Tipo),
                    Contato = c.Contato,
                    Observacao = c.Observacao,
                });
            }

            var tipos = new List<PessoaViewModel.PessoaTipo>();

            foreach (var t in pessoa.Tipos)
            {
                tipos.Add(new PessoaViewModel.PessoaTipo
                {
                    Id = t.Id,
                    Tipo = (int)t.Tipo,
                    TipoNome = Utils.ObterDescricaoEnum(t.Tipo)
                });
            }

            var pessoaRetorno = new PessoaViewModel()
            {
                Dados = dados,
                DataAlteracao = pessoa.DataAlteracao,
                Endereco = endereco,
                Contatos = contatos,
                Tipos = tipos
            };

            return pessoaRetorno;
        }

        public bool Excluir(int id, string[] requisicao)
        {
            bool operacao = false;
            var dadosExistente = _pessoaService.Obter(id);
            operacao = _pessoaService.Excluir(id);

            if (operacao)
            {
                _logAplicacaoService.Registrar(id, requisicao, dadosExistente, null, "Pessoa", MethodBase.GetCurrentMethod()?.DeclaringType?.Name, MethodBase.GetCurrentMethod()?.Name);
            }
            return operacao;




        }

        public IEnumerable<PessoaConsultaViewModel> Consultar(string nome, PaginacaoDados paginacao, PessoaTipo.TpPessoa tpPessoa = PessoaTipo.TpPessoa.Indefinido)
        {
            List<PessoaConsultaViewModel> retorno = new List<PessoaConsultaViewModel>();

            var pessoas = _pessoaService.Consultar(nome, paginacao, tpPessoa);

            foreach (var pessoa in pessoas)
            {
                retorno.Add(FormatarRetornoConsulta(pessoa));
            }

            paginacao.OrdenacaoNome = Utils.ObterDescricaoEnum(paginacao.Ordenacao);
            if (paginacao.Ordenacao == TpOrdenacao.Nome)
            {
                retorno = retorno.OrderBy(o => o.Nome).ToList();
            }

            return retorno;
        }

        public IEnumerable<PessoaConsultaViewModel> ObterUltimos(int top, PaginacaoDados paginacao, PessoaTipo.TpPessoa tpPessoa = PessoaTipo.TpPessoa.Indefinido)
        {
            var pessoas = _pessoaService.ObterUltimos(top, paginacao, tpPessoa);
            List<PessoaConsultaViewModel> retorno = new List<PessoaConsultaViewModel>();

            foreach (var pessoa in pessoas)
            {
                retorno.Add(FormatarRetornoConsulta(pessoa));
            }

            paginacao.OrdenacaoNome = Utils.ObterDescricaoEnum(paginacao.Ordenacao);
            if (paginacao.Ordenacao == TpOrdenacao.Nome)
            {
                retorno = retorno.OrderBy(o => o.Nome).ToList();
            }

            return retorno;
        }

        /*public IEnumerable<PessoaConsultaViewModel> ConsultarClientes(string nome)
        {
            List<PessoaConsultaViewModel> retorno = new List<PessoaConsultaViewModel>();

            if (!string.IsNullOrEmpty(nome))
            {
                var pessoas = _pessoaService.Consultar(nome, PessoaTipo.TpPessoa.ClienteEmpresa);

                foreach (var pessoa in pessoas)
                {
                    retorno.Add(FormatarRetornoConsulta(pessoa));
                }
            }

            return retorno;
        }*/

        /*public IEnumerable<PessoaConsultaViewModel> ObterUltimos(int top, PessoaTipo.TpPessoa tpPessoa = PessoaTipo.TpPessoa.Indefinido)
        {
            var pessoas = _pessoaService.ObterUltimos(top, tpPessoa);
            List<PessoaConsultaViewModel> retorno = new List<PessoaConsultaViewModel>();

            foreach (var pessoa in pessoas)
            {
                retorno.Add(FormatarRetornoConsulta(pessoa));
            }

            return retorno;
        }*/

        internal PessoaConsultaViewModel FormatarRetornoConsulta(Domain.Pessoa.Entities.Pessoa pessoa)
        {
            var dados = new PessoaConsultaViewModel.PessoaDados()
            {
                Id = pessoa.Id,
                Nome = pessoa.Nome,
                DocIdNro = PessoaUtils.FormatarCPFCPNJ(pessoa.DocIdNro),
                DocIdTipo = (int)pessoa.DocIdTipo,
                DocIdTipoNome = Utils.ObterDescricaoEnum(pessoa.DocIdTipo),
                Ativo = pessoa.Ativo
            };

            var endereco = new PessoaConsultaViewModel.PessoaEndereco()
            {
                Latitude = pessoa.Endereco.Latitude,
                Longitude = pessoa.Endereco.Longitude
            };

            var contatos = new List<PessoaConsultaViewModel.PessoaContato>();

            foreach (var c in pessoa.Contatos)
            {
                contatos.Add(new PessoaConsultaViewModel.PessoaContato
                {
                    Id = c.Id,
                    Tipo = ((int)c.Tipo).ToString(),
                    TipoNome = Utils.ObterDescricaoEnum(c.Tipo),
                    Contato = c.Contato,
                });
            }

            var tipos = new List<PessoaConsultaViewModel.PessoaTipo>();

            foreach (var t in pessoa.Tipos)
            {
                tipos.Add(new PessoaConsultaViewModel.PessoaTipo
                {
                    Id = t.Id,
                    Tipo = ((int)t.Tipo).ToString(),
                    TipoNome = Utils.ObterDescricaoEnum(t.Tipo)
                });
            }

            var pessoaRetorno = new PessoaConsultaViewModel()
            {
                Dados = dados,
                Endereco = endereco,
                Contatos = contatos,
                Tipos = tipos
            };

            return pessoaRetorno;
        }

        public void Dispose()
        {
        }
    }
}