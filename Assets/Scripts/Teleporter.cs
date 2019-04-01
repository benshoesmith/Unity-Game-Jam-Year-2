using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Teleporter : MonoBehaviour {

    [SerializeField]
    private GameObject player;

    [SerializeField]
    private GameObject teleportPosition;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject == player)
        {
            player.transform.position = teleportPosition.transform.position;
        }
    }
}
