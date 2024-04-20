-- phpMyAdmin SQL Dump
-- version 5.2.0
-- https://www.phpmyadmin.net/
--
-- Gép: 127.0.0.1
-- Létrehozás ideje: 2024. Feb 21. 15:32
-- Kiszolgáló verziója: 10.4.25-MariaDB
-- PHP verzió: 8.1.10

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";
--
-- Tábla szerkezet ehhez a táblához `cities`
--

CREATE TABLE `cities` (
  `id` INT PRIMARY KEY AUTO_INCREMENT,
  `city_name` varchar(255) DEFAULT NULL,
  `longitude` decimal(10,8) DEFAULT NULL,
  `latitude` decimal(10,8) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT INTO `cities` (`id`, `city_name`, `longitude`, `latitude`) VALUES
(1, 'Budapest', '19.05140000', '47.49250000'),
(2, 'Debrecen', '21.63920000', '47.53000000'),
(3, 'Székesfehérvár', '18.40890000', '47.19560000'),
(4, 'Győr', '17.63440000', '47.68420000'),
(5, 'Szeged', '20.14500000', '46.25500000'),
(6, 'Nyíregyháza', '21.72710000', '47.95310000'),
(7, 'Kecskemét', '19.68970000', '46.90610000'),
(8, 'Miskolc', '20.66670000', '48.08330000'),
(9, 'Szombathely', '16.62190000', '47.23510000'),
(10, 'Pécs', '18.23310000', '46.07080000'),
(11, 'Veszprém', '17.91380000', '47.09300000'),
(12, 'Kaposvár', '17.78230000', '46.36380000'),
(13, 'Sopron', '16.58310000', '47.68490000'),
(14, 'Érd', '18.92200000', '47.37840000'),
(15, 'Szolnok', '20.19650000', '47.17470000'),
(16, 'Tatabánya', '18.39490000', '47.58620000'),
(17, 'Békéscsaba', '21.09100000', '46.67900000'),
(18, 'Zalaegerszeg', '16.85110000', '46.83920000'),
(19, 'Eger', '20.37470000', '47.89900000'),
(20, 'Nagykanizsa', '16.99250000', '46.45500000'),
(21, 'Dunaújváros', '18.91270000', '46.98060000'),
(22, 'Dunakeszi', '19.14120000', '47.63030000'),
(23, 'Hódmezővásárhely', '20.31880000', '46.43040000'),
(24, 'Szigetszentmiklós', '19.04830000', '47.34560000'),
(25, 'Cegléd', '19.80200000', '47.17430000'),
(26, 'Baja', '18.95360000', '46.18330000'),
(27, 'Mosonmagyaróvár', '17.26870000', '47.87370000'),
(28, 'Vác', '19.13100000', '47.77520000'),
(29, 'Salgótarján', '19.80750000', '48.10400000'),
(30, 'Gödöllő', '19.36670000', '47.60000000'),
(31, 'Ózd', '20.28690000', '48.21920000'),
(32, 'Szekszárd', '18.70380000', '46.35600000'),
(33, 'Pápa', '17.46800000', '47.32370000'),
(34, 'Budaörs', '18.95800000', '47.46070000'),
(35, 'Gyöngyös', '19.93330000', '47.78330000'),
(36, 'Esztergom', '18.74030000', '47.78560000'),
(37, 'Ajka', '17.55220000', '47.10060000'),
(38, 'Szentendre', '19.06860000', '47.70440000'),
(39, 'Siófok', '18.09010000', '46.92390000'),
(40, 'Kazincbarcika', '20.64560000', '48.25310000'),
(41, 'Gyál', '19.21920000', '47.38610000'),
(42, 'Dunaharaszti', '19.09480000', '47.35390000'),
(43, 'Tata', '18.32380000', '47.65260000'),
(44, 'Komló', '18.26130000', '46.19120000'),
(45, 'Vecsés', '19.26480000', '47.40570000'),
(46, 'Göd', '19.13440000', '47.69060000'),
(47, 'Fót', '19.19030000', '47.61810000'),
(48, 'Veresegyház', '19.28300000', '47.65050000'),
(49, 'Hatvan', '19.66970000', '47.66810000'),
(50, 'Gyömrő', '19.39770000', '47.42500000'),
(51, 'Komárom', '18.12440000', '47.74030000'),
(52, 'Várpalota', '18.13760000', '47.19890000'),
(53, 'Keszthely', '17.24810000', '46.76960000'),
(54, 'Monor', '19.44890000', '47.34750000'),
(55, 'Szigethalom', '19.00780000', '47.31540000'),
(56, 'Békés', '21.13330000', '46.76670000'),
(57, 'Százhalombatta', '18.91360000', '47.30040000'),
(58, 'Dombóvár', '18.14220000', '46.38200000'),
(59, 'Oroszlány', '18.31670000', '47.48330000'),
(60, 'Pomáz', '19.01960000', '47.64310000'),
(61, 'Mohács', '18.67980000', '45.99590000'),
(62, 'Pécel', '19.33540000', '47.48930000'),
(63, 'Mátészalka', '22.31670000', '47.95000000'),
(64, 'Mezőkövesd', '20.58330000', '47.81670000'),
(65, 'Kisvárda', '22.08330000', '48.21670000'),
(66, 'Kalocsa', '18.98580000', '46.53350000'),
(67, 'Budakeszi', '18.92810000', '47.51230000'),
(68, 'Sárvár', '16.93330000', '47.25000000'),
(69, 'Tapolca', '17.40810000', '46.88280000'),
(70, 'Biatorbágy', '18.82520000', '47.47120000'),
(71, 'Tiszaújváros', '21.08330000', '47.93330000'),
(72, 'Pilisvörösvár', '18.91080000', '47.62110000'),
(73, 'Törökbálint', '18.91220000', '47.43670000'),
(74, 'Balassagyarmat', '19.29420000', '48.07860000'),
(75, 'Sátoraljaújhely', '21.66670000', '48.40000000'),
(76, 'Kerepestarcsa', '19.26340000', '47.54780000'),
(77, 'Hajdúsámson', '21.76670000', '47.60000000'),
(78, 'Albertirsa', '19.60670000', '47.24000000'),
(79, 'Bonyhád', '18.53090000', '46.30060000'),
(80, 'Balatonfüred', '17.88330000', '46.95000000'),
(81, 'Gárdony', '18.60910000', '47.19730000'),
(82, 'Hajdúhadház', '21.66670000', '47.68330000'),
(83, 'Üllő', '19.34390000', '47.38540000'),
(84, 'Maglód', '19.35260000', '47.44390000'),
(85, 'Nagykáta', '19.73960000', '47.41200000'),
(86, 'Bicske', '18.63630000', '47.49070000'),
(87, 'Kőszeg', '16.55220000', '47.38190000'),
(88, 'Pilis', '19.54350000', '47.28440000'),
(89, 'Dorog', '18.72920000', '47.71940000'),
(90, 'Nyírbátor', '22.13330000', '47.83330000'),
(91, 'Halásztelek', '18.98780000', '47.36080000'),
(92, 'Sajószentpéter', '20.71830000', '48.21690000'),
(93, 'Budakalász', '19.04600000', '47.62150000'),
(94, 'Isaszeg', '19.40000000', '47.53330000'),
(95, 'Diósd', '18.94580000', '47.40420000'),
(96, 'Bátonyterenye', '19.83090000', '47.99060000'),
(97, 'Körmend', '16.60580000', '47.01100000'),
(98, 'Tolna', '18.79030000', '46.42360000'),
(99, 'Ráckeve', '18.94560000', '47.16080000'),
(100, 'Solymár', '18.92900000', '47.59100000'),
(101, 'Celldömölk', '17.14910000', '47.25570000'),
(102, 'Tököl', '18.96710000', '47.32030000'),
(103, 'Szigetvár', '17.79940000', '46.04750000'),
(104, 'Tárnok', '18.85860000', '47.35970000'),
(105, 'Nagyatád', '17.36430000', '46.22270000'),
(106, 'Csömör', '19.23330000', '47.55000000'),
(107, 'Balatonalmádi', '18.01670000', '47.03330000'),
(108, 'Edelény', '20.74420000', '48.29670000'),
(109, 'Siklós', '18.29860000', '45.85190000'),
(110, 'Piliscsaba', '18.83330000', '47.61670000'),
(111, 'Erdőkertes', '19.31670000', '47.66670000'),
(112, 'Dunavarsány', '19.06720000', '47.27810000'),
(113, 'Nagykovácsi', '18.88000000', '47.58000000'),
(114, 'Szerencs', '21.20500000', '48.16220000'),
(115, 'Győrújbarát', '17.64640000', '47.60790000'),
(116, 'Sándorfalva', '20.11440000', '46.36840000'),
(117, 'Üröm', '19.01670000', '47.60000000'),
(118, 'Páty', '18.83330000', '47.51670000'),
(119, 'Fehérgyarmat', '22.51720000', '47.98500000'),
(120, 'Őrbottyán', '19.26670000', '47.68330000'),
(121, 'Tura', '19.59670000', '47.60670000'),
(122, 'Mogyoród', '19.25000000', '47.60000000'),
(123, 'Nyergesújfalu', '18.54870000', '47.75680000'),
(124, 'Füzesabony', '20.40900000', '47.75100000'),
(125, 'Velence', '18.64790000', '47.24180000'),
(126, 'Zirc', '17.87250000', '47.26330000'),
(127, 'Kozármisleny', '18.29200000', '46.02800000'),
(128, 'Felsőzsolca', '20.85560000', '48.10870000'),
(129, 'Taksony', '19.08330000', '47.33330000'),
(130, 'Téglás', '21.68330000', '47.71670000'),
(131, 'Putnok', '20.43670000', '48.29360000'),
(132, 'Nagyecsed', '22.40000000', '47.86670000'),
(133, 'Aszód', '19.48000000', '47.65450000'),
(134, 'Szentlőrinc', '17.98560000', '46.04220000'),
(135, 'Martfő', '20.28390000', '47.01690000'),
(136, 'Tápiószecső', '19.60000000', '47.45000000'),
(137, 'Szada', '19.31670000', '47.63330000'),
(138, 'Encs', '21.12190000', '48.33070000'),
(139, 'Nagytarcsa', '19.28620000', '47.52580000'),
(140, 'Tahitófalu', '19.07810000', '47.75220000'),
(141, 'Tápiószele', '19.88330000', '47.33330000'),
(142, 'Martonvásár', '18.78850000', '47.31400000'),
(143, 'Kartal', '19.55000000', '47.66670000'),
(144, 'Balatonboglár', '17.66670000', '46.76670000'),
(145, 'Zsámbék', '18.71860000', '47.54830000'),
(146, 'Délegyháza', '19.06670000', '47.25000000'),
(147, 'Lőrinci', '19.68330000', '47.73330000'),
(148, 'Rákóczifalva', '20.22890000', '47.09310000'),
(149, 'Tát', '18.64470000', '47.74070000'),
(150, 'Szikszó', '20.94610000', '48.19500000'),
(151, 'Nagymaros', '18.95650000', '47.79060000'),
(152, 'Mikepércs', '21.63330000', '47.45000000'),
(153, 'Harkány', '18.23690000', '45.84760000'),
(154, 'Nyékládháza', '20.83690000', '47.99170000'),
(155, 'Onga', '20.90740000', '48.11900000'),
(156, 'Lábatlan', '18.48750000', '47.74800000'),
(157, 'Sződliget', '19.15000000', '47.73330000'),
(158, 'Hévíz', '17.18500000', '46.79230000'),
(159, 'Balatonfőzfő', '18.03900000', '47.06390000'),
(160, 'Inárcs', '19.33330000', '47.26670000'),
(161, 'Nyúl', '17.68070000', '47.58130000'),
(162, 'Kótaj', '21.71670000', '48.05000000'),
(163, 'Telki', '18.83330000', '47.55000000'),
(164, 'Tab', '18.03560000', '46.73270000'),
(165, 'Ceglédbercel', '19.66640000', '47.22250000'),
(166, 'Záhony', '22.17440000', '48.40690000'),
(167, 'Pilisszentiván', '18.90000000', '47.61670000'),
(168, 'Mende', '19.45000000', '47.41670000'),
(169, 'Szirmabesenyő', '20.79580000', '48.14960000'),
(170, 'Hernád', '19.40530000', '47.16330000'),
(171, 'Mályi', '20.83330000', '48.01670000'),
(172, 'Pannonhalma', '17.74990000', '47.54680000'),
(173, 'Ecser', '19.32600000', '47.44440000'),
(174, 'Pilisborosjenő', '19.00000000', '47.61670000'),
(175, 'Tokod', '18.67450000', '47.72990000'),
(176, 'Leányfalu', '19.08940000', '47.72670000'),
(177, 'Sződ', '19.20000000', '47.71670000'),
(178, 'Tokaj', '21.41000000', '48.12000000'),
(179, 'Bóly', '18.51820000', '45.96730000'),
(180, 'Vámosszabadi', '17.65000000', '47.76670000'),
(181, 'Bag', '19.48370000', '47.63570000'),
(182, 'Felsőpakony', '19.25000000', '47.35000000'),
(183, 'Gyenesdiás', '17.29300000', '46.77070000'),
(184, 'Bük', '16.75090000', '47.38450000'),
(185, 'Ajak', '22.03330000', '48.18330000'),
(186, 'Csobánka', '18.96780000', '47.64250000'),
(187, 'Nyírpazony', '21.80000000', '47.98330000'),
(188, 'Tuzsér', '22.12030000', '48.33580000'),
(189, 'Herend', '17.75000000', '47.13330000'),
(190, 'Vértesszőlős', '18.38030000', '47.61880000'),
(191, 'Abda', '17.54190000', '47.69510000'),
(192, 'Cserszegtomaj', '17.23120000', '46.80610000'),
(193, 'Hévizgyörk', '19.52000000', '47.63160000'),
(194, 'Gönyő', '17.83330000', '47.73330000'),
(195, 'Csolnok', '18.71300000', '47.69350000'),
(196, 'Kakucs', '19.36670000', '47.25000000'),
(197, 'Múcsony', '20.68190000', '48.26870000'),
(198, 'Harka', '16.60000000', '47.63330000'),
(199, 'Pilisszántó', '18.88670000', '47.66980000'),
(200, 'Sásd', '18.10580000', '46.25440000'),
(201, 'Herceghalom', '18.74520000', '47.49710000'),
(202, 'Sajólád', '20.90300000', '48.04360000'),
(203, 'Sajóbábony', '20.73730000', '48.17420000'),
(204, 'Sárisáp', '18.68100000', '47.67470000'),
(205, 'Rétság', '19.13670000', '47.92960000'),
(206, 'Andornaktálya', '20.41170000', '47.84440000'),
(207, 'Forró', '21.08800000', '48.32420000'),
(208, 'Répcelak', '17.01840000', '47.42110000'),
(209, 'Pócsmegyer', '19.10000000', '47.71670000'),
(210, 'Úrhida', '18.33190000', '47.13060000'),
(211, 'Petőfibánya', '19.71670000', '47.76670000'),
(212, 'Péteri', '19.41670000', '47.38330000'),
(213, 'Szigetcsép', '18.98330000', '47.26670000'),
(214, 'Verőcemaros', '19.03330000', '47.83330000'),
(215, 'Kisbocskaykert', '21.65830000', '47.64230000'),
(216, 'Győrújfalu', '17.60840000', '47.72240000'),
(217, 'Szigetszentmárton', '18.96670000', '47.23330000'),
(218, 'Kaposmérő', '17.70240000', '46.36010000');