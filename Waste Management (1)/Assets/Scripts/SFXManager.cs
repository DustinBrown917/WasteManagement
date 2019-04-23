using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SFXManager : MonoBehaviour
{
    private static SFXManager instance_ = null;
    public static SFXManager Instance { get { return instance_; } }

    [SerializeField] private AudioSource[] channels;

    private void Awake()
    {
        if(instance_ == null)
        {
            instance_ = this;
        } else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySound(AudioClip clip)
    {
        foreach(AudioSource audioSource in channels)
        {
            if (audioSource.isPlaying) { continue; }
            audioSource.clip = clip;
            audioSource.Play();
            break;
        }
    }

    private void OnDestroy()
    {
        if(instance_ == this)
        {
            instance_ = null;
        }
    }
}
