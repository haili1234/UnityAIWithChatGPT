#if UNITY_EDITOR
using UnityEditor;
using Crosstales.RTVoice.EditorUtil;

namespace Crosstales.RTVoice.EditorExtension
{
   /// <summary>Custom editor for the 'Sequencer'-class.</summary>
   [CustomEditor(typeof(Tool.Sequencer))]
   public class SequencerEditor : Editor
   {
      #region Variables

      private Tool.Sequencer script;

      #endregion


      #region Editor methods

      public void OnEnable()
      {
         script = (Tool.Sequencer)target;
      }

      public override void OnInspectorGUI()
      {
         DrawDefaultInspector();

         if (script.isActiveAndEnabled)
         {
            if (script.Sequences != null && script.Sequences.Length > 0)
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
               EditorGUILayout.HelpBox("Please add an entry to 'Sequences'!", MessageType.Warning);
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
// © 2016-2020 crosstales LLC (https://www.crosstales.com)