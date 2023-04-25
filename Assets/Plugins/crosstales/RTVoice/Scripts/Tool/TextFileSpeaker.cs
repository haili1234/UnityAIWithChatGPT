using UnityEngine;

namespace Crosstales.RTVoice.Tool
{
   /// <summary>Allows to speak text files.</summary>
   [ExecuteInEditMode]
   [HelpURL("https://www.crosstales.com/media/data/assets/rtvoice/api/class_crosstales_1_1_r_t_voice_1_1_tool_1_1_text_file_speaker.html")]
   public class TextFileSpeaker : MonoBehaviour
   {
      #region Variables

      /// <summary>Text files to speak.</summary>
      [Tooltip("Text files to speak.")] public TextAsset[] TextFiles;

      /// <summary>Voices for the speech.</summary>
      [Tooltip("Voices for the speech.")] public Model.VoiceAlias Voices;

      /// <summary>Speak mode (default: 'Speak').</summary>
      [Tooltip("Speak mode (default: 'Speak').")] public Model.Enum.SpeakMode Mode = Model.Enum.SpeakMode.Speak;


      /// <summary>Enable speaking of a random text file on start (default: false).</summary>
      [Header("Behaviour Settings")] [Tooltip("Enable speaking of a random text file on start (default: false).")]
      public bool PlayOnStart = false;

      /// <summary>
      /// Enable speaking of a all random text files on start (default: false).
      /// NOTE: this can only be stopped with the "StopAll"-method
      /// </summary>
      [Tooltip("Enable speaking of a random text file on start (default: false).")] public bool PlayAllOnStart = false;

      /// <summary>Speaks the text files in random order (default: false).</summary>
      [Tooltip("Speaks the text files in random order (default: false).")] public bool SpeakRandom = false;

      /// <summary>Delay until the speech for this text starts (default: 0).</summary>
      [Tooltip("Delay until the speech for this text starts (default: 0).")] public float Delay = 0f;


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

      private string[] texts;
      private string[] randomTexts;

      private int textIndex = -1;
      private int randomTextIndex = -1;

      //private Voice voice;

      private static readonly System.Random rnd = new System.Random();

      private string uid = string.Empty;

      private bool played = false;
      private bool playAll = false;

      private float lastSpeaktime = float.MinValue;

      private int lastNumberOfTextfiles = -1;

      #endregion


      #region Properties

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
         // Subscribe event listeners
         Speaker.OnVoicesReady += onVoicesReady;
         Speaker.OnSpeakComplete += onSpeakComplete;

         Reload();

         play();
      }

      public void Update()
      {
         if (TextFiles.Length != lastNumberOfTextfiles)
            Reload();
      }

      public void OnDestroy()
      {
         //Silence();

         // Unsubscribe event listeners
         Speaker.OnVoicesReady -= onVoicesReady;
         Speaker.OnSpeakComplete -= onSpeakComplete;
      }

      public void OnValidate()
      {
         if (Delay < 0f)
            Delay = 0f;

         Rate = Mathf.Clamp(Rate, 0f, 3f);
         Pitch = Mathf.Clamp(Pitch, 0f, 2f);
         Volume = Mathf.Clamp01(Volume);
      }

      #endregion


      #region Public methods

      /// <summary>Speaks all texts until StopAll is called.</summary>
      public void SpeakAll()
      {
         playAll = true;
         Next();
      }

      /// <summary>Stops speaking all texts.</summary>
      public void StopAll()
      {
         playAll = false;
         Silence();
      }

      /// <summary>Speaks the next text (main use for UI).</summary>
      public void Next()
      {
         Next(SpeakRandom);
      }

      /// <summary>Speaks the next text.</summary>
      /// <param name="random">Speak a random text</param>
      public void Next(bool random)
      {
         int index;

         if (random)
         {
            if (randomTextIndex > -1 && randomTextIndex + 1 < randomTexts.Length)
            {
               randomTextIndex++;
            }
            else
            {
               randomTextIndex = 0;
            }

            index = randomTextIndex;
         }
         else
         {
            if (textIndex > -1 && textIndex + 1 < texts.Length)
            {
               textIndex++;
            }
            else
            {
               textIndex = 0;
            }

            index = textIndex;
         }

         SpeakText(index, random);
      }

      /// <summary>Speaks the previous text (main use for UI).</summary>
      public void Previous()
      {
         Previous(SpeakRandom);
      }

      /// <summary>Speaks the previous text.</summary>
      /// <param name="random">Speak a random text</param>
      public void Previous(bool random)
      {
         int index;

         if (random)
         {
            if (randomTextIndex > 0 && randomTextIndex < randomTexts.Length)
            {
               randomTextIndex--;
            }
            else
            {
               randomTextIndex = randomTexts.Length - 1;
            }

            index = randomTextIndex;
         }
         else
         {
            if (textIndex > 0 && textIndex < texts.Length)
            {
               textIndex--;
            }
            else
            {
               textIndex = texts.Length - 1;
            }

            index = textIndex;
         }

         SpeakText(index, random);
      }

      /// <summary>Speaks a text (main use for UI).</summary>
      public void Speak()
      {
         Next();
      }

      /// <summary>Speaks a text with an optional index.</summary>
      /// <param name="index">Index of the text (default: -1 (random), optional).</param>
      /// <param name="random">Speak a random text (default: false, optional)</param>
      /// <returns>UID of the speaker.</returns>
      public string SpeakText(int index = -1, bool random = false)
      {
         float currentTime = Time.realtimeSinceStartup;

         if (lastSpeaktime + Util.Constants.SPEAK_CALL_SPEED < currentTime)
         {
            lastSpeaktime = currentTime;

            Silence();

            string result = string.Empty;

            if (texts.Length > 0)
            {
               if (random)
               {
                  if (index < 0)
                  {
                     result = speak(randomTexts[rnd.Next(randomTexts.Length)]);
                  }
                  else
                  {
                     if (index < texts.Length)
                     {
                        result = speak(randomTexts[index]);
                     }
                     else
                     {
                        Debug.LogWarning("Text file index is out of bounds: " + index +
                                         " - maximal index is: " + (randomTexts.Length - 1));
                        result = speak(randomTexts[randomTexts.Length - 1]);
                     }
                  }
               }
               else
               {
                  if (index < 0)
                  {
                     result = speak(texts[rnd.Next(texts.Length)]);
                  }
                  else
                  {
                     if (index < texts.Length)
                     {
                        result = speak(texts[index]);
                     }
                     else
                     {
                        Debug.LogWarning("Text file index is out of bounds: " + index +
                                         " - maximal index is: " + (texts.Length - 1));
                        result = speak(texts[texts.Length - 1]);
                     }
                  }
               }
            }
            else
            {
               Debug.LogError("No text files added - speak cancelled!");
            }

            uid = result;
         }
         else
         {
            Debug.LogWarning("'SpeakText' called too fast - please slow down!");
         }

         return uid;
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

      /// <summary>Reloads all text files (e.g. when new text files were added during runtime).</summary>
      public void Reload()
      {
         if (TextFiles.Length > 0)
         {
            texts = new string[TextFiles.Length];
            randomTexts = new string[TextFiles.Length];
            lastNumberOfTextfiles = TextFiles.Length;

            for (int ii = 0; ii < TextFiles.Length; ii++)
            {
               if (TextFiles[ii] != null)
               {
                  randomTexts[ii] = texts[ii] = TextFiles[ii].text;
               }
               else
               {
                  randomTexts[ii] = texts[ii] = string.Empty;
               }
            }

            randomTexts.CTShuffle();

            textIndex = -1;
            randomTextIndex = -1;
         }
      }

      #endregion


      #region Private methods

      private void play()
      {
         if (!Util.Helper.isEditorMode)
         {
            if (!played && Speaker.Voices.Count > 0)
            {
               played = true;

               if (PlayOnStart)
               {
                  //Invoke(nameof(Next), Delay);
                  Invoke("Next", Delay);
               }
               else if (PlayAllOnStart)
               {
                  //Invoke(nameof(SpeakAll), Delay);
                  Invoke("SpeakAll", Delay);
               }
            }
         }
      }

      private string speak(string text)
      {
         return Mode == Model.Enum.SpeakMode.Speak
            ? Speaker.Speak(text, Source, Voices.Voice, true, Rate, Pitch, Volume)
            : Speaker.SpeakNative(text, Voices.Voice, Rate, Pitch, Volume);
      }

      #endregion


      #region Callbacks

      private void onVoicesReady()
      {
         play();
      }

      private void onSpeakComplete(Model.Wrapper wrapper)
      {
         if (playAll && wrapper.Uid.Equals(uid))
         {
            //Invoke(nameof(Next), Delay);
            Invoke("Next", Delay);
         }
      }

      #endregion
   }
}
// © 2016-2020 crosstales LLC (https://www.crosstales.com)