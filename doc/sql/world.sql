-- --------------------------------------------------------
-- Servidor:                     127.0.0.1
-- Versão do servidor:           10.1.13-MariaDB - mariadb.org binary distribution
-- OS do Servidor:               Win64
-- HeidiSQL Versão:              9.1.0.4867
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;

-- Copiando estrutura do banco de dados para world
CREATE DATABASE IF NOT EXISTS `world` /*!40100 DEFAULT CHARACTER SET latin1 */;
USE `world`;


-- Copiando estrutura para tabela world.cash_item
CREATE TABLE IF NOT EXISTS `cash_item` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `usernum` int(11) NOT NULL DEFAULT '0',
  `itemid` int(11) NOT NULL DEFAULT '0',
  `itemopt` varbinary(16) NOT NULL DEFAULT '00000000',
  `itemopt2` int(11) NOT NULL DEFAULT '0',
  `duration` int(11) NOT NULL DEFAULT '0',
  `recv` tinyint(4) NOT NULL DEFAULT '0',
  KEY `ID` (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Exportação de dados foi desmarcado.


-- Copiando estrutura para tabela world.characters
CREATE TABLE IF NOT EXISTS `characters` (
  `id` int(11) NOT NULL,
  `account` int(11) NOT NULL,
  `slot` tinyint(4) unsigned NOT NULL,
  `name` varchar(32) NOT NULL,
  `level` int(11) unsigned NOT NULL DEFAULT '1',
  `class` tinyint(4) unsigned NOT NULL,
  `gender` tinyint(1) NOT NULL,
  `face` tinyint(4) unsigned NOT NULL,
  `hair` tinyint(4) unsigned NOT NULL,
  `colour` tinyint(4) unsigned NOT NULL,
  `map` tinyint(4) unsigned NOT NULL,
  `x` tinyint(4) unsigned NOT NULL,
  `y` tinyint(4) unsigned NOT NULL,
  `created` datetime NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Exportação de dados foi desmarcado.


-- Copiando estrutura para tabela world.characters_equipment
CREATE TABLE IF NOT EXISTS `characters_equipment` (
  `charId` int(11) NOT NULL,
  `head` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `body` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `hands` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `feet` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `lefthand` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `righthand` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `neck` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `finger1` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `finger2` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `finger3` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `finger4` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `leftear` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `rightear` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `leftwrist` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `rightwrist` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `back` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `card` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `belt` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `charm` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `lefteffector` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `righteffector` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `cornalina` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `talisman` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `leftarcane` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `rightarcane` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  PRIMARY KEY (`charId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1 COLLATE=latin1_general_ci;

-- Exportação de dados foi desmarcado.


-- Copiando estrutura para tabela world.characters_equipment3
CREATE TABLE IF NOT EXISTS `characters_equipment3` (
  `charId` int(11) NOT NULL,
  `head` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `body` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `hands` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `feet` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `lefthand` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `righthand` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `neck` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `finger1` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `finger2` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `finger3` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `finger4` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `leftear` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `rightear` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `leftwrist` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `rightwrist` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `back` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `card` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `belt` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `charm` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `lefteffector` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `righteffector` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `cornalina` binary(15) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  `talisman` binary(15) NOT NULL DEFAULT '000000000000000',
  `leftarcane` binary(15) NOT NULL DEFAULT '000000000000000',
  `rightarcane` binary(15) NOT NULL DEFAULT '000000000000000',
  PRIMARY KEY (`charId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Exportação de dados foi desmarcado.


-- Copiando estrutura para tabela world.characters_items
CREATE TABLE IF NOT EXISTS `characters_items` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `charId` int(11) NOT NULL,
  `item` binary(15) NOT NULL,
  `amount` smallint(5) unsigned NOT NULL,
  `slot` tinyint(3) unsigned NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Exportação de dados foi desmarcado.


-- Copiando estrutura para tabela world.characters_quickslots
CREATE TABLE IF NOT EXISTS `characters_quickslots` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `charId` int(11) NOT NULL,
  `skill` tinyint(3) unsigned NOT NULL,
  `slot` tinyint(3) unsigned NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Exportação de dados foi desmarcado.


-- Copiando estrutura para tabela world.characters_skills
CREATE TABLE IF NOT EXISTS `characters_skills` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `charId` int(11) NOT NULL,
  `skill` smallint(5) unsigned NOT NULL,
  `level` tinyint(3) unsigned NOT NULL,
  `slot` tinyint(3) unsigned NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Exportação de dados foi desmarcado.


-- Copiando estrutura para tabela world.characters_stats
CREATE TABLE IF NOT EXISTS `characters_stats` (
  `charId` int(11) NOT NULL,
  `curhp` smallint(5) unsigned NOT NULL,
  `maxhp` smallint(5) unsigned NOT NULL,
  `curmp` smallint(5) unsigned NOT NULL,
  `maxmp` smallint(5) unsigned NOT NULL,
  `cursp` smallint(5) unsigned NOT NULL DEFAULT '0',
  `maxsp` smallint(5) unsigned NOT NULL DEFAULT '0',
  `exp` bigint(20) unsigned NOT NULL DEFAULT '0',
  `str_stat` int(11) unsigned NOT NULL,
  `int_stat` int(11) unsigned NOT NULL,
  `dex_stat` int(11) unsigned NOT NULL,
  `pnt_stat` int(11) unsigned NOT NULL DEFAULT '0',
  `honour` int(11) unsigned NOT NULL DEFAULT '1',
  `rank` int(11) unsigned NOT NULL DEFAULT '1',
  `swordrank` tinyint(3) unsigned NOT NULL DEFAULT '1',
  `swordxp` smallint(5) unsigned NOT NULL DEFAULT '0',
  `swordpoints` smallint(5) unsigned NOT NULL DEFAULT '0',
  `magicrank` tinyint(3) unsigned NOT NULL DEFAULT '1',
  `magicxp` smallint(5) unsigned NOT NULL DEFAULT '0',
  `magicpoints` smallint(5) unsigned NOT NULL DEFAULT '0',
  `alz` bigint(20) unsigned NOT NULL DEFAULT '0',
  `wexp` bigint(20) unsigned NOT NULL DEFAULT '0',
  `honor` bigint(20) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`charId`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Exportação de dados foi desmarcado.


-- Copiando estrutura para tabela world.slotorder
CREATE TABLE IF NOT EXISTS `slotorder` (
  `id` int(11) NOT NULL,
  `slotorder` int(11) NOT NULL DEFAULT '1193046',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- Exportação de dados foi desmarcado.
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
