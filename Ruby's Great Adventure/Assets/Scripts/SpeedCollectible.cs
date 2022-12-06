using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedCollectible : MonoBehaviour
{
    public AudioClip boostClip;

    void OnTriggerEnter2D(Collider2D other)
    {
        RubyController controller = other.GetComponent<RubyController>();

        if (controller != null)
        {
            if(controller.isBoosted == false)
            {
            controller.SpeedPickup();
            Destroy(gameObject);
            controller.PlaySound(boostClip);
            }
        }
    }
}
