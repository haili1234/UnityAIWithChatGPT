using System.Linq;
using UnityEngine;

namespace Crosstales.RTVoice.Demo.Util
{
   /// <summary>Enables or disable game objects for native mode.</summary>
   [HelpURL("https://www.crosstales.com/media/data/assets/rtvoice/api/class_crosstales_1_1_r_t_voice_1_1_demo_1_1_util_1_1_native_controller.html")]
   public class NativeController : MonoBehaviour
   {
      #region Variables

      ///<summary>Enable or disable the 'Objects' for native mode (default: true).</summary>
      [Header("Configuration")] [Tooltip("Enable or disable the 'Objects' for native mode (default: true).")]
      public bool Active = true;


      ///<summary>Selected objects for the controller.</summary>
      [Header("Objects")] [Tooltip("Selected objects for the controller.")] public GameObject[] Objects;

      #endregion


      #region MonoBehaviour methods

      public void Update()
      {
         foreach (var go in Objects.Where(go => go != null))
         {
            if (GUISpeech.isNative)
            {
               go.SetActive(Active);
            }
            else
            {
               go.SetActive(!Active);
            }
         }
      }

      #endregion
   }
}
// © 2016-2020 crosstales LLC (https://www.crosstales.com)