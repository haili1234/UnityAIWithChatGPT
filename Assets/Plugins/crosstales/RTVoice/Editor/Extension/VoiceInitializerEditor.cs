#if UNITY_EDITOR
using UnityEditor;
using Crosstales.RTVoice.EditorUtil;

namespace Crosstales.RTVoice.EditorExtension
{
   /// <summary>Custom editor for the 'VoiceInitalizer'-class.</summary>
   [CustomEditor(typeof(Tool.VoiceInitializer))]
   public class VoiceInitializerEditor : Editor
   {
      #region Variables

      private Tool.VoiceInitializer script;

      #endregion


      #region Editor methods

      public void OnEnable()
      {
         script = (Tool.VoiceInitializer)target;
      }

      public override void OnInspectorGUI()
      {
         DrawDefaultInspector();

         if (script.isActiveAndEnabled)
         {
            if (script.AllVoices || script.VoiceNames != null && script.VoiceNames.Length > 0)
            {
               if (!Speaker.isTTSAvailable && EditorHelper.isRTVoiceInScene)
               {
                  EditorHelper.SeparatorUI();
                  EditorHelper.NoVoicesUI();
               }
            }
            else
            {
               EditorHelper.SeparatorUI();
               EditorGUILayout.HelpBox("Please add an entry to 'Voice Names'!", MessageType.Warning);
            }
         }
         else
         {
            EditorHelper.SeparatorUI();
            EditorGUILayout.HelpBox("Script is disabled!", MessageType.Info);
         }
      }

      #endregion
   }
}
#endif
// © 2017-2020 crosstales LLC (https://www.crosstales.com)