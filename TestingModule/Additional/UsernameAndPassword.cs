using System;
using System.Collections.Generic;

namespace TestingModule.Additional
{
    public class UsernameAndPassword
    {
        Dictionary<string, string> dictionaryChar = new Dictionary<string, string>()
            {
                {"'",""},
                {"а","a"},
                {"б","b"},
                {"в","v"},
                {"г","g"},
                {"ґ","g"},
                {"д","d"},
                {"е","e"},
                {"є","e"},
                {"ё","yo"},
                {"ж","zh"},
                {"з","z"},
                {"и","i"},
                {"і","i"},
                {"ї","i"},
                {"й","y"},
                {"к","k"},
                {"л","l"},
                {"м","m"},
                {"н","n"},
                {"о","o"},
                {"п","p"},
                {"р","r"},
                {"с","s"},
                {"т","t"},
                {"у","u"},
                {"ф","f"},
                {"х","h"},
                {"ц","ts"},
                {"ч","ch"},
                {"ш","sh"},
                {"щ","sch"},
                {"ъ","'"},
                {"ы","yi"},
                {"ь",""},
                {"э","e"},
                {"ю","yu"},
                {"я","ya"}

            };
        public string TranslitFileName(string source)
        {
            var result = "";
            try
            {
                foreach (var ch in source)
                {
                    var ss = "";
                    if (dictionaryChar.TryGetValue(ch.ToString(), out ss))
                    {
                        result += ss;
                    }
                    else result += ch;
                }
            }
            catch (Exception e)
            {
                result = source;
            }
            
            return result.TrimEnd().TrimStart();
        }

        public string Password()
        {
            var symbols = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789~!@#$%^&*()_+=,./{|}";
            Random rand = new Random();
            var randomText = "";
            for (int i = 0; i < 10; i++)
            {
                randomText += symbols[rand.Next(0, symbols.Length)];
            }
            return randomText;
        }
    }
}
