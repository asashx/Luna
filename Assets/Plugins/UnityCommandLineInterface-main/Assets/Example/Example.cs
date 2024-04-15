using System.Collections;
using System.Collections.Generic;
using RedSaw.CommandLineInterface;
using UnityEngine;

public class Example : MonoBehaviour
{
    //[CommandProperty("player")]
    //private static Example Instance { get; set; }

    //public Vector3 pos
    //{
    //    get => transform.position;
    //    set => transform.position = value;
    //}

    //public void Start()
    //{
    //    Example.Instance = this;
    //}

    //public void Jump(float value = 10)
    //{
    //    GetComponent<Rigidbody>().AddForce(Vector3.up * value, ForceMode.Impulse);
    //    transform.rotation = Random.rotation;
    //}
    [Command("v3")]
    public static Vector3 V3(float x = 0,float y = 0,float z = 0)
    {
        return new Vector3(x,y,z);
    }
    [Command("q")]
    public static Quaternion Q3(float x = 0,float y = 0,float z = 0)
    {
        return new Quaternion(x,y,z,1f);
    }
}
