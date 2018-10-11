using System.Collections.Generic;

namespace Vakapay.Models
{
    public class Constants
    {

        public static Dictionary<string, string> listTimeZone = new Dictionary<string, string>
        {
            {"American Samoa", "(GMT-11:00) American Samoa"},
            {"International Date Line West", "(GMT-11:00) International Date Line West"},
            {"Midway Island", "(GMT-11:00) Midway Island"},
            {"Hawaii", "(GMT-10:00) Hawaii"},
            {"Alaska", "(GMT-09:00) Alaska"},
            {"Pacific Time (US & Canada)", "(GMT-08:00) Pacific Time (US & Canada)"},
            {"Tijuana", "(GMT-08:00) Tijuana"},
            {"Arizona", "(GMT-07:00) Arizona"},
            {"Chihuahua", "(GMT-07:00) Chihuahua"},
            {"Mazatlan", "(GMT-07:00) Mazatlan"},
            {"Mountain Time (US & Canada)", "(GMT-07:00) Mountain Time (US & Canada)"},
            {"Central America", "(GMT-06:00) Central America"},
            {"Central Time (US & Canada)", "(GMT-06:00) Central Time (US & Canada)"},
            {"Guadalajara", "(GMT-06:00) Guadalajara"},
            {"Mexico City", "(GMT-06:00) Mexico City"},
            {"Monterrey", "(GMT-06:00) Monterrey"},
            {"Saskatchewan", "(GMT-06:00) Saskatchewan"},
            {"Bogota", "(GMT-05:00) Bogota"},
            {"Eastern Time (US & Canada)", "(GMT-05:00) Eastern Time (US & Canada)"},
            {"Indiana (East)", "(GMT-05:00) Indiana (East)"},
            {"Lima", "(GMT-05:00) Lima"},
            {"Quito", "(GMT-05:00) Quito"},
            {"Atlantic Time (Canada)", "(GMT-04:00) Atlantic Time (Canada)"},
            {"Caracas", "(GMT-04:00) Caracas"},
            {"Georgetown", "(GMT-04:00) Georgetown"},
            {"La Paz", "(GMT-04:00) La Paz"},
            {"Santiago", "(GMT-04:00) Santiago"},
            {"Newfoundland", "(GMT-03:30) Newfoundland"},
            {"Brasilia", "(GMT-03:00) Brasilia"},
            {"Buenos Aires", "(GMT-03:00) Buenos Aires"},
            {"Greenland", "(GMT-03:00) Greenland"},
            {"Montevideo", "(GMT-03:00) Montevideo"},
            {"Mid-Atlantic", "(GMT-02:00) Mid-Atlantic"},
            {"Azores", "(GMT-01:00) Azores"},
            {"Cape Verde Is.", "(GMT-01:00) Cape Verde Is."},
            {"Casablanca", "(GMT+00:00) Casablanca"},
            {"Edinburgh", "(GMT+00:00) Edinburgh"},
            {"Lisbon", "(GMT+00:00) Lisbon"},
            {"London", "(GMT+00:00) London"},
            {"Monrovia", "(GMT+00:00) Monrovia"},
            {"UTC", "(GMT+00:00) UTC"},
            {"Amsterdam", "(GMT+01:00) Amsterdam"},
            {"Belgrade", "(GMT+01:00) Belgrade"},
            {"Berlin", "(GMT+01:00) Berlin"},
            {"Bern", "(GMT+01:00) Bern"},
            {"Bratislava", "(GMT+01:00) Bratislava"},
            {"Brussels", "(GMT+01:00) Brussels"},
            {"Budapest", "(GMT+01:00) Budapest"},
            {"Copenhagen", "(GMT+01:00) Copenhagen"},
            {"Dublin", "(GMT+01:00) Dublin"},
            {"Europe/Berlin", "(GMT+01:00) Europe/Berlin"},
            {"Ljubljana", "(GMT+01:00) Ljubljana"},
            {"Madrid", "(GMT+01:00) Madrid"},
            {"Paris", "(GMT+01:00) Paris"},
            {"Prague", "(GMT+01:00) Prague"},
            {"Rome", "(GMT+01:00) Rome"},
            {"Sarajevo", "(GMT+01:00) Sarajevo"},
            {"Skopje", "(GMT+01:00) Skopje"},
            {"Stockholm", "(GMT+01:00) Stockholm"},
            {"Vienna", "(GMT+01:00) Vienna"},
            {"Warsaw", "(GMT+01:00) Warsaw"},
            {"West Central Africa", "(GMT+01:00) West Central Africa"},
            {"Zagreb", "(GMT+01:00) Zagreb"},
            {"Zurich", "(GMT+01:00) Zurich"},
            {"Athens", "(GMT+02:00) Athens"},
            {"Bucharest", "(GMT+02:00) Bucharest"},
            {"Cairo", "(GMT+02:00) Cairo"},
            {"Harare", "(GMT+02:00) Harare"},
            {"Helsinki", "(GMT+02:00) Helsinki"},
            {"Jerusalem", "(GMT+02:00) Jerusalem"},
            {"Kaliningrad", "(GMT+02:00) Kaliningrad"},
            {"Kyiv", "(GMT+02:00) Kyiv"},
            {"Pretoria", "(GMT+02:00) Pretoria"},
            {"Riga", "(GMT+02:00) Riga"},
            {"Sofia", "(GMT+02:00) Sofia"},
            {"Tallinn", "(GMT+02:00) Tallinn"},
            {"Vilnius", "(GMT+02:00) Vilnius"},
            {"Baghdad", "(GMT+03:00) Baghdad"},
            {"Istanbul", "(GMT+03:00) Istanbul"},
            {"Kuwait", "(GMT+03:00) Kuwait"},
            {"Minsk", "(GMT+03:00) Minsk"},
            {"Moscow", "(GMT+03:00) Moscow"},
            {"Nairobi", "(GMT+03:00) Nairobi"},
            {"Riyadh", "(GMT+03:00) Riyadh"},
            {"St. Petersburg", "(GMT+03:00) St. Petersburg"},
            {"Volgograd", "(GMT+03:00) Volgograd"},
            {"Tehran", "(GMT+03:30) Tehran"},
            {"Abu Dhabi", "(GMT+04:00) Abu Dhabi"},
            {"Baku", "(GMT+04:00) Baku"},
            {"Muscat", "(GMT+04:00) Muscat"},
            {"Samara", "(GMT+04:00) Samara"},
            {"Tbilisi", "(GMT+04:00) Tbilisi"},
            {"Yerevan", "(GMT+04:00) Yerevan"},
            {"Kabul", "(GMT+04:30) Kabul"},
            {"Ekaterinburg", "(GMT+05:00) Ekaterinburg"},
            {"Islamabad", "(GMT+05:00) Islamabad"},
            {"Karachi", "(GMT+05:00) Karachi"},
            {"Tashkent", "(GMT+05:00) Tashkent"},
            {"Chennai", "(GMT+05:30) Chennai"},
            {"Kolkata", "(GMT+05:30) Kolkata"},
            {"Mumbai", "(GMT+05:30) Mumbai"},
            {"New Delhi", "(GMT+05:30) New Delhi"},
            {"Sri Jayawardenepura", "(GMT+05:30) Sri Jayawardenepura"},
            {"Kathmandu", "(GMT+05:45) Kathmandu"},
            {"Almaty", "(GMT+06:00) Almaty"},
            {"Astana", "(GMT+06:00) Astana"},
            {"Dhaka", "(GMT+06:00) Dhaka"},
            {"Urumqi", "(GMT+06:00) Urumqi"},
            {"Rangoon", "(GMT+06:30) Rangoon"},
            {"Bangkok", "(GMT+07:00) Bangkok"},
            {"Hanoi", "(GMT+07:00) Hanoi"},
            {"Jakarta", "(GMT+07:00) Jakarta"},
            {"Krasnoyarsk", "(GMT+07:00) Krasnoyarsk"},
            {"Novosibirsk", "(GMT+07:00) Novosibirsk"},
            {"Beijing", "(GMT+08:00) Beijing"},
            {"Chongqing", "(GMT+08:00) Chongqing"},
            {"Hong Kong", "(GMT+08:00) Hong Kong"},
            {"Irkutsk", "(GMT+08:00) Irkutsk"},
            {"Kuala Lumpur", "(GMT+08:00) Kuala Lumpur"},
            {"Perth", "(GMT+08:00) Perth"},
            {"Singapore", "(GMT+08:00) Singapore"},
            {"Taipei", "(GMT+08:00) Taipei"},
            {"Ulaanbaatar", "(GMT+08:00) Ulaanbaatar"},
            {"Osaka", "(GMT+09:00) Osaka"},
            {"Sapporo", "(GMT+09:00) Sapporo"},
            {"Seoul", "(GMT+09:00) Seoul"},
            {"Tokyo", "(GMT+09:00) Tokyo"},
            {"Yakutsk", "(GMT+09:00) Yakutsk"},
            {"Adelaide", "(GMT+09:30) Adelaide"},
            {"Darwin", "(GMT+09:30) Darwin"},
            {"Brisbane", "(GMT+10:00) Brisbane"},
            {"Canberra", "(GMT+10:00) Canberra"},
            {"Guam", "(GMT+10:00) Guam"},
            {"Hobart", "(GMT+10:00) Hobart"},
            {"Melbourne", "(GMT+10:00) Melbourne"},
            {"Port Moresby", "(GMT+10:00) Port Moresby"},
            {"Sydney", "(GMT+10:00) Sydney"},
            {"Vladivostok", "(GMT+10:00) Vladivostok"},
            {"Magadan", "(GMT+11:00) Magadan"},
            {"New Caledonia", "(GMT+11:00) New Caledonia"},
            {"Solomon Is.", "(GMT+11:00) Solomon Is."},
            {"Srednekolymsk", "(GMT+11:00) Srednekolymsk"},
            {"Auckland", "(GMT+12:00) Auckland"},
            {"Fiji", "(GMT+12:00) Fiji"},
            {"Kamchatka", "(GMT+12:00) Kamchatka"},
            {"Marshall Is.", "(GMT+12:00) Marshall Is."},
            {"Wellington", "(GMT+12:00) Wellington"},
            {"Chatham Is.", "(GMT+12:45) Chatham Is."},
            {"Nuku'alofa", "(GMT+13:00) Nuku'alofa"},
            {"Samoa", "(GMT+13:00) Samoa"},
            {"Tokelau Is.", "(GMT+13:00) Tokelau Is."}
        };

        public static Dictionary<string, string> listCurrency = new Dictionary<string, string>
        {
            {
                "USD",
                "United States Dollar (USD)"
            },
            {
                "EUR",
                "Euro (EUR)"
            },
            {
                "CNY",
                "Chinese Renminbi Yuan (CNY)"
            },
            {
                "GBP",
                "British Pound (GBP)"
            },
            {
                "CAD",
                "Canadian Dollar (CAD)"
            },
            {
                "AFN",
                "Afghan Afghani (AFN)"
            },
            {
                "ALL",
                "Albanian Lek (ALL)"
            },
            {
                "DZD",
                "Algerian Dinar (DZD)"
            },
            {
                "ARS",
                "Argentine Peso (ARS)"
            },
            {
                "AMD",
                "Armenian Dram (AMD)"
            },
            {
                "AWG",
                "Aruban Florin (AWG)"
            },
            {
                "AOA",
                "Aurora (AOA)"
            },
            {
                "AUD",
                "Australian Dollar (AUD)"
            },
            {
                "AZN",
                "Azerbaijani Manat (AZN)"
            },
            {
                "BSD",
                "Bahamian Dollar (BSD)"
            },
            {
                "BHD",
                "Bahraini Dinar (BHD)"
            },
            {
                "BDT",
                "Bangladeshi Taka (BDT)"
            },
            {
                "BBD",
                "Barbadian Dollar (BBD)"
            },
            {
                "BYN",
                "Belarusian Ruble (BYN)"
            },
            {
                "BYR",
                "Belarusian Ruble (BYR)"
            },
            {
                "BZD",
                "Belize Dollar (BZD)"
            },
            {
                "BMD",
                "Bermudian Dollar (BMD)"
            },
            {
                "BTN",
                "Bhutanese Ngultrum (BTN)"
            },
            {
                "BOB",
                "Bolivian Boliviano (BOB)"
            },
            {
                "BAM",
                "Bosnia and Herzegovina Convertible Mark (BAM)"
            },
            {
                "BWP",
                "Botswana Pula (BWP)"
            },
            {
                "BRL",
                "Brazilian Real (BRL)"
            },
            {
                "BND",
                "Brunei Dollar (BND)"
            },
            {
                "BGN",
                "Bulgarian Lev (BGN)"
            },
            {
                "BIF",
                "Burundian Franc (BIF)"
            },
            {
                "KHR",
                "Cambodian Riel (KHR)"
            },
            {
                "CVE",
                "Cape Verdean Escudo (CVE)"
            },
            {
                "KYD",
                "Cayman Islands Dollar (KYD)"
            },
            {
                "XAF",
                "Central African Cfa Franc (XAF)"
            },
            {
                "XPF",
                "Cfp Franc (XPF)"
            },
            {
                "CLP",
                "Chilean Peso (CLP)"
            },
            {
                "CNH",
                "Chinese Renminbi Yuan Offshore (CNH)"
            },
            {
                "COP",
                "Colombian Peso (COP)"
            },
            {
                "KMF",
                "Comorian Franc (KMF)"
            },
            {
                "CDF",
                "Congolese Franc (CDF)"
            },
            {
                "CRC",
                "Costa Rican Colón (CRC)"
            },
            {
                "HRK",
                "Croatian Kuna (HRK)"
            },
            {
                "CUC",
                "Cuban Convertible Peso (CUC)"
            },
            {
                "CZK",
                "Czech Koruna (CZK)"
            },
            {
                "DKK",
                "Danish Krone (DKK)"
            },
            {
                "DJF",
                "Djiboutian Franc (DJF)"
            },
            {
                "DOP",
                "Dominican Peso (DOP)"
            },
            {
                "XCD",
                "East Caribbean Dollar (XCD)"
            },
            {
                "EGP",
                "Egyptian Pound (EGP)"
            },
            {
                "ERN",
                "Eritrean Nakfa (ERN)"
            },
            {
                "EEK",
                "Estonian Kroon (EEK)"
            },
            {
                "ETB",
                "Ethiopian Birr (ETB)"
            },
            {
                "FKP",
                "Falkland Pound (FKP)"
            },
            {
                "FJD",
                "Fijian Dollar (FJD)"
            },
            {
                "GMD",
                "Gambian Dalasi (GMD)"
            },
            {
                "GEL",
                "Georgian Lari (GEL)"
            },
            {
                "GHS",
                "Ghanaian Cedi (GHS)"
            },
            {
                "GIP",
                "Gibraltar Pound (GIP)"
            },
            {
                "XAU",
                "Gold (Troy Ounce) (XAU)"
            },
            {
                "GTQ",
                "Guatemalan Quetzal (GTQ)"
            },
            {
                "GGP",
                "Guernsey Pound (GGP)"
            },
            {
                "GNF",
                "Guinean Franc (GNF)"
            },
            {
                "GYD",
                "Guyanese Dollar (GYD)"
            },
            {
                "HTG",
                "Haitian Gourde (HTG)"
            },
            {
                "HNL",
                "Honduran Lempira (HNL)"
            },
            {
                "HKD",
                "Hong Kong Dollar (HKD)"
            },
            {
                "HUF",
                "Hungarian Forint (HUF)"
            },
            {
                "ISK",
                "Icelandic Króna (ISK)"
            },
            {
                "INR",
                "Indian Rupee (INR)"
            },
            {
                "IDR",
                "Indonesian Rupiah (IDR)"
            },
            {
                "IQD",
                "Iraqi Dinar (IQD)"
            },
            {
                "IMP",
                "Isle of Man Pound (IMP)"
            },
            {
                "ILS",
                "Israeli New Sheqel (ILS)"
            },
            {
                "JMD",
                "Jamaican Dollar (JMD)"
            },
            {
                "JPY",
                "Japanese Yen (JPY)"
            },
            {
                "JEP",
                "Jersey Pound (JEP)"
            },
            {
                "JOD",
                "Jordanian Dinar (JOD)"
            },
            {
                "KZT",
                "Kazakhstani Tenge (KZT)"
            },
            {
                "KES",
                "Kenyan Shilling (KES)"
            },
            {
                "KWD",
                "Kuwaiti Dinar (KWD)"
            },
            {
                "KGS",
                "Kyrgyzstani Som (KGS)"
            },
            {
                "LAK",
                "Lao Kip (LAK)"
            },
            {
                "LVL",
                "Latvian Lats (LVL)"
            },
            {
                "LBP",
                "Lebanese Pound (LBP)"
            },
            {
                "LSL",
                "Lesotho Loti (LSL)"
            },
            {
                "LRD",
                "Liberian Dollar (LRD)"
            },
            {
                "LYD",
                "Libyan Dinar (LYD)"
            },
            {
                "LTL",
                "Lithuanian Litas (LTL)"
            },
            {
                "MOP",
                "Macanese Pataca (MOP)"
            },
            {
                "MKD",
                "Macedonian Denar (MKD)"
            },
            {
                "MGA",
                "Malagasy Ariary (MGA)"
            },
            {
                "MWK",
                "Malawian Kwacha (MWK)"
            },
            {
                "MYR",
                "Malaysian Ringgit (MYR)"
            },
            {
                "MVR",
                "Maldivian Rufiyaa (MVR)"
            },
            {
                "MTL",
                "Maltese Lira (MTL)"
            },
            {
                "MRO",
                "Mauritanian Ouguiya (MRO)"
            },
            {
                "MUR",
                "Mauritian Rupee (MUR)"
            },
            {
                "MXN",
                "Mexican Peso (MXN)"
            },
            {
                "MDL",
                "Moldovan Leu (MDL)"
            },
            {
                "MNT",
                "Mongolian Tögrög (MNT)"
            },
            {
                "MAD",
                "Moroccan Dirham (MAD)"
            },
            {
                "MZN",
                "Mozambican Metical (MZN)"
            },
            {
                "MMK",
                "Myanmar Kyat (MMK)"
            },
            {
                "NAD",
                "Namibian Dollar (NAD)"
            },
            {
                "NPR",
                "Nepalese Rupee (NPR)"
            },
            {
                "ANG",
                "Netherlands Antillean Gulden (ANG)"
            },
            {
                "TWD",
                "New Taiwan Dollar (TWD)"
            },
            {
                "NZD",
                "New Zealand Dollar (NZD)"
            },
            {
                "NIO",
                "Nicaraguan Córdoba (NIO)"
            },
            {
                "NGN",
                "Nigerian Naira (NGN)"
            },
            {
                "NOK",
                "Norwegian Krone (NOK)"
            },
            {
                "OMR",
                "Omani Rial (OMR)"
            },
            {
                "PKR",
                "Pakistani Rupee (PKR)"
            },
            {
                "XPD",
                "Palladium (XPD)"
            },
            {
                "PAB",
                "Panamanian Balboa (PAB)"
            },
            {
                "PGK",
                "Papua New Guinean Kina (PGK)"
            },
            {
                "PYG",
                "Paraguayan Guaraní (PYG)"
            },
            {
                "PEN",
                "Peruvian Sol (PEN)"
            },
            {
                "PHP",
                "Philippine Peso (PHP)"
            },
            {
                "XPT",
                "Platinum (XPT)"
            },
            {
                "PLN",
                "Polish Z?oty (PLN)"
            },
            {
                "QAR",
                "Qatari Riyal (QAR)"
            },
            {
                "RON",
                "Romanian Leu (RON)"
            },
            {
                "RUB",
                "Russian Ruble (RUB)"
            },
            {
                "RWF",
                "Rwandan Franc (RWF)"
            },
            {
                "SHP",
                "Saint Helenian Pound (SHP)"
            },
            {
                "SVC",
                "Salvadoran Colón (SVC)"
            },
            {
                "WST",
                "Samoan Tala (WST)"
            },
            {
                "SAR",
                "Saudi Riyal (SAR)"
            },
            {
                "RSD",
                "Serbian Dinar (RSD)"
            },
            {
                "SCR",
                "Seychellois Rupee (SCR)"
            },
            {
                "SLL",
                "Sierra Leonean Leone (SLL)"
            },
            {
                "XAG",
                "Silver (Troy Ounce) (XAG)"
            },
            {
                "SGD",
                "Singapore Dollar (SGD)"
            },
            {
                "SBD",
                "Solomon Islands Dollar (SBD)"
            },
            {
                "SOS",
                "Somali Shilling (SOS)"
            },
            {
                "ZAR",
                "South African Rand (ZAR)"
            },
            {
                "KRW",
                "South Korean Won (KRW)"
            },
            {
                "SSP",
                "South Sudanese Pound (SSP)"
            },
            {
                "XDR",
                "Special Drawing Rights (XDR)"
            },
            {
                "LKR",
                "Sri Lankan Rupee (LKR)"
            },
            {
                "SRD",
                "Surinamese Dollar (SRD)"
            },
            {
                "SZL",
                "Swazi Lilangeni (SZL)"
            },
            {
                "SEK",
                "Swedish Krona (SEK)"
            },
            {
                "CHF",
                "Swiss Franc (CHF)"
            },
            {
                "STD",
                "S?o Tomé and Príncipe Dobra (STD)"
            },
            {
                "TJS",
                "Tajikistani Somoni (TJS)"
            },
            {
                "TZS",
                "Tanzanian Shilling (TZS)"
            },
            {
                "THB",
                "Thai Baht (THB)"
            },
            {
                "TOP",
                "Tongan Pa?anga (TOP)"
            },
            {
                "TTD",
                "Trinidad and Tobago Dollar (TTD)"
            },
            {
                "TND",
                "Tunisian Dinar (TND)"
            },
            {
                "TRY",
                "Turkish Lira (TRY)"
            },
            {
                "TMT",
                "Turkmenistani Manat (TMT)"
            },
            {
                "UGX",
                "Ugandan Shilling (UGX)"
            },
            {
                "UAH",
                "Ukrainian Hryvnia (UAH)"
            },
            {
                "CLF",
                "Unidad de Fomento (CLF)"
            },
            {
                "AED",
                "United Arab Emirates Dirham (AED)"
            },
            {
                "UYU",
                "Uruguayan Peso (UYU)"
            },
            {
                "UZS",
                "Uzbekistan Som (UZS)"
            },
            {
                "VUV",
                "Vanuatu Vatu (VUV)"
            },
            {
                "VEF",
                "Venezuelan Bolívar (VEF)"
            },
            {
                "VND",
                "Vietnamese Ðong (VND)"
            },
            {
                "XOF",
                "West African Cfa Franc (XOF)"
            },
            {
                "YER",
                "Yemeni Rial (YER)"
            },
            {
                "ZMK",
                "Zambian Kwacha (ZMK)"
            },
            {
                "ZMW",
                "Zambian Kwacha (ZMW)"
            },
            {
                "ZWL",
                "Zimbabwean Dollar (ZWL)"
            }
        };
    }
}