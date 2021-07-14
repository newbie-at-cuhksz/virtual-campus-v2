using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System;
using UnityEngine;

public class WebSocketClient : MonoBehaviour
{
    // Start is called before the first frame update
    ClientWebSocket ws;
    CancellationToken ct;
    string url;
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        
    }

    public WebSocketClient()
    {
        
        url = "ws://119.23.211.236:10086/ws/player/";
    }

    public async void Connect(string uid)
    {
        ws = new ClientWebSocket();
        ct = new CancellationToken();
        try
        {
            Uri myurl = new Uri(url + uid.Replace('@','-') + "/");
            await ws.ConnectAsync(myurl, ct);
            while (true)
            {
                var result = new byte[1024];
                await ws.ReceiveAsync(new ArraySegment<byte>(result), new CancellationToken());//接受数据
                var str = Encoding.UTF8.GetString(result, 0, result.Length);
                Debug.Log(str);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public async void send(string message)
    {
        await ws.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(message)), WebSocketMessageType.Text, true, ct);
    }

    public async void stop()
    {
        await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "test", ct);
    }
}
