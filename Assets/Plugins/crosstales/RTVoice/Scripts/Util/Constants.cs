using UnityEngine;

namespace Crosstales.RTVoice.Util
{
   /// <summary>Collected constants of very general utility for the asset.</summary>
   public abstract class Constants : Common.Util.BaseConstants
   {
      #region Constant variables

      /// <summary>Name of the asset.</summary>
      public const string ASSET_NAME = "RT-Voice PRO";

      /// <summary>Short name of the asset.</summary>
      public const string ASSET_NAME_SHORT = "RTV PRO";

      /// <summary>Version of the asset.</summary>
      public const string ASSET_VERSION = "2020.1.2";

      /// <summary>Build number of the asset.</summary>
      public const int ASSET_BUILD = 20200326;

      /// <summary>Create date of the asset (YYYY, MM, DD).</summary>
      public static readonly System.DateTime ASSET_CREATED = new System.DateTime(2015, 4, 29);

      /// <summary>Change date of the asset (YYYY, MM, DD).</summary>
      public static readonly System.DateTime ASSET_CHANGED = new System.DateTime(2020, 3, 26);

      /// <summary>URL of the PRO asset in UAS.</summary>
      public const string ASSET_PRO_URL = "https://assetstore.unity.com/packages/slug/41068?aid=1011lNGT";

      /// <summary>URL of the 2019 asset in UAS.</summary>
      public const string ASSET_2019_URL = "https://www.assetstore.unity3d.com/#!/content/41068?aid=1011lNGT"; //TODO TBD!

      /// <summary>URL of the 3rd party assets in UAS.</summary>
      public const string ASSET_3P_URL = "https://assetstore.unity.com/lists/rt-voice-friends-42209?aid=1011lNGT"; // RTV&Friends list

      /// <summary>URL for update-checks of the asset</summary>
      public const string ASSET_UPDATE_CHECK_URL = "https://www.crosstales.com/media/assets/rtvoice_versions.txt";
      //public const string ASSET_UPDATE_CHECK_URL = "https://www.crosstales.com/media/assets/test/rtvoice_versions_test.txt";

      /// <summary>Contact to the owner of the asset.</summary>
      public const string ASSET_CONTACT = "rtvoice@crosstales.com";

      /// <summary>URL of the asset manual.</summary>
      public const string ASSET_MANUAL_URL = "https://www.crosstales.com/media/data/assets/rtvoice/RTVoice-doc.pdf";

      /// <summary>URL of the asset API.</summary>
      public const string ASSET_API_URL = "http://www.crosstales.com/en/assets/rtvoice/api/";

      /// <summary>URL of the asset forum.</summary>
      public const string ASSET_FORUM_URL = "http://forum.unity3d.com/threads/rt-voice-run-time-text-to-speech-solution.340046/";

      /// <summary>URL of the asset in crosstales.</summary>
      public const string ASSET_WEB_URL = "https://www.crosstales.com/en/portfolio/rtvoice/";

      /// <summary>URL of the promotion video of the asset (Youtube).</summary>
      public const string ASSET_VIDEO_PROMO = "https://youtu.be/iVhTWDLY7g8?list=PLgtonIOr6Tb41XTMeeZ836tjHlKgOO84S";

      /// <summary>URL of the tutorial video of the asset (Youtube).</summary>
      public const string ASSET_VIDEO_TUTORIAL = "https://youtu.be/OJyVgCmX3wU?list=PLgtonIOr6Tb41XTMeeZ836tjHlKgOO84S";

      /// <summary>URL of the 3rd party asset "Adventure Creator".</summary>
      public const string ASSET_3P_ADVENTURE_CREATOR = "https://assetstore.unity.com/packages/slug/11896?aid=1011lNGT";

      /// <summary>URL of the 3rd party asset "Cinema Director".</summary>
      public const string ASSET_3P_CINEMA_DIRECTOR = "https://assetstore.unity.com/packages/slug/19779?aid=1011lNGT";

      /// <summary>URL of the 3rd party asset "Dialogue System".</summary>
      public const string ASSET_3P_DIALOGUE_SYSTEM = "https://assetstore.unity.com/packages/slug/11672?aid=1011lNGT";

      /// <summary>URL of the 3rd party asset "Localized Dialogs".</summary>
      public const string ASSET_3P_LOCALIZED_DIALOGS = "https://assetstore.unity.com/packages/slug/5020?aid=1011lNGT";

      /// <summary>URL of the 3rd party asset "LipSync Pro".</summary>
      public const string ASSET_3P_LIPSYNC = "https://assetstore.unity.com/packages/slug/32117?aid=1011lNGT";

      /// <summary>URL of the 3rd party asset "NPC Chat".</summary>
      public const string ASSET_3P_NPC_CHAT = "https://assetstore.unity.com/packages/slug/9723?aid=1011lNGT";

      /// <summary>URL of the 3rd party asset "Quest System Pro".</summary>
      public const string ASSET_3P_QUEST_SYSTEM = "https://assetstore.unity.com/packages/slug/63460?aid=1011lNGT";

      /// <summary>URL of the 3rd party asset "SALSA".</summary>
      public const string ASSET_3P_SALSA = "https://assetstore.unity.com/packages/slug/148442?aid=1011lNGT";

      /// <summary>URL of the 3rd party asset "SLATE".</summary>
      public const string ASSET_3P_SLATE = "https://assetstore.unity.com/packages/slug/56558?aid=1011lNGT";

      /// <summary>URL of the 3rd party asset "THE Dialogue Engine".</summary>
      public const string ASSET_3P_AMPLITUDE = "https://assetstore.unity.com/packages/slug/111277?aid=1011lNGT";

      /// <summary>URL of the 3rd party asset "uSequencer".</summary>
      public const string ASSET_3P_KLATTERSYNTH = "https://assetstore.unity.com/packages/slug/95453?aid=1011lNGT";

      /// <summary>URL of the 3rd party asset "WebGL Speech Synthesis".</summary>
      public const string ASSET_3P_WEBGL = "https://assetstore.unity.com/packages/slug/81861?aid=1011lNGT";

      /// <summary>URL of the 3rd party asset "Google Cloud Text To Speech".</summary>
      public const string ASSET_3P_GOOGLE = "https://assetstore.unity.com/packages/slug/115170?aid=1011lNGT";


      // Keys for the configuration of the asset
      public const string KEY_PREFIX = "RTVOICE_CFG_";
      public const string KEY_ASSET_PATH = KEY_PREFIX + "ASSET_PATH";
      public const string KEY_DEBUG = KEY_PREFIX + "DEBUG";
      public const string KEY_AUDIOFILE_PATH = KEY_PREFIX + "AUDIOFILE_PATH";
      public const string KEY_AUDIOFILE_AUTOMATIC_DELETE = KEY_PREFIX + "AUDIOFILE_AUTOMATIC_DELETE";

      public const string KEY_ENFORCE_32BIT_WINDOWS = KEY_PREFIX + "ENFORCE_32BIT_WINDOWS";

      public const string KEY_ENFORCE_STANDALONE_TTS = KEY_PREFIX + "ENFORCE_STANDALONE_TTS";

      //public const string KEY_TTS_MACOS = KEY_PREFIX + "TTS_MACOS";
      public const string KEY_ENSURE_NAME = KEY_PREFIX + "ENSURE_NAME";

      // Default values
      public static readonly string DEFAULT_AUDIOFILE_PATH = Helper.ValidatePath(Application.temporaryCachePath);
      public const bool DEFAULT_AUDIOFILE_AUTOMATIC_DELETE = true;
      public const bool DEFAULT_ENFORCE_32BIT_WINDOWS = false;
      public const bool DEFAULT_ENFORCE_STANDALONE_TTS = true;
      public const string DEFAULT_TTS_WINDOWS_BUILD = @"/RTVoiceTTSWrapper.exe";
      public const string DEFAULT_TTS_MACOS = "say";

#if ENABLE_IL2CPP && (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN)
        public const string DEFAULT_TTS_LINUX =
 "C:\\Program Files (x86)\\eSpeak\\command_line\\espeak.exe"; // eSpeak under Windows
        //public const string DEFAULT_TTS_LINUX = "C:\\Program Files\\eSpeak NG\\espeak-ng.exe"; // eSpeak-NG under Windows
#else
      public const string DEFAULT_TTS_LINUX = "espeak";
      //public const string DEFAULT_TTS_LINUX = "espeak-ng";
#endif

      public const string DEFAULT_TTS_LINUX_DATA = "";

      //public const string DEFAULT_TTS_LINUX_DATA = @"C:\Program Files (x86)\eSpeak\espeak-data\"; //  eSpeak data under Windows
      //public const string DEFAULT_TTS_LINUX_DATA = @"C:\Program Files\eSpeak NG\espeak-ng-data"; // eSpeak-NG data under Windows
      public const int DEFAULT_TTS_KILL_TIME = 7000;
      public const bool DEFAULT_ENSURE_NAME = true;

      /// <summary>RTVoice prefab scene name.</summary>
      public const string RTVOICE_SCENE_OBJECT_NAME = "RTVoice";

      #endregion


      #region Changable variables

      // Technical settings

      /// <summary>Sub-path to the TTS-wrapper under Windows (Editor).</summary>
      public static string TTS_WINDOWS_SUBPATH = "Libraries/Windows/RTVoiceTTSWrapper.exe";

      /// <summary>Sub-path to the TTS-wrapper (32bit) under Windows (Editor).</summary>
      public static string TTS_WINDOWS_x86_SUBPATH = "Libraries/Windows/RTVoiceTTSWrapper_x86.exe";

      /// <summary>Female modifier for eSpeak.</summary>
      public static string ESPEAK_FEMALE_MODIFIER = "+f3";

      /// <summary>Audio file prefix to identify the files.</summary>
      public static string AUDIOFILE_PREFIX = "rtvoice_";

      /// <summary>Defines the speed of 'Speak'-calls in seconds.</summary>
      public static float SPEAK_CALL_SPEED = 0.5f;

      #endregion
   }
}
// © 2015-2020 crosstales LLC (https://www.crosstales.com)