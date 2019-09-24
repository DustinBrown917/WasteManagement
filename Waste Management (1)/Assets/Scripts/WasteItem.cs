using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WasteItem : MonoBehaviour
{
    private static float initialScore = 100;
    private static float pointDeductionStartTime = 1.0f;
    private static float timeToMinPoints = 7.5f;
    private static float minScore = 5.0f;
    private static float maxDistanceToBin = 1.0f;

    public WasteBinTypes TargetBinType { get { return stages[disposalStage].properWasteBin; } }

    [SerializeField] private Collider2D overlapCollider;
    [SerializeField] private ContactFilter2D filter;
    private bool canBeGrabbed_ = true;
    public bool CanBeGrabbed { get { return canBeGrabbed_; } set { canBeGrabbed_ = value; } }

    public SeparatableItem parentItem;

    public bool isSubItem { get; set; }
    private bool drag = false;
    private float timeInExistence = 0;
    private float pointLossRate = (initialScore - minScore) / timeToMinPoints;
    private float scoreValue;

    [SerializeField] private WasteItemDisposalStageInfo[] stages;
    private Animator animator;
    private int MaxDisposalStages { get { return stages.Length; } }
    private int disposalStage = 0;

    private bool isInitialized = false;

    private Vector3 homePosition;
    private Coroutine cr_ReturnToHomePos = null;
    private Coroutine cr_LerpToSize = null;
    private Vector3 initialScale;
    private Quaternion initialRotation;

    private Coroutine cr_WaitForAnimation;

    [SerializeField] private AudioClip touchNoise;
    [SerializeField] private AudioClip correctDisposalNoise;
    [SerializeField] private AudioClip incorrectDisposalNoise;

    //****************************************************************************************\\
    //*********************************** UNITY BEHAVIOURS ***********************************\\
    //****************************************************************************************\\

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        if(!isInitialized) {
            isInitialized = true;
            return; }
        if (!isSubItem)
        {
            SetAnimation();
        }
        scoreValue = initialScore;
        homePosition = transform.position;
        WasteSpawner.Instance.RegisterItem(this);
        timeInExistence = 0.0f;
    }

    void Start()
    {
        initialScale = transform.localScale;
        initialRotation = transform.rotation;
    }

    public void PlaySound(AudioClip clip)
    {
        SFXManager.Instance.PlaySound(clip);
    }

    private void Update()
    {
        timeInExistence += Time.deltaTime;
        if (timeInExistence > pointDeductionStartTime && timeInExistence < pointDeductionStartTime + timeToMinPoints) {
            scoreValue -= pointLossRate * Time.deltaTime;
            if(scoreValue < minScore) { scoreValue = minScore; }
        }
    }

    public void OnMouseDown() //May need to swap out mouse functionality for touch functionality. Using mouse just for testing logic.
    {
        if (GameManager.isTouchBlocked) { return; }
        if(cr_ReturnToHomePos != null) { StopCoroutine(cr_ReturnToHomePos); }
        if (canBeGrabbed_) {
            drag = true;
            if(cr_LerpToSize != null) {
                StopCoroutine(cr_LerpToSize);
            }

            BinManager.Instance.GetItemBins(stages[disposalStage].properWasteBin);
            cr_LerpToSize = StartCoroutine(LerpToScale(1.0f, 0.25f, 20.0f));
            SFXManager.Instance.PlaySound(touchNoise);

            // Tell 2 bins to lerp to the positions here
            BinManager.Instance.LerpBinsToCenter();
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
            if (scoreValue == 100) {

            } else if (scoreValue < 100 && scoreValue >= 75) {
                scoreValue = 75;
            } else if (scoreValue < 75 && scoreValue >= 50) {
                scoreValue = 50;
            } else if (scoreValue < 50 && scoreValue >= 25) {
                scoreValue = 25;
            } else {
                scoreValue = 5;
            }

            //Add the score
            ScoreCounter.Instance.AddScore(scoreValue);

            //Spawn the appropriate particle effect
            if(scoreValue == 100) {
                ParticleManager.Instance.FireParticleSystem(0, overlapCollider.transform.position + new Vector3(0, 0, 10.0f));
            } else {
                ParticleManager.Instance.FireParticleSystem(1, overlapCollider.transform.position + new Vector3(0, 0, 10.0f));
            }

            //Handle disposal stages
            if(disposalStage >= MaxDisposalStages-1)
            {
                //If this is the final disposal stage, deregister the item and return it to the pool
                WasteSpawner.Instance.DeRegisterItem(this);
                PointTextSpawner.Instance.SpawnPointText(scoreValue, transform.position);

                if (!isSubItem) {
                    WasteSpawner.Instance.AddItemToPool(gameObject);
                }

                SFXManager.Instance.PlaySound(correctDisposalNoise);
                OnDisposedCorrectly(new DisposedCorrectlyArgs(this));
                BinManager.Instance.LerpBinsToHome();

                if (parentItem != null) {
                    parentItem.RestoreAllItemPoints();
                }

            } else {
                //Otherwise progress to the next disposal stage
                disposalStage++;
                
                SFXManager.Instance.PlaySound(correctDisposalNoise);
                PointTextSpawner.Instance.SpawnPointText(scoreValue, transform.position);
                SetAnimation();

                if (stages[disposalStage].waitForCompletion) {
                    cr_WaitForAnimation = StartCoroutine(ReturnAfterAnimation(stages[disposalStage].animationName));
                } else {
                    ReturnToHome();
                    BinManager.Instance.LerpBinsToHome();
                    ResetScoreValue();
                }  
                
                if(parentItem != null) {
                    parentItem.RestoreAllItemPoints();
                }
            }

        }
        else {
            SFXManager.Instance.PlaySound(incorrectDisposalNoise);
            ReturnToHome();
            BinManager.Instance.LerpBinsToHome();
        }
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
    /// Resets the scoreValue to the initialScore.
    /// </summary>
    public void ResetScoreValue()
    {
        scoreValue = initialScore;
        timeInExistence = 0.0f;
    }

    /// <summary>
    /// Reset scale and rotation to initial values.
    /// </summary>
    public void ResetScaleAndRotation()
    {
        StopAllCoroutines();
        disposalStage = 0;
        transform.localScale = initialScale;
        transform.rotation = initialRotation;
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

        foreach(Collider2D c in results)
        {
            if(c == null) { continue; }
            float distance = Vector2.Distance(transform.position, c.gameObject.transform.position);
            if(distance < maxDistanceToBin)
            {
                WasteBin wb = c.GetComponent<WasteBin>();
                if(wb != null)
                {
                    if(wb.BinType == stages[disposalStage].properWasteBin)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Begins the routine that will lerp the object back to its home position.
    /// </summary>
    public void ReturnToHome()
    {
        cr_ReturnToHomePos = StartCoroutine(ReturnToHomePos());

        if (cr_LerpToSize != null) {
            StopCoroutine(cr_LerpToSize);
        }

        cr_LerpToSize = StartCoroutine(LerpToScale(0.75f, 0.25f, 0.0f));
    }

    /// <summary>
    /// Sets the animation that should be played via the stage string.
    /// </summary>
    public void SetAnimation()
    {
        if(animator == null || disposalStage >= stages.Length) { return; }
        animator.Play(stages[disposalStage].animationName);
    }

    /// <summary>
    /// Coroutine to swell the item to a specified scale.
    /// </summary>
    /// <param name="newScale">What size should the item lerp to?</param>
    /// <param name="timeToScale">How long should the swell take?</param>
    /// <param name="targetRotation">How much should the item rotate as it swells?</param>
    /// <returns></returns>
    private IEnumerator LerpToScale(float newScale, float timeToScale, float targetRotation)
    {
        float elapsedTime = 0;
        float startScale = transform.localScale.x;
        float startRotation = transform.eulerAngles.z;
        float currentScale = startScale;
        float currentRotation = startRotation;

        while (elapsedTime < timeToScale)
        {
            currentScale = Mathf.Lerp(startScale, newScale, elapsedTime / timeToScale);
            transform.localScale = new Vector3(currentScale, currentScale, currentScale);
            currentRotation = Mathf.Lerp(startRotation, targetRotation, elapsedTime / timeToScale);
            transform.eulerAngles = new Vector3(0, 0, currentRotation);
           
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        transform.localScale = new Vector3(newScale, newScale, newScale);
        transform.eulerAngles = new Vector3(0, 0, targetRotation);

    }

    /// <summary>
    /// Coroutine that causes the object to postpone other behaviours while an animation is playing.
    /// </summary>
    /// <param name="animationName">The animation to wait for.</param>
    /// <returns></returns>
    private IEnumerator ReturnAfterAnimation(string animationName)
    {
        GameManager.isTouchBlocked = true;
        transform.GetComponent<Collider2D>().enabled = false;
        yield return new WaitForEndOfFrame();

        Timer.Instance.AddTime(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);

        while (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == animationName && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }

        ReturnToHome();
        BinManager.Instance.LerpBinsToHome();
        ResetScoreValue();
        cr_WaitForAnimation = null;

        transform.GetComponent<Collider2D>().enabled = true;
        GameManager.isTouchBlocked = false;
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
        DisposedCorrectly?.Invoke(this, args);
    }

    /// <summary>
    /// Presently not used.
    /// </summary>
    public event EventHandler IncorrectDisposalAttempt;

    private void OnIncorrectDisposalAttempt()
    {
        IncorrectDisposalAttempt?.Invoke(this, EventArgs.Empty);
    }

}
