using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameObjectHistoryWindow : EditorWindow
{
    private GameObject _target;
    private Scene _currentScene;


    [MenuItem("Histories/GameObject")]
    private static void CreateWindow()
    {
        GetWindow<GameObjectHistoryWindow>();
    }

    public void OnGUI()
    {
        // ゲームオブジェクトの選択
        if (_target != Selection.activeTransform?.gameObject && Selection.activeTransform?.gameObject != null)
        {
            Debug.Log($"Selection: {_target} -> {Selection.activeObject}");
            _target = Selection.activeTransform?.gameObject;
        }

        if (_target != null)
        {
            EditorGUILayout.LabelField(_target.name);
        }
        else
        {

        }
    }

    private void InitScene()
    {
        _currentScene = SceneManager.GetActiveScene();
        Debug.Log(_currentScene.path);
        Debug.Log(GitCommandUtil.ExecGitCommand($"log -n 30 --pretty=\" % H % x09 % s\" -- {_currentScene.path}"));
        Repaint();
    }

    private void Update()
    {
        var currentScene = SceneManager.GetActiveScene();

        if (!currentScene.Equals(_currentScene)) InitScene();
        if (_target != Selection.activeTransform?.gameObject) Repaint();
    }
}
