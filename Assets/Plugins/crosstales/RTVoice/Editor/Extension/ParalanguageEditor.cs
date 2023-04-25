#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Crosstales.RTVoice.EditorExtension
{
   /// <summary>Custom editor for the 'Paralanguage'-class.</summary>
   [CustomEditor(typeof(Tool.Paralanguage))]
   [CanEditMultipleObjects]
   public class ParalanguageEditor : Editor
   {
      #region Variables

      private Tool.Paralanguage script;

      #endregion


      #region Editor methods

      public void OnEnable()
      {
         script = (Tool.Paralanguage)target;
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

         if (script.isActiveAndEnabled)
         {
            if (!string.IsNullOrEmpty(script.Text))
            {
               if (Speaker.isTTSAvailable && EditorUtil.EditorHelper.isRTVoiceInScene)
               {
                  //TODO add stuff if needed
               }
               else
               {
                  EditorUtil.EditorHelper.NoVoicesUI();
               }
            }
            else
            {
               EditorGUILayout.HelpBox("Please enter a 'Text'!", MessageType.Warning);
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