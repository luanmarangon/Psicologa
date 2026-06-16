using Microsoft.Extensions.Logging;
using Psicologa.Domain.Psicologo.Interfaces.Repositories;
using Psicologa.Domain.Servico.Entities;
using Psicologa.Infra.Data.Repository.Paciente;
using Shared.Infra.Data.Providers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Psicologa.Infra.Data.Repository.Psicologo
{
    public class PsicologoRepository : RepositoryBase<Domain.Psicologo.Entities.Psicologo>, IPsicologoRepository
    {
        private readonly ILogger<PsicologoRepository> _logger;

        public PsicologoRepository(IDBContextFactory dbContextFactory, ILogger<PsicologoRepository> logger)
            : base(dbContextFactory)
        {
            _logger = logger;
        }
                
        public bool Salvar(Domain.Psicologo.Entities.Psicologo psicologo)
        {
            bool operacao = false;
            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    if (psicologo.Id == 0)
                    {
                        cmd.CommandText = @"INSERT INTO Psicologo (PessoaId, Crp, CrpUf, DataEmissaoCrp, Ativo, DataCriacao, DataAtualizacao) 
                                            VALUES (@PessoaId, @Crp, @CrpUf, @DataEmissaoCrp, @Ativo, @DataCriacao, @DataAtualizacao);";
                    }
                    else
                    {
                        cmd.CommandText = @"UPDATE Psicologo SET PessoaId = @PessoaId, 
                                            Crp = @Crp, 
                                            CrpUf = @CrpUf, 
                                            DataEmissaoCrp = @DataEmissaoCrp,
                                            Ativo = @Ativo, 
                                            DataAtualizacao = @DataAtualizacao 
                                            WHERE Id = @Id";
                        cmd.ParameterAdd("@Id", psicologo.Id);
                    }
                    cmd.ParameterAdd("@PessoaId", psicologo.PessoaId);
                    cmd.ParameterAdd("@Crp", psicologo.Crp);
                    cmd.ParameterAdd("@CrpUf", psicologo.CrpUf);
                    cmd.ParameterAdd("@DataEmissaoCrp", psicologo.DataEmissaoCrp);
                    cmd.ParameterAdd("@Ativo", psicologo.Ativo);
                    cmd.ParameterAdd("@DataCriacao", psicologo.DataCriacao);
                    cmd.ParameterAdd("@DataAtualizacao", psicologo.DataAtualizacao);

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        if(psicologo.Id == 0)
                        {
                            cmd.ParametersClear();
                            cmd.CommandText = "SELECT LAST_INSERT_ID();";
                            psicologo.Id = Convert.ToInt32(cmd.ExecuteScalar());
                        }
                        operacao = true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar psicólogo com ID {PsicologoId}", psicologo.Id);
            }
            return operacao;
        }

        public Domain.Psicologo.Entities.Psicologo Obter(int id)
        {
            Domain.Psicologo.Entities.Psicologo psicologo = new Domain.Psicologo.Entities.Psicologo();
            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM Psicologo WHERE PsicologoId = @PsicologoId;";
                    cmd.ParameterAdd("@PsicologoId", id);
                    using (var dr = cmd.ExecuteReader())
                    {
                        psicologo = Map(dr).FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter o serviço por ID");
            }
            return psicologo;
        }

        public Domain.Psicologo.Entities.Psicologo ObterPorPessoaId(int pessoaId)
        {
            Domain.Psicologo.Entities.Psicologo psicologo = new Domain.Psicologo.Entities.Psicologo();
            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM Psicologo WHERE PessoaId = @PessoaId;";
                    cmd.ParameterAdd("@PessoaId", pessoaId);
                    using (var dr = cmd.ExecuteReader())
                    {
                        psicologo = Map(dr).FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter o serviço por ID");
            }
            return psicologo;
        }


        internal override IEnumerable<Domain.Psicologo.Entities.Psicologo> Map(IDataReader dr)
        {
            List<Domain.Psicologo.Entities.Psicologo> psicologos = new List<Domain.Psicologo.Entities.Psicologo>();
            while (dr.Read())
            {
                psicologos.Add(new Domain.Psicologo.Entities.Psicologo
                {
                    Id = Convert.ToInt32(dr["PsicologoId"]),
                    PessoaId = Convert.ToInt32(dr["PessoaId"]),
                    Crp = dr["Crp"].ToString(),
                    CrpUf = dr["CrpUf"].ToString(),
                    DataEmissaoCrp = Convert.ToDateTime(dr["DataEmissaoCrp"]),
                    Ativo = Convert.ToBoolean(dr["Ativo"]),
                    DataCriacao = Convert.ToDateTime(dr["DataCriacao"]),
                    DataAtualizacao = Convert.ToDateTime(dr["DataAtualizacao"])
                });
            }
            return psicologos;
        }



    }
}
