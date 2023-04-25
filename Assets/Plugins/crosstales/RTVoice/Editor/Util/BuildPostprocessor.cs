#if UNITY_EDITOR && UNITY_STANDALONE_WIN
using UnityEditor;
using UnityEngine;
using UnityEditor.Callbacks;

namespace Crosstales.RTVoice.EditorUtil
{
   /// <summary>BuildPostprocessor for Windows. Adds the TTS-wrapper to the build.</summary>
   public static class BuildPostprocessor
   {
      [PostProcessBuildAttribute(1)]
      public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
      {
         if (EditorHelper.isWindowsPlatform)
         {
            string dataPath = pathToBuiltProject.Substring(0, pathToBuiltProject.Length - 4) + "_Data/";

            if (Util.Config.ENFORCE_32BIT_WINDOWS)
            {
               FileUtil.CopyFileOrDirectory(Application.dataPath + Util.Config.TTS_WINDOWS_EDITOR_x86, dataPath + "RTVoiceTTSWrapper.exe");
            }
            else
            {
               FileUtil.CopyFileOrDirectory(Application.dataPath + Util.Config.TTS_WINDOWS_EDITOR, dataPath + "RTVoiceTTSWrapper.exe");
            }

            if (Util.Config.DEBUG)
               Debug.Log("Wrapper copied to: " + dataPath);
         }
      }
   }
}
#endif
// © 2015-2020 crosstales LLC (https://www.crosstales.com)