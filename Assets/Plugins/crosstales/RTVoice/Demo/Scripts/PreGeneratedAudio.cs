using UnityEngine;

namespace Crosstales.RTVoice.Demo
{
   /// <summary>Simple example with pre-generated audio for exact timing.</summary>
   [HelpURL("https://www.crosstales.com/media/data/assets/rtvoice/api/class_crosstales_1_1_r_t_voice_1_1_demo_1_1_pre_generated_audio.html")]
   public class PreGeneratedAudio : MonoBehaviour
   {
      #region Variables

      public string SpeechText = "This is an example with pre-generated audio for exact timing (e.g. animations).";
      public bool PlayOnStart = false;

      private AudioSource audioSource;
      private bool isPlayed = false;

      #endregion


      #region MonoBehaviour methods

      public void Start()
      {
         audioSource = gameObject.AddComponent<AudioSource>();

         Speaker.Speak(SpeechText, audioSource, Speaker.VoiceForCulture("en", 1), false);
      }

      public void Update()
      {
         if (!RTVoice.Util.Helper.hasActiveClip(audioSource) && isPlayed)
         {
            Stop();
         }
      }

      public void OnEnable()
      {
         // Subscribe event listeners
         Speaker.OnSpeakAudioGenerationComplete += speakAudioGenerationCompleteMethod;
      }

      public void OnDisable()
      {
         // Unsubscribe event listeners
         Speaker.OnSpeakAudioGenerationComplete -= speakAudioGenerationCompleteMethod;
      }

      #endregion


      #region Public methods

      public void Play()
      {
         Debug.Log("Play your animations!");

         isPlayed = true;

         audioSource.Play();

         //Here belongs your stuff, like animations
      }

      public void Silence()
      {
         audioSource.Stop();
      }

      public void Stop()
      {
         Debug.Log("Stop your animations!");

         isPlayed = false;

         //Here belongs your stuff, like animations
      }

      #endregion

      private void speakAudioGenerationCompleteMethod(Model.Wrapper wrapper,AudioClip clip)
      {
         if (PlayOnStart)
         {
            Play();
         }
      }
   }
}
// © 2015-2020 crosstales LLC (https://www.crosstales.com)