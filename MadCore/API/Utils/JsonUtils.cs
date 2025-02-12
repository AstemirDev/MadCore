using System;
using System.Collections.Generic;
using System.IO;
using Spine;

namespace MadCore.API.Utils
{
    public static class JsonUtils
    {
     
        public static float[] GetFloatArray(Dictionary<string, object> map, string name)
        {
            var objectList = (List<object>) map[name];
            var floatArray = new float[objectList.Count];
            var index = 0;
            for (var count = objectList.Count; index < count; ++index)
                floatArray[index] = (float) objectList[index];
            return floatArray;
        }
        
        public static int[] GetIntArray(Dictionary<string, object> map, string name)
        {
            var objectList = (List<object>) map[name];
            var intArray = new int[objectList.Count];
            var index = 0;
            for (var count = objectList.Count; index < count; ++index)
                intArray[index] = (int) (float) objectList[index];
            return intArray;
        }
        
        public static float GetFloat(Dictionary<string, object> map, string name, float defaultValue)
        {
            return !map.ContainsKey(name) ? defaultValue : (float) map[name];
        }

        public static int GetInt(Dictionary<string, object> map, string name, int defaultValue)
        {
            return !map.ContainsKey(name) ? defaultValue : (int) (float) map[name];
        }

        public static bool GetBoolean(Dictionary<string, object> map, string name, bool defaultValue)
        {
            return !map.ContainsKey(name) ? defaultValue : (bool) map[name];
        }
        
        public static string GetString(
            Dictionary<string, object> map,
            string name,
            string defaultValue)
        {
            return !map.ContainsKey(name) ? defaultValue : (string) map[name];
        }
        
        public static Dictionary<string, object> ReadJson(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                var jsonMap = Json.Deserialize(reader) as Dictionary<string, object>;
                if (jsonMap != null)
                {
                    return jsonMap;
                }
                throw new Exception("Invalid json file content: "+reader.ReadToEnd());
            }
        }
    }
}