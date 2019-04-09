using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WasteSpawner : MonoBehaviour
{
    private static WasteSpawner instance_;
    public static WasteSpawner Instance { get { return instance_; } }

    [SerializeField] private GameObject[] wasteItems;
    [SerializeField] private Vector3 spawnPosition;

    private List<WasteItem> registeredItems;

    private bool shouldSpawnItem = false;

    private void Awake()
    {
        if(instance_ == null) {
            instance_ = this;
            registeredItems = new List<WasteItem>();
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
        StartSpawningItems();
    }

    private void GameManager_GameOver(object sender, System.EventArgs e)
    {
        StopSpawningItems();
        while(registeredItems.Count > 0)
        {
            WasteItem item = registeredItems[0];
            DeRegisterItem(item);
            Destroy(item.gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(spawnPosition, 0.5f);
    }

    private void OnDestroy()
    {
        if(instance_ == this) { instance_ = null; }
    }

    public void SpawnItem()
    {
        Instantiate(wasteItems[UnityEngine.Random.Range(0, wasteItems.Length)], spawnPosition, Quaternion.identity);
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
}
