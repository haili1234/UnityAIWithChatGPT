#if UNITY_EDITOR
using UnityEditor;
using Crosstales.RTVoice.EditorUtil;

namespace Crosstales.RTVoice.EditorExtension
{
   /// <summary>Custom editor for the 'Loudspeaker'-class.</summary>
   [CustomEditor(typeof(Tool.Loudspeaker))]
   public class LoudspeakerEditor : Editor
   {
      #region Variables

      private Tool.Loudspeaker script;

      #endregion


      #region Editor methods

      public void OnEnable()
      {
         script = (Tool.Loudspeaker)target;
      }

      public override void OnInspectorGUI()
      {
         DrawDefaultInspector();

         if (script.isActiveAndEnabled)
         {
            if (script.Source != null)
            {
               //TODO add stuff if needed
            }
            else
            {
               EditorHelper.SeparatorUI();
               EditorGUILayout.HelpBox("Please add a 'Source'!", MessageType.Warning);
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