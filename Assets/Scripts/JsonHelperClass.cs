using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonHelperClass {

    public static T[] getJsonArray<T>(string json, bool gotArray = false)
    {
        string newJson = "{ \"array\": " + json + "}";
        if (gotArray)
        {
            newJson = json;
        }
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.array;
    }

    public static string ToJsonArray<T>(T[] array)
    {
        Wrapper<T> arrayWraper = new Wrapper<T>();
        arrayWraper.array = array;
        string s = JsonUtility.ToJson(arrayWraper, true);
        return s;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] array;
    }
}
