using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BinManager : MonoBehaviour
{

    private static BinManager instance_ = null;
    public static BinManager Instance { get { return instance_; } }

    [SerializeField] private GameObject[] bins;
    [SerializeField] private Transform[] binStartingPositions;
    [SerializeField] private Vector3[] lerpToPositions;
    [SerializeField] private Vector3[] lerpFromPositions;

    [SerializeField] private float binLerpTime = 0.25f;

    private WasteBinTypes correctBinType;

    [SerializeField] private int randomNumber;

    // Determines whether the correct bin is the first bin or the second bin
    [SerializeField] private int correctBinNumber; 

    private void Awake()
    {
        if (instance_ == null) {
            instance_ = this;   
        } else {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        GameManager.Instance.GameOver += GameManager_GameOver;
    }

    private void GameManager_GameOver(object sender, System.EventArgs e)
    {
        ReturnLerp();
    }

    private void StartLerp()
    {
        if (correctBinNumber == 0)
        {
            bins[randomNumber].GetComponent<WasteBin>().LerpTo(lerpFromPositions[1], lerpToPositions[1], binLerpTime);
        }
        else
        {
            bins[randomNumber].GetComponent<WasteBin>().LerpTo(lerpFromPositions[0], lerpToPositions[0], binLerpTime);
        }
        bins[(int)correctBinType].GetComponent<WasteBin>().LerpTo(lerpFromPositions[correctBinNumber], lerpToPositions[correctBinNumber], binLerpTime);
    }

    private void ReturnLerp()
    {
        float dist = Vector3.Distance(lerpFromPositions[0], binStartingPositions[0].position);

        if (correctBinNumber == 0)
        {
            bins[randomNumber].GetComponent<WasteBin>()?.LerpTo(lerpFromPositions[1], binStartingPositions[1].position, binLerpTime);
        }
        else
        {
            bins[randomNumber].GetComponent<WasteBin>()?.LerpTo(lerpFromPositions[0], binStartingPositions[0].position, binLerpTime);
        }
        bins[(int)correctBinType].GetComponent<WasteBin>()?.LerpTo(lerpFromPositions[correctBinNumber], binStartingPositions[correctBinNumber].position, binLerpTime);
    }

    public void LerpBinsToCenter()
    {
        if (correctBinNumber == 0)
        {
            lerpFromPositions[1] = bins[randomNumber].transform.position;
        }
        else
        {
            lerpFromPositions[0] = bins[randomNumber].transform.position;
        }
        lerpFromPositions[correctBinNumber] = bins[(int)correctBinType].transform.position;
        // At this point we have determined that the number doesn't match, therefore we have our incorrect bin

        // Get the bin matching the random number 0 = Waste, 1 = Recycling, 2 = Organic, 3 = Paper
        StartLerp();
    }

    public void LerpBinsToHome()
    {
        if (correctBinNumber == 0)
        {
            lerpFromPositions[1] = bins[randomNumber].transform.position;
        }
        else
        {
            lerpFromPositions[0] = bins[randomNumber].transform.position;
        }
        lerpFromPositions[correctBinNumber] = bins[(int)correctBinType].transform.position;

        // Get the bin matching the random number 0 = Waste, 1 = Recycling, 2 = Organic, 3 = Paper
        ReturnLerp();
    }

    public void GetItemBins(WasteBinTypes correctType)
    {
        // Store the correctType
        correctBinType = correctType;

        // Get a random number to determine the incorrect bin
        randomNumber = Random.Range(0, 4);

        // The number will be 0, 1, 2 or 3. Each of these will represent one incorrect bin
        //print(randomNumber);

        // Check if the number is equal to the byte value of the enum
        // The number matches
        if ((int)correctType == randomNumber)
        {
            // Make the number not match
            if (randomNumber == 3) {
                randomNumber = 0;
            } else {
                randomNumber++;
            }
        }

        correctBinNumber = Random.Range(0, 2); 
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        foreach(Vector3 pos in lerpToPositions)
        {
            Gizmos.DrawWireSphere(pos, 0.5f);
        }

        Gizmos.color = Color.cyan;
        foreach (Vector3 pos in lerpFromPositions)
        {
            Gizmos.DrawWireSphere(pos, 0.5f);
        }
    }

    private void OnDestroy()
    {
        if(instance_ == this) { instance_ = null; }
    }

}
