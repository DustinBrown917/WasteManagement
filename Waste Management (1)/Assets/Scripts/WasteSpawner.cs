using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WasteSpawner : MonoBehaviour
{
    private static WasteSpawner instance_;
    public static WasteSpawner Instance { get { return instance_; } }

    [SerializeField] private GameObject[] wasteItems;
    private List<GameObject> pooledWasteItems;
    [SerializeField] private Vector3 spawnPosition;
    [SerializeField] private int poolSize;
    public Vector3 PoolPosition { get { return poolPosition_; } }
    [SerializeField] private Vector3 poolPosition_;

    private List<WasteItem> registeredItems;

    private bool shouldSpawnItem = false;

    private void Awake()
    {
        if(instance_ == null) {
            instance_ = this;
            registeredItems = new List<WasteItem>();
            pooledWasteItems = new List<GameObject>();
            
        }
        else {
            Destroy(this.gameObject);
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.StartGame += GameManager_StartGame;
        GameManager.Instance.GameOver += GameManager_GameOver;       
    }

    private void GameManager_StartGame(object sender, System.EventArgs e)
    {
        ClearPool();
        FillPool();
        StartSpawningItems();
    }

    private void GameManager_GameOver(object sender, System.EventArgs e)
    {
        StopSpawningItems();
        while(registeredItems.Count > 0)
        {
            WasteItem item = registeredItems[0];
            DeRegisterItem(item);
            AddItemToPool(item.gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(spawnPosition, 0.5f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(poolPosition_, 0.5f);
    }

    private void OnDestroy()
    {
        if(instance_ == this) { instance_ = null; }
    }

    public void SpawnItem()
    {
        GameObject g = GetItemFromPool();        
        g.transform.position = spawnPosition;
        g.SetActive(true);
    }

    public void RegisterItem(WasteItem item)
    {
        if (registeredItems.Contains(item)) { return; }
        registeredItems.Add(item);
    }

    public void DeRegisterItem(WasteItem item)
    {
        
        if (!registeredItems.Contains(item)) { return; }
        registeredItems.Remove(item);
        if(registeredItems.Count == 0 && shouldSpawnItem) { SpawnItem(); }
    }

    public void StartSpawningItems()
    {
        shouldSpawnItem = true;
        SpawnItem();
    }

    public void StopSpawningItems()
    {
        shouldSpawnItem = false;
    }

    private void FillPool()
    {
        for(int i = 0; i < poolSize; i++) {
            GameObject item = Instantiate(wasteItems[UnityEngine.Random.Range(0, wasteItems.Length)], poolPosition_, Quaternion.identity);
            pooledWasteItems.Add(item);
            item.SetActive(false);
        }
    }

    public void ClearPool()
    {
        foreach(GameObject go in pooledWasteItems)
        {
            Destroy(go);
        }

        pooledWasteItems.Clear();
    }

    private GameObject GetItemFromPool()
    {
        int index = UnityEngine.Random.Range(0, pooledWasteItems.Count);
        GameObject g = pooledWasteItems[index];
        pooledWasteItems.RemoveAt(index);
        return g;
    }

    public void AddItemToPool(GameObject g)
    {
        WasteItem wi = g.GetComponent<WasteItem>();
        if(wi != null)
        {
            wi.ResetScaleAndRotation();
        }
        pooledWasteItems.Add(g);
        g.transform.position = poolPosition_;
        g.SetActive(false);
    }
}
