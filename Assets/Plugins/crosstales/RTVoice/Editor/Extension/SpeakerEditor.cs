#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Crosstales.RTVoice.EditorUtil;

namespace Crosstales.RTVoice.EditorExtension
{
   /// <summary>Custom editor for the 'Speaker'-class.</summary>
   [InitializeOnLoad]
   [CustomEditor(typeof(Speaker))]
   public class SpeakerEditor : Editor
   {
      #region Variables

      private int voiceIndex;
      private float rate = 1f;
      private float pitch = 1f;
      private float volume = 1f;
      private Speaker script;

      private bool showVoices = false;

      private Object customProvider;
      private bool customMode;

      private bool maryTTSMode;
      private string maryTTSUrl;
      private int maryTTSPort;
      private string maryTTSUser;
      private string maryTTSPassword;
      private Model.Enum.MaryTTSType maryTTSType;

      private bool eSpeakMode;
      private Model.Enum.ESpeakModifiers eSpeakModifier;

      private bool autoClearTags;

      //private bool windowsLegacy;
      private bool wsaNative;
      private bool silenceOnDisable;
      private bool silenceOnFocusLost;
      private bool dontDestroy;

      #endregion


      #region Static constructor

      static SpeakerEditor()
      {
         EditorApplication.hierarchyWindowItemOnGUI += hierarchyItemCB;
      }

      #endregion


      #region Editor methods

      public void OnEnable()
      {
         script = (Speaker)target;
      }

      public void OnDisable()
      {
         if (Util.Helper.isEditorMode)
         {
            Speaker.Silence();
         }
      }

      public override bool RequiresConstantRepaint()
      {
         return true;
      }

      public override void OnInspectorGUI()
      {
         EditorHelper.BannerOC();

         if (Speaker.enforcedStandaloneTTS)
         {
            EditorGUILayout.HelpBox("Standalone TTS is used for development. The TTS on the current build target may have other voices and features.", MessageType.Warning);
         }

         if (Speaker.Voices.Count == 0)
         {
            if (Speaker.isPlatformSupported && !Speaker.isWorkingInPlaymode)
            {
               EditorGUILayout.HelpBox("The current TTS only works in builds!", MessageType.Error);
            }
            else if (!Speaker.isPlatformSupported)
            {
               EditorGUILayout.HelpBox("The current platform is not supported by the active voice provider. Please use MaryTTS or a custom provider (e.g. Klattersynth).", MessageType.Error);
            }
            else
            {
               if (Speaker.hasVoicesInEditor)
                  EditorGUILayout.HelpBox("TTS with the current settings is not possible!", MessageType.Error);
            }
         }

         if (Util.Helper.isIL2CPP && !Speaker.isIL2CPPSupported)
         {
            GUILayout.Space(6);
            EditorGUILayout.HelpBox("IL2CPP is not supported by the current voice provider. Please use Mono, MaryTTS or a custom provider (e.g. Klattersynth).", MessageType.Error);
         }

         serializedObject.Update();

         GUILayout.Label("Custom Provider", EditorStyles.boldLabel);

         customMode = EditorGUILayout.BeginToggleGroup(new GUIContent("Active", "Enables or disables the custom provider (default: false)."), script.CustomMode);
         if (customMode != script.CustomMode)
         {
            serializedObject.FindProperty("CustomMode").boolValue = customMode;
            serializedObject.ApplyModifiedProperties();

            voiceIndex = 0;

            Speaker.ReloadProvider();
         }

         EditorGUI.indentLevel++;

         customProvider = EditorGUILayout.ObjectField("Custom Provider", script.CustomProvider, typeof(Provider.BaseCustomVoiceProvider), true);
         if (customProvider != script.CustomProvider)
         {
            serializedObject.FindProperty("CustomProvider").objectReferenceValue = customProvider;
            serializedObject.ApplyModifiedProperties();

            voiceIndex = 0;

            Speaker.ReloadProvider();
         }

         EditorGUI.indentLevel--;
         EditorGUILayout.EndToggleGroup();

         if (customMode)
         {
            if (script.CustomProvider == null)
            {
               EditorGUILayout.HelpBox("'Custom Provider' is null! Please add a valid provider.", MessageType.Warning);
            }
            else
            {
               if (!script.CustomProvider.isPlatformSupported)
               {
                  EditorGUILayout.HelpBox("'Custom Provider' does not support the current platform!", MessageType.Warning);
               }
            }
         }

         GUILayout.Space(8);
         GUILayout.Label("MaryTTS", EditorStyles.boldLabel);

         maryTTSMode = EditorGUILayout.BeginToggleGroup(new GUIContent("Active", "Enables or disables MaryTTS (default: false)."), script.MaryTTSMode);
         if (maryTTSMode != script.MaryTTSMode)
         {
            serializedObject.FindProperty("MaryTTSMode").boolValue = maryTTSMode;
            serializedObject.ApplyModifiedProperties();

            voiceIndex = 0;

            Speaker.ReloadProvider();
         }

         EditorGUI.indentLevel++;

         maryTTSUrl = EditorGUILayout.TextField(new GUIContent("URL", "Server URL for MaryTTS."), script.MaryTTSUrl);
         if (!maryTTSUrl.Equals(script.MaryTTSUrl))
         {
            serializedObject.FindProperty("MaryTTSUrl").stringValue = maryTTSUrl;
            serializedObject.ApplyModifiedProperties();

            Speaker.ReloadProvider();
         }

         maryTTSPort = EditorGUILayout.IntSlider("Port", script.MaryTTSPort, 0, 65535);
         if (maryTTSPort != script.MaryTTSPort)
         {
            serializedObject.FindProperty("MaryTTSPort").intValue = maryTTSPort;
            serializedObject.ApplyModifiedProperties();

            //Speaker.ReloadProvider();
         }

         maryTTSUser = EditorGUILayout.TextField(new GUIContent("User", "Username for MaryTTS (default: empty)."), script.MaryTTSUser);
         if (!maryTTSUser.Equals(script.MaryTTSUser))
         {
            serializedObject.FindProperty("MaryTTSUser").stringValue = maryTTSUser;
            serializedObject.ApplyModifiedProperties();

            Speaker.ReloadProvider();
         }

         maryTTSPassword = EditorGUILayout.PasswordField(new GUIContent("Password", "User password for MaryTTS (default: empty)."), script.MaryTTSPassword);
         if (!maryTTSPassword.Equals(script.MaryTTSPassword))
         {
            serializedObject.FindProperty("MaryTTSPassword").stringValue = maryTTSPassword;
            serializedObject.ApplyModifiedProperties();

            Speaker.ReloadProvider();
         }

         maryTTSType = (Model.Enum.MaryTTSType)EditorGUILayout.EnumPopup(new GUIContent("Type", "Input type for MaryTTS (default: MaryTTSType.RAWMARYXML)."), script.MaryTTSType);
         if (maryTTSType != script.MaryTTSType)
         {
            serializedObject.FindProperty("MaryTTSType").enumValueIndex = (int)maryTTSType;
            serializedObject.ApplyModifiedProperties();

            Speaker.ReloadProvider();
         }

         EditorGUI.indentLevel--;
         EditorGUILayout.EndToggleGroup();

         if (maryTTSMode)
         {
            if (string.IsNullOrEmpty(maryTTSUrl))
            {
               EditorGUILayout.HelpBox("'URL' is null or empty! Please add a valid MaryTTS-server.", MessageType.Warning);
            }
            else
            {
               if (maryTTSUrl.Contains("mary.dfki.de") || maryTTSUrl.Contains("crosstales.com") || maryTTSUrl.Contains("46.101.111.65"))
               {
                  EditorGUILayout.HelpBox("You are using the test server of MaryTTS. Please setup your own server from 'http://mary.dfki.de'.", MessageType.Warning);
               }
            }
         }

         GUILayout.Space(8);
         GUILayout.Label("eSpeak Settings", EditorStyles.boldLabel);

         eSpeakMode = EditorGUILayout.BeginToggleGroup(new GUIContent("Active", "Enable or disable eSpeak for standalone platforms (default: false)."), script.ESpeakMode);
         if (eSpeakMode != script.ESpeakMode)
         {
            serializedObject.FindProperty("ESpeakMode").boolValue = eSpeakMode;
            serializedObject.ApplyModifiedProperties();

            voiceIndex = 0;

            Speaker.ReloadProvider();
         }

         EditorGUI.indentLevel++;

         eSpeakModifier = (Model.Enum.ESpeakModifiers)EditorGUILayout.EnumPopup(new GUIContent("Modifier", "Active modifier for all eSpeak voices (default: none, m1-m6 = male, f1-f4 = female)."), script.ESpeakModifier);
         if (eSpeakModifier != script.ESpeakModifier)
         {
            serializedObject.FindProperty("ESpeakModifier").enumValueIndex = (int)eSpeakModifier;
            serializedObject.ApplyModifiedProperties();
         }

         EditorGUI.indentLevel--;
         EditorGUILayout.EndToggleGroup();

         if (eSpeakMode && !Provider.VoiceProviderLinux.isSupported)
         {
            EditorGUILayout.HelpBox("'eSpeak' is not supported on the current platform!", MessageType.Warning);
         }

         GUILayout.Space(8);
         GUILayout.Label("Advanced Settings", EditorStyles.boldLabel);

         autoClearTags = EditorGUILayout.Toggle(new GUIContent("Auto Clear Tags", "Automatically clear tags from speeches depending on the capabilities of the current TTS-system (default: false)."), script.AutoClearTags);
         if (autoClearTags != script.AutoClearTags)
         {
            serializedObject.FindProperty("AutoClearTags").boolValue = autoClearTags;
            serializedObject.ApplyModifiedProperties();
         }

         /*
         windowsLegacy = EditorGUILayout.Toggle(new GUIContent("Windows: Legacy", "Enable or disable the legacy Windows provider (default: true)."), script.WindowsLegacy);
         if (windowsLegacy != script.WindowsLegacy)
         {
             serializedObject.FindProperty("WindowsLegacy").boolValue = windowsLegacy;
             serializedObject.ApplyModifiedProperties();

             voiceIndex = 0;

             Speaker.ReloadProvider();
         }
         */

         bool wsaActive = (Util.Helper.isEditor || Util.Helper.isWSABasedPlatform) && (!Util.Helper.isIL2CPP || !Util.Helper.isWSABasedPlatform);

         GUI.enabled = wsaActive;

         wsaNative = EditorGUILayout.Toggle(new GUIContent("WSA: Native", "Enable or disable native speak under WSA. If enabled, the build type must be 'XAML' and '.NET'! (default: false)"), script.WSANative);

         if (!wsaActive && wsaNative)
            wsaNative = false;

         if (wsaNative != script.WSANative)
         {
            serializedObject.FindProperty("WSANative").boolValue = wsaNative;
            serializedObject.ApplyModifiedProperties();
         }

         GUI.enabled = true;

         GUILayout.Space(8);
         GUILayout.Label("Behaviour Settings", EditorStyles.boldLabel);

         silenceOnDisable = EditorGUILayout.Toggle(new GUIContent("Silence On Disable", "Silence any speeches if this component gets disabled (default: false)."), script.SilenceOnDisable);
         if (silenceOnDisable != script.SilenceOnDisable)
         {
            serializedObject.FindProperty("SilenceOnDisable").boolValue = silenceOnDisable;
            serializedObject.ApplyModifiedProperties();
         }

         silenceOnFocusLost = EditorGUILayout.Toggle(new GUIContent("Silence On Focus Lost", "Silence any speeches if the application loses the focus (default: true)."), script.SilenceOnFocusLost);
         if (silenceOnFocusLost != script.SilenceOnFocusLost)
         {
            serializedObject.FindProperty("SilenceOnFocusLost").boolValue = silenceOnFocusLost;
            serializedObject.ApplyModifiedProperties();
         }

         dontDestroy = EditorGUILayout.Toggle(new GUIContent("Dont Destroy", "Don't destroy gameobject during scene switches (default: true)."), script.DontDestroy);
         if (dontDestroy != script.DontDestroy)
         {
            serializedObject.FindProperty("DontDestroy").boolValue = dontDestroy;
            serializedObject.ApplyModifiedProperties();
         }

         EditorHelper.SeparatorUI();

         if (script.isActiveAndEnabled)
         {
            GUILayout.Label("Data", EditorStyles.boldLabel);

            showVoices = EditorGUILayout.Foldout(showVoices, "Voices (" + Speaker.Voices.Count + ")");
            if (showVoices)
            {
               EditorGUI.indentLevel++;

               foreach (string voice in Speaker.Voices.CTToString())
               {
                  EditorGUILayout.SelectableLabel(voice, GUILayout.Height(16), GUILayout.ExpandHeight(false));
               }

               EditorGUI.indentLevel--;
            }

            if (GUILayout.Button(new GUIContent(" Reload", EditorHelper.Icon_Refresh, "Reload the provider.")))
            {
               Speaker.ReloadProvider();
            }

            EditorHelper.SeparatorUI();

            GUILayout.Label("Test-Drive", EditorStyles.boldLabel);

            if (Speaker.Voices.Count > 0)
            {
               //EditorHelper.SeparatorUI();

               //GUILayout.Label("Test-Drive", EditorStyles.boldLabel);

               if (Util.Helper.isEditorMode)
               {
                  if (Speaker.isWorkingInEditor)
                  {
                     voiceIndex = EditorGUILayout.Popup("Voice", voiceIndex, Speaker.Voices.CTToString().ToArray());
                     rate = EditorGUILayout.Slider("Rate", rate, 0f, 3f);

                     if (Util.Helper.isWindowsPlatform)
                     {
                        pitch = EditorGUILayout.Slider("Pitch", pitch, 0f, 2f);

                        volume = EditorGUILayout.Slider("Volume", volume, 0f, 1f);
                     }

                     GUILayout.Space(8);

                     if (Speaker.isSpeaking)
                     {
                        if (GUILayout.Button(new GUIContent(" Silence", EditorHelper.Icon_Silence, "Silence all active speakers.")))
                        {
                           Speaker.Silence();
                        }
                     }
                     else
                     {
                        if (GUILayout.Button(new GUIContent(" Speak", EditorHelper.Icon_Speak, "Speaks the text with the selected voice and settings.")))
                        {
                           Speaker.SpeakNative("You have selected " + Speaker.Voices[voiceIndex].Name, Speaker.Voices[voiceIndex], rate, pitch, volume);
                        }
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
            else
            {
               if (Util.Helper.isEditorMode)
               {
                  if (!Speaker.isWorkingInEditor)
                  {
                     EditorGUILayout.HelpBox("Test-Drive is not supported for the current TTS-system.", MessageType.Info);
                  }
               }
               else
               {
                  EditorGUILayout.HelpBox("Disabled in Play-mode!", MessageType.Info);
               }
            }

            /*
            else
            {
                if (Speaker.isPlatformSupported && !Speaker.isWorkingInPlaymode)
                {
                    EditorGUILayout.HelpBox("The current TTS only works in builds!", MessageType.Error);
                }
                else if (!Speaker.isPlatformSupported)
                {
                    EditorGUILayout.HelpBox("The current platform is not supported by the active voice provider. Please use MaryTTS or a custom provider (e.g. Klattersynth).", MessageType.Error);
                }
                else
                {
                    EditorGUILayout.HelpBox("TTS with the current settings is not possible!", MessageType.Error);
                }
            }
            */

            EditorHelper.SeparatorUI();

            GUILayout.Label("Information", EditorStyles.boldLabel);

            GUILayout.Label("Speech count:\t" + Speaker.SpeechCount);
         }
         else
         {
            EditorGUILayout.HelpBox("Script is disabled!", MessageType.Info);
         }

         serializedObject.ApplyModifiedProperties();
      }

      #endregion


      #region Private methods

      private static void hierarchyItemCB(int instanceID, Rect selectionRect)
      {
         if (EditorConfig.HIERARCHY_ICON)
         {
            GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

            if (go != null && go.GetComponent<Speaker>())
            {
               Rect r = new Rect(selectionRect);
               r.x = r.width - 4;

               //Debug.Log("HierarchyItemCB: " + r);

               GUI.Label(r, EditorHelper.Logo_Asset_Small);
            }
         }
      }

      #endregion
   }
}
#endif
// © 2016-2020 crosstales LLC (https://www.crosstales.com)