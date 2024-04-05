using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlatForm : MonoBehaviour
{
    [SerializeField]
    float moveSpeed = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Alpha1))
        {
            transform.Translate(new(-moveSpeed, 0, 0));
        }
        if(Input.GetKey(KeyCode.Alpha2))
        {
            transform.Translate(new(moveSpeed, 0, 0));
        }
    }
}
