using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Shared.Infra.CrossCutting.JSONConverter;

namespace Psicologa.Application.Pessoa.ViewsModel
{
    public class PessoaViewModel
    {
        public PessoaDados Dados { get; set; }
        public DateTime DataAlteracao { get; set; }
        public PessoaEndereco Endereco { get; set; }
        public IEnumerable<PessoaContato> Contatos { get; set; }
        public IEnumerable<PessoaTipo> Tipos { get; set; }
        public PessoaPaciente? Paciente { get; set; }
        public PessoaPsicologo? Psicologo { get; set; }

        public class PessoaDados
        {
            [JsonConverter(typeof(EncryptIdJSONConverter))]
            public int Id { get; set; }
            public string Nome { get; set; }
            public string DocIdNro { get; set; }
            [JsonConverter(typeof(Int32JSONConverter))]
            public int DocIdTipo { get; set; }
            [JsonConverter(typeof(DateTimeJSONConverter))]
            public DateTime DataNascimento { get; set; }
            public string RazaoSocial { get; set; }
            [JsonConverter(typeof(Int32JSONConverter))]
            public int Sexo { get; set; }
            public bool Ativo { get; set; }
        }
        public struct PessoaEndereco
        {
            [JsonConverter(typeof(EncryptIdJSONConverter))]
            public int Id { get; set; }
            public string Logradouro { get; set; }
            public string Numero { get; set; }
            public string Bairro { get; set; }
            public string CEP { get; set; }
            public string Complemento { get; set; }
            public string UF { get; set; }
            public string PontoReferencia { get; set; }
            public string Latitude { get; set; }
            public string Longitude { get; set; }

            //[JsonConverter(typeof(EncryptIdJSONConverter))]
            //public int CidadeId { get; set; }

            public string Cidade { get; set; }

        }
        public struct PessoaContato
        {
            [JsonConverter(typeof(EncryptIdJSONConverter))]
            public int Id { get; set; }

            [JsonConverter(typeof(Int32JSONConverter))]
            public int Tipo { get; set; }
            public string TipoNome { get; set; }
            public string Contato { get; set; }
            public string Observacao { get; set; }
        }
        public struct PessoaTipo
        {
            [JsonConverter(typeof(EncryptIdJSONConverter))]
            public int Id { get; set; }
            [JsonConverter(typeof(Int32JSONConverter))]
            public int Tipo { get; set; }
            public string TipoNome { get; set; }
        }

        public struct PessoaPaciente
        {
            public string? ContatoEmergenciaNome { get; set; }
            public string? ContatoEmergenciaTelefone { get; set; }
            [JsonConverter(typeof(EncryptIdJSONConverter))]
            public int? ResponsavelId { get; set; }
            public string? ResponsavelNome { get; set; }
        }

        public struct PessoaPsicologo
        {
            public string? Crp { get; set; }
            public string? CrpUf { get; set; }
            [JsonConverter(typeof(DateTimeJSONConverter))]
            public DateTime DataEmissaoCrp { get; set; }
        }
    }

    public class PessoaConsultaViewModel
    {

        public PessoaDados Dados { get; set; }
        public DateTime DataAlteracao { get; set; }
        public PessoaEndereco Endereco { get; set; }
        public IEnumerable<PessoaContato> Contatos { get; set; }
        public IEnumerable<PessoaTipo> Tipos { get; set; }
        public object Nome { get; internal set; }
        public PessoaPaciente? Paciente { get; set; }
        public PessoaPsicologo? Psicologo { get; set; }


        public class PessoaDados
        {

            [JsonConverter(typeof(EncryptIdJSONConverter))]
            public int Id { get; set; }
            public string Nome { get; set; }
            public string DocIdNro { get; set; }
            [JsonConverter(typeof(Int32JSONConverter))]
            public int DocIdTipo { get; set; }
            public string DocIdTipoNome { get; set; }
            public string DataNascimento { get; set; }
            public string RazaoSocial { get; set; }
            public int? Sexo { get; set; }
            public bool Ativo { get; set; }
        }

        public struct PessoaEndereco
        {
            public string Latitude { get; set; }
            public string Longitude { get; set; }
        }

        public struct PessoaContato
        {
            [JsonConverter(typeof(EncryptIdJSONConverter))]
            public int Id { get; set; }
            public string Tipo { get; set; }
            public string TipoNome { get; set; }
            public string Contato { get; set; }
        }


        public struct PessoaTipo
        {
            [JsonConverter(typeof(EncryptIdJSONConverter))]
            public int Id { get; set; }
            public string Tipo { get; set; }
            public string TipoNome { get; set; }
        }

        public struct PessoaPaciente
        {
            public string? ContatoEmergenciaNome { get; set; }
            public string? ContatoEmergenciaTelefone { get; set; }
            [JsonConverter(typeof(EncryptIdJSONConverter))]
            public int? ResponsavelId { get; set; }
            public string? ResponsavelNome { get; set; }
        }
        public struct PessoaPsicologo
        {
            public string? Crp { get; set; }
            public string? CrpUf { get; set; }
            [JsonConverter(typeof(DateTimeJSONConverter))]
            public DateTime DataEmissaoCrp { get; set; }
        }
    }


    //public class PessoaCadastroInicialViewModel
    //{
    //    [JsonConverter(typeof(EncryptIdJSONConverter))]
    //    public int Id { get; set; }
    //    public string Nome { get; set; }
    //    public string DocIdNro { get; set; }
    //    [JsonConverter(typeof(Int32JSONConverter))]
    //    public int DocIdTipo { get; set; }
    //    [JsonConverter(typeof(DateTimeJSONConverter))]
    //    public DateTime? DataNascimento { get; set; }
    //    public string RazaoSocial { get; set; }
    //    [JsonConverter(typeof(Int32JSONConverter))]
    //    public int Sexo { get; set; }
    //    public bool Ativo { get; set; }
    //    public PessoaEndereco Endereco { get; set; }
    //    public IEnumerable<PessoaContato> Contatos { get; set; }
    //    public IEnumerable<PessoaTipo> Tipos { get; set; }

    //    public class PessoaEndereco
    //    {
    //        [JsonConverter(typeof(EncryptIdJSONConverter))]
    //        public int Id { get; set; }
    //        public string Logradouro { get; set; }
    //        public string Numero { get; set; }
    //        public string Bairro { get; set; }
    //        public string CEP { get; set; }
    //        public string Complemento { get; set; }
    //        public string UF { get; set; }
    //        public string PontoReferencia { get; set; }
    //        public string Latitude { get; set; }
    //        public string Longitude { get; set; }
    //        public string Cidade { get; set; }
    //    }

    //    public class PessoaContato
    //    {
    //        [JsonConverter(typeof(EncryptIdJSONConverter))]
    //        public int Id { get; set; }
    //        [JsonConverter(typeof(Int32JSONConverter))]
    //        public int Tipo { get; set; }
    //        public string TipoNome { get; set; }
    //        public string Contato { get; set; }
    //    }

    //    public class PessoaTipo
    //    {
    //        [JsonConverter(typeof(EncryptIdJSONConverter))]
    //        public int Id { get; set; }
    //        [JsonConverter(typeof(Int32JSONConverter))]
    //        public int Tipo { get; set; }
    //        public string TipoNome { get; set; }
    //    }
    //}
}
