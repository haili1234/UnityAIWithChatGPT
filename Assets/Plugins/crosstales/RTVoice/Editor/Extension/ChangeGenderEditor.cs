#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Crosstales.RTVoice.EditorUtil;

namespace Crosstales.RTVoice.EditorExtension
{
   /// <summary>Custom editor for the 'ChangeGender'-class.</summary>
   [CustomEditor(typeof(Tool.ChangeGender))]
   [CanEditMultipleObjects]
   public class ChangeGenderEditor : Editor
   {
      #region Variables

      private Tool.ChangeGender script;

      #endregion


      #region Editor methods

      public void OnEnable()
      {
         script = (Tool.ChangeGender)target;
      }

      public override void OnInspectorGUI()
      {
         DrawDefaultInspector();

         EditorHelper.SeparatorUI();

         if (script.isActiveAndEnabled)
         {
            if (Speaker.isTTSAvailable && EditorHelper.isRTVoiceInScene)
            {
               GUILayout.Label("Action", EditorStyles.boldLabel);

               if (Util.Helper.isEditorMode)
               {
                  if (GUILayout.Button(new GUIContent(" Change Gender", EditorHelper.Icon_Refresh, "Change the gender of all voices (useful for eSpeak).")))
                  {
                     script.Change();
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
            EditorGUILayout.HelpBox("Script is disabled!", MessageType.Info);
         }
      }

      #endregion
   }
}
#endif
// © 2019 crosstales LLC (https://www.crosstales.com)