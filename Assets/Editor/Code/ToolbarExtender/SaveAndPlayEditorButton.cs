using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ToolbarExtender
{
    [InitializeOnLoad]
    public class SaveAndPlayEditorButton
    {
        private const string PlayerPrefsPreviousScenePath = "Circum_PrevOpenScene";
        private static readonly Texture _texture;
    
        static SaveAndPlayEditorButton()
        {
            _texture = EditorGUIUtility.IconContent(@"winbtn_mac_inact").image;
            ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);

            EditorApplication.playModeStateChanged += ChangingPlayModeState;
        }

        static void ChangingPlayModeState(PlayModeStateChange stateChange)
        {
            if (stateChange != PlayModeStateChange.EnteredEditMode || !PlayerPrefs.HasKey(PlayerPrefsPreviousScenePath))
            {
                return;
            }
        
            string scenePath = PlayerPrefs.GetString(PlayerPrefsPreviousScenePath);
            if (string.IsNullOrEmpty(scenePath))
            {
                return;
            }
        
            PlayerPrefs.SetString(PlayerPrefsPreviousScenePath, "");
            EditorSceneManager.OpenScene(scenePath);
        }

        static void OnToolbarGUI()
        {
            GUILayout.Space(225);

            GUI.enabled = !EditorApplication.isPlaying;

            if (GUILayout.Button(new GUIContent(_texture, "Play from Boot"), ToolbarStyles.s_commandButtonStyle))
            {
                var currentScene = SceneManager.GetActiveScene();
                PlayerPrefs.SetString(PlayerPrefsPreviousScenePath, currentScene.path);

                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    Scene scene = SceneManager.GetSceneAt(i);
                    EditorSceneManager.MarkSceneDirty(scene);
                    EditorSceneManager.SaveScene(scene);
                }

                EditorSceneManager.OpenScene(EditorBuildSettings.scenes[0].path, OpenSceneMode.Single);
                EditorApplication.EnterPlaymode();
            }
        }
    }
}