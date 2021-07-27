using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shootable : MonoBehaviour
{
    public float forwardVelocity = 4f;
    private Transform barrelTip;

    public void Start()
    {
        // shoot from camera, could change this to find a barrel tip gameobject e.g. using a tag
        barrelTip = Camera.main.transform;
    }

    public void Shoot()
    {
        transform.position = barrelTip.position;
        transform.rotation = barrelTip.rotation;

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * forwardVelocity;
        rb.isKinematic = false;
    }
}
