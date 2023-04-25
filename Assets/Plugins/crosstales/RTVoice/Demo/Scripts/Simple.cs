using UnityEngine;
using UnityEngine.UI;

namespace Crosstales.RTVoice.Demo
{
   /// <summary>Simple TTS example.</summary>
   [HelpURL("https://www.crosstales.com/media/data/assets/rtvoice/api/class_crosstales_1_1_r_t_voice_1_1_demo_1_1_simple.html")]
   public class Simple : MonoBehaviour
   {
      #region Variables

      [Header("Configuration")] public AudioSource SourceA;
      public AudioSource SourceB;

      [Range(0f, 3f)] public float RateSpeakerA = 1.25f;

      [Range(0f, 3f)] public float RateSpeakerB = 1.75f;

      public bool PlayOnStart = false;

      [Header("UI Objects")] public Text TextSpeakerA;
      public Text TextSpeakerB;

      public Text PhonemeSpeakerA;
      public Text PhonemeSpeakerB;

      public Text VisemeSpeakerA;
      public Text VisemeSpeakerB;

      private string uidSpeakerA;
      private string uidSpeakerB;

      private string textA = "Text A";
      private string textB = "Text B";

      private Model.Wrapper currentWrapper;

      private bool silent = true;

      #endregion


      #region MonoBehaviour methods

      public void Start()
      {
         if (TextSpeakerA != null)
            textA = TextSpeakerA.text;

         if (TextSpeakerB != null)
            textB = TextSpeakerB.text;

         if (PlayOnStart)
         {
            Play();
         }
      }

      public void OnEnable()
      {
         // Subscribe event listeners
         Speaker.OnSpeakAudioGenerationStart += speakAudioGenerationStartMethod;
         Speaker.OnSpeakAudioGenerationComplete += speakAudioGenerationCompleteMethod;
         Speaker.OnSpeakCurrentWord += speakCurrentWordMethod;
         Speaker.OnSpeakCurrentPhoneme += speakCurrentPhonemeMethod;
         Speaker.OnSpeakCurrentViseme += speakCurrentVisemeMethod;
         Speaker.OnSpeakStart += speakStartMethod;
         Speaker.OnSpeakComplete += speakCompleteMethod;
      }

      public void OnDisable()
      {
         // Unsubscribe event listeners
         Speaker.OnSpeakAudioGenerationStart -= speakAudioGenerationStartMethod;
         Speaker.OnSpeakAudioGenerationComplete -= speakAudioGenerationCompleteMethod;
         Speaker.OnSpeakCurrentWord -= speakCurrentWordMethod;
         Speaker.OnSpeakCurrentPhoneme -= speakCurrentPhonemeMethod;
         Speaker.OnSpeakCurrentViseme -= speakCurrentVisemeMethod;
         Speaker.OnSpeakStart -= speakStartMethod;
         Speaker.OnSpeakComplete -= speakCompleteMethod;
      }

      #endregion


      #region Public methods

      public void Play()
      {
         silent = false;

         if (TextSpeakerA != null)
            TextSpeakerA.text = textA;

         if (TextSpeakerB != null)
            TextSpeakerB.text = textB;

         //usedGuids.Clear();

         SpeakerA(); //start with speaker A
         //SpeakerB(); //start with speaker B
      }

      public void SpeakerA()
      {
         //Don't speak the text immediately
         uidSpeakerA = Speaker.Speak(textA, SourceA, Speaker.VoiceForGender(Model.Enum.Gender.MALE, "en"), false, RateSpeakerA);
      }

      public void SpeakerB()
      {
         //Don't speak the text immediately
         uidSpeakerB = Speaker.Speak(textB, SourceB, Speaker.VoiceForGender(Model.Enum.Gender.FEMALE, "en"), false, RateSpeakerB);
      }

      public void Silence()
      {
         silent = true;
         Speaker.Silence();

         if (SourceA != null)
            SourceA.Stop();

         if (SourceB != null)
            SourceB.Stop();

         if (TextSpeakerA != null)
            TextSpeakerA.text = textA;

         if (TextSpeakerB != null)
            TextSpeakerB.text = textB;

         VisemeSpeakerB.text = PhonemeSpeakerB.text = VisemeSpeakerA.text = PhonemeSpeakerA.text = "-";
      }

      #endregion


      #region Private methods

      private void speakAudio()
      {
         Speaker.SpeakMarkedWordsWithUID(currentWrapper);
      }

      #endregion


      #region Callback methods

      private static void speakAudioGenerationStartMethod(Model.Wrapper wrapper)
      {
         Debug.Log("speakAudioGenerationStartMethod: " + wrapper);
      }

      private void speakAudioGenerationCompleteMethod(Model.Wrapper wrapper)
      {
         Debug.Log("speakAudioGenerationCompleteMethod: " + wrapper);
         currentWrapper = wrapper;

         //Invoke(nameof(speakAudio), 0.1f); //needs a small delay
         Invoke("speakAudio", 0.1f); //needs a small delay
      }

      private void speakStartMethod(Model.Wrapper wrapper)
      {
         if (wrapper.Uid.Equals(uidSpeakerA))
         {
            Debug.Log("Speaker A - Speech start: " + wrapper);
         }
         else if (wrapper.Uid.Equals(uidSpeakerB))
         {
            Debug.Log("Speaker B - Speech start: " + wrapper);
         }
         else
         {
            Debug.LogWarning("Unknown speaker: " + wrapper);
         }
      }

      private void speakCompleteMethod(Model.Wrapper wrapper)
      {
         if (wrapper.Uid.Equals(uidSpeakerA))
         {
            Debug.Log("Speaker A - Speech complete: " + wrapper);

            if (TextSpeakerA != null)
               TextSpeakerA.text = wrapper.Text;

            if (VisemeSpeakerA != null)
               VisemeSpeakerA.text = PhonemeSpeakerA.text = "-";

            if (!silent)
               SpeakerB();
         }
         else if (wrapper.Uid.Equals(uidSpeakerB))
         {
            Debug.Log("Speaker B - Speech complete: " + wrapper);

            if (TextSpeakerB != null)
               TextSpeakerB.text = wrapper.Text;

            if (VisemeSpeakerB != null)
               VisemeSpeakerB.text = PhonemeSpeakerB.text = "-";

            if (!silent)
               SpeakerA();
         }
         else
         {
            Debug.LogWarning("Unknown speaker: " + wrapper);
         }
      }

      private void speakCurrentWordMethod(Model.Wrapper wrapper, string[] speechTextArray, int wordIndex)
      {
         if (wrapper.Uid.Equals(uidSpeakerA))
         {
            if (TextSpeakerA != null)
               TextSpeakerA.text = RTVoice.Util.Helper.MarkSpokenText(speechTextArray, wordIndex);
         }
         else if (wrapper.Uid.Equals(uidSpeakerB))
         {
            if (TextSpeakerB != null)
               TextSpeakerB.text = RTVoice.Util.Helper.MarkSpokenText(speechTextArray, wordIndex);
         }
         else
         {
            Debug.LogWarning("Unknown speaker: " + wrapper);
         }
      }

      private void speakCurrentPhonemeMethod(Model.Wrapper wrapper, string phoneme)
      {
         if (wrapper.Uid.Equals(uidSpeakerA))
         {
            if (PhonemeSpeakerA != null)
               PhonemeSpeakerA.text = phoneme;
         }
         else if (wrapper.Uid.Equals(uidSpeakerB))
         {
            if (PhonemeSpeakerB != null)
               PhonemeSpeakerB.text = phoneme;
         }
         else
         {
            Debug.LogWarning("Unknown speaker: " + wrapper);
         }
      }

      private void speakCurrentVisemeMethod(Model.Wrapper wrapper, string viseme)
      {
         if (wrapper.Uid.Equals(uidSpeakerA))
         {
            if (VisemeSpeakerA != null)
               VisemeSpeakerA.text = viseme;
         }
         else if (wrapper.Uid.Equals(uidSpeakerB))
         {
            if (VisemeSpeakerB != null)
               VisemeSpeakerB.text = viseme;
         }
         else
         {
            Debug.LogWarning("Unknown speaker: " + wrapper);
         }
      }

      #endregion
   }
}
// © 2015-2020 crosstales LLC (https://www.crosstales.com)