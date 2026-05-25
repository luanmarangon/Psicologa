using Org.BouncyCastle.Bcpg.OpenPgp;
using Shared.Infra.CrossCutting.JSONConverter;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Psicologa.Application.Dashboard.ViewsModel
{
    //public class IndicadoresAdmin
    //{
    //    public string Perfil { get; set; }

    //    [JsonConverter(typeof(Int32JSONConverter))]
    //    public int QuantidadeClientes { get; set; }
    //    [JsonConverter(typeof(Int32JSONConverter))]
    //    public int QuantidadeColaboradores { get; set; }
    //    [JsonConverter(typeof(Int32JSONConverter))]
    //    public int QuantidadePsicologos { get; set; }
    //    [JsonConverter(typeof(Int32JSONConverter))]
    //    public int QuantidadeBlogs { get; set; }
    //    [JsonConverter(typeof(Int32JSONConverter))]
    //    public int QuantidadeBlogsPublicado { get; set; }
    //    [JsonConverter(typeof(Int32JSONConverter))]
    //    public int QuantidadeUsuarios { get; set; }
    //    [JsonConverter(typeof(Int32JSONConverter))]
    //    public int QuantidadeServicosAtivos { get; set; }

    //    public List<IndicadorItemVM> Master { get; set; }
    //    public IndicadorSaudacao Saudacao { get; set; }
    //}

    //public class IndicadoresAdministrativo
    //{

    //    public List<IndicadorItemVM> Administrativo { get; set; }
    //    public IndicadorSaudacao Saudacao { get; set; }
    //}

    //public class IndicadoresPsicologo
    //{

    //    public List<IndicadorItemVM> Psicologo { get; set; }
    //    public IndicadorSaudacao Saudacao { get; set; }
    //}

    //public class IndicadorItemVM
    //{
    //    public string Label { get; set; }
    //    public string Icon { get; set; }
    //    public string Gradient { get; set; }
    //    public int Value { get; set; }
    //}

    //public class IndicadorSaudacao
    //{
    //    public string Titulo { get; set; }
    //    public string SubTitulo { get; set; }
    //}


    public class IndicadoresVM
    {
        /// <summary>
        /// Perfil do usuário logado
        /// Ex: Administrador, Psicólogo, Cliente
        /// </summary>
        public string Perfil { get; set; }

        /// <summary>
        /// Tipo do dashboard
        /// Ex: ADMIN, PSICOLOGO, CLIENTE
        /// </summary>
        public string TipoDashboard { get; set; }

        /// <summary>
        /// Saudação/título da página
        /// </summary>
        public IndicadorSaudacaoVM Saudacao { get; set; }

        /// <summary>
        /// Lista dinâmica de indicadores
        /// </summary>
        public List<IndicadorItemVM> Indicadores { get; set; } = new();
    }

    public class IndicadorItemVM
    {
        /// <summary>
        /// Nome do indicador
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Valor do indicador
        /// </summary>
        [JsonConverter(typeof(Int32JSONConverter))]
        public int Value { get; set; }

        /// <summary>
        /// Ícone FontAwesome
        /// Ex: fas fa-users
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Gradient CSS do card
        /// </summary>
        public string Gradient { get; set; }

        /// <summary>
        /// Link opcional ao clicar no card
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// Ordem do card na tela
        /// </summary>
        public int Ordem { get; set; }

        /// <summary>
        /// Exibe ou não o card
        /// </summary>
        public bool Exibir { get; set; } = true;
    }

    public class IndicadorSaudacaoVM
    {
        /// <summary>
        /// Título principal
        /// </summary>
        public string Titulo { get; set; }

        /// <summary>
        /// Subtítulo/descrição
        /// </summary>
        public string SubTitulo { get; set; }
    }
}