using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MeadowBatController : NetworkBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Player hit bat");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Player hit bat");
        }
    }
}
