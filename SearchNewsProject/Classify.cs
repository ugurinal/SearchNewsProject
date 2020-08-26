using System.Collections.Generic;

namespace SearchNewsProject
{
    public class Classify
    {
        private List<ListItem> mainList;

        public Classify(List<ListItem> list)
        {
            mainList = list;
        }

        public List<ListItem> categorise()
        {
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
                "dışişleri","korona","aşı","abd","ingiltere"
            };
            return null;
        }
    }
}