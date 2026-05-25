using Psicologa.Domain;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Psicologa.Domain.LogAplicacao.Services

{
    public class LogAplicacaoService : ServiceBase<Entities.LogAplicacao>, IServiceBase<Entities.LogAplicacao>
    {
        public readonly Interfaces.Repositories.ILogAplicacaoRepository _repository;

        public LogAplicacaoService(Interfaces.Repositories.ILogAplicacaoRepository repository)
            : base(repository)
        {
            _repository = repository;
        }

        public bool Salvar(Entities.LogAplicacao log)
        {
            bool operacao = false;
            operacao = _repository.Salvar(log);
            return operacao;
        }

        public  Domain.LogAplicacao.Entities.LogAplicacao Criar(
        string[] requisicao,
        //int usuarioId,
        string entidade,
        int entidadeId,
        string operacao,
        object dadosAntes,
        object dadosDepois,
        Dictionary<string, object> dadosAlterados,
        string aplicacao,
        string metodo)
    {
        return new Domain.LogAplicacao.Entities.LogAplicacao
        {
            DataCriacao    = DateTime.Now,
            UsuarioId      = Convert.ToInt32(requisicao[4]),
            UsuarioNome    = requisicao[3],
            IP             = requisicao[0],
            UserAgent      = requisicao[1],
            Dispostivo     = requisicao[2],
            Entidade       = entidade,
            EntidadeId     = entidadeId,
            Operacao       = operacao,
            Aplicacao      = aplicacao,
            Metodo         = metodo,
            DadosAntes     = JsonSerializer.Serialize(dadosAntes),
            DadosDepois    = JsonSerializer.Serialize(dadosDepois),
            DadosAlterados = JsonSerializer.Serialize(dadosAlterados)
        };
    }

        public (string Operacao, Dictionary<string, object> Diferencas) ObterDiferencas<T>(T existente, T novo)
        {
            var diferencas = new Dictionary<string, object>();

            // Inserção — sem estado anterior
            if (existente == null)
            {
                var jsonNovo = JsonSerializer.Serialize(novo);
                var dictNovo = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonNovo);

                foreach (var prop in dictNovo)
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null) continue;
                    diferencas[prop.Key] = prop.Value;
                }

                return ("Inserção", diferencas);
            }

            // Inserção — sem estado anterior
            if (novo == null)
            {
                var jsonNovo = JsonSerializer.Serialize(existente);
                var dictNovo = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonNovo);

                foreach (var prop in dictNovo)
                {
                    if (prop.Value.ValueKind == JsonValueKind.Null) continue;
                    diferencas[prop.Key] = prop.Value;
                }

                return ("Exclusão", diferencas);
            }


            // Atualização — compara os dois estados
            var ignorar = new HashSet<string> { "Id", "DataCadastro", "DataAtualizacao", "DataCriacao", "DtCadastro", "DataAlteracao" };

            var jsonExistente = JsonSerializer.Serialize(existente);
            var dictExistente = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonExistente);
            var jsonNovo2 = JsonSerializer.Serialize(novo);
            var dictNovo2 = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonNovo2);

            foreach (var prop in dictNovo2)
            {
                if (ignorar.Contains(prop.Key)) continue;
                if (prop.Value.ValueKind == JsonValueKind.Null) continue;

                if (!dictExistente.TryGetValue(prop.Key, out var valorExistente) ||
                    valorExistente.ToString() != prop.Value.ToString())
                {
                    diferencas[prop.Key] = prop.Value;
                }
            }

            return ("Atualização", diferencas);
        }

        //public Domain.LogAplicacao.Entities.LogAplicacao GerarLog(int entidadeId, string operacao, string metodo, string dadosNovo, string dadosAntes, int usuarioId)
        //{
        //    bool isInsert = dadosAntes == null;

        //    var log = new Domain.LogAplicacao.Entities.LogAplicacao
        //    {
        //        DataCriacao = DateTime.Now,
        //        UsuarioId = usuarioId,
        //        UsuarioNome = "",
        //        Dispostivo = "",
        //        IP = "",
        //        UserAgent = "",
        //        Entidade = "Servico",
        //        EntidadeId = entidadeId,
        //        Operacao = operacao,
        //        Aplicacao = "ApplicationServicoService",
        //        Metodo = metodo,
        //        DadosAntes = isInsert ? "" : JsonSerializer.Serialize(dadosAntes),
        //        DadosDepois = JsonSerializer.Serialize(dadosNovo),
        //        DadosAlterados = isInsert ? "" : JsonSerializer.Serialize(GerarDiff(dadosAntes, dadosNovo)),
        //    };
        //    return log;
        //}

        //public Dictionary<string, object> GerarDiff<T>(T antes, T depois)
        //{
        //    var diff = new Dictionary<string, object>();

        //    foreach (var prop in typeof(T).GetProperties())
        //    {
        //        var valorAntes = prop.GetValue(antes);
        //        var valorDepois = prop.GetValue(depois);

        //        if (!Equals(valorAntes, valorDepois))
        //            diff[prop.Name] = new { de = valorAntes, para = valorDepois };
        //    }

        //    return diff;
        //}

    }
}