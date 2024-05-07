using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Reflection;
public class ShadowCasterSetter : MonoBehaviour
{
    void Awake()
    {
        StartCoroutine(F());
    }
    IEnumerator F()
    {
        ShadowCaster2D shadowCaster2D = GetComponent<ShadowCaster2D>();
        EdgeCollider2D edgeCollider2D = GetComponent<EdgeCollider2D>();
        FieldInfo m_ShapePath = typeof(ShadowCaster2D).GetField("m_ShapePath", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        FieldInfo m_Mesh = typeof(ShadowCaster2D).GetField("m_Mesh", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        shadowCaster2D.enabled = false;
        m_ShapePath.SetValue(shadowCaster2D, null);
        yield return 0;
        m_Mesh.SetValue(shadowCaster2D, null);
        m_ShapePath.SetValue(shadowCaster2D, V2_to_3(edgeCollider2D.points));
        shadowCaster2D.enabled = true;
    }
    Vector3[] V2_to_3(Vector2[] v2)
    {
        Vector3[] v3 = new Vector3[v2.Length];
        for(int i=0;i<v2.Length; i++)
            v3[i] = v2[i];
        return v3;
    }
}
