using Psicologa.Application.Dashboard.ViewsModel;
using System;
using System.Collections.Generic;

namespace Psicologa.Application.Dashboard.Services
{
    public class ApplicationIndicadorService : IDisposable
    {
        private readonly Domain.Dashboard.Services.IndicadorService _indicadorService;

        public ApplicationIndicadorService(Domain.Dashboard.Services.IndicadorService indicadorService)
        {
            _indicadorService = indicadorService;
        }

        public IndicadoresVM ObterIndicadoresMaster()
        {
            return new IndicadoresVM
            {
                Perfil = "Master",
                TipoDashboard = "MASTER",

                Saudacao = new IndicadorSaudacaoVM
                {
                    Titulo = "Visão Geral da Plataforma",
                    SubTitulo = "Acompanhe os indicadores globais."
                },

                Indicadores = new List<IndicadorItemVM>
                {
                    new IndicadorItemVM
                    {
                        Label = "Psicólogos",
                        Value = _indicadorService.ObterQuantidadePsicologos(),
                        Icon = "fas fa-brain",
                        Gradient = "linear-gradient(135deg, #4bc0c0 0%, #2c8b8b 100%)",
                        Ordem = 1
                    },

                    new IndicadorItemVM
                    {
                        Label = "Colaboradores",
                        Icon = "fas fa-headset",
                        Gradient = "linear-gradient(135deg, #9B59B6 0%, #8E44AD 100%)",
                        Value = _indicadorService.ObterQuantidadeColaboradores()
                    },

                    new IndicadorItemVM
                    {
                        Label = "Clientes",
                        Value = _indicadorService.ObterQuantidadeClientes(),
                        Icon = "fas fa-users",
                        Gradient = "linear-gradient(135deg, #3498DB 0%, #2980B9 100%)",
                        Ordem = 2
                    },

                    new IndicadorItemVM
                    {
                        Label = "Blogs",
                        Value = _indicadorService.ObterQuantidadeBlogs(),
                        Icon = "fas fa-newspaper",
                        Gradient = "linear-gradient(135deg, #FFA500 0%, #FF8C00 100%)",
                        Ordem = 3
                    },

                    new IndicadorItemVM
                    {
                        Label = "Blogs publicados",
                        Value = _indicadorService.ObterQuantidadeBlogsPublicados(),
                        Icon = "fas fa-check-circle",
                        Gradient = "linear-gradient(135deg, #e74c3c 0%, #c0392b 100%)",
                        Ordem = 4
                    },

                    new IndicadorItemVM
                    {
                        Label = "Usuários",
                        Value = _indicadorService.ObterQuantidadeUsuarios(),
                        Icon = "fas fa-users-cog",
                        Gradient = "linear-gradient(135deg, #2ecc71 0%, #27ae60 100%)",
                        Ordem = 5
                    },

                    new IndicadorItemVM
                    {
                        Label = "Serviços Ativos",
                        Value = _indicadorService.ObterQuantidadeServicosAtivos(),
                        Icon = "fas fa-cogs",
                        Gradient = "linear-gradient(135deg, #4bc0c0 0%, #2c8b8b 100%)",
                        Ordem = 6
                    },

                    new IndicadorItemVM
                    {
                        Label = "Agendamentos (Hoje)",
                        Value = _indicadorService.ObterQuantidadeAgendamentosAgendadoHoje(),
                        Icon = "fas fa-calendar-check",
                        Gradient = "linear-gradient(135deg, #f39c12 0%, #d35400 100%)",
                        Ordem = 7
                    },
                }
            };
        }

        public IndicadoresVM ObterIndicadoresAdministrativo()
        {
            return new IndicadoresVM
            {
                Perfil = "Administrativo",
                TipoDashboard = "ADMINISTRATIVO",

                Saudacao = new IndicadorSaudacaoVM
                {
                    Titulo = "Visão Geral da Plataforma",
                    SubTitulo = "Acompanhe os indicadores globais."
                },

                Indicadores = new List<IndicadorItemVM>
                {
                    new IndicadorItemVM
                    {
                        Label = "Psicólogos",
                        Value = _indicadorService.ObterQuantidadePsicologos(),
                        Icon = "fas fa-brain",
                        Gradient = "linear-gradient(135deg, #4bc0c0 0%, #2c8b8b 100%)",
                        Ordem = 1
                    },

                    new IndicadorItemVM
                    {
                        Label = "Colaboradores",
                        Icon = "fas fa-headset",
                        Gradient = "linear-gradient(135deg, #9B59B6 0%, #8E44AD 100%)",
                        Value = _indicadorService.ObterQuantidadeColaboradores()
                    },

                    new IndicadorItemVM
                    {
                        Label = "Clientes",
                        Value = _indicadorService.ObterQuantidadeClientes(),
                        Icon = "fas fa-users",
                        Gradient = "linear-gradient(135deg, #3498DB 0%, #2980B9 100%)",
                        Ordem = 2
                    },

                    new IndicadorItemVM
                    {
                        Label = "Blogs",
                        Value = _indicadorService.ObterQuantidadeBlogs(),
                        Icon = "fas fa-newspaper",
                        Gradient = "linear-gradient(135deg, #FFA500 0%, #FF8C00 100%)",
                        Ordem = 3
                    },

                    new IndicadorItemVM
                    {
                        Label = "Blogs publicados",
                        Value = _indicadorService.ObterQuantidadeBlogsPublicados(),
                        Icon = "fas fa-check-circle",
                        Gradient = "linear-gradient(135deg, #e74c3c 0%, #c0392b 100%)",
                        Ordem = 4
                    },

                    new IndicadorItemVM
                    {
                        Label = "Usuários",
                        Value = _indicadorService.ObterQuantidadeUsuarios(),
                        Icon = "fas fa-users-cog",
                        Gradient = "linear-gradient(135deg, #2ecc71 0%, #27ae60 100%)",
                        Ordem = 5
                    },

                    new IndicadorItemVM
                    {
                        Label = "Serviços Ativos",
                        Value = _indicadorService.ObterQuantidadeServicosAtivos(),
                        Icon = "fas fa-cogs",
                        Gradient = "linear-gradient(135deg, #4bc0c0 0%, #2c8b8b 100%)",
                        Ordem = 6
                    }
                }
            };
        }

        public IndicadoresVM ObterIndicadoresPsicologos(int psicologoId)
        {
            return new IndicadoresVM
            {
                Perfil = "Psicólogo",
                TipoDashboard = "PSICOLOGO",

                Saudacao = new IndicadorSaudacaoVM
                {
                    Titulo = "Visão Geral da Plataforma",
                    SubTitulo = "Acompanhe os indicadores globais."
                },

                Indicadores = new List<IndicadorItemVM>
                {
                    new IndicadorItemVM
                    {
                        Label = "Psicólogos",
                        Value = _indicadorService.ObterQuantidadePsicologos(),
                        Icon = "fas fa-brain",
                        Gradient = "linear-gradient(135deg, #4bc0c0 0%, #2c8b8b 100%)",
                        Ordem = 1
                    },

                    new IndicadorItemVM
                    {
                        Label = "Clientes",
                        Value = _indicadorService.ObterQuantidadeClientes(),
                        Icon = "fas fa-users",
                        Gradient = "linear-gradient(135deg, #3498DB 0%, #2980B9 100%)",
                        Ordem = 2
                    },

                    new IndicadorItemVM
                    {
                        Label = "Blogs",
                        Value = _indicadorService.ObterQuantidadeBlogs(),
                        Icon = "fas fa-newspaper",
                        Gradient = "linear-gradient(135deg, #FFA500 0%, #FF8C00 100%)",
                        Ordem = 3
                    },

                    new IndicadorItemVM
                    {
                        Label = "Blogs publicados",
                        Value = _indicadorService.ObterQuantidadeBlogsPublicados(),
                        Icon = "fas fa-check-circle",
                        Gradient = "linear-gradient(135deg, #e74c3c 0%, #c0392b 100%)",
                        Ordem = 4
                    },

                    new IndicadorItemVM
                    {
                        Label = "Serviços Ativos",
                        Value = _indicadorService.ObterQuantidadeServicosAtivos(),
                        Icon = "fas fa-cogs",
                        Gradient = "linear-gradient(135deg, #4bc0c0 0%, #2c8b8b 100%)",
                        Ordem = 6
                    },

                    new IndicadorItemVM
                    {
                        Label = "Agendamentos (Hoje)",
                        Value = _indicadorService.ObterQuantidadeAgendamentosAgendadoHoje(),
                        Icon = "fas fa-calendar-check",
                        Gradient = "linear-gradient(135deg, #f39c12 0%, #d35400 100%)",
                        Ordem = 7
                    },

                    new IndicadorItemVM
                    {
                        Label = "Meus Agendamentos (Hoje)",
                        Value = _indicadorService.ObterMeusAgendamentos(psicologoId),
                        Icon = "fas fa-calendar-check",
                        Gradient = "linear-gradient(135deg, #f39c12 0%, #d35400 100%)",
                    }
                }
            };
        }

        public IndicadoresVM ObterIndicador()
        {
            return new IndicadoresVM
            {
                Perfil = "Suporte",
                TipoDashboard = "SUPORTE",

                Saudacao = new IndicadorSaudacaoVM
                {
                    Titulo = "Visão Geral da Plataforma",
                    SubTitulo = "Acompanhe os indicadores globais."
                },

                Indicadores = new List<IndicadorItemVM>
                {
                    new IndicadorItemVM
                    {
                        Label = "Blogs",
                        Value = _indicadorService.ObterQuantidadeBlogs(),
                        Icon = "fas fa-newspaper",
                        Gradient = "linear-gradient(135deg, #FFA500 0%, #FF8C00 100%)",
                        Ordem = 3
                    },

                    new IndicadorItemVM
                    {
                        Label = "Blogs publicados",
                        Value = _indicadorService.ObterQuantidadeBlogsPublicados(),
                        Icon = "fas fa-check-circle",
                        Gradient = "linear-gradient(135deg, #e74c3c 0%, #c0392b 100%)",
                        Ordem = 4
                    },

                    new IndicadorItemVM
                    {
                        Label = "Serviços Ativos",
                        Value = _indicadorService.ObterQuantidadeServicosAtivos(),
                        Icon = "fas fa-cogs",
                        Gradient = "linear-gradient(135deg, #4bc0c0 0%, #2c8b8b 100%)",
                        Ordem = 6
                    }
                }
            };
        }

        //public IndicadoresAdmin ObterIndicadoresAdmin()
        //{
        //    IndicadoresAdmin i = new IndicadoresAdmin();

        //    Domain.Dashboard.Entities.Indicador indic = _indicadorService.ObterIndicadoresAdmin();

        //    return FormatarRetornoConsultaMaster(indic);
        //}

        //public IndicadoresPsicologo ObterIndicadoresPsicologo(int usuarioId)
        //{
        //    IndicadoresPsicologo i = new IndicadoresPsicologo();
        //    Domain.Dashboard.Entities.Indicador indic = _indicadorService.ObterIndicadoresPsicologo();
        //    return FormatarRetornoConsultaPsicologos(indic);
        //}

        //public IndicadoresAdministrativo ObterIndicadoresAdministrativo(int usuarioId)
        //{
        //    throw new NotImplementedException();
        //}

        //public IndicadoresAdministrativo ObterIndicadoresCliente(int usuarioId)
        //{
        //    throw new NotImplementedException();
        //}

        //internal IndicadoresAdmin FormatarRetornoConsultaMaster(Domain.Dashboard.Entities.Indicador indicador)
        //{
        //    IndicadoresAdmin i = new IndicadoresAdmin();
        //    i.QuantidadeClientes = indicador.QuantidadeClientes;
        //    i.QuantidadeColaboradores = indicador.QuantidadeColaboradores;
        //    i.QuantidadePsicologos = indicador.QuantidadePsicologos;
        //    i.QuantidadeBlogs = indicador.QuantidadeBlogs;
        //    i.QuantidadeBlogsPublicado = indicador.QuantidadeBlogsPublicado;
        //    i.QuantidadeUsuarios = indicador.QuantidadeUsuarios;
        //    i.QuantidadeServicosAtivos = indicador.QuantidadeServicosAtivos;

        //    i.Master = new List<IndicadorItemVM>
        //    {
        //        new IndicadorItemVM
        //                {
        //                    Label = "Psicólogos",
        //                    Icon = "fas fa-brain",
        //                    Gradient = "linear-gradient(135deg, #4bc0c0 0%, #2c8b8b 100%)",
        //                    Value = indicador.QuantidadePsicologos
        //    },
        //                new IndicadorItemVM
        //                {
        //                    Label = "Colaboradores",
        //                    Icon = "fas fa-headset",
        //                    Gradient = "linear-gradient(135deg, #9B59B6 0%, #8E44AD 100%)",
        //                    Value = indicador.QuantidadeColaboradores
        //},
        //                new IndicadorItemVM
        //                {
        //                    Label = "Clientes",
        //                    Icon = "fas fa-user",
        //                    Gradient = "linear-gradient(135deg, #3498DB 0%, #2980B9 100%)",
        //                    Value = indicador.QuantidadeClientes
        //                },
        //                new IndicadorItemVM
        //                {
        //                    Label = "Blogs",
        //                    Icon = "fas fa-newspaper",
        //                    Gradient = "linear-gradient(135deg, #FFA500 0%, #FF8C00 100%)",
        //                    Value = indicador.QuantidadeBlogs
        //                },
        //                new IndicadorItemVM
        //                {
        //                    Label = "Blogs publicados",
        //                    Icon = "fas fa-check-circle",
        //                    Gradient = "linear-gradient(135deg, #e74c3c 0%, #c0392b 100%)",
        //                    Value = indicador.QuantidadeBlogsPublicado
        //                },
        //                new IndicadorItemVM
        //                {
        //                    Label = "Usuários",
        //                    Icon = "fas fa-users-cog",
        //                    Gradient = "linear-gradient(135deg, #2ecc71 0%, #27ae60 100%)",
        //                    Value = indicador.QuantidadeUsuarios
        //                },
        //                new IndicadorItemVM
        //                {
        //                    Label = "Serviços Ativos",
        //                    Icon = "fas fa-brain",
        //                    Gradient = "linear-gradient(135deg, #4bc0c0 0%, #2c8b8b 100%)",
        //                    Value = indicador.QuantidadeServicosAtivos
        //                }
        //    };
        //    i.Saudacao = new IndicadorSaudacao
        //    {
        //        Titulo = "Visão Geral da Plataforma",
        //        SubTitulo = "Acompanhe os indicadores globais da plataforma."
        //    };

        //    return i;
        //}

        //internal IndicadoresPsicologo FormatarRetornoConsultaPsicologos(Domain.Dashboard.Entities.Indicador indicador)
        //{
        //    IndicadoresPsicologo i = new IndicadoresPsicologo();
        //    i.Psicologo = new List<IndicadorItemVM>
        //    {
        //         new IndicadorItemVM
        //        {
        //            Label = "Psicólogos",
        //            Icon = "fas fa-brain",
        //            Gradient = "linear-gradient(135deg, #4bc0c0 0%, #2c8b8b 100%)",
        //            Value = indicador.QuantidadePsicologos
        //        },
        //         new IndicadorItemVM
        //        {
        //            Label = "Blogs publicados",
        //            Icon = "fas fa-check-circle",
        //            Gradient = "linear-gradient(135deg, #e74c3c 0%, #c0392b 100%)",
        //            Value = indicador.QuantidadeBlogsPublicado
        //        },
        //        new IndicadorItemVM
        //        {
        //            Label = "Serviços Ativos",
        //            Icon = "fas fa-brain",
        //            Gradient = "linear-gradient(135deg, #4bc0c0 0%, #2c8b8b 100%)",
        //            Value = indicador.QuantidadeServicosAtivos
        //        }
        //    };
        //    i.Saudacao = new IndicadorSaudacao
        //    {
        //        Titulo = "Indicadores Administrativos",
        //        SubTitulo = "Acompanhe os indicadores relacionados à gestão administrativa."
        //    };
        //    return i;
        //}

        public void Dispose()
        {
        }
    }
}