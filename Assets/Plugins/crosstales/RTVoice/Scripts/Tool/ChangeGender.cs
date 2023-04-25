using UnityEngine;

namespace Crosstales.RTVoice.Tool
{
   /// <summary>Change the gender of all voices (useful for eSpeak).</summary>
   [HelpURL("https://www.crosstales.com/media/data/assets/rtvoice/api/class_crosstales_1_1_r_t_voice_1_1_tool_1_1_change_gender.html")]
   [ExecuteInEditMode]
   public class ChangeGender : MonoBehaviour
   {
      #region Variables

      /// <summary>The new gender for all voices.</summary>
      [Tooltip("The new gender for all voices.")] public Model.Enum.Gender NewGender;

/*
        /// <summary>Refresh on voices ready (default: true).</summary>
        [Tooltip("Refresh on voices ready (default: true).")]
        public bool RefreshOnVoicesReady = true;
*/
      /// <summary>Change voices only when eSpeak is used (default: true).</summary>
      [Tooltip("Change voices only when eSpeak is used (default: true).")] public bool ESpeakOnly = true;

      #endregion


      #region MonoBehaviour methods

      public void Start()
      {
         Speaker.OnVoicesReady += Change;
      }

      public void OnDestroy()
      {
         Speaker.OnVoicesReady -= Change;
      }

      #endregion


      #region Public methods

      public void GenderChanged(System.Int32 index)
      {
         NewGender = (Model.Enum.Gender)index;

         Change();
      }

      public void Change()
      {
         if (!ESpeakOnly || ESpeakOnly && Speaker.isESpeakMode && !Speaker.isMaryMode)
         {
            foreach (Model.Voice voice in Speaker.Voices)
            {
               voice.Gender = NewGender;
            }
         }
      }

      #endregion
   }
}
// © 2018-2020 crosstales LLC (https://www.crosstales.com)