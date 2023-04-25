using UnityEngine;

namespace Crosstales.RTVoice.Model
{
   /// <summary>Alias for multiple voices on different platforms.</summary>
   [System.Serializable]
   public class VoiceAlias
   {
      #region Variables

      /// <summary>Name of the voice under Windows.</summary>
      [Tooltip("Name of the voice under Windows.")] public string VoiceNameWindows = "David";

      /// <summary>Name of the voice under macOS.</summary>
      [Tooltip("Name of the voice under macOS.")] public string VoiceNameMac = "Alex";

      /// <summary>Name of the voice under Linux and for eSpeak.</summary>
      [Tooltip("Name of the voice under Linux and for eSpeak.")] public string VoiceNameLinux = "en";

      /// <summary>Name of the voice under Android.</summary>
      [Tooltip("Name of the voice under Android.")] public string VoiceNameAndroid = "en";

      /// <summary>Name of the voice under iOS.</summary>
      [Tooltip("Name of the voice under iOS.")] public string VoiceNameIOS = "Daniel";

      /// <summary>Name of the voice under WSA.</summary>
      [Tooltip("Name of the voice under WSA.")] public string VoiceNameWSA = "David";

      /// <summary>Name of the voice under MaryTTS.</summary>
      [Tooltip("Name of the voice under MaryTTS.")] public string VoiceNameMaryTTS = "cmu-rms-hsmm";

      /// <summary>Name of the voice for custom TTS-systems.</summary>
      [Tooltip("Name of the voice for custom TTS-systems.")] public string VoiceNameCustom = string.Empty;

      /// <summary>Fallback culture for the text (e.g. 'en', optional).</summary>
      [Tooltip("Fallback culture for the text (e.g. 'en', optional).")] public string Culture = "en";

      /// <summary>Fallback gender for the text.</summary>
      [Tooltip("Fallback gender for the text.")] public Enum.Gender Gender = Enum.Gender.UNKNOWN;

      #endregion


      #region Properties

      /// <summary>Returns the name of the voice for the current platform.</summary>
      /// <returns>The name of the voice for the current platform.</returns>
      public string VoiceName
      {
         get
         {
            string result;

            if (Speaker.CustomVoiceProvider == null)
            {
               if (Speaker.isMaryMode)
               {
                  result = VoiceNameMaryTTS;
               }
               else
               {
                  if (Util.Helper.isWindowsPlatform && !Speaker.isESpeakMode)
                  {
                     result = VoiceNameWindows;
                  }
                  else if (Util.Helper.isMacOSPlatform && !Speaker.isESpeakMode)
                  {
                     result = VoiceNameMac;
                  }
                  else if (Util.Helper.isAndroidPlatform)
                  {
                     result = VoiceNameAndroid;
                  }
                  else if (Util.Helper.isWSABasedPlatform)
                  {
                     result = VoiceNameWSA;
                  }
                  else if (Util.Helper.isIOSBasedPlatform)
                  {
                     result = VoiceNameIOS;
                  }
                  else
                  {
                     result = VoiceNameLinux;
                  }
               }
            }
            else
            {
               result = VoiceNameCustom;
            }

            return result;
         }
      }

      /// <summary>Returns the voice for the current platform.</summary>
      /// <returns>The voice for the current platform.</returns>
      public Voice Voice
      {
         get
         {
            Voice result = Speaker.VoiceForName(VoiceName) ?? Speaker.VoiceForGender(Gender, Culture);

            return result;
         }
      }

      #endregion


      #region Overridden methods

      public override string ToString()
      {
         System.Text.StringBuilder result = new System.Text.StringBuilder();

         result.Append(GetType().Name);
         result.Append(Util.Constants.TEXT_TOSTRING_START);

         result.Append("VoiceNameWindows='");
         result.Append(VoiceNameWindows);
         result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("VoiceNameMac='");
         result.Append(VoiceNameMac);
         result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("VoiceNameLinux='");
         result.Append(VoiceNameLinux);
         result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("VoiceNameAndroid='");
         result.Append(VoiceNameAndroid);
         result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("VoiceNameIOS='");
         result.Append(VoiceNameIOS);
         result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("VoiceNameWSA='");
         result.Append(VoiceNameWSA);
         result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("VoiceNameMaryTTS='");
         result.Append(VoiceNameMaryTTS);
         result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("VoiceNameCustom='");
         result.Append(VoiceNameCustom);
         result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("Culture='");
         result.Append(Culture);
         result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("Gender='");
         result.Append(Gender);
         result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER_END);

         result.Append(Util.Constants.TEXT_TOSTRING_END);

         return result.ToString();
      }

      #endregion
   }
}
// © 2018-2020 crosstales LLC (https://www.crosstales.com)