#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Crosstales.RTVoice.EditorUtil;

namespace Crosstales.RTVoice.EditorIntegration
{
   /// <summary>Editor window extension.</summary>
   [InitializeOnLoad]
   public class ConfigWindow : ConfigBase
   {
      #region Variables

      private int tab = 0;
      private int lastTab = 0;
      private string text = "Test all your voices with different texts and settings.";
      private int voiceIndex;
      private float rate = 1f;
      private float pitch = 1f;
      private float volume = 1f;
      private bool silenced = true;

      private Vector2 scrollPosPrefabs;
      private Vector2 scrollPosTD;

      public delegate void StopPlayback();

      public static event StopPlayback OnStopPlayback;

      #endregion


      #region Static constructor

      static ConfigWindow()
      {
         EditorApplication.update += onEditorUpdate;
      }

      #endregion


      #region EditorWindow methods

      [MenuItem("Tools/" + Util.Constants.ASSET_NAME + "/Configuration...", false, EditorHelper.MENU_ID + 1)]
      public static void ShowWindow()
      {
         GetWindow(typeof(ConfigWindow));
      }

      public static void ShowWindow(int tab)
      {
         ConfigWindow window = GetWindow(typeof(ConfigWindow)) as ConfigWindow;
         if (window != null) window.tab = tab;
      }

      public void OnEnable()
      {
         titleContent = new GUIContent(Util.Constants.ASSET_NAME_SHORT, EditorHelper.Logo_Asset_Small);

         OnStopPlayback += silence;
      }

      public void OnDisable()
      {
         Speaker.Silence();

         OnStopPlayback -= silence;
      }

      public void OnGUI()
      {
         tab = GUILayout.Toolbar(tab, new[] {"Config", "Prefabs", "TD", "Help", "About"});

         if (tab != lastTab)
         {
            lastTab = tab;
            GUI.FocusControl(null);
         }

         switch (tab)
         {
            case 0:
            {
               showConfiguration();

               EditorHelper.SeparatorUI();

               GUILayout.BeginHorizontal();
               {
                  if (GUILayout.Button(new GUIContent(" Save", EditorHelper.Icon_Save, "Saves the configuration settings for this project.")))
                  {
                     save();
                  }

                  if (GUILayout.Button(new GUIContent(" Reset", EditorHelper.Icon_Reset, "Resets the configuration settings for this project.")))
                  {
                     if (EditorUtility.DisplayDialog("Reset configuration?", "Reset the configuration of " + Util.Constants.ASSET_NAME + "?", "Yes", "No"))
                     {
                        Util.Config.Reset();
                        EditorConfig.Reset();
                        save();
                     }
                  }
               }
               GUILayout.EndHorizontal();

               GUILayout.Space(6);
               break;
            }
            case 1:
               showPrefabs();
               break;
            case 2:
               showTestDrive();
               break;
            case 3:
               showHelp();
               break;
            default:
               showAbout();
               break;
         }
      }

      public void OnInspectorUpdate()
      {
         Repaint();
      }

      #endregion


      #region Private methods

      private static void onEditorUpdate()
      {
         if (EditorApplication.isCompiling || EditorApplication.isPlaying || BuildPipeline.isBuildingPlayer)
         {
            onStopPlayback();
         }
      }

      private static void onStopPlayback()
      {
         if (OnStopPlayback != null) OnStopPlayback.Invoke();
      }

      private void silence()
      {
         if (!silenced)
         {
            Speaker.Silence();
            silenced = true;
         }
      }

      private void showPrefabs()
      {
         EditorHelper.BannerOC();

         scrollPosPrefabs = EditorGUILayout.BeginScrollView(scrollPosPrefabs, false, false);
         {
            //GUILayout.Space(8);
            GUILayout.Label("Available Prefabs", EditorStyles.boldLabel);

            GUILayout.Space(6);
            //EditorHelper.SeparatorUI (6);

            GUI.enabled = !EditorHelper.isRTVoiceInScene;

            GUILayout.Label(Util.Constants.RTVOICE_SCENE_OBJECT_NAME);

            if (GUILayout.Button(new GUIContent(" Add", EditorHelper.Icon_Plus, "Adds the '" + Util.Constants.RTVOICE_SCENE_OBJECT_NAME + "'-prefab to the scene.")))
            {
               EditorHelper.InstantiatePrefab(Util.Constants.RTVOICE_SCENE_OBJECT_NAME);
            }

            GUI.enabled = true;

            EditorHelper.SeparatorUI();

            GUILayout.Label("AudioFileGenerator");

            if (GUILayout.Button(new GUIContent(" Add", EditorHelper.Icon_Plus, "Adds a 'AudioFileGenerator'-prefab to the scene.")))
            {
               EditorHelper.InstantiatePrefab("AudioFileGenerator");
            }

            GUILayout.Space(6);

            GUILayout.Label("Paralanguage");

            if (GUILayout.Button(new GUIContent(" Add", EditorHelper.Icon_Plus, "Adds a 'Paralanguage'-prefab to the scene.")))
            {
               EditorHelper.InstantiatePrefab("Paralanguage");
            }

            GUILayout.Space(6);

            GUILayout.Label("Sequencer");

            if (GUILayout.Button(new GUIContent(" Add", EditorHelper.Icon_Plus, "Adds a 'Sequencer'-prefab to the scene.")))
            {
               EditorHelper.InstantiatePrefab("Sequencer");
            }

            GUILayout.Space(6);

            GUILayout.Label("SpeechText");

            if (GUILayout.Button(new GUIContent(" Add", EditorHelper.Icon_Plus, "Adds a 'SpeechText'-prefab to the scene.")))
            {
               EditorHelper.InstantiatePrefab("SpeechText");
            }

            GUILayout.Space(6);

            GUILayout.Label("TextFileSpeaker");

            if (GUILayout.Button(new GUIContent(" Add", EditorHelper.Icon_Plus, "Adds a 'TextFileSpeaker'-prefab to the scene.")))
            {
               EditorHelper.InstantiatePrefab("TextFileSpeaker");
            }

            EditorHelper.SeparatorUI();

            GUILayout.Label("Loudspeaker");

            if (GUILayout.Button(new GUIContent(" Add", EditorHelper.Icon_Plus, "Adds a 'Loudspeaker'-prefab to the scene.")))
            {
               EditorHelper.InstantiatePrefab("Loudspeaker");
            }

            EditorHelper.SeparatorUI();

            GUILayout.Label("VoiceInitializer");

            if (GUILayout.Button(new GUIContent(" Add", EditorHelper.Icon_Plus, "Adds a 'VoiceInitializer'-prefab to the scene.")))
            {
               EditorHelper.InstantiatePrefab("VoiceInitializer");
            }

            GUILayout.Space(6);
         }
         EditorGUILayout.EndScrollView();
      }

      private void showTestDrive()
      {
         EditorHelper.BannerOC();

         GUILayout.Space(3);
         GUILayout.Label("Test-Drive", EditorStyles.boldLabel);

         if (Util.Helper.isEditorMode)
         {
            if (Speaker.isWorkingInEditor)
            {
               if (Speaker.Voices.Count > 0 && EditorHelper.isRTVoiceInScene)
               {
                  scrollPosTD = EditorGUILayout.BeginScrollView(scrollPosTD, false, false);
                  {
                     if (Speaker.isWorkingInEditor)
                     {
                        text = EditorGUILayout.TextField("Text: ", text);

                        voiceIndex = EditorGUILayout.Popup("Voice", voiceIndex, Speaker.Voices.CTToString().ToArray());
                        rate = EditorGUILayout.Slider("Rate", rate, 0f, 3f);

                        if (Util.Helper.isWindowsPlatform)
                        {
                           pitch = EditorGUILayout.Slider("Pitch", pitch, 0f, 2f);

                           volume = EditorGUILayout.Slider("Volume", volume, 0f, 1f);
                        }
                     }
                     else
                     {
                        EditorGUILayout.HelpBox("Test-Drive is not supported for the current TTS-system.", MessageType.Info);
                     }
                  }
                  EditorGUILayout.EndScrollView();


                  EditorHelper.SeparatorUI();

                  GUILayout.BeginHorizontal();
                  {
                     if (GUILayout.Button(new GUIContent(" Speak", EditorHelper.Icon_Speak, "Speaks the text with the selected voice and settings.")))
                     {
                        Speaker.SpeakNative(text, Speaker.Voices[voiceIndex], rate, pitch, volume);
                        silenced = false;
                     }

                     GUI.enabled = Speaker.isSpeaking;
                     if (GUILayout.Button(new GUIContent(" Silence", EditorHelper.Icon_Silence, "Silence all active speakers.")))
                     {
                        silence();
                     }

                     GUI.enabled = true;
                  }
                  GUILayout.EndHorizontal();

                  GUILayout.Space(6);
               }
               else
               {
                  EditorHelper.NoVoicesUI();
               }
            }
            else
            {
               EditorGUILayout.HelpBox("Test-Drive is not supported for the current TTS-system.", MessageType.Info);
            }
         }
         else
         {
            EditorGUILayout.HelpBox("Disabled in Play-mode!", MessageType.Info);
         }
      }

      #endregion
   }
}
#endif
// © 2016-2020 crosstales LLC (https://www.crosstales.com)