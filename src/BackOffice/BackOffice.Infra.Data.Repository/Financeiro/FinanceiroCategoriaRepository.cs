using Microsoft.Extensions.Logging;
using Psicologa.Domain.Financeiro.Entities;
using Psicologa.Domain.Financeiro.Interfaces.Repositories;
using Shared.Infra.Data.Providers;
using System;
using System.Collections.Generic;
using System.Data;

namespace Psicologa.Infra.Data.Repository.Financeiro
{
    public class FinanceiroCategoriaRepository : RepositoryBase<Domain.Financeiro.Entities.FinanceiroCategoria>, IFinanceiroCategoriaRepository
    {
        private readonly ILogger<FinanceiroCategoriaRepository> _logger;

        public FinanceiroCategoriaRepository(IDBContextFactory dbContextFactory, ILogger<FinanceiroCategoriaRepository> logger)
            : base(dbContextFactory)
        {
            _logger = logger;
        }

        public IEnumerable<FinanceiroCategoria> ObterTodasCategoria(int tipo)
        {
            IEnumerable<FinanceiroCategoria> categorias = new List<FinanceiroCategoria>();
            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText = @"SELECT FinanceiroCategoriaId, Nome, Tipo, Ativo, DataCriacao, DataAtualizacao
                                        FROM FinanceiroCategoria
                                        WHERE Tipo = @tipo";
                    cmd.ParameterAdd("@tipo", tipo);
                    using (var dr = cmd.ExecuteReader())
                    {
                        categorias = Map(dr);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter todas as categorias financeiras.");
                throw;
            }
            return categorias;
        }

        internal override IEnumerable<FinanceiroCategoria> Map(IDataReader dr)
        {
            List<FinanceiroCategoria> categorias = new List<FinanceiroCategoria>();

            while (dr.Read())
            {
                int id = Convert.ToInt32(dr["FinanceiroCategoriaId"]);
                FinanceiroCategoria categoria = categorias.Find(c => c.Id == id);

                if (categoria == null)
                {
                    categoria = new FinanceiroCategoria
                    {
                        Id = id,
                        Nome = dr["Nome"].ToString(),
                        Tipo = (FinanceiroCategoria.TipoCategoria)Convert.ToInt32(dr["Tipo"]),
                        Ativo = Convert.ToBoolean(dr["Ativo"]),
                        DataCriacao = Convert.ToDateTime(dr["DataCriacao"]),
                        DataAtualizacao = Convert.ToDateTime(dr["DataAtualizacao"])
                    };
                    categorias.Add(categoria);
                }
            }
            return categorias;
        }
    }
}