using System.Collections.Generic;
using UnityEngine;

public class ShelfManager : MonoBehaviour
{
    [SerializeField] GameObject[] items = new GameObject[8];
    [SerializeField] Transform[] transforms = new Transform[8];

    //Make it so that it doesn't check full slots
    public void Restock(GameObject newItem)
    {
        int check;

        for (int i = 0; i < items.Length; i++)
        {
            check = Random.Range(0, items.Length);

            if (items[check] == null)
            {
                items[check] = newItem;
                Instantiate(newItem, transforms[check].position, Quaternion.identity);
            }
        }
    }

    public void RemoveItem(int num)
    {
        Destroy(items[num].gameObject);

        items[num] = null;
    }
}
