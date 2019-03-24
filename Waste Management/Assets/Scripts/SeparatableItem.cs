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
        Debug.Log("Click");
        Separate();
    }

    //****************************************************************************************\\
    //************************************** BEHAVIOURS **************************************\\
    //****************************************************************************************\\

    /// <summary>
    /// Begin the routine to separate the item into its individual components.
    /// </summary>
    private void Separate()
    {
        if (hasBeenSeparated) { return; }
        StartCoroutine(SeparateRoutine());
    }

    /// <summary>
    /// Smooth damps the item's subItems to their target positions, then unparents them and deletes self.
    /// </summary>
    /// <returns></returns>
    private IEnumerator SeparateRoutine()
    {
        hasBeenSeparated = true;
        Vector2[] velocities = new Vector2[subItems.Length];
        int numOfItemsInPosition = 0;

        while(numOfItemsInPosition < subItems.Length) {
            for(int i = 0; i < subItems.Length; i++) {
                if(i >= subItemTravelTargets.Length) { continue; } //If there is no travel target, move on.

                float distance = Vector2.Distance(subItems[i].transform.position, subItemTravelTargets[i]);

                if(distance <= Mathf.Epsilon) { continue; } //If already at travel target, move on.

                Vector3 newPos = Vector2.SmoothDamp(subItems[i].transform.position, subItemTravelTargets[i], ref velocities[i], 0.25f);
                if(Vector2.Distance(newPos, subItemTravelTargets[i]) < 0.005f) {
                    newPos = subItemTravelTargets[i];
                    numOfItemsInPosition++;

                    WasteItem wi = subItems[i].GetComponent<WasteItem>();
                    if(wi != null) {
                        wi.CanBeGrabbed = true;
                        wi.SetHomePosition(newPos.x, newPos.y, wi.transform.position.z);
                    }
                }

                newPos.z = subItems[i].transform.position.z;
                subItems[i].transform.position = newPos;
            }

            yield return null;
        }

        foreach(GameObject si in subItems)
        {
            if(si.transform.parent == this.transform) {
                si.transform.parent = transform.parent;
            }
        }

        Destroy(gameObject);
    }
}
