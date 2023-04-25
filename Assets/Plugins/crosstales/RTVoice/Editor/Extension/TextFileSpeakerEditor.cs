#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Crosstales.RTVoice.EditorUtil;

namespace Crosstales.RTVoice.EditorExtension
{
   /// <summary>Custom editor for the 'TextFileSpeaker'-class.</summary>
   [CustomEditor(typeof(Tool.TextFileSpeaker))]
   [CanEditMultipleObjects]
   public class TextFileSpeakerEditor : Editor
   {
      #region Variables

      private Tool.TextFileSpeaker script;

      #endregion


      #region Editor methods

      public void OnEnable()
      {
         script = (Tool.TextFileSpeaker)target;
      }

      public void OnDisable()
      {
         if (Util.Helper.isEditorMode)
         {
            Speaker.Silence();
         }
      }

      public override void OnInspectorGUI()
      {
         DrawDefaultInspector();

         EditorHelper.SeparatorUI();

         if (script.isActiveAndEnabled)
         {
            if (script.TextFiles != null && script.TextFiles.Length > 0)
            {
               if (script.PlayOnStart && script.PlayAllOnStart)
               {
                  EditorGUILayout.HelpBox("Can't use 'Play On Start' and 'Play All On Start' in combination. Please decide for one approach!", MessageType.Warning);
               }
               else
               {
                  if (Speaker.isTTSAvailable && EditorHelper.isRTVoiceInScene)
                  {
                     GUILayout.Label("Test-Drive", EditorStyles.boldLabel);

                     if (Util.Helper.isEditorMode)
                     {
                        if (Speaker.isWorkingInEditor)
                        {
                           GUILayout.BeginHorizontal();
                           {
                              /*
                              if (GUILayout.Button(new GUIContent(" Previous", EditorHelper.Icon_Previous, "Plays the previous radio station.")))
                              {
                                  script.Previous();
                              }
                              */

                              if (Speaker.isSpeaking)
                              {
                                 if (GUILayout.Button(new GUIContent(" Silence", EditorHelper.Icon_Silence, "Silence the active speaker.")))
                                 {
                                    script.Silence();
                                 }
                              }
                              else
                              {
                                 if (GUILayout.Button(new GUIContent(" Speak", EditorHelper.Icon_Speak, "Speaks a random text file with the selected voice and settings.")))
                                 {
                                    script.Speak();
                                 }
                              }

                              /*
                              if (GUILayout.Button(new GUIContent(" Next", EditorHelper.Icon_Next, "Speaks the next text file.")))
                              {
                                  script.Next();
                              }
                              */
                           }
                           GUILayout.EndHorizontal();
                        }
                        else
                        {
                           EditorGUILayout.HelpBox("Test-Drive is not supported for current TTS-system inside the Unity Editor.", MessageType.Info);
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
// © 2016-2020 crosstales LLC (https://www.crosstales.com)