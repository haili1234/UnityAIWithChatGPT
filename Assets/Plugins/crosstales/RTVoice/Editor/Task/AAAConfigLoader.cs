#if UNITY_EDITOR
using UnityEditor;

namespace Crosstales.RTVoice.EditorTask
{
   /// <summary>Loads the configuration at startup.</summary>
   [InitializeOnLoad]
   public static class AAAConfigLoader
   {
      #region Constructor

      static AAAConfigLoader()
      {
         //UnityEngine.Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name);

         if (!Util.Config.isLoaded)
         {
            Util.Config.Load();

            if (Util.Config.DEBUG)
               UnityEngine.Debug.Log("Config data loaded");
         }
      }

      #endregion
   }
}
#endif
// © 2017-2020 crosstales LLC (https://www.crosstales.com)