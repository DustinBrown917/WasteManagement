using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    private static ParticleManager instance_;
    public static ParticleManager Instance { get { return instance_; } }

    [SerializeField] private ParticleSystem[] particleSystems;

    private void Awake()
    {
        if(instance_ == null) {
            instance_ = this;
        } else {
            Destroy(gameObject);
        }
    }

    public void FireParticleSystem(int particleIndex, Vector3 location)
    {
        if(particleIndex < 0 || particleIndex >= particleSystems.Length) { return; }
        particleSystems[particleIndex].gameObject.transform.position = location;
        particleSystems[particleIndex].Play();
    }

    private void OnDestroy()
    {
        if(instance_ == this) { instance_ = null; }
    }
}
