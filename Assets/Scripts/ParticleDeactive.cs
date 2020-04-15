using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDeactive : MonoBehaviour
{
    private float lifetime;

    private void OnEnable()
    {
        lifetime = GetComponent<ParticleSystem>().main.startLifetime.Evaluate(1);
    }

    private void Update()
    {
        lifetime -= Time.fixedDeltaTime;
        if(lifetime <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
