using Microsoft.Extensions.Logging;
using Psicologa.Domain.Pessoa.Entities;
using Psicologa.Domain.Pessoa.Interfaces.Repositories;
using Shared.Infra.CrossCutting;
using Shared.Infra.Data.Providers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Psicologa.Infra.Data.Repository.Pessoa
{
    public class PessoaRepository : RepositoryBase<Domain.Pessoa.Entities.Pessoa>, IPessoaRepository
    {
        private readonly ILogger<PessoaRepository> _logger;

        public PessoaRepository(IDBContextFactory dbContextFactory, ILogger<PessoaRepository> logger)
            : base(dbContextFactory)
        {
            _logger = logger;
        }

        public bool Salvar(Domain.Pessoa.Entities.Pessoa pessoa)
        {
            bool operacao = false;
            using (var uow = DbContext.CreateUnitOfWork())
            {
                try
                {
                    using (var cmd = DbContext.CreateCommand())
                    {
                        int novaPessoaId = 0;

                        if (pessoa.Id == 0)
                        {
                            #region Nova pessoa

                            //Pessoa
                            cmd.CommandText = @"insert into Pessoa (Nome, DataCadastro, DataAlteracao, DocIdNro, DocIdTipo, Ativo)
                                                values (@Nome, @DataCadastro, @DataAlteracao, @DocIdNro, @DocIdTipo, @Ativo);";

                            cmd.ParameterAdd("@Nome", pessoa.Nome.Trim());
                            cmd.ParameterAdd("@DataCadastro", pessoa.DataCadastro);
                            cmd.ParameterAdd("@DataAlteracao", pessoa.DataAlteracao);
                            cmd.ParameterAdd("@DocIdNro", pessoa.DocIdNro);
                            cmd.ParameterAdd("@DocIdTipo", (int)pessoa.DocIdTipo);
                            cmd.ParameterAdd("@Ativo", pessoa.Ativo);
                            cmd.ExecuteNonQuery();

                            cmd.ParametersClear();
                            cmd.CommandText = "select LAST_INSERT_ID();";
                            novaPessoaId = Convert.ToInt32(cmd.ExecuteScalar());

                            //Endereço

                            if (pessoa.Endereco != null)
                            {
                                cmd.ParametersClear();
                                cmd.CommandText = @"insert into PessoaEndereco (Logradouro, Numero, Bairro, Cep, Complemento, Cidade, UF, PessoaId, PontoReferencia, Latitude, Longitude)
                                                values (@Logradouro, @Numero, @Bairro, @Cep, @Complemento, @Cidade, @UF, @PessoaId, @PontoReferencia, @Latitude, @Longitude);";

                                cmd.ParameterAdd("@Logradouro", DBUtils.ConvertEmptyStringToNull(pessoa.Endereco.Logradouro));
                                cmd.ParameterAdd("@Numero", DBUtils.ConvertEmptyStringToNull(pessoa.Endereco.Numero));
                                cmd.ParameterAdd("@Bairro", DBUtils.ConvertEmptyStringToNull(pessoa.Endereco.Bairro));
                                cmd.ParameterAdd("@Cep", DBUtils.ConvertEmptyStringToNull(pessoa.Endereco.CEP));
                                cmd.ParameterAdd("@Complemento", DBUtils.ConvertEmptyStringToNull(pessoa.Endereco.Complemento));
                                cmd.ParameterAdd("@Cidade", DBUtils.ConvertEmptyStringToNull(pessoa.Endereco.Cidade));
                                cmd.ParameterAdd("@UF", DBUtils.ConvertEmptyStringToNull(pessoa.Endereco.UF));
                                cmd.ParameterAdd("@PontoReferencia", DBUtils.ConvertEmptyStringToNull(pessoa.Endereco.PontoReferencia));
                                cmd.ParameterAdd("@Latitude", DBUtils.ConvertEmptyStringToNull(pessoa.Endereco.Latitude));
                                cmd.ParameterAdd("@Longitude", DBUtils.ConvertEmptyStringToNull(pessoa.Endereco.Longitude));
                                cmd.ParameterAdd("@PessoaId", novaPessoaId);

                                cmd.ExecuteNonQuery();
                                cmd.ParametersClear();
                                cmd.CommandText = "select LAST_INSERT_ID();";
                                int enderecoId = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            //Contato

                            cmd.CommandText = @"insert into PessoaContato (Tipo, Contato, Observacao, PessoaId)
                                                values (@Tipo, @Contato, @Observacao, @PessoaId)";

                            foreach (var c in pessoa.Contatos)
                            {
                                cmd.ParametersClear();
                                cmd.ParameterAdd("@Tipo", (int)c.Tipo);
                                cmd.ParameterAdd("@Contato", c.Contato.ToLower());
                                cmd.ParameterAdd("@Observacao", DBUtils.ConvertEmptyStringToNull(c.Observacao));
                                cmd.ParameterAdd("@PessoaId", novaPessoaId);
                                cmd.ExecuteNonQuery();
                            }

                            //Tipo

                            cmd.CommandText = @"insert into PessoaTipo (Tipo, PessoaId)
                                                values (@Tipo, @PessoaId)";

                            foreach (var t in pessoa.Tipos)
                            {
                                cmd.ParametersClear();
                                cmd.ParameterAdd("@Tipo", (int)t.Tipo);
                                cmd.ParameterAdd("@PessoaId", novaPessoaId);
                                cmd.ExecuteNonQuery();
                            }

                            //Dados Específicos
                            if (pessoa.GetType() == typeof(PessoaJuridica))
                            {
                                PessoaJuridica pj = (PessoaJuridica)pessoa;

                                cmd.ParametersClear();
                                cmd.CommandText = @"insert into PessoaJuridica (PessoaId, RazaoSocial)
                                                    values (@PessoaId, @RazaoSocial)";

                                cmd.ParameterAdd("@PessoaId", novaPessoaId);
                                cmd.ParameterAdd("@RazaoSocial", pj.RazaoSocial);
                                cmd.ExecuteNonQuery();
                            }
                            else
                            {
                                PessoaFisica pf = (PessoaFisica)pessoa;

                                cmd.ParametersClear();
                                cmd.CommandText = @"insert into PessoaFisica (PessoaId, DataNascimento, Sexo)
                                                    values (@PessoaId, @DataNascimento, @Sexo)";

                                cmd.ParameterAdd("@PessoaId", novaPessoaId);
                                cmd.ParameterAdd("@DataNascimento", pf.DataNascimento);
                                cmd.ParameterAdd("@Sexo", pf.Sexo);

                                cmd.ExecuteNonQuery();
                            }

                            //Tipo. 1 - Cliente. 2 - Colaborador

                            #endregion Nova pessoa
                        }
                        else
                        {
                            #region Atualiza pessoa

                            //Pessoa
                            cmd.CommandText = @"update Pessoa
                                                set Nome = @Nome,
                                                    DataAlteracao = @DataAlteracao,
                                                    DocIdNro = @DocIdNro,
                                                    DocIdTipo = @DocIdTipo,
                                                    Ativo = @Ativo
                                                where PessoaId = @Id";

                            cmd.ParameterAdd("@Nome", pessoa.Nome.Trim());
                            cmd.ParameterAdd("@DataAlteracao", pessoa.DataAlteracao);
                            cmd.ParameterAdd("@DocIdNro", pessoa.DocIdNro);
                            cmd.ParameterAdd("@DocIdTipo", (int)pessoa.DocIdTipo);
                            cmd.ParameterAdd("@Ativo", pessoa.Ativo);
                            cmd.ParameterAdd("@Id", pessoa.Id);
                            cmd.ExecuteNonQuery();

                            //Original
                            if (pessoa.Endereco != null)
                            {
                                //Endereço
                                cmd.ParametersClear();
                                cmd.CommandText = @"update PessoaEndereco
                                                set Logradouro = @Logradouro,
                                                    Numero = @Numero,
                                                    Bairro = @Bairro,
                                                    Cep = @Cep,
                                                    Complemento = @Complemento,
                                                    Cidade = @Cidade,
                                                    UF = @UF,
                                                    PontoReferencia = @PontoReferencia,
                                                    Latitude = @Latitude,
                                                    Longitude = @Longitude
                                                where PessoaId = @PessoaId and PessoaEnderecoId = @Id";

                                cmd.ParameterAdd("@Logradouro", DBUtils.ConvertEmptyStringToNull(pessoa.Endereco.Logradouro));
                                cmd.ParameterAdd("@Numero", DBUtils.ConvertEmptyStringToNull(pessoa.Endereco.Numero));
                                cmd.ParameterAdd("@Bairro", DBUtils.ConvertEmptyStringToNull(pessoa.Endereco.Bairro));
                                cmd.ParameterAdd("@Cep", DBUtils.ConvertEmptyStringToNull(pessoa.Endereco.CEP));
                                cmd.ParameterAdd("@Complemento", DBUtils.ConvertEmptyStringToNull(pessoa.Endereco.Complemento));
                                cmd.ParameterAdd("@Cidade", DBUtils.ConvertEmptyStringToNull(pessoa.Endereco.Cidade));
                                cmd.ParameterAdd("@UF", DBUtils.ConvertEmptyStringToNull(pessoa.Endereco.UF));
                                cmd.ParameterAdd("@PontoReferencia", DBUtils.ConvertEmptyStringToNull(pessoa.Endereco.PontoReferencia));
                                cmd.ParameterAdd("@Latitude", DBUtils.ConvertEmptyStringToNull(pessoa.Endereco.Latitude));
                                cmd.ParameterAdd("@Longitude", DBUtils.ConvertEmptyStringToNull(pessoa.Endereco.Longitude));
                                cmd.ParameterAdd("@PessoaId", pessoa.Id);
                                cmd.ParameterAdd("@Id", pessoa.Endereco.Id);
                                cmd.ExecuteNonQuery();
                            }

                            //contato

                            //deletando os contatos que não estão na lista.
                            if (pessoa.Contatos.Count > 0)
                            {
                                cmd.ParametersClear();
                                cmd.CommandText = @"delete from PessoaContato
                                                    where PessoaId = @PessoaId and PessoaContatoId not in (" + string.Join(",", pessoa.Contatos.Select(c => c.Id).ToArray()) + ")";
                                cmd.ParameterAdd("@PessoaId", pessoa.Id);
                                cmd.ExecuteNonQuery();
                            }
                            //novo ou atualiza

                            foreach (var c in pessoa.Contatos)
                            {
                                cmd.ParametersClear();

                                if (c.Id <= 0)
                                {
                                    cmd.CommandText = @"insert into PessoaContato (Tipo, Contato, Observacao, PessoaId)
                                                        values (@Tipo, @Contato, @Observacao, @PessoaId)";
                                }
                                else
                                {
                                    cmd.CommandText = @"update PessoaContato
                                                        set Tipo = @Tipo,
                                                            Contato = @Contato,
                                                            Observacao = @Observacao
                                                        where PessoaId = @PessoaId and PessoaContatoId = @Id";
                                }

                                cmd.ParameterAdd("@Tipo", (int)c.Tipo);
                                cmd.ParameterAdd("@Contato", c.Contato.ToLower());
                                cmd.ParameterAdd("@Observacao", DBUtils.ConvertEmptyStringToNull(c.Observacao));
                                cmd.ParameterAdd("@PessoaId", pessoa.Id);
                                cmd.ParameterAdd("@Id", c.Id);
                                cmd.ExecuteNonQuery();
                            }

                            //tipos

                            //deletando os tipos que não estão na lista.
                            if (pessoa.Tipos.Count > 0)
                            {
                                cmd.ParametersClear();
                                cmd.CommandText = @"delete from PessoaTipo
                                                    where PessoaId = @PessoaId and PessoaTipoId not in (" + string.Join(",", pessoa.Tipos.Select(p => p.Id).ToArray()) + ")";
                                cmd.ParameterAdd("@PessoaId", pessoa.Id);
                                cmd.ExecuteNonQuery();
                            }
                            //novo ou atualiza

                            foreach (var t in pessoa.Tipos)
                            {
                                cmd.ParametersClear();

                                if (t.Id <= 0)
                                {
                                    cmd.CommandText = @"insert into PessoaTipo (Tipo, PessoaId)
                                                        values (@Tipo, @PessoaId)";
                                }
                                else
                                {
                                    cmd.CommandText = @"update PessoaTipo
                                                        set Tipo = @Tipo
                                                        where  PessoaId = @PessoaId and PessoaTipoId = @Id";
                                }

                                cmd.ParameterAdd("@Tipo", (int)t.Tipo);
                                cmd.ParameterAdd("@PessoaId", pessoa.Id);
                                cmd.ParameterAdd("@Id", t.Id);
                                cmd.ExecuteNonQuery();
                            }

                            //Dados Específicos
                            if (pessoa.GetType() == typeof(PessoaJuridica))
                            {
                                PessoaJuridica pj = (PessoaJuridica)pessoa;

                                cmd.ParametersClear();
                                cmd.CommandText = @"delete from PessoaFisica where PessoaId = @PessoaId";
                                cmd.ParameterAdd("@PessoaId", pessoa.Id);
                                cmd.ExecuteNonQuery();

                                cmd.ParametersClear();
                                cmd.CommandText = @"update PessoaJuridica
                                                    set RazaoSocial = @RazaoSocial
                                                    where PessoaId = @PessoaId";

                                cmd.ParameterAdd("@RazaoSocial", pj.RazaoSocial);
                                cmd.ParameterAdd("@PessoaId", pj.Id);
                                cmd.ExecuteNonQuery();
                            }
                            else
                            {
                                PessoaFisica pf = (PessoaFisica)pessoa;

                                cmd.ParametersClear();
                                cmd.CommandText = @"delete from PessoaJuridica where PessoaId = @PessoaId";
                                cmd.ParameterAdd("@PessoaId", pessoa.Id);
                                cmd.ExecuteNonQuery();

                                cmd.ParametersClear();
                                cmd.CommandText = @"update PessoaFisica
                                                    set DataNascimento = @DataNascimento, Sexo = @Sexo
                                                    where PessoaId = @PessoaId";

                                cmd.ParameterAdd("@DataNascimento", pf.DataNascimento);
                                cmd.ParameterAdd("@Sexo", pf.Sexo);
                                cmd.ParameterAdd("@PessoaId", pessoa.Id);

                                cmd.ExecuteNonQuery();
                            }

                            #endregion Atualiza pessoa
                        }

                        #region Paciente
                        
                        int idAux = 0;

                        bool ehPaciente = pessoa.Tipos.Exists(p => p.Tipo == PessoaTipo.TpPessoa.Paciente);
                        
                        if(pessoa.Id == 0 && novaPessoaId > 0)
                            idAux = novaPessoaId;
                        else 
                            idAux = pessoa.Id;

                        if(ehPaciente)
                        {
                            cmd.CommandText = $@"SELECT COUNT(*) FROM Paciente where PessoaId = @PessoaId";
                            cmd.ParametersClear();
                            cmd.ParameterAdd("@PessoaId", idAux);

                            if(Convert.ToInt32(cmd.ExecuteScalar()) == 0)
                            {
                                cmd.CommandText = $@"INSERT INTO Paciente (PessoaId, Ativo) VALUES (@PessoaId, @Ativo)";
                                cmd.ParametersClear();
                                cmd.ParameterAdd("@PessoaId", idAux);
                                cmd.ParameterAdd("@Ativo", pessoa.Ativo);

                                cmd.ExecuteNonQuery();
                                }
                            else
                            {
                                cmd.CommandText = $@"UPDATE Paciente SET Ativo = @Ativo WHERE PessoaId = @PessoaId";
                                cmd.ParametersClear();
                                cmd.ParameterAdd("@PessoaId", idAux);
                                cmd.ParameterAdd("@Ativo", pessoa.Ativo);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        #endregion

                        operacao = true;

                        if (pessoa.Id == 0 && novaPessoaId > 0)
                        {
                            pessoa.Id = novaPessoaId;
                        }
                    }

                    uow.SaveChanges();
                }
                catch (Exception ex)
                {
                    uow.CancelChanges();
                    _logger.LogError(ex, "Erro ao salvar o Pessoa.");
                }
            }

            return operacao;
        }

        public bool SalvarPorIntegracao(Domain.Pessoa.Entities.Pessoa pessoa)
        {
            bool operacao = false;
            using (var uow = DbContext.CreateUnitOfWork())
            {
                try
                {
                    using (var cmd = DbContext.CreateCommand())
                    {
                        int novaPessoaId = 0;

                        if (pessoa.Id == 0)
                        {
                            #region Nova pessoa

                            //Pessoa
                            cmd.CommandText = @"insert into Pessoa (Nome, DataCadastro, DataAlteracao, DocIdNro, DocIdTipo, Ativo)
                                                values (@Nome, @DataCadastro, @DataAlteracao, @DocIdNro, @DocIdTipo, @Ativo);";

                            cmd.ParameterAdd("@Nome", pessoa.Nome.Trim());
                            cmd.ParameterAdd("@DataCadastro", pessoa.DataCadastro);
                            cmd.ParameterAdd("@DataAlteracao", pessoa.DataAlteracao);
                            cmd.ParameterAdd("@DocIdNro", pessoa.DocIdNro);
                            cmd.ParameterAdd("@DocIdTipo", (int)pessoa.DocIdTipo);
                            cmd.ParameterAdd("@Ativo", pessoa.Ativo);
                            cmd.ExecuteNonQuery();

                            cmd.ParametersClear();
                            cmd.CommandText = "select LAST_INSERT_ID();";
                            novaPessoaId = Convert.ToInt32(cmd.ExecuteScalar());

                            //Endereço
                            cmd.ParametersClear();
                            cmd.CommandText = @"insert into PessoaEndereco (Logradouro, Numero, Bairro, Cep, Complemento, CidadeId, PessoaId, PontoReferencia, Latitude, Longitude)
                                                values (@Logradouro, @Numero, @Bairro, @Cep, @Complemento, @CidadeId, @PessoaId, @PontoReferencia, @Latitude, @Longitude);";

                            cmd.ParameterAdd("@Logradouro", DBUtils.ConvertEmptyStringToNull(pessoa.Endereco.Logradouro));
                            cmd.ParameterAdd("@Numero", DBUtils.ConvertEmptyStringToNull(pessoa.Endereco.Numero));
                            cmd.ParameterAdd("@Bairro", DBUtils.ConvertEmptyStringToNull(pessoa.Endereco.Bairro));
                            cmd.ParameterAdd("@Cep", DBUtils.ConvertEmptyStringToNull(pessoa.Endereco.CEP));
                            cmd.ParameterAdd("@Complemento", DBUtils.ConvertEmptyStringToNull(pessoa.Endereco.Complemento));
                            cmd.ParameterAdd("@Cidade", DBUtils.ConvertEmptyStringToNull(pessoa.Endereco.Cidade));
                            cmd.ParameterAdd("@UF", DBUtils.ConvertEmptyStringToNull(pessoa.Endereco.UF));
                            cmd.ParameterAdd("@PontoReferencia", DBUtils.ConvertEmptyStringToNull(pessoa.Endereco.PontoReferencia));
                            cmd.ParameterAdd("@Latitude", DBUtils.ConvertEmptyStringToNull(pessoa.Endereco.Latitude));
                            cmd.ParameterAdd("@Longitude", DBUtils.ConvertEmptyStringToNull(pessoa.Endereco.Longitude));
                            cmd.ParameterAdd("@PessoaId", novaPessoaId);

                            cmd.ExecuteNonQuery();
                            cmd.ParametersClear();
                            cmd.CommandText = "select LAST_INSERT_ID();";
                            int enderecoId = Convert.ToInt32(cmd.ExecuteScalar());

                            //Contato

                            cmd.CommandText = @"insert into PessoaContato (Tipo, Contato, Observacao, PessoaId)
                                                values (@Tipo, @Contato, @Observacao, @PessoaId)";

                            foreach (var c in pessoa.Contatos)
                            {
                                cmd.ParametersClear();
                                cmd.ParameterAdd("@Tipo", (int)c.Tipo);
                                cmd.ParameterAdd("@Contato", c.Contato.ToLower());
                                cmd.ParameterAdd("@Observacao", DBUtils.ConvertEmptyStringToNull(c.Observacao));
                                cmd.ParameterAdd("@PessoaId", novaPessoaId);
                                cmd.ExecuteNonQuery();
                            }

                            //Tipo

                            cmd.CommandText = @"insert into PessoaTipo (Tipo, PessoaId)
                                                values (@Tipo, @PessoaId)";

                            foreach (var t in pessoa.Tipos)
                            {
                                cmd.ParametersClear();
                                cmd.ParameterAdd("@Tipo", (int)t.Tipo);
                                cmd.ParameterAdd("@PessoaId", novaPessoaId);
                                cmd.ExecuteNonQuery();
                            }

                            PessoaJuridica pj = (PessoaJuridica)pessoa;

                            cmd.ParametersClear();
                            cmd.CommandText = @"insert into PessoaJuridica (PessoaId, RazaoSocial)
                                                    values (@PessoaId, @RazaoSocial)";

                            cmd.ParameterAdd("@PessoaId", novaPessoaId);
                            cmd.ParameterAdd("@RazaoSocial", pj.RazaoSocial);
                            cmd.ExecuteNonQuery();

                            //Tipo. 1 - Cliente. 2 - Colaborador

                            #endregion Nova pessoa
                        }
                        else
                        {
                            #region Atualiza pessoa

                            //Pessoa
                            cmd.CommandText = @"update Pessoa
                                                set Nome = @Nome,
                                                    DataAlteracao = @DataAlteracao,
                                                    DocIdNro = @DocIdNro,
                                                    DocIdTipo = @DocIdTipo,
                                                    Ativo = @Ativo
                                                where PessoaId = @Id";

                            cmd.ParameterAdd("@Nome", pessoa.Nome.Trim());
                            cmd.ParameterAdd("@DataAlteracao", pessoa.DataAlteracao);
                            cmd.ParameterAdd("@DocIdNro", pessoa.DocIdNro);
                            cmd.ParameterAdd("@DocIdTipo", (int)pessoa.DocIdTipo);
                            cmd.ParameterAdd("@Ativo", pessoa.Ativo);
                            cmd.ParameterAdd("@Id", pessoa.Id);
                            cmd.ExecuteNonQuery();

                            //contato

                            //deletando os contatos que não estão na lista.
                            if (pessoa.Contatos.Count > 0)
                            {
                                cmd.ParametersClear();
                                cmd.CommandText = @"delete from PessoaContato
                                                    where PessoaId = @PessoaId";
                                cmd.ParameterAdd("@PessoaId", pessoa.Id);
                                cmd.ExecuteNonQuery();
                            }
                            //novo ou atualiza

                            foreach (var c in pessoa.Contatos)
                            {
                                cmd.ParametersClear();

                                cmd.CommandText = @"insert into PessoaContato (Tipo, Contato, Observacao, PessoaId)
                                                        values (@Tipo, @Contato, @Observacao, @PessoaId)";

                                cmd.ParameterAdd("@Tipo", (int)c.Tipo);
                                cmd.ParameterAdd("@Contato", c.Contato.ToLower());
                                cmd.ParameterAdd("@Observacao", DBUtils.ConvertEmptyStringToNull(c.Observacao));
                                cmd.ParameterAdd("@PessoaId", pessoa.Id);
                                cmd.ParameterAdd("@Id", c.Id);
                                cmd.ExecuteNonQuery();
                            }

                            //tipos

                            //deletando os tipos que não estão na lista.
                            if (pessoa.Tipos.Count > 0)
                            {
                                cmd.ParametersClear();
                                cmd.CommandText = @"delete from PessoaTipo
                                                    where PessoaId = @PessoaId and PessoaTipoId not in (" + string.Join(",", pessoa.Tipos.Select(p => p.Id).ToArray()) + ")";
                                cmd.ParameterAdd("@PessoaId", pessoa.Id);
                                cmd.ExecuteNonQuery();
                            }
                            //novo ou atualiza

                            foreach (var t in pessoa.Tipos)
                            {
                                cmd.ParametersClear();

                                if (t.Id <= 0)
                                {
                                    cmd.CommandText = @"insert into PessoaTipo (Tipo, PessoaId)
                                                        values (@Tipo, @PessoaId)";
                                }
                                else
                                {
                                    cmd.CommandText = @"update PessoaTipo
                                                        set Tipo = @Tipo
                                                        where  PessoaId = @PessoaId and PessoaTipoId = @Id";
                                }

                                cmd.ParameterAdd("@Tipo", (int)t.Tipo);
                                cmd.ParameterAdd("@PessoaId", pessoa.Id);
                                cmd.ParameterAdd("@Id", t.Id);
                                cmd.ExecuteNonQuery();
                            }

                            //Dados Específicos

                            PessoaJuridica pj = (PessoaJuridica)pessoa;

                            cmd.ParametersClear();
                            cmd.CommandText = @"delete from PessoaFisica where PessoaId = @PessoaId";
                            cmd.ParameterAdd("@PessoaId", pessoa.Id);
                            cmd.ExecuteNonQuery();

                            cmd.ParametersClear();
                            cmd.CommandText = @"update PessoaJuridica
                                                    set RazaoSocial = @RazaoSocial
                                                    where PessoaId = @PessoaId";

                            cmd.ParameterAdd("@RazaoSocial", pj.RazaoSocial);
                            cmd.ParameterAdd("@PessoaId", pj.Id);
                            cmd.ExecuteNonQuery();

                            #endregion Atualiza pessoa
                        }

                        #region Novo cliente

                        int idAux = 0;

                        bool ehCliente = pessoa.Tipos.Exists(p => p.Tipo == PessoaTipo.TpPessoa.Paciente);
                        if (pessoa.Id == 0 && novaPessoaId > 0)
                        {
                            idAux = novaPessoaId;
                        }
                        else idAux = pessoa.Id;

                        if (ehCliente)
                        {
                            cmd.CommandText = @"select count(*)
                                                from Cliente
                                                where PessoaId = @PessoaId";
                            cmd.ParametersClear();
                            cmd.ParameterAdd("@PessoaId", idAux);

                            if (Convert.ToInt32(cmd.ExecuteScalar()) == 0)
                            {
                                cmd.CommandText = @"insert into Cliente (PessoaId)
                                                    values (@PessoaId)";

                                cmd.ParametersClear();
                                cmd.ParameterAdd("@PessoaId", idAux);
                                cmd.ExecuteNonQuery();
                            }
                            else
                            {
                                cmd.CommandText = @"update Cliente
                                                    set DataExclusao = null
                                                    where PessoaId = @PessoaId";

                                cmd.ParametersClear();
                                cmd.ParameterAdd("@PessoaId", idAux);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            cmd.CommandText = @"update Cliente
                                                set DataExclusao = now()
                                                where PessoaId = @PessoaId";

                            cmd.ParametersClear();
                            cmd.ParameterAdd("@PessoaId", idAux);
                            cmd.ExecuteNonQuery();
                        }

                        #endregion Novo cliente

                        operacao = true;

                        if (pessoa.Id == 0 && novaPessoaId > 0)
                        {
                            pessoa.Id = novaPessoaId;
                        }
                    }

                    uow.SaveChanges();
                }
                catch (Exception ex)
                {
                    uow.CancelChanges();
                    _logger.LogError(ex, "Erro ao salvar o Pessoa.");
                }
            }

            return operacao;
        }

        public Domain.Pessoa.Entities.Pessoa Obter(int id)
        {
            Domain.Pessoa.Entities.Pessoa p = null;

            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText =
                    $@"select p.*, pj.*, pf.*,
                            pc.PessoaContatoId, pc.Contato, pc.Tipo as ContatoTipo, pc.Observacao as ContatoObservacao,
                            e.PessoaEnderecoId, e.Bairro as EnderecoBairro, e.Cep as EnderecoCep, e.Complemento as EnderecoComplemento, e.Logradouro as EnderecoLogradouro, e.Numero as EnderecoNumero, e.PontoReferencia as EnderecoPontoReferencia, e.Latitude as EnderecoLatitude, e.Longitude as EnderecoLongitude,
                            e.Cidade, e.UF,
                            pt.PessoaTipoId as PessoaTipoId, pt.Tipo as PessoaTipoTipo
                        from
                            Pessoa p
                            left outer join PessoaJuridica pj on p.PessoaId = pj.PessoaId
                            left outer join PessoaFisica pf on p.PessoaId = pf.PessoaId
                            left outer join PessoaContato pc on p.PessoaId = pc.PessoaId
                            left outer join PessoaTipo pt on p.PessoaId = pt.PessoaId
                            left outer join PessoaEndereco e on p.PessoaId = e.PessoaId
                        where p.PessoaId = @Id";

                    cmd.ParametersClear();
                    cmd.ParameterAdd("@Id", id);

                    using (var dr = cmd.ExecuteReader())
                    {
                        p = Map(dr).FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter a pessoa.");
            }
            finally
            {
            }

            return p;
        }

        public Domain.Pessoa.Entities.Pessoa ObterPorDocumentoIdentificacao(string documento)
        {
            Domain.Pessoa.Entities.Pessoa p = null;

            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText =
                    $@"select PessoaId
                       from Pessoa
                       where DocIdNro = @DocIdNro";

                    cmd.ParameterAdd("@DocIdNro", documento);

                    var pessoaId = cmd.ExecuteScalar();

                    if (pessoaId != null && pessoaId != DBNull.Value)
                    {
                        p = Obter((int)pessoaId);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter a pessoa.");
            }

            return p;
        }

        public Domain.Pessoa.Entities.Pessoa ObterPorFormaContato(string contato)
        {
            Domain.Pessoa.Entities.Pessoa p = null;

            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText =
                    $@"select PessoaId
                       from PessoaContato
                       where Contato = @Contato";

                    cmd.ParameterAdd("@Contato", contato);

                    var pessoaId = cmd.ExecuteScalar();

                    if (pessoaId != null && pessoaId != DBNull.Value)
                    {
                        p = Obter((int)pessoaId);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter a pessoa.");
            }
            finally
            {
            }

            return p;
        }

        public bool Excluir(int id)
        {
            bool operacao = false;
            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText = @"delete from Pessoa
                                        where PessoaId = @PessoaId";
                    cmd.ParameterAdd("@PessoaId", id);
                    cmd.ExecuteNonQuery();
                    cmd.ParametersClear();
                    //exclusao
                    cmd.CommandText = @"update Cliente
                                        set DataExclusao=@DataExclusao where PessoaId = @PessoaId;";
                    cmd.ParameterAdd("@PessoaId", id);
                    cmd.ParameterAdd("@DataExclusao", DateTime.Now);
                    cmd.ExecuteNonQuery();
                }
                operacao = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir a pessoa.");
            }
            finally
            {
            }

            return operacao;
        }

        public IEnumerable<Domain.Pessoa.Entities.Pessoa> Consultar(string nome, PessoaTipo.TpPessoa tpPessoa = PessoaTipo.TpPessoa.Indefinido)
        {
            IEnumerable<Domain.Pessoa.Entities.Pessoa> pessoas = new List<Domain.Pessoa.Entities.Pessoa>();

            string sqlFiltroPessoaTipo = "";

            if (tpPessoa != PessoaTipo.TpPessoa.Indefinido)
            {
                sqlFiltroPessoaTipo = " and pt.Tipo = " + (int)tpPessoa;
            }

            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText =
                    $@"select p.*, pj.*, pf.*,
                            pc.PessoaContatoId, pc.Contato, pc.Tipo as ContatoTipo, pc.Observacao as ContatoObservacao,
                            e.PessoaEnderecoId, e.Bairro as EnderecoBairro, e.Cep as EnderecoCep, e.Complemento as EnderecoComplemento, e.Logradouro as EnderecoLogradouro, e.Numero as EnderecoNumero, e.PontoReferencia as EnderecoPontoReferencia, e.Latitude as EnderecoLatitude, e.Longitude as EnderecoLongitude,
                            e.Cidade, e.UF,
                            pt.PessoaTipoId as PessoaTipoId, pt.Tipo as PessoaTipoTipo
                        from
                            Pessoa p
                            left outer join PessoaJuridica pj on p.PessoaId = pj.PessoaId
                            left outer join PessoaFisica pf on p.PessoaId = pf.PessoaId
                            left outer join PessoaContato pc on p.PessoaId = pc.PessoaId
                            left outer join PessoaTipo pt on p.PessoaId = pt.PessoaId
                            left outer join PessoaEndereco e on p.PessoaId = e.PessoaId
                        where p.Nome Like @Nome {sqlFiltroPessoaTipo}
                        order by p.Nome desc";

                    cmd.ParametersClear();
                    cmd.ParameterAdd("@Nome", "%" + nome.Trim() + "%");

                    using (var dr = cmd.ExecuteReader())
                    {
                        pessoas = Map(dr);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar pessoas.");
            }
            finally
            {
            }

            return pessoas;
        }

        public IEnumerable<Domain.Pessoa.Entities.Pessoa> ObterUltimos(int top, PessoaTipo.TpPessoa tpPessoa = PessoaTipo.TpPessoa.Indefinido)
        {
            IEnumerable<Domain.Pessoa.Entities.Pessoa> pessoas = new List<Domain.Pessoa.Entities.Pessoa>();

            string sqlFiltroPessoaTipo = "";

            if (tpPessoa != PessoaTipo.TpPessoa.Indefinido)
            {
                sqlFiltroPessoaTipo = " where pt.Tipo = " + (int)tpPessoa;
            }

            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText =
                      $@"select p.*, pj.*, pf.*,
                            pc.PessoaContatoId, pc.Contato, pc.Tipo as ContatoTipo, pc.Observacao as ContatoObservacao,
                            e.PessoaEnderecoId, e.Bairro as EnderecoBairro, e.Cep as EnderecoCep, e.Complemento as EnderecoComplemento, e.Logradouro as EnderecoLogradouro, e.Numero as EnderecoNumero, e.PontoReferencia as EnderecoPontoReferencia, e.Latitude as EnderecoLatitude, e.Longitude as EnderecoLongitude,
                            e.Cidade, e.UF,
                            pt.PessoaTipoId as PessoaTipoId, pt.Tipo as PessoaTipoTipo
                        from
                            Pessoa p
                            left outer join PessoaJuridica pj on p.PessoaId = pj.PessoaId
                            left outer join PessoaFisica pf on p.PessoaId = pf.PessoaId
                            left outer join PessoaContato pc on p.PessoaId = pc.PessoaId
                            left outer join PessoaTipo pt on p.PessoaId = pt.PessoaId
                            left outer join PessoaEndereco e on p.PessoaId = e.PessoaId
                            {sqlFiltroPessoaTipo}
                        order by p.PessoaId desc
                        limit {top}";

                    cmd.ParametersClear();

                    using (var dr = cmd.ExecuteReader())
                    {
                        pessoas = Map(dr);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar pessoas.");
            }
            finally
            {
            }

            return pessoas;
        }

        public IEnumerable<Domain.Pessoa.Entities.Pessoa> ObterTodosClientesAtivos()
        {
            IEnumerable<Domain.Pessoa.Entities.Pessoa> pessoas = new List<Domain.Pessoa.Entities.Pessoa>();

            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText =
                      $@"select p.*, pj.*, pf.*,
                            pc.PessoaContatoId, pc.Contato, pc.Tipo as ContatoTipo, pc.Observacao as ContatoObservacao,
                            e.PessoaEnderecoId, e.Bairro as EnderecoBairro, e.Cep as EnderecoCep, e.Complemento as EnderecoComplemento, e.Logradouro as EnderecoLogradouro, e.Numero as EnderecoNumero, e.PontoReferencia as EnderecoPontoReferencia, e.Latitude as EnderecoLatitude, e.Longitude as EnderecoLongitude,
                            e.Cidade, e.UF,
                            pt.PessoaTipoId as PessoaTipoId, pt.Tipo as PessoaTipoTipo
                        from
                            Pessoa p
                            left outer join PessoaJuridica pj on p.PessoaId = pj.PessoaId
                            left outer join PessoaFisica pf on p.PessoaId = pf.PessoaId
                            left outer join PessoaContato pc on p.PessoaId = pc.PessoaId
                            left outer join PessoaTipo pt on p.PessoaId = pt.PessoaId
                            left outer join PessoaEndereco e on p.PessoaId = e.PessoaId
                            where p.Ativo and pt.Tipo = {(int)PessoaTipo.TpPessoa.Paciente}
                        order by p.PessoaId desc";

                    cmd.ParametersClear();

                    using (var dr = cmd.ExecuteReader())
                    {
                        pessoas = Map(dr);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar pessoas.");
            }
            finally
            {
            }

            return pessoas;
        }

        public IEnumerable<Domain.Pessoa.Entities.Pessoa> ObterUsuariosColaboradores(int clienteIdVinculo, string termo)
        {
            IEnumerable<Domain.Pessoa.Entities.Pessoa> pessoas = new List<Domain.Pessoa.Entities.Pessoa>();

            if (termo == null)
                termo = "";

            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText =
                      $@"select p.*, pj.*, pf.*,
                            pc.PessoaContatoId, pc.Contato, pc.Tipo as ContatoTipo, pc.Observacao as ContatoObservacao,
                            e.PessoaEnderecoId, e.Bairro as EnderecoBairro, e.Cep as EnderecoCep, e.Complemento as EnderecoComplemento, e.Logradouro as EnderecoLogradouro, e.Numero as EnderecoNumero, e.PontoReferencia as EnderecoPontoReferencia, e.Latitude as EnderecoLatitude, e.Longitude as EnderecoLongitude,
                            e.Cidade, e.UF,
                            pt.PessoaTipoId as PessoaTipoId, pt.Tipo as PessoaTipoTipo
                        from
                            Pessoa p
                            left outer join PessoaJuridica pj on p.PessoaId = pj.PessoaId
                            left outer join PessoaFisica pf on p.PessoaId = pf.PessoaId
                            left outer join PessoaContato pc on p.PessoaId = pc.PessoaId
                            left outer join PessoaTipo pt on p.PessoaId = pt.PessoaId
                            left outer join PessoaEndereco e on p.PessoaId = e.PessoaId
                        where p.Ativo and pt.Tipo = {(int)PessoaTipo.TpPessoa.Psicologo} and
                              p.Nome Like @Termo and
                              u.ClienteIdVinculo = @clienteIdVinculo
                        order by p.PessoaId ASC";

                    cmd.ParameterAdd("@clienteIdVinculo", clienteIdVinculo);
                    cmd.ParameterAdd("@Termo", "%" + termo.Trim() + "%");
                    using (var dr = cmd.ExecuteReader())
                    {
                        pessoas = Map(dr);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar Usuário Colaboradores.");
            }

            return pessoas;
        }

        internal override IEnumerable<Domain.Pessoa.Entities.Pessoa> Map(IDataReader dr)
        {
            List<Domain.Pessoa.Entities.Pessoa> pessoas = new();

            while (dr.Read())
            {
                int id = Convert.ToInt32(dr["PessoaId"]);
                Domain.Pessoa.Entities.Pessoa p = pessoas.Find(i => i.Id == id);

                if (p == null)
                {
                    if ((Domain.Pessoa.Entities.Pessoa.TpDoc)Convert.ToInt32(dr["DocIdTipo"]) == Domain.Pessoa.Entities.Pessoa.TpDoc.CNPJ)
                    {
                        p = new PessoaJuridica();
                    }
                    else
                    {
                        p = new PessoaFisica();
                    }

                    p.Id = id;
                    p.Nome = dr["Nome"].ToString();
                    p.DocIdTipo = (Domain.Pessoa.Entities.Pessoa.TpDoc)Convert.ToInt32(dr["DocIdTipo"]);
                    p.DocIdNro = dr["DocIdNro"].ToString();
                    p.DataCadastro = Convert.ToDateTime(dr["DataCadastro"]);
                    p.Ativo = Convert.ToBoolean(dr["Ativo"]);

                    if (dr["DataAlteracao"] != DBNull.Value)
                        p.DataAlteracao = Convert.ToDateTime(dr["DataAlteracao"]);

                    if (p.DocIdTipo == Domain.Pessoa.Entities.Pessoa.TpDoc.CNPJ)
                    {
                        var pj = (PessoaJuridica)p;
                        pessoas.Add(pj);
                        pj.RazaoSocial = dr["RazaoSocial"].ToString();
                    }
                    else
                    {
                        var pf = (PessoaFisica)p;
                        pessoas.Add(pf);
                        pf.DataNascimento = dr["DataNascimento"] != DBNull.Value ? Convert.ToDateTime(dr["DataNascimento"]) : DateTime.MinValue;
                        pf.Sexo = dr["Sexo"] != DBNull.Value ? (PessoaFisica.TpSexo)Convert.ToInt32(dr["Sexo"]) : PessoaFisica.TpSexo.NaoEspecificado;
                    }

                    //relacionamentos 1 para 1
                    p.Endereco = new Endereco()
                    {
                        Id = dr["PessoaEnderecoId"] != DBNull.Value ? Convert.ToInt32(dr["PessoaEnderecoId"]) : 0,
                        Logradouro = dr["EnderecoLogradouro"].ToString(),
                        Numero = dr["EnderecoNumero"].ToString(),
                        Bairro = dr["EnderecoBairro"].ToString(),
                        CEP = dr["EnderecoCEP"].ToString(),
                        Complemento = dr["EnderecoComplemento"].ToString(),
                        PontoReferencia = dr["EnderecoPontoReferencia"].ToString(),
                        Latitude = dr["EnderecoLatitude"].ToString(),
                        Longitude = dr["EnderecoLongitude"].ToString(),
                        Cidade = dr["Cidade"].ToString(),
                        UF = dr["UF"].ToString(),
                        //Cidade = new Shared.Domain.Cidade.Entities.Cidade()
                        //{
                        //    Id = dr["CidadeId"] != DBNull.Value ? Convert.ToInt32(dr["CidadeId"]) : 0,
                        //    Nome = dr["CidadeNome"].ToString(),
                        //    IBGEMunicipio = dr["IBGEMunicipio"] != DBNull.Value ? Convert.ToInt32(dr["IBGEMunicipio"]) : 0,
                        //    IBGEMunicipioCompleto = dr["IBGEMunicipioCompleto"] != DBNull.Value ? Convert.ToInt32(dr["IBGEMunicipioCompleto"]) : 0,
                        //    UF = new Shared.Domain.Cidade.Entities.CidadeUF()
                        //    {
                        //        Id = dr["CidadeUFId"] != DBNull.Value ? Convert.ToInt32(dr["CidadeUFId"]) : 0,
                        //        Nome = dr["UFNome"].ToString(),
                        //        Sigla = dr["UFSigla"].ToString(),
                        //    }
                        //}
                    };
                }

                //relacionamentos * para *
                if (dr["PessoaContatoId"] != DBNull.Value)
                {
                    if (p.Contatos.FindIndex(i => i.Id == Convert.ToInt64(dr["PessoaContatoId"])) == -1)
                    {
                        p.Contatos.Add(new PessoaContato
                        {
                            Id = Convert.ToInt32(dr["PessoaContatoId"]),
                            Contato = dr["Contato"].ToString(),
                            Observacao = dr["ContatoObservacao"].ToString(),
                            Tipo = dr["ContatoTipo"] != DBNull.Value ? (PessoaContato.TpContato)Convert.ToInt32(dr["ContatoTipo"]) : PessoaContato.TpContato.Indefinido
                        });
                    }
                }

                if (dr["PessoaTipoId"] != DBNull.Value)
                {
                    if (p.Tipos.FindIndex(i => i.Id == Convert.ToInt64(dr["PessoaTipoId"])) == -1)
                    {
                        p.Tipos.Add(new PessoaTipo
                        {
                            Id = Convert.ToInt32(dr["PessoaTipoId"]),
                            Tipo = dr["PessoaTipoTipo"] != DBNull.Value ? (PessoaTipo.TpPessoa)Convert.ToInt32(dr["PessoaTipoTipo"]) : PessoaTipo.TpPessoa.Indefinido,
                        });
                    }
                }
            }

            return pessoas;
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public IEnumerable<Domain.Pessoa.Entities.Pessoa> Consultar(string nome, PaginacaoDados paginacao, PessoaTipo.TpPessoa tpPessoa = PessoaTipo.TpPessoa.Indefinido)
        {
            IEnumerable<Domain.Pessoa.Entities.Pessoa> pessoas = new List<Domain.Pessoa.Entities.Pessoa>();

            string sqlFiltroPessoaTipo = "";

            if (tpPessoa != PessoaTipo.TpPessoa.Indefinido)
            {
                sqlFiltroPessoaTipo = " and pt.Tipo = " + (int)tpPessoa;
            }

            try
            {
                int pular = 0;
                if (paginacao.PaginaAtual > 0)
                    pular = paginacao.PaginaAtual * paginacao.TamanhoPagina;

                if (pular < 0)
                    pular = 0;

                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText =
                    $@"select SQL_CALC_FOUND_ROWS p.*, pj.*, pf.*,
                            pc.PessoaContatoId, pc.Contato, pc.Tipo as ContatoTipo, pc.Observacao as ContatoObservacao,
                            e.PessoaEnderecoId, e.Bairro as EnderecoBairro, e.Cep as EnderecoCep, e.Complemento as EnderecoComplemento, e.Logradouro as EnderecoLogradouro, e.Numero as EnderecoNumero, e.PontoReferencia as EnderecoPontoReferencia, e.Latitude as EnderecoLatitude, e.Longitude as EnderecoLongitude,
                            e.Cidade,e.UF,
                            pt.PessoaTipoId as PessoaTipoId, pt.Tipo as PessoaTipoTipo
                        from
                            Pessoa p
                            left outer join PessoaJuridica pj on p.PessoaId = pj.PessoaId
                            left outer join PessoaFisica pf on p.PessoaId = pf.PessoaId
                            left outer join PessoaContato pc on p.PessoaId = pc.PessoaId
                            left outer join PessoaTipo pt on p.PessoaId = pt.PessoaId
                            left outer join PessoaEndereco e on p.PessoaId = e.PessoaId

                        where p.Nome Like @Nome {sqlFiltroPessoaTipo}
                        order by p.Nome ASC
                        limit {pular},{paginacao.TamanhoPagina}";

                    cmd.ParametersClear();
                    cmd.ParameterAdd("@Nome", "%" + nome.Trim() + "%");

                    using (var dr = cmd.ExecuteReader())
                    {
                        pessoas = Map(dr);
                        dr.Close();

                        cmd.CommandText = $@"select FOUND_ROWS()";
                        paginacao.TotalItens = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar pessoas.");
            }
            finally
            {
            }

            return pessoas;
        }

        public IEnumerable<Domain.Pessoa.Entities.Pessoa> ObterUltimos(int top, PaginacaoDados paginacao, PessoaTipo.TpPessoa tpPessoa = PessoaTipo.TpPessoa.Indefinido)
        {
            IEnumerable<Domain.Pessoa.Entities.Pessoa> pessoas = new List<Domain.Pessoa.Entities.Pessoa>();

            string sqlFiltroPessoaTipo = "";

            if (tpPessoa != PessoaTipo.TpPessoa.Indefinido)
            {
                sqlFiltroPessoaTipo = " where pt.Tipo = " + (int)tpPessoa;
            }

            try
            {
                int pular = 0;
                if (paginacao.PaginaAtual > 0)
                    pular = paginacao.PaginaAtual * paginacao.TamanhoPagina;

                if (pular < 0)
                    pular = 0;

                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText =
                      $@"select p.*, pj.*, pf.*,
                            pc.PessoaContatoId, pc.Contato, pc.Tipo as ContatoTipo, pc.Observacao as ContatoObservacao,
                            e.PessoaEnderecoId, e.Bairro as EnderecoBairro, e.Cep as EnderecoCep, e.Complemento as EnderecoComplemento, e.Logradouro as EnderecoLogradouro, e.Numero as EnderecoNumero, e.PontoReferencia as EnderecoPontoReferencia, e.Latitude as EnderecoLatitude, e.Longitude as EnderecoLongitude,
                            e.Cidade, e.UF,
                            pt.PessoaTipoId as PessoaTipoId, pt.Tipo as PessoaTipoTipo
                        from
                            Pessoa p
                            left outer join PessoaJuridica pj on p.PessoaId = pj.PessoaId
                            left outer join PessoaFisica pf on p.PessoaId = pf.PessoaId
                            left outer join PessoaContato pc on p.PessoaId = pc.PessoaId
                            left outer join PessoaTipo pt on p.PessoaId = pt.PessoaId
                            left outer join PessoaEndereco e on p.PessoaId = e.PessoaId
                            {sqlFiltroPessoaTipo}
                        order by p.PessoaId desc
                        limit {pular}, {top}";

                    cmd.ParametersClear();

                    using (var dr = cmd.ExecuteReader())
                    {
                        pessoas = Map(dr);

                        dr.Close();
                        cmd.CommandText = $@"select FOUND_ROWS()";
                        paginacao.TotalItens = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar pessoas.");
            }
            finally
            {
            }

            return pessoas;
        }
    }
}