using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Aisle<T>
{
    public List<ShelfManager> shelves;
}

public class AisleManager : MonoBehaviour
{
    [SerializeField] List<Aisle<GameObject>> aisles;
    int randAisle;
    int randShelf;

    public void CallForRestock(GameObject newItem)
    {
        if (aisles.Count > 0)
        {
            randAisle = Random.Range(0, aisles.Count);

            if (aisles[randAisle].shelves.Count > 0)
            {
                randShelf = Random.Range(0, aisles[randAisle].shelves.Count);

                aisles[randAisle].shelves[randShelf].Restock(newItem);
            }
        }
    }
}
