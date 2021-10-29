using System.Collections.Generic;
using System.Linq;
using Code.Level.Player;
using Code.UI;
using Lean.Localization;
using UnityEditor;
using UnityExtras.Code.Core;

#if UNITY_EDITOR
using Code.Core;
using Code.Flow;
using UnityEngine;
using Lean.Localization.Editor;
#endif

namespace Code.Debugging
{
    public static class CircumTools
    {
#if UNITY_EDITOR
        [MenuItem("Circum/Setup recording view")]
        public static void SetupRecordingView()
        {
            Player player = Object.FindObjectOfType<Player>();
            GameObject cameraPosRoot = new GameObject("CameraPositionRoot", typeof(PositionBetweenTransforms));
            PositionBetweenTransforms positionBetweenTransforms = cameraPosRoot.GetComponent<PositionBetweenTransforms>();
            positionBetweenTransforms.Setup(player.transform, player.OrbiterTransform);
            
            Transform child = new GameObject("Offset").transform;
            child.SetParent(cameraPosRoot.transform);
            child.localPosition = Vector3.up * 3.5f;
            
            Camera main = Camera.main;
            main.orthographicSize = 3;
            
            Transform camera = main.transform;
            Transform cameraRoot = camera.root;
            cameraRoot.SetParent(child);
            cameraRoot.localPosition = Vector3.zero;

            AspectRatioToScreenSize[] aspectRatioController = GameObject.FindObjectsOfType<AspectRatioToScreenSize>();
            aspectRatioController.ApplyFunction(p => p.enabled = false);

            GameObject.Find("GameUI").SetActive(false);
            GameObject.Find("AppUI").SetActive(false);
        }
        
        [MenuItem("Circum/Language/English")]
        public static void ChangeLanguageToEnglish()
        {
            LeanLocalization.SetCurrentLanguageAll("English");
        }
        
        [MenuItem("Circum/Language/French")]
        public static void ChangeLanguageToFrench()
        {
            LeanLocalization.SetCurrentLanguageAll("French");
        }
        
        [MenuItem("Circum/Language/Russian")]
        public static void ChangeLanguageToRussian()
        {
            LeanLocalization.SetCurrentLanguageAll("Russian");
        }

        [MenuItem("Circum/Persistent data/Reset splash screen")]
        public static void ResetSplashScreen()
        {
            PersistentDataHelper.DeleteKey(PersistentDataKeys.SplashScreenLastRunTime, true);
        }

        [MenuItem("Circum/Persistent data/Reset saved data")]
        public static void ResetSavedPlayerStats()
        {
            Object.FindObjectOfType<PersistentDataManager>().ResetStats();
        }

        [MenuItem("Circum/Persistent data/Set starter player stats")]
        public static void SetStarterPlayerStats()
        {
            PlayerStats.SetStarterPlayerStats();
        }
        
        [MenuItem("Circum/Persistent data/Set perfect player stats")]
        public static void SetPerfectPlayerStats()
        {
            PlayerStats.SetPerfectPlayerStats();
        }

        [MenuItem("Circum/Persistent data/Reset player firsts")]
        public static void ResetPlayerFirsts()
        {
            PlayerFirsts.ResetPlayerFirsts();
        }
        
        [MenuItem("Circum/Build/Run validation")]
        public static void RunValidation()
        {
            IEnumerable<IValidateable> validateables = Object.FindObjectsOfType<MonoBehaviour>().OfType<IValidateable>();
            validateables.ApplyFunction(v => v.Validate());

            LeanPhrase[] localizations = Object.FindObjectsOfType<LeanPhrase>();
            localizations.ApplyFunction(p => p.Entries.ApplyFunction(q =>
            {
                if (string.IsNullOrEmpty(q.Text))
                {
                    Debug.LogWarning($"There is no entry in localisation {p.gameObject} for {q.Language}");
                }
            }));
            
#if CIRCUM_LOGGING
            Debug.LogWarning("Circum logging is still enabled. Don't want this for a published build");
#endif
            
            Debug.Log("--- Validation complete ---");
        }

        [MenuItem("Circum/Translate/Auto translate all")]
        public static void AutoTranslateAll()
        {
            AutoTranslateScene(true);
        }

        [MenuItem("Circum/Translate/Auto translate new")]
        public static void AutoTranslateNew()
        {
            AutoTranslateScene(false);
        }

        [MenuItem("Circum/Translate/Auto translate selected")]
        public static void AutoTranslateSelected()
        {
            GameObject[] selected = Selection.gameObjects;
            LeanPhrase[] phrases = selected.Select(p => p.GetComponent<LeanPhrase>()).Where(p => p).ToArray();
            
            if (phrases.Length == 0)
            {
                Debug.LogError("Could not find a phrase ");
                return;
            }

            LeanLanguage[] languages = GetLanguages();
            AutoTranslatePhrases(phrases, languages, true);
        }

        private static LeanLanguage[] GetLanguages()
        {
            LeanLocalization localisation = Object.FindObjectOfType<LeanLocalization>();
            return localisation.Prefabs.Select(p => (p.Root as GameObject)?.GetComponent<LeanLanguage>()).ToArray();
        }

        private static void AutoTranslateScene(bool force)
        {
            LeanPhrase[] phrases = Object.FindObjectsOfType<LeanPhrase>();
            LeanLanguage[] languages = GetLanguages();

            AutoTranslatePhrases(phrases, languages, force);
        }

        private static void AutoTranslatePhrases(LeanPhrase[] phrases, LeanLanguage[] languages, bool force)
        {
            string englishLanguageName = "English";
            
            foreach (LeanPhrase phrase in phrases)
            {
                LeanPhrase.Entry english = phrase.Entries.FirstOrDefault(p => p.Language.EqualsIgnoreCase(englishLanguageName));
                if (english == null)
                {
                    CircumDebug.Log($"Cannot find entry for english for {phrase.gameObject}");
                    english = phrase.AddEntry(englishLanguageName, phrase.gameObject.name);
                }

                if (string.IsNullOrEmpty(english.Text))
                {
                    english.Text = phrase.name;
                }

                string textOutput = default;
                string languageCodeInput = "en";
                
                Undo.RecordObject(phrase, "Auto Translate");

                foreach (LeanLanguage leanLanguage in languages)
                {
                    if (leanLanguage.name.EqualsIgnoreCase(englishLanguageName))
                    {
                        continue;
                    }
                    
                    LeanPhrase.Entry entry = phrase.Entries.FirstOrDefault(p => p.Language.EqualsIgnoreCase(leanLanguage.name)) ?? phrase.AddEntry(leanLanguage.name, phrase.name);

                    if (string.IsNullOrEmpty(entry.Text) && !force)
                    {
                        continue;
                    }
                    
                    if (LeanPhrase_Editor.TryAutoTranslate(languageCodeInput, leanLanguage.TranslationCode, english.Text, ref textOutput))
                    {
                        entry.Text = textOutput;
                    }
                }
                
                EditorUtility.SetDirty(phrase);
            }
        }
#endif
    }
}