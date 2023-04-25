#if UNITY_WSA || UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Linq;

namespace Crosstales.RTVoice.Provider
{
   /// <summary>WSA (UWP) voice provider.</summary>
   public class VoiceProviderWSA : BaseVoiceProvider
   {
      #region Variables

#if !UNITY_EDITOR
        private static RTVoiceUWPBridge ttsHandler;
#endif

      #endregion


      #region Constructor

      /// <summary>
      /// Constructor for VoiceProviderWSA.
      /// </summary>
      /// <param name="obj">Instance of the speaker</param>
      public VoiceProviderWSA(MonoBehaviour obj) : base(obj)
      {
         //Util.Config.DEBUG = true; //TODO only for tests

#if !UNITY_EDITOR
            if (ttsHandler == null)
            {
                if (Util.Constants.DEV_DEBUG)
                    Debug.Log("Initializing TTS...");

                ttsHandler = new RTVoiceUWPBridge();

                ttsHandler.DEBUG = Util.Config.DEBUG;
            }

            speakerObj.StartCoroutine(getVoices());
#endif
      }

      #endregion


      #region Implemented methods

      public override string AudioFileExtension
      {
         get { return ".wav"; }
      }

      public override AudioType AudioFileType
      {
         get { return AudioType.WAV; }
      }

      public override string DefaultVoiceName
      {
         get { return "Microsoft David"; }
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
         get { return 64000; }
      }

      public override bool isSpeakNativeSupported
      {
         get { return Speaker.isWSANative; }
      }

      public override bool isSpeakSupported
      {
         get { return true; }
      }

      public override bool isPlatformSupported
      {
         get { return Util.Helper.isWSABasedPlatform; }
      }

      public override bool isSSMLSupported
      {
         get { return true; }
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
         if (Speaker.isWSANative)
         {
            if (wrapper == null)
            {
               Debug.LogWarning("'wrapper' is null!");
            }
            else
            {
               if (string.IsNullOrEmpty(wrapper.Text))
               {
                  Debug.LogWarning("'wrapper.Text' is null or empty: " + wrapper);
               }
               else
               {
                  yield return null; //return to the main process (uid)
#if !UNITY_EDITOR
                        ttsHandler.isBusyNative = true;

                        string voiceName = getVoiceName(wrapper);

                        onSpeakStart(wrapper);
                        silence = false;

                        //ttsHandler.SpeakNative(prepareText(wrapper), voiceName);
                        UnityEngine.WSA.Application.InvokeOnUIThread(() => { ttsHandler.SpeakNative(prepareText(wrapper), voiceName); }, false);
                        //UnityEngine.WSA.Application.InvokeOnAppThread(() => { ttsHandler.SpeakNative(prepareText(wrapper), voiceName); }, false);

                        yield return new WaitForSeconds(0.1f);

                        while (!silence && ttsHandler.isBusyNative)
                        {
                            yield return null;
                        }
#endif
                  onSpeakComplete(wrapper);
               }
            }
         }
         else
         {
            yield return speak(wrapper, true);
         }
      }

      public override IEnumerator Speak(Model.Wrapper wrapper)
      {
         yield return speak(wrapper, false);
      }

      public override IEnumerator Generate(Model.Wrapper wrapper)
      {
         if (wrapper == null)
         {
            Debug.LogWarning("'wrapper' is null!");
         }
         else
         {
            if (string.IsNullOrEmpty(wrapper.Text))
            {
               Debug.LogWarning("'wrapper.Text' is null or empty: " + wrapper);
               yield return null;
            }
            else
            {
               yield return null; //return to the main process (uid)
#if !UNITY_EDITOR
                    ttsHandler.isBusy = true;

                    string voiceName = getVoiceName(wrapper);
                    string outputFile = getOutputFile(wrapper.Uid, true);

                    //ttsHandler.SynthesizeToFile(prepareText(wrapper), Application.persistentDataPath.Replace('/', '\\'), Util.Constants.AUDIOFILE_PREFIX + wrapper.Uid + AudioFileExtension, voiceName);
                    UnityEngine.WSA.Application.InvokeOnAppThread(() => { ttsHandler.SynthesizeToFile(prepareText(wrapper), Application.persistentDataPath.Replace('/', '\\'), Util.Constants.AUDIOFILE_PREFIX + wrapper.Uid + AudioFileExtension, voiceName); }, false);

                    silence = false;

                    onSpeakAudioGenerationStart(wrapper);

                    do
                    {
                        yield return null;
                    } while (!silence && ttsHandler.isBusy);

                    //Debug.Log("FILE: " + "file://" + outputFile + "/" + wrapper.Uid + extension);

                    processAudioFile(wrapper, outputFile);
#endif
            }
         }
      }

      public override void Silence()
      {
#if !UNITY_EDITOR
            UnityEngine.WSA.Application.InvokeOnUIThread(() => { ttsHandler.StopNative(); }, false);
#endif
         base.Silence();
      }

      #endregion


      #region Private methods

      private IEnumerator getVoices()
      {
#if !UNITY_EDITOR
            try
            {
                System.Collections.Generic.List<Model.Voice> voices =
 new System.Collections.Generic.List<Model.Voice>(70);
                string[] myStringVoices = ttsHandler.Voices;
                string name;

                foreach (string voice in myStringVoices)
                {
                    string[] currentVoiceData = voice.Split(';');
                    name = currentVoiceData[0];
                    Model.Voice newVoice =
 new Model.Voice(name, "UWP voice: " + voice, Util.Helper.WSAVoiceNameToGender(name), "unknown", currentVoiceData[1]);
                    voices.Add(newVoice);
                }

                cachedVoices = voices.OrderBy(s => s.Name).ToList();

                if (Util.Constants.DEV_DEBUG)
                    Debug.Log("Voices read: " + cachedVoices.CTDump());
            }
            catch (System.Exception ex)
            {
                string errorMessage = "Could not get any voices!" + System.Environment.NewLine + ex;
                Debug.LogError(errorMessage);
                onErrorInfo(null, errorMessage);
            }
#endif
         yield return null;

         onVoicesReady();
      }

      private IEnumerator speak(Model.Wrapper wrapper, bool isNative)
      {
         if (wrapper == null)
         {
            Debug.LogWarning("'wrapper' is null!");
         }
         else
         {
            if (string.IsNullOrEmpty(wrapper.Text))
            {
               Debug.LogWarning("'wrapper.Text' is null or empty: " + wrapper);
            }
            else
            {
               if (wrapper.Source == null)
               {
                  Debug.LogWarning("'wrapper.Source' is null: " + wrapper);
               }
               else
               {
                  yield return null; //return to the main process (uid)
#if !UNITY_EDITOR
                        ttsHandler.isBusy = true;

                        string voiceName = getVoiceName(wrapper);
                        string outputFile = getOutputFile(wrapper.Uid, true);

                        //ttsHandler.SynthesizeToFile(prepareText(wrapper), Application.persistentDataPath.Replace('/', '\\'), Util.Constants.AUDIOFILE_PREFIX + wrapper.Uid + AudioFileExtension, voiceName);
                        UnityEngine.WSA.Application.InvokeOnAppThread(() => { ttsHandler.SynthesizeToFile(prepareText(wrapper), Application.persistentDataPath.Replace('/', '\\'), Util.Constants.AUDIOFILE_PREFIX + wrapper.Uid + AudioFileExtension, voiceName); }, false);

                        silence = false;

                        if (!isNative)
                        {
                            onSpeakAudioGenerationStart(wrapper);
                        }

                        do
                        {
                            yield return null;
                        } while (!silence && ttsHandler.isBusy);

                        yield return playAudioFile(wrapper, Util.Constants.PREFIX_FILE + outputFile, outputFile, AudioType.WAV, isNative);
#endif
               }
            }
         }
      }

      private static string prepareText(Model.Wrapper wrapper)
      {
         //TEST
         //wrapper.ForceSSML = false;

         if (wrapper.ForceSSML && !Speaker.isAutoClearTags)
         {
            System.Text.StringBuilder sbXML = new System.Text.StringBuilder();

            sbXML.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
            sbXML.Append("<speak version=\"1.0\" xmlns=\"http://www.w3.org/2001/10/synthesis\" xml:lang=\"");
            sbXML.Append(wrapper.Voice == null ? "en-US" : wrapper.Voice.Culture);
            sbXML.Append("\">");

            sbXML.Append("<prosody pitch=\"");

            float _pitch = wrapper.Pitch - 1f;

            sbXML.Append(_pitch >= 0f
               ? _pitch.ToString("+#0%", Util.Helper.BaseCulture)
               : _pitch.ToString("#0%", Util.Helper.BaseCulture));

            sbXML.Append("\">");

            sbXML.Append("<prosody rate=\"");
            sbXML.Append(wrapper.Rate.ToString());
            sbXML.Append("\">");

            sbXML.Append("<prosody volume=\"");

            float _volume = wrapper.Volume - 1f;

            sbXML.Append(_volume >= 0f
               ? _volume.ToString("+#0%", Util.Helper.BaseCulture)
               : _volume.ToString("#0%", Util.Helper.BaseCulture));

            sbXML.Append("\">");

            sbXML.Append(wrapper.Text);

            sbXML.Append("</prosody>");
            sbXML.Append("</prosody>");
            sbXML.Append("</prosody>");

            sbXML.Append("</speak>");

            return getValidXML(sbXML.ToString());
         }

         return wrapper.Text;
      }

      #endregion


      #region Editor-only methods

#if UNITY_EDITOR

      public override void GenerateInEditor(Model.Wrapper wrapper)
      {
         Debug.LogError("'GenerateInEditor' is not supported for UWP (WSA)!");
      }

      public override void SpeakNativeInEditor(Model.Wrapper wrapper)
      {
         Debug.LogError("'SpeakNativeInEditor' is not supported for UWP (WSA)!");
      }

#endif

      #endregion
   }
}
#endif
// © 2016-2020 crosstales LLC (https://www.crosstales.com)