using UnityEngine;
using UnityEngine.SceneManagement;

namespace Crosstales.RTVoice.Demo
{
   /// <summary>Main GUI scene manager for all demo scenes.</summary>
   [HelpURL("https://www.crosstales.com/media/data/assets/rtvoice/api/class_crosstales_1_1_r_t_voice_1_1_demo_1_1_g_u_i_scenes.html")]
   public class GUIScenes : MonoBehaviour
   {
      #region Variables

      [Tooltip("Name of the previous scene.")] public string PreviousScene;

      [Tooltip("Name of the previous scene (WebGL only).")] public string PreviousSceneWebGL;

      [Tooltip("Name of the next scene.")] public string NextScene;

      [Tooltip("Name of the next scene (WebGL only).")] public string NextSceneWebGL;

      public void LoadPreviousScene()
      {
         Speaker.Silence();
         SceneManager.LoadScene(RTVoice.Util.Helper.isWebGLPlatform ? PreviousSceneWebGL : PreviousScene);
      }

      #endregion


      #region Public methods

      public void LoadNextScene()
      {
         Speaker.Silence();
         SceneManager.LoadScene(RTVoice.Util.Helper.isWebGLPlatform ? NextSceneWebGL : NextScene);
      }

      #endregion
   }
}
// © 2015-2020 crosstales LLC (https://www.crosstales.com)