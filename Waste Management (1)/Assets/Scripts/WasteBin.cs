using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WasteBin : MonoBehaviour
{
    [SerializeField] private WasteBinTypes binType_;
    public WasteBinTypes BinType { get { return binType_; } }
}

public enum WasteBinTypes
{
    TRASH,
    RECYCLING,
    ORGANIC,
    PAPER
}
