using Shared.Infra.CrossCutting.JSONConverter;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Psicologa.Application.Pessoa.ViewsModel
{
    public class ClienteEGestorViewModel
    {
        public int Id { get; set; }

        public int PessoaJuridicaId { get; set; }
        public int EmpresaId { get; set; }
        public int? GrupoEconomicoId { get; set; }

        public bool Status { get; set; }
        [JsonConverter(typeof(DateTimeJSONConverter))]
        public DateTime DataCadastro { get; set; }

        public string Observacao { get; set; }

        public string RegimeApuracao { get; set; }

        public string CorPrimaria { get; set; }
        public string CorSecundaria1 { get; set; }
        public string CorSecundaria2 { get; set; }

        public int? CustomerSuccessId { get; set; }


        public PessoaJuridica Pessoa { get; set; }


        public class PessoaJuridica
        {
            public PessoaJuridica()
            {
                Enderecos = new List<Endereco>();
                Telefones = new List<Telefone>();
                Emails = new List<Email>();
            }

            public int Id { get; set; }
            [JsonConverter(typeof(DateTimeJSONConverter))]
            public DateTime DataCadastro { get; set; }
            [JsonConverter(typeof(DateTimeJSONConverter))]
            public DateTime DataAtualizacao { get; set; }

            public string RazaoSocial { get; set; }

            public string NomeFantasia { get; set; }

            public string InscricaoEstadual { get; set; }

            public string InscricaoMunicipal { get; set; }

            public string TipoSegmento { get; set; }

            public string Cnpj { get; set; }

            public bool Status { get; set; }

            public bool Prospect { get; set; }
            [JsonConverter(typeof(DateTimeJSONConverter))]
            public DateTime? DataFundacao { get; set; }

            public int? CodigoAdm { get; set; }

            public virtual List<Endereco> Enderecos { get; set; }

            public virtual List<Telefone> Telefones { get; set; }

            public virtual List<Email> Emails { get; set; }

        }

        public class Email
        {
            public int Id { get; set; }

            public int PessoaId { get; set; }

            public int? PessoaFisicaId { get; set; }

            public int? PessoaJuridicaId { get; set; }

            public string TipoPessoa { get; set; }

            public string Mail { get; set; }

            public string Descricao { get; set; }

            public bool EmailAdmin { get; set; }
        }

        public class Endereco
        {
            public int Id { get; set; }

            public int PessoaId { get; set; }

            public decimal? CodigoAdm { get; set; }

            public string TipoPessoa { get; set; }

            public string Cep { get; set; }

            public string Estado { get; set; }

            public string CidadeIbge { get; set; }

            public string Cidade { get; set; }

            public string Logradouro { get; set; }

            public string Bairro { get; set; }

            public string Numero { get; set; }

            public string Complemento { get; set; }
        }

        public class Telefone
        {
            public int Id { get; set; }

            public double? CodigoAdm { get; set; }

            public int PessoaId { get; set; }

            public string TipoPessoa { get; set; }

            public int? PessoaFisicaId { get; set; }

            public int? PessoaJuridicaId { get; set; }

            public string Numero { get; set; }

            public string Descricao { get; set; }
        }
    }
}
