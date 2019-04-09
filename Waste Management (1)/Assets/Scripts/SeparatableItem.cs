using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeparatableItem : MonoBehaviour
{
    [SerializeField] private GameObject[] subItems;
    [SerializeField, Tooltip("Where will the sub items travel to once separated?")] private Vector2[] subItemTravelTargets;

    bool hasBeenSeparated = false;

    //****************************************************************************************\\
    //*********************************** UNITY BEHAVIOURS ***********************************\\
    //****************************************************************************************\\

    private void Start()
    {
        foreach(GameObject go in subItems)
        {
            WasteItem wi = go.GetComponent<WasteItem>();
            if(wi != null) { wi.CanBeGrabbed = false; }
        }
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
        if(hasBeenSeparated) { return; }
        hasBeenSeparated = true;
        for(int i = 0; i < subItems.Length; i++)
        {
            WasteItem wi = subItems[i].GetComponent<WasteItem>();
            if(wi != null)
            {
                wi.SetHomePosition(subItemTravelTargets[i]);
                wi.ReturnToHome();
                wi.CanBeGrabbed = true;

                if (wi.transform.parent == this.transform) {
                    wi.transform.parent = transform.parent;
                }
            }
        }

        Destroy(gameObject);
    }
}
