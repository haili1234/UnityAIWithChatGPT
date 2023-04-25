using UnityEngine;

namespace Crosstales.RTVoice.Model
{
   /// <summary>Wrapper for "Speak"-function calls.</summary>
   [System.Serializable]
   public class Wrapper
   {
      #region Variables

      /// <summary>UID of the speech.</summary>
      public readonly string Uid;

      /// <summary>AudioSource for the speech.</summary>
      public AudioSource Source;

      /// <summary>Voice for the speech.</summary>
      public Voice Voice;

      /// <summary>Speak immediately after the audio generation. Only works if 'Source' is not null.</summary>
      public bool SpeakImmediately = true;

      /// <summary>Output file (without extension) for the generated audio.</summary>
      public string OutputFile;

      /// <summary>Force SSML on supported platforms.</summary>
      public bool ForceSSML = true;

      private string text = string.Empty;
      private float rate = 1f;
      private float pitch = 1f;
      private float volume = 1f;

      private string cachedString = null;
      private readonly System.DateTime created = System.DateTime.Now;

      #endregion


      #region Properties

      /// <summary>Text for the speech.</summary>
      public string Text
      {
         get
         {
            if (cachedString == null)
            {
               string result = cachedString = Util.Helper.CleanText(text, Speaker.isAutoClearTags /*&& !(Speaker.isMaryMode /* || Util.Helper.isWindowsPlatform )*/);

               if (Speaker.isMaryMode)
               {
                  // Defined by the URI-request size!
                  if (result.Length > 8000)
                     Debug.LogWarning("Text is longer than 8000 characters, a timeout could happen: " + this);
               }
               else
               {
                  if (result.Length > Speaker.MaxTextLength)
                  {
                     Debug.LogWarning("Text is too long! It will be shortened to " + Speaker.MaxTextLength + " characters: " + this);

                     cachedString = result.Substring(0, Speaker.MaxTextLength);
                  }
               }
            }

            return cachedString;
         }

         set
         {
            cachedString = null;
            text = value;
         }
      }

      /// <summary>Rate of the speech (values: 0-3).</summary>
      public float Rate
      {
         get { return rate; }

         set { rate = Mathf.Clamp(value, 0f, 3f); }
      }

      /// <summary>Pitch of the speech (values: 0-2).</summary>
      public float Pitch
      {
         get { return pitch; }

         set { pitch = Mathf.Clamp(value, 0f, 2f); }
      }

      /// <summary>Volume of the speech (values: 0-1).</summary>
      public float Volume
      {
         get { return volume; }

         set { volume = Mathf.Clamp(value, 0f, 1f); }
      }

      /// <summary>Returns the creation time of the Wrapper.</summary>
      /// <returns>Creation time of the Wrapper.</returns>
      public System.DateTime Created
      {
         get { return created; }
      }

      /// <summary>Returns the speech time in seconds (0: no audio file was generated).</summary>
      /// <returns>Speech time in seconds.</returns>
      public float SpeechTime
      {
         get
         {
            if (Source != null && Source.clip != null)
            {
               return Source.clip.length;
            }

            return 0f;
         }
      }

      #endregion


      #region Constructors

      /// <summary>Default.</summary>
      public Wrapper()
      {
         Uid = System.Guid.NewGuid().ToString();
      }

      /// <summary>Instantiate the class.</summary>
      /// <param name="text">Text for the speech.</param>
      /// <param name="voice">Voice for the speech (default: null, optional).</param>
      /// <param name="rate">Rate of the speech (values: 0-3, default: 1, optional).</param>
      /// <param name="pitch">Pitch of the speech (values: 0-2, default: 1, optional).</param>
      /// <param name="volume">Volume of the speech (values: 0-1, default: 1, optional).</param>
      /// <param name="forceSSML">Force SSML on supported platforms (default: true, optional).</param>
      public Wrapper(string text, Voice voice = null, float rate = 1f, float pitch = 1f, float volume = 1f, bool forceSSML = true)
      {
         Uid = System.Guid.NewGuid().ToString();
         Text = text;
         Voice = voice;
         Rate = rate;
         Pitch = pitch;
         Volume = volume;
         ForceSSML = forceSSML;
      }

      /// <summary>Instantiate the class.</summary>
      /// <param name="text">Text for the speech.</param>
      /// <param name="voice">Voice for the speech (default: null, optional).</param>
      /// <param name="rate">Rate of the speech (values: 0-3, default: 1, optional).</param>
      /// <param name="pitch">Pitch of the speech (values: 0-2, default: 1, optional).</param>
      /// <param name="volume">Volume of the speech (values: 0-1, default: 1, optional).</param>
      /// <param name="source">AudioSource for the speech (default: null, optional).</param>
      /// <param name="speakImmediately">Speak immediately after the audio generation. Only works if 'Source' is not null (default: true, optional).</param>
      /// <param name="outputFile">Output file (without extension) for the generated audio (default: empty, optional).</param>
      /// <param name="forceSSML">Force SSML on supported platforms (default: true, optional).</param>
      public Wrapper(string text, Voice voice = null, float rate = 1f, float pitch = 1f, float volume = 1f, AudioSource source = null, bool speakImmediately = true, string outputFile = "", bool forceSSML = true)
      {
         Uid = System.Guid.NewGuid().ToString();
         Text = text;
         Source = source;
         Voice = voice;
         SpeakImmediately = speakImmediately;
         Rate = rate;
         Pitch = pitch;
         Volume = volume;
         OutputFile = outputFile;
         ForceSSML = forceSSML;
         Debug.Log("AnimeKing source 456:"+Source+OutputFile);
      }

      /// <summary>Instantiate the class.</summary>
      /// <param name="uid">UID of the speech.</param>
      /// <param name="text">Text for the speech.</param>
      /// <param name="voice">Voice for the speech (default: null, optional).</param>
      /// <param name="rate">Rate of the speech (values: 0-3, default: 1, optional).</param>
      /// <param name="pitch">Pitch of the speech (values: 0-2, default: 1, optional).</param>
      /// <param name="volume">Volume of the speech (values: 0-1, default: 1, optional).</param>
      /// <param name="source">AudioSource for the speech (default: null, optional).</param>
      /// <param name="speakImmediately">Speak immediately after the audio generation. Only works if 'Source' is not null (default: true, optional).</param>
      /// <param name="outputFile">Output file (without extension) for the generated audio (default: empty, optional).</param>
      /// <param name="forceSSML">Force SSML on supported platforms (default: true, optional).</param>
      public Wrapper(string uid, string text, Voice voice = null, float rate = 1f, float pitch = 1f, float volume = 1f, AudioSource source = null, bool speakImmediately = true, string outputFile = "", bool forceSSML = true) : this(text, voice, rate, pitch, volume, source, speakImmediately, outputFile, forceSSML)
      {
         Uid = uid;
      }

      #endregion


      #region Overridden methods

      public override string ToString()
      {
         System.Text.StringBuilder result = new System.Text.StringBuilder();

         result.Append(GetType().Name);
         result.Append(Util.Constants.TEXT_TOSTRING_START);

         result.Append("Uid='");
         result.Append(Uid);
         result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("Text='");
         result.Append(text);
         result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("Source='");
         result.Append(Source);
         result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("Voice='");
         result.Append(Voice);
         result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("SpeakImmediately='");
         result.Append(SpeakImmediately);
         result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("Rate='");
         result.Append(rate);
         result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("Pitch='");
         result.Append(pitch);
         result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("Volume='");
         result.Append(volume);
         result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("OutputFile='");
         result.Append(OutputFile);
         result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("ForceSSML='");
         result.Append(ForceSSML);
         result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("Created='");
         result.Append(Created);
         result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER_END);

         result.Append(Util.Constants.TEXT_TOSTRING_END);

         return result.ToString();
      }

      #endregion
   }
}
// © 2015-2020 crosstales LLC (https://www.crosstales.com)