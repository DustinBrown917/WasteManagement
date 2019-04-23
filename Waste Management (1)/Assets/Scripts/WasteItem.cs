using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WasteItem : MonoBehaviour
{
    private static float initialScore = 1000;
    private static float pointDeductionStartTime = 1.0f;
    private static float timeToZeroPoints = 5.0f;

    [SerializeField] private WasteBinTypes targetBinType_;
    public WasteBinTypes TargetBinType { get { return targetBinType_; } }

    [SerializeField] private Collider2D overlapCollider;
    [SerializeField] private ContactFilter2D filter;
    private bool canBeGrabbed_ = true;
    public bool CanBeGrabbed { get { return canBeGrabbed_; } set { canBeGrabbed_ = value; } }

    public bool isSubItem { get; set; }
    private bool drag = false;
    private float timeInExistence = 0;
    private float pointLossRate = initialScore / timeToZeroPoints;
    private float scoreValue;

    private Vector3 homePosition;
    private Coroutine cr_ReturnToHomePos = null;

    [SerializeField] private AudioClip touchNoise;
    [SerializeField] private AudioClip correctDisposalNoise;
    [SerializeField] private AudioClip incorrectDisposalNoise;

    //****************************************************************************************\\
    //*********************************** UNITY BEHAVIOURS ***********************************\\
    //****************************************************************************************\\

    private void OnEnable()
    {
        if(GameManager.Instance.GameState != GameStates.PLAYING) { return; }
        scoreValue = initialScore;
        homePosition = transform.position;
        WasteSpawner.Instance.RegisterItem(this);
        timeInExistence = 0.0f;
    }

    void Start()
    {
        
    }

    private void Update()
    {
        timeInExistence += Time.deltaTime;
        if (timeInExistence > pointDeductionStartTime && timeInExistence < pointDeductionStartTime + timeToZeroPoints) {
            scoreValue -= pointLossRate * Time.deltaTime;
            if(scoreValue < 0) { scoreValue = 0; }
        }
    }

    public void OnMouseDown() //May need to swap out mouse functionality for touch functionality. Using mouse just for testing logic.
    {
        if(cr_ReturnToHomePos != null) { StopCoroutine(cr_ReturnToHomePos); }
        if (canBeGrabbed_) {
            drag = true;
            SFXManager.Instance.PlaySound(touchNoise);
        }
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
            
            ScoreCounter.Instance.AddScore(scoreValue);
            WasteSpawner.Instance.DeRegisterItem(this);
            PointTextSpawner.Instance.SpawnPointText(scoreValue, transform.position);
            if (!isSubItem) {
                WasteSpawner.Instance.AddItemToPool(gameObject);
            }
            SFXManager.Instance.PlaySound(correctDisposalNoise);
            OnDisposedCorrectly(new DisposedCorrectlyArgs(this));
        }
        else {
            SFXManager.Instance.PlaySound(incorrectDisposalNoise);
            ReturnToHome();
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
        cr_ReturnToHomePos = null;
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

    public void ReturnToHome()
    {
        cr_ReturnToHomePos = StartCoroutine(ReturnToHomePos());
    }


    //****************************************************************************************\\
    //**************************************** EVENTS ****************************************\\
    //****************************************************************************************\\

    /// <summary>
    /// Called immediately before the object is destroyed once it has been disposed correctly.
    /// Listeners could use this to update score, trigger animation, etc.
    /// </summary>
    public event EventHandler<DisposedCorrectlyArgs> DisposedCorrectly;

    public class DisposedCorrectlyArgs : EventArgs
    {
        public WasteItem wasteItem;

        public DisposedCorrectlyArgs(WasteItem wasteItem_) {
            wasteItem = wasteItem_;
        }
    }

    private void OnDisposedCorrectly(DisposedCorrectlyArgs args)
    {
        EventHandler<DisposedCorrectlyArgs> handler = DisposedCorrectly;

        if(handler != null) { handler(this, args); }
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
