using UnityEngine;

public class ShelvingUnit : MonoBehaviour
{
    [Header("Properties")]
    [field: SerializeField] public ItemIndex WeaponIndex { get; private set; }

    [Header("Spawn Points")]
    [field: SerializeField] public Transform[] SpawnPoints { get; private set; }

    [Header("Components")]
    private GameManager gameManager;

    #region Initialization Method

    private void Start()
    {
        // Subscribe Refreshes to wave changes
        gameManager = GameManager.Instance;
        gameManager.OnWaveFinish += RefreshShelf;

        // Refresh before the first round starts
        RefreshShelf();
    }

    #endregion

    #region Unity Callbacks

    private void OnDestroy()
    {
        gameManager.OnWaveFinish -= RefreshShelf;
    }

    #endregion

    private void RefreshShelf()
    {
        foreach (var i in SpawnPoints)
        {
            Debug.Log("Refresh Shelf");

            //Destroy children
            if (i.childCount > 0)
                Destroy(i.GetChild(0));

            // Spawn new item if nothing is present
            float replaceItem = Random.Range(0f, 1f);
            if(replaceItem > 0.75f)// 25% chance to spawn an item
            {
                int random = Random.Range(0, WeaponIndex.WeaponIndex.Length);
                MeleeWeapon weaponObject = Instantiate(WeaponIndex.WeaponIndex[random], i.position, i.rotation, i);
            }
        }
    }
}
