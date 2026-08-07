/*
CREATE USER 'psicologia'@'%' IDENTIFIED BY 'Pg2026#Clinica!';
GRANT ALL PRIVILEGES ON *.* TO 'psicologia'@'%' WITH GRANT OPTION;
FLUSH PRIVILEGES;
EXIT;
*/



SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- =========================================
-- Tabela: Pessoa (precisa vir antes das que referenciam ela)
-- =========================================
CREATE TABLE Pessoa (
  PessoaId int NOT NULL AUTO_INCREMENT,
  Nome varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  DocIdNro varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  DocIdTipo smallint DEFAULT NULL,
  DataCadastro datetime NOT NULL,
  Ativo tinyint DEFAULT '1',
  DataAlteracao datetime DEFAULT NULL,
  PRIMARY KEY (PessoaId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =========================================
-- Tabela: Agendamento
-- =========================================
CREATE TABLE Agendamento (
  AgendamentoId int NOT NULL AUTO_INCREMENT,
  PacienteId int DEFAULT NULL,
  PsicologoId int DEFAULT NULL,
  ServicoId int DEFAULT NULL,
  DataConsulta date DEFAULT NULL,
  HoraInicio time DEFAULT NULL,
  HoraFim time DEFAULT NULL,
  TempoSessao int DEFAULT NULL,
  Online tinyint DEFAULT '0',
  Presencial tinyint DEFAULT '0',
  StatusAgendamento int DEFAULT NULL,
  TipoAgendamento int DEFAULT NULL,
  Ativo tinyint DEFAULT '0',
  ConfirmouAgendamento tinyint DEFAULT '0',
  DataConfirmacao datetime DEFAULT NULL,
  DataCriacao datetime DEFAULT NULL,
  DataAtualizacao datetime DEFAULT NULL,
  PRIMARY KEY (AgendamentoId),
  KEY FK_Agendamento_Paciente_idx (PacienteId),
  KEY FK_Agendamento_Psicologo_idx (PsicologoId),
  KEY FK_Agendamento_Servico_idx (ServicoId),
  CONSTRAINT FK_Agendamento_Paciente FOREIGN KEY (PacienteId) REFERENCES Pessoa (PessoaId),
  CONSTRAINT FK_Agendamento_Psicologo FOREIGN KEY (PsicologoId) REFERENCES Pessoa (PessoaId),
  CONSTRAINT FK_Agendamento_Servico FOREIGN KEY (ServicoId) REFERENCES Servico (ServicoId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =========================================
-- Tabela: Blogpost
-- =========================================
CREATE TABLE Blogpost (
  BlogPostId int NOT NULL AUTO_INCREMENT,
  Titulo varchar(200) COLLATE utf8mb4_unicode_ci NOT NULL,
  Url varchar(200) COLLATE utf8mb4_unicode_ci NOT NULL,
  Conteudo longtext COLLATE utf8mb4_unicode_ci NOT NULL,
  Resumo varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  ImagemCapa longblob,
  Autor varchar(150) COLLATE utf8mb4_unicode_ci NOT NULL,
  DataCriacao datetime DEFAULT NULL,
  DataAtualizacao datetime DEFAULT NULL,
  DataPublicacao datetime DEFAULT NULL,
  DataRevogacao datetime DEFAULT NULL,
  Ativo tinyint(1) NOT NULL DEFAULT '1',
  PessoaId int DEFAULT NULL,
  PRIMARY KEY (BlogPostId),
  UNIQUE KEY Url (Url),
  KEY FK_BlogPost_Pessoa (PessoaId),
  CONSTRAINT FK_BlogPost_Pessoa FOREIGN KEY (PessoaId) REFERENCES Pessoa (PessoaId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =========================================
-- Tabela: Configuracao
-- =========================================
CREATE TABLE Configuracao (
  ConfiguracaoId int NOT NULL AUTO_INCREMENT,
  Nome varchar(150) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  CEP varchar(10) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  Endereco varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  Numero varchar(10) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  Complemento varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  Bairro varchar(70) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  Cidade varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  Estado varchar(2) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  Whatsapp varchar(15) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  Email varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  Facebook varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  Instagram varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  Linkedin varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  DataCriacao datetime DEFAULT NULL,
  DataAtualizacao datetime DEFAULT NULL,
  Slogan varchar(200) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (ConfiguracaoId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =========================================
-- Tabela: ConfiguracaoFuncionamento
-- =========================================
CREATE TABLE ConfiguracaoFuncionamento (
  ConfiguracaoFuncionamentoId int NOT NULL AUTO_INCREMENT,
  DiaSemana int DEFAULT NULL,
  Ativo tinyint DEFAULT NULL,
  HoraInicio time DEFAULT NULL,
  HoraFim time DEFAULT NULL,
  Ordem int DEFAULT NULL,
  DataCriacao datetime DEFAULT NULL,
  DataAtualizacao datetime DEFAULT NULL,
  PRIMARY KEY (ConfiguracaoFuncionamentoId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =========================================
-- Tabela: Convenio
-- =========================================
CREATE TABLE Convenio (
  ConvenioId int NOT NULL AUTO_INCREMENT,
  Nome varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  Icon varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  DestaqueHome tinyint DEFAULT NULL,
  Ativo tinyint DEFAULT NULL,
  DataCriacao datetime DEFAULT NULL,
  DataAtualizacao datetime DEFAULT NULL,
  PRIMARY KEY (ConvenioId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT=' ';

-- =========================================
-- Tabela: Documentos
-- =========================================
CREATE TABLE Documentos (
  DocumentosId int NOT NULL AUTO_INCREMENT,
  Nome varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  Categoria int DEFAULT NULL,
  Ativo tinyint DEFAULT NULL,
  Conteudo longtext COLLATE utf8mb4_unicode_ci,
  DataCriacao datetime DEFAULT NULL,
  DataAtualizacao datetime DEFAULT NULL,
  PRIMARY KEY (DocumentosId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =========================================
-- Tabela: Log
-- =========================================
CREATE TABLE Log (
  LogId int NOT NULL AUTO_INCREMENT,
  DataCriacao datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  UsuarioId int NOT NULL,
  UsuarioNome varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  Dispositivo varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  IP varchar(45) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  UserAgent varchar(300) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  Entidade varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  EntidadeId int DEFAULT NULL,
  Operacao varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  Aplicacao varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  Metodo varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  DadosAntes longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
  DadosDepois longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
  DadosAlterados longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
  PRIMARY KEY (LogId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =========================================
-- Tabela: Paciente
-- =========================================
CREATE TABLE Paciente (
  PacienteId int NOT NULL AUTO_INCREMENT,
  PessoaId int DEFAULT NULL,
  DataPrimeiraSessao datetime DEFAULT NULL,
  Ativo tinyint DEFAULT NULL,
  Observacoes varchar(1000) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  ContatoEmergenciaNome varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  ContatoEmergenciaTelefone varchar(20) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  Matricula varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  ResponsavelId int DEFAULT NULL,
  DataCriacao datetime DEFAULT NULL,
  DataAtualizacao datetime DEFAULT NULL,
  PRIMARY KEY (PacienteId),
  KEY FK_Paciente_Pessoa_idx (PessoaId),
  CONSTRAINT FK_Paciente_Pessoa FOREIGN KEY (PessoaId) REFERENCES Pessoa (PessoaId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =========================================
-- Tabela: PerfilUsuario
-- =========================================
CREATE TABLE PerfilUsuario (
  PerfilUsuarioId int NOT NULL,
  Nome varchar(45) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  PRIMARY KEY (PerfilUsuarioId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =========================================
-- Tabela: PerfilUsuarioPermissao
-- =========================================
CREATE TABLE PerfilUsuarioPermissao (
  PerfilUsuarioId int NOT NULL,
  Permissao smallint NOT NULL,
  PRIMARY KEY (Permissao,PerfilUsuarioId),
  KEY FK_PerfilUsuarioPermissao_PerfilUsuario_idx (PerfilUsuarioId),
  CONSTRAINT FK_PerfilUsuarioPermisao_PerfilUsuario FOREIGN KEY (PerfilUsuarioId) REFERENCES PerfilUsuario (PerfilUsuarioId) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =========================================
-- Tabela: PessoaContato
-- =========================================
CREATE TABLE PessoaContato (
  PessoaContatoId int NOT NULL AUTO_INCREMENT,
  Tipo smallint NOT NULL,
  Contato varchar(80) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  Observacao varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  PessoaId int NOT NULL,
  PRIMARY KEY (PessoaContatoId,PessoaId),
  KEY FK_PessoaContato_Pessoa_idx (PessoaId),
  CONSTRAINT FK_PessoaContato_Pessoa FOREIGN KEY (PessoaId) REFERENCES Pessoa (PessoaId) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =========================================
-- Tabela: PessoaEndereco
-- =========================================
CREATE TABLE PessoaEndereco (
  PessoaEnderecoId int NOT NULL AUTO_INCREMENT,
  Logradouro varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  Numero varchar(14) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  Bairro varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  Cep varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  PontoReferencia varchar(70) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  Complemento varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  Cidade varchar(70) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  UF varchar(2) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PessoaId int DEFAULT NULL,
  Latitude varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  Longitude varchar(15) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (PessoaEnderecoId),
  KEY FK_PessoaEndereco_Pessoa_idx (PessoaId),
  CONSTRAINT FK_PessoaEndereco_Pessoa FOREIGN KEY (PessoaId) REFERENCES Pessoa (PessoaId) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =========================================
-- Tabela: PessoaFisica
-- =========================================
CREATE TABLE PessoaFisica (
  PessoaId int NOT NULL,
  DataNascimento datetime DEFAULT NULL,
  Sexo int DEFAULT '0',
  PRIMARY KEY (PessoaId),
  CONSTRAINT FK_PessoaFisica_Pessoa FOREIGN KEY (PessoaId) REFERENCES Pessoa (PessoaId) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =========================================
-- Tabela: PessoaJuridica
-- =========================================
CREATE TABLE PessoaJuridica (
  PessoaId int NOT NULL,
  RazaoSocial varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  PRIMARY KEY (PessoaId),
  CONSTRAINT FK_PessoaJuridica_Pessoa FOREIGN KEY (PessoaId) REFERENCES Pessoa (PessoaId) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =========================================
-- Tabela: PessoaTipo
-- =========================================
CREATE TABLE PessoaTipo (
  PessoaTipoId int NOT NULL AUTO_INCREMENT,
  Tipo smallint NOT NULL,
  PessoaId int NOT NULL,
  PRIMARY KEY (PessoaTipoId),
  KEY FK_PessoaTIPO_Pessoa_idx (PessoaId),
  CONSTRAINT FK_PessoaTIPO_Pessoa FOREIGN KEY (PessoaId) REFERENCES Pessoa (PessoaId) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =========================================
-- Tabela: Prontuario
-- =========================================
CREATE TABLE Prontuario (
  ProntuarioId int NOT NULL AUTO_INCREMENT,
  PacienteId int DEFAULT NULL,
  QueixaPrincipal varchar(1000) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  ObjetivoTratamento varchar(1000) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  HistoricoFamiliar varchar(1000) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  ObservacoesIniciais varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  Ativo tinyint DEFAULT NULL,
  DataCriacao datetime DEFAULT NULL,
  DataAtualizacao datetime DEFAULT NULL,
  DataEncerramento datetime DEFAULT NULL,
  PRIMARY KEY (ProntuarioId),
  KEY FK_Prontuario_Paciente_idx (PacienteId),
  CONSTRAINT FK_Prontuario_Paciente FOREIGN KEY (PacienteId) REFERENCES Paciente (PacienteId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =========================================
-- Tabela: ProntuarioAnexo
-- =========================================
CREATE TABLE ProntuarioAnexo (
  ProntuarioAnexoId int NOT NULL AUTO_INCREMENT,
  ProntuarioId int DEFAULT NULL,
  TipoAnexo int DEFAULT NULL,
  Nome varchar(200) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  NomeArquivo varchar(200) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  MimeType varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  TamanhoArquivo bigint DEFAULT NULL,
  Arquivo longblob,
  Observacao varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  DataCriacao datetime DEFAULT NULL,
  DataAtualizacao datetime DEFAULT NULL,
  PRIMARY KEY (ProntuarioAnexoId),
  KEY FK_ProntuarioAnexo_Prontuario_idx (ProntuarioId),
  CONSTRAINT FK_ProntuarioAnexo_Prontuario FOREIGN KEY (ProntuarioId) REFERENCES Prontuario (ProntuarioId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =========================================
-- Tabela: Servico (precisa vir antes de Agendamento normalmente,
-- mas com FOREIGN_KEY_CHECKS=0 a ordem não importa)
-- =========================================
CREATE TABLE Servico (
  ServicoId int NOT NULL AUTO_INCREMENT,
  Nome varchar(70) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  Url varchar(150) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  DescricaoCurta varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  Descricao varchar(500) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  TempoSessaoMinutos int DEFAULT NULL,
  ValorSessao decimal(10,2) DEFAULT NULL,
  ImagemCapa longblob,
  Online tinyint DEFAULT NULL,
  Presencial tinyint DEFAULT NULL,
  DestaqueHome tinyint DEFAULT NULL,
  OrdemExibicao int DEFAULT NULL,
  Ativo tinyint DEFAULT NULL,
  DataCriacao datetime DEFAULT NULL,
  DataAtualizacao datetime DEFAULT NULL,
  PRIMARY KEY (ServicoId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =========================================
-- Tabela: ProntuarioSessao
-- =========================================
CREATE TABLE ProntuarioSessao (
  ProntuarioSessaoId int NOT NULL AUTO_INCREMENT,
  ProntuarioId int DEFAULT NULL,
  AgendamentoId int DEFAULT NULL,
  DataSessao datetime DEFAULT NULL,
  HoraInicio time DEFAULT NULL,
  HoraFim time DEFAULT NULL,
  PsicologaId int DEFAULT NULL,
  TipoAtendimento int DEFAULT NULL,
  Evolucao varchar(5000) DEFAULT NULL,
  DataCriacao datetime DEFAULT NULL,
  DataAtualizacao datetime DEFAULT NULL,
  PRIMARY KEY (ProntuarioSessaoId),
  KEY FK_ProntuarioSessao_Prontuario_idx (ProntuarioId),
  KEY FK_ProntuatioSessao_Pessoa_idx (PsicologaId),
  CONSTRAINT FK_ProntuarioSessao_Prontuario FOREIGN KEY (ProntuarioId) REFERENCES Prontuario (ProntuarioId),
  CONSTRAINT FK_ProntuatioSessao_Pessoa FOREIGN KEY (PsicologaId) REFERENCES Pessoa (PessoaId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- =========================================
-- Tabela: Psicologo
-- =========================================
CREATE TABLE Psicologo (
  PsicologoId int NOT NULL AUTO_INCREMENT,
  PessoaId int DEFAULT NULL,
  Crp varchar(50) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  CrpUf varchar(2) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  DataEmissaoCrp datetime DEFAULT NULL,
  Ativo tinyint DEFAULT NULL,
  DataCriacao datetime DEFAULT NULL,
  DataAtualizacao datetime DEFAULT NULL,
  PRIMARY KEY (PsicologoId),
  KEY FK_Psicologo_Pessoa_idx (PessoaId),
  CONSTRAINT FK_Psicologo_Pessoa FOREIGN KEY (PessoaId) REFERENCES Pessoa (PessoaId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =========================================
-- Tabela: ServicoContato
-- =========================================
CREATE TABLE ServicoContato (
  ServicoContatoId int NOT NULL AUTO_INCREMENT,
  ServicoId int DEFAULT NULL,
  Nome varchar(120) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  Contato varchar(30) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  Email varchar(150) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  Mensagem text COLLATE utf8mb4_unicode_ci,
  StatusContato int DEFAULT NULL,
  EntrouContato tinyint DEFAULT NULL,
  DataContato datetime DEFAULT NULL,
  DataRetorno datetime DEFAULT NULL,
  ObservacaoInterna text COLLATE utf8mb4_unicode_ci,
  Origem varchar(100) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  VirouPaciente tinyint DEFAULT NULL,
  Prioridade int DEFAULT '0',
  PreferenciaContato int DEFAULT '0',
  IP varchar(45) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  UserAgent varchar(300) COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  DataCriacao datetime DEFAULT NULL,
  DataAtualizacao datetime DEFAULT NULL,
  PRIMARY KEY (ServicoContatoId),
  KEY FK_ServicoContato_Servico_idx (ServicoId),
  CONSTRAINT FK_ServicoContato_Servico FOREIGN KEY (ServicoId) REFERENCES Servico (ServicoId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='  ';

-- =========================================
-- Tabela: Usuario
-- =========================================
CREATE TABLE Usuario (
  UsuarioId int NOT NULL AUTO_INCREMENT,
  Nome varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  Senha varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  DtCadastro datetime NOT NULL,
  DtSenha datetime NOT NULL,
  PessoaId int NOT NULL,
  Perfil smallint NOT NULL,
  CodigoSeguranca varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PrimeiroAcesso tinyint DEFAULT '1',
  PRIMARY KEY (UsuarioId),
  KEY FK_Usuario_Pessoa_idx (PessoaId),
  CONSTRAINT FK_Usuario_Pessoa FOREIGN KEY (PessoaId) REFERENCES Pessoa (PessoaId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;



CREATE TABLE FinanceiroCategoria (
  FinanceiroCategoriaId INT NOT NULL AUTO_INCREMENT,
  Nome VARCHAR(100) NULL,
  Tipo INT NULL,
  Ativo TINYINT NULL,
  DataCriacao DATETIME NULL,
  DataAtualizacao DATETIME NULL,
  PRIMARY KEY (FinanceiroCategoriaId))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_unicode_ci;

CREATE TABLE Financeiro (
  FinanceiroId INT NOT NULL AUTO_INCREMENT,
  Tipo INT NULL,
  Descricao VARCHAR(300) NULL,
  CategoriaId INT NULL,
  Valor DECIMAL(10,2) NULL,
  DataLancamento DATETIME NULL,
  Observacao VARCHAR(500) NULL,
  Ativo TINYINT NULL,
  Quitado TINYINT NULL,
  DataQuitacao DATETIME NULL,
  DataCriacao DATETIME NULL,
  DataAtualizacao DATETIME NULL,
  PRIMARY KEY (FinanceiroId),
  INDEX FK_Financeiro_FinanceiroCategoria_idx (CategoriaId ASC) VISIBLE,
  CONSTRAINT FK_Financeiro_FinanceiroCategoria
    FOREIGN KEY (CategoriaId)
    REFERENCES Psicologa.FinanceiroCategoria (FinanceiroCategoriaId)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_unicode_ci;




SET FOREIGN_KEY_CHECKS = 1;