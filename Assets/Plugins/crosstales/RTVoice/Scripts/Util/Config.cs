namespace Crosstales.RTVoice.Util
{
   /// <summary>Configuration for the asset.</summary>
   public static class Config
   {
      #region Variables

      /// <summary>Path to the asset inside the Unity project.</summary>
      public static string ASSET_PATH = "/Plugins/crosstales/RTVoice/";

      /// <summary>Enable or disable debug logging for the asset.</summary>
      public static bool DEBUG = Constants.DEFAULT_DEBUG || Constants.DEV_DEBUG;

      /// <summary>Automatically delete the generated audio files.</summary>
      public static bool AUDIOFILE_AUTOMATIC_DELETE = Constants.DEFAULT_AUDIOFILE_AUTOMATIC_DELETE;

      /// <summary>Enforce 32bit versions of voices under Windows.</summary>
      public static bool ENFORCE_32BIT_WINDOWS = Constants.DEFAULT_ENFORCE_32BIT_WINDOWS;

      /// <summary>Enforce standalone TTS (for development).</summary>
      public static bool ENFORCE_STANDALONE_TTS = Constants.DEFAULT_ENFORCE_STANDALONE_TTS;

      // Technical settings

      /// <summary>Location of the TTS-wrapper under Windows (standalone).</summary>
      public static string TTS_WINDOWS_BUILD = Constants.DEFAULT_TTS_WINDOWS_BUILD;

      /// <summary>Location of the TTS-system under MacOS.</summary>
      public static string TTS_MACOS = Constants.DEFAULT_TTS_MACOS;

      /// <summary>Location of the TTS-system under Linux.</summary>
      public static string TTS_LINUX = Constants.DEFAULT_TTS_LINUX;

      /// <summary>Location of the data for the TTS-system under Linux.</summary>
      public static string TTS_LINUX_DATA = Constants.DEFAULT_TTS_LINUX_DATA;

      /// <summary>Enable or disable the ensuring the name of the RTVoice gameobject.</summary>
      public static bool ENSURE_NAME = Constants.DEFAULT_ENSURE_NAME;

      /// <summary>Is the configuration loaded?</summary>
      public static bool isLoaded = false;

      private static string audiofilePath = Constants.DEFAULT_AUDIOFILE_PATH;

      #endregion


      #region Properties

      /// <summary>Path to the generated audio files.</summary>
      public static string AUDIOFILE_PATH
      {
         get { return audiofilePath; }

         set { audiofilePath = Helper.ValidatePath(value); }
      }

      /// <summary>Location of the TTS-wrapper under Windows (Editor).</summary>
      public static string TTS_WINDOWS_EDITOR
      {
         get { return ASSET_PATH + Constants.TTS_WINDOWS_SUBPATH; }
      }

      /// <summary>Location of the TTS-wrapper (32bit) under Windows (Editor).</summary>
      public static string TTS_WINDOWS_EDITOR_x86
      {
         get { return ASSET_PATH + Constants.TTS_WINDOWS_x86_SUBPATH; }
      }

      #endregion

#if UNITY_EDITOR

      #region Public static methods

      /// <summary>Resets all changeable variables to their default value.</summary>
      public static void Reset()
      {
         if (!Constants.DEV_DEBUG)
            DEBUG = Constants.DEFAULT_DEBUG;

         AUDIOFILE_PATH = Constants.DEFAULT_AUDIOFILE_PATH;
         AUDIOFILE_AUTOMATIC_DELETE = Constants.DEFAULT_AUDIOFILE_AUTOMATIC_DELETE;

         ENFORCE_32BIT_WINDOWS = Constants.DEFAULT_ENFORCE_32BIT_WINDOWS;
         ENFORCE_STANDALONE_TTS = Constants.DEFAULT_ENFORCE_STANDALONE_TTS;
         TTS_WINDOWS_BUILD = Constants.DEFAULT_TTS_WINDOWS_BUILD;
         TTS_MACOS = Constants.DEFAULT_TTS_MACOS;
         TTS_LINUX = Constants.DEFAULT_TTS_LINUX;
         TTS_LINUX_DATA = Constants.DEFAULT_TTS_LINUX_DATA;
         ENSURE_NAME = Constants.DEFAULT_ENSURE_NAME;
      }

      /// <summary>Loads all changeable variables.</summary>
      public static void Load()
      {
         if (Common.Util.CTPlayerPrefs.HasKey(Constants.KEY_ASSET_PATH))
         {
            ASSET_PATH = Common.Util.CTPlayerPrefs.GetString(Constants.KEY_ASSET_PATH);
         }

         if (!Constants.DEV_DEBUG)
         {
            if (Common.Util.CTPlayerPrefs.HasKey(Constants.KEY_DEBUG))
            {
               DEBUG = Common.Util.CTPlayerPrefs.GetBool(Constants.KEY_DEBUG);
            }
         }
         else
         {
            DEBUG = Constants.DEV_DEBUG;
         }

         if (Common.Util.CTPlayerPrefs.HasKey(Constants.KEY_AUDIOFILE_PATH))
         {
            AUDIOFILE_PATH = Common.Util.CTPlayerPrefs.GetString(Constants.KEY_AUDIOFILE_PATH);
         }

         if (Common.Util.CTPlayerPrefs.HasKey(Constants.KEY_AUDIOFILE_AUTOMATIC_DELETE))
         {
            AUDIOFILE_AUTOMATIC_DELETE =
               Common.Util.CTPlayerPrefs.GetBool(Constants.KEY_AUDIOFILE_AUTOMATIC_DELETE);
         }

         if (Common.Util.CTPlayerPrefs.HasKey(Constants.KEY_ENFORCE_32BIT_WINDOWS))
         {
            ENFORCE_32BIT_WINDOWS = Common.Util.CTPlayerPrefs.GetBool(Constants.KEY_ENFORCE_32BIT_WINDOWS);
         }

         if (Common.Util.CTPlayerPrefs.HasKey(Constants.KEY_ENFORCE_STANDALONE_TTS))
         {
            ENFORCE_STANDALONE_TTS = Common.Util.CTPlayerPrefs.GetBool(Constants.KEY_ENFORCE_STANDALONE_TTS);
         }

         //if (Common.Util.CTPlayerPrefs.HasKey(Constants.KEY_TTS_MACOS))
         //{
         //    TTS_MACOS = Common.Util.CTPlayerPrefs.GetString(Constants.KEY_TTS_MACOS);
         //}

         if (Common.Util.CTPlayerPrefs.HasKey(Constants.KEY_ENSURE_NAME))
         {
            ENSURE_NAME = Common.Util.CTPlayerPrefs.GetBool(Constants.KEY_ENSURE_NAME);
         }

         isLoaded = true;
      }

      /// <summary>Saves all changeable variables.</summary>
      public static void Save()
      {
         if (!Constants.DEV_DEBUG)
            Common.Util.CTPlayerPrefs.SetBool(Constants.KEY_DEBUG, DEBUG);

         Common.Util.CTPlayerPrefs.SetString(Constants.KEY_AUDIOFILE_PATH, AUDIOFILE_PATH);
         Common.Util.CTPlayerPrefs.SetBool(Constants.KEY_AUDIOFILE_AUTOMATIC_DELETE, AUDIOFILE_AUTOMATIC_DELETE);
         Common.Util.CTPlayerPrefs.SetBool(Constants.KEY_ENFORCE_32BIT_WINDOWS, ENFORCE_32BIT_WINDOWS);
         Common.Util.CTPlayerPrefs.SetBool(Constants.KEY_ENFORCE_STANDALONE_TTS, ENFORCE_STANDALONE_TTS);
         //Common.Util.CTPlayerPrefs.SetString(Constants.KEY_TTS_MACOS, TTS_MACOS);
         Common.Util.CTPlayerPrefs.SetBool(Constants.KEY_ENSURE_NAME, ENSURE_NAME);

         Common.Util.CTPlayerPrefs.Save();
      }

      #endregion

#endif
   }
}
// © 2017-2020 crosstales LLC (https://www.crosstales.com)