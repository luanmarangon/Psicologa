using Shared.Infra.CrossCutting.ValidationResult;
using System.Collections.Generic;
using System.ComponentModel;


namespace Psicologa.Domain.Usuario.Entities
{
    public class PerfilUsuario : EntityBase
    {
        public enum TpPerfil
        {
            [Description("Indefinido")]
            Indefinido = 0,
            [Description("MASTER")]
            Master = 1,
            [Description("ADMINISTRATIVO")]
            Administrativo = 2,
            [Description("PSICOLOGO")]
            Psicologo = 3,
            [Description("SUPORTE")]
            Suporte = 4
        }


        public enum TpPermissao
        {
            //***Manter uma cópia em BackOffice.Application.ViewModels.PerfilUsuarioViewModal

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
            [Description("Gerenciar Pacientes")]
            GerenciarPacientes = 11,
            [Description("Gerenciar Psicólogos")]
            GerenciarPsicologos = 12,
            [Description("Gerenciar Logs Aplicação")]
            GerenciarLogsAplicacao = 13,
            [Description("Gerenciar Documentos")]
            GerenciarDocumentos = 14,

            [Description("Gerenciar Categorias/Subcategorias Materiais Educativos")]
            GerenciarCategoriasMateriaisEducativos = 100,
            [Description("Relatórios")]
            Relatorios = 20,
            [Description("Acessar Conteúdo Restrito Materiais Educativos")]
            AcessarConteudoRestritoMateriaisEducativos = 21,


        }

        public PerfilUsuario.TpPerfil Perfil { get; set; }
        //public string Perfil { get; set; }
        public List<PerfilUsuario.TpPermissao> Permissoes { get; set; }

        public PerfilUsuario()
        {
            Perfil = TpPerfil.Indefinido;
            Permissoes = new List<TpPermissao>();
        }

        public override bool Validar()
        {
            //if (Perfil == PerfilUsuario.TpPerfil.Indefinido)
            //{
            //    base.ValidationResult.Add(Message.TypeMessage.InvalidField, "Selecione um Perfil (usuário).");
            //}
            //else 
            if (Permissoes.Count == 0)
            {
                base.ValidationResult.Add(Message.TypeMessage.InvalidField, "Nenhuma permissão foi fornecida.");
            }
            else 
            {
                foreach (var item in Permissoes)
                {
                    if (item == TpPermissao.Indefinido)
                    {
                        base.ValidationResult.Add(Message.TypeMessage.InvalidField, "Erro na seleção da permissão.");
                        break;
                    }
                }
            }

            return base.ValidationResult.Count == 0;
        }
    }
}
