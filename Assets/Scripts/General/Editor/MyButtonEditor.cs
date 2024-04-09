using UnityEditor;//编辑器类在UnityEditor命名空间下。所以当使用C#脚本时，你需要在脚本前面加上 "using UnityEditor"引用。
using UnityEditor.UI;//ButtonEditor位于此命名空间下
using UnityEngine.Events;

//指定我们要自定义编辑器的脚本 
[CustomEditor(typeof(MyButton), true)]
//使用了 SerializedObject 和 SerializedProperty 系统，因此，可以自动处理“多对象编辑”，“撤销undo” 和 “预制覆盖prefab override”。
[CanEditMultipleObjects]
public class MyButtonEditor : ButtonEditor
{
    //对应我们在MyButton中创建的字段
    //PS:需要注意一点，使用SerializedProperty 必须在MyButton类_newNumber字段前加[SerializeField]
    //private SerializedProperty _newNumber;
    private SerializedProperty onPointerDown;
    private SerializedProperty onPointerUp;
    private SerializedProperty longPressTime;
    private SerializedProperty onStartLongPress;
    protected override void OnEnable()
    {
        base.OnEnable();
        //_newNumber = serializedObject.FindProperty("_newNumber");
        onPointerDown = serializedObject.FindProperty("onPointerDown");
        onPointerUp = serializedObject.FindProperty("onPointerUp");
        longPressTime = serializedObject.FindProperty("longPressTime");
        onStartLongPress = serializedObject.FindProperty("onStartLongPress");
    }
    //并且特别注意，如果用这种序列化方式，需要在 OnInspectorGUI 开头和结尾各加一句 serializedObject.Update();  serializedObject.ApplyModifiedProperties();
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();//空行
        serializedObject.Update();
        EditorGUILayout.PropertyField(onPointerDown);//显示我们创建的属性
        EditorGUILayout.PropertyField(onPointerUp);//显示我们创建的属性
        EditorGUILayout.PropertyField(longPressTime);//显示我们创建的属性
        EditorGUILayout.PropertyField(onStartLongPress);//显示我们创建的属性
        serializedObject.ApplyModifiedProperties();
    }
}