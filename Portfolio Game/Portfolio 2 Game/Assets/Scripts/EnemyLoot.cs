using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;
using System.Collections.Generic;

public class EnemyLoot : MonoBehaviour
{
    [System.Serializable]
    public class  LootDrop
    {
        public GameObject itemPrefab;
        public float dropChance = 0.5f; // 50% chance by default
        public int minQuantity = 1;
        public int maxQuantity = 1;
    }

    public List<LootDrop> lootTable = new List<LootDrop>();

    public void DropLoot()
    {
        foreach (LootDrop drop in lootTable)
        {
            if(Random.value <= drop.dropChance)
            {
                int quantity = Random.Range(drop.minQuantity, drop.maxQuantity + 1); // +1 because max is exclusive
                for( int i = 0; i < quantity; i++)
                {
                    if(drop.itemPrefab != null)
                        Instantiate(drop.itemPrefab, transform.position, Quaternion.identity);
                    else
                        Debug.LogWarning("Item prefab is null " + gameObject.name);
                }
            }
        }
    }
    public void Die()
    {
        DropLoot();
        Destroy(gameObject);
    }
}
