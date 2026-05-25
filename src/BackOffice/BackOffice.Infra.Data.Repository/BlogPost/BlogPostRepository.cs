using Microsoft.Extensions.Logging;
using Psicologa.Domain.BlogPost.Entities;
using Psicologa.Domain.BlogPost.Interfaces.Repositories;
using Psicologa.Infra.Data.Repository.Pessoa;
using Shared.Infra.CrossCutting;
using Shared.Infra.Data.Providers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Psicologa.Infra.Data.Repository.BlogPost
{
    public class BlogPostRepository : RepositoryBase<Domain.BlogPost.Entities.BlogPost>, IBlogPostRepository
    {
        private readonly ILogger<BlogPostRepository> _logger;

        public BlogPostRepository(IDBContextFactory dbContextFactory, ILogger<BlogPostRepository> logger)
            : base(dbContextFactory)
        {
            _logger = logger;
        }

        public bool Salvar(Domain.BlogPost.Entities.BlogPost blogPost)
        {
            bool operacao = false;

            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    if (blogPost.Id == 0)
                    {
                        cmd.CommandText = @"INSERT INTO BlogPost (Titulo, Url, Conteudo, Resumo, ImagemCapa, Autor, DataCriacao, DataAtualizacao, DataPublicacao, DataRevogacao, Ativo, PessoaId)
                                            VALUES (@Titulo, @Url, @Conteudo, @Resumo, @ImagemCapa, @Autor, @DataCriacao, @DataAtualizacao, @DataPublicacao, @DataRevogacao, @Ativo, @PessoaId);";
                    }
                    else
                    {
                        cmd.CommandText = @"UPDATE BlogPost SET
                                                    Titulo = @Titulo,
                                                    Url = @Url,
                                                    Conteudo = @Conteudo,
                                                    Resumo = @Resumo,
                                                    ImagemCapa = @ImagemCapa,
                                                    Autor = @Autor,
                                                    DataCriacao = @DataCriacao,
                                                    DataAtualizacao = @DataAtualizacao,
                                                    DataPublicacao = @DataPublicacao,
                                                    DataRevogacao = @DataRevogacao,
                                                    Ativo = @Ativo,
                                                    PessoaId = @PessoaId
                                            WHERE BlogPostId = @Id;";
                        cmd.ParameterAdd("@Id", blogPost.Id);
                    }

                    cmd.ParameterAdd("@Titulo", blogPost.Titulo);
                    cmd.ParameterAdd("@Url", blogPost.Url);
                    cmd.ParameterAdd("@Conteudo", blogPost.Conteudo);
                    cmd.ParameterAdd("@Resumo", blogPost.Resumo);
                    cmd.ParameterAdd("@ImagemCapa", blogPost.ImagemCapa);
                    cmd.ParameterAdd("@Autor", blogPost.Autor);
                    cmd.ParameterAdd("@DataCriacao", blogPost.DataCriacao);
                    cmd.ParameterAdd("@DataAtualizacao", blogPost.DataAtualizacao);
                    cmd.ParameterAdd("@DataPublicacao", blogPost.DataPublicacao);
                    cmd.ParameterAdd("@DataRevogacao", blogPost.DataRevogacao == DateTime.MinValue ? DBNull.Value : blogPost.DataRevogacao);
                    cmd.ParameterAdd("@Ativo", blogPost.Ativo);
                    cmd.ParameterAdd("@PessoaId", blogPost.Pessoa.Id);

                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        if (blogPost.Id == 0)
                        {
                            cmd.ParametersClear();
                            cmd.CommandText = "select LAST_INSERT_ID();";
                            blogPost.Id = Convert.ToInt32(cmd.ExecuteScalar());
                        }
                        operacao = true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar BlogPost");
            }

            return operacao;
        }

        //public IEnumerable<Domain.BlogPost.Entities.BlogPost> Consultar(string termo, PaginacaoDados paginacao)
        //{
        //    IEnumerable<Domain.BlogPost.Entities.BlogPost> blogPosts = new List<Domain.BlogPost.Entities.BlogPost>();

        //    try
        //    {
        //        int pular = 0;
        //        if (paginacao.PaginaAtual > 0)
        //            pular = paginacao.PaginaAtual * paginacao.TamanhoPagina;

        //        if (pular < 0)
        //            pular = 0;

        //        using (var cmd = DbContext.CreateCommand())
        //        {
        //            cmd.CommandText = $@"
        //                            SELECT b.BlogPostId, b.Titulo, b.Url, b.Conteudo, b.Resumo, b.ImagemCapa,
        //                                   b.Autor, b.DataCriacao, b.DataAtualizacao, b.DataPublicacao,
        //                                   b.DataRevogacao, b.Ativo, p.PessoaId, p.Nome
        //                            FROM blogPost b
        //                            JOIN Pessoa p ON b.PessoaId = p.PessoaId
        //                            WHERE b.Titulo LIKE @Termo OR b.Resumo LIKE @Termo OR p.Nome LIKE @Termo OR b.Conteudo LIKE @Termo
        //                            ORDER BY b.Titulo
        //                            LIMIT {pular}, {paginacao.TamanhoPagina};";

        //            cmd.ParametersClear();
        //            cmd.ParameterAdd("@Termo", "%" + termo.Trim() + "%");

        //            using (var dr = cmd.ExecuteReader())
        //            {
        //                blogPosts = Map(dr);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Erro ao consultar BlogPost");
        //    }
        //    return blogPosts;
        //}

        public IEnumerable<Domain.BlogPost.Entities.BlogPost> Consultar(string termo, PaginacaoDados paginacao)
        {
            IEnumerable<Domain.BlogPost.Entities.BlogPost> blogPosts = new List<Domain.BlogPost.Entities.BlogPost>();

            try
            {
                int pular = 0;
                if (paginacao.PaginaAtual > 0)
                    pular = paginacao.PaginaAtual * paginacao.TamanhoPagina;

                if (pular < 0)
                    pular = 0;

                using (var cmd = DbContext.CreateCommand())
                {
                   string consultaPrincipal = $@"
                                    SELECT b.BlogPostId, b.Titulo, b.Url, b.Conteudo, b.Resumo, b.ImagemCapa,
                                           b.Autor, b.DataCriacao, b.DataAtualizacao, b.DataPublicacao,
                                           b.DataRevogacao, b.Ativo, p.PessoaId, p.Nome
                                    FROM blogPost b
                                    JOIN Pessoa p ON b.PessoaId = p.PessoaId
                                    WHERE b.Titulo LIKE @Termo || b.Resumo LIKE @Termo || b.Autor LIKE @Termo || b.Conteudo LIKE @Termo
                                    ORDER BY b.Titulo
                                    #paginacaoFiltro";

                    cmd.CommandText = $"select count(*) from ({consultaPrincipal.Replace("#paginacaoFiltro", "")}) as t";

                    cmd.ParametersClear();
                    cmd.ParameterAdd("@Termo", "%" + termo.Trim() + "%");

                    paginacao.TotalItens = Convert.ToInt32(cmd.ExecuteScalar());
                    string paginacaoFiltro = $@" limit {pular},{paginacao.TamanhoPagina}";
                    cmd.CommandText = $@"{consultaPrincipal.Replace("#paginacaoFiltro", paginacaoFiltro)}";

                    using (var dr = cmd.ExecuteReader())
                    {
                        blogPosts = Map(dr);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar BlogPost");
            }
            return blogPosts;
        }

        public IEnumerable<Domain.BlogPost.Entities.BlogPost> ConsultarUltimos(int quantidade)
        {
            IEnumerable<Domain.BlogPost.Entities.BlogPost> blogPosts = new List<Domain.BlogPost.Entities.BlogPost>();
            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText = $@"
                                    SELECT b.BlogPostId, b.Titulo, b.Url, b.Conteudo, b.Resumo, b.ImagemCapa,
                                           b.Autor, b.DataCriacao, b.DataAtualizacao, b.DataPublicacao,
                                           b.DataRevogacao, b.Ativo, p.PessoaId, p.Nome
                                    FROM blogPost b
                                    JOIN Pessoa p ON b.PessoaId = p.PessoaId
                                    WHERE b.Ativo = 1 AND (b.DataPublicacao <= NOW() OR b.DataPublicacao IS NULL) AND (b.DataRevogacao > NOW() OR b.DataRevogacao IS NULL)
                                    ORDER BY b.DataPublicacao DESC
                                    LIMIT {quantidade};";
                    cmd.ParametersClear();
                    using (var dr = cmd.ExecuteReader())
                    {
                        blogPosts = Map(dr);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar últimos BlogPosts");
            }
            return blogPosts;
        }
        public IEnumerable<Domain.BlogPost.Entities.BlogPost> ObterTodosPublicados()
        {
            IEnumerable<Domain.BlogPost.Entities.BlogPost> blogPosts = new List<Domain.BlogPost.Entities.BlogPost>();
            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText = $@"
                                    SELECT b.BlogPostId, b.Titulo, b.Url, b.Conteudo, b.Resumo, b.ImagemCapa,
                                           b.Autor, b.DataCriacao, b.DataAtualizacao, b.DataPublicacao,
                                           b.DataRevogacao, b.Ativo, p.PessoaId, p.Nome
                                    FROM blogPost b
                                    JOIN Pessoa p ON b.PessoaId = p.PessoaId
                                    WHERE b.Ativo = 1 AND (b.DataPublicacao <= NOW() OR b.DataPublicacao IS NULL) AND (b.DataRevogacao > NOW() OR b.DataRevogacao IS NULL)
                                    ORDER BY b.DataPublicacao DESC;";
                    cmd.ParametersClear();
                    using (var dr = cmd.ExecuteReader())
                    {
                        blogPosts = Map(dr);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar todos os Blogs");
            }
            return blogPosts;
        }



        public Domain.BlogPost.Entities.BlogPost Obter(int blogPostId)
        {
            Domain.BlogPost.Entities.BlogPost blogPost = new Domain.BlogPost.Entities.BlogPost();

            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText = @"
                                    SELECT b.BlogPostId, b.Titulo, b.Url, b.Conteudo, b.Resumo, b.ImagemCapa,
                                           b.Autor, b.DataCriacao, b.DataAtualizacao, b.DataPublicacao,
                                           b.DataRevogacao, b.Ativo, p.PessoaId, p.Nome
                                    FROM blogPost b
                                    JOIN Pessoa p ON b.PessoaId = p.PessoaId
                                    WHERE b.BlogPostId = @Id;";

                    cmd.ParametersClear();
                    cmd.ParameterAdd("@Id", blogPostId);

                    using (var dr = cmd.ExecuteReader())
                    {
                        blogPost = Map(dr).FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter BlogPost");
            }

            return blogPost;
        }
        public Domain.BlogPost.Entities.BlogPost ObterPorUrl(string blogUrl)
        {
            Domain.BlogPost.Entities.BlogPost blogPost = new Domain.BlogPost.Entities.BlogPost();

            try
            {
                using (var cmd = DbContext.CreateCommand())
                {
                    cmd.CommandText = @"
                                    SELECT b.BlogPostId, b.Titulo, b.Url, b.Conteudo, b.Resumo, b.ImagemCapa,
                                           b.Autor, b.DataCriacao, b.DataAtualizacao, b.DataPublicacao,
                                           b.DataRevogacao, b.Ativo, p.PessoaId, p.Nome
                                    FROM blogPost b
                                    JOIN Pessoa p ON b.PessoaId = p.PessoaId
                                    WHERE b.Url = @Url;";

                    cmd.ParametersClear();
                    cmd.ParameterAdd("@Url", blogUrl);

                    using (var dr = cmd.ExecuteReader())
                    {
                        blogPost = Map(dr).FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter BlogPost");
            }

            return blogPost;
        }

        internal override IEnumerable<Domain.BlogPost.Entities.BlogPost> Map(IDataReader dr)
        {
            List<Domain.BlogPost.Entities.BlogPost> blogPosts = new List<Domain.BlogPost.Entities.BlogPost>();

            while (dr.Read())
            {
                Domain.BlogPost.Entities.BlogPost blogPost = new Domain.BlogPost.Entities.BlogPost
                {
                    Id = Convert.ToInt32(dr["BlogPostId"]),
                    Titulo = dr["Titulo"].ToString(),
                    Url = dr["Url"].ToString(),
                    Conteudo = dr["Conteudo"].ToString(),
                    Resumo = dr["Resumo"].ToString(),
                    //ImagemCapa = dr["ImagemCapa"] != DBNull.Value ? Convert.FromBase64String(dr["ImagemCapa"].ToString()) : null,
                    ImagemCapa = dr["ImagemCapa"] != DBNull.Value
                                                    ? (byte[])dr["ImagemCapa"]
                                                    : null,
                    Autor = dr["Autor"].ToString(),
                    DataCriacao = Convert.ToDateTime(dr["DataCriacao"]),
                    DataAtualizacao = Convert.ToDateTime(dr["DataAtualizacao"]),
                    DataPublicacao = dr["DataPublicacao"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dr["DataPublicacao"]),
                    DataRevogacao = dr["DataRevogacao"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(dr["DataRevogacao"]),
                    Ativo = Convert.ToBoolean(dr["Ativo"]),
                    Pessoa = new Domain.Pessoa.Entities.Pessoa
                    {
                        Id = Convert.ToInt32(dr["PessoaId"]),
                        Nome = dr["Nome"].ToString()
                    }
                };
                blogPosts.Add(blogPost);
            }
            return blogPosts;
        }
    }
}