using UnityEngine;

namespace Crosstales.RTVoice.Demo.Util
{
   /// <summary>Enables MaryTTS on iOS for specific scenes.</summary>
   [HelpURL("https://www.crosstales.com/media/data/assets/rtvoice/api/class_crosstales_1_1_r_t_voice_1_1_demo_1_1_util_1_1i_o_s_controller.html")]
   public class iOSController : MonoBehaviour
   {
      public void Start()
      {
         if (RTVoice.Util.Helper.hasBuiltInTTS)
         {
            Speaker.isMaryMode = RTVoice.Util.Helper.isIOSBasedPlatform;
         }
      }

      public void OnDestroy()
      {
         if (RTVoice.Util.Helper.hasBuiltInTTS)
         {
            Speaker.isMaryMode = false;
         }
      }
   }
}
// © 2016-2020 crosstales LLC (https://www.crosstales.com)