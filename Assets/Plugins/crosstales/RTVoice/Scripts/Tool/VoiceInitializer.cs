using UnityEngine;
using System.Collections;
using System.Linq;

namespace Crosstales.RTVoice.Tool
{
   /// <summary>Allows to initialize voices (useful on Android).</summary>
   [HelpURL("https://crosstales.com/media/data/assets/rtvoice/api/class_crosstales_1_1_r_t_voice_1_1_tool_1_1_voice_initalizer.html")] //TODO update
   public class VoiceInitializer : MonoBehaviour
   {
      #region Variables

      /// <summary>Selected provider to initialize the voices (default: Android).</summary>
      [Tooltip("Selected provider to initialize the voices (default: Android).")] public Model.Enum.ProviderType Provider = Model.Enum.ProviderType.Android;

      /// <summary>Initialize voices by name.</summary>
      [Tooltip("Initialize voices by name.")] public string[] VoiceNames;

      /// <summary>Initialize all voices (default: false).</summary>
      [Tooltip("Initialize all voices (default: false).")] public bool AllVoices = false;


      /// <summary>Destroy the gameobject after initialize (default: true).</summary>
      [Header("Behaviour Settings")] [Tooltip("Destroy the gameobject after initialize (default: true).")] public bool DestroyWhenFinished = true;

      private string activeUid = string.Empty;
      private string completedUid = string.Empty;

      private const string text = "crosstales";

      #endregion


      #region MonoBehaviour methods

      public void Start()
      {
         if (!Util.Helper.isEditorMode)
         {
            DontDestroyOnLoad(transform.root.gameObject);
         }
      }

      public void OnEnable()
      {
         Speaker.OnVoicesReady += onVoicesReady;
         Speaker.OnSpeakComplete += onSpeakComplete;
      }

      public void OnDisable()
      {
         Speaker.OnVoicesReady -= onVoicesReady;
         Speaker.OnSpeakComplete -= onSpeakComplete;
      }

      #endregion


      #region Private methods

      private IEnumerator initalizeVoices()
      {
#if !UNITY_STANDALONE_OSX && !UNITY_EDITOR_OSX
         if (AllVoices)
         {
            foreach (Model.Voice voice in Speaker.Voices)
            {
               activeUid = Speaker.SpeakNative(text, voice, 3, 1, 0);

               do
               {
                  yield return null;
               } while (!activeUid.Equals(completedUid));
            }
         }
         else
         {
            foreach (var voice in from voiceName in VoiceNames
               where !string.IsNullOrEmpty(voiceName)
               where Speaker.isVoiceForNameAvailable(voiceName)
               select Speaker.VoiceForName(voiceName))
            {
               activeUid = Speaker.SpeakNative(text, voice, 3, 1, 0);

               do
               {
                  yield return null;
               } while (!activeUid.Equals(completedUid));
            }
         }

         if (DestroyWhenFinished)
            Destroy(gameObject);
#else
         yield return null;
#endif
      }

      #endregion


      #region Callbacks

      private void onVoicesReady()
      {
         if (Provider == Util.Helper.CurrentProviderType)
         {
            StopAllCoroutines();
            StartCoroutine(initalizeVoices());
         }
         else
         {
            if (DestroyWhenFinished)
               Destroy(gameObject);
         }
      }

      private void onSpeakComplete(Model.Wrapper wrapper)
      {
         completedUid = wrapper.Uid;
      }

      #endregion
   }
}
// © 2017-2020 crosstales LLC (https://www.crosstales.com)