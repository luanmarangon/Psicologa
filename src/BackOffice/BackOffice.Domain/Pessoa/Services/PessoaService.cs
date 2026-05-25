using Psicologa.Domain.Pessoa.Entities;
using Shared.Infra.CrossCutting;
using Shared.Infra.CrossCutting.ValidationResult;
using System;
using System.Collections.Generic;

namespace Psicologa.Domain.Pessoa.Services
{
    public class PessoaService : ServiceBase<Entities.Pessoa>, IServiceBase<Entities.Pessoa>
    {
        public readonly Interfaces.Repositories.IPessoaRepository _repository;

         
        public PessoaService(Interfaces.Repositories.IPessoaRepository repository)
            :base(repository)
        {
            _repository = repository;
        }

        public bool Salvar(Psicologa.Domain.Pessoa.Entities.Pessoa pessoa)
        {
            bool operacao = false;

            if (pessoa.Id == 0)
            { 
                pessoa.DataCadastro = DateTime.Now;
                pessoa.DataAlteracao = pessoa.DataCadastro;
            }
            else
            {
                pessoa.DataAlteracao = DateTime.Now;
            }

            pessoa.Nome = pessoa.Nome.ToUpper();

            if(pessoa.GetType() == typeof(PessoaJuridica))
            {
                if (!string.IsNullOrEmpty(pessoa.DocIdNro))
                {
                    pessoa.DocIdNro = pessoa.DocIdNro.ToUpper();
                }
            }

            if (pessoa.Endereco != null)
            {
                if (!string.IsNullOrEmpty(pessoa.Endereco.Latitude))
                {
                    pessoa.Endereco.Latitude = pessoa.Endereco.Latitude.Replace(",", ".");
                    if (pessoa.Endereco.Latitude.Length > 11)
                        pessoa.Endereco.Latitude = pessoa.Endereco.Latitude.Substring(0, 11);
                }

                if (!string.IsNullOrEmpty(pessoa.Endereco.Longitude))
                {
                    pessoa.Endereco.Longitude = pessoa.Endereco.Longitude.Replace(",", ".");
                    if (pessoa.Endereco.Longitude.Length > 11)
                        pessoa.Endereco.Longitude = pessoa.Endereco.Longitude.Substring(0, 11);
                }
             }

            foreach (var item in pessoa.Contatos)
            {
                item.Contato = item.Contato.ToLower();
            }

            bool valido = pessoa.Validar();

            if (valido)
            {
                var vr = DocumentoIdentificacaoUnico(pessoa);
                if (!vr.IsValid())
                {
                    pessoa.ValidationResult.Add(vr.Messages);
                    valido = false;
                }

                if (valido)
                {
                    vr = FormaContatoUnica(pessoa);
                    if (!vr.IsValid())
                    {
                        pessoa.ValidationResult.Add(vr.Messages);
                        valido = false;
                    }
                }
            }

            if (valido)
            {
                operacao = _repository.Salvar(pessoa);
            }

            return operacao;
        }

        public bool SalvarPorIntegracao(Psicologa.Domain.Pessoa.Entities.Pessoa pessoa)
        {
            bool operacao = false;

            if (pessoa.Id == 0)
            {
                pessoa.DataCadastro = DateTime.Now;
                pessoa.DataAlteracao = pessoa.DataCadastro;
            }
            else
            {
                pessoa.DataAlteracao = DateTime.Now;
            }

            pessoa.Nome = pessoa.Nome.ToUpper();

            foreach (var item in pessoa.Contatos)
            {
                item.Contato = item.Contato != null ? item.Contato.ToLower() : "";
            }

            bool valido = pessoa.Validar();

            if (valido)
            {
                var vr = DocumentoIdentificacaoUnico(pessoa);
                if (!vr.IsValid())
                {
                    pessoa.ValidationResult.Add(vr.Messages);
                    valido = false;
                }

                if (valido)
                {
                    vr = FormaContatoUnica(pessoa);
                    if (!vr.IsValid())
                    {
                        pessoa.ValidationResult.Add(vr.Messages);
                        valido = false;
                    }
                }
            }

            if (valido)
            {
                operacao = _repository.Salvar(pessoa);
            }

            return operacao;
        }

        public Domain.Pessoa.Entities.Pessoa Obter(int id)
        {
            return _repository.Obter(id);
        }

        public bool Excluir(int id)
        {
            return _repository.Excluir(id);
        }

        private ValidationResult DocumentoIdentificacaoUnico(Entities.Pessoa pessoa)
        {
            bool valido;
            ValidationResult vr = new ValidationResult();

            if (pessoa.Id <= 0)
            {
                if (ObterPorDocumentoIdentificacao(pessoa.DocIdNro) != null)
                    valido = false;
                else valido = true;
            }
            else
            {
                var pAux = ObterPorDocumentoIdentificacao(pessoa.DocIdNro);

                if (pAux != null && pAux.Id != pessoa.Id)
                    valido = false;
                else valido = true;
            }

            if (!valido)
            {
                vr.AddUserMessageInvalidField($"O documento de identificação {pessoa.DocIdNro} é utilizado por outra pessoa.");
            }

            return vr;
        }

        private ValidationResult FormaContatoUnica(Entities.Pessoa pessoa)
        {
            bool valido = true;
            ValidationResult vr = new ValidationResult();

            string contato = "";

            foreach (var c in pessoa.Contatos)
            {
                contato = c.Contato;
                if (pessoa.Id <= 0)
                {
                    if (ObterPorFormaContato(contato) != null)
                    {
                        valido = false;
                        break;
                    }
                    else valido = true;
                }
                else
                {
                    var pAux = ObterPorFormaContato(contato);

                    if (pAux != null && pAux.Id != pessoa.Id)
                    {
                        valido = false;
                        break;
                    }
                    else valido = true;
                }

            }

            if (!valido)
            {
                vr.AddUserMessageInvalidField($"A Forma de Contato {contato} é utilizada por outra pessoa.");
            }

            return vr;
        }

        public Entities.Pessoa ObterPorDocumentoIdentificacao(string documento)
        {
            documento = documento.Replace(".", "").Replace("-", "").Replace("/", "");
            return _repository.ObterPorDocumentoIdentificacao(documento);
        }

        public Entities.Pessoa ObterPorFormaContato(string contato)
        {
            return _repository.ObterPorFormaContato(contato);
        }


        public IEnumerable<Psicologa.Domain.Pessoa.Entities.Pessoa> Consultar(string nome, PaginacaoDados paginacao, PessoaTipo.TpPessoa tpPessoa = PessoaTipo.TpPessoa.Indefinido)
        {
            if (string.IsNullOrEmpty(nome))
                nome = "";
            else
                nome = nome.Replace("%", "").Replace("_", "");

            return _repository.Consultar(nome, paginacao, tpPessoa);
        }

        public IEnumerable<Psicologa.Domain.Pessoa.Entities.Pessoa> ObterUltimos(int top, PaginacaoDados paginacao, PessoaTipo.TpPessoa tpPessoa = PessoaTipo.TpPessoa.Indefinido)
        {
            return _repository.ObterUltimos(top, paginacao, tpPessoa);
        }


        public IEnumerable<Psicologa.Domain.Pessoa.Entities.Pessoa> ObterTodosClientesAtivos()
        {
            return _repository.ObterTodosClientesAtivos();
        }

        public IEnumerable<Entities.Pessoa> ObterUsuariosColaboradores(int clienteIdVinculo, string termo)
        {
            return _repository.ObterUsuariosColaboradores(clienteIdVinculo, termo);
        }



    }
}
