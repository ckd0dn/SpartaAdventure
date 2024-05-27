using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    private LayerMask playerLayerMask;
    public float jumpForce;

    private void Start()
    {
        playerLayerMask = LayerMask.NameToLayer("Player");
    }

    private void OnCollisionEnter(Collision collision)
    {
            if(collision.gameObject.layer == playerLayerMask)
        {
            Rigidbody rb  = collision.gameObject.GetComponent<Rigidbody>();
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); 
        }
    }
}
