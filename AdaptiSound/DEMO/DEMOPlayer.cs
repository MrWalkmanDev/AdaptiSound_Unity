using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdaptiSound
{

public class DEMOPlayer : MonoBehaviour
{
    private Rigidbody2D rb;
    public float speed = 1f; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
    float moveHorizontal = Input.GetAxis("Horizontal");
    float moveVertical = Input.GetAxis("Vertical");

    Vector2 movement = new Vector2(moveHorizontal, moveVertical).normalized;
    rb.velocity = movement * speed;
    }
}

}
