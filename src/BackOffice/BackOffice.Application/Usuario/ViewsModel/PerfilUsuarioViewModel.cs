using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;
using Shared.Infra.CrossCutting.JSONConverter;

namespace Psicologa.Application.Usuario.ViewsModel
{
    public class PerfilUsuarioViewModel
    {
        
        public enum TpPermissao
        {
            //***Manter igual ao Domain.Usuario.Entities.PerfilUsuario.TpPermissao
            Indefinido = 0,
            [Description("Gerenciar Backoffice (ADM)")]
            Backoffice = 1,
            [Description("Gerenciar Pessoas")]
            GerenciarPessoas = 2,
            [Description("Gerenciar Usuários")]
            GerenciarUsuarios = 3,
            [Description("Gerenciar Permissões")]
            GerenciarPermissoes = 4,
            [Description("Acessar Conteúdo Blog")]
            AcessarConteudoBlog = 5,
            [Description("Gerenciar Configuracoes")]
            GerenciarConfiguracoes = 6,
            [Description("Gerenciar Serviços")]
            GerenciarServicos = 7,
            [Description("Gerenciar Convênios")]
            GerenciarConvenios = 8,
            [Description("Gerenciar Leads")]
            GerenciarLeads = 9,
            [Description("Gerenciar Agendamentos")]
            GerenciarAgendamentos = 10,



            [Description("Gerenciar Categorias/Subcategorias Materiais Educativos")]
            GerenciarCategoriasMateriaisEducativos = 11,
            [Description("Relatórios")]
            Relatorios = 20,
            [Description("Acessar Conteúdo Restrito Materiais Educativos")]
            AcessarConteudoRestritoMateriaisEducativos = 21,
        }


        [JsonConverter(typeof(EncryptIdJSONConverter))]
        public int Id { get; set; }
        public string Nome { get; set; }
        public List<TpPermissao> Permissoes { get; set; }
        public List<string> PermissoesNomes { get; set; }
    }
    public class PerfilUsuarioCadastroViewModel
    {
        [JsonConverter(typeof(EncryptIdJSONConverter))]
        public int Id { get; set; }
        public string Nome { get; set; }
    }
}
