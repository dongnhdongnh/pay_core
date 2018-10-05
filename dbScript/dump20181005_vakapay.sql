-- MySQL dump 10.13  Distrib 5.7.23, for Linux (x86_64)
--
-- Host: 127.0.0.1    Database: vakapay
-- ------------------------------------------------------
-- Server version	5.7.23-0ubuntu0.18.04.1

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
-- Table structure for table `BitcoinAddress`
--

DROP TABLE IF EXISTS `BitcoinAddress`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `BitcoinAddress` (
  `Id` varchar(200) NOT NULL,
  `Address` varchar(45) DEFAULT NULL,
  `WalletId` varchar(200) DEFAULT NULL,
  `Status` varchar(45) DEFAULT NULL,
  `CreatedAt` int(11) DEFAULT NULL,
  `UpdatedAt` int(11) DEFAULT NULL,
  `Password` varchar(60) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='utf8_general_ci';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `BitcoinDepositTransaction`
--

DROP TABLE IF EXISTS `BitcoinDepositTransaction`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `BitcoinDepositTransaction` (
  `Id` varchar(200) NOT NULL,
  `Hash` varchar(200) DEFAULT NULL,
  `BlockNumber` varchar(200) DEFAULT NULL,
  `BlockHash` varchar(200) DEFAULT NULL,
  `NetworkName` varchar(200) DEFAULT NULL,
  `Amount` decimal(20,8) NOT NULL,
  `FromAddress` varchar(200) NOT NULL,
  `ToAddress` varchar(200) NOT NULL,
  `Fee` varchar(200) NOT NULL,
  `Status` varchar(200) DEFAULT NULL,
  `CreatedAt` int(11) NOT NULL,
  `UpdatedAt` int(11) NOT NULL,
  `InProcess` int(11) NOT NULL,
  `Version` int(11) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `BitcoinWithdrawTransaction`
--

DROP TABLE IF EXISTS `BitcoinWithdrawTransaction`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `BitcoinWithdrawTransaction` (
  `Id` varchar(200) NOT NULL,
  `Hash` varchar(200) DEFAULT NULL,
  `BlockNumber` varchar(200) DEFAULT NULL,
  `BlockHash` varchar(200) DEFAULT NULL,
  `NetworkName` varchar(200) DEFAULT NULL,
  `Amount` decimal(20,8) NOT NULL,
  `FromAddress` varchar(200) DEFAULT NULL,
  `ToAddress` varchar(200) NOT NULL,
  `Fee` varchar(200) NOT NULL,
  `Status` varchar(200) DEFAULT NULL,
  `CreatedAt` int(11) NOT NULL,
  `UpdatedAt` int(11) NOT NULL,
  `InProcess` int(11) NOT NULL,
  `Version` int(11) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `idxStatus` (`Status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='utf8_general_ci';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `EmailQueue`
--

DROP TABLE IF EXISTS `EmailQueue`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `EmailQueue` (
  `Id` varchar(200) NOT NULL,
  `ToEmail` varchar(50) DEFAULT NULL,
  `Content` varchar(255) DEFAULT NULL,
  `Subject` varchar(200) DEFAULT NULL,
  `Status` varchar(20) DEFAULT NULL,
  `CreatedAt` int(11) DEFAULT NULL,
  `UpdatedAt` int(11) DEFAULT NULL,
  `InProcess` int(11) DEFAULT '0',
  `Version` int(11) DEFAULT '0',
  `Template` int(11) DEFAULT '-1',
  `DeviceLocation` varchar(255) DEFAULT NULL,
  `DeviceIP` varchar(255) DEFAULT NULL,
  `DeviceBrowser` varchar(255) DEFAULT NULL,
  `DeviceAuthorizeUrl` varchar(255) DEFAULT NULL,
  `Amount` decimal(20,8) DEFAULT NULL,
  `NetworkName` varchar(20) DEFAULT NULL,
  `VerifyUrl` varchar(255) DEFAULT NULL,
  `TransationId` varchar(100) DEFAULT NULL,
  `SignInUrl` varchar(256) DEFAULT NULL,
  `TransactionId` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='utf8_general_ci';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `EthereumAddress`
--

DROP TABLE IF EXISTS `EthereumAddress`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `EthereumAddress` (
  `Id` varchar(200) NOT NULL,
  `Status` varchar(200) NOT NULL,
  `Address` varchar(45) NOT NULL,
  `CreatedAt` int(11) NOT NULL,
  `Password` varchar(200) NOT NULL,
  `UpdatedAt` int(11) NOT NULL,
  `WalletId` varchar(200) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `EthereumWithdrawTransaction`
--

DROP TABLE IF EXISTS `EthereumWithdrawTransaction`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `EthereumWithdrawTransaction` (
  `Id` varchar(200) NOT NULL,
  `Hash` varchar(200) DEFAULT NULL,
  `BlockNumber` varchar(200) DEFAULT NULL,
  `NetworkName` varchar(200) DEFAULT NULL,
  `Amount` decimal(20,0) NOT NULL,
  `FromAddress` varchar(200) DEFAULT NULL,
  `ToAddress` varchar(200) NOT NULL,
  `Fee` varchar(200) NOT NULL,
  `Status` varchar(200) DEFAULT NULL,
  `CreatedAt` int(11) NOT NULL,
  `UpdatedAt` int(11) NOT NULL,
  `InProcess` int(11) DEFAULT NULL,
  `Version` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `User`
--

DROP TABLE IF EXISTS `User`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `User` (
  `Id` varchar(200) NOT NULL,
  `Email` varchar(255) DEFAULT NULL,
  `Phone` varchar(200) DEFAULT NULL,
  `Status` varchar(45) DEFAULT NULL,
  `Fullname` varchar(255) DEFAULT NULL,
  `SecondPassword` varchar(255) DEFAULT NULL,
  `IpWhiteList` varchar(255) DEFAULT NULL,
  `CreatedAt` int(11) DEFAULT NULL,
  `UpdatedAt` int(11) DEFAULT NULL,
  `Avatar` varchar(255) DEFAULT '',
  `FirstName` varchar(255) DEFAULT '',
  `LastName` varchar(255) DEFAULT '',
  `CountryCode` varchar(50) DEFAULT '',
  `Country` varchar(50) DEFAULT '',
  `StreetAddress` varchar(255) DEFAULT '',
  `PostalCode` varchar(50) DEFAULT '',
  `PhoneNumber` varchar(255) NOT NULL DEFAULT '',
  `Birthday` varchar(255) DEFAULT '',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='utf8_general_ci';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `VakacoinAccount`
--

DROP TABLE IF EXISTS `VakacoinAccount`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `VakacoinAccount` (
  `Id` varchar(200) NOT NULL,
  `Address` varchar(45) DEFAULT NULL,
  `WalletId` varchar(200) DEFAULT NULL,
  `Status` varchar(45) DEFAULT NULL,
  `CreatedAt` int(11) DEFAULT NULL,
  `UpdatedAt` int(11) DEFAULT NULL,
  `AccountName` varchar(15) DEFAULT NULL,
  `OwnerPrivateKey` varchar(60) DEFAULT NULL,
  `OwnerPublicKey` varchar(60) DEFAULT NULL,
  `ActivePrivateKey` varchar(60) DEFAULT NULL,
  `ActivePublicKey` varchar(60) DEFAULT NULL,
  `Password` varchar(60) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `VakacoinDepositTransaction`
--

DROP TABLE IF EXISTS `VakacoinDepositTransaction`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `VakacoinDepositTransaction` (
  `Id` varchar(200) NOT NULL,
  `TrxId` varchar(100) DEFAULT NULL,
  `Hash` varchar(100) DEFAULT NULL,
  `BlockNumber` int(11) DEFAULT NULL,
  `NetworkName` varchar(20) DEFAULT NULL,
  `Amount` decimal(16,8) DEFAULT NULL,
  `FromAddress` varchar(45) DEFAULT NULL,
  `ToAddress` varchar(45) DEFAULT NULL,
  `Fee` decimal(16,8) DEFAULT NULL,
  `Status` varchar(10) DEFAULT NULL,
  `CreatedAt` int(11) DEFAULT NULL,
  `UpdatedAt` int(11) DEFAULT NULL,
  `InProcess` int(11) DEFAULT NULL,
  `Version` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='utf8_general_ci';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `VakacoinWithdrawTransaction`
--

DROP TABLE IF EXISTS `VakacoinWithdrawTransaction`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `VakacoinWithdrawTransaction` (
  `Id` varchar(200) NOT NULL,
  `TrxId` varchar(100) DEFAULT NULL,
  `Hash` varchar(100) DEFAULT NULL,
  `BlockNumber` int(11) DEFAULT NULL,
  `NetworkName` varchar(20) DEFAULT NULL,
  `Amount` decimal(16,4) DEFAULT NULL,
  `FromAddress` varchar(45) DEFAULT NULL,
  `ToAddress` varchar(45) DEFAULT NULL,
  `Fee` decimal(16,8) DEFAULT NULL,
  `Status` varchar(10) DEFAULT NULL,
  `CreatedAt` int(11) DEFAULT NULL,
  `UpdatedAt` int(11) DEFAULT NULL,
  `InProcess` int(11) DEFAULT NULL,
  `Version` int(11) DEFAULT NULL,
  `Memo` varchar(256) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `idxStatus` (`Status`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='utf8_general_ci';
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `Wallet`
--

DROP TABLE IF EXISTS `Wallet`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `Wallet` (
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
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2018-10-05 11:00:27
