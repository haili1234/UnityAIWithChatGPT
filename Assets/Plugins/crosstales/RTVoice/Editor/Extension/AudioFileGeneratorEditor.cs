#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Crosstales.RTVoice.EditorUtil;

namespace Crosstales.RTVoice.EditorExtension
{
   /// <summary>Custom editor for the 'SpeechText'-class.</summary>
   [CustomEditor(typeof(Tool.AudioFileGenerator))]
   [CanEditMultipleObjects]
   public class AudioFileGeneratorEditor : Editor
   {
      #region Variables

      private Tool.AudioFileGenerator script;

      #endregion


      #region Editor methods

      public void OnEnable()
      {
         script = (Tool.AudioFileGenerator)target;
      }

      public override void OnInspectorGUI()
      {
         DrawDefaultInspector();

         EditorHelper.SeparatorUI();

         if (script.isActiveAndEnabled)
         {
            if (script.TextFiles != null && script.TextFiles.Length > 0)
            {
               if (Speaker.isTTSAvailable && EditorHelper.isRTVoiceInScene)
               {
                  if (Util.Helper.isEditorMode)
                  {
                     GUILayout.Label("Generate Audio Files", EditorStyles.boldLabel);

                     GUILayout.BeginHorizontal();
                     {
                        if (Speaker.isWorkingInEditor)
                        {
                           if (GUILayout.Button(new GUIContent(" Generate", EditorHelper.Icon_Speak, "Generates the speeches from the text files.")))
                           {
                              script.Generate();
                           }
                        }
                        else
                        {
                           EditorGUILayout.HelpBox("Generate is not supported for current TTS-system inside the Unity Editor.", MessageType.Info);
                        }
                     }
                     GUILayout.EndHorizontal();

                     EditorHelper.SeparatorUI();

                     GUILayout.Label("Editor", EditorStyles.boldLabel);

                     if (GUILayout.Button(new GUIContent(" Refresh AssetDatabase", EditorHelper.Icon_Refresh, "Refresh the AssetDatabase from the Editor.")))
                     {
                        EditorHelper.RefreshAssetDatabase();
                     }
                  }
                  else
                  {
                     EditorGUILayout.HelpBox("Disabled in Play-mode!", MessageType.Info);
                  }
               }
               else
               {
                  EditorHelper.NoVoicesUI();
               }
            }
            else
            {
               EditorGUILayout.HelpBox("Please add an entry to 'Text Files'!", MessageType.Warning);
            }
         }
         else
         {
            EditorGUILayout.HelpBox("Script is disabled!", MessageType.Info);
         }
      }

      #endregion
   }
}
#endif
// © 2017-2020 crosstales LLC (https://www.crosstales.com)