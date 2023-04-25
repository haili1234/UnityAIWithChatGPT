using UnityEngine;

namespace Crosstales.RTVoice.Model
{
   /// <summary>Model for a sequence.</summary>
   [System.Serializable]
   public class Sequence
   {
      #region Variables

      /// <summary>Text to speak.</summary>
      [Tooltip("Text to speak.")] [TextArea] public string Text = string.Empty;

      /// <summary>Voices for the speech.</summary>
      [Tooltip("Voices for the speech.")] public VoiceAlias Voices;

      /// <summary>Speak mode (default: 'Speak').</summary>
      [Tooltip("Speak mode (default: 'Speak').")] public Enum.SpeakMode Mode = Enum.SpeakMode.Speak;


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

      [HideInInspector] public bool initalized = false;

      #endregion


      #region Overridden methods

      public override string ToString()
      {
         System.Text.StringBuilder result = new System.Text.StringBuilder();

         result.Append(GetType().Name);
         result.Append(Util.Constants.TEXT_TOSTRING_START);

         result.Append("Text='");
         result.Append(Text);
         result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("Voices='");
         result.Append(Voices);
         result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("Source='");
         result.Append(Source);
         result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("Rate='");
         result.Append(Rate);
         result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("Pitch='");
         result.Append(Pitch);
         result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

         result.Append("Volume='");
         result.Append(Volume);
         result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER_END);

         result.Append(Util.Constants.TEXT_TOSTRING_END);

         return result.ToString();
      }

      #endregion
   }
}
// © 2016-2020 crosstales LLC (https://www.crosstales.com)