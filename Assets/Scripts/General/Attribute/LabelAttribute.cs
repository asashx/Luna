using UnityEngine;
//自定义特性[Label]
//允许变量以别的名字显示在Inspector面板
//[Label("xxx")]
public class LabelAttribute : PropertyAttribute
{
    public string label;
    public LabelAttribute(string label)
    {
        this.label = label;
    }
}