using UnityEngine;

namespace Crosstales.RTVoice.Model
{
   /// <summary>Model for a voice.</summary>
   [System.Serializable]
   public class Voice
   {
      #region Variables

      /// <summary>Name of the voice.</summary>
      [Tooltip("Name of the voice.")] public string Name;

      /// <summary>Description of the voice.</summary>
      [Tooltip("Description of the voice.")] public string Description;

      /// <summary>Gender of the voice.</summary>
      [Tooltip("Gender of the voice.")] public Enum.Gender Gender;

      /// <summary>Age of the voice.</summary>
      [Tooltip("Age of the voice.")] public string Age;

      /// <summary>Identifier of the voice.</summary>
      [Tooltip("Identifier of the voice.")] public string Identifier = string.Empty;

      /// <summary>Vendor of the voice.</summary>
      [Tooltip("Vendor of the voice.")] public string Vendor = string.Empty;

      /// <summary>Version of the voice.</summary>
      [Tooltip("Version of the voice.")] public string Version = string.Empty;

      /// <summary>Sample rate in Hz of the voice.</summary>
      [Tooltip("Sample rate in Hz of the voice.")] public int SampleRate = 0;

      private string culture;
      private string simplifiedCulture;

      #endregion


      #region Properties

      /// <summary>Culture of the voice (ISO 639-1).</summary>
      public string Culture
      {
         get { return culture; }

         set
         {
            if (value != null)
            {
               culture = value.Trim().Replace('_', '-');
               SimplifiedCulture = culture;
            }
         }
      }

      /// <summary>Simpified culture of the voice.</summary>
      public string SimplifiedCulture
      {
         get { return simplifiedCulture; }

         private set
         {
            if (value != null)
               simplifiedCulture = value.Replace("-", string.Empty);
         }
      }

      #endregion


      #region Constructors

      /// <summary>Instantiate the class.</summary>
      /// <param name="name">Name of the voice.</param>
      /// <param name="description">Description of the voice.</param>
      /// <param name="gender">Gender of the voice.</param>
      /// <param name="age">Age of the voice.</param>
      /// <param name="culture">Culture of the voice.</param>
      /// <param name="id">Identifier of the voice (optional).</param>
      /// <param name="vendor">Vendor of the voice (optional).</param>
      /// <param name="version">Version of the voice (optional).</param>
      /// <param name="sampleRate">Sample rate in Hz of the voice (optional).</param>
      public Voice(string name, string description, Enum.Gender gender, string age, string culture, string id = "", string vendor = "unknown", string version = "unknown", int sampleRate = 0)
      {
         Name = name;
         Description = description;
         Gender = gender;
         Age = age;
         Culture = culture;
         Identifier = id;
         Vendor = vendor;
         Version = version;
         SampleRate = sampleRate;
      }

      #endregion


      #region Overridden methods

      public override string ToString()
      {
         return Name + " (" + Culture + ", " + Gender + ")";
      }

      /*
      public override string ToString()
      {
          System.Text.StringBuilder result = new System.Text.StringBuilder();

          result.Append(GetType().Name);
          result.Append(Util.Constants.TEXT_TOSTRING_START);

          result.Append("Name='");
          result.Append(Name);
          result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

          result.Append("Description='");
          result.Append(Description);
          result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

          result.Append("Gender='");
          result.Append(Gender);
          result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

          result.Append("Age='");
          result.Append(Age);
          result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

          result.Append("Culture='");
          result.Append(Culture);
          result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

          result.Append("Identifier='");
          result.Append(Identifier);
          result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);
          
          result.Append("Vendor='");
          result.Append(Vendor);
          //result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);
          result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);

          result.Append("Version='");
          result.Append(Version);
          //result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER);
          result.Append(Util.Constants.TEXT_TOSTRING_DELIMITER_END);

          result.Append(Util.Constants.TEXT_TOSTRING_END);

          return result.ToString();
      }
      */

      #endregion
   }
}
// © 2015-2020 crosstales LLC (https://www.crosstales.com)