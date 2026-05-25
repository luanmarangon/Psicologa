using Shared.Domain.Cidade.Entities;
using Microsoft.Extensions.Logging;
using Shared.Infra.Data.Providers;
using System;
using System.Collections.Generic;
using System.Data;

namespace Shared.Infra.Data.Repository.Cidade
{
    public class CidadeUFRepository : RepositoryBase<CidadeUF>, Domain.Cidade.Interfaces.Repositories.ICidadeUFRepository
    {
        private readonly ILogger<CidadeUFRepository> _logger;

        public CidadeUFRepository(IDBContextFactory dbContextFactory, ILogger<CidadeUFRepository> logger)
            : base(dbContextFactory)
        {
            _logger = logger;
        }
        public IEnumerable<CidadeUF> ObterTodos()
        {
            IEnumerable<CidadeUF> ufs = new List<CidadeUF>();

            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText = @"select *
                                        from CidadeUF
                                        order by Nome";

                    using (var dr = cmd.ExecuteReader())
                    {
                        ufs = Map(dr);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter os UFs.");
            }

            return ufs;
        }
 

        internal override IEnumerable<CidadeUF> Map(IDataReader dr)
        {
            List<CidadeUF> ufs = new List<CidadeUF>();

            while (dr.Read())
            {
                CidadeUF uf = new CidadeUF();
                uf.Id = Convert.ToInt32(dr["id"]);
                uf.Nome = dr["nome"].ToString();
                ufs.Add(uf);
            }

            return ufs;
        }
         

        public override void Dispose()
        {
            base.Dispose();
        }

       
    }
}
