using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerGUA : MonoBehaviour
{
    [Label("跳跃力度")]
    [SerializeField]
    private float jumpForce = 1f;
    [Label("左右平移速度")]
    [SerializeField]
    private float moveSpeed = 1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.W))
        {
            GetComponent<Rigidbody2D>().AddForce(new(0, jumpForce));
        }
        if(Input.GetKeyDown(KeyCode.S))
        {
            GetComponent<Rigidbody2D>().AddForce(new(0, -jumpForce));
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(new(-moveSpeed, 0));
        }
        if(Input.GetKey(KeyCode.D))
        {
            transform.Translate(new(moveSpeed, 0));
        }
    }
    
}
