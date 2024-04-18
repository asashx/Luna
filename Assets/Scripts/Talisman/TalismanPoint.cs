using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalismanPoint : MonoBehaviour
{
    [SerializeField]
    ObservableValue<bool> passed;
    SpriteRenderer spriteRenderer;
    Talisman talisman;
    private void Awake()
    {
        passed = new(false, OnPassedChange);
    }
    public void Initialize(Talisman t)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        talisman = t;
    }
    public void Refresh()
    {
        passed.Value = false;
    }
    void OnPassedChange(bool oldV, bool newV)
    {
        if(newV)
        {
            spriteRenderer.color = Color.green;
            talisman.DrawPoint(this);
            return;
        }
        spriteRenderer.color = Color.red;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Ball"))
        {
            if (passed.Value)
                return;
            passed.Value = true;
        }
        
    }
}
