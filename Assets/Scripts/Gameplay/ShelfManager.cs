using System.Collections.Generic;
using UnityEngine;

public class ShelfManager : MonoBehaviour
{
    [SerializeField] GameObject[] items = new GameObject[8];
    [SerializeField] Transform[] transforms = new Transform[8];
    List<int> emptySlots = new List<int>();

    //Make it so that it doesn't check full slots
    public void Restock(GameObject newItem)
    {
        int check;

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                emptySlots.Add(i);
            }
        }

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
