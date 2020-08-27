using System.Collections.Generic;
using System.Linq;

namespace SearchNewsProject
{
    public class Classify
    {
        private List<ListItem> mainList;
        private List<ListItem> gundemList = new List<ListItem>();
        private List<ListItem> educationList = new List<ListItem>();
        private List<ListItem> economyList = new List<ListItem>();
        private List<ListItem> worldList = new List<ListItem>();
        private List<ListItem> sportList = new List<ListItem>();
        private List<ListItem> healthList = new List<ListItem>();
        private List<ListItem> techList = new List<ListItem>();

        public Classify(List<ListItem> list)
        {
            mainList = list;
        }

        public void categorise()
        {
            int gundemCounter = 0;
            int eduCounter = 0;
            int ecoCounter = 0;
            int worldCounter = 0;
            int sportCounter = 0;
            int healthCounter = 0;
            int techCounter = 0;
            int result = 0;

            string[] gundemKeysTR = {
                "bakan","doğu akdeniz","erdoğan","trump","savaş","abd","türkiye",
                "sağlık bakanı","fahrettin koca","yunanistan","suriye","dışişleri",
                "şehit","imamoğlu","akşener","mansur","fetö","cinayet","korona","aşı",
                "karantina","sel", "yök","atama","intihar","çin","rusya","doğal gaz",
                "benzin","zam","albayrak","ekonomi","indirim","cumhurbaşkanı","içişleri",
                "pkk","pyd","terör","yangın","salgın","gündem","deprem","felaket","katliam"
            };

            string[] educationKeysTR = {
                "eğitim","öğretim","okul","lise","üniversite","kpss",
                "atama","milli eğitim","meb","yök","tyt","ayt","mezun","öğrenci","öğretmen",
                "matematik","fizik","kimya","biyoloji","sayısal","sözel","yds","burs",
                "uzaktan eğitim","yks","lgs","ösym","öss","önlisans","lisans","harç","diploma",
                "yekta saraç","eba","tercih","transkript","fakülte","tıp","sınıf","dönem","yaz tatil",
                "aöl","kreş","anaokul","kyk"
            };

            string[] economyKeysTR = {
                "ekonomi","döviz","dolar","lira","altın","euro","sterlin","albayrak","kripto",
                "para","yatırım","konkordato","iflas","varank","sgk","imf","banka","hisse",
                "bitcoin","petrol","piyasa","ihracat","ithalat","tarım","borç","fiyat","finans",
                "kobi","kredi","borsa"
            };

            string[] worldKeysTR ={
                "dünya","türkiye","rusya","çin","suriye","fransa","yunanistan","kriz","savaş",
                "dışişleri","korona","aşı","abd","ingiltere","nato","bm","trump","erdoğan",
                "almanya","ırk","iran","ırak","avrupa","akdeniz","karadeniz",
            };

            string[] sportKeysTR = {
                "spor","futbol","basketbol","voleybol","formula","süperlig","premier","lig","nba",
                "uefa","şampiyon","galatasaray","barcelona","bayern","psg","fenerbahçe","beşiktaş",
                "trabzon","başakşehir","fatih terim","mustafa cengiz","ali koç","iddia","transfer",
                "sergen","gol","teknik direktör","bonservis","real madrid","juventus","ronaldo",
                "tenis","yüzme","boks","olimpiyat","madalya","milli takım","tff","mhk","la liga",
                "serie a","bundesliga","wnba","euroleague","bein","sport"
            };

            string[] healthKeysTR = {
                "sağlık","who","fahrettin koca","korona","aşı","grip","enfeksiyon","sancı",
                "hekim","kene","vaka","kovid-19","covid","virüs","test","sağlı","bilim insa",
                "tedavi","vitamin","bakan koca","salgın","bakteri","maske","ağrı","hasta","kanser",
                "fitness","beslenme","kilo","diyet","dünya sağlık örgüt"
            };

            string[] techKeysTR = {
                "teknoloji","samsung","apple","telefon","bilgisayar","televizyon","tablet","ios",
                "android","huawei","xiaomi","beta","youtube","oyun","playstation","ps4","ps5","xbox",
                "nintendo","konsol","facebook","instagram","twitch","wi-fi","wps","lte","modem",
                "epic","steam","playstore","appstore","google","vivo","oppo","internet","sosyal medya",
                "bilişim","yazılım","cihaz","uygulama","hack","tiktok","online","offline","yapay zeka",
                "ai","türk telekom","turkcell","5g","4g","twitter","whatsapp","telegram","bilim","sanayi",
                "endüstri","microsoft","gpt-3","nasa","mars","zuckerberg","amazon","jeff bezos","elon musk",
                "space","tesla","windows","linux","yapay gerçek","yahoo","bing"
            };

            //List<List<string>> parentList = new List<List<string>>();
            //List<string> gundemKeysTRList = new List<string>();
            //List<string> eduKeysTRList = new List<string>();
            //List<string> ecoKeysTRList = new List<string>();
            //List<string> worldKeysTRList = new List<string>();
            //List<string> sportKeysTRList = new List<string>();
            //List<string> healthKeysTRList = new List<string>();
            //List<string> techKeysTRList = new List<string>();

            //for (int i = 0; i < 7; i++)
            //{
            //    if (i == 0)
            //    {
            //        parentList.Add(addKeysToList(gundemKeysTRList, gundemKeysTR));
            //    }
            //    else if (i == 1)
            //    {
            //        parentList.Add(addKeysToList(eduKeysTRList, educationKeysTR));
            //    }
            //    else if (i == 2)
            //    {
            //        parentList.Add(addKeysToList(ecoKeysTRList, economyKeysTR));
            //    }
            //    else if (i == 3)
            //    {
            //        parentList.Add(addKeysToList(worldKeysTRList, worldKeysTR));
            //    }
            //    else if (i == 4)
            //    {
            //        parentList.Add(addKeysToList(sportKeysTRList, sportKeysTR));
            //    }
            //    else if (i == 5)
            //    {
            //        parentList.Add(addKeysToList(healthKeysTRList, healthKeysTR));
            //    }
            //    else if (i == 6)
            //    {
            //        parentList.Add(addKeysToList(techKeysTRList, techKeysTR));
            //    }
            //}

            for (int i = 0; i < mainList.Count; i++)
            {
                string content = mainList.ElementAt(i).Content;
                gundemCounter = refreshCounter(content, gundemKeysTR);
                eduCounter = refreshCounter(content, educationKeysTR);
                ecoCounter = refreshCounter(content, economyKeysTR);
                worldCounter = refreshCounter(content, worldKeysTR);
                sportCounter = refreshCounter(content, sportKeysTR);
                healthCounter = refreshCounter(content, healthKeysTR);
                techCounter = refreshCounter(content, techKeysTR);

                result = findMax(gundemCounter, eduCounter, ecoCounter, worldCounter, sportCounter, healthCounter, techCounter);

                if (result == gundemCounter)
                {
                    gundemList.Add(mainList.ElementAt(i));
                }
                if (result == eduCounter)
                {
                    educationList.Add(mainList.ElementAt(i));
                }
                if (result == ecoCounter)
                {
                    economyList.Add(mainList.ElementAt(i));
                }
                if (result == worldCounter)
                {
                    worldList.Add(mainList.ElementAt(i));
                }
                if (result == sportCounter)
                {
                    sportList.Add(mainList.ElementAt(i));
                }
                if (result == healthCounter)
                {
                    healthList.Add(mainList.ElementAt(i));
                }
                if (result == techCounter)
                {
                    techList.Add(mainList.ElementAt(i));
                }
            }
        }

        //private List<string> addKeysToList(List<string> list, string[] keywords)
        //{
        //    for (int i = 0; i < keywords.Length; i++)
        //    {
        //        list.Add(keywords[i]);
        //    }

        //    return list;
        //}

        private int refreshCounter(string content, string[] keyWords)
        {
            int counter = 0;

            for (int i = 0; i < keyWords.Length; i++)
            {
                if (content.Contains(keyWords[i]))
                {
                    counter++;
                }
            }

            return counter;
        }

        private int findMax(int a, int b, int c, int d, int e, int f, int g)
        {
            List<int> numbers = new List<int>();
            numbers.Add(a);
            numbers.Add(b);
            numbers.Add(c);
            numbers.Add(d);
            numbers.Add(e);
            numbers.Add(f);
            numbers.Add(g);

            return numbers.Max();
        }

        public List<ListItem> getGundemList()
        {
            return gundemList;
        }

        public List<ListItem> getEduList()
        {
            return educationList;
        }

        public List<ListItem> getEcoList()
        {
            return economyList;
        }

        public List<ListItem> getWorldList()
        {
            return worldList;
        }

        public List<ListItem> getSportList()
        {
            return sportList;
        }

        public List<ListItem> getHealthList()
        {
            return healthList;
        }

        public List<ListItem> getTechList()
        {
            return techList;
        }
    }
}