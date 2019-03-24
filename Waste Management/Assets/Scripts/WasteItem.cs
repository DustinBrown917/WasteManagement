using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WasteItem : MonoBehaviour
{

    [SerializeField] private WasteBinTypes targetBinType_;
    public WasteBinTypes TargetBinType { get { return targetBinType_; } }

    [SerializeField] private Collider2D overlapCollider;
    [SerializeField] private ContactFilter2D filter;
    private bool canBeGrabbed_ = true;
    public bool CanBeGrabbed { get { return canBeGrabbed_; } set { canBeGrabbed_ = value; } }

    private bool drag = false;

    private Vector3 homePosition;
    private Coroutine cr_ReturnToHomePos = null;

    //****************************************************************************************\\
    //*********************************** UNITY BEHAVIOURS ***********************************\\
    //****************************************************************************************\\

    void Start()
    {
        homePosition = transform.position;
        WasteSpawner.Instance.RegisterItem(this);
    }

    public void OnMouseDown() //May need to swap out mouse functionality for touch functionality. Using mouse just for testing logic.
    {
        if(cr_ReturnToHomePos != null) { StopCoroutine(cr_ReturnToHomePos); }
        if (canBeGrabbed_) { drag = true; }
    }

    public void OnMouseDrag()
    {
        if (!drag) { return; }
        Vector3 newPos = GameManager.MainCamera.ScreenToWorldPoint(Input.mousePosition);
        newPos.z = transform.position.z;
        transform.position = newPos;
    }

    private void OnMouseUp()
    {
        drag = false;
        if (IsOverlappingCorrectBin()) {
            //Handle point distribution and disposal of item here.
            OnDisposedCorrectly();
            WasteSpawner.Instance.DeRegisterItem(this);
            Destroy(gameObject);
        }
        else {
            cr_ReturnToHomePos = StartCoroutine(ReturnToHomePos());
        }      
    }

    private void OnDestroy()
    {
        
    }

    //****************************************************************************************\\
    //************************************** BEHAVIOURS **************************************\\
    //****************************************************************************************\\

    /// <summary>
    /// Sets the position the object will lerp to when dropped in an invalid location.
    /// </summary>
    /// <param name="position"></param>
    public void SetHomePosition(Vector3 position)
    {
        homePosition = position;
    }

    /// <summary>
    /// Sets the position the object will lerp to when dropped in an invalid location.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public void SetHomePosition(float x, float y, float z)
    {
        SetHomePosition(new Vector3(x, y, z));
    }

    /// <summary>
    /// Smooth damps the objet back to its home position.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ReturnToHomePos()
    {
        Vector3 vel = new Vector3();

        while(Vector3.Distance(transform.position, homePosition) > 0.005f)
        {
            transform.position = Vector3.SmoothDamp(transform.position, homePosition, ref vel, 0.25f); //Swap magic number with class variable?
            yield return null;
        }

        transform.position = homePosition;
    }

    /// <summary>
    /// Checks that the closest bin being overlapped is the correct bin.
    /// </summary>
    /// <returns>True if the closest bin is of the correct type.</returns>
    private bool IsOverlappingCorrectBin()
    {
        Collider2D[] results = new Collider2D[4];
        overlapCollider.OverlapCollider(filter, results);

        float shortestDistance = 10000;

        WasteBin closestBin = null;

        foreach(Collider2D c in results)
        {
            if(c == null) { continue; }
            float newDistance = Vector3.Distance(transform.position, c.gameObject.transform.position);
            if (newDistance < shortestDistance)
            {
                shortestDistance = newDistance;
                WasteBin wb = c.gameObject.GetComponent<WasteBin>();

                if(wb == null) { continue; }
                if(wb.BinType == targetBinType_) { closestBin = wb; }
                else { wb = null; }
            }
        }

        return closestBin != null;
    }


    //****************************************************************************************\\
    //**************************************** EVENTS ****************************************\\
    //****************************************************************************************\\

    /// <summary>
    /// Called immediately before the object is destroyed once it has been disposed correctly.
    /// Listeners could use this to update score, trigger animation, etc.
    /// </summary>
    public event EventHandler DisposedCorrectly;

    private void OnDisposedCorrectly()
    {
        EventHandler handler = DisposedCorrectly;

        if(handler != null) { handler(this, EventArgs.Empty); }
    }

    /// <summary>
    /// Presently not used.
    /// </summary>
    public event EventHandler IncorrectDisposalAttempt;

    private void OnIncorrectDisposalAttempt()
    {
        EventHandler handler = IncorrectDisposalAttempt;

        if (handler != null) { handler(this, EventArgs.Empty); }
    }

}
