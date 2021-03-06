--VAKACOIN DATABASE SETUP
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
-- Table structure for table `vakacoinaccount`
--

DROP TABLE IF EXISTS `vakacoinaccount`;
--
-- Table structure for table `bitcoinddress`a
--
CREATE TABLE `vakacoinaccount` (
  `Id`        varchar(200) NOT NULL,
  `Address`   varchar(45)  DEFAULT NULL,
  `WalletId`  varchar(200) DEFAULT NULL,
  `Status`    varchar(45)  DEFAULT NULL,
  `CreatedAt` int(11)      DEFAULT NULL,
  `UpdatedAt` int(11)      DEFAULT NULL,
  `AccountName`   varchar(15)  DEFAULT NULL,
  `OwnerPrivateKey` varchar(60) DEFAULT NULL,
  `OwnerPublicKey` varchar(60) DEFAULT NULL,
  `ActivePrivateKey` varchar(60) DEFAULT NULL,
  `ActivePublicKey` varchar(60) DEFAULT NULL,
  `Password` varchar(60) DEFAULT NULL,
  
  PRIMARY KEY (`Id`)
)
  ENGINE = InnoDB
  DEFAULT CHARSET = utf8;
--
-- Table structure for table `VakacoinDepositTransaction`
--

DROP TABLE IF EXISTS `vakacoindeposittransaction`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `vakacoindeposittransaction` (
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
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `VakacoinDepositTransaction`
--

LOCK TABLES `vakacoindeposittransaction` WRITE;
/*!40000 ALTER TABLE `VakacoinDepositTransaction` DISABLE KEYS */;
INSERT INTO `vakacoindeposittransaction` VALUES ('14cf3f9a-c23e-4369-9b13-19fd0fff3e2d','aa10afae2ec8218504365edf36f06cfb268d496e2da2d20c72bc3278eae2929e',NULL,17204006,'EOS',1.00000000,'gilprowallet','eosbetdice11',0.00000000,'Success',1537345328,1537345328,0,0),('195423ea-03a3-4251-876a-8f900b1a5bcb','76efe12467f01e0ae700781c9248c2dc13aa1a3e1ef0be5fd95f137777fb225c',NULL,17204005,'EOS',1.00000000,'ha4dqmrvgege','eosbetdice11',0.00000000,'Success',1537345326,1537345326,0,0),('24bfe5cf-2e1f-4f0d-8e1d-f137b14b14e7','cd57743bc4f10af36d20987440b24539340c224b3455740891ee426e7f50e406',NULL,17204011,'EOS',0.14120000,'ssibalmonkey','eosbetdice11',0.00000000,'Success',1537345330,1537345330,0,0),('26962aed-5cdf-4da5-b250-5baa0539ba08','376103e75c2827dea7a40c4e0d9144760867f704f82084fe61c2c461b73aa855',NULL,17204011,'EOS',10.00000000,'gu3tamrzhage','eosbetdice11',0.00000000,'Success',1537345330,1537345330,0,0),('3b36b2f6-96d6-4cc0-81aa-7e83c544d07c','58c76b887767de0cf7443d134b0f3238d23808430ad72a73f839d588c3cf364f',NULL,17204003,'EOS',0.50000000,'gotothemoonn','eosbetdice11',0.00000000,'Success',1537345325,1537345325,0,0),('3f29288d-16a0-46b2-8794-066e85111231','20e0096c3e399c9b32a3a85dc981f775fe963aa66d9a729f23fac6d85a1e3fc3',NULL,17203996,'EOS',1.00000000,'eosgamelover','eosbetdice11',0.00000000,'Success',1537345322,1537345322,0,0),('471d0bc2-2dfd-45af-b530-d3591c2b153f','7cd169c4e49031892518e891eeb26f727f5600013d6bef455cf2a64f6f4f7d74',NULL,17204002,'EOS',12.00000000,'abcdefdqs123','eosbetdice11',0.00000000,'Success',1537345325,1537345325,0,0),('48fd5fe2-5ec2-44b1-8063-d375475f28e7','121be5316b58286217c82f4e65a595a920e6b36f3e40e486d376ac9ef03d6d67',NULL,17204009,'EOS',0.60000000,'marekvrba111','eosbetdice11',0.00000000,'Success',1537345329,1537345329,0,0),('4a48a782-3fa8-4843-93ad-7185a8ca5062','3fd0273abb856fcac1281284eae09697d21232f76e7a6afa0907f8cba224a0c4',NULL,17203993,'EOS',1.00000000,'ha4dqmrvgege','eosbetdice11',0.00000000,'Success',1537345318,1537345318,0,0),('4a8c9f6d-98c0-49a0-b0b7-17420cac0f5a','cb0dc58b6fdac5d0443e42c8a4bf1f8d08debdc60b0bcf6a3fbd660c9637e27f',NULL,17204015,'EOS',1.00000000,'gu2donbtgyge','eosbetdice11',0.00000000,'Success',1537345332,1537345332,0,0),('4c24f395-a002-41d8-995b-a723c5e44109','82226f30b375066d60f0749a5b4dff9a96622d1a5683a8e73379f25474cb5c86',NULL,17203994,'EOS',0.60000000,'marekvrba111','eosbetdice11',0.00000000,'Success',1537345321,1537345321,0,0),('54867dd7-4647-440c-81b2-ebbbd595fa56','ab22889afa506e055214cedf9fe2f98dba7c7e34d0b571e97a74e613fee227d5',NULL,17203999,'EOS',10.00000000,'gu3tamrzhage','eosbetdice11',0.00000000,'Success',1537345323,1537345323,0,0),('557e1f8b-0eb2-4373-8936-52b87ca90da6','62c8700cd5d542217001f868aafeb36dfedfcaba58f59e53fb4830ba5f99ff38',NULL,17203999,'EOS',1.00000000,'gilprowallet','eosbetdice11',0.00000000,'Success',1537345323,1537345323,0,0),('6bdc81e4-d27f-49bb-82fc-564542e34652','99631ebd4fff19d8ac3c31e291beac0606b6aab16bb6c57ea9c1b0615f1a12ad',NULL,17204002,'EOS',0.10000000,'duc1xx1eosio','eosbetdice11',0.00000000,'Success',1537345325,1537345325,0,0),('6c68e0fa-69b8-41da-ad63-cd097bea8e3b','21b6562d47a2cd7050d8d1fbaf78b9c1cce00642480eed2f98dd81d770bb0e9e',NULL,17204013,'EOS',0.10000000,'duc1xx1eosio','eosbetdice11',0.00000000,'Success',1537345331,1537345331,0,0),('6d01aaa6-db56-48ea-9b0b-1e98ab0d9f9f','10595cb1c5dbad9e81d68d97a944223f67773aeb8ba18703a5d18f01eb2dfeca',NULL,17203999,'EOS',0.60000000,'marekvrba111','eosbetdice11',0.00000000,'Success',1537345324,1537345324,0,0),('6f322d9f-9ef2-476b-8700-388ab4d5dc1a','f2fbfae70e3810fa7154d12018bbb9ef44f9e34a22e09b694b1fb21620dc9209',NULL,17204017,'EOS',0.50000000,'heydgmjrhage','eosbetdice11',0.00000000,'Success',1537345333,1537345333,0,0),('73d4bee1-4dce-4ff3-8855-2096d00d406c','402b0649afe5d4c440f5fad6d565ed6076938baa594447bbaf1ce9b7d196fb5c',NULL,17203993,'EOS',1.00000000,'gilprowallet','eosbetdice11',0.00000000,'Success',1537345318,1537345318,0,0),('75bac253-5e5d-4273-a540-766eb1f1e9f8','a7b394fcc43be8589c7282edd349c79f2d128ccd6ebd330e40e4e95fcca8ace8',NULL,17204005,'EOS',0.10000000,'skylose2sky2','eosbetdice11',0.00000000,'Success',1537345326,1537345326,0,0),('7d240192-4a8f-4491-a382-19a6c9351ffc','ea518084a51fd96ea8a5e54d2ec66a756255f87b958b8889c2056b639a6225a1',NULL,17204004,'EOS',0.12500000,'gu3dmnjuhege','eosbetdice11',0.00000000,'Success',1537345326,1537345326,0,0),('7d7f665f-e8fa-4fca-ac39-6ffabf8b477a','540cb17057f0b3c913789fee5cfcc84ffd6a91379f4fce1eccab3be44944cfef',NULL,17204017,'EOS',1.00000000,'gilprowallet','eosbetdice11',0.00000000,'Success',1537345333,1537345333,0,0),('7de854a8-ef00-4c16-8cef-64f3f202ad73','5306834c13ecf699520a1a4e507532f2eb2f1b36b42602c70d49a2cdffec0273',NULL,17204005,'EOS',1.00000000,'hazdanzsgqge','eosbetdice11',0.00000000,'Success',1537345326,1537345326,0,0),('93589b38-a53c-4fd2-b0bd-3c01e6ab1695','399964dc95cd39b548284de10dca3697ac18d52846793decf23bc4d3696d8abd',NULL,17203997,'EOS',0.10000000,'skylose2sky2','eosbetdice11',0.00000000,'Success',1537345323,1537345323,0,0),('949332c7-c481-4c12-a7b6-a6098affb120','1f20bbcff0d048e960f59b10a56c24f2da04848b1258b97e3756b59ae81b97bd',NULL,17204007,'EOS',1.00000000,'kang2222eos5','eosbetdice11',0.00000000,'Success',1537345328,1537345328,0,0),('a0268f92-6c81-45c4-81d4-a3f760d7570c','927ae79a83f8301d57cbed5e9b17a82e3e252814c147abc3f36431305ae33afb',NULL,17203994,'EOS',0.50000000,'gotothemoonn','eosbetdice11',0.00000000,'Success',1537345321,1537345321,0,0),('ae91fe4b-9e7e-4ad3-952b-099da5452c1d','146d81b77709b60e5147a75fa3e6fdf3913d45833767ace0f87d2b4603750584',NULL,17204004,'EOS',0.60000000,'marekvrba111','eosbetdice11',0.00000000,'Success',1537345326,1537345326,0,0),('b3823aef-fd9a-4067-98f7-518298175d01','bf314ebd057ca109e62a7942a0e9febd9c34550a64f43f1d91289474916f4b80',NULL,17203999,'EOS',1.00000000,'ha4dqmrvgege','eosbetdice11',0.00000000,'Success',1537345323,1537345323,0,0),('bc15a9d7-d1e1-4d6a-98e4-761b4d0d86b2','0e5b952ac6fdf8e988805d9cc047081aaa3f00daf8ebda30d9cb85afcc77c017',NULL,17204012,'EOS',1.00000000,'ha4dqmrvgege','eosbetdice11',0.00000000,'Success',1537345331,1537345331,0,0),('c4d2e4c5-bd3e-4bd9-8536-fb0c1eee433b','bb8f1f8f1a75fa7235e265b1fd2f9644538b658a918532e2a80707065d70de40',NULL,17204014,'EOS',0.10000000,'skylose2sky2','eosbetdice11',0.00000000,'Success',1537345332,1537345332,0,0),('c86b6d49-4de6-47ad-94fc-3c48cbcbf1a5','802e48c04a0470789a12c4a96cee1d6d27b9297df153c9b87456749f6c171a8c',NULL,17204015,'EOS',0.60000000,'marekvrba111','eosbetdice11',0.00000000,'Success',1537345332,1537345332,0,0),('d91b44da-8c2f-447d-b809-e92c7fe6ffac','0eff329fc99a05ccfa915f3426ace954bf31b0ed0b7a945cf46a4ede00cadacf',NULL,17203995,'EOS',0.25000000,'lihongwallet','eosbetdice11',0.00000000,'Success',1537345322,1537345322,0,0),('e570fb44-9db0-4d75-b37e-2fcc9afa5300','19c876873125ac541f7bc049436eb1601550ab3a7d9977dace3cac73734e86e5',NULL,17204010,'EOS',3.00000000,'ipfsdaily123','eosbetdice11',0.00000000,'Success',1537345330,1537345330,0,0),('e63259ca-81c6-40c5-b7f0-45834e66ba75','068655edc3614f769dcecb46ce9241acc69218d108a1db3a9057f120ac32ae85',NULL,17204015,'EOS',4.00000000,'eeeooossssss','eosbetdice11',0.00000000,'Success',1537345332,1537345332,0,0),('ee6a8d0d-24f7-478a-9580-a0dec58090d7','7d480e9de991e9e0273bf84bfec78c7d62a9ebd3a21b86d9a8fa96aad95e0d5f',NULL,17203995,'EOS',1.00000000,'hazdanzsgqge','eosbetdice11',0.00000000,'Success',1537345322,1537345322,0,0);
/*!40000 ALTER TABLE `VakacoinDepositTransaction` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `VakacoinWithDrawTransaction`
--

DROP TABLE IF EXISTS `vakacoinwithdrawtransaction`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `vakacoinwithdrawtransaction` (
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
  `Memo` varchar(256) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

CREATE INDEX idxStatus ON vakacoinwithdrawtransaction (Status) 

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2018-09-19 15:31:02
