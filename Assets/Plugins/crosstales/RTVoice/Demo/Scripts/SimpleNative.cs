using UnityEngine;
using UnityEngine.UI;

namespace Crosstales.RTVoice.Demo
{
   /// <summary>Simple native TTS example.</summary>
   [HelpURL("https://www.crosstales.com/media/data/assets/rtvoice/api/class_crosstales_1_1_r_t_voice_1_1_demo_1_1_simple_native.html")]
   public class SimpleNative : MonoBehaviour
   {
      #region Variables

      [Header("Configuration")] [Range(0f, 3f)] public float RateSpeakerA = 1.25f;
      [Range(0f, 3f)] public float RateSpeakerB = 1.75f;
      [Range(0f, 3f)] public float RateSpeakerC = 2.5f;

      public bool PlayOnStart = false;

      [Header("UI Objects")] public Text TextSpeakerA;
      public Text TextSpeakerB;
      public Text TextSpeakerC;

      public Text PhonemeSpeakerA;
      public Text PhonemeSpeakerB;
      public Text PhonemeSpeakerC;

      public Text VisemeSpeakerA;
      public Text VisemeSpeakerB;
      public Text VisemeSpeakerC;

      private string uidSpeakerA;
      private string uidSpeakerB;
      private string uidSpeakerC;

      private string textA = "Text A";
      private string textB = "Text B";
      private string textC = "Text C";

      private bool silent = true;

      #endregion


      #region MonoBehaviour methods

      public void Start()
      {
         if (TextSpeakerA != null)
            textA = TextSpeakerA.text;

         if (TextSpeakerB != null)
            textB = TextSpeakerB.text;

         if (TextSpeakerC != null)
            textC = TextSpeakerC.text;

         Speaker.isMaryMode = !RTVoice.Util.Helper.hasBuiltInTTS;

         if (PlayOnStart)
         {
            Play();
         }
      }

      public void OnEnable()
      {
         // Subscribe event listeners
         Speaker.OnSpeakCurrentWord += speakCurrentWordMethod;
         Speaker.OnSpeakCurrentPhoneme += speakCurrentPhonemeMethod;
         Speaker.OnSpeakCurrentViseme += speakCurrentVisemeMethod;
         Speaker.OnSpeakStart += speakStartMethod;
         Speaker.OnSpeakComplete += speakCompleteMethod;
      }

      public void OnDisable()
      {
         // Unsubscribe event listeners
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

         if (TextSpeakerC != null)
            TextSpeakerC.text = textC;

         SpeakerA(); //start with speaker A
         //SpeakerB(); //start with speaker B
         //SpeakerC(); //start with speaker C
      }

      public void SpeakerA()
      {
         uidSpeakerA = Speaker.SpeakNative(textA, Speaker.VoiceForGender(Model.Enum.Gender.MALE, "en"), RateSpeakerA);
      }

      public void SpeakerB()
      {
         uidSpeakerB = Speaker.SpeakNative(textB, Speaker.VoiceForGender(Model.Enum.Gender.FEMALE, "en"), RateSpeakerB);
      }

      public void SpeakerC()
      {
         //default voice
         uidSpeakerC = Speaker.SpeakNative(textC, Speaker.VoiceForGender(Model.Enum.Gender.MALE, "en", 1), RateSpeakerC);
      }

      public void Silence()
      {
         silent = true;
         Speaker.Silence();

         if (TextSpeakerA != null)
            TextSpeakerA.text = textA;

         if (TextSpeakerB != null)
            TextSpeakerB.text = textB;

         if (TextSpeakerC != null)
            TextSpeakerC.text = textC;

         VisemeSpeakerC.text = PhonemeSpeakerC.text = VisemeSpeakerB.text = PhonemeSpeakerB.text = VisemeSpeakerA.text = PhonemeSpeakerA.text = "-";
      }

      #endregion


      #region Callback methods

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
         else if (wrapper.Uid.Equals(uidSpeakerC))
         {
            Debug.Log("Speaker C - Speech start: " + wrapper);
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
               SpeakerC();
         }
         else if (wrapper.Uid.Equals(uidSpeakerC))
         {
            Debug.Log("Speaker C - Speech complete: " + wrapper);

            if (TextSpeakerC != null)
               TextSpeakerC.text = wrapper.Text;

            if (VisemeSpeakerC != null)
               VisemeSpeakerC.text = PhonemeSpeakerC.text = "-";

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
         else if (wrapper.Uid.Equals(uidSpeakerC))
         {
            if (TextSpeakerC != null)
               TextSpeakerC.text = RTVoice.Util.Helper.MarkSpokenText(speechTextArray, wordIndex);
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
         else if (wrapper.Uid.Equals(uidSpeakerC))
         {
            if (PhonemeSpeakerC != null)
               PhonemeSpeakerC.text = phoneme;
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
         else if (wrapper.Uid.Equals(uidSpeakerC))
         {
            if (VisemeSpeakerC != null)
               VisemeSpeakerC.text = viseme;
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