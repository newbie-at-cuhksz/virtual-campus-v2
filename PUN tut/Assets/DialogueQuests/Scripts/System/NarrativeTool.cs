using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

namespace DialogueQuests
{

    public class NarrativeTool 
    {
        public static string Translate(string txt)
        {
            //Add any translation system code here

            //txt is the original english text, and it should return the translated value
            //This function is not connected to any translation system by default, but you can connect it to your own system by adding code here
            //This is called for dialogue messages, dialogue choices, actor names, and for quests text

            return txt; //If translation not found, return original text
        }

        //Replace all codes like [i:custom_id] in the string
        //[i:custom_id] is for CustomInt
        //[f:custom_id] is for CustomFloat
        //[s:custom_id] is for CustomString
        public static string ReplaceCodes(string txt)
        {
            string regex_str = @"\[\w:\w+\]";
            if (Regex.IsMatch(txt, regex_str, RegexOptions.None))
            {
                Regex regex = new Regex(regex_str);
                MatchCollection matches = regex.Matches(txt);
                foreach (Match match in matches)
                {
                    string code = match.Value;
                    string value = GetCodeValue(code);
                    txt = txt.Replace(match.Value, value);
                }
            }
            return txt;
        }

        //Get the value of a single code ex: [i:variable_id]
        public static string GetCodeValue(string code)
        {
            string output = "";

            if (code.Length >= 3 && code.Contains(":"))
            {
                string[] vals = code.Substring(1, code.Length - 2).Split(':');
                string type = vals[0];
                string id = vals[1];

                if (type.ToLower() == "i")
                    output = NarrativeData.Get().GetCustomInt(id).ToString();
                if (type.ToLower() == "f")
                    output = NarrativeData.Get().GetCustomFloat(id).ToString();
                if (type.ToLower() == "s")
                    output = NarrativeData.Get().GetCustomString(id);
            }

            return output;
        }
    }

}
