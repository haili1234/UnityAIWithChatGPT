#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Crosstales.RTVoice.EditorTask;
using Crosstales.RTVoice.EditorUtil;

namespace Crosstales.RTVoice.EditorIntegration
{
   /// <summary>Base class for editor windows.</summary>
   public abstract class ConfigBase : EditorWindow
   {
      #region Variables

      private static string updateText = UpdateCheck.TEXT_NOT_CHECKED;
      private static UpdateStatus updateStatus = UpdateStatus.NOT_CHECKED;

      private System.Threading.Thread worker;

      private Vector2 scrollPosConfig;
      private Vector2 scrollPosHelp;
      private Vector2 scrollPosAboutUpdate;
      private Vector2 scrollPosAboutReadme;
      private Vector2 scrollPosAboutVersions;

      private static string readme;
      private static string versions;
      private static string ssml;
      private static string emotionml;

      private bool enforceStandaloneTTS;

      private int aboutTab = 0;

      private static readonly System.Random rnd = new System.Random();

      private readonly int adRnd1 = rnd.Next(0, 4);
      //private bool isAdRnd1 = false;

      private readonly int adRnd2 = rnd.Next(0, 4);
      //private bool isAdRnd2 = false;

      private const int maxChars = 16000;

      #endregion


      #region Protected methods

      protected void showConfiguration()
      {
         EditorHelper.BannerOC();

         GUI.skin.label.wordWrap = true;

         scrollPosConfig = EditorGUILayout.BeginScrollView(scrollPosConfig, false, false);
         {
            GUILayout.Label("General Settings", EditorStyles.boldLabel);

            EditorConfig.PREFAB_AUTOLOAD = EditorGUILayout.Toggle(new GUIContent("Prefab Auto-Load", "Enable or disable auto-loading of the prefabs to the scene (default: " + EditorConstants.DEFAULT_PREFAB_AUTOLOAD + ")."), EditorConfig.PREFAB_AUTOLOAD);

            Util.Config.AUDIOFILE_PATH = EditorGUILayout.TextField(new GUIContent("Audio Path", "Path to the generated audio files (default: '" + Util.Constants.DEFAULT_AUDIOFILE_PATH + "')."), Util.Config.AUDIOFILE_PATH);
            Util.Config.AUDIOFILE_AUTOMATIC_DELETE = EditorGUILayout.Toggle(new GUIContent("Audio Auto-Delete", "Enable or disable auto-delete of the generated audio files (default: " + Util.Constants.DEFAULT_AUDIOFILE_AUTOMATIC_DELETE + ")."), Util.Config.AUDIOFILE_AUTOMATIC_DELETE);

            Util.Config.DEBUG = EditorGUILayout.Toggle(new GUIContent("Debug", "Enable or disable debug logs (default: " + Util.Constants.DEFAULT_DEBUG + ")."), Util.Config.DEBUG);

            EditorConfig.UPDATE_CHECK = EditorGUILayout.Toggle(new GUIContent("Update Check", "Enable or disable the update-check (default: " + EditorConstants.DEFAULT_UPDATE_CHECK + ")"), EditorConfig.UPDATE_CHECK);

            EditorHelper.SeparatorUI();

            GUILayout.Label("Speaker", EditorStyles.boldLabel);
            EditorConfig.HIERARCHY_ICON = EditorGUILayout.Toggle(new GUIContent("Show Hierarchy Icon", "Show hierarchy icon (default: " + EditorConstants.DEFAULT_HIERARCHY_ICON + ")."), EditorConfig.HIERARCHY_ICON);
            Util.Config.ENSURE_NAME = EditorGUILayout.Toggle(new GUIContent("Ensure Name", "Ensure the name of the RTVoice gameobject (default: " + Util.Constants.DEFAULT_ENSURE_NAME + ")."), Util.Config.ENSURE_NAME);

            EditorHelper.SeparatorUI();
            GUILayout.Label("Development Settings", EditorStyles.boldLabel);
            enforceStandaloneTTS = EditorGUILayout.Toggle(new GUIContent("Enforce Standalone TTS", "Enforce standalone TTS for development (default: " + Util.Constants.DEFAULT_ENFORCE_STANDALONE_TTS + ")."), Util.Config.ENFORCE_STANDALONE_TTS);
            if (enforceStandaloneTTS != Util.Config.ENFORCE_STANDALONE_TTS)
            {
               Util.Config.ENFORCE_STANDALONE_TTS = enforceStandaloneTTS;
               Speaker.ReloadProvider();
            }

            EditorHelper.SeparatorUI();
            GUILayout.Label("Windows Settings", EditorStyles.boldLabel);
            Util.Config.ENFORCE_32BIT_WINDOWS = EditorGUILayout.Toggle(new GUIContent("Enforce 32bit Voices", "Enforce 32bit versions of voices under Windows (default: " + Util.Constants.DEFAULT_ENFORCE_32BIT_WINDOWS + ")."), Util.Config.ENFORCE_32BIT_WINDOWS);
         }
         EditorGUILayout.EndScrollView();
      }


      protected void showHelp()
      {
         EditorHelper.BannerOC();

         scrollPosHelp = EditorGUILayout.BeginScrollView(scrollPosHelp, false, false);
         {
            GUILayout.Label("Resources", EditorStyles.boldLabel);

            //GUILayout.Space(8);

            GUILayout.BeginHorizontal();
            {
               GUILayout.BeginVertical();
               {
                  if (GUILayout.Button(new GUIContent(" Manual", EditorHelper.Icon_Manual, "Show the manual.")))
                  {
                     Application.OpenURL(Util.Constants.ASSET_MANUAL_URL);
                  }

                  GUILayout.Space(6);

                  if (GUILayout.Button(new GUIContent(" Forum", EditorHelper.Icon_Forum, "Visit the forum page.")))
                  {
                     Application.OpenURL(Util.Constants.ASSET_FORUM_URL);
                  }
               }
               GUILayout.EndVertical();

               GUILayout.BeginVertical();
               {
                  if (GUILayout.Button(new GUIContent(" API", EditorHelper.Icon_API, "Show the API.")))
                  {
                     Application.OpenURL(Util.Constants.ASSET_API_URL);
                  }

                  GUILayout.Space(6);

                  if (GUILayout.Button(new GUIContent(" Product", EditorHelper.Icon_Product, "Visit the product page.")))
                  {
                     Application.OpenURL(Util.Constants.ASSET_WEB_URL);
                  }
               }
               GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();

            EditorHelper.SeparatorUI();

            GUILayout.Label("Videos", EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();
            {
               if (GUILayout.Button(new GUIContent(" Promo", EditorHelper.Video_Promo, "View the promotion video on 'Youtube'.")))
               {
                  Application.OpenURL(Util.Constants.ASSET_VIDEO_PROMO);
               }

               if (GUILayout.Button(new GUIContent(" Tutorial", EditorHelper.Video_Tutorial, "View the tutorial video on 'Youtube'.")))
               {
                  Application.OpenURL(Util.Constants.ASSET_VIDEO_TUTORIAL);
               }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(6);

            if (GUILayout.Button(new GUIContent(" All Videos", EditorHelper.Icon_Videos, "Visit our 'Youtube'-channel for more videos.")))
            {
               Application.OpenURL(Util.Constants.ASSET_SOCIAL_YOUTUBE);
            }

            EditorHelper.SeparatorUI();

            GUILayout.Label("3rd Party Assets", EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();
            {
               if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Asset_PlayMaker, "More information about 'PlayMaker'.")))
               {
                  Application.OpenURL(Util.Constants.ASSET_3P_PLAYMAKER);
               }

               if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Store_AdventureCreator, "More information about 'Adventure Creator'.")))
               {
                  Application.OpenURL(Util.Constants.ASSET_3P_ADVENTURE_CREATOR);
               }

               if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Store_CinemaDirector, "More information about 'Cinema Director'.")))
               {
                  Application.OpenURL(Util.Constants.ASSET_3P_CINEMA_DIRECTOR);
               }

               if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Store_DialogueSystem, "More information about 'Dialogue System'.")))
               {
                  Application.OpenURL(Util.Constants.ASSET_3P_DIALOGUE_SYSTEM);
               }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
               if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Store_LDC, "More information about 'Localized Dialogs'.")))
               {
                  Application.OpenURL(Util.Constants.ASSET_3P_LOCALIZED_DIALOGS);
               }

               if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Store_LipSync, "More information about 'LipSync'.")))
               {
                  Application.OpenURL(Util.Constants.ASSET_3P_LIPSYNC);
               }

               if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Store_NPC_Chat, "More information about 'NPC Chat'.")))
               {
                  Application.OpenURL(Util.Constants.ASSET_3P_NPC_CHAT);
               }

               if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Store_QuestSystem, "More information about 'Quest System'.")))
               {
                  Application.OpenURL(Util.Constants.ASSET_3P_QUEST_SYSTEM);
               }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
               if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Store_SALSA, "More information about 'SALSA'.")))
               {
                  Application.OpenURL(Util.Constants.ASSET_3P_SALSA);
               }

               if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Store_SLATE, "More information about 'SLATE'.")))
               {
                  Application.OpenURL(Util.Constants.ASSET_3P_SLATE);
               }

               if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Store_Amplitude, "More information about 'Amplitude'.")))
               {
                  Application.OpenURL(Util.Constants.ASSET_3P_AMPLITUDE);
               }

               if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Store_Klattersynth, "More information about 'Klattersynth'.")))
               {
                  Application.OpenURL(Util.Constants.ASSET_3P_KLATTERSYNTH);
               }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
               if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Store_WebGL, "More information about 'WebGL Speech Synthesis'.")))
               {
                  Application.OpenURL(Util.Constants.ASSET_3P_WEBGL);
               }

               if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Store_Google, "More information about 'Google Cloud Text To Speech'.")))
               {
                  Application.OpenURL(Util.Constants.ASSET_3P_GOOGLE);
               }

               switch (adRnd1)
               {
                  /*
              if (!isAdRnd1) {
                  //adRnd1 = rnd.Next(0, 4);
                  adRnd1 = Random.Range(0, 4);
                  isAdRnd1 = true;
                  Debug.LogWarning("adRnd1: " + adRnd1);
              }
              */
                  //CT Ads
                  case 0:
                  {
                     if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Logo_Asset_BWF, "More information about 'Bad Word Filter'.")))
                     {
                        Application.OpenURL(Util.Constants.ASSET_BWF);
                     }

                     break;
                  }
                  case 1:
                  {
                     if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Logo_Asset_DJ, "More information about 'DJ'.")))
                     {
                        Application.OpenURL(Util.Constants.ASSET_DJ);
                     }

                     break;
                  }
                  case 2:
                  {
                     if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Logo_Asset_FB, "More information about 'File Browser'.")))
                     {
                        Application.OpenURL(Util.Constants.ASSET_FB);
                     }

                     break;
                  }
                  default:
                  {
                     if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Logo_Asset_OC, "More information about 'Online Check'.")))
                     {
                        Application.OpenURL(Util.Constants.ASSET_OC);
                     }

                     break;
                  }
               }

               switch (adRnd2)
               {
                  /*
              if (!isAdRnd2)
              {
                  //adRnd2 = rnd.Next(0, 4);
                  adRnd2 = Random.Range(0, 4);
                  isAdRnd2 = true;
                  Debug.LogWarning("adRnd2: " + adRnd2);
              }
              */
                  case 0:
                  {
                     if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Logo_Asset_Radio, "More information about 'Radio'.")))
                     {
                        Application.OpenURL(Util.Constants.ASSET_RADIO);
                     }

                     break;
                  }
                  case 1:
                  {
                     if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Logo_Asset_TB, "More information about 'Turbo Backup'.")))
                     {
                        Application.OpenURL(Util.Constants.ASSET_TB);
                     }

                     break;
                  }
                  case 2:
                  {
                     if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Logo_Asset_TPS, "More information about 'Turbo Switch'.")))
                     {
                        Application.OpenURL(Util.Constants.ASSET_TPS);
                     }

                     break;
                  }
                  default:
                  {
                     if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Logo_Asset_TR, "More information about 'True Random'.")))
                     {
                        Application.OpenURL(Util.Constants.ASSET_TR);
                     }

                     break;
                  }
               }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(6);

            if (GUILayout.Button(new GUIContent(" All Supported Assets", EditorHelper.Icon_3p_Assets, "More information about the all supported assets.")))
            {
               Application.OpenURL(Util.Constants.ASSET_3P_URL);
            }
         }
         EditorGUILayout.EndScrollView();

         GUILayout.Space(6);
      }

      protected void showAbout()
      {
         EditorHelper.BannerOC();

         GUILayout.Space(3);
         GUILayout.Label(Util.Constants.ASSET_NAME, EditorStyles.boldLabel);

         GUILayout.BeginHorizontal();
         {
            GUILayout.BeginVertical(GUILayout.Width(60));
            {
               GUILayout.Label("Version:");

               GUILayout.Space(12);

               GUILayout.Label("Web:");

               GUILayout.Space(2);

               GUILayout.Label("Email:");
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width(170));
            {
               GUILayout.Space(0);

               GUILayout.Label(Util.Constants.ASSET_VERSION);

               GUILayout.Space(12);

               EditorGUILayout.SelectableLabel(Util.Constants.ASSET_AUTHOR_URL, GUILayout.Height(16), GUILayout.ExpandHeight(false));

               GUILayout.Space(2);

               EditorGUILayout.SelectableLabel(Util.Constants.ASSET_CONTACT, GUILayout.Height(16), GUILayout.ExpandHeight(false));
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            {
               //GUILayout.Space(0);
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width(64));
            {
               if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Logo_Asset, "Visit asset website")))
               {
                  Application.OpenURL(EditorConstants.ASSET_URL);
               }
            }
            GUILayout.EndVertical();
         }
         GUILayout.EndHorizontal();

         GUILayout.Label("© 2015-2020 by " + Util.Constants.ASSET_AUTHOR);

         EditorHelper.SeparatorUI();

         GUILayout.BeginHorizontal();
         {
            if (GUILayout.Button(new GUIContent(" AssetStore", EditorHelper.Logo_Unity, "Visit the 'Unity AssetStore' website.")))
            {
               Application.OpenURL(Util.Constants.ASSET_CT_URL);
            }

            if (GUILayout.Button(new GUIContent(" " + Util.Constants.ASSET_AUTHOR, EditorHelper.Logo_CT, "Visit the '" + Util.Constants.ASSET_AUTHOR + "' website.")))
            {
               Application.OpenURL(Util.Constants.ASSET_AUTHOR_URL);
            }
         }
         GUILayout.EndHorizontal();

         EditorHelper.SeparatorUI();

         aboutTab = GUILayout.Toolbar(aboutTab, new[] {"Readme", "Versions", "SSML", "EML", "Update"});

         switch (aboutTab)
         {
            case 4:
            {
               scrollPosAboutUpdate = EditorGUILayout.BeginScrollView(scrollPosAboutUpdate, false, false);
               {
                  Color fgColor = GUI.color;

                  GUI.color = Color.yellow;

                  switch (updateStatus)
                  {
                     case UpdateStatus.NO_UPDATE:
                        GUI.color = Color.green;
                        GUILayout.Label(updateText);
                        break;
                     case UpdateStatus.UPDATE:
                     {
                        GUILayout.Label(updateText);

                        if (GUILayout.Button(new GUIContent(" Download", "Visit the 'Unity AssetStore' to download the latest version.")))
                        {
                           UnityEditorInternal.AssetStore.Open("content/" + EditorConstants.ASSET_ID);
                        }

                        break;
                     }
                     case UpdateStatus.UPDATE_PRO:
                     {
                        GUILayout.Label(updateText);

                        if (GUILayout.Button(new GUIContent(" Upgrade", "Upgrade to the PRO-version in the 'Unity AssetStore'.")))
                        {
                           Application.OpenURL(Util.Constants.ASSET_PRO_URL);
                        }

                        break;
                     }
                     case UpdateStatus.V2019:
                     {
                        GUILayout.Label(updateText);

                        if (GUILayout.Button(new GUIContent(" Upgrade", "Upgrade to the 2019-version in the 'Unity AssetStore'.")))
                        {
                           Application.OpenURL(Util.Constants.ASSET_PRO_URL);
                        }

                        break;
                     }
                     case UpdateStatus.UPDATE_VERSION:
                     {
                        GUILayout.Label(updateText);

                        if (GUILayout.Button(new GUIContent(" Upgrade", "Upgrade to the newer version in the 'Unity AssetStore'")))
                        {
                           Application.OpenURL(Util.Constants.ASSET_CT_URL);
                        }

                        break;
                     }
                     case UpdateStatus.DEPRECATED:
                     {
                        GUILayout.Label(updateText);

                        if (GUILayout.Button(new GUIContent(" More Information", "Visit the 'crosstales'-site for more information.")))
                        {
                           Application.OpenURL(Util.Constants.ASSET_AUTHOR_URL);
                        }

                        break;
                     }
                     default:
                        GUI.color = Color.cyan;
                        GUILayout.Label(updateText);
                        break;
                  }

                  GUI.color = fgColor;
               }
               EditorGUILayout.EndScrollView();

               if (updateStatus == UpdateStatus.NOT_CHECKED || updateStatus == UpdateStatus.NO_UPDATE)
               {
                  bool isChecking = !(worker == null || worker != null && !worker.IsAlive);

                  GUI.enabled = Util.Helper.isInternetAvailable && !isChecking;

                  if (GUILayout.Button(new GUIContent(isChecking ? "Checking... Please wait." : " Check For Update", EditorHelper.Icon_Check, "Checks for available updates of " + Util.Constants.ASSET_NAME)))
                  {
                     worker = new System.Threading.Thread(() => UpdateCheck.UpdateCheckForEditor(out updateText, out updateStatus));
                     worker.Start();
                  }

                  GUI.enabled = true;
               }

               break;
            }
            case 0:
            {
               if (readme == null)
               {
                  string path = Application.dataPath + EditorConfig.ASSET_PATH + "README.txt";

                  try
                  {
                     readme = verifyTextLength(System.IO.File.ReadAllText(path));
                  }
                  catch (System.Exception)
                  {
                     readme = "README not found: " + path;
                  }
               }

               scrollPosAboutReadme = EditorGUILayout.BeginScrollView(scrollPosAboutReadme, false, false);
               {
                  GUILayout.Label(readme);
               }
               EditorGUILayout.EndScrollView();
               break;
            }
            case 1:
            {
               if (versions == null)
               {
                  string path = Application.dataPath + EditorConfig.ASSET_PATH + "Documentation/VERSIONS.txt";

                  try
                  {
                     versions = verifyTextLength(System.IO.File.ReadAllText(path));
                  }
                  catch (System.Exception)
                  {
                     versions = "VERSIONS not found: " + path;
                  }
               }

               scrollPosAboutVersions = EditorGUILayout.BeginScrollView(scrollPosAboutVersions, false, false);
               {
                  GUILayout.Label(versions);
               }

               EditorGUILayout.EndScrollView();
               break;
            }
            case 2:
            {
               if (ssml == null)
               {
                  string path = Application.dataPath + EditorConfig.ASSET_PATH + "Documentation/SSML.txt";

                  try
                  {
                     ssml = verifyTextLength(System.IO.File.ReadAllText(path));
                  }
                  catch (System.Exception)
                  {
                     ssml = "SSML not found: " + path;
                  }
               }

               scrollPosAboutVersions = EditorGUILayout.BeginScrollView(scrollPosAboutVersions, false, false);
               {
                  GUILayout.Label(ssml);
               }

               EditorGUILayout.EndScrollView();
               break;
            }
            default:
            {
               if (emotionml == null)
               {
                  string path = Application.dataPath + EditorConfig.ASSET_PATH + "Documentation/EMOTIONML.txt";

                  try
                  {
                     emotionml = verifyTextLength(System.IO.File.ReadAllText(path));
                  }
                  catch (System.Exception)
                  {
                     emotionml = "EmotionML not found: " + path;
                  }
               }

               scrollPosAboutVersions = EditorGUILayout.BeginScrollView(scrollPosAboutVersions, false, false);
               {
                  GUILayout.Label(emotionml);
               }

               EditorGUILayout.EndScrollView();
               break;
            }
         }

         EditorHelper.SeparatorUI();

         GUILayout.BeginHorizontal();
         {
            if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Social_Discord, "Communicate with us via 'Discord'.")))
            {
               Application.OpenURL(Util.Constants.ASSET_SOCIAL_DISCORD);
            }

            if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Social_Facebook, "Follow us on 'Facebook'.")))
            {
               Application.OpenURL(Util.Constants.ASSET_SOCIAL_FACEBOOK);
            }

            if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Social_Twitter, "Follow us on 'Twitter'.")))
            {
               Application.OpenURL(Util.Constants.ASSET_SOCIAL_TWITTER);
            }

            if (GUILayout.Button(new GUIContent(string.Empty, EditorHelper.Social_Linkedin, "Follow us on 'LinkedIn'.")))
            {
               Application.OpenURL(Util.Constants.ASSET_SOCIAL_LINKEDIN);
            }
         }
         GUILayout.EndHorizontal();

         GUILayout.Space(6);
      }

      private string verifyTextLength(string text)
      {
         string result = text;

         if (text.Length > maxChars)
         {
            result = text.Substring(0, maxChars) + "..." + System.Environment.NewLine + "<--- Content truncated --->";
         }

         return result;
      }

      protected static void save()
      {
         Util.Config.Save();
         EditorConfig.Save();

         if (Util.Config.DEBUG)
            Debug.Log("Config data saved");
      }

      #endregion
   }
}
#endif
// © 2016-2020 crosstales LLC (https://www.crosstales.com)