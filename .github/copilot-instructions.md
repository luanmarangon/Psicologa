# Instruções para o GitHub Copilot

## Mensagens de commit

- Sempre gere mensagens de commit em **português do Brasil**.
- Siga o padrão **Conventional Commits**: `tipo: descrição breve`
  - Tipos permitidos: `feat`, `fix`, `refactor`, `docs`, `style`, `test`, `chore`, `perf`
  - Exemplo: `fix: corrige conversão de prontuarioId no EvoluirSessao`
- A primeira linha deve ser curta e objetiva (até ~72 caracteres), no imperativo (ex: "corrige", "adiciona", "remove", "ajusta").
- Se necessário, adicione um corpo explicando o "porquê" da mudança, não apenas o "o quê".
- Não use emojis nas mensagens de commit.
- Não traduza nomes de classes, métodos, variáveis ou termos técnicos do código (ex: `ProntuarioSessaoViewModel`, `EncryptIdJSONConverter`) — mantenha como estão no código.

## Contexto do projeto

- Este é um sistema de gestão de clínica de psicologia (PsicoProntuário / Portal Psicólogo), com backend em C# / ASP.NET Core e frontend em React.
- Segue convenções específicas: uso de `EncryptIdJSONConverter`, `DateTimeJSONConverter`, `Int32JSONConverter` para serialização de campos sensíveis/criptografados.