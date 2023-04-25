#if UNITY_IOS || UNITY_TVOS || UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Linq;

namespace Crosstales.RTVoice.Provider
{
   /// <summary>iOS voice provider.</summary>
   public class VoiceProviderIOS : BaseVoiceProvider
   {
      #region Variables

      private static System.Collections.Generic.List<Model.Voice> cachediOSVoices =
         new System.Collections.Generic.List<Model.Voice>();

#if !UNITY_EDITOR || CT_DEVELOP
      private static string[] speechTextArray = null;
      private static int wordIndex = 0;
      private static bool isWorking = false;
      private static Model.Wrapper wrapperNative = null;
#endif

      #endregion


      #region Constructor

      /// <summary>
      /// Constructor for VoiceProviderIOS.
      /// </summary>
      /// <param name="obj">Instance of the speaker</param>
      public VoiceProviderIOS(MonoBehaviour obj) : base(obj)
      {
#if !UNITY_EDITOR || CT_DEVELOP
         NativeMethods.GetVoices();
#endif
      }

      #endregion


      #region Wrapper callbacks

#if !UNITY_EDITOR || CT_DEVELOP
      /// <summary>Receives all voices</summary>
      /// <param name="voicesText">All voices as text string.</param>
      public static void SetVoices(string voicesText)
      {
         string[] voices = voicesText.Split(new[] {','}, System.StringSplitOptions.RemoveEmptyEntries);

         if (voices.Length % 3 == 0)
         {
            System.Collections.Generic.List<Model.Voice> voicesList =
               new System.Collections.Generic.List<Model.Voice>(60);

            //for (int ii = 0; ii < voices.Length; ii += 2)
            for (int ii = 0; ii < voices.Length; ii += 3)
            {
               var name = voices[ii + 1];
               var culture = voices[ii + 2];
               var newVoice = new Model.Voice(name, "iOS voice: " + name + " " + culture,
                  Util.Helper.AppleVoiceNameToGender(name), "unknown", culture, voices[ii], "Apple");

               voicesList.Add(newVoice);
            }

            cachediOSVoices = voicesList.OrderBy(s => s.Name).ToList();

            if (Util.Constants.DEV_DEBUG)
               Debug.Log("Voices read: " + cachediOSVoices.CTDump());
         }
         else
         {
            Debug.LogWarning("Voice-string contains wrong number of elements!");
         }

         onVoicesReady();

         //NativeMethods.FreeMemory();
      }

      /// <summary>Receives the state of the speaker.</summary>
      /// <param name="state">The state of the speaker.</param>
      public static void SetState(string state)
      {
         if (state.Equals("Start"))
         {
            // do nothing
         }
         else if (state.Equals("Finsish"))
         {
            isWorking = false;
         }
         else
         {
            //cancel
            isWorking = false;
         }
      }

      /// <summary>Called every time a new word is spoken.</summary>
      public static void WordSpoken()
      {
         if (wrapperNative != null)
         {
            onSpeakCurrentWord(wrapperNative, speechTextArray, wordIndex);
            wordIndex++;
         }
      }
#endif

      #endregion


      #region Implemented methods

      public override string AudioFileExtension
      {
         get { return "none"; }
      }

      public override AudioType AudioFileType
      {
         get { return AudioType.UNKNOWN; }
      }

      public override string DefaultVoiceName
      {
         get { return "Daniel"; }
      }

      public override System.Collections.Generic.List<Model.Voice> Voices
      {
         get { return cachediOSVoices; }
      }

      public override bool isWorkingInEditor
      {
         get { return false; }
      }

      public override bool isWorkingInPlaymode
      {
         get { return false; }
      }

      public override int MaxTextLength
      {
         get { return 256000; }
      }

      public override bool isSpeakNativeSupported
      {
         get { return true; }
      }

      public override bool isSpeakSupported
      {
         get { return false; }
      }

      public override bool isPlatformSupported
      {
         get { return Util.Helper.isIOSBasedPlatform; }
      }

      public override bool isSSMLSupported
      {
         get { return false; }
      }

      public override bool isOnlineService
      {
         get { return false; }
      }

      public override bool hasCoRoutines
      {
         get { return true; }
      }

      public override bool isIL2CPPSupported
      {
         get { return true; }
      }

      public override bool hasVoicesInEditor
      {
         get { return false; }
      }

      public override IEnumerator SpeakNative(Model.Wrapper wrapper)
      {
         yield return speak(wrapper, true);
      }

      public override IEnumerator Speak(Model.Wrapper wrapper)
      {
         yield return speak(wrapper, false);
      }

      public override IEnumerator Generate(Model.Wrapper wrapper)
      {
         Debug.LogError("'Generate' is not supported for iOS!");
         yield return null;
      }

      public override void Silence()
      {
#if !UNITY_EDITOR || CT_DEVELOP
         NativeMethods.Stop();
         //NativeMethods.FreeMemory();
#endif

         base.Silence();
      }

      public override void Silence(string uid)
      {
         Silence();

         base.Silence(uid);

      }

      #endregion


      #region Private methods

      private IEnumerator speak(Model.Wrapper wrapper, bool isNative)
      {
#if !UNITY_EDITOR || CT_DEVELOP
         if (wrapper == null)
         {
            Debug.LogWarning("'wrapper' is null!");
         }
         else
         {
            if (string.IsNullOrEmpty(wrapper.Text))
            {
               Debug.LogWarning("'wrapper.Text' is null or empty!");
            }
            else
            {
               yield return null; //return to the main process (uid)

               string voiceId = getVoiceId(wrapper);

               silence = false;

               if (!isNative)
               {
                  onSpeakAudioGenerationStart(wrapper); //just a fake event if some code needs the feedback...

                  yield return null;

                  onSpeakAudioGenerationComplete(wrapper); //just a fake event if some code needs the feedback...
               }

               onSpeakStart(wrapper);
               isWorking = true;

               speechTextArray = Util.Helper.CleanText(wrapper.Text, false)
                  .Split(splitCharWords, System.StringSplitOptions.RemoveEmptyEntries);
               wordIndex = 0;
               wrapperNative = wrapper;

               NativeMethods.Speak(voiceId, wrapper.Text, calculateRate(wrapper.Rate), wrapper.Pitch,
                  wrapper.Volume);

               do
               {
                  yield return null;
               } while (isWorking && !silence);

               if (Util.Config.DEBUG)
                  Debug.Log("Text spoken: " + wrapper.Text);

               wrapperNative = null;
               onSpeakComplete(wrapper);

               //NativeMethods.FreeMemory();
            }
         }
#else
            yield return null;
#endif
      }

      private static float calculateRate(float rate)
      {
         float result = rate;

         if (rate > 1f)
         {
            //result = (rate + 1f) * 0.5f;
            result = 1f + (rate - 1f) * 0.25f;
         }

         if (Util.Constants.DEV_DEBUG)
            Debug.Log("calculateRate: " + result + " - " + rate);

         return result;
      }

      private string getVoiceId(Model.Wrapper wrapper)
      {
         if (wrapper != null && (wrapper.Voice == null || string.IsNullOrEmpty(wrapper.Voice.Identifier)))
         {
            if (Util.Config.DEBUG)
               Debug.LogWarning(
                  "'wrapper.Voice' or 'wrapper.Voice.Identifier' is null! Using the OS 'default' voice.");

            return Speaker.VoiceForName(DefaultVoiceName).Identifier;
         }

         return wrapper != null ? wrapper.Voice.Identifier : Speaker.VoiceForName(DefaultVoiceName).Identifier;
      }

      #endregion


      #region Editor-only methods

#if UNITY_EDITOR

      public override void GenerateInEditor(Model.Wrapper wrapper)
      {
         Debug.LogError("'GenerateInEditor' is not supported for iOS!");
      }

      public override void SpeakNativeInEditor(Model.Wrapper wrapper)
      {
         Debug.LogError("'SpeakNativeInEditor' is not supported for iOS!");
      }
#endif

      #endregion
   }

   /// <summary>Native methods (bridge to iOS).</summary>
   internal static class NativeMethods
   {
      /*
      /// <summary>Bridge to the native tts system</summary>
      /// <param name="name">Name of the voice to speak.</param>
      /// <param name="text">Text to speak.</param>
      /// <param name="rate">Speech rate of the speaker in percent (default: 1, optional).</param>
      /// <param name="pitch">Pitch of the speech in percent (default: 1, optional).</param>
      /// <param name="volume">Volume of the speaker in percent (default: 1, optional).</param>
      [System.Runtime.InteropServices.DllImport("__Internal")]
      internal static extern Speak(string name, string text, float rate = 1f, float pitch = 1f, float volume = 1f);
      */

      /// <summary>Bridge to the native tts system</summary>
      /// <param name="id">Identifier of the voice to speak.</param>
      /// <param name="text">Text to speak.</param>
      /// <param name="rate">Speech rate of the speaker in percent (default: 1, optional).</param>
      /// <param name="pitch">Pitch of the speech in percent (default: 1, optional).</param>
      /// <param name="volume">Volume of the speaker in percent (default: 1, optional).</param>
      [System.Runtime.InteropServices.DllImport("__Internal")]
      internal static extern void Speak(string id, string text, float rate = 1f, float pitch = 1f, float volume = 1f);

      /// <summary>Silence the current TTS-provider.</summary>
      [System.Runtime.InteropServices.DllImport("__Internal")]
      internal static extern void GetVoices();

      /// <summary>Silence the current TTS-provider.</summary>
      [System.Runtime.InteropServices.DllImport("__Internal")]
      internal static extern void Stop();
   }
}
#endif
// © 2016-2020 crosstales LLC (https://www.crosstales.com)