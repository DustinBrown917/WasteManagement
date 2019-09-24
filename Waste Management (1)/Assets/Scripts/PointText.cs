using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointText : MonoBehaviour
{
    [SerializeField] private Text label;

    private Vector3 vel;
    [SerializeField] private float driftHeight;
    [SerializeField] float smoothedTime = 0.5f;
    [SerializeField] float terminationDistance = 0.01f;
    [SerializeField] Sprite[] stars;
    [SerializeField] SpriteRenderer sr;
    [SerializeField] GameObject star;
    private Vector3 smoothDampTarget;
    

    // Start is called before the first frame update
    void Start()
    {
        smoothDampTarget = transform.position;
        smoothDampTarget.y += driftHeight;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position, smoothDampTarget, ref vel, smoothedTime);
        if(Vector3.Distance(transform.position, smoothDampTarget) < terminationDistance) { Destroy(gameObject); }
    }

    public void SetPoints(float points)
    {
        label.text = ((int)points).ToString();
        if(points == 100)
        {
            sr.sprite = stars[0];
        }
        else
        {
            star.transform.localScale = new Vector3(20, 20, 20);
            label.fontSize = 72;
            sr.sprite = stars[1];
        }
    }
}
