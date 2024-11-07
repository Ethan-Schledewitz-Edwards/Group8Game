using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Aisle<T>
{
    public List<T> shelves;
}

public class AisleManager : MonoBehaviour
{
    [SerializeField] List<Aisle<GameObject>> aisles;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
