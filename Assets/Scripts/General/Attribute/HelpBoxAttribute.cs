using System;
using UnityEngine;
//自定义特性[HelpBox]
//显示提示框
//[HelpBox("tipppp", HelpBoxType.xxxx/*enum selection*/, 0/*shown order*/)]
public enum HelpBoxType
{
    None,
    Info,
    Warning,
    Error,
}
[AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
public sealed class HelpBoxAttribute : PropertyAttribute
{
    public string Message;
    public HelpBoxType Type;
    
    public HelpBoxAttribute(string message, HelpBoxType type = HelpBoxType.None, int order = 0)
    {
        Message = message;
        Type = type;
        this.order = order; //PropertyAttribute.order 在多个DecoratorDrawer叠加时 设置调用次序
    }
}
