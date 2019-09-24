using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeparatableItem : MonoBehaviour
{
    [SerializeField] private GameObject[] subItems;
    private Vector3[] localPositions;
    [SerializeField, Tooltip("Where will the sub items travel to once separated?")] private Vector2[] subItemTravelTargets;

    bool hasBeenSeparated = false;
    private int currentlyAttachedItems = 0;

    //****************************************************************************************\\
    //*********************************** UNITY BEHAVIOURS ***********************************\\
    //****************************************************************************************\\

    private void OnEnable()
    {
        hasBeenSeparated = false;
        foreach(GameObject go in subItems)
        {
            Animator anim = go.GetComponent<Animator>();
            if (anim != null) { anim.enabled = false; }
        }
    }

    private void Start()
    {
        localPositions = new Vector3[subItems.Length];
        int index = 0;
        currentlyAttachedItems = subItems.Length;
        foreach (GameObject go in subItems)
        {
            WasteItem wi = go.GetComponent<WasteItem>();

            wi.parentItem = this;
            wi.isSubItem = true;
            localPositions[index] = wi.transform.localPosition;
            wi.DisposedCorrectly += WasteItem_DisposedCorrectly;
            if(wi != null) { wi.CanBeGrabbed = false; }
            index++;
            
        }
    }

    private void WasteItem_DisposedCorrectly(object sender, WasteItem.DisposedCorrectlyArgs e)
    {
        ReturnSubItem(e.wasteItem.gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        foreach(Vector2 v in subItemTravelTargets) {
            Gizmos.DrawWireSphere(v, 0.2f);
        }
    }

    private void OnMouseDown()
    {
        Separate();
    }

    //****************************************************************************************\\
    //************************************** BEHAVIOURS **************************************\\
    //****************************************************************************************\\


    /// <summary>
    /// Separate the item into its individual components.
    /// </summary>
    private void Separate()
    {
        transform.eulerAngles = Vector3.zero;
        if(hasBeenSeparated) { return; }
        hasBeenSeparated = true;
        for(int i = 0; i < subItems.Length; i++)
        {
            Animator anim = subItems[i].GetComponent<Animator>();
            if(anim != null) { anim.enabled = true; }
            WasteItem wi = subItems[i].GetComponent<WasteItem>();
            if(wi != null)
            {
                currentlyAttachedItems--;
                wi.SetHomePosition(subItemTravelTargets[i]);
                wi.ReturnToHome();
                wi.CanBeGrabbed = true;
                wi.ResetScoreValue();

                if (wi.transform.parent == this.transform) {
                    wi.transform.parent = transform.parent;
                }
            }
        }

        transform.position = WasteSpawner.Instance.PoolPosition;
    }

    private void ReturnSubItem(GameObject subItem)
    {
        int index = 0;
        foreach(GameObject go in subItems)
        {
            if(go == subItem) { break; }
            index++;
        }
        currentlyAttachedItems++;
        subItem.transform.parent = this.transform;
        subItem.transform.localPosition = localPositions[index];
        subItem.transform.eulerAngles = Vector3.zero;
        subItem.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        WasteItem wi = subItem.GetComponent<WasteItem>();
        wi.CanBeGrabbed = false;
        wi.ResetScaleAndRotation();
        wi.ResetScoreValue();

        if (currentlyAttachedItems == subItems.Length)
        {
            WasteSpawner.Instance.AddItemToPool(gameObject);
        }
    }

    public void RestoreAllItemPoints()
    {
        Debug.Log("THis");
        foreach(GameObject subItem in subItems)
        {
            subItem.GetComponent<WasteItem>().ResetScoreValue();
        }
    }
    
}
