using Shared.Domain.Cidade.Entities;
using Shared.Domain.Cidade.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using Shared.Infra.Data.Providers;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;

namespace Shared.Infra.Data.Repository.Cidade
{
    public class CidadeRepository : RepositoryBase<Domain.Cidade.Entities.Cidade>, ICidadeRepository
    {
        private readonly ILogger<CidadeRepository> _logger;

        public CidadeRepository(IDBContextFactory dbContextFactory, ILogger<CidadeRepository> logger)
            : base(dbContextFactory)
        {
            _logger = logger;
        }

        public Domain.Cidade.Entities.Cidade ObterPorIBGECodigo(int ibge)
        {
            Domain.Cidade.Entities.Cidade cidade = null;

            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText = @"select *, u.Nome as UFNome
                                        from Cidade c, CidadeUF u
                                        where c.CidadeUFId = u.CidadeUFId  and  
                                              c.IBGEMunicipioCompleto = @ibge";
                    cmd.ParameterAdd("@ibge", ibge);

                    using (var dr = cmd.ExecuteReader())
                    {
                        cidade = Map(dr).FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter a Cidade.");
            }

            return cidade;
        }

        public Domain.Cidade.Entities.Cidade ObterPorNome(string nome)
        {
            Domain.Cidade.Entities.Cidade cidade = null;

            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText = @"select *, u.Nome as UFNome
                                        from Cidade c, CidadeUF u
                                        where c.CidadeUFId = u.CidadeUFId  and  
                                              c.Nome = @Nome";
                    cmd.ParameterAdd("@Nome", nome);

                    using (var dr = cmd.ExecuteReader())
                    {
                        cidade = Map(dr).FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter a Cidade.");
            }

            return cidade;
        }


        public Domain.Cidade.Entities.Cidade ObterPorNomeUf(string nome, string uf)
        {
            Domain.Cidade.Entities.Cidade cidade = null;

            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText = @"select *, u.Nome as UFNome
                                        from Cidade c, CidadeUF u
                                        where c.CidadeUFId = u.CidadeUFId  and  
                                              c.Nome = @Nome and u.Sigla = @Sigla";
                    cmd.ParameterAdd("@Nome", nome);
                    cmd.ParameterAdd("@Sigla", uf);

                    using (var dr = cmd.ExecuteReader())
                    {
                        cidade = Map(dr).FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter a Cidade.");
            }

            return cidade;
        }


        public IEnumerable<Domain.Cidade.Entities.Cidade> Obter(string UF)
        {
            IEnumerable<Domain.Cidade.Entities.Cidade> cidades = new List<Domain.Cidade.Entities.Cidade>();

            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText = @"select *, u.Nome as UFNome
                                        from Cidade c, CidadeUF u
                                        where c.CidadeUFId = u.CidadeUFId  and  
                                              u.Sigla = @Sigla
                                        order by c.Nome";
                    cmd.ParameterAdd("@Sigla", UF);
                    using (var dr = cmd.ExecuteReader())
                    {
                        cidades = Map(dr);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter as Cidades.");
            }

            return cidades;
        }

        public IEnumerable<Domain.Cidade.Entities.Cidade> ObterTodas()
        {
            IEnumerable<Domain.Cidade.Entities.Cidade> cidades = new List<Domain.Cidade.Entities.Cidade>();

            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText = @"select *, u.Nome as UFNome
                                        from Cidade c, CidadeUF u
                                        where c.CidadeUFId = u.CidadeUFId
                                        order by c.Nome";

                    using (var dr = cmd.ExecuteReader())
                    {
                        cidades = Map(dr);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter as Cidades.");
            }

            return cidades;
        }

        internal override IEnumerable<Domain.Cidade.Entities.Cidade> Map(IDataReader dr)
        {
            List<Domain.Cidade.Entities.Cidade> cidades = new List<Domain.Cidade.Entities.Cidade>();

            while (dr.Read())
            {
                Domain.Cidade.Entities.Cidade cidade = new Domain.Cidade.Entities.Cidade();
                cidade.Id = Convert.ToInt32(dr["CidadeId"]);
                cidade.Nome = dr["Nome"].ToString();
                cidade.IBGEMunicipio = Convert.ToInt32(dr["IBGEMunicipio"]);
                cidade.IBGEMunicipioCompleto = Convert.ToInt32(dr["IBGEMunicipioCompleto"]);
                cidade.Latitude = dr["Latitude"].ToString();
                cidade.Longitude = dr["Longitude"].ToString();

                cidade.UF = new CidadeUF()
                {
                    Id = Convert.ToInt32(dr["CidadeUFId"]),
                    Nome = dr["UFNome"].ToString(),
                    Sigla = dr["Sigla"].ToString()
                };

                cidades.Add(cidade);
            }

            return cidades;
        }
         
        public override void Dispose()
        {
            base.Dispose();
        }
        
    }
}
