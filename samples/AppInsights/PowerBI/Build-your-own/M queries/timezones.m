// Power Query for a table that holds all timezones (and their UTC offsets) with a formatting similar to what is shown in Microsoft Windows

#table(
 type table
    [
        #"Display name"=text, 
        #"UTC offset"=number
    ], 
 {
{ "(UTC-12:00) International Date Line West" , 12 }
, { "(UTC-11:00) Coordinated Universal Time-11" , 11 }
, { "(UTC-10:00) Aleutian Islands" , 10 }
, { "(UTC-10:00) Hawaii" , 10 }
, { "(UTC-09:30) Marqueseas Islands" , 09.5 }
, { "(UTC-09:00) Alaska" , 09 }
, { "(UTC-09:00) Coordinated Universal Time-09" , 09 }
, { "(UTC-08:00) Baja California" , 08 }
, { "(UTC-07:00) Pacific Daylight Time (US & Canada)" , 07 }
, { "(UTC-08:00) Coordinated Universal Time-08" , 08 }
, { "(UTC-08:00) Pacific Standard Time (US & Canada)" , 08 }
, { "(UTC-07:00) Arizona" , 07 }
, { "(UTC-07:00) Chihuahua, La Paz, Mazatlan" , 07 }
, { "(UTC-07:00) Mountain Time (US & Canada)" , 07 }
, { "(UTC-07:00) Yukon" , 07 }
, { "(UTC-06:00) Central America" , 06 }
, { "(UTC-06:00) Central Time (US & Canada)" , 06 }
, { "(UTC-06:00) Easter Island" , 06 }
, { "(UTC-06:00) Guadalajara, Mexico City, Monterrey" , 06 }
, { "(UTC-06:00) Saskatchewan" , 06 }
, { "(UTC-05:00) Bogota, Lima, Quito" , 05 }
, { "(UTC-05:00) Chetumal" , 05 }
, { "(UTC-05:00) Eastern Time (US & Canada)" , 05 }
, { "(UTC-04:00) Eastern Daylight Time (US & Canada)" , 04 }
, { "(UTC-05:00) Haiti" , 05 }
, { "(UTC-05:00) Havana" , 05 }
, { "(UTC-05:00) Indiana (East)" , 05 }
, { "(UTC-05:00) Turks and Caicos" , 05 }
, { "(UTC-04:00) Caracas" , 04 }
, { "(UTC-04:00) Cuiaba" , 04 }
, { "(UTC-04:00) Georgetown, La Paz, Manaus, San Juan" , 04 }
, { "(UTC-04:00) Santiago" , 04 }
, { "(UTC-03:30) Newfoundland" , 03.5 }
, { "(UTC-03:00) Araguaina" , 03 }
, { "(UTC-03:00) Brasilia" , 03 }
, { "(UTC-03:00) Cayenne, Fortaleza" , 03 }
, { "(UTC-03:00) City of Buenos Aires" , 03 }
, { "(UTC-03:00) Greenland" , 03 }
, { "(UTC-03:00) Montevideo" , 03 }
, { "(UTC-03:00) Punta Arenas" , 03 }
, { "(UTC-03:00) Saint Pierre and Miquelon" , 03 }
, { "(UTC-03:00) Salvador" , 03 }
, { "(UTC-02:00) Coordinated Universal Time-02" , 02 }
, { "(UTC-01:00) Azores" , 01 }
, { "(UTC-01:00) Cape Verde Is." , 01 }
, { "(UTC) Coordinated Universal Time" ,  0 }
, { "(UTC+00:00) Dublin, Edinburgh, London, Lisbon" , 00 }
, { "(UTC+00:00) Monrovia, Reykjavik" , 00 }
, { "(UTC+00:00) Sao Tome" , 00 }
, { "(UTC+00:00) Casablanca" , 00 }
, { "(UTC+01:00) Amsterdam, Berlin, Bern, Rome, Stockholm, Vienna" , 01 }
, { "(UTC+01:00) Belgrade, Bratislava, Budapest, Ljubljana, Prague" , 01 }
, { "(UTC+01:00) Brussels, Copenhagen, Madrid, Paris" , 01 }
, { "(UTC+01:00) Sarajevo, Skopje, Warsaw, Zagreb" , 01 }
, { "(UTC+01:00) West Central Africa" , 01 }
, { "(UTC+02:00) Amman" , 02 }
, { "(UTC+02:00) Athens, Bucharest" , 02 }
, { "(UTC+02:00) Beirut" , 02 }
, { "(UTC+02:00) Cairo" , 02 }
, { "(UTC+02:00) Chisinau" , 02 }
, { "(UTC+02:00) Damascus" , 02 }
, { "(UTC+02:00) Gaza, Hebron" , 02 }
, { "(UTC+02:00) Harare, Pretoria" , 02 }
, { "(UTC+02:00) Helsinki, Kyiv, Riga, Sofia, Tallinn, Vilnius" , 02 }
, { "(UTC+02:00) Jerusalem" , 02 }
, { "(UTC+02:00) Juba" , 02 }
, { "(UTC+02:00) Kaliningrad" , 02 }
, { "(UTC+02:00) Khartoum" , 02 }
, { "(UTC+02:00) Tripoli" , 02 }
, { "(UTC+02:00) Windhoek" , 02 }
, { "(UTC+03:00) Baghdad" , 03 }
, { "(UTC+03:00) Istanbul" , 03 }
, { "(UTC+03:00) Kuwait, Riyadh" , 03 }
, { "(UTC+03:00) Minsk" , 03 }
, { "(UTC+03:00) Moscow, St. Petersburg" , 03 }
, { "(UTC+03:00) Nairobi" , 03 }
, { "(UTC+03:00) Volgograd" , 03 }
, { "(UTC+03:30) Tehran" , 03.5 }
, { "(UTC+04:00) Abu Dhabi, Muscat" , 04 }
, { "(UTC+04:00) Astrakhan, Ulyanovsk" , 04 }
, { "(UTC+04:00) Baku" , 04 }
, { "(UTC+04:00) Izhevsk, Samara" , 04 }
, { "(UTC+04:00) Port Louis" , 04 }
, { "(UTC+04:00) Saratov" , 04 }
, { "(UTC+04:00) Tbilisi" , 04 }
, { "(UTC+04:00) Yerevan" , 04 }
, { "(UTC+04:30) Kabul" , 04.5 }
, { "(UTC+05:00) Ashgabat, Tashkent" , 05 }
, { "(UTC+05:00) Yekaterinburg" , 05 }
, { "(UTC+05:00) Islamabad, Karachi" , 05 }
, { "(UTC+05:00) Qyzylorda" , 05 }
, { "(UTC+05:30) Chennai, Kolkata, Mumbai, New Delhi" , 05.5 }
, { "(UTC+05:30) Sri Jayawardenepura" , 05.5 }
, { "(UTC+05.75 }) Kathmandu" , 05.75 }
, { "(UTC+06:00) Astana" , 06 }
, { "(UTC+06:00) Dhaka" , 06 }
, { "(UTC+06:00) Omsk" , 06 }
, { "(UTC+06:30) Yangon (Rangoon)" , 06.5 }
, { "(UTC+07:00) Bangkok, Hanoi, Jakarta" , 07 }
, { "(UTC+07:00) Barnaul, Gorno-Altaysk" , 07 }
, { "(UTC+07:00) Hovd" , 07 }
, { "(UTC+07:00) Krasnoyarsk" , 07 }
, { "(UTC+07:00) Novosibirsk" , 07 }
, { "(UTC+07:00) Tomsk" , 07 }
, { "(UTC+08:00) Beijing, Chongqing, Hong Kong, Urumqi" , 08 }
, { "(UTC+08:00) Irkutsk" , 08 }
, { "(UTC+08:00) Kuala Lumpur, Singapore" , 08 }
, { "(UTC+08:00) Perth" , 08 }
, { "(UTC+08:00) Taipei" , 08 }
, { "(UTC+08:00) Ulaanbaatar" , 08 }
, { "(UTC+08.75 }) Eucla" , 08.75 }
, { "(UTC+09:00) Chita" , 09 }
, { "(UTC+09:00) Osaka, Sapporo, Tokyo" , 09 }
, { "(UTC+09:00) Pyongyang" , 09 }
, { "(UTC+09:00) Seoul" , 09 }
, { "(UTC+09:00) Yakutsk" , 09 }
, { "(UTC+09:30) Adelaide" , 09.5 }
, { "(UTC+09:30) Darwin" , 09.5 }
, { "(UTC+10:00) Brisbane" , 10 }
, { "(UTC+10:00) Canberra, Melbourne, Sydney" , 10 }
, { "(UTC+10:00) Guam, Port Moresby" , 10 }
, { "(UTC+10:00) Hobart" , 10 }
, { "(UTC+10:00) Vladivostok" , 10 }
, { "(UTC+10:30) Lord Howe Island" , 10.5 }
, { "(UTC+11:00) Bougainville Island" , 11 }
, { "(UTC+11:00) Chokurdakh" , 11 }
, { "(UTC+11:00) Magadan" , 11 }
, { "(UTC+11:00) Norfolk Island" , 11 }
, { "(UTC+11:00) Sakhalin" , 11 }
, { "(UTC+11:00) Solomon Is., New Caledonia" , 11 }
, { "(UTC+12:00) Anadyr, Petropavlovsk-Kamchatsky" , 12 }
, { "(UTC+12:00) Auckland, Wellington" , 12 }
, { "(UTC+12:00) Coordinated Universal Time+12" , 12 }
, { "(UTC+12:00) Fiji" , 12 }
, { "(UTC+12.75 }) Chatham Islands" , 12.75 }
, { "(UTC+13:00) Coordinated Universal Time+13" , 13 }
, { "(UTC+13:00) Nuku'alofa" , 13 }
, { "(UTC+13:00) Samoa" , 13 }
, { "(UTC+14:00) Kiritimati Island" , 14 }
 }
)