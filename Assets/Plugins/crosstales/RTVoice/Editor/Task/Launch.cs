#if UNITY_EDITOR
using UnityEditor;
using Crosstales.RTVoice.EditorUtil;

namespace Crosstales.RTVoice.EditorTask
{
   /// <summary>Show the configuration window on the first launch.</summary>
   [InitializeOnLoad]
   public static class Launch
   {
      #region Constructor

      static Launch()
      {
         bool launched = EditorPrefs.GetBool(EditorConstants.KEY_LAUNCH);

         if (!launched)
         {
            EditorIntegration.ConfigWindow.ShowWindow(4);
            EditorPrefs.SetBool(EditorConstants.KEY_LAUNCH, true);
         }
      }

      #endregion
   }
}
#endif
// © 2017-2020 crosstales LLC (https://www.crosstales.com)