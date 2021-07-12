using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using System.IO;

public class WebServerClient : MonoBehaviour
{
    private static readonly string baseURL = "http://119.23.211.236:10086/";
    private string sessionID;
    private string UID;
    private string vCode;
    private int RetryTime;
    private const int MaxRetryTime = 3;
    private string modelSavePath;

    public GameObject sc;
    private WebSocketClient socketClient;
    public bool loginStatus = false;
    public string nickname;
    public int token;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        UnityWebRequest.ClearCookieCache();
        socketClient = sc.GetComponent<WebSocketClient>();
    }

    public WebServerClient()
    {
        RetryTime = 0;
        UID = "";
        
    }


    public void login(string email, string password)
    {
        loginStatus = false;
        string url = baseURL + "login/";
        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("password", password);
        StartCoroutine(Post(form, url, "login"));
        UID = email;
    }

    private IEnumerator Post(WWWForm form, string url, string type)
    {
        UnityWebRequest request = UnityWebRequest.Post(url, form);
        //request.SetRequestHeader("Cookie", string.Format("{0};", sessionID));
        request.timeout = 5;
        yield return request.SendWebRequest();
        if (request.isNetworkError)
        {
            Debug.LogErrorFormat("加载出错： {0}", request.error);  
            if(RetryTime < MaxRetryTime)
            {
                RetryTime++;
                Debug.Log("Retrying");
                StartCoroutine(Post(form, url, type));
                request.Dispose();
            }
            else
            {
                Debug.Log("Stop connection");
                
                RetryTime = 0;
            }
            yield break;
        }

        if (request.isDone)
        {
            RetryTime = 0;
            Debug.Log("Connection Finished!");
            string status = request.downloadHandler.text;
            if(request.responseCode == 202)
            {
                Debug.Log("Wrong Session, please login again");
            }
            if (type == "login")
            {
                if (request.responseCode == 200)
                {
                    string cookies = request.GetResponseHeader("Set-Cookie");
                    sessionID = Regex.Match(cookies, @"(?<=sessionid=)[^;]*(?=;)?").Value;
                    loginStatus = true;
                    string[] playerStatus = request.downloadHandler.text.Split(',');
                    nickname = playerStatus[0];
                    token = int.Parse(playerStatus[1]);
                    socketClient.Connect(UID);
                    Debug.Log(cookies);
                    Debug.Log(sessionID);
                }
                else
                {
                    Debug.Log(status);
                }
            }
            else if (type == "downloadUniqueModel")
            {
                if (request.responseCode == 200)
                {
                    byte[] results = request.downloadHandler.data;
                    Debug.Log(request.GetResponseHeader("Content-Disposition"));
                    FileInfo fileInfo = new FileInfo("D:/CS_related/UnityProject/AndroidLocation/Assets/Scripts/" + request.GetResponseHeader("Content-Disposition"));
                    FileStream fs = fileInfo.Create();
                    //fs.Write(字节数组, 开始位置, 数据长度);
                    fs.Write(results, 0, results.Length);
                    fs.Flush(); 
                    fs.Close();
                }
                else
                {
                    Debug.Log("Download failure");
                }
            }
            else if(type == "getDateRoute")
            {
                if(request.responseCode == 200)
                {
                    string locations = request.downloadHandler.text;
                    string[] locList = locations.Split(';');
                    Debug.Log(locList);
                }
                else
                {
                    Debug.Log(status);
                }
            }
            else if (type == "setNickname")
            {
                if (request.responseCode == 200)
                {
                    nickname = request.downloadHandler.text;
                    Debug.Log(nickname);
                }
                else
                {
                    Debug.Log(status);
                }
            }
            else if (type == "setToken")
            {
                if (request.responseCode == 200)
                {
                    token = int.Parse(request.downloadHandler.text);
                    Debug.Log(token);
                }
                else
                {
                    Debug.Log(status);
                }
            }
            else if (type == "getAchievements")
            {
                if (request.responseCode == 200)
                {
                    string response = request.downloadHandler.text;
                    string[] achievementList = response.Split(';');
                }
                else
                {
                    Debug.Log(status);
                }
            }
            else if (type == "getMarketList")
            {
                if (request.responseCode == 200)
                {
                    string response = request.downloadHandler.text;
                    string[] markettList = response.Split(';');
                }
                else
                {
                    Debug.Log(status);
                }
            }
            else if (type == "getMarketPlayerList")
            {
                if (request.responseCode == 200)
                {
                    string response = request.downloadHandler.text;
                    string[] markettList = response.Split(';');
                }
                else
                {
                    Debug.Log(status);
                }
            }
            else if (type == "getPopulation")
            {
                if (request.responseCode == 200)
                {
                    string response = request.downloadHandler.text;
                }
                else
                {
                    Debug.Log(status);
                }
            }
            else if (type == "enterPosition")
            {
                if (request.responseCode == 200)
                {
                    string response = request.downloadHandler.text;
                }
                else
                {
                    Debug.Log(status);
                }
            }

            /*else if(type == "target string")
            {
                if(status == "s")
                {
                    Debug.Log("Write action when request is success");
                }
                else
                {
                    Debug.Log("Write action when request is failed");
                }
            }*/


            yield break;
        }
    }

    public void SetNickName(string nickname)
    {
        string url = baseURL + "nickname/set/";
        WWWForm form = new WWWForm();
        form.AddField("UID",UID);
        form.AddField("nickname", nickname);
        StartCoroutine(Post(form, url, "setNickname"));
    }

    public void sendVCode(string email)
    {
        Regex emailValid = new Regex(@"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
        if (emailValid.IsMatch(email) == false)
        {
            Debug.Log("invalid email address");
            return;
        }
        int code = Random.Range(100000, 999999);
        vCode = code.ToString();
        Debug.Log(code);
        string url = baseURL + "sendVC/";
        WWWForm form = new WWWForm();
        form.AddField("email", email);
        UID = email;
        form.AddField("code", vCode);
        StartCoroutine(Post(form, url, "sendVCode"));
    }

    public void registry(string email, string password, string veriationCode)
    {
        Regex passwordValid = new Regex(@"(?=(.*[a-zA-Z]))(?=(.*\d))^.{6,32}$");
        //Must sta
        if (veriationCode == vCode && UID == email && passwordValid.IsMatch(password))
        {
            string url = baseURL + "registry/";
            WWWForm form = new WWWForm();
            form.AddField("email", email);
            form.AddField("password", password);
            //form.AddField("name", name);
            StartCoroutine(Post(form, url, "registry"));
        }

    }

    public void SetPosition(string location, string start, string end)
    {

        string url = baseURL + "position/write/";
        WWWForm form = new WWWForm();
        form.AddField("UID", UID);
        form.AddField("location", location);
        form.AddField("start", start);
        form.AddField("end", end);
        StartCoroutine(Post(form, url, "setPosition"));

    }

    public void UploadUniqueModel(string fileDir, string fileName, string modelName)
    {
        /*string fileDir = "D:/CS_related/UnityProject/AndroidLocation/Assets/Models/chair_test.obj";
        string fileName = "chair_test.obj";
        string modelName = "chair_test.obj";*/
        string url = baseURL + "upload/model/";
        WWWForm form = new WWWForm();
        byte[] objBytes = File.ReadAllBytes(fileDir + fileName + ".obj");
        byte[] mtlBytes = File.ReadAllBytes(fileDir + fileName + ".mtl");
        form.AddField("UID", UID);
        form.AddField("name", modelName);
        form.AddField("id", fileName);
        form.AddBinaryData("obj", objBytes);
        form.AddBinaryData("mtl", mtlBytes);

        StartCoroutine(Post(form, url, "uploadUniqueModel"));
    }

    public void DownloadUniqueModel(string fileName)
    {
        //string fileName = "chair_test.obj";
        string url = baseURL + "download/model/";
        WWWForm form = new WWWForm();
        form.AddField("UID", UID);
        form.AddField("model_id", fileName);
        StartCoroutine(Post(form, url, "downloadUniqueModel"));
    }

    public void GetDateRoute(string date)
    {
        date = "2021-06-15";
        string url = baseURL + "position/get/";
        WWWForm form = new WWWForm();
        form.AddField("UID", UID);
        form.AddField("date", date);
        StartCoroutine(Post(form, url, "getDateRoute"));
    }

    public void EnterPosition(string location)
    {
        string url = baseURL + "position/enter/";
        WWWForm form = new WWWForm();
        form.AddField("UID", UID);
        form.AddField("location", location);
        StartCoroutine(Post(form, url, "enterPosition"));
    }

    public void GetPopulation()
    {
        string url = baseURL + "position/population/";
        WWWForm form = new WWWForm();
        form.AddField("UID", UID);
        StartCoroutine(Post(form, url, "getPopulation"));
    }

    public void SetToken(string add_num)
    {
        //Please enter the change of the token, like 1000, -1000, instead of the token the player possess.
        string url = baseURL + "token/set/";
        WWWForm form = new WWWForm();
        form.AddField("UID", UID);
        form.AddField("token", add_num);
        StartCoroutine(Post(form, url, "setToekn"));
    }

    public void UploadAchievement(string achievement)
    {
        string url = baseURL + "achievement/set/";
        WWWForm form = new WWWForm();
        form.AddField("UID", UID);
        form.AddField("achievement", achievement);
        StartCoroutine(Post(form, url, "uploadAchievement"));
    }

    public void GetAchievements()
    {
        string url = baseURL + "achievement/get/";
        WWWForm form = new WWWForm();
        form.AddField("UID", UID);
        StartCoroutine(Post(form, url, "getAchievements"));
    }

    private IEnumerator LoopRequest()
    {
        while (loginStatus)
        {
            string url = baseURL + "loop_request/";
            WWWForm form = new WWWForm();
            form.AddField("UID", UID);
            UnityWebRequest request = UnityWebRequest.Post(url, form);
            request.SetRequestHeader("Cookie", string.Format("sessionid={0};", sessionID));
            request.timeout = 5;
            yield return request.SendWebRequest();
            if (request.isNetworkError)
            {
                Debug.Log("Loop request connect failed");
                yield return new WaitForSeconds(2);
                continue;
            }
            if (request.isDone)
            {
                string[] result = request.downloadHandler.text.Split(':');
                if(result[0] == "None")
                {
                    yield return new WaitForSeconds(5);
                }
                else
                {
                    yield return new WaitForSeconds(5);
                }
            }
        }
    }

    public void MarketBuy(string item_id)
    {
        string url = baseURL + "market/buy/";
        WWWForm form = new WWWForm();
        form.AddField("UID", UID);
        form.AddField("item_id", item_id);
        StartCoroutine(Post(form, url, "marketBuy"));
    }

    public void MarketUpload(string item_id, string name, int price)
    {
        string url = baseURL + "market/upload/";
        WWWForm form = new WWWForm();
        form.AddField("UID", UID);
        form.AddField("item_id", item_id);
        form.AddField("name", name);
        form.AddField("price", price);
        StartCoroutine(Post(form, url, "marketUpload"));
    }

    public void GetMarketList()
    {
        string url = baseURL + "market/list/";
        WWWForm form = new WWWForm();
        form.AddField("UID", UID);
        StartCoroutine(Post(form, url, "getMarketList"));
    }

    public void GetMarketPlayerList()
    {
        string url = baseURL + "market/playerlist/";
        WWWForm form = new WWWForm();
        form.AddField("UID", UID);
        StartCoroutine(Post(form, url, "getMarketPlayerList"));
    }

    public void MarketChangePrice(string item_id, int price)
    {
        string url = baseURL + "market/change_price/";
        WWWForm form = new WWWForm();
        form.AddField("UID", UID);
        form.AddField("item_id", item_id);
        form.AddField("price", price);
        StartCoroutine(Post(form, url, "marketChangePrice"));
    }

    public void MarketRemove(string item_id)
    {
        string url = baseURL + "market/remove/";
        WWWForm form = new WWWForm();
        form.AddField("UID", UID);
        form.AddField("item_id", item_id);
        StartCoroutine(Post(form, url, "marketRemove"));
    }

    public void add_friend(string uid)
    {
        uid = uid.Replace('@','-');
        socketClient.send("add_friend:" + uid);
    }


}
