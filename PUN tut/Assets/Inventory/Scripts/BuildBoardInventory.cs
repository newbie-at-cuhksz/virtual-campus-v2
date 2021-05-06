// the script is kind of similar to the UIListViews
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildBoardInventory:MonoBehaviour
{

    [SerializeField]
    private GameObject BoardPrefab;

    private GameObject BoardContent; // to find the content in the Canvas
    
    
    private void Start()
    {
        BoardContent = this.gameObject.transform.Find("BoardViewport").Find("Content").gameObject;
        Debug.Log("the BoardContent finds the port");
        loadBoardInventory();
    }

    
    public void loadBoardInventory()
    {
        // fetch the data from the data base here
        BoardInventory boardInventory = new BoardInventory();
        // the following additems are for test
        boardInventory.AddItem(new Item { itemType = Item.ItemType.Board, amount = 2, ItemIndex = 1 });
        boardInventory.AddItem(new Item { itemType = Item.ItemType.Board, amount = 2, ItemIndex = 2 });

        // below is the loadBoardInventory
        //int boardNum = boardInventory.GetBoardNumber();
        List<Item> boardList = boardInventory.GetItemList();
        for (int i = 0; i < boardList.Count; i++)
        {
            for (int k = 0; k < boardList[i].amount; k++) {
                Debug.Log("the boardList is invoked, please check");
                GameObject boardTemp = Instantiate(BoardPrefab) as GameObject;
                boardTemp.SetActive(true);
                int index = boardList[i].ItemIndex;
                boardTemp.transform.Find("Text").gameObject.GetComponent<Text>().text = "Board" + index.ToString();
                boardTemp.transform.parent = BoardContent.transform;
                boardTemp.transform.localScale = new Vector3(1, 1, 1);
            }
        }

    }
}