using UnityEngine;
using System.Collections;

namespace Crosstales.RTVoice.Demo
{
   /// <summary>Simple dialog system with TTS voices.</summary>
   [HelpURL("https://www.crosstales.com/media/data/assets/rtvoice/api/class_crosstales_1_1_r_t_voice_1_1_demo_1_1_dialog.html")]
   public class Dialog : MonoBehaviour
   {
      #region Variables

      [Header("Configuration")] public string CultureA = "en";
      public string CultureB = "en";
      [Range(0f, 3f)] public float RateA = 1f;
      [Range(0f, 3f)] public float RateB = 1f;

      [Range(0f, 2f)] public float PitchA = 1f;
      [Range(0f, 2f)] public float PitchB = 1f;

      [Range(0f, 1f)] public float VolumeA = 1f;
      [Range(0f, 1f)] public float VolumeB = 1f;

      public Model.Enum.Gender GenderA = Model.Enum.Gender.UNKNOWN;
      public Model.Enum.Gender GenderB = Model.Enum.Gender.UNKNOWN;

      public AudioSource AudioPersonA;
      public AudioSource AudioPersonB;

      public Model.Enum.SpeakMode ModeA = Model.Enum.SpeakMode.Speak;
      public Model.Enum.SpeakMode ModeB = Model.Enum.SpeakMode.Speak;

      [Header("Dialogues")] public string[] DialogPersonA;
      public string[] DialogPersonB;
      public string CurrentDialogA = string.Empty;
      public string CurrentDialogB = string.Empty;

      public bool Running = false;

      private string uidSpeakerA;
      private string uidSpeakerB;

      private bool playingA = false;
      private bool playingB = false;

      #endregion


      #region MonoBehaviour methods

      public void OnEnable()
      {
         // Subscribe event listeners
         Speaker.OnSpeakStart += speakStartMethod;
         Speaker.OnSpeakComplete += speakCompleteMethod;
      }

      public void OnDisable()
      {
         // Unsubscribe event listeners
         Speaker.OnSpeakStart -= speakStartMethod;
         Speaker.OnSpeakComplete -= speakCompleteMethod;
      }

      #endregion


      #region Public methods

      public IEnumerator DialogSequence()
      {
         if (!Running)
         {
            Running = true;

            playingA = false;
            playingB = false;

            int index = 0;

            while (Running && DialogPersonA != null && index < DialogPersonA.Length || DialogPersonB != null && index < DialogPersonB.Length)
            {
               //Person A
               if (DialogPersonA != null && index < DialogPersonA.Length)
               {
                  CurrentDialogA = DialogPersonA[index];
               }

               uidSpeakerA = ModeA == Model.Enum.SpeakMode.Speak ? Speaker.Speak(CurrentDialogA, AudioPersonA, Speaker.VoiceForGender(GenderA, CultureA), true, RateA, PitchA, VolumeA) : Speaker.SpeakNative(CurrentDialogA, Speaker.VoiceForGender(GenderA, CultureA), RateA, PitchA, VolumeA);

               //wait until ready
               do
               {
                  yield return null;
               } while (!playingA && Running);

               //wait until played
               do
               {
                  yield return null;
               } while (playingA && Running);

               CurrentDialogA = string.Empty;

               if (Running)
               {
                  //ensure it's still running

                  // Person B
                  if (DialogPersonB != null && index < DialogPersonB.Length)
                  {
                     CurrentDialogB = DialogPersonB[index];
                  }

                  if (ModeB == Model.Enum.SpeakMode.Speak)
                  {
                     uidSpeakerB = Speaker.Speak(CurrentDialogB, AudioPersonB, Speaker.VoiceForGender(GenderB, CultureB, 1), true, RateB, PitchB, VolumeB);
                  }
                  else
                  {
                     uidSpeakerB = Speaker.SpeakNative(CurrentDialogB, Speaker.VoiceForGender(GenderB, CultureB, 1), RateB, PitchB, VolumeB);
                  }

                  //wait until ready
                  do
                  {
                     yield return null;
                  } while (!playingB && Running);

                  //wait until played
                  do
                  {
                     yield return null;
                  } while (playingB && Running);

                  CurrentDialogB = string.Empty;
               }

               index++;
            }

            Running = false;
         }
      }

      #endregion


      #region Callback methods

      private void speakStartMethod(Model.Wrapper wrapper)
      {
         if (wrapper.Uid.Equals(uidSpeakerA))
         {
            Debug.Log("speakStartMethod - Speaker A: " + wrapper);
            playingA = true;
         }
         else if (wrapper.Uid.Equals(uidSpeakerB))
         {
            Debug.Log("speakStartMethod - Speaker B: " + wrapper);
            playingB = true;
         }
         else
         {
            Debug.LogWarning("speakStartMethod - Unknown speaker: " + wrapper);

            Running = false;
         }
      }

      private void speakCompleteMethod(Model.Wrapper wrapper)
      {
         if (wrapper.Uid.Equals(uidSpeakerA))
         {
            Debug.Log("speakCompleteMethod - Speaker A: " + wrapper);
            playingA = false;
         }
         else if (wrapper.Uid.Equals(uidSpeakerB))
         {
            Debug.Log("speakCompleteMethod - Speaker B: " + wrapper);
            playingB = false;
         }
         else
         {
            Debug.LogWarning("speakCompleteMethod - Unknown speaker: " + wrapper);

            Running = false;
         }
      }

      #endregion
   }
}
// © 2015-2020 crosstales LLC (https://www.crosstales.com)