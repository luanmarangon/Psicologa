-- ==============================================================
--  MultServicos - Script de Criacao do Banco de Dados
--  MySQL 8.0.44 - Compativel com MySQL Workbench
--  IDs: INT AUTO_INCREMENT
--  ENUMs: VARCHAR(30) - validacao feita no backend
-- ==============================================================

DROP DATABASE IF EXISTS `MultiServicos`;
CREATE DATABASE `MultiServicos`
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_unicode_ci;

USE `MultiServicos`;

SET FOREIGN_KEY_CHECKS = 0;
SET UNIQUE_CHECKS = 0;
SET SQL_MODE = 'STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';


-- ==============================================================
-- TABELA: assinaturas
-- ==============================================================
CREATE TABLE `Assinaturas` (
  `AssinaturaId`                   INT        NOT NULL AUTO_INCREMENT,
  `PrestadorId`         INT        NOT NULL,
  `Plano`                VARCHAR(30) NOT NULL DEFAULT 'teste',
  `Status`               VARCHAR(30) NOT NULL DEFAULT 'ativa',
  `DataInicio`          DATE        NOT NULL,
  `DataFim`             DATE        NOT NULL,
  `RenovacaoAutomatica` TINYINT(1)  NOT NULL DEFAULT 1,
  `DataCriacao`            DATETIME    NOT NULL,
  `DataAtualizacao`        DATETIME    NOT NULL,
  PRIMARY KEY (`AssinaturaId`),
  KEY `idx_assinaturas_prestador` (`PrestadorId`),
  KEY `idx_assinaturas_status` (`Status`),
  KEY `idx_assinaturas_data_fim` (`DataFim`),
  CONSTRAINT `fk_assinaturas_prestador`
    FOREIGN KEY (`PrestadorId`) REFERENCES `prestadores` (`PrestadorId`)
    ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='Planos de assinatura dos prestadores';

-- ==============================================================
-- TABELA: servicos
-- ==============================================================
CREATE TABLE `Servicos` (
  `ServicoId`             INT           NOT NULL AUTO_INCREMENT,
  `PrestadorId`   INT           NOT NULL,
  `Nome`           VARCHAR(100)  NOT NULL,
  `Descricao`      TEXT          DEFAULT NULL,
  `Preco`          DECIMAL(10,2) NOT NULL DEFAULT 0.00,
  `TempoExecucao` INT UNSIGNED  NOT NULL DEFAULT 60,
  `Ativo`          TINYINT(1)    NOT NULL DEFAULT 1,
  `DataCriacao`      DATETIME      NOT NULL,
  `DataAtualizacao`  DATETIME      NOT NULL,
  PRIMARY KEY (`ServicoId`),
  KEY `idx_servicos_prestador` (`PrestadorId`),
  KEY `idx_servicos_ativo` (`Ativo`),
  CONSTRAINT `fk_servicos_prestador`
    FOREIGN KEY (`PrestadorId`) REFERENCES `prestadores` (`PrestadorId`)
    ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='Servicos oferecidos por cada prestador com preco e duracao em minutos';
  
  -- CREATE TABLE `Servicos` (
  -- `ServicoId`             INT           NOT NULL AUTO_INCREMENT,
  -- `PrestadorId`   INT           NOT NULL,
  -- `Nome`           VARCHAR(100)  NOT NULL,
  -- `Descricao`      TEXT          DEFAULT NULL,
  -- `Preco`          DECIMAL(10,2) NOT NULL DEFAULT 0.00,
  -- `TempoExecucao` INT UNSIGNED  NOT NULL DEFAULT 60,
  -- `Ativo`          TINYINT(1)    NOT NULL DEFAULT 1,
  -- `DataCriacao`      DATETIME      NOT NULL,
  -- `DataAtualizacao`  DATETIME      NOT NULL,
  -- PRIMARY KEY (`ServicoId`),
  -- KEY `idx_servicos_prestador` (`PrestadorId`),
  -- KEY `idx_servicos_ativo` (`Ativo`),
  -- CONSTRAINT `fk_servicos_prestador`
    -- FOREIGN KEY (`PrestadorId`) REFERENCES `Prestador` (`PrestadorId`)
    -- ON DELETE CASCADE ON UPDATE CASCADE
-- ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  -- COMMENT='Servicos oferecidos por cada prestador com preco e duracao em minutos';


-- ==============================================================
-- TABELA: disponibilidades
-- ==============================================================
CREATE TABLE `Disponibilidades` (
  `DisponibilidadeId`           INT         NOT NULL AUTO_INCREMENT,
  `PrestadorId` INT         NOT NULL,
  `DiaSemana`   VARCHAR(30) NOT NULL,
  `HoraInicio`  TIME        NOT NULL,
  `HoraFim`     TIME        NOT NULL,
  `Ativo`        TINYINT(1)  NOT NULL DEFAULT 1,
  PRIMARY KEY (`DisponibilidadeId`),
  UNIQUE KEY `uq_disponibilidade_dia` (`PrestadorId`, `DiaSemana`),
  KEY `idx_disponibilidades_dia` (`DiaSemana`),
  CONSTRAINT `fk_disponibilidades_prestador`
    FOREIGN KEY (`PrestadorId`) REFERENCES `prestadores` (`PrestadorId`)
    ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='Horarios semanais de atendimento configurados pelo prestador';

-- ==============================================================
-- TABELA: bloqueios_agenda
-- ==============================================================
CREATE TABLE `BloqueiosAgenda` (
  `BloqueiosAgendaId`           INT          NOT NULL AUTO_INCREMENT,
  `PrestadorId` INT          NOT NULL,
  `DataInicio`  DATETIME     NOT NULL,
  `DataFim`     DATETIME     NOT NULL,
  `Motivo`       VARCHAR(255) DEFAULT NULL,
  `DataCriacao`    DATETIME     NOT NULL,
  PRIMARY KEY (`BloqueiosAgendaId`),
  KEY `idx_bloqueios_prestador` (`PrestadorId`),
  KEY `idx_bloqueios_periodo` (`DataInicio`, `DataFim`),
  CONSTRAINT `fk_bloqueios_prestador`
    FOREIGN KEY (`PrestadorId`) REFERENCES `prestadores` (`PrestadorId`)
    ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='Bloqueios manuais de agenda - ferias, folgas e indisponibilidades';

-- ==============================================================
-- TABELA: agendamentos
-- ==============================================================
CREATE TABLE `Agendamentos` (
  `AgendamentoId`                  INT         NOT NULL AUTO_INCREMENT,
  `ClienteId`          INT         NOT NULL,
  `PrestadorId`        INT         NOT NULL,
  `ServicoId`          INT         NOT NULL,
  `DataHoraInicio`    DATETIME    NOT NULL,
  `DataHoraFim`       DATETIME    NOT NULL,
  `Status`              VARCHAR(30) NOT NULL DEFAULT 'pendente',
  `CanceladoPor`       VARCHAR(30) DEFAULT NULL,
  `MotivoCancelamento` TEXT        DEFAULT NULL,
  `DataCriacao`           DATETIME    NOT NULL,
  `DataAtualizacao`       DATETIME    NOT NULL,
  PRIMARY KEY (`AgendamentoId`),
  KEY `idx_agendamentos_cliente` (`ClienteId`),
  KEY `idx_agendamentos_prestador` (`PrestadorId`),
  KEY `idx_agendamentos_servico` (`ServicoId`),
  KEY `idx_agendamentos_status` (`Status`),
  KEY `idx_agendamentos_data` (`DataHoraInicio`),
  CONSTRAINT `fk_agendamentos_cliente`
    FOREIGN KEY (`ClienteId`) REFERENCES `clientes` (`ClienteId`)
    ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `fk_agendamentos_prestador`
    FOREIGN KEY (`PrestadorId`) REFERENCES `prestadores` (`PrestadorId`)
    ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `fk_agendamentos_servico`
    FOREIGN KEY (`ServicoId`) REFERENCES `servicos` (`ServicoId`)
    ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='Tabela central de agendamentos - relaciona cliente, prestador e servico';

-- ==============================================================
-- TABELA: avaliacoes
-- ==============================================================
CREATE TABLE `Avaliacoes` (
  `AvaliacaoId`             INT          NOT NULL AUTO_INCREMENT,
  `AgendamentoId` INT          NOT NULL,
  `ClienteId`     INT          NOT NULL,
  `PrestadorId`   INT          NOT NULL,
  `Nota`           DECIMAL(2,1) NOT NULL,
  `Comentario`     TEXT         DEFAULT NULL,
  `DataCriacao`      DATETIME     NOT NULL,
  PRIMARY KEY (`AvaliacaoId`),
  UNIQUE KEY `uq_avaliacoes_agendamento` (`AgendamentoId`),
  KEY `idx_avaliacoes_cliente` (`ClienteId`),
  KEY `idx_avaliacoes_prestador` (`PrestadorId`),
  CONSTRAINT `chk_avaliacoes_nota` CHECK (`nota` BETWEEN 1.0 AND 5.0),
  CONSTRAINT `fk_avaliacoes_agendamento`
    FOREIGN KEY (`AgendamentoId`) REFERENCES `agendamentos` (`AgendamentoId`)
    ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_avaliacoes_cliente`
    FOREIGN KEY (`ClienteId`) REFERENCES `usuarios` (`UsuarioId`)
    ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `fk_avaliacoes_prestador`
    FOREIGN KEY (`PrestadorId`) REFERENCES `prestadores` (`PrestadorId`)
    ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='Avaliacoes dos clientes pos-servico - nota de 1.0 a 5.0';

-- ==============================================================
-- TABELA: mural
-- ==============================================================
CREATE TABLE `Mural` (
  `MuralId`            INT          NOT NULL AUTO_INCREMENT,
  `PrestadorId`  INT          NOT NULL,
  `Titulo`        VARCHAR(150) DEFAULT NULL,
  `Descricao`     TEXT         DEFAULT NULL,
  `Ativo`         TINYINT(1)   NOT NULL DEFAULT 1,
  `DataCriacao`     DATETIME     NOT NULL,
  `DataAtualizacao` DATETIME     NOT NULL,
  PRIMARY KEY (`MuralId`),
  KEY `idx_mural_prestador` (`PrestadorId`),
  KEY `idx_mural_ativo` (`Ativo`),
  CONSTRAINT `fk_mural_prestador`
    FOREIGN KEY (`PrestadorId`) REFERENCES `prestadores` (`PrestadorId`)
    ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='Posts do mural de portfolio do prestador';

-- ==============================================================
-- TABELA: mural_imagens
-- ==============================================================
CREATE TABLE `MuralImagens` (
  `MuralImagemId`         INT          NOT NULL AUTO_INCREMENT,
  `MuralId`   INT          NOT NULL,
  `urlImagem` VARCHAR(500) NOT NULL,
  `Ordem`      INT UNSIGNED NOT NULL DEFAULT 0,
  `DataCriacao`  DATETIME     NOT NULL,
  PRIMARY KEY (`MuralImagemId`),
  KEY `idx_mural_imagens_mural` (`MuralId`),
  KEY `idx_mural_imagens_ordem` (`MuralId`, `Ordem`),
  CONSTRAINT `fk_mural_imagens_mural`
    FOREIGN KEY (`MuralId`) REFERENCES `mural` (`MuralId`)
    ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='Imagens de cada post do mural do prestador';

-- ==============================================================
-- TABELA: notificacoes
-- ==============================================================
CREATE TABLE `Notificacoes` (
  `NotificacaoId`         INT          NOT NULL AUTO_INCREMENT,
  `UsuarioId` INT          NOT NULL,
  `Tipo`       VARCHAR(30)  NOT NULL,
  `Titulo`     VARCHAR(150) NOT NULL,
  `Mensagem`   TEXT         NOT NULL,
  `Lida`       TINYINT(1)   NOT NULL DEFAULT 0,
  `DataCriacao`  DATETIME     NOT NULL,
  PRIMARY KEY (`NotificacaoId`),
  KEY `idx_notificacoes_usuario` (`UsuarioId`),
  KEY `idx_notificacoes_lida` (`Lida`),
  KEY `idx_notificacoes_tipo` (`Tipo`),
  CONSTRAINT `fk_notificacoes_usuario`
    FOREIGN KEY (`UsuarioId`) REFERENCES `usuarios` (`UsuarioId`)
    ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='Notificacoes enviadas para clientes e prestadores';

-- ==============================================================
-- RESTAURAR CONFIGURACOES
-- ==============================================================
SET FOREIGN_KEY_CHECKS = 1;
SET UNIQUE_CHECKS = 1;

-- ==============================================================
-- VERIFICACAO FINAL
-- ==============================================================
SELECT TABLE_NAME AS 'Tabela', TABLE_COMMENT AS 'Descricao'
FROM information_schema.TABLES
WHERE TABLE_SCHEMA = 'MultServicos'
ORDER BY TABLE_NAME;