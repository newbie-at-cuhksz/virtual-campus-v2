using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using System.IO;
using UnityEngine.UI;

public class WebServerClient : MonoBehaviour
{
    private static readonly string baseURL = "http://119.23.211.236:10086/";
    private string sessionID;
    private string UID;
    private string vCode;
    public bool loginStatus = false;
    private int RetryTime;
    private const int MaxRetryTime = 3;
    private string modelSavePath;

    public GameObject sc;
    private WebSocketClient socketClient;
    public bool routine_stop = false;
    public GameObject[] own_card_list = new GameObject[2];
    public Text location_path;
    public Text population_map;
    public string nickname;
    public int token;

    private Dictionary<string, int> buildingHour = new Dictionary<string, int>();
    private Dictionary<string, int> buildingMinute = new Dictionary<string, int>();
    public static Dictionary<string, float> buildingToken = new Dictionary<string, float>();




    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        UnityWebRequest.ClearCookieCache();
        socketClient = sc.GetComponent<WebSocketClient>();
        init_building();
        init_token();
    }

    private void init_token()
    {
        buildingToken["Administration Building"] = 3;
        buildingToken["University Library"] = 3;
        buildingToken["Student Center"] = 3;
        buildingToken["TA"] = 3;
        buildingToken["Million Avenue"] = 3;
        buildingToken["TB"] = 3;
        buildingToken["TC"] = 3;
        buildingToken["TD"] = 3;
        buildingToken["RA"] = 3;
        buildingToken["RB"] = 3;
        buildingToken["Shaw College East"] = 3;
        buildingToken["Shaw College West"] = 3;
        buildingToken["Zhixin Building"] = 3;
        buildingToken["GYM"] = 3;
        buildingToken["Harmonia College"] = 3;
        buildingToken["Dligentia College"] = 3;
        buildingToken["Muse College"] = 3;
        buildingToken["Staff Quarters"] = 3;
        buildingToken["Chengdao Building"] = 3;
        buildingToken["Zhiren Building"] = 3;
        buildingToken["Letian Building"] = 3;
        buildingToken["Shaw International Conference Centre"] = 3;
        buildingToken["Start-up Zone"] = 3;
        buildingToken["Daoyuan Building"] = 3;
        buildingToken["not found"] = 3;
    }
    private void init_building()
    {
        buildingHour["Administration Building"] = 0;
        buildingHour["University Library"] = 0;
        buildingHour["Student Center"] = 0;
        buildingHour["TA"] = 0;
        buildingHour["Million Avenue"] = 0;
        buildingHour["TB"] = 0;
        buildingHour["TC"] = 0;
        buildingHour["TD"] = 0;
        buildingHour["RA"] = 0;
        buildingHour["RB"] = 0;
        buildingHour["Shaw College East"] = 0;
        buildingHour["Shaw College West"] = 0;
        buildingHour["Zhixin Building"] = 0;
        buildingHour["GYM"] = 0;
        buildingHour["Harmonia College"] = 0;
        buildingHour["Dligentia College"] = 0;
        buildingHour["Muse College"] = 0;
        buildingHour["Staff Quarters"] = 0;
        buildingHour["Chengdao Building"] = 0;
        buildingHour["Zhiren Building"] = 0;
        buildingHour["Letian Building"] = 0;
        buildingHour["Shaw International Conference Centre"] = 0;
        buildingHour["Start-up Zone"] = 0;
        buildingHour["Daoyuan Building"] = 0;
        buildingHour["not found"] = 0;

        buildingMinute["Administration Building"] = 0;
        buildingMinute["University Library"] = 0;
        buildingMinute["Student Center"] = 0;
        buildingMinute["TA"] = 0;
        buildingMinute["Million Avenue"] = 0;
        buildingMinute["TB"] = 0;
        buildingMinute["TC"] = 0;
        buildingMinute["TD"] = 0;
        buildingMinute["RA"] = 0;
        buildingMinute["RB"] = 0;
        buildingMinute["Shaw College East"] = 0;
        buildingMinute["Shaw College West"] = 0;
        buildingMinute["Zhixin Building"] = 0;
        buildingMinute["GYM"] = 0;
        buildingMinute["Harmonia College"] = 0;
        buildingMinute["Dligentia College"] = 0;
        buildingMinute["Muse College"] = 0;
        buildingMinute["Staff Quarters"] = 0;
        buildingMinute["Chengdao Building"] = 0;
        buildingMinute["Zhiren Building"] = 0;
        buildingMinute["Letian Building"] = 0;
        buildingMinute["Shaw International Conference Centre"] = 0;
        buildingMinute["Start-up Zone"] = 0;
        buildingMinute["Daoyuan Building"] = 0;
        buildingMinute["not found"] = 0;
    }
    public WebServerClient()
    {
        RetryTime = 0;
        UID = "";
    }

    public void login(string email, string password)
    {
        UnityWebRequest.ClearCookieCache();
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
        //  request.SetRequestHeader("Cookie", string.Format("sessionid={0};", sessionID));
        request.timeout = 5;
        yield return request.SendWebRequest();
        if (request.isNetworkError)
        {
            Debug.LogErrorFormat("加载出错： {0}", request.error);
            if (RetryTime < MaxRetryTime)
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
            routine_stop = true;
            yield break;
        }

        if (request.isDone)
        {
            RetryTime = 0;
            Debug.Log("Connection Finished!");
            string status = request.downloadHandler.text;
            //  Debug.Log(status);
            if (request.responseCode == 202)
            {
                Debug.Log("Wrong Session, please login again");
            }
            if (type == "login")
            {
                if (request.responseCode == 200)
                {
                    loginStatus = true;
                    Debug.Log(loginStatus);
                    string cookies = request.GetResponseHeader("Set-Cookie");
                    sessionID = Regex.Match(cookies, @"(?<=sessionid=)[^;]*(?=;)?").Value;
                    string[] playerStatus = request.downloadHandler.text.Split(',');
                    nickname = playerStatus[0];
                    token = int.Parse(playerStatus[1]);
                    socketClient.Connect(UID);
                    Debug.Log(cookies);
                    Debug.Log(sessionID);
                    GameObject.Find("UIScript").SendMessage("login_ui_operation_1");
                }
                else
                {
                    Debug.Log(status);
                    GameObject.Find("UIScript").SendMessage("login_ui_operation_2");
                }
                routine_stop = true;
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
            else if (type == "getDateRoute")
            {
                if (request.responseCode == 200)
                {
                    string locations = request.downloadHandler.text;
                    string[] locList = locations.Split(';');

                    foreach (string element in locList)
                    {
                        string[] sub_info = element.Split(',');
                        int sub_time_hour_1 = int.Parse(sub_info[1].Split(' ')[1].Split(':')[0]);
                        int sub_time_hour_2 = int.Parse(sub_info[2].Split(' ')[1].Split(':')[0]);
                        int sub_time_minute_1 = int.Parse(sub_info[1].Split(' ')[1].Split(':')[1]);
                        int sub_time_minute_2 = int.Parse(sub_info[2].Split(' ')[1].Split(':')[1]);
                        // Debug.Log(string.Format("subtime:{0} {1} {2} {3}",sub_time_hour_1, sub_time_hour_2, sub_time_minute_1, sub_time_minute_2));
                        int sub_hour = sub_time_hour_2 - sub_time_hour_1;
                        int sub_minute = sub_time_minute_2 - sub_time_minute_1;
                        if (sub_minute < 0)
                        {
                            sub_minute = 60 - sub_time_minute_1 + sub_time_minute_2;
                            sub_hour = sub_hour - 1;
                        }
                        buildingHour[sub_info[0]] += sub_hour;
                        buildingMinute[sub_info[0]] += sub_minute;
                        if (buildingMinute[sub_info[0]] > 60)
                        {
                            buildingHour[sub_info[0]] += 1;
                            buildingMinute[sub_info[0]] -= 60;
                        }


                    }
                    // Debug.Log(locList);
                    foreach (string location_name in buildingHour.Keys)
                    {
                        if (buildingHour[location_name] != 0 || buildingMinute[location_name] != 0)
                        {
                            location_path.text = string.Format("{0} {1:D2}.{2:D2}h \n", location_name, buildingHour[location_name], buildingHour[location_name]);
                        }
                    }
                    // location_path.text = string.Join("\n", locList);
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
                    //Debug.Log("Achievement here !");
                    string response = request.downloadHandler.text;
                    string[] achievementList = response.Split(';');
                    foreach (string element in achievementList)
                    {
                        int card_number = (int)(float.Parse(element.Substring(0, element.LastIndexOf('*'))));
                        // Debug.Log("card_number" + card_number);
                        own_card_list[card_number].SetActive(true);
                        // GameObject.Find("connect_backgroud").GetComponent<LocationAccess2>().own_card_list[card_number].SetActive(true);
                    }
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
                    string population_get = request.downloadHandler.text;
                    string[] population = population_get.Split(',');
                    int total_people = 0;
                    foreach (string sub_str in population)
                    {
                        Regex regex = new Regex("\"[^\"]*\"");
                        string location = regex.Match(sub_str).Value.Replace("\"", "");
                        string sub_people = sub_str.Split(':')[1];
                        sub_people = Regex.Replace(sub_people, "}", "");
                        int location_people = int.Parse(sub_people);
                        // Debug.Log(location_people);
                        if (location_people > 0)
                        {
                            Debug.Log("total_people"+total_people+location);
                            total_people = total_people + location_people;
                            buildingToken[location] = location_people;
                        }
                        else
                        {
                            buildingToken[location] = 0;
                        }
                        //population_map.text = population;
                    }

                    List<string> list = new List<string>();
                    list.AddRange(buildingToken.Keys);
                    string token_text = "";
                    
                    foreach (string t in list)
                    {
                        if (total_people == 0)
                        {
                            init_token();
                            break;
                        }
                        else
                        {
                            
                            buildingToken[t] = 1 + (total_people  - buildingToken[t] )/ total_people *5;
                            //token_text = token_text + t + buildingToken[t].ToString() + "\n";
                        }
                    }
                    DictionarySort(buildingToken);
                    foreach (string s in buildingToken.Keys)
                    {
                        token_text = token_text + s +  " " + buildingToken[s].ToString() + "\n";
                    }
                    population_map.text = token_text;
                    Debug.Log("population_map");
                    Debug.Log(population_get);
                   // string token_text = token_text + location_name + buildingToken[location_name].ToString() + "\n";
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

    public static float return_token(string location)
    {
        return buildingToken[location];
    }

    private void DictionarySort(Dictionary<string, float> dic)
    {
        if (dic.Count > 0)
        {
            List<KeyValuePair<string, float>> lst = new List<KeyValuePair<string, float>>(dic);
            lst.Sort(delegate (KeyValuePair<string, float> s1, KeyValuePair<string, float> s2)
            {
                return s2.Value.CompareTo(s1.Value);
            });
            dic.Clear();

            foreach (KeyValuePair<string, float> kvp in lst)
                dic[kvp.Key] = kvp.Value;
            //   Response.Write(kvp.Key + “：” +kvp.Value + “”);
        }
    }

    public void SetNickName(string nickname)
    {
        string url = baseURL + "nickname/set/";
        WWWForm form = new WWWForm();
        form.AddField("UID", UID);
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

    public string registry(string email, string password, string veriationCode)
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
            return "Registered successfully!";
        }
        else if (veriationCode != vCode)
        {
            return "Veriation code invalid!";
        }
        else if (UID != email)
        {
            return "Email invalid!";
        }
        else if (!passwordValid.IsMatch(password))
        {
            return "Password invalid!";
        }
        return "Registered fail!";

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
        form.AddField("model_name", fileName);
        StartCoroutine(Post(form, url, "downloadUniqueModel"));
    }

    public void GetDateRoute(/*string date*/)
    {
        string date = string.Format("{0:D4}-{1:D2}-{2:D2}", System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day);

        // string date = "2021-06-15";
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
                if (result[0] == "None")
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
        uid = uid.Replace('@', '-');
        socketClient.send("add_friend:" + uid);
    }
}
