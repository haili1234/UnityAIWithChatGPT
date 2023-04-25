using System;
using UnityEngine;
using System.Linq;
using Unity.Plastic.Newtonsoft.Json.Serialization;
using UnityEngine.Serialization;

namespace Crosstales.RTVoice
{
   /// <summary>Main component of RTVoice.</summary>
   [ExecuteInEditMode]
   [DisallowMultipleComponent]
   [HelpURL("https://www.crosstales.com/media/data/assets/rtvoice/api/class_crosstales_1_1_r_t_voice_1_1_speaker.html")]
   public class Speaker : MonoBehaviour
   {
      #region Variables

      /// <summary>Custom provider for RT-Voice.</summary>
      [Header("Custom Provider")] [Tooltip("Custom provider for RT-Voice.")] public Provider.BaseCustomVoiceProvider CustomProvider;

      /// <summary>Enables or disables the custom provider (default: false).</summary>
      [Tooltip("Enable or disable the custom provider (default: false).")] public bool CustomMode = false;


      /// <summary>Enables or disables MaryTTS (default: false).</summary>
      [Header("MaryTTS")] [Tooltip("Enable or disable MaryTTS (default: false).")] public bool MaryTTSMode = false;

      /// <summary>Server URL for MaryTTS.</summary>
      [Tooltip("Server URL for MaryTTS.")] public string MaryTTSUrl = "http://mary.dfki.de";

      /// <summary>Server port for MaryTTS (default: 59125).</summary>
      [Tooltip("Server port for MaryTTS (default: 59125).")] [Range(0, 65535)] public int MaryTTSPort = 59125;

      /// <summary>User name for MaryTTS (default: empty).</summary>
      [Tooltip("User name for MaryTTS (default: empty).")] public string MaryTTSUser = string.Empty;

      /// <summary>User password for MaryTTS (default: empty).</summary>
      [Tooltip("User password for MaryTTS (default: empty).")] public string MaryTTSPassword = string.Empty;

      /// <summary>Input type for MaryTTS (default: MaryTTSType.RAWMARYXML).</summary>
      [Tooltip("Input type for MaryTTS (default: MaryTTSType.RAWMARYXML).")] public Model.Enum.MaryTTSType MaryTTSType = Model.Enum.MaryTTSType.RAWMARYXML;


      /// <summary>Enable or disable eSpeak for standalone platforms (default: false).</summary>
      [Header("eSpeak Settings")] [Tooltip("Enable or disable eSpeak for standalone platforms (default: false).")]
      public bool ESpeakMode = false;

      /// <summary>Active modifier for all eSpeak voices (default: none, m1-m6 = male, f1-f4 = female).</summary>
      [Tooltip("Active modifier for all eSpeak voices (default: none, m1-m6 = male, f1-f4 = female).")] public Model.Enum.ESpeakModifiers ESpeakModifier = Model.Enum.ESpeakModifiers.none;


      /// <summary>Automatically clear tags from speeches depending on the capabilities of the current TTS-system (default: false).</summary>
      [Header("Advanced Settings")] [Tooltip("Automatically clear tags from speeches depending on the capabilities of the current TTS-system (default: false).")]
      public bool AutoClearTags = false;

      /*
      /// <summary>Enable or disable the legacy Windows provider (default: true).</summary>
      [Tooltip("Enable or disable the legacy Windows provider (default: true).")]
      public bool WindowsLegacy = true;
      */

      /// <summary>Enable or disable native speak under WSA. If enabled, the build type must be 'XAML'! (default: false).</summary>
      [Tooltip("Enable or disable native speak under WSA. If enabled, the build type must be 'XAML'! (default: false)")]
      public bool WSANative = false;


      /// <summary>Silence any speeches if this component gets disabled (default: false).</summary>
      [Header("Behaviour Settings")] [Tooltip("Silence any speeches if this component gets disabled (default: false).")]
      public bool SilenceOnDisable = false;

      /// <summary>Silence any speeches if the application loses the focus (default: true).</summary>
      [FormerlySerializedAs("SilenceOnFocustLost")] [Tooltip("Silence any speeches if the application loses the focus. Otherwise the speeches are paused and unpaused (default: false).")]
      public bool SilenceOnFocusLost = false;

      /// <summary>Don't destroy gameobject during scene switches (default: true).</summary>
      [Tooltip("Don't destroy gameobject during scene switches (default: true).")] public bool DontDestroy = true;

      /*
      /// <summary>Files to delete at the application end.</summary>
      public static readonly System.Collections.Generic.List<string> FilesToDelete = new System.Collections.Generic.List<string>();
      */

      private readonly System.Collections.Generic.Dictionary<string, AudioSource> removeSources = new System.Collections.Generic.Dictionary<string, AudioSource>();

      private float cleanUpTimer = 0f;

      private static Provider.IVoiceProvider voiceProvider;
      private static Speaker instance;
      private static GameObject go;
      private static readonly System.Collections.Generic.Dictionary<string, AudioSource> genericSources = new System.Collections.Generic.Dictionary<string, AudioSource>();
      private static readonly System.Collections.Generic.Dictionary<string, AudioSource> providedSources = new System.Collections.Generic.Dictionary<string, AudioSource>();
      private static bool loggedVPIsNull = false;
      private static bool loggedOnlyOneInstance = false;
      private static int speechCount = 0;
      private static int busyCount = 0;
      private static bool deleted = false; //ignore in reset!

      private static readonly char[] splitCharWords = {' '};
      private const float cleanUpTime = 5f; //in seconds

      private static System.Threading.Thread deleteWorker;

      #endregion


      #region Events

      private static VoicesReady _onVoicesReady;

      private static SpeakStart _onSpeakStart;
      private static SpeakComplete _onSpeakComplete;

      private static SpeakCurrentWord _onSpeakCurrentWord;
      private static SpeakCurrentPhoneme _onSpeakCurrentPhoneme;
      private static SpeakCurrentViseme _onSpeakCurrentViseme;

      private static SpeakAudioGenerationStart _onSpeakAudioGenerationStart;
      private static SpeakAudioGenerationComplete _onSpeakAudioGenerationComplete;

      private static ProviderChange _onProviderChange;

      private static ErrorInfo _onErrorInfo;

      /// <summary>An event triggered whenever the voices of a provider are ready.</summary>
      public static event VoicesReady OnVoicesReady
      {
         add { _onVoicesReady += value; }
         remove { _onVoicesReady -= value; }
      }

      /// <summary>An event triggered whenever a speak is started.</summary>
      public static event SpeakStart OnSpeakStart
      {
         add { _onSpeakStart += value; }
         remove { _onSpeakStart -= value; }
      }

      /// <summary>An event triggered whenever a speak is completed.</summary>
      public static event SpeakComplete OnSpeakComplete
      {
         add { _onSpeakComplete += value; }
         remove { _onSpeakComplete -= value; }
      }

      /// <summary>An event triggered whenever a new word is spoken (native, Windows and iOS only).</summary>
      public static event SpeakCurrentWord OnSpeakCurrentWord
      {
         add { _onSpeakCurrentWord += value; }
         remove { _onSpeakCurrentWord -= value; }
      }

      /// <summary>An event triggered whenever a new phoneme is spoken (native, Windows only).</summary>
      public static event SpeakCurrentPhoneme OnSpeakCurrentPhoneme
      {
         add { _onSpeakCurrentPhoneme += value; }
         remove { _onSpeakCurrentPhoneme -= value; }
      }

      /// <summary>An event triggered whenever a new viseme is spoken (native, Windows only).</summary>
      public static event SpeakCurrentViseme OnSpeakCurrentViseme
      {
         add { _onSpeakCurrentViseme += value; }
         remove { _onSpeakCurrentViseme -= value; }
      }

      /// <summary>An event triggered whenever a speak audio generation is started.</summary>
      public static event SpeakAudioGenerationStart OnSpeakAudioGenerationStart
      {
         add { _onSpeakAudioGenerationStart += value; }
         remove { _onSpeakAudioGenerationStart -= value; }
      }

      /// <summary>An event triggered whenever a speak audio generation is completed.</summary>
      public static event SpeakAudioGenerationComplete OnSpeakAudioGenerationComplete
      {
         add { _onSpeakAudioGenerationComplete += value; }
         remove { _onSpeakAudioGenerationComplete -= value; }
      }

      /// <summary>An event triggered whenever a provider changes (e.g. Windows to MaryTTS).</summary>
      public static event ProviderChange OnProviderChange
      {
         add { _onProviderChange += value; }
         remove { _onProviderChange -= value; }
      }

      /// <summary>An event triggered whenever an error occurs.</summary>
      public static event ErrorInfo OnErrorInfo
      {
         add { _onErrorInfo += value; }
         remove { _onErrorInfo -= value; }
      }

      #endregion


      #region Static properties

      /// <summary>Number of active speeches.</summary>
      public static int SpeechCount
      {
         get { return speechCount; }

         private set { speechCount = value < 0 ? 0 : value; }
      }

      /// <summary>Number of active calls.</summary>
      public static int BusyCount
      {
         get { return busyCount; }

         private set { busyCount = value < 0 ? 0 : value; }
      }

      /// <summary>Are all voices ready to speak?</summary>
      public static bool areVoicesReady { get; private set; }

      /// <summary>Enables or disables MaryTTS.</summary>
      public static Provider.BaseCustomVoiceProvider CustomVoiceProvider
      {
         get { return instance != null ? instance.CustomProvider : null; }

         set
         {
            if (instance == null || instance.CustomProvider == value) return;
            instance.CustomProvider = value;

            ReloadProvider();
         }
      }

      /// <summary>Enables or disables the custom voice provider.</summary>
      public static bool isCustomMode
      {
         get { return instance != null && instance.CustomMode; }

         set
         {
            if (instance == null || instance.CustomMode == value) return;
            instance.CustomMode = value;

            ReloadProvider();
         }
      }

      /// <summary>Enables or disables MaryTTS.</summary>
      public static bool isMaryMode
      {
         get { return instance != null && instance.MaryTTSMode; }

         set
         {
            if (instance == null || instance.MaryTTSMode == value) return;
            instance.MaryTTSMode = value;

            ReloadProvider();
         }
      }

      /// <summary>Server URL for MaryTTS.</summary>
      public static string MaryUrl
      {
         get { return instance != null ? instance.MaryTTSUrl : "http://mary.dfki.de"; }

         set
         {
            if (instance == null) return;
            instance.MaryTTSUrl = value;

            ReloadProvider(); //TODO disable?
         }
      }

      /// <summary>Server port for MaryTTS.</summary>
      public static int MaryPort
      {
         get { return instance != null ? instance.MaryTTSPort : 59125; }

         set
         {
            if (instance == null) return;
            instance.MaryTTSPort = value;

            ReloadProvider(); //TODO disable?
         }
      }

      /// <summary>User name for MaryTTS.</summary>
      public static string MaryUser
      {
         get { return instance != null ? instance.MaryTTSUser : string.Empty; }

         set
         {
            if (instance == null) return;
            instance.MaryTTSUser = value;

            ReloadProvider(); //TODO disable?
         }
      }

      /// <summary>Password for MaryTTS.</summary>
      public static string MaryPassword
      {
         private get { return instance != null ? instance.MaryTTSPassword : string.Empty; }

         set
         {
            if (instance == null) return;
            instance.MaryTTSPassword = value;

            ReloadProvider(); //TODO disable?
         }
      }

      /// <summary>Input type for MaryTTS.</summary>
      public static Model.Enum.MaryTTSType MaryType
      {
         get { return instance != null ? instance.MaryTTSType : Model.Enum.MaryTTSType.RAWMARYXML; }

         set
         {
            if (instance != null)
            {
               instance.MaryTTSType = value;

               //ReloadProvider();
            }
         }
      }

      /// <summary>Enable or disable eSpeak for standalone platforms.</summary>
      public static bool isESpeakMode
      {
         get { return instance != null && instance.ESpeakMode; }

         set
         {
            if (instance == null || instance.ESpeakMode == value) return;
            instance.ESpeakMode = value;

            ReloadProvider();
         }
      }

      /// <summary>Active modifier for all eSpeak voices (m1-m6 = male, f1-f4 = female).</summary>
      public static Model.Enum.ESpeakModifiers ESpeakMod
      {
         get { return instance != null ? instance.ESpeakModifier : Model.Enum.ESpeakModifiers.none; }

         set
         {
            if (instance != null)
               instance.ESpeakModifier = value;
         }
      }

      /*
      /// <summary>Enable or disable the legacy Windows provider.</summary>
      public static bool isWindowsLegacy
      {
          get
          {
              if (instance != null)
                  return instance.WindowsLegacy;

              return false;
          }

          set
          {
              if (instance != null)
                  instance.WindowsLegacy = value;
          }
      }
      */

      /// <summary>Enable or disable native speak under WSA. If enabled, the build type must be 'XAML'!</summary>
      public static bool isWSANative
      {
         get { return instance != null && instance.WSANative; }

         set
         {
            if (instance != null)
               instance.WSANative = value;
         }
      }

      /// <summary>Automatically clear tags from speeches depending on the capabilities of the current TTS-system.</summary>
      public static bool isAutoClearTags
      {
         get { return instance != null && instance.AutoClearTags; }

         set
         {
            if (instance != null)
               instance.AutoClearTags = value;
         }
      }

      /// <summary>Silence any speeches if this component gets disabled.</summary>
      public static bool isSilenceOnDisable
      {
         get { return instance != null && instance.SilenceOnDisable; }

         set
         {
            if (instance != null)
               instance.SilenceOnDisable = value;
         }
      }

      /// <summary>Silence any speeches if the application loses the focus.</summary>
      public static bool isSilenceOnFocusLost
      {
         get { return instance == null || instance.SilenceOnFocusLost; }

         set
         {
            if (instance != null)
               instance.SilenceOnFocusLost = value;
         }
      }

      /// <summary>Checks if TTS is available on this system.</summary>
      /// <returns>True if TTS is available on this system.</returns>
      public static bool isTTSAvailable
      {
         get
         {
            if (voiceProvider != null)
            {
               return voiceProvider.Voices.Count > 0;
            }

            logVPIsNull();

            return false;
         }
      }

      /// <summary>Checks if RT-Voice is speaking on this system.</summary>
      /// <returns>True if RT-Voice is speaking on this system.</returns>
      public static bool isSpeaking
      {
         get { return SpeechCount > 0; }
      }

      /// <summary>Checks if RT-Voice is busy on this system.</summary>
      /// <returns>True if RT-Voice is busy on this system.</returns>
      public static bool isBusy
      {
         get { return BusyCount > 0; }
      }


      /// <summary>Is standalone TTS enforced?</summary>
      public static bool enforcedStandaloneTTS { get; private set; }

      /// <summary>Is RT-Voice paused?</summary>
      public static bool isPaused { get; private set; }

      /// <summary>Is RT-Voice muted?</summary>
      public static bool isMuted { get; private set; }

      #region Provider delegates

      /// <summary>Returns the extension of the generated audio files.</summary>
      /// <returns>Extension of the generated audio files.</returns>
      public static string AudioFileExtension
      {
         get
         {
            if (voiceProvider != null)
            {
               return voiceProvider.AudioFileExtension;
            }

            logVPIsNull();

            return ".wav"; //best guess
         }
      }

      /// <summary>Returns the default voice name of the current TTS-provider.</summary>
      /// <returns>Default voice name of the current TTS-provider.</returns>
      public static string DefaultVoiceName
      {
         get
         {
            if (voiceProvider != null)
            {
               return voiceProvider.DefaultVoiceName;
            }

            logVPIsNull();

            return string.Empty;
         }
      }

      /// <summary>Get all available voices from the current TTS-system.</summary>
      /// <returns>All available voices (alphabetically ordered by 'Name') as a list.</returns>
      public static System.Collections.Generic.List<Model.Voice> Voices
      {
         get
         {
            if (voiceProvider != null)
            {
               return voiceProvider.Voices;
            }

            logVPIsNull();

            return new System.Collections.Generic.List<Model.Voice>();
         }
      }

      /// <summary>Indicates if this TTS-system is working directly inside the Unity Editor (without 'Play'-mode).</summary>
      /// <returns>True if this TTS-system is working directly inside the Unity Editor.</returns>
      public static bool isWorkingInEditor
      {
         get
         {
            if (voiceProvider != null)
            {
               return voiceProvider.isWorkingInEditor;
            }

            logVPIsNull();

            return false;
         }
      }

      /// <summary>Indicates if this TTS-system is working with 'Play'-mode inside the Unity Editor.</summary>
      /// <returns>True if this TTS-system is working with 'Play'-mode inside the Unity Editor.</returns>
      public static bool isWorkingInPlaymode
      {
         get
         {
            if (voiceProvider != null)
            {
               return voiceProvider.isWorkingInPlaymode;
            }

            logVPIsNull();

            return false;
         }
      }

      /// <summary>Maximal length of the speech text (in characters) for the current TTS-system.</summary>
      /// <returns>The maximal length of the speech text.</returns>
      public static int MaxTextLength
      {
         get
         {
            if (voiceProvider != null)
            {
               return voiceProvider.MaxTextLength;
            }

            logVPIsNull();

            return 3999; //minimum (Android)
         }
      }

      /// <summary>Indicates if this TTS-system is supporting SpeakNative.</summary>
      /// <returns>True if this TTS-system supports SpeakNative.</returns>
      public static bool isSpeakNativeSupported
      {
         get
         {
            if (voiceProvider != null)
            {
               return voiceProvider.isSpeakNativeSupported;
            }

            logVPIsNull();

            return false;
         }
      }

      /// <summary>Indicates if this TTS-system is supporting Speak.</summary>
      /// <returns>True if this TTS-system supports Speak.</returns>
      public static bool isSpeakSupported
      {
         get
         {
            if (voiceProvider != null)
            {
               return voiceProvider.isSpeakSupported;
            }

            logVPIsNull();

            return false;
         }
      }

      /// <summary>Indicates if this TTS-system is supporting the current platform.</summary>
      /// <returns>True if this TTS-system supports current platform.</returns>
      public static bool isPlatformSupported
      {
         get
         {
            return voiceProvider != null && voiceProvider.isPlatformSupported;
            /*
            else
            {
                logVPIsNull();
            }
            */
         }
      }

      /// <summary>Indicates if this TTS-system is supporting SSML.</summary>
      /// <returns>True if this TTS-system supports SSML.</returns>
      public static bool isSSMLSupported
      {
         get
         {
            if (voiceProvider != null)
            {
               return voiceProvider.isSSMLSupported;
            }

            logVPIsNull();

            return false;
         }
      }

      /// <summary>Indicates if this TTS-system is an online service like MaryTTS or AWS Polly.</summary>
      /// <returns>True if this TTS-system is an online service.</returns>
      public static bool isOnlineService
      {
         get
         {
            if (voiceProvider != null)
            {
               return voiceProvider.isOnlineService;
            }

            logVPIsNull();

            return false;
         }
      }

      /// <summary>Indicates if this TTS-system uses co-routines.</summary>
      /// <returns>True if this TTS-system uses co-routines.</returns>
      public static bool hasCoRoutines
      {
         get
         {
            if (voiceProvider != null)
            {
               return voiceProvider.hasCoRoutines;
            }

            logVPIsNull();

            return true;
         }
      }

      /// <summary>Indicates if this TTS-system is supporting IL2CPP.</summary>
      /// <returns>True if this TTS-system supports IL2CPP.</returns>
      public static bool isIL2CPPSupported
      {
         get
         {
            if (voiceProvider != null)
            {
               return voiceProvider.isIL2CPPSupported;
            }

            logVPIsNull();

            return true;
         }
      }

      /// <summary>Indicates if this provider returns voices in the Editor mode.</summary>
      /// <returns>True if this provider returns voices in the Editor mode.</returns>
      public static bool hasVoicesInEditor
      {
         get
         {
            if (voiceProvider != null)
            {
               return voiceProvider.hasVoicesInEditor;
            }

            logVPIsNull();

            return false;
         }
      }

      /// <summary>Get all available cultures from the current TTS-system (ISO 639-1).</summary>
      /// <returns>All available cultures (alphabetically ordered by 'Culture') as a list.</returns>
      public static System.Collections.Generic.List<string> Cultures
      {
         get
         {
            if (voiceProvider != null)
            {
               return voiceProvider.Cultures;
            }

            logVPIsNull();

            return new System.Collections.Generic.List<string>();
         }
      }

      #endregion

      #endregion


      #region MonoBehaviour methods

      public void OnEnable()
      {
         if (instance == null)
         {
            instance = this;
            go = gameObject;
            go.name = Util.Constants.RTVOICE_SCENE_OBJECT_NAME;

            if (!deleted)
            {
               deleted = true;

               if (Util.Helper.isWindowsPlatform && Util.Config.AUDIOFILE_AUTOMATIC_DELETE) //only delete files under Windows
                  DeleteAudioFiles();
            }

            // Subscribe event listeners
            Provider.BaseVoiceProvider.OnVoicesReady += onVoicesReady;
            Provider.BaseVoiceProvider.OnSpeakStart += onSpeakStart;
            Provider.BaseVoiceProvider.OnSpeakComplete += onSpeakComplete;
            Provider.BaseVoiceProvider.OnSpeakCurrentWord += onSpeakCurrentWord;
            Provider.BaseVoiceProvider.OnSpeakCurrentPhoneme += onSpeakCurrentPhoneme;
            Provider.BaseVoiceProvider.OnSpeakCurrentViseme += onSpeakCurrentViseme;
            Provider.BaseVoiceProvider.OnSpeakAudioGenerationStart += onSpeakAudioGenerationStart;
            Provider.BaseVoiceProvider.OnSpeakAudioGenerationComplete += onSpeakAudioGenerationComplete;
            Provider.BaseVoiceProvider.OnErrorInfo += onErrorInfo;


            //initProvider();

            if (!Util.Helper.isEditorMode && DontDestroy)
               DontDestroyOnLoad(transform.root.gameObject);

            if (!Util.Helper.hasBuiltInTTS)
               MaryTTSMode = true;

            if (Util.Helper.isLinuxPlatform && !isMaryMode)
               ESpeakMode = true;

            /*
            if (isESpeakMode && !Util.Helper.isStandalonePlatform)
                ESpeakMode = false;
                */
            initProvider();

            if (Util.Config.DEBUG)
               Debug.LogWarning("Using new instance!");
         }
         else
         {
            if (!Util.Helper.isEditorMode && DontDestroy && instance != this)
            {
               if (!loggedOnlyOneInstance)
               {
                  Debug.LogWarning("Only one active instance of 'RTVoice' allowed in all scenes!" + System.Environment.NewLine + "This object will now be destroyed.");

                  loggedOnlyOneInstance = true;
               }

               Destroy(gameObject, 0.2f);
            }

            if (Util.Config.DEBUG)
               Debug.LogWarning("Using old instance!");
         }
      }

      public void Update()
      {
         cleanUpTimer += Time.deltaTime;

         if (cleanUpTimer > cleanUpTime)
         {
            cleanUpTimer = 0f;

            if (genericSources.Count > 0)
            {
               foreach (var source in genericSources.Where(source => source.Value != null && source.Value.clip != null && !Common.Util.BaseHelper.hasActiveClip(source.Value)))
               {
                  
                  removeSources.Add(source.Key, source.Value);
               }

               foreach (System.Collections.Generic.KeyValuePair<string, AudioSource> source in removeSources)
               {
                  genericSources.Remove(source.Key);
                  Destroy(source.Value);
               }

               removeSources.Clear();
            }

            if (providedSources.Count > 0)
            {
               foreach (var source in providedSources.Where(source => source.Value != null && source.Value.clip != null && !Common.Util.BaseHelper.hasActiveClip(source.Value)))
               {
                  source.Value.clip = null; //remove clip

                  removeSources.Add(source.Key, source.Value);
               }

               foreach (System.Collections.Generic.KeyValuePair<string, AudioSource> source in removeSources)
               {
                  //genericSources.Remove(source.Key);
                  providedSources.Remove(source.Key);
               }

               removeSources.Clear();
            }
         }

         if (Common.Util.BaseHelper.isEditorMode && go != null && Util.Config.ENSURE_NAME)
            go.name = Util.Constants.RTVOICE_SCENE_OBJECT_NAME; //ensure name
      }

      public void OnDisable()
      {
         if (SilenceOnDisable)
            Silence();

         if (instance == this)
         {
            // Unsubscribe event listeners
            Provider.BaseVoiceProvider.OnVoicesReady -= onVoicesReady;
            Provider.BaseVoiceProvider.OnSpeakStart -= onSpeakStart;
            Provider.BaseVoiceProvider.OnSpeakComplete -= onSpeakComplete;
            Provider.BaseVoiceProvider.OnSpeakCurrentWord -= onSpeakCurrentWord;
            Provider.BaseVoiceProvider.OnSpeakCurrentPhoneme -= onSpeakCurrentPhoneme;
            Provider.BaseVoiceProvider.OnSpeakCurrentViseme -= onSpeakCurrentViseme;
            Provider.BaseVoiceProvider.OnSpeakAudioGenerationStart -= onSpeakAudioGenerationStart;
            Provider.BaseVoiceProvider.OnSpeakAudioGenerationComplete -= onSpeakAudioGenerationComplete;
            Provider.BaseVoiceProvider.OnErrorInfo -= onErrorInfo;

            unsubscribeCustomEvents();
         }
      }

      public void OnApplicationQuit()
      {
         Silence();

#if UNITY_ANDROID || UNITY_EDITOR
         if (voiceProvider != null && voiceProvider.GetType() == typeof(Provider.VoiceProviderAndroid))
         {
            Provider.VoiceProviderAndroid.ShutdownTTS();
         }
#endif

         /*
         if (!Util.Helper.isEditorMode)
         {
             foreach (string outputFile in FilesToDelete)
             {
                 if (System.IO.File.Exists(outputFile))
                 {
                     try
                     {
                         System.IO.File.Delete(outputFile);
                     }
                     catch (System.Exception ex)
                     {
                         string errorMessage = "Could not delete file '" + outputFile + "'!" + System.Environment.NewLine + ex;
                         Debug.LogError(errorMessage);
                     }
                 }
             }
         }
         */

#if !UNITY_WSA || UNITY_EDITOR
         if (deleteWorker != null && deleteWorker.IsAlive)
         {
            if (Util.Constants.DEV_DEBUG)
               Debug.Log("Kill worker");

            deleteWorker.Abort(); //TODO dangerous - find a better solution!
         }
#endif
      }

      public void OnApplicationFocus(bool hasFocus)
      {
         if (!Application.runInBackground)
         {
            if (SilenceOnFocusLost)
            {
               if (!hasFocus)
                  Silence();
            }
            else
            {
               if (hasFocus)
               {
                  UnPause();
               }
               else
               {
                  Pause();
               }
            }
         }
      }

      #endregion


      #region Static methods

      /// <summary>Resets this object.</summary>
      public static void Reset()
      {
         voiceProvider = null;
         instance = null;
         genericSources.Clear();
         providedSources.Clear();
         go = null;
         loggedVPIsNull = false;
         loggedOnlyOneInstance = false;
         speechCount = 0;
      }

      /// <summary>
      /// Approximates the speech length in seconds of a given text and rate.
      /// Note: This is an experimental method and doesn't provide an exact value; +/- 15% is "normal"!
      /// </summary>
      /// <param name="text">Text for the length approximation.</param>
      /// <param name="rate">Speech rate of the speaker in percent for the length approximation (1 = 100%, default: 1, optional).</param>
      /// <param name="wordsPerMinute">Words per minute (default: 175, optional).</param>
      /// <param name="timeFactor">Time factor for the calculated value (default: 0.9, optional).</param>
      /// <returns>Approximated speech length in seconds of the given text and rate.</returns>
      public static float ApproximateSpeechLength(string text, float rate = 1f, float wordsPerMinute = 175f, float timeFactor = 0.9f)
      {
         float words = text.Split(splitCharWords, System.StringSplitOptions.RemoveEmptyEntries).Length;
         float characters = text.Length - words + 1;
         float ratio = characters / words;

         if (Common.Util.BaseHelper.isWindowsPlatform && !isMaryMode && !isESpeakMode && !isCustomMode)
         {
            if (Mathf.Abs(rate - 1f) > Common.Util.BaseConstants.FLOAT_TOLERANCE)
            {
               //relevant?
               if (rate > 1f)
               {
                  //larger than 1
                  if (rate >= 2.75f)
                  {
                     rate = 2.78f;
                  }
                  else if (rate >= 2.6f && rate < 2.75f)
                  {
                     rate = 2.6f;
                  }
                  else if (rate >= 2.35f && rate < 2.6f)
                  {
                     rate = 2.39f;
                  }
                  else if (rate >= 2.2f && rate < 2.35f)
                  {
                     rate = 2.2f;
                  }
                  else if (rate >= 2f && rate < 2.2f)
                  {
                     rate = 2f;
                  }
                  else if (rate >= 1.8f && rate < 2f)
                  {
                     rate = 1.8f;
                  }
                  else if (rate >= 1.6f && rate < 1.8f)
                  {
                     rate = 1.6f;
                  }
                  else if (rate >= 1.4f && rate < 1.6f)
                  {
                     rate = 1.45f;
                  }
                  else if (rate >= 1.2f && rate < 1.4f)
                  {
                     rate = 1.28f;
                  }
                  else if (rate > 1f && rate < 1.2f)
                  {
                     rate = 1.14f;
                  }
               }
               else
               {
                  //smaller than 1
                  if (rate <= 0.3f)
                  {
                     rate = 0.33f;
                  }
                  else if (rate > 0.3 && rate <= 0.4f)
                  {
                     rate = 0.375f;
                  }
                  else if (rate > 0.4 && rate <= 0.45f)
                  {
                     rate = 0.42f;
                  }
                  else if (rate > 0.45 && rate <= 0.5f)
                  {
                     rate = 0.47f;
                  }
                  else if (rate > 0.5 && rate <= 0.55f)
                  {
                     rate = 0.525f;
                  }
                  else if (rate > 0.55 && rate <= 0.6f)
                  {
                     rate = 0.585f;
                  }
                  else if (rate > 0.6 && rate <= 0.7f)
                  {
                     rate = 0.655f;
                  }
                  else if (rate > 0.7 && rate <= 0.8f)
                  {
                     rate = 0.732f;
                  }
                  else if (rate > 0.8 && rate <= 0.9f)
                  {
                     rate = 0.82f;
                  }
                  else if (rate > 0.9 && rate < 1f)
                  {
                     rate = 0.92f;
                  }
               }
            }
         }

         float speechLength = words / (wordsPerMinute / 60 * rate);

         if (ratio < 2)
         {
            speechLength *= 1f;
         }
         else if (ratio >= 2f && ratio < 3f)
         {
            speechLength *= 1.05f;
         }
         else if (ratio >= 3f && ratio < 3.5f)
         {
            speechLength *= 1.15f;
         }
         else if (ratio >= 3.5f && ratio < 4f)
         {
            speechLength *= 1.2f;
         }
         else if (ratio >= 4f && ratio < 4.5f)
         {
            speechLength *= 1.25f;
         }
         else if (ratio >= 4.5f && ratio < 5f)
         {
            speechLength *= 1.3f;
         }
         else if (ratio >= 5f && ratio < 5.5f)
         {
            speechLength *= 1.4f;
         }
         else if (ratio >= 5.5f && ratio < 6f)
         {
            speechLength *= 1.45f;
         }
         else if (ratio >= 6f && ratio < 6.5f)
         {
            speechLength *= 1.5f;
         }
         else if (ratio >= 6.5f && ratio < 7f)
         {
            speechLength *= 1.6f;
         }
         else if (ratio >= 7f && ratio < 8f)
         {
            speechLength *= 1.7f;
         }
         else if (ratio >= 8f && ratio < 9f)
         {
            speechLength *= 1.8f;
         }
         else
         {
            speechLength *= ratio * (ratio / 100f + 0.02f) + 1f;
         }

         if (speechLength < 0.8f)
            speechLength += 0.6f;

         return speechLength * timeFactor;
      }

      /// <summary>Is a voice available for a given gender and optional culture from the current TTS-system?</summary>
      /// <param name="gender">Gender of the voice</param>
      /// <param name="culture">Culture of the voice (e.g. "en", optional)</param>
      /// <returns>True if a voice is available for a given gender and culture.</returns>
      public static bool isVoiceForGenderAvailable(Model.Enum.Gender gender, string culture = "")
      {
         return VoicesForGender(gender, culture, false).Count > 0;
      }

      /// <summary>Get all available voices for a given gender and optional culture from the current TTS-system.</summary>
      /// <param name="gender">Gender of the voice</param>
      /// <param name="culture">Culture of the voice (e.g. "en", optional)</param>
      /// <param name="isFuzzy">Always returns voices if there is no match with the gender and/or culture (default: false, optional)</param>
      /// <returns>All available voices (alphabetically ordered by 'Name') for a given gender and culture as a list.</returns>
      public static System.Collections.Generic.List<Model.Voice> VoicesForGender(Model.Enum.Gender gender, string culture = "", bool isFuzzy = false)
      {
         System.Collections.Generic.List<Model.Voice> voices = new System.Collections.Generic.List<Model.Voice>(Voices.Count);

         if (string.IsNullOrEmpty(culture))
         {
            if (Model.Enum.Gender.UNKNOWN == gender)
            {
               return Voices;
            }

            voices.AddRange(Voices.Where(voice => voice.Gender == gender));
         }
         else
         {
            if (Model.Enum.Gender.UNKNOWN == gender)
            {
               return VoicesForCulture(culture, isFuzzy);
            }

            voices.AddRange(VoicesForCulture(culture, isFuzzy).Where(voice => voice.Gender == gender));
         }

         return voices;
      }

      /// <summary>Get a voice from for a given gender and optional culture and optional index from the current TTS-system.</summary>
      /// <param name="gender">Gender of the voice</param>
      /// <param name="culture">Culture of the voice (e.g. "en", optional)</param>
      /// <param name="index">Index of the voice (default: 0, optional)</param>
      /// <param name="fallbackCulture">Fallback culture of the voice (e.g. "en", default "", optional)</param>
      /// <param name="isFuzzy">Always returns voices if there is no match with the gender and/or culture (default: false, optional)</param>
      /// <returns>Voice for the given culture and index.</returns>
      public static Model.Voice VoiceForGender(Model.Enum.Gender gender, string culture = "", int index = 0, string fallbackCulture = "", bool isFuzzy = false)
      {
         Model.Voice result = null;

         System.Collections.Generic.List<Model.Voice> voices = VoicesForGender(gender, culture, isFuzzy);

         if (voices.Count > 0)
         {
            if (voices.Count - 1 >= index && index >= 0)
            {
               result = voices[index];
            }
            else
            {
               //use the default voice
               //result = voices[0];
               Debug.LogWarning("No voices for gender '" + gender + "' and culture '" + culture + "' with index '" + index + "' found! Speaking with the default voice!");
            }
         }
         else
         {
            voices = VoicesForGender(gender, fallbackCulture, isFuzzy);

            if (voices.Count > 0)
            {
               result = voices[0];
               Debug.LogWarning("No voices for gender '" + gender + "' and culture '" + culture + "' found! Speaking with the fallback culture: '" + fallbackCulture + "'");
            }
            else
            {
               //use the default voice
               Debug.LogWarning("No voice for gender '" + gender + "' and culture '" + culture + "' found! Speaking with the default voice!");
            }
         }

         return result;
      }

      /// <summary>Is a voice available for a given culture from the current TTS-system?</summary>
      /// <param name="culture">Culture of the voice (e.g. "en")</param>
      /// <returns>True if a voice is available for a given culture.</returns>
      public static bool isVoiceForCultureAvailable(string culture)
      {
         return VoicesForCulture(culture, false).Count > 0;
      }

      /// <summary>Get all available voices for a given culture from the current TTS-system.</summary>
      /// <param name="culture">Culture of the voice (e.g. "en")</param>
      /// <param name="isFuzzy">Always returns voices if there is no match with the culture (default: false, optional)</param>
      /// <returns>All available voices (alphabetically ordered by 'Name') for a given culture as a list.</returns>
      public static System.Collections.Generic.List<Model.Voice> VoicesForCulture(string culture, bool isFuzzy = false)
      {
         if (string.IsNullOrEmpty(culture))
         {
            if (Util.Config.DEBUG)
               Debug.LogWarning("The given 'culture' is null or empty! Returning all available voices.");

            return Voices;
         }

         string _culture = culture.Trim().Replace(" ", string.Empty).Replace("_", string.Empty).Replace("-", string.Empty);
#if UNITY_WSA
         System.Collections.Generic.List<Model.Voice> voices = Voices.Where(s => s.SimplifiedCulture.StartsWith(_culture, System.StringComparison.OrdinalIgnoreCase)).OrderBy(s => s.Name).ToList();
#else
         System.Collections.Generic.List<Model.Voice> voices = Voices.Where(s => s.SimplifiedCulture.StartsWith(_culture, System.StringComparison.InvariantCultureIgnoreCase)).OrderBy(s => s.Name).ToList();
#endif
         if (voices.Count == 0 && isFuzzy)
         {
            return Voices;
         }

         return voices;
      }

      /// <summary>Get a voice from for a given culture and optional index from the current TTS-system.</summary>
      /// <param name="culture">Culture of the voice (e.g. "en")</param>
      /// <param name="index">Index of the voice (default: 0, optional)</param>
      /// <param name="fallbackCulture">Fallback culture of the voice (e.g. "en", default "", optional)</param>
      /// <param name="isFuzzy">Always returns voices if there is no match with the culture (default: false, optional)</param>
      /// <returns>Voice for the given culture and index.</returns>
      public static Model.Voice VoiceForCulture(string culture, int index = 0, string fallbackCulture = "", bool isFuzzy = false)
      {
         Model.Voice result = null;

         if (!string.IsNullOrEmpty(culture))
         {
            System.Collections.Generic.List<Model.Voice> voices = VoicesForCulture(culture, isFuzzy);

            if (voices.Count > 0)
            {
               if (voices.Count - 1 >= index && index >= 0)
               {
                  result = voices[index];
               }
               else
               {
                  //use the default voice
                  //result = voices[0];
                  Debug.LogWarning("No voices for culture '" + culture + "' with index '" + index + "' found! Speaking with the default voice!");
               }
            }
            else
            {
               voices = VoicesForCulture(fallbackCulture, isFuzzy);

               if (voices.Count > 0)
               {
                  result = voices[0];
                  Debug.LogWarning("No voices for culture '" + culture + "' found! Speaking with the fallback culture: '" + fallbackCulture + "'");
               }
               else
               {
                  //use the default voice
                  Debug.LogWarning("No voice for culture '" + culture + "' found! Speaking with the default voice!");
               }
            }
         }

         return result;
      }

      /// <summary>Is a voice available for a given name from the current TTS-system?</summary>
      /// <param name="name">Name of the voice (e.g. "Alex")</param>
      /// <param name="isExact">Exact match for the voice name (default: false, optional)</param>
      /// <returns>True if a voice is available for a given name.</returns>
      public static bool isVoiceForNameAvailable(string name, bool isExact = false)
      {
         return VoiceForName(name, isExact) != null;
      }

      /// <summary>Get a voice for a given name from the current TTS-system.</summary>
      /// <param name="name">Name of the voice (e.g. "Alex")</param>
      /// <param name="isExact">Exact match for the voice name (default: false, optional)</param>
      /// <returns>Voice for the given name or null if not found.</returns>
      public static Model.Voice VoiceForName(string name, bool isExact = false)
      {
         Model.Voice result = null;

         if (string.IsNullOrEmpty(name))
         {
            Debug.LogWarning("The given 'name' is null or empty! Returning null.");
         }
         else
         {
            result = isExact ? Voices.FirstOrDefault(voice => voice.Name.CTEquals(name)) : Voices.FirstOrDefault(voice => voice.Name.CTContains(name));

            if (result == null)
            {
               //use the default voice
               Debug.LogWarning("No voice for name '" + name + "' found! Speaking with the default voice!");
            }
         }

         return result;
      }

      /// <summary>Speaks a text with a given voice (native mode).</summary>
      /// <param name="text">Text to speak.</param>
      /// <param name="voice">Voice to speak (optional).</param>
      /// <param name="rate">Speech rate of the speaker in percent (1 = 100%, values: 0-3, default: 1, optional).</param>
      /// <param name="pitch">Pitch of the speech in percent (1 = 100%, values: 0-2, default: 1, optional).</param>
      /// <param name="volume">Volume of the speaker in percent (1 = 100%, values: 0-1, default: 1, optional).</param>
      /// <param name="forceSSML">Force SSML on supported platforms (default: true, optional).</param>
      /// <returns>UID of the speaker.</returns>
      public static string SpeakNative(string text, Model.Voice voice = null, float rate = 1f, float pitch = 1f, float volume = 1f, bool forceSSML = true)
      {
         Model.Wrapper wrapper = new Model.Wrapper(text, voice, rate, pitch, volume, forceSSML);

         SpeakNativeWithUID(wrapper);

         return wrapper.Uid;
      }

      /// <summary>Speaks a text with a given voice (native mode).</summary>
      /// <param name="wrapper">Speak wrapper.</param>
      public static void SpeakNativeWithUID(Model.Wrapper wrapper)
      {
         if (Util.Constants.DEV_DEBUG)
            Debug.LogWarning("SpeakNativeWithUID called: " + wrapper);
         if (wrapper != null)
         {
            if (Util.Helper.isEditorMode)
            {
#if UNITY_EDITOR
               speakNativeInEditor(wrapper);
#endif
            }
            else
            {
               if (voiceProvider != null)
               {
                  if (string.IsNullOrEmpty(wrapper.Text))
                  {
                     Debug.LogWarning("'wrapper.Text' is null or empty!");
                  }
                  else
                  {
                     if (instance != null)
                     {
                        BusyCount++;

                        if (!voiceProvider.isSpeakNativeSupported) //add an AudioSource for providers without native support
                        {
                           if (wrapper.Source == null)
                           {
                              wrapper.Source = go.AddComponent<AudioSource>();
                              Debug.Log("AnimeKing  wrapper.Source:"+ wrapper.Source);
                              genericSources.Add(wrapper.Uid, wrapper.Source);
                           }
                           else
                           {
                              if (!providedSources.ContainsKey(wrapper.Uid))
                                 providedSources.Add(wrapper.Uid, wrapper.Source);
                           }

                           wrapper.SpeakImmediately = true; //must always speak immediately
                        }

                        instance.StartCoroutine(voiceProvider.SpeakNative(wrapper));
                     }
                  }
               }
               else
               {
                  logVPIsNull();
               }
            }
         }
         else
         {
            logWrapperIsNull();
         }
      }

      /// <summary>Speaks a text with a given wrapper (native mode).</summary>
      /// <param name="wrapper">Speak wrapper.</param>
      /// <returns>UID of the speaker.</returns>
      public static string SpeakNative(Model.Wrapper wrapper)
      {
         if (wrapper != null)
         {
            SpeakNativeWithUID(wrapper);

            return wrapper.Uid;
         }

         logWrapperIsNull();

         return string.Empty;
      }

      /// <summary>Speaks a text with a given voice.</summary>
      /// <param name="text">Text to speak.</param>
      /// <param name="source">AudioSource for the output (optional).</param>
      /// <param name="voice">Voice to speak (optional).</param>
      /// <param name="speakImmediately">Speak the text immediately (default: true). Only works if 'Source' is not null.</param>
      /// <param name="rate">Speech rate of the speaker in percent (1 = 100%, values: 0-3, default: 1, optional).</param>
      /// <param name="pitch">Pitch of the speech in percent (1 = 100%, values: 0-2, default: 1, optional).</param>
      /// <param name="volume">Volume of the speaker in percent (1 = 100%, values: 0-1, default: 1, optional).</param>
      /// <param name="outputFile">Saves the generated audio to an output file (without extension, optional).</param>
      /// <param name="forceSSML">Force SSML on supported platforms (default: true, optional).</param>
      /// <returns>UID of the speaker.</returns>
      public static string Speak(string text, AudioSource source = null, Model.Voice voice = null, bool speakImmediately = true, float rate = 1f, float pitch = 1f, float volume = 1f, string outputFile = "", bool forceSSML = true,Action<AudioSource> callBack=null)
      {
         Model.Wrapper wrapper = new Model.Wrapper(text, voice, rate, pitch, volume, source, speakImmediately, outputFile, forceSSML);

         
         SpeakWithUID(wrapper);

         Debug.Log("AnimeKing source 123:"+source);
         callBack?.Invoke(source);
         return wrapper.Uid;
      }

      /// <summary>Speaks a text with a given voice.</summary>
      /// <param name="wrapper">Speak wrapper.</param>
      public static void SpeakWithUID(Model.Wrapper wrapper)
      {
         if (Util.Constants.DEV_DEBUG)
            Debug.LogWarning("SpeakWithUID called: " + wrapper);

         if (wrapper != null)
         {
            if (Util.Helper.isEditorMode)
            {
#if UNITY_EDITOR
               speakNativeInEditor(wrapper);
#endif
            }
            else
            {
               if (voiceProvider != null)
               {
                  if (string.IsNullOrEmpty(wrapper.Text))
                  {
                     Debug.LogWarning("'wrapper.Text' is null or empty!");
                  }
                  else
                  {
                     if (instance != null)
                     {
                        BusyCount++;

                        if (voiceProvider.isSpeakSupported) //audio file generation possible
                        {
                           if (wrapper.Source == null)
                           {
                              wrapper.Source = go.AddComponent<AudioSource>();

                              genericSources.Add(wrapper.Uid, wrapper.Source);

                              if (string.IsNullOrEmpty(wrapper.OutputFile))
                              {
                                 wrapper.SpeakImmediately = true; //must always speak immediately (since there is no AudioSource given and no output file wanted)
                              }
                           }
                           else
                           {
                              if (!providedSources.ContainsKey(wrapper.Uid))
                                 providedSources.Add(wrapper.Uid, wrapper.Source);
                           }

                           wrapper.Source.mute = isMuted;
                           
                           Debug.Log("AnimeKing wrapper.Source:"+wrapper.Source.clip);

                           //TODO activate in providers (waiting for it)
                           //if (isPaused)
                           //    wrapper.Source.Pause();
                        }

                        instance.StartCoroutine(voiceProvider.Speak(wrapper));
                     }
                  }
               }
               else
               {
                  logVPIsNull();
               }
            }
         }
         else
         {
            logWrapperIsNull();
         }
      }

      /// <summary>Speaks a text with a given wrapper.</summary>
      /// <param name="wrapper">Speak wrapper.</param>
      /// <returns>UID of the speaker.</returns>
      public static string Speak(Model.Wrapper wrapper)
      {
         if (wrapper != null)
         {
            SpeakWithUID(wrapper);

            return wrapper.Uid;
         }

         logWrapperIsNull();

         return string.Empty;
      }

      /// <summary>Speaks and marks a text with a given wrapper.</summary>
      /// <param name="wrapper">Speak wrapper.</param>
      public static void SpeakMarkedWordsWithUID(Model.Wrapper wrapper)
      {
         if (Util.Constants.DEV_DEBUG)
            Debug.LogWarning("SpeakMarkedWordsWithUID called: " + wrapper);

         if (voiceProvider != null)
         {
            if (string.IsNullOrEmpty(wrapper.Text))
            {
               Debug.LogWarning("'wrapper.Text' is null or empty!");
            }
            else
            {
               if (wrapper.Source == null || wrapper.Source.clip == null)
               {
                  Debug.LogError("'wrapper.Source' must be a valid AudioSource with a clip! Use 'Speak()' before!");
               }
               else
               {
                  BusyCount++;

                  wrapper.SpeakImmediately = true;

                  //TODO improve the detection for supported providers
                  if (!Util.Helper.isMacOSPlatform && !Util.Helper.isWSABasedPlatform && !isMaryMode) //prevent "double-speak"
                  {
                     wrapper.Volume = 0f;
                     wrapper.Source.PlayDelayed(0.1f);
                  }

                  SpeakNativeWithUID(wrapper);
               }
            }
         }
         else
         {
            logVPIsNull();
         }
      }


      /// <summary>Speaks and marks a text with a given voice and tracks the word position.</summary>
      /// <param name="uid">UID of the speaker</param>
      /// <param name="text">Text to speak.</param>
      /// <param name="source">AudioSource for the output.</param>
      /// <param name="voice">Voice to speak (optional).</param>
      /// <param name="rate">Speech rate of the speaker in percent (1 = 100%, values: 0-3, default: 1, optional).</param>
      /// <param name="pitch">Pitch of the speech in percent (1 = 100%, values: 0-2, default: 1, optional).</param>
      /// <param name="forceSSML">Force SSML on supported platforms (default: true, optional).</param>
      public static void SpeakMarkedWordsWithUID(string uid, string text, AudioSource source, Model.Voice voice = null, float rate = 1f, float pitch = 1f, bool forceSSML = true)
      {
         SpeakMarkedWordsWithUID(new Model.Wrapper(uid, text, voice, rate, pitch, 0, source, true, "", forceSSML));
      }

      //      /// <summary>
      //      /// Speaks a text with a given voice and tracks the word position.
      //      /// </summary>
      //      public static Guid SpeakMarkedWords(string text, AudioSource source = null, Voice voice = null, int rate = 1, int volume = 100) {
      //         Guid result = Guid.NewGuid();
      //
      //         SpeakMarkedWordsWithUID(result, text, source, voice, rate, volume);
      //
      //         return result;
      //      }

      /// <summary>Generates an audio file from a given wrapper.</summary>
      /// <param name="wrapper">Speak wrapper.</param>
      /// <returns>UID of the generator.</returns>
      public static string Generate(Model.Wrapper wrapper)
      {
         if (wrapper != null)
         {
            if (Util.Helper.isEditorMode)
            {
#if UNITY_EDITOR
               generateInEditor(wrapper);
#endif
            }
            else
            {
               if (voiceProvider != null)
               {
                  if (string.IsNullOrEmpty(wrapper.Text))
                  {
                     Debug.LogWarning("'wrapper.Text' is null or empty! Can't generate audio file.");
                  }
                  else
                  {
                     if (string.IsNullOrEmpty(wrapper.OutputFile))
                     {
                        Debug.LogWarning("'wrapper.OutputFile' is null or empty! Can't generate audio file.");
                     }
                     else
                     {
                        instance.StartCoroutine(voiceProvider.Generate(wrapper));
                     }
                  }

                  return wrapper.Uid;
               }

               logVPIsNull();
            }
         }
         else
         {
            logWrapperIsNull();
         }

         return string.Empty;
      }


      /// <summary>Generates an audio file from a text with a given voice.</summary>
      /// <param name="text">Text to generate.</param>
      /// <param name="outputFile">Saves the generated audio to an output file (without extension).</param>
      /// <param name="voice">Voice to speak (optional).</param>
      /// <param name="rate">Speech rate of the speaker in percent (1 = 100%, values: 0-3, default: 1, optional).</param>
      /// <param name="pitch">Pitch of the speech in percent (1 = 100%, values: 0-2, default: 1, optional).</param>
      /// <param name="volume">Volume of the speaker in percent (1 = 100%, values: 0-1, default: 1, optional).</param>
      /// <param name="forceSSML">Force SSML on supported platforms (default: true, optional).</param>
      /// <returns>UID of the generator.</returns>
      public static string Generate(string text, string outputFile, Model.Voice voice = null, float rate = 1f, float pitch = 1f, float volume = 1f, bool forceSSML = true)
      {
         Model.Wrapper wrapper = new Model.Wrapper(text, voice, rate, pitch, volume, null, false, outputFile, forceSSML);

         return Generate(wrapper);
      }

      /// <summary>Silence all active TTS-voices.</summary>
      public static void Silence()
      {
         if (Util.Constants.DEV_DEBUG)
            Debug.LogWarning("Silence called");

         if (voiceProvider != null)
         {
            voiceProvider.Silence();

            /*
            if (instance != null && voiceProvider.hasCoRoutines)
                instance.StopAllCoroutines();
            */

            foreach (var source in genericSources.Where(source => source.Value != null))
            {
               source.Value.Stop();
               Destroy(source.Value, 0.1f);
            }

            genericSources.Clear();

            foreach (var source in providedSources.Where(source => source.Value != null))
            {
               source.Value.Stop();
            }
         }
         else
         {
            providedSources.Clear();

            if (!Common.Util.BaseHelper.isEditorMode)
               logVPIsNull();
         }

         SpeechCount = 0;
         BusyCount = 0;
      }

      /// <summary>Silence an active TTS-voice with a UID.</summary>
      /// <param name="uid">UID of the speaker</param>
      public static void Silence(string uid)
      {
         if (Common.Util.BaseConstants.DEV_DEBUG)
            Debug.LogWarning("Silence called: " + uid);

         if (voiceProvider != null)
         {
            if (!string.IsNullOrEmpty(uid))
            {
               if (genericSources.ContainsKey(uid))
               {
                  AudioSource source;

                  if (genericSources.TryGetValue(uid, out source))
                  {
                     source.Stop();
                     genericSources.Remove(uid);
                  }
               }
               else if (providedSources.ContainsKey(uid))
               {
                  AudioSource source;

                  if (providedSources.TryGetValue(uid, out source))
                  {
                     source.Stop();
                     providedSources.Remove(uid);
                  }
               }
               else
               {
                  voiceProvider.Silence(uid);
               }
            }
         }
         else
         {
            logVPIsNull();
         }

         //SpeechCount--;
      }

      /// <summary>Pause all active TTS-voices (optional with a UID, only for 'Speak'-calls).</summary>
      /// <param name="uid">UID of the speaker (optional)</param>
      public static void Pause(string uid = null)
      {
         if (Util.Constants.DEV_DEBUG)
            Debug.LogWarning("Pause called: " + uid);

         isPaused = true;

         if (voiceProvider != null)
         {
            if (!string.IsNullOrEmpty(uid))
            {
               if (genericSources.ContainsKey(uid))
               {
                  AudioSource source;

                  if (genericSources.TryGetValue(uid, out source))
                     source.Pause();
               }
               else if (providedSources.ContainsKey(uid))
               {
                  AudioSource source;

                  if (providedSources.TryGetValue(uid, out source))
                     source.Pause();
               }
               else
               {
                  Debug.Log("No AudioSource for uid found: " + uid);
               }
            }
            else
            {
               foreach (var source in genericSources.Where(source => source.Value != null))
               {
                  source.Value.Pause();
               }

               foreach (var source in providedSources.Where(source => source.Value != null))
               {
                  source.Value.Pause();
               }
            }
         }
         else
         {
            logVPIsNull();
         }
      }

      /// <summary>Un-Pause all active TTS-voices (optional with a UID, only for 'Speak'-calls).</summary>
      /// <param name="uid">UID of the speaker (optional)</param>
      public static void UnPause(string uid = null)
      {
         if (Util.Constants.DEV_DEBUG)
            Debug.LogWarning("UnPause called: " + uid);

         isPaused = false;

         if (voiceProvider != null)
         {
            if (!string.IsNullOrEmpty(uid))
            {
               if (genericSources.ContainsKey(uid))
               {
                  AudioSource source;

                  if (genericSources.TryGetValue(uid, out source))
                     source.UnPause();
               }
               else if (providedSources.ContainsKey(uid))
               {
                  AudioSource source;

                  if (providedSources.TryGetValue(uid, out source))
                     source.UnPause();
               }
               else
               {
                  Debug.Log("No AudioSource for uid found: " + uid);
               }
            }
            else
            {
               foreach (var source in genericSources.Where(source => source.Value != null))
               {
                  source.Value.UnPause();
               }

               foreach (var source in providedSources.Where(source => source.Value != null))
               {
                  source.Value.UnPause();
               }
            }
         }
         else
         {
            logVPIsNull();
         }
      }

      /// <summary>Pause or unpause all active TTS-voices (optional with a UID, only for 'Speak'-calls).</summary>
      /// <param name="uid">UID of the speaker (optional)</param>
      /// <param name="uid">UID of the speaker (optional)</param>
      public static void PauseOrUnPause(string uid = null)
      {
         if (isPaused)
         {
            UnPause(uid);
         }
         else
         {
            Pause(uid);
         }
      }

      /// <summary>Mute all active TTS-voices (optional with a UID, only for 'Speak'-calls).</summary>
      /// <param name="uid">UID of the speaker (optional)</param>
      public static void Mute(string uid = null)
      {
         if (Util.Constants.DEV_DEBUG)
            Debug.LogWarning("Mute called: " + uid);

         isMuted = true;

         if (voiceProvider != null)
         {
            if (!string.IsNullOrEmpty(uid))
            {
               if (genericSources.ContainsKey(uid))
               {
                  AudioSource source;

                  if (genericSources.TryGetValue(uid, out source))
                     source.mute = true;
               }
               else if (providedSources.ContainsKey(uid))
               {
                  AudioSource source;

                  if (providedSources.TryGetValue(uid, out source))
                     source.mute = true;
               }
               else
               {
                  Debug.Log("No AudioSource for uid found: " + uid);
               }
            }
            else
            {
               foreach (var source in genericSources.Where(source => source.Value != null))
               {
                  source.Value.mute = true;
               }

               foreach (var source in providedSources.Where(source => source.Value != null))
               {
                  source.Value.mute = true;
               }
            }
         }
         else
         {
            logVPIsNull();
         }
      }

      /// <summary>Un-mute all active TTS-voices (optional with a UID, only for 'Speak'-calls).</summary>
      /// <param name="uid">UID of the speaker (optional)</param>
      public static void UnMute(string uid = null)
      {
         if (Util.Constants.DEV_DEBUG)
            Debug.LogWarning("UnMute called: " + uid);

         isMuted = false;

         if (voiceProvider != null)
         {
            if (!string.IsNullOrEmpty(uid))
            {
               if (genericSources.ContainsKey(uid))
               {
                  AudioSource source;

                  if (genericSources.TryGetValue(uid, out source))
                     source.mute = false;
               }
               else if (providedSources.ContainsKey(uid))
               {
                  AudioSource source;

                  if (providedSources.TryGetValue(uid, out source))
                     source.mute = false;
               }
               else
               {
                  Debug.Log("No AudioSource for uid found: " + uid);
               }
            }
            else
            {
               foreach (var source in genericSources.Where(source => source.Value != null))
               {
                  source.Value.mute = false;
               }

               foreach (var source in providedSources.Where(source => source.Value != null))
               {
                  source.Value.mute = false;
               }
            }
         }
         else
         {
            logVPIsNull();
         }
      }

      /// <summary>Mute or unmute all active TTS-voices (optional with a UID, only for 'Speak'-calls).</summary>
      /// <param name="uid">UID of the speaker (optional)</param>
      public static void MuteOrUnMute(string uid = null)
      {
         if (isMuted)
         {
            UnMute(uid);
         }
         else
         {
            Mute(uid);
         }
      }

      /// <summary>Reloads the provider.</summary>
      public static void ReloadProvider()
      {
         //Debug.LogWarning("RELOAD");
         Silence();
         initProvider();
      }

      /// <summary>Deletes all generated audio files.</summary>
      public static void DeleteAudioFiles()
      {
         if (!Util.Helper.isWebPlatform)
         {
            string path = Application.persistentDataPath;

#if !UNITY_WSA || UNITY_EDITOR
            if (deleteWorker != null && deleteWorker.IsAlive)
            {
               if (Util.Constants.DEV_DEBUG)
                  Debug.Log("Kill worker");

               deleteWorker.Abort(); //TODO dangerous - find a better solution!
            }

            deleteWorker = new System.Threading.Thread(() => deleteAudioFiles(path));
            deleteWorker.Start();
#else
                deleteAudioFiles(path);
#endif
         }
      }

      #endregion


      #region Private methods

      private static void deleteAudioFiles(string persistentDataPath)
      {
         try
         {
            System.Random rnd = new System.Random();
            string filesToDelete = Util.Constants.AUDIOFILE_PREFIX + "*"; // + AudioFileExtension;
            string path = Util.Helper.isAndroidPlatform || Util.Helper.isWSABasedPlatform ? Util.Helper.ValidatePath(persistentDataPath) : Util.Config.AUDIOFILE_PATH;
            string[] fileList = System.IO.Directory.GetFiles(path, filesToDelete);

            foreach (var t in fileList)
            {
               try
               {
#if !UNITY_WSA || UNITY_EDITOR
                  if (Util.Helper.isWindowsPlatform /* && ii % 10 == 0 */) //only for Windows to prevent issues with AV
                  {
                     //Debug.Log("++ Sleeping ++");
                     System.Threading.Thread.Sleep(rnd.Next(1200, 1800));
                  }
#endif
                  //Debug.Log("++ Deleting: " + fileList[ii]);
                  System.IO.File.Delete(t);
               }
               catch (System.Exception ex)
               {
                  if (!Util.Helper.isEditor)
                     Debug.LogWarning("Could not delete the file " + t + ": " + ex);
               }
            }
         }
         catch (System.Exception ex)
         {
            if (!Util.Helper.isEditor)
               Debug.LogWarning("Could not scan the path for files: " + ex);
         }
      }

      private static void initProvider()
      {
         if (instance != null)
         {
            //TODO MaryTTS or CustomVoiceProvider, what matters most?

            areVoicesReady = false;
            enforcedStandaloneTTS = false;

            bool useCustom = CustomVoiceProvider != null && isCustomMode && CustomVoiceProvider.enabled;

            //Debug.Log("CustomVoiceProvider != null: " + (CustomVoiceProvider != null));
            //Debug.Log("isCustomMode: " + isCustomMode);
            //Debug.Log("CustomVoiceProvider.enabled: " + CustomVoiceProvider.enabled);

            if (useCustom)
            {
               if (CustomVoiceProvider.isPlatformSupported)
               {
                  subscribeCustomEvents();
                  voiceProvider = CustomVoiceProvider;
                  CustomVoiceProvider.Load();
               }
               else
               {
                  Debug.LogWarning("'Custom Provider' does not support the current platform!");
                  useCustom = false;

                  //if (!Util.Helper.isEditorMode)
                  //    instance.CustomMode = false;
               }
            }

            if (!useCustom)
            {
               unsubscribeCustomEvents();

               if (isMaryMode)
               {
                  if (MaryUrl.Contains("mary.dfki.de") || MaryUser.CTEquals("rtvdemo"))
                  {
                     if (Util.Helper.isEditor)
                     {
                        Debug.LogWarning("You are using the test server of MaryTTS. Please request an account for our service at 'rtvoice@crosstales' or setup your own server from 'http://mary.dfki.de'.");

                        voiceProvider = new Provider.VoiceProviderMary(instance, MaryUrl, MaryPort, MaryUser, MaryPassword);
                     }
                     else
                     {
#if CT_DEVELOP
                        voiceProvider = new Provider.VoiceProviderMary(instance, MaryUrl, MaryPort, MaryUser, MaryPassword);
#else
                                Debug.LogError("You are using the test server of MaryTTS - this is not allowed in builds! Please request an account for our service at 'rtvoice@crosstales' or setup your own server from 'http://mary.dfki.de'.");

                                voiceProvider = new Provider.VoiceProviderMary(instance, string.Empty);
                                //isMaryMode = false;
                                //initOSProvider();
#endif
                     }
                  }
                  else
                  {
                     voiceProvider = new Provider.VoiceProviderMary(instance, MaryUrl, MaryPort, MaryUser, MaryPassword);
                  }
               }
               else
               {
                  initOSProvider();
               }
            }

            if (_onProviderChange != null) _onProviderChange.Invoke(voiceProvider.GetType().ToString());
         }
      }

      private static void initOSProvider()
      {
         if (!Util.Helper.isMacOSEditor && !Util.Helper.isLinuxEditor && Util.Helper.isWindowsPlatform && !instance.ESpeakMode || Util.Helper.isWindowsEditor && Util.Config.ENFORCE_STANDALONE_TTS && !instance.ESpeakMode)
         {
            enforcedStandaloneTTS = !Util.Helper.isWindowsPlatform && Util.Helper.isWindowsEditor && Util.Config.ENFORCE_STANDALONE_TTS;
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            voiceProvider = new Provider.VoiceProviderWindows(instance);
#endif
         }
         else if (!Util.Helper.isWindowsEditor && !Util.Helper.isLinuxEditor && Util.Helper.isMacOSPlatform && !instance.ESpeakMode || Util.Helper.isMacOSEditor && Util.Config.ENFORCE_STANDALONE_TTS && !instance.ESpeakMode)
         {
            enforcedStandaloneTTS = !Util.Helper.isMacOSPlatform && Util.Helper.isMacOSEditor && Util.Config.ENFORCE_STANDALONE_TTS;
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
            voiceProvider = new Provider.VoiceProviderMacOS(instance);
#endif
         }
#if UNITY_STANDALONE || UNITY_EDITOR
         else if (instance.ESpeakMode && Provider.VoiceProviderLinux.isSupported)
         {
            voiceProvider = new Provider.VoiceProviderLinux(instance);
         }
#endif
         else if (Util.Helper.isAndroidPlatform)
         {
#if UNITY_ANDROID || UNITY_EDITOR
            voiceProvider = new Provider.VoiceProviderAndroid(instance);
#endif
         }
         else if (Util.Helper.isIOSBasedPlatform)
         {
#if UNITY_IOS || UNITY_TVOS || UNITY_EDITOR
            voiceProvider = new Provider.VoiceProviderIOS(instance);
#endif
         }
         else if (Util.Helper.isWSABasedPlatform)
         {
#if UNITY_WSA || UNITY_EDITOR
            voiceProvider = new Provider.VoiceProviderWSA(instance);
#endif
         }
         else
         {
            Debug.LogError("No valid TTS provider found!");
            voiceProvider = null;
            //voiceProvider = new Provider.VoiceProviderLinux(instance); // always add a default provider
         }
      }

      private static void logWrapperIsNull()
      {
         const string errorMessage = "'wrapper' is null!";

         onErrorInfo(null, errorMessage);

         Debug.LogError(errorMessage);
      }

      private static void logVPIsNull()
      {
         string errorMessage = "'voiceProvider' is null!" + System.Environment.NewLine + "Did you add the 'RTVoice'-prefab to the current scene?";

         onErrorInfo(null, errorMessage);

         if (!loggedVPIsNull && !Common.Util.BaseHelper.isEditorMode)
         {
            Debug.LogWarning(errorMessage);
            loggedVPIsNull = true;
         }
      }

      private static void subscribeCustomEvents()
      {
         if (CustomVoiceProvider != null)
         {
            CustomVoiceProvider.isActive = true;
            CustomVoiceProvider.OnVoicesReady += onVoicesReady;
            CustomVoiceProvider.OnSpeakStart += onSpeakStart;
            CustomVoiceProvider.OnSpeakComplete += onSpeakComplete;
            CustomVoiceProvider.OnSpeakCurrentWord += onSpeakCurrentWord;
            CustomVoiceProvider.OnSpeakCurrentPhoneme += onSpeakCurrentPhoneme;
            CustomVoiceProvider.OnSpeakCurrentViseme += onSpeakCurrentViseme;
            CustomVoiceProvider.OnSpeakAudioGenerationStart += onSpeakAudioGenerationStart;
            CustomVoiceProvider.OnSpeakAudioGenerationComplete += onSpeakAudioGenerationComplete;
            CustomVoiceProvider.OnErrorInfo += onErrorInfo;
         }
      }

      private static void unsubscribeCustomEvents()
      {
         if (CustomVoiceProvider != null)
         {
            CustomVoiceProvider.isActive = false;
            CustomVoiceProvider.OnVoicesReady -= onVoicesReady;
            CustomVoiceProvider.OnSpeakStart -= onSpeakStart;
            CustomVoiceProvider.OnSpeakComplete -= onSpeakComplete;
            CustomVoiceProvider.OnSpeakCurrentWord -= onSpeakCurrentWord;
            CustomVoiceProvider.OnSpeakCurrentPhoneme -= onSpeakCurrentPhoneme;
            CustomVoiceProvider.OnSpeakCurrentViseme -= onSpeakCurrentViseme;
            CustomVoiceProvider.OnSpeakAudioGenerationStart -= onSpeakAudioGenerationStart;
            CustomVoiceProvider.OnSpeakAudioGenerationComplete -= onSpeakAudioGenerationComplete;
            CustomVoiceProvider.OnErrorInfo -= onErrorInfo;
         }
      }

      #endregion


      #region Event-trigger methods

      private static void onVoicesReady()
      {
         areVoicesReady = true;

         if (_onVoicesReady != null) _onVoicesReady.Invoke();
      }

      private static void onSpeakStart(Model.Wrapper wrapper)
      {
         if (_onSpeakStart != null) _onSpeakStart.Invoke(wrapper);

         SpeechCount++;
      }

      private static void onSpeakComplete(Model.Wrapper wrapper)
      {
         if (_onSpeakComplete != null) _onSpeakComplete.Invoke(wrapper);

         SpeechCount--;
         BusyCount--;
      }

      private static void onSpeakCurrentWord(Model.Wrapper wrapper, string[] speechTextArray, int wordIndex)
      {
         if (_onSpeakCurrentWord != null) _onSpeakCurrentWord.Invoke(wrapper, speechTextArray, wordIndex);
      }

      private static void onSpeakCurrentPhoneme(Model.Wrapper wrapper, string phoneme)
      {
         if (_onSpeakCurrentPhoneme != null) _onSpeakCurrentPhoneme.Invoke(wrapper, phoneme);
      }

      private static void onSpeakCurrentViseme(Model.Wrapper wrapper, string viseme)
      {
         if (_onSpeakCurrentViseme != null) _onSpeakCurrentViseme.Invoke(wrapper, viseme);
      }

      private static void onSpeakAudioGenerationStart(Model.Wrapper wrapper)
      {
         if (_onSpeakAudioGenerationStart != null) _onSpeakAudioGenerationStart.Invoke(wrapper);
      }

      private static void onSpeakAudioGenerationComplete(Model.Wrapper wrapper)
      {
         if (_onSpeakAudioGenerationComplete != null) _onSpeakAudioGenerationComplete.Invoke(wrapper);
      }

      private static void onErrorInfo(Model.Wrapper wrapper, string errorInfo)
      {
         if (_onErrorInfo != null) _onErrorInfo.Invoke(wrapper, errorInfo);
      }

      #endregion


      #region Editor-only methods

#if UNITY_EDITOR

      private static void speakNativeInEditor(Model.Wrapper wrapper)
      {
         if (Util.Helper.isEditorMode)
         {
            if (voiceProvider != null)
            {
               if (string.IsNullOrEmpty(wrapper.Text))
               {
                  Debug.LogWarning("'wrapper.Text' is null or empty!");
               }
               else
               {
                  System.Threading.Thread worker = new System.Threading.Thread(() => voiceProvider.SpeakNativeInEditor(wrapper));
                  worker.Start();
               }

               //return wrapper.Uid;
            }
            else
            {
               logVPIsNull();
            }
         }
         else
         {
            Debug.LogWarning("'SpeakNativeInEditor()' works only inside the Unity Editor!");
         }

         //return string.Empty;
      }

      private static void generateInEditor(Model.Wrapper wrapper)
      {
         if (Util.Helper.isEditorMode)
         {
            if (voiceProvider != null)
            {
               if (string.IsNullOrEmpty(wrapper.Text))
               {
                  Debug.LogWarning("'wrapper.Text' is null or empty!");
               }
               else
               {
                  System.Threading.Thread worker = new System.Threading.Thread(() => voiceProvider.GenerateInEditor(wrapper));
                  worker.Start();
               }

               //return wrapper.Uid;
            }

            logVPIsNull();
         }
         else
         {
            Debug.LogWarning("'GenerateInEditor()' works only inside the Unity Editor!");
         }

         //return string.Empty;
      }

#endif

      #endregion
   }
}
// © 2015-2020 crosstales LLC (https://www.crosstales.com)