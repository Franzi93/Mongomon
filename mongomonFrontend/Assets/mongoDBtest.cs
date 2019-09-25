using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Http;

public class mongoDBtest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetResponse();
    }

    static async void GetResponse()
    {
        var values = new Dictionary<string, string>
        {
        { "thing1", "hello" },
        { "thing2", "world" }
        };
        MongomonLib.User u = new MongomonLib.User();
        u.name = "gsdfsd";
        var content = new FormUrlEncodedContent(values);
        HttpClient client = new HttpClient();
        var x = await client.PostAsync("http://localhost:8080/game/", content);
        Debug.Log(x.ToString());
    }
   
}
