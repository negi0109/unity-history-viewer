using UnityEditor;
using UnityEngine;

public class GameObjectHistoryWindow : EditorWindow
{
    private GameObject target;

    [MenuItem("Histories/GameObject")]
    private static void CreateWindow()
    {
        GetWindow<GameObjectHistoryWindow>();
    }

    public void OnGUI()
    {
        // 初期化処理
        if (target != Selection.activeTransform?.gameObject)
        {
            Debug.Log($"Selection: {target} -> {Selection.activeObject}");
            target = Selection.activeTransform?.gameObject;
        }

        if (target != null)
        {
            EditorGUILayout.LabelField(target.name);
        }
        else
        {

        }
    }

    private void Update()
    {
        if (target != Selection.activeTransform?.gameObject) Repaint();
    }
}
