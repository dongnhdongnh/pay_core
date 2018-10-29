using System.Collections.Generic;

namespace Vakapay.Commons.Constants
{
    public class Timezone
    {
        public static readonly Dictionary<string, string> LIST_TIME_ZONE = new Dictionary<string, string>
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
    }
}