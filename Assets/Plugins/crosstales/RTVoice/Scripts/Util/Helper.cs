using System.Linq;
using UnityEngine;

namespace Crosstales.RTVoice.Util
{
   /// <summary>Various helper functions.</summary>
   public abstract class Helper : Common.Util.BaseHelper
   {
      #region Variables

      private static readonly string[] appleFemales =
      {
         "Alice",
         "Alva",
         "Amelie",
         "Anna",
         "Carmit",
         "Catherine",
         "Damayanti",
         "Ellen",
         "Fiona",
         "Helena",
         "Ioana",
         "Joana",
         "Kanya",
         "Karen",
         "Kyoko",
         "Laura",
         "Lekha",
         "Li-mu",
         "Luciana",
         "Marie",
         "Mariska",
         "Martha",
         "Mei-Jia",
         "Melina",
         "Milena",
         "Moira",
         "Monica",
         "Nicky",
         "Nora",
         "O-ren",
         "Paulina",
         "Samantha",
         "Sara",
         "Satu",
         "Sin-ji",
         "Tessa",
         "Ting-Ting",
         "Veena",
         "Victoria",
         "Yelda",
         "Yu-shu",
         "Yuna",
         "Zosia",
         "Zuzana"
      };

      private static readonly string[] appleMales =
      {
         "Aaron",
         "Alex",
         "Arthur",
         "Daniel",
         "Diego",
         "Fred",
         "Gordon",
         "Hattori",
         "Jorge",
         "Juan",
         "Luca",
         "Maged",
         "Martin",
         "Thomas",
         "Xander",
         "Yuri"
      };

      private static readonly string[] wsaFemales =
      {
         "Ayumi",
         "Haruka",
         "Caroline",
         "Catherine",
         "Elsa",
         "Hazel",
         "Susan",
         "Heami",
         "Hedda",
         "Katja",
         "Heera",
         "Heidi",
         "Helena",
         "Laura",
         "Helia",
         "Helle",
         "Herena",
         "Hoda",
         "Hortence",
         "Julie",
         "Huihui",
         "Yaoyao",
         "Irina",
         "Kalpana",
         "Linda",
         "Maria",
         "Paulina",
         "Sabina",
         "Tracy",
         "Yating",
         "Hanhan",
         "Zira"
      };

      private static readonly string[] wsaMales =
      {
         "Adam",
         "An",
         "Andika",
         "Andrei",
         "Asaf",
         "Bart",
         "Bengt",
         "Claude",
         "Cosimo",
         "Daniel",
         "Danny",
         "David",
         "Mark",
         "Filip",
         "Frank",
         "George",
         "Hemant",
         "Ichiro",
         "Ivan",
         "James",
         "Jon",
         "Kangkang",
         "Karsten",
         "Lado",
         "Matej",
         "Naayf",
         "Pablo",
         "Pattara",
         "Paul",
         "Pavel",
         "Raul",
         "Ravi",
         "Richard",
         "Rizwan",
         "Shaun",
         "Stefan",
         "Stefanos",
         "Szabolcs",
         "Tolga",
         "Valluvar",
         "Vit",
         "Zhiwei"
      };

      public static readonly System.Collections.Generic.Dictionary<int, string> LocaleCodes = new System.Collections.Generic.Dictionary<int, string>(161);

      #endregion


      #region Static constructor

      static Helper()
      {
         LocaleCodes.Add(1025, "ar-sa");
         LocaleCodes.Add(1026, "bg");
         LocaleCodes.Add(1027, "ca");
         LocaleCodes.Add(1028, "zh-tw");
         LocaleCodes.Add(1029, "cs");
         LocaleCodes.Add(1030, "da");
         LocaleCodes.Add(1031, "de-de");
         LocaleCodes.Add(1032, "el");
         LocaleCodes.Add(1033, "en-us");
         LocaleCodes.Add(1034, "es-es"); //traditional
         LocaleCodes.Add(1035, "fi");
         LocaleCodes.Add(1036, "fr-fr");
         LocaleCodes.Add(1037, "he");
         LocaleCodes.Add(1038, "hu");
         LocaleCodes.Add(1039, "is");
         LocaleCodes.Add(1040, "it-it");
         LocaleCodes.Add(1041, "ja");
         LocaleCodes.Add(1042, "ko");
         LocaleCodes.Add(1043, "nl-nl");
         LocaleCodes.Add(1044, "no-no");
         LocaleCodes.Add(1045, "pl");
         LocaleCodes.Add(1046, "pt-br");
         LocaleCodes.Add(1047, "rm");
         LocaleCodes.Add(1048, "ro");
         LocaleCodes.Add(1049, "ru");
         LocaleCodes.Add(1050, "hr");
         LocaleCodes.Add(1051, "sk");
         LocaleCodes.Add(1052, "sq");
         LocaleCodes.Add(1053, "sv-se");
         LocaleCodes.Add(1054, "th");
         LocaleCodes.Add(1055, "tr");
         LocaleCodes.Add(1056, "ur");
         LocaleCodes.Add(1057, "id");
         LocaleCodes.Add(1058, "uk");
         LocaleCodes.Add(1059, "be");
         LocaleCodes.Add(1060, "sl");
         LocaleCodes.Add(1061, "et");
         LocaleCodes.Add(1062, "lv");
         LocaleCodes.Add(1063, "lt");
         LocaleCodes.Add(1064, "tg");
         LocaleCodes.Add(1065, "fa");
         LocaleCodes.Add(1066, "vi");
         LocaleCodes.Add(1067, "hy");
         LocaleCodes.Add(1068, "az-az");
         LocaleCodes.Add(1069, "eu");
         LocaleCodes.Add(1070, "sb");
         LocaleCodes.Add(1071, "mk");
         LocaleCodes.Add(1073, "ts");
         LocaleCodes.Add(1074, "tn");
         LocaleCodes.Add(1076, "xh");
         LocaleCodes.Add(1077, "zu");
         LocaleCodes.Add(1078, "af");
         LocaleCodes.Add(1080, "fo");
         LocaleCodes.Add(1081, "hi");
         LocaleCodes.Add(1082, "mt");
         LocaleCodes.Add(1084, "gd");
         LocaleCodes.Add(1085, "yi");
         LocaleCodes.Add(1086, "ms-my");
         LocaleCodes.Add(1087, "kk");
         LocaleCodes.Add(1089, "sw");
         LocaleCodes.Add(1090, "tk");
         LocaleCodes.Add(1091, "uz-uz");
         LocaleCodes.Add(1092, "tt");
         LocaleCodes.Add(1093, "bn");
         LocaleCodes.Add(1094, "pa");
         LocaleCodes.Add(1095, "gu");
         LocaleCodes.Add(1096, "or");
         LocaleCodes.Add(1097, "ta");
         LocaleCodes.Add(1098, "te");
         LocaleCodes.Add(1099, "kn");
         LocaleCodes.Add(1100, "ml");
         LocaleCodes.Add(1101, "as");
         LocaleCodes.Add(1102, "mr");
         LocaleCodes.Add(1103, "sa");
         LocaleCodes.Add(1104, "mn");
         LocaleCodes.Add(1105, "bo");
         LocaleCodes.Add(1106, "cy");
         LocaleCodes.Add(1107, "km");
         LocaleCodes.Add(1108, "lo");
         LocaleCodes.Add(1109, "my");
         LocaleCodes.Add(1113, "sd");
         LocaleCodes.Add(1118, "am");
         LocaleCodes.Add(1120, "ks");
         LocaleCodes.Add(1121, "ne");
         LocaleCodes.Add(1140, "gn");
         LocaleCodes.Add(1142, "la");
         LocaleCodes.Add(1143, "so");
         LocaleCodes.Add(1153, "mi");
         LocaleCodes.Add(2049, "ar-iq");
         LocaleCodes.Add(2052, "zh-cn");
         LocaleCodes.Add(2055, "de-ch");
         LocaleCodes.Add(2057, "en-gb");
         LocaleCodes.Add(2058, "es-mx");
         LocaleCodes.Add(2060, "fr-be");
         LocaleCodes.Add(2064, "it-ch");
         LocaleCodes.Add(2067, "nl-be");
         LocaleCodes.Add(2068, "no-no");
         LocaleCodes.Add(2070, "pt-pt");
         LocaleCodes.Add(2072, "ro-mo");
         LocaleCodes.Add(2073, "ru-mo");
         LocaleCodes.Add(2074, "sr-sp");
         LocaleCodes.Add(2077, "sv-fi");
         LocaleCodes.Add(2092, "az-az");
         LocaleCodes.Add(2108, "gd-ie");
         LocaleCodes.Add(2110, "ms-bn");
         LocaleCodes.Add(2115, "uz-uz");
         LocaleCodes.Add(2117, "bn");
         LocaleCodes.Add(2128, "mn");
         LocaleCodes.Add(3073, "ar-eg");
         LocaleCodes.Add(3076, "zh-hk");
         LocaleCodes.Add(3079, "de-at");
         LocaleCodes.Add(3081, "en-au");
         LocaleCodes.Add(3082, "es-es"); //modern
         LocaleCodes.Add(3084, "fr-ca");
         LocaleCodes.Add(3098, "sr-sp");
         LocaleCodes.Add(4097, "ar-ly");
         LocaleCodes.Add(4100, "zh-sg");
         LocaleCodes.Add(4103, "de-lu");
         LocaleCodes.Add(4105, "en-ca");
         LocaleCodes.Add(4106, "es-gt");
         LocaleCodes.Add(4108, "fr-ch");
         LocaleCodes.Add(5121, "ar-dz");
         LocaleCodes.Add(5124, "zh-mo");
         LocaleCodes.Add(5127, "de-li");
         LocaleCodes.Add(5129, "en-nz");
         LocaleCodes.Add(5130, "es-cr");
         LocaleCodes.Add(5132, "fr-lu");
         LocaleCodes.Add(5146, "bs");
         LocaleCodes.Add(6145, "ar-ma");
         LocaleCodes.Add(6153, "en-ie");
         LocaleCodes.Add(6154, "es-pa");
         LocaleCodes.Add(7169, "ar-tn");
         LocaleCodes.Add(7177, "en-za");
         LocaleCodes.Add(7178, "es-do");
         LocaleCodes.Add(8193, "ar-om");
         LocaleCodes.Add(8201, "en-jm");
         LocaleCodes.Add(8202, "es-ve");
         LocaleCodes.Add(9217, "ar-ye");
         LocaleCodes.Add(9225, "en-cb");
         LocaleCodes.Add(9226, "es-co");
         LocaleCodes.Add(10241, "ar-sy");
         LocaleCodes.Add(10249, "en-bz");
         LocaleCodes.Add(10250, "es-pe");
         LocaleCodes.Add(11265, "ar-jo");
         LocaleCodes.Add(11273, "en-tt");
         LocaleCodes.Add(11274, "es-ar");
         LocaleCodes.Add(12289, "ar-lb");
         LocaleCodes.Add(12298, "es-ec");
         LocaleCodes.Add(13313, "ar-kw");
         LocaleCodes.Add(13321, "en-ph");
         LocaleCodes.Add(13322, "es-cl");
         LocaleCodes.Add(14337, "ar-ae");
         LocaleCodes.Add(14346, "es-uy");
         LocaleCodes.Add(15361, "ar-bh");
         LocaleCodes.Add(15370, "es-py");
         LocaleCodes.Add(16385, "ar-qa");
         LocaleCodes.Add(16393, "en-in");
         LocaleCodes.Add(16394, "es-bo");
         LocaleCodes.Add(17418, "es-sv");
         LocaleCodes.Add(18442, "es-hn");
         LocaleCodes.Add(19466, "es-ni");
         LocaleCodes.Add(20490, "es-pr");
      }

      #endregion


      #region Static properties

      /// <summary>Checks if the current platform has built-in TTS.</summary>
      /// <returns>True if the current platform has built-in TTS.</returns>
      public static bool hasBuiltInTTS
      {
         get
         {
            return isWindowsBasedPlatform || isAppleBasedPlatform || isAndroidPlatform || isLinuxPlatform ||
                   Speaker.isCustomMode && Speaker.CustomVoiceProvider != null &&
                   Speaker.CustomVoiceProvider.isPlatformSupported;
         }
      }

      /// <summary>The current provider type.</summary>
      /// <returns>Current provider type.</returns>
      public static Model.Enum.ProviderType CurrentProviderType
      {
         get
         {
            if (Speaker.isMaryMode)
               return Model.Enum.ProviderType.MaryTTS;

            if (isWindowsPlatform && !Speaker.isESpeakMode)
               return Model.Enum.ProviderType.Windows;

            if (isAndroidPlatform)
               return Model.Enum.ProviderType.Android;

            if (isIOSBasedPlatform)
               return Model.Enum.ProviderType.iOS;

            if (isWSABasedPlatform)
               return Model.Enum.ProviderType.WSA;

            if (isMacOSPlatform && !Speaker.isESpeakMode)
               return Model.Enum.ProviderType.macOS;

            return Model.Enum.ProviderType.Linux;
         }
      }

      #endregion


      #region Static methods

      /// <summary>Converts a string to a Gender.</summary>
      /// <param name="gender">Gender as text.</param>
      /// <returns>Gender from the given string.</returns>
      public static Model.Enum.Gender StringToGender(string gender)
      {
         if ("male".CTEquals(gender) || "m".CTEquals(gender))
         {
            return Model.Enum.Gender.MALE;
         }

         if ("female".CTEquals(gender) || "f".CTEquals(gender))
         {
            return Model.Enum.Gender.FEMALE;
         }

         return Model.Enum.Gender.UNKNOWN;
      }

      /// <summary>Converts an Apple voice name to a Gender.</summary>
      /// <param name="voiceName">Voice name.</param>
      /// <returns>Gender from the given Apple voice name.</returns>
      public static Model.Enum.Gender AppleVoiceNameToGender(string voiceName)
      {
         if (!string.IsNullOrEmpty(voiceName))
         {
            if (appleFemales.Any(female => voiceName.CTContains(female)))
            {
               return Model.Enum.Gender.FEMALE;
            }

            if (appleMales.Any(male => voiceName.CTContains(male)))
            {
               return Model.Enum.Gender.MALE;
            }
         }

         return Model.Enum.Gender.UNKNOWN;
      }

      /// <summary>Converts an WSA voice name to a Gender.</summary>
      /// <param name="voiceName">Voice name.</param>
      /// <returns>Gender from the given WSA voice name.</returns>
      public static Model.Enum.Gender WSAVoiceNameToGender(string voiceName)
      {
         if (!string.IsNullOrEmpty(voiceName))
         {
            if (wsaFemales.Any(female => voiceName.CTContains(female)))
            {
               return Model.Enum.Gender.FEMALE;
            }

            if (wsaMales.Any(male => voiceName.CTContains(male)))
            {
               return Model.Enum.Gender.MALE;
            }
         }

         return Model.Enum.Gender.UNKNOWN;
      }

      /// <summary>Cleans a given text to contain only letters or digits.</summary>
      /// <param name="text">Text to clean.</param>
      /// <param name="removeTags">Removes tags from text (default: true, optional).</param>
      /// <param name="clearSpaces">Clears multiple spaces from text (default: true, optional).</param>
      /// <param name="clearLineEndings">Clears line endings from text (default: true, optional).</param>
      /// <returns>Clean text with only letters and digits.</returns>
      public static string CleanText(string text, bool removeTags = true, bool clearSpaces = true, bool clearLineEndings = true)
      {
         string result = text;

         if (removeTags)
         {
            result = ClearTags(result);
         }

         if (clearSpaces)
         {
            result = ClearSpaces(result);
         }

         if (clearLineEndings)
         {
            result = ClearLineEndings(result);
         }

         return result;
      }

      /// <summary>Marks the current word or all spoken words from a given text array.</summary>
      /// <param name="speechTextArray">Array with all text fragments</param>
      /// <param name="wordIndex">Current word index</param>
      /// <param name="markAllSpokenWords">Mark the spoken words (default: false, optional)</param>
      /// <param name="markPrefix">Prefix for every marked word (default: green, optional)</param>
      /// <param name="markPostfix">Postfix for every marked word (default: green, optional)</param>
      /// <returns>Marked current word or all spoken words.</returns>
      public static string MarkSpokenText(string[] speechTextArray, int wordIndex, bool markAllSpokenWords = false, string markPrefix = "<color=green><b>", string markPostfix = "</b></color>")
      {
         System.Text.StringBuilder sb = new System.Text.StringBuilder();

         if (speechTextArray == null)
         {
            Debug.LogWarning("The given 'speechTextArray' is null!");
         }
         else
         {
            if (wordIndex < 0 || wordIndex > speechTextArray.Length - 1)
            {
               Debug.LogWarning("The given 'wordIndex' is invalid: " + wordIndex);
            }
            else
            {
               for (int ii = 0; ii < wordIndex; ii++)
               {
                  if (markAllSpokenWords)
                     sb.Append(markPrefix);
                  sb.Append(speechTextArray[ii]);
                  if (markAllSpokenWords)
                     sb.Append(markPostfix);
                  sb.Append(" ");
               }

               sb.Append(markPrefix);
               sb.Append(speechTextArray[wordIndex]);
               sb.Append(markPostfix);
               sb.Append(" ");

               for (int ii = wordIndex + 1; ii < speechTextArray.Length; ii++)
               {
                  sb.Append(speechTextArray[ii]);
                  sb.Append(" ");
               }
            }
         }

         return sb.ToString();
      }

      #endregion
   }
}
// © 2015-2020 crosstales LLC (https://www.crosstales.com)