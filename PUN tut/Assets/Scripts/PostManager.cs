using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostManager : MonoBehaviour
{
    public GameObject clientObject;
    private WebServerClient myClient;
    
    void Start()
    {
        myClient = clientObject.GetComponent<WebServerClient>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnLoginClicked()
    {
        myClient.login("testemail@test.com","123456");
    }

    public void OnSearchClicked()
    {
        myClient.GetMarketList();
        
    }
}
