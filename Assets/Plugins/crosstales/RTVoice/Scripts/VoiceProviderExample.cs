using UnityEngine;
using System.Collections;

namespace Crosstales.RTVoice
{
   /// <summary>
   /// Example for a custom voice provider (TTS-system) with all callbacks (only for demonstration - it doesn't do anything).
   /// NOTE: please make sure you understand the Wrapper and its variables
   /// </summary>
   [HelpURL("https://www.crosstales.com/media/data/assets/rtvoice/api/class_crosstales_1_1_r_t_voice_1_1_voice_provider_example.html")]
   public class VoiceProviderExample : Provider.BaseCustomVoiceProvider
   {
      #region Properties

      public override string AudioFileExtension
      {
         get
         {
            // SKELETON CODE - replace with your own code!
            // DEFINE: audio extension of the generated audio files from the provider (e.g. '.wav')
            return ".wav";
         }
      }

      public override AudioType AudioFileType
      {
         get
         {
            // SKELETON CODE - replace with your own code!
            // DEFINE: audio type of the generated audio files from the provider
            return AudioType.WAV;
         }
      }

      public override string DefaultVoiceName
      {
         get
         {
            // SKELETON CODE - replace with your own code!
            // DEFINE: default voice name
            return "Marisa";
         }
      }

      public override bool isWorkingInEditor
      {
         get
         {
            // SKELETON CODE - replace with your own code!
            // DEFINE: Indicates if this provider is working directly inside the Unity Editor (without 'Play'-mode)
            return true;
         }
      }

      public override bool isWorkingInPlaymode
      {
         get
         {
            // SKELETON CODE - replace with your own code!
            // DEFINE: Indicates if this provider is working with 'Play'-mode inside the Unity Editor
            return true;
         }
      }

      public override bool isPlatformSupported
      {
         get
         {
            // SKELETON CODE - replace with your own code!
            // DEFINE: checks if the current platform is supported
            return true;
         }
      }

      public override int MaxTextLength
      {
         get
         {
            // SKELETON CODE - replace with your own code!
            // DEFINE: Maximal length of the speech text (in characters) that could be processed by the provider
            return 32000;
         }
      }

      public override bool isSpeakNativeSupported
      {
         get
         {
            // SKELETON CODE - replace with your own code!
            // DEFINE: Indicates if this provider is supporting SpeakNative
            return false;
         }
      }

      public override bool isSpeakSupported
      {
         get
         {
            // SKELETON CODE - replace with your own code!
            // DEFINE: Indicates if this provider is supporting Speak
            return true;
         }
      }

      public override bool isSSMLSupported
      {
         get
         {
            // SKELETON CODE - replace with your own code!
            // DEFINE: Indicates if this provider is supporting SSML
            return true;
         }
      }

      public override bool isOnlineService
      {
         get
         {
            // SKELETON CODE - replace with your own code!
            // DEFINE: Indicates if this provider is an online service like MaryTTS or AWS Polly
            return false;
         }
      }

      public override bool hasCoRoutines
      {
         get
         {
            // SKELETON CODE - replace with your own code!
            // DEFINE: Indicates if this provider uses co-routines
            return true;
         }
      }

      public override bool isIL2CPPSupported
      {
         get
         {
            // SKELETON CODE - replace with your own code!
            // DEFINE: Indicates if this provider is supporting IL2CPP
            return true;
         }
      }

      public override bool hasVoicesInEditor
      {
         get
         {
            // SKELETON CODE - replace with your own code!
            // DEFINE: Indicates if this provider returns voices in the Editor mode
            return true;
         }
      }

      #endregion


      #region Implemented methods

      public override void Load()
      {
         Debug.Log("Reload");

         // SKELETON CODE - replace with your own code!

         cachedVoices = new System.Collections.Generic.List<Model.Voice>
         {
            new Model.Voice("Marisa", "RTV custom provider test -> female", Model.Enum.Gender.FEMALE, "adult", "en-US"),
            new Model.Voice("Stefan", "RTV custom provider test -> male", Model.Enum.Gender.MALE, "adult", "en-US")
         };

         onVoicesReady();
      }

      public override IEnumerator Generate(Model.Wrapper wrapper)
      {
         Debug.Log("Generate: " + wrapper);

         // SKELETON CODE - replace with your own code!
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
               silence = false;

               // GENERATE: audio file based on the Wrapper configuration (pitch, rate, volume, output file etc.)
               //... <code>
               onSpeakAudioGenerationStart(wrapper); //NECESSARY

               // WAIT: wait until speech is generated
               //... <code>

               //... load file (with WWW) and add the clip to the AudioSource (see VoiceProviderWindows)
               //... <code>

               // SIMULATE GENERATE (remove this)
               foreach (char character in wrapper.Text)
               {
                  Debug.Log(character);

                  yield return null;
               }

               onSpeakAudioGenerationComplete(wrapper); //NECESSARY

               //OPTIONAL: broadcast possible errors
               //onErrorInfo(wrapper, "errorMessage");
            }
         }
      }

      public override IEnumerator Speak(Model.Wrapper wrapper)
      {
         Debug.Log("Speak: " + wrapper);

         // SKELETON CODE - replace with your own code!
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
               silence = false;

               //GENERATE: audio file based on the Wrapper configuration (pitch, rate, volume, output file etc.)
               //... <code>
               onSpeakAudioGenerationStart(wrapper); //NECESSARY

               // WAIT: wait until speech is generated
               //... <code>

               //... load file (with WWW) and add the clip to the AudioSource (see VoiceProviderWindows)
               //... <code>

               onSpeakAudioGenerationComplete(wrapper); //NECESSARY

               //OPTIONAL: play audio file
               //... <code>
               if (wrapper.SpeakImmediately)
               {
                  onSpeakStart(wrapper); //NECESSARY

                  // WAIT: wait until speech is finished
                  //... <code>

                  //SIMULATE SPEECH (remove this)
                  foreach (char character in wrapper.Text)
                  {
                     Debug.Log(character);

                     yield return null;
                  }

                  onSpeakComplete(wrapper); //NECESSARY
               }

               //OPTIONAL: broadcast possible errors
               //onErrorInfo(wrapper, "errorMessage");
            }
         }
      }

      public override IEnumerator SpeakNative(Model.Wrapper wrapper)
      {
         Debug.Log("SpeakNative: " + wrapper);

         // SKELETON CODE - replace with your own code!
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
               silence = false;

               // SPEAK: speech based on the Wrapper configuration (pitch, rate, volume, output file etc.)
               //... <code>

               onSpeakStart(wrapper); //NECESSARY

               // WAIT: wait until speech is finished
               //... <code>

               //SIMULATE SPEECH (remove this)
               foreach (char character in wrapper.Text)
               {
                  //OPTIONAL: enable if you have access to the words, phonemes and visemes
                  //onSpeakCurrentWord(wrapper, speechTextArray, wordIndex - 1);
                  onSpeakCurrentPhoneme(wrapper, character.ToString());
                  onSpeakCurrentViseme(wrapper, character.ToString());

                  Debug.Log(character);

                  yield return null;
               }

               onSpeakComplete(wrapper); //NECESSARY

               //OPTIONAL: broadcast possible errors
               //onErrorInfo(wrapper, "errorMessage");
            }
         }
      }

      #endregion


      #region Editor-only methods

#if UNITY_EDITOR
      public override void GenerateInEditor(Model.Wrapper wrapper)
      {
         Debug.Log("Generate: " + wrapper);

         // SKELETON CODE - replace with your own code!
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
               silence = false;

               // GENERATE: audio file based on the Wrapper configuration (pitch, rate, volume, output file etc.)
               //... <code>
               onSpeakAudioGenerationStart(wrapper); //NECESSARY

               // WAIT: wait until speech is generated
               //... <code>

               //... load file (with WWW) and add the clip to the AudioSource (see VoiceProviderWindows)
               //... <code>

               // SIMULATE GENERATE (remove this)
               foreach (char character in wrapper.Text)
               {
                  Debug.Log(character);

                  System.Threading.Thread.Sleep(100);
               }

               onSpeakAudioGenerationComplete(wrapper); //NECESSARY

               //OPTIONAL: broadcast possible errors
               //onErrorInfo(wrapper, "errorMessage");
            }
         }
      }

      public override void SpeakNativeInEditor(Model.Wrapper wrapper)
      {
         Debug.Log("SpeakNative: " + wrapper);

         // SKELETON CODE - replace with your own code!
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
               silence = false;

               // SPEAK: speech based on the Wrapper configuration (pitch, rate, volume, output file etc.)
               //... <code>

               onSpeakStart(wrapper); //NECESSARY

               // WAIT: wait until speech is finished
               //... <code>

               //SIMULATE SPEECH (remove this)
               foreach (char character in wrapper.Text)
               {
                  //OPTIONAL: enable if you have access to the words, phonemes and visemes
                  //onSpeakCurrentWord(wrapper, speechTextArray, wordIndex - 1);
                  onSpeakCurrentPhoneme(wrapper, character.ToString());
                  onSpeakCurrentViseme(wrapper, character.ToString());

                  Debug.Log(character);

                  System.Threading.Thread.Sleep(100);
               }

               onSpeakComplete(wrapper); //NECESSARY

               //OPTIONAL: broadcast possible errors
               //onErrorInfo(wrapper, "errorMessage");
            }
         }
      }
#endif

      #endregion
   }
}
// © 2018-2020 crosstales LLC (https://www.crosstales.com)