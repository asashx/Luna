using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Reflection;
public class ShadowCasterSetter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            F();
    }
    void F()
    {
        ShadowCaster2D shadowCaster2D = GetComponent<ShadowCaster2D>();
        EdgeCollider2D edgeCollider2D = GetComponent<EdgeCollider2D>();
        print("shadowCaster2D.shapePath.Length = " + shadowCaster2D.shapePath.Length);
        print("edgeCollider2D.points.Length = " + edgeCollider2D.points.Length);
        //Type[] types = Assembly.Load("Assembly-CSharp").GetTypes();
        //var mems = type.GetMembers(); //获取类型中的成员信息
        Type type = typeof(ShadowCaster2D);

        foreach(var it in type.GetMembers(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
            print(it.Name);
        FieldInfo field = type.GetField("m_ShapePath", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        field.SetValue(shadowCaster2D, V2_to_3(edgeCollider2D.points));
        print("shadowCaster2D.shapePath.Length2 = " + shadowCaster2D.shapePath.Length);
        //// 判断指定名称的字段合法且类型与反射配置一致
        //if (field != null)
        //{
        //    if (field.FieldType == type)
        //    {
        //        // 注入反射配置
        //        field.SetValue(TargetObj, result);
        //    }
        //    else
        //    {
        //        Debug.LogError($"Inject Error: {TargetObj.name} {VARIABLE.PropertyName} type is not match");
        //    }
        //}


        //Vector2[] added = new Vector2[edgeCollider2D.points.Length - shadowCaster2D.shapePath.Length];
        //shadowCaster2D.shapePath.AddRange(added);
        //print("shadowCaster2D.shapePath.Length2 = " + shadowCaster2D.shapePath.Length);
        //for (int i = 0;i< edgeCollider2D.points.Length;i++)
        //    shadowCaster2D.shapePath[i] = edgeCollider2D.points[i];


        //Array.Resize(ref shadowCaster2D.shapePath, edgeCollider2D.points.Length);
        //for (int i = 0; i < shadowCaster2D.shapePath.Length; i++)
        //    shadowCaster2D.shapePath[i] = edgeCollider2D.points[0];
        //shadowCaster2D.shapePath.AddRange(edgeCollider2D.points);
        //print("shadowCaster2D.shapePath.Length2 = " + shadowCaster2D.shapePath.Length);

        //foreach(var it in edgeCollider2D.points)
        //    print("Edge point :" + it.x + " , " + it.y);
        //for (int i = 0;i< shadowCaster2D.shapePath.Length;i++)
        //    shadowCaster2D.shapePath[i] = new(0, 0, 0);

        //GetComponent<ShadowCaster2D>().shapePath = GetComponent<EdgeCollider2D>().points;
    }
    Vector3[] V2_to_3(Vector2[] v2)
    {
        Vector3[] v3 = new Vector3[v2.Length];
        for(int i=0;i<v2.Length; i++)
            v3[i] = v2[i];
        return v3;
    }
}
