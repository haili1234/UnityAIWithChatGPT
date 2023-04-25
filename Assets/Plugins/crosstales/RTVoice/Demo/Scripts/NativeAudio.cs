using UnityEngine;

namespace Crosstales.RTVoice.Demo
{
   /// <summary>Simple example with native audio for exact timing.</summary>
   [HelpURL("https://www.crosstales.com/media/data/assets/rtvoice/api/class_crosstales_1_1_r_t_voice_1_1_demo_1_1_native_audio.html")]
   public class NativeAudio : MonoBehaviour
   {
      #region Variables

      public string SpeechText = "This is an example with native audio for exact timing (e.g. animations).";
      public bool PlayOnStart = false;
      public float Delay = 1f;

      #endregion


      #region MonoBehaviour methods

      public void Start()
      {
         if (PlayOnStart)
         {
            //Invoke(nameof(StartTTS), Delay); //Invoke the TTS-system after x seconds
            Invoke("StartTTS", Delay); //Invoke the TTS-system after x seconds
         }
      }

      public void OnEnable()
      {
         // Subscribe event listeners
         Speaker.OnSpeakStart += play;
         Speaker.OnSpeakComplete += stop;
      }

      public void OnDisable()
      {
         // Unsubscribe event listeners
         Speaker.OnSpeakStart -= play;
         Speaker.OnSpeakComplete -= stop;
      }

      #endregion


      #region Public methods

      public void StartTTS()
      {
         Speaker.SpeakNative(SpeechText, Speaker.VoiceForCulture("en", 1));
      }

      public void Silence()
      {
         Speaker.Silence();
      }

      #endregion


      #region Callback methods

      private static void play(Model.Wrapper wrapper)
      {
         Debug.Log("Play your animations to the event: " + wrapper);

         //Here belongs your stuff, like animations
      }

      private static void stop(Model.Wrapper wrapper)
      {
         Debug.Log("Stop your animations from the event: " + wrapper);

         //Here belongs your stuff, like animations
      }

      #endregion
   }
}
// © 2015-2020 crosstales LLC (https://www.crosstales.com)