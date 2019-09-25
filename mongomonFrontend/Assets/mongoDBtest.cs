using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Http;

public class mongoDBtest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var values = new Dictionary<string, string>
{
{ "thing1", "hello" },
{ "thing2", "world" }
};

        var content = new FormUrlEncodedContent(values);
        HttpClient client = new HttpClient();
        var response = client.PostAsync("http://localhost:8080/game/", content);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
