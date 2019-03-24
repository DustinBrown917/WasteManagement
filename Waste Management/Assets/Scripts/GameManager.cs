using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    private static GameManager instance_ = null;
    public static GameManager Instance { get { return instance_; } }

    private static Camera mainCamera_ = null;
    public static Camera MainCamera { get
        {
            if(mainCamera_ == null) { mainCamera_ = Camera.main; }
            return mainCamera_;
        }
    }

    public int score;
    public Text scoreText;

    private void Awake()
    {
        if(instance_ == null) {
            instance_ = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    public void UpdateScore()
    {
        scoreText.text = "" + score;
    }

    private void OnDestroy()
    {
        if(instance_ == this) { instance_ = null; }
    }
}
