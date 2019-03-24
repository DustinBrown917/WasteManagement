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
        SpawnItem();
    }

    // Update is called once per frame
    void Update()
    {
        
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

        if(registeredItems.Count == 0) { SpawnItem(); }
    }
}
