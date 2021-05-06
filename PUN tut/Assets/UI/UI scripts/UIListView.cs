﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIListView : MonoBehaviour
{
    [SerializeField]
    private GameObject FriendItemPrefab;

    private GameObject content;

    // Start is called before the first frame update
    void Start()
    {
        content = this.gameObject.transform.Find("Viewport").Find("Content").gameObject;
        LoadFriendList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadFriendList()
    {
        // 服务器调用数据

        // 显示

        List<string> textlist = new List<string>() { "chh", "hyy", "lyy", "lzh", "rz", "ct", "ynj", "cw" };
        for(int i=0;i<textlist.Count;++i)
        {
            GameObject go = Instantiate(FriendItemPrefab) as GameObject;
            go.transform.Find("Text").gameObject.GetComponent<Text>().text = textlist[i];
            go.transform.parent = content.transform;
            go.transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
