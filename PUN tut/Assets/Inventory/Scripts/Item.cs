using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public enum ItemType // may add some tools if needed 
    {
        Board,
        Clothes
    }

    public int ItemIndex; // itemIndex denotes the sub-type of a item type
                                          // eg: boad1, board2 types under the Board type, the index starts from 1
                                          
    public ItemType itemType;
    public int amount;
}
