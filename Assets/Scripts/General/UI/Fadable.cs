using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//显现消失UI
//需要第0个子物体是背景、第1个子物体是内容

public class Fadable : MonoBehaviour
{
    public Image panelBack => transform.GetChild(0).GetComponent<Image>();
    public GameObject panelContent => transform.GetChild(1).gameObject;
    [HelpBox("第0个子物体是背景、第1个子物体是内容", HelpBoxType.Warning/*enum selection*/, 0/*shown order*/)]
    public float inDuration = 0.3f;
    public float outDuration = 3f;
    public void StartFade(bool isIn)
    {
        StopFade();
        StartCoroutine(Fade(isIn));
    }
    public void StopFade()
    {
        StopCoroutine(nameof(Fade));
    }
    IEnumerator Fade(bool isIn)
    {
        if (!isIn)
            panelContent.SetActive(false);
        panelBack.gameObject.SetActive(true);
        float time,timer;
        time = timer = isIn ? inDuration : outDuration;
        while (timer > 0)
        {
            panelBack.color = new Color(panelBack.color.r, panelBack.color.g, panelBack.color.b, (isIn ? (time - timer) : timer) / time);
            timer -= Time.deltaTime;
            yield return null;
        }
        if (!isIn)
            panelBack.gameObject.SetActive(false);
        if (isIn)
            panelContent.SetActive(true);
        yield break;
    }
}
