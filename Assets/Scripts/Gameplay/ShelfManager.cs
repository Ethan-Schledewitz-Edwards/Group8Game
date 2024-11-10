using System.Collections.Generic;
using UnityEngine;

public class ShelfManager : MonoBehaviour
{
    [SerializeField] GameObject[] items = new GameObject[8];
    [SerializeField] Transform[] transforms = new Transform[8];
    List<int> emptySlots = new List<int>();

    // Restocks an item onto a random shelf slot
    public void Restock(GameObject newItem)
    {
        int check;

        // Checks which slots on the shelf are empty
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                emptySlots.Add(i);
            }
        }

        // If there is at least one empty shelf slot, pick one at random and instantiate an item there
        if (emptySlots.Count > 0)
        {
            check = Random.Range(0, emptySlots.Count);
            items[emptySlots[check]] = Instantiate(newItem, transforms[emptySlots[check]].position, Quaternion.identity);
            emptySlots.Clear();
        }
    }

    public GameObject GetItem(int num)
    {
        return items[num];
    }

    public void RemoveItem(int num)
    {
        Destroy(items[num]);

        items[num] = null;
    }
}
