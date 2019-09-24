using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WasteBin : MonoBehaviour
{
    [SerializeField] private WasteBinTypes binType_;
    public WasteBinTypes BinType { get { return binType_; } }

    private Coroutine cr_LerpTo;


    public void LerpTo(Vector3 startPos, Vector3 endPos, float lerpDuration)
    {
        if(cr_LerpTo != null)
        {
            StopCoroutine(cr_LerpTo);
            cr_LerpTo = null;
        }

        cr_LerpTo = StartCoroutine(LerpToRoutine(startPos, endPos, lerpDuration));
    }

    private IEnumerator LerpToRoutine(Vector3 startPos, Vector3 endPos, float lerpDuration)
    {      

        float elapsedTime = 0;

        while(elapsedTime < lerpDuration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;

        cr_LerpTo = null;
    }
}

public enum WasteBinTypes
{
    TRASH,
    RECYCLING,
    ORGANIC,
    PAPER
}
