using System;
using UnityEngine;

namespace Crosstales.RTVoice.Tool
{
   /// <summary>Allows to speak and store generated audio.</summary>
   [HelpURL("https://www.crosstales.com/media/data/assets/rtvoice/api/class_crosstales_1_1_r_t_voice_1_1_tool_1_1_speech_text.html")]
   public class SpeechText : MonoBehaviour
   {
      #region Variables

      /// <summary>Text to speak.</summary>
      [Tooltip("Text to speak.")] [TextArea] public string Text = string.Empty;

      /// <summary>Voices for the speech.</summary>
      [Tooltip("Voices for the speech.")] public Model.VoiceAlias Voices;

      /// <summary>Speak mode (default: 'Speak').</summary>
      [Tooltip("Speak mode (default: 'Speak').")] public Model.Enum.SpeakMode Mode = Model.Enum.SpeakMode.Speak;


      /// <summary>AudioSource for the output (optional).</summary>
      [Header("Optional Settings")] [Tooltip("AudioSource for the output (optional).")] public AudioSource Source;

      /// <summary>Speech rate of the speaker in percent (1 = 100%, default: 1, optional).</summary>
      [Tooltip("Speech rate of the speaker in percent (1 = 100%, default: 1, optional).")] [Range(0f, 3f)] public float Rate = 1f;

      /// <summary>Speech pitch of the speaker in percent (1 = 100%, default: 1, optional, mobile only).</summary>
      [Tooltip("Speech pitch of the speaker in percent (1 = 100%, default: 1, optional, mobile only).")] [Range(0f, 2f)]
      public float Pitch = 1f;

      /// <summary>Volume of the speaker in percent (1 = 100%, default: 1, optional, Windows only).</summary>
      [Tooltip("Volume of the speaker in percent (1 = 100%, default: 1, optional, Windows only).")] [Range(0f, 1f)]
      public float Volume = 1f;


      /// <summary>Enable speaking of the text on start (default: false).</summary>
      [Header("Behaviour Settings")] [Tooltip("Enable speaking of the text on start (default: false).")] public bool PlayOnStart = false;

      /// <summary>Delay in seconds until the speech for this text starts (default: 0).</summary>
      [Tooltip("Delay in seconds until the speech for this text starts (default: 0).")] public float Delay = 0f;


      /// <summary>Generate audio file on/off (default: false).</summary>
      [Header("Output File Settings")] [Tooltip("Generate audio file on/off (default: false).")] public bool GenerateAudioFile = false;

      /// <summary>File name (incl. path) for the generated audio.</summary>
      [Tooltip("File name (incl. path) for the generated audio.")] public string FileName = @"_generatedAudio/Speech01";

      /*
      /// <summary>File name of the generated audio.</summary>
      [Tooltip("File name of the generated audio.")]
      public string FileName = "_generatedAudio/Speech01";
      */

      /// <summary>Is the generated file path inside the Assets-folder (current project)? If this option is enabled, it prefixes the path with 'Application.dataPath'.</summary>
      [Tooltip(
         "Is the generated file path inside the Assets-folder (current project)? If this option is enabled, it prefixes the path with 'Application.dataPath'.")]
      public bool FileInsideAssets = true;

      private string uid;

      private bool played = false;

      //private long lastPlaytime = long.MinValue;
      private float lastSpeaktime = float.MinValue;

      #endregion


      #region Events

      private SpeechTextStart _onStart;
      private SpeechTextComplete _onComplete;

      /// <summary>An event triggered whenever a SpeechText 'Speak' is started.</summary>
      public event SpeechTextStart OnSpeechTextStart
      {
         add { _onStart += value; }
         remove { _onStart -= value; }
      }

      /// <summary>An event triggered whenever a SpeechText 'Speak' is completed.</summary>
      public event SpeechTextComplete OnSpeechTextComplete
      {
         add { _onComplete += value; }
         remove { _onComplete -= value; }
      }

      #endregion


      #region Properties

      /// <summary>Text to speak (main use is for UI).</summary>
      public string CurrentText
      {
         get { return Text; }

         set { Text = value; }
      }

      /// <summary>Speech rate of the speaker in percent (main use is for UI).</summary>
      public float CurrentRate
      {
         get { return Rate; }

         set { Rate = value; }
      }

      /// <summary>Speech pitch of the speaker in percent (main use is for UI).</summary>
      public float CurrentPitch
      {
         get { return Pitch; }

         set { Pitch = value; }
      }

      /// <summary>Volume of the speaker in percent (main use is for UI).</summary>
      public float CurrentVolume
      {
         get { return Volume; }

         set { Volume = value; }
      }

      #endregion


      #region MonoBehaviour methods

      public void Start()
      {
         Speaker.OnVoicesReady += onVoicesReady;
         Speaker.OnSpeakStart += onSpeakStart;
         Speaker.OnSpeakComplete += onSpeakComplete;

         play();
      }

      public void OnDestroy()
      {
         Speaker.OnVoicesReady -= onVoicesReady;
         Speaker.OnSpeakStart -= onSpeakStart;
         Speaker.OnSpeakComplete -= onSpeakComplete;
      }

      public void OnValidate()
      {
         if (Delay < 0f)
            Delay = 0f;

         Rate = Mathf.Clamp(Rate, 0f, 3f);
         Pitch = Mathf.Clamp(Pitch, 0f, 2f);
         Volume = Mathf.Clamp01(Volume);

         if (!string.IsNullOrEmpty(FileName))
         {
            FileName = Util.Helper.ValidateFile(FileName);
         }
      }

      #endregion


      #region Public methods

      /// <summary>Speak the text.</summary>
      public void Speak(Action<AudioSource> callBack=null)
      {
         float currentTime = Time.realtimeSinceStartup;

         if (lastSpeaktime + Util.Constants.SPEAK_CALL_SPEED < currentTime)
         {
            lastSpeaktime = currentTime;

            Silence();

            string path = null;

            if (GenerateAudioFile)
            {
               if (!string.IsNullOrEmpty(FileName))
               {
                  path = FileInsideAssets
                     ? Util.Helper.ValidateFile(Application.dataPath + @"/" + FileName)
                     : Util.Helper.ValidateFile(FileName);
                  
                  Debug.Log("AnimeKing path:"+FileName +  Voices.Voice);
               }
               else
               {
                  Debug.LogWarning("'FileName' is null or empty! Can't generate audio file.");
               }
            }

            if (Util.Helper.isEditorMode)
            {
#if UNITY_EDITOR
               Speaker.SpeakNative(Text, Voices.Voice, Rate, Pitch, Volume);
               if (GenerateAudioFile)
               {
                  Speaker.Generate(Text, path, Voices.Voice, Rate, Pitch, Volume);
               }
#endif
            }
            else
            {
               uid = Mode == Model.Enum.SpeakMode.Speak
                  ? Speaker.Speak(Text, Source, Voices.Voice, true, Rate, Pitch, Volume, path,true,callBack)
                  : Speaker.SpeakNative(Text, Voices.Voice, Rate, Pitch, Volume);
            }
         }
         else
         {
            Debug.LogWarning("'Speak' called too fast - please slow down!");
         }
      }

      /// <summary>Silence the speech.</summary>
      public void Silence()
      {
         if (Util.Helper.isEditorMode)
         {
            Speaker.Silence();
         }
         else
         {
            Speaker.Silence(uid);
         }
      }

      #endregion


      #region Private methods

      private void play()
      {
         if (PlayOnStart && !played && Speaker.Voices.Count > 0)
         {
            played = true;

            //Invoke(nameof(Speak), Delay);
            Invoke("Speak", Delay);
         }
      }

      #endregion


      #region Callbacks

      private void onVoicesReady()
      {
         play();
      }

      private void onSpeakStart(Model.Wrapper wrapper)
      {
         onStart();
      }

      private void onSpeakComplete(Model.Wrapper wrapper)
      {
         onComplete();
      }

      #endregion


      #region Event-trigger methods

      private void onStart()
      {
         if (Util.Config.DEBUG)
            Debug.Log("onStart");

         if (_onStart != null) _onStart.Invoke();
      }

      private void onComplete()
      {
         if (Util.Config.DEBUG)
            Debug.Log("onComplete");

         if (_onComplete != null) _onComplete.Invoke();
      }

      #endregion
   }
}
// © 2016-2020 crosstales LLC (https://www.crosstales.com)