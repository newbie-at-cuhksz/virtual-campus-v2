using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Linq;

public class UIManager_Store : MonoBehaviour
{

    public GameObject itemPrefab;
    public Transform marketContent;
    private Dictionary<GameObject, int> itemDic=new Dictionary<GameObject, int>();

    private void OnEnable()
    {
        EventManager.GetInstance.StartListening("UpdateMarketList", UpdateMarketList);
    }
    private void OnDisable()
    {
        EventManager.GetInstance.StopListening("UpdateMarketList", UpdateMarketList);
    }

    void UpdateMarketList(string[] marketList)
    {
        int num = marketList.Length;
        for (int i = 0; i < num; i++)
        {
            string[] tmp = marketList[i].Split(',');
            string owner, price, name;
            owner = tmp[0];
            price = tmp[1];
            name  = tmp[2];
            GameObject item=GameObject.Instantiate(itemPrefab, marketContent);
            itemDic.Add(item,int.Parse(price));
            item.GetComponent<RectTransform>().offsetMin = new Vector2(0f,0f);
            item.GetComponent<RectTransform>().offsetMax = new Vector2(0f, 80f);
            item.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -540 - 100 *i);
            item.transform.Find("Content").Find("Owner").GetComponent<Text>().text = owner;
            item.transform.Find("Content").Find("Description").GetComponent<Text>().text = name;
            item.transform.Find("Content").Find("Price").GetComponent<Text>().text = price;
        }
    }

    public void SortByPrice(int value)
    {
        switch (value)
        {
            case 0:
                var dicSorted = from objDic in itemDic orderby objDic.Value descending select objDic;
                int i = 0;

                foreach (KeyValuePair<GameObject, int> item in dicSorted)
                {
                    item.Key.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -540 - 100 * i);
                    i++;
                }
                break;
            case 1:
                var dicSortedAs = from objDic in itemDic orderby objDic.Value ascending select objDic;
                int n = 0;

                foreach (KeyValuePair<GameObject, int> item in dicSortedAs)
                {
                    item.Key.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -540 - 100 * n);
                    n++;
                }
                break;
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
