using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardInventory 
{
    private List<Item> itemList;
    private int countBoard;
    private int countClothes; // count the total number of the board and the clothe

    public BoardInventory() // constructor
    {
        Debug.Log("The inventory is constructed");
        itemList = new List<Item>();
        countBoard = 0;
        countClothes = 0;
        //AddItem(new Item { itemType = Item.ItemType.Board, amount = 1, ItemIndex = 1});
        //AddItem(new Item { itemType = Item.ItemType.Clothes, amount = 1, ItemIndex = 2 });
        
    }

    public void AddItem(Item item)
    {
        itemList.Add(item);
        if (item.itemType == Item.ItemType.Board) countBoard += item.amount;
        if (item.itemType == Item.ItemType.Clothes) countClothes += item.amount;
    }

    public List<Item> GetItemList() {
        return itemList;
    }


    public int GetBoardNumber() {
        return countBoard;
    }

    public int GetClothesNumber() {
        return countClothes;
    }

}