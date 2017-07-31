using System;
using System.Collections.Generic;
using System.Linq;

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
                {"г","h"},
                {"ґ","g"},
                {"д","d"},
                {"е","e"},
                {"є","ie"},
                {"ё","yo"},
                {"ж","zh"},
                {"з","z"},
                {"и","y"},
                {"і","i"},
                {"ї","yi"},
                {"й","i"},
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
                {"х","kh"},
                {"ц","ts"},
                {"ч","ch"},
                {"ш","sh"},
                {"щ","shch"},
                {"ъ","'"},
                {"ы","yi"},
                {"ь",""},
                {"э","e"},
                {"ю","iu"},
                {"я","ia"}
            };
        public string TranslitFileName(string source)
        {
            var result = "";
            string[] opt = { "г", "є", "ї", "й", "ю", "я" };
            try
            {
                foreach (var ch in source)
                {
                    var ss = "";

                    if (ch.ToString() == opt[0])
                    {
                        if (source.IndexOf(ch) == 0)
                        {
                            result += "h";
                        }
                        else
                        {
                            if (source[source.IndexOf(ch) - 1].ToString() == "з")
                            {
                                result += "gh";
                            }
                            else
                            {
                                result += "h";
                            }
                        }
                    }
                    else if (ch.ToString() == opt[1] && source.IndexOf(ch) == 0)
                    {
                        result += "ye";
                    }
                    else if (ch.ToString() == opt[2] && source.IndexOf(ch) == 0)
                    {
                        result += "yi";
                    }
                    else if (ch.ToString() == opt[3] && source.IndexOf(ch) == 0)
                    {
                        result += "y";
                    }
                    else if (ch.ToString() == opt[4] && source.IndexOf(ch) == 0)
                    {
                        result += "yu";
                    }
                    else if (ch.ToString() == opt[5] && source.IndexOf(ch) == 0)
                    {
                        result += "ya";
                    }
                    else
                    {
                        if (dictionaryChar.TryGetValue(ch.ToString(), out ss))
                        {
                            result += ss;
                        }
                        else result += ch;
                    }

                }
            }
            catch (Exception)
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
