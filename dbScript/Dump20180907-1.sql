CREATE DATABASE  IF NOT EXISTS `vakapay` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `vakapay`;
-- MySQL dump 10.16  Distrib 10.1.34-MariaDB, for debian-linux-gnu (x86_64)
--
-- Host: localhost    Database: vakapay
-- ------------------------------------------------------
-- Server version	10.1.34-MariaDB-0ubuntu0.18.04.1

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `wallet`
--

DROP TABLE IF EXISTS `wallet`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `wallet` (
  `Id` varchar(200) NOT NULL,
  `UserId` varchar(200) NOT NULL,
  `Balance` decimal(16,8) DEFAULT NULL,
  `NetworkName` varchar(45) DEFAULT NULL,
  `Address` varchar(45) DEFAULT NULL,
  `CreatedAt` int(11) DEFAULT NULL,
  `UpdatedAt` int(11) DEFAULT NULL,
  `Version` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `wallet`
--

--
-- Table structure for table `ethereumwithdrawtransaction`
--

CREATE TABLE `ethereumwithdrawtransaction` (
  `Id` varchar(200) NOT NULL,
  `Hash` varchar(200) DEFAULT NULL,
  `BlockNumber` varchar(200) DEFAULT NULL,
  `NetworkName` varchar(200) DEFAULT NULL,
  `Amount` decimal(20,0) NOT NULL,
  `FromAddress` varchar(200) NOT NULL,
  `ToAddress` varchar(200) NOT NULL,
  `Fee` varchar(200) NOT NULL,
  `Status` varchar(200) DEFAULT NULL,
  `CreatedAt` int(11) NOT NULL,
  `UpdatedAt` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Dumping data for table `ethereumwithdrawtransaction`
--

--
-- Table structure for table `ethereumaddress`
--

CREATE TABLE `ethereumaddress` (
  `Id` varchar(200) NOT NULL,
  `Status` varchar(200) NOT NULL,
  `Address` varchar(45) NOT NULL,
  `CreatedAt` int(11) NOT NULL,
  `Password` varchar(200) NOT NULL,
  `UpdatedAt` int(11) NOT NULL,
  `WalletId` varchar(200) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Dumping data for table `ethereumaddress`
--


LOCK TABLES `wallet` WRITE;
/*!40000 ALTER TABLE `wallet` DISABLE KEYS */;
INSERT INTO `wallet` VALUES ('b342e41e-f96b-4da6-ae74-8e7a0e4a60ce','9e69c659-c274-4543-ac82-002c16fe6fae',0.00000000,'Ethereum',NULL,0,0,0);
/*!40000 ALTER TABLE `wallet` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2018-09-07  3:44:01
