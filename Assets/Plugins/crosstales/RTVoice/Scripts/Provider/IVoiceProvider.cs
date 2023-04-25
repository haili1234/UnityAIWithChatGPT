using UnityEngine;
using System.Collections;

namespace Crosstales.RTVoice.Provider
{
   /// <summary>Interface for all voice providers.</summary>
   public interface IVoiceProvider
   {
      #region Properties

      /// <summary>Returns the extension of the generated audio files.</summary>
      /// <returns>Extension of the generated audio files.</returns>
      string AudioFileExtension { get; }

      /// <summary>Returns the type of the generated audio files.</summary>
      /// <returns>Type of the generated audio files.</returns>
      AudioType AudioFileType { get; }

      /// <summary>Returns the default voice name of the current TTS-provider.</summary>
      /// <returns>Default voice name of the current TTS-provider.</returns>
      string DefaultVoiceName { get; }

      /// <summary>Get all available voices from the current TTS-provider and fills it into a given list.</summary>
      /// <returns>All available voices (alphabetically ordered by 'Name') as a list.</returns>
      System.Collections.Generic.List<Model.Voice> Voices { get; }

      /// <summary>Maximal length of the speech text (in characters).</summary>
      /// <returns>The maximal length of the speech text.</returns>
      int MaxTextLength { get; }

      /// <summary>Indicates if this provider is working directly inside the Unity Editor (without 'Play'-mode).</summary>
      /// <returns>True if the provider is working directly inside the Unity Editor.</returns>
      bool isWorkingInEditor { get; }

      /// <summary>Indicates if this provider is working with 'Play'-mode inside the Unity Editor.</summary>
      /// <returns>True if this provider is working with 'Play'-mode inside the Unity Editor.</returns>
      bool isWorkingInPlaymode { get; }

      /// <summary>Indicates if this provider is supporting SpeakNative.</summary>
      /// <returns>True if this provider supports SpeakNative.</returns>
      bool isSpeakNativeSupported { get; }

      /// <summary>Indicates if this provider is supporting Speak.</summary>
      /// <returns>True if this provider supports Speak.</returns>
      bool isSpeakSupported { get; }

      /// <summary>Indicates if this provider is supporting the current platform.</summary>
      /// <returns>True if this provider supports current platform.</returns>
      bool isPlatformSupported { get; }

      /// <summary>Indicates if this provider is supporting SSML.</summary>
      /// <returns>True if this provider supports SSML.</returns>
      bool isSSMLSupported { get; }

      /// <summary>Indicates if this provider is an online service like MaryTTS or AWS Polly.</summary>
      /// <returns>True if this provider is an online service.</returns>
      bool isOnlineService { get; }

      /// <summary>Indicates if this provider uses co-routines.</summary>
      /// <returns>True if this provider uses co-routines.</returns>
      bool hasCoRoutines { get; }

      /// <summary>Indicates if this provider is supporting IL2CPP.</summary>
      /// <returns>True if this provider supports IL2CPP.</returns>
      bool isIL2CPPSupported { get; }

      /// <summary>Indicates if this provider returns voices in the Editor mode.</summary>
      /// <returns>True if this provider returns voices in the Editor mode.</returns>
      bool hasVoicesInEditor { get; }

      /// <summary>Get all available cultures from the current provider (ISO 639-1).</summary>
      /// <returns>All available cultures (alphabetically ordered by 'Culture') as a list.</returns>
      System.Collections.Generic.List<string> Cultures { get; }

      #endregion


      #region Methods

      /// <summary>Silence all active TTS-providers.</summary>
      void Silence();

      /// <summary>Silence the current TTS-provider (native mode).</summary>
      /// <param name="uid">UID of the speaker</param>
      void Silence(string uid);

      /// <summary>The current provider speaks a text with a given voice (native mode).</summary>
      /// <param name="wrapper">Wrapper containing the data.</param>
      IEnumerator SpeakNative(Model.Wrapper wrapper);

      /// <summary>The current provider speaks a text with a given voice.</summary>
      /// <param name="wrapper">Wrapper containing the data.</param>
      IEnumerator Speak(Model.Wrapper wrapper);

      /// <summary>The current provider generates an audio file from a text with a given voice.</summary>
      /// <param name="wrapper">Wrapper containing the data.</param>
      IEnumerator Generate(Model.Wrapper wrapper);

      #endregion


      #region Editor-only methods

#if UNITY_EDITOR

      /// <summary>The current provider speaks a text with a given voice (native mode & Editor only).</summary>
      /// <param name="wrapper">Wrapper containing the data.</param>
      void SpeakNativeInEditor(Model.Wrapper wrapper);

      /// <summary>Generates an audio file with the current provider (Editor only).</summary>
      /// <param name="wrapper">Wrapper containing the data.</param>
      void GenerateInEditor(Model.Wrapper wrapper);

#endif

      #endregion
   }
}
// © 2018-2020 crosstales LLC (https://www.crosstales.com)