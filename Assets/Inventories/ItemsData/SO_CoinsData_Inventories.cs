using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

[CreateAssetMenu(fileName = "CoinsData", menuName = "Items/ItemData/CoinsData")]
public class SO_CoinsData_Inventories : SO_ItemData_Inventories
{
    
    // Reference to The different Sprites for the coins icons
    public Sprite smallPileSprite;
    public Sprite bagSprite;
    public Sprite chestSprite;

    public override Sprite GetSpriteBasedOnQuantity(int quantity)
    {
        if (quantity > 100)
        {
            return chestSprite;
        }
        else if (quantity > 20)
        {
            return bagSprite;
        }
        else
        {
            return smallPileSprite;
        }
    }
}
