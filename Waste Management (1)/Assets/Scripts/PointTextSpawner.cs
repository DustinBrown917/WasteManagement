using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointTextSpawner : MonoBehaviour
{
    private static PointTextSpawner instance_ = null;
    public static PointTextSpawner Instance { get { return instance_; } }

    [SerializeField] private GameObject pointTextPrefab;

    private void Awake()
    {
        if(instance_ == null)
        {
            instance_ = this;
        } else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        if(instance_ == this)
        {
            instance_ = null;
        }
    }

    public void SpawnPointText(float points, Vector3 pos)
    {
        PointText pt = Instantiate(pointTextPrefab, pos, Quaternion.identity).GetComponent<PointText>();
        pt.SetPoints(points);
    }
}
