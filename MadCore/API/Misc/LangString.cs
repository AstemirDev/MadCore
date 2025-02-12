using System;
using System.Collections.Generic;
using System.Linq;

namespace MadCore.API.Misc
{
    public enum LangKey
    {
        Japanese,
        English,
        Chinese
    }

    public class LangString : Dictionary<LangKey, string>
    {
        public LangString With(LangKey lang, string value) {
            Add(lang, value);
            return this;
        }

        public static LangString Text(String text)
        {
            return new LangString()
                .With(LangKey.Japanese, text)
                .With(LangKey.English, text);
        }
        
        public string[] ToArray()
        {
            if (Count <= 0) return new string[] {};
            var array = new string[3];
            for (var i = 0; i < 3; i++)
            {
                var text = "";
                if (i < Count - 1)
                {
                    text = this.ElementAt(i).Value;
                }

                array[i] = text;
            }
            return array;
        }
    }
}