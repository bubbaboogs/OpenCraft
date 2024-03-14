using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemStack
{
    public blockType id;
    public int amount;

    public ItemStack(blockType _id, int _amount)
    {
        id = _id;
        amount = _amount;
    }
}
