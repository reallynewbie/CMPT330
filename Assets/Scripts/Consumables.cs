using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles loot box related tasks
/// </summary>
/// 
/// Source: https://www.youtube.com/watch?v=eg7i6iLHoIY
/// 
/// Author: Chamod Welhenge
/// 

public class Consumables : MonoBehaviour
{
    /// <summary>
    /// Class for information about a consumable
    /// </summary>
    /// 
    /// Field       Description
    /// name        Name of the item
    /// item        The actual item
    /// dropRarity  Rarity of item drop
    /// 
    /// 
    [System.Serializable]
    public class Consumable
    {

        public string name;
        public GameObject item;
        public int dropRarity;
    }

    public List<Consumable> lootTable = new List<Consumable>();

    void CalculateLoot()
    {
        int itemWeight = 0;

        foreach (Consumable item in lootTable)
        {
            itemWeight += item.dropRarity;
        }

        int randomChance = Random.Range(0, itemWeight);

        foreach (Consumable item in lootTable)
        {
            if (randomChance <= item.dropRarity)
            {
                Instantiate(item.item, transform.position, Quaternion.identity);
                Destroy(this.gameObject);
                return;
            }
            randomChance -= item.dropRarity;
        }


    }


    void OnMouseDown()
    {
        CalculateLoot();

    }
}
