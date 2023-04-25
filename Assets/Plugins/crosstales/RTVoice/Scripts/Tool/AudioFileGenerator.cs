using UnityEngine;
using System.Collections;

namespace Crosstales.RTVoice.Tool
{
   /// <summary>Process files with configured speeches.</summary>
   [ExecuteInEditMode]
   [HelpURL("https://crosstales.com/media/data/assets/rtvoice/api/class_crosstales_1_1_r_t_voice_1_1_tool_1_1_audio_file_generator.html")]
   public class AudioFileGenerator : MonoBehaviour
   {
      #region Variables

      /// <summary>Text files to generate.</summary>
      [Tooltip("Text files to generate.")] public TextAsset[] TextFiles;

      /// <summary>Are the specified file paths inside the Assets-folder (current project)? If this option is enabled, it prefixes the path with 'Application.dataPath' (default: true).</summary>
      [Tooltip("Are the specified file paths inside the Assets-folder (current project)? If this option is enabled, it prefixes the path with 'Application.dataPath' (default: true).")]
      public bool FileInsideAssets = true;

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
      /// <summary>Set the sample rate of the WAV files (default: 48000). Note: this works only under Windows standalone.</summary>
      [Tooltip("Set the sample rate of the WAV files (default: 48000). Note: this works only under Windows standalone.")]
      public Common.Model.Enum.SampleRate SampleRate = Common.Model.Enum.SampleRate._48000Hz;

      /// <summary>Set the bits per sample of the WAV files (default: 16). Note: this works only under Windows standalone.</summary>
      [Tooltip("Set the bits per sample of the WAV files (default: 16). Note: this works only under Windows standalone.")]
      public int BitsPerSample = 16;

      /// <summary>Set the channels of the WAV files (default: 2). Note: this works only under Windows standalone.</summary>
      [Tooltip("Set the channels of the WAV files (default: 1). Note: this works only under Windows standalone.")] [Range(1, 2)]
      public int Channels = 2;

      /// <summary>Creates a copy of the downsampled WAV file and leaves the original intact (default: false). Note: this works only under Windows standalone..</summary>
      [Tooltip("Creates a copy of the downsampled WAV file and leaves the original intact (default: false). Note: this works only under Windows standalone.")]
      public bool CreateCopy = false;
#endif

      ///// <summary>Normalize the volume of the WAV files (default: false). Note: this works only under Windows standalone.</summary>
      [Tooltip("Normalize the volume of the WAV files (default: false). Note: this works only under Windows standalone.")]
      public bool isNormalize = false;

      /// <summary>Enable generating of the texts on start (default: false).</summary>
      [Tooltip("Enable generating of the texts on start (default: false).")] public bool GenerateOnStart = false;

      private static readonly char[] splitChar = {';'};

      private string lastUid = "crosstales";

      private bool isGenerate = false;

      #endregion


      #region Events

      private AudioFileGeneratorStart _onStart;
      private AudioFileGeneratorComplete _onComplete;

      /// <summary>An event triggered whenever a AudioFileGenerator 'Generate' is started.</summary>
      public event AudioFileGeneratorStart OnAudioFileGeneratorStart
      {
         add { _onStart += value; }
         remove { _onStart -= value; }
      }

      /// <summary>An event triggered whenever a AudioFileGenerator 'Generate' is completed.</summary>
      public event AudioFileGeneratorComplete OnAudioFileGeneratorComplete
      {
         add { _onComplete += value; }
         remove { _onComplete -= value; }
      }

      #endregion


      #region MonoBehaviour methods

      public void OnEnable()
      {
         // Subscribe event listeners
         Speaker.OnSpeakAudioGenerationComplete += onSpeakAudioGenerationComplete;
         Speaker.OnVoicesReady += onVoicesReady;
      }

      public void OnDisable()
      {
         // Unsubscribe event listeners
         Speaker.OnSpeakAudioGenerationComplete -= onSpeakAudioGenerationComplete;
         Speaker.OnVoicesReady -= onVoicesReady;
      }

      public void OnValidate()
      {
#if UNITY_STANDALONE_WIN
         if (BitsPerSample < 15)
         {
            BitsPerSample = 8;
         }
         else if (BitsPerSample < 31)
         {
            BitsPerSample = 16;
         }
         else
         {
            BitsPerSample = 32;
         }

         Channels = Channels <= 1 ? 1 : 2;
#endif
      }

      #endregion


      #region Public methods

      /// <summary>Generate the audio files from the text files.</summary>
      public void Generate()
      {
         if (!isGenerate)
         {
            isGenerate = true;

            if (Util.Helper.isEditorMode)
            {
#if UNITY_EDITOR
               generateInEditor();
#endif
            }
            else
            {
               StartCoroutine(generate());
            }
         }
      }

      #endregion


      #region Private methods

      public IEnumerator generate()
      {
         onStart();

         foreach (TextAsset textFile in TextFiles)
         {
            if (textFile != null)
            {
               System.Collections.Generic.List<string> speeches = Util.Helper.SplitStringToLines(textFile.text);

               foreach (string speech in speeches)
               {
                  if (!speech.StartsWith("#"))
                  {
                     string[] args = speech.Split(splitChar, System.StringSplitOptions.RemoveEmptyEntries);

                     if (args.Length >= 2)
                     {
                        Model.Wrapper wrapper = prepare(args, speech);
                        string uid = Speaker.Generate(wrapper);

                        if (Util.Helper.isWindowsPlatform)
                        {
                           do
                           {
                              yield return null;
                           } while (!uid.Equals(lastUid));

                           convert(wrapper.OutputFile);
#if UNITY_STANDALONE_WIN
                           if (isNormalize)
                              Normalize(wrapper.OutputFile);
#endif
                        }
                        else
                        {
                           yield return null;
                        }
                     }
                     else
                     {
                        Debug.LogWarning("Invalid speech: " + speech);
                     }
                  }
               }
            }
         }

         if (Util.Config.DEBUG)
            Debug.Log("Generate finished!");

         onComplete();

         isGenerate = false;
      }

      private void convert(string outputFile)
      {
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN) && !UNITY_EDITOR_OSX && !UNITY_EDITOR_LINUX
         string tmpFile = outputFile.Substring(0, outputFile.Length - 4) + "_" + SampleRate +
                          Speaker.AudioFileExtension;
         bool converted = false;

         try
         {
            using (NAudio.Wave.WaveFileReader reader = new NAudio.Wave.WaveFileReader(outputFile))
            {
               if (reader.WaveFormat.SampleRate != (int)SampleRate)
               {
                  NAudio.Wave.WaveFormat newFormat = new NAudio.Wave.WaveFormat((int)SampleRate, BitsPerSample, Channels);

                  using (NAudio.Wave.WaveFormatConversionStream conversionStream =
                     new NAudio.Wave.WaveFormatConversionStream(newFormat, reader))
                  {
                     NAudio.Wave.WaveFileWriter.CreateWaveFile(tmpFile, conversionStream);
                  }

                  converted = true;
               }
            }
         }
         catch (System.Exception ex)
         {
            Debug.LogError("Could not convert audio file: " + ex);
         }

         if (converted)
         {
            try
            {
               if (!CreateCopy)
               {
                  System.IO.File.Delete(outputFile);

                  System.IO.File.Move(tmpFile, outputFile);
               }
            }
            catch (System.Exception ex)
            {
               Debug.LogError("Could not delete and move audio files: " + ex);
            }
         }
#else
            Debug.LogError("Can only convert WAV audio files under Windows standalone!");
#endif
      }


      //TODO document!
      public void Normalize(string inputFile)
      {
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN) && !UNITY_EDITOR_OSX && !UNITY_EDITOR_LINUX
         string tmpFile = inputFile.Substring(0, inputFile.Length - 4) + "_normalized" + Speaker.AudioFileExtension;

         try
         {
            //float max = 0;

            using (NAudio.Wave.AudioFileReader reader = new NAudio.Wave.AudioFileReader(inputFile))
            {
               float max = GetMaxPeak(inputFile);

               if (Mathf.Abs(max) < Common.Util.BaseConstants.FLOAT_TOLERANCE || max > 1f)
               {
                  Debug.LogWarning("File cannot be normalized!");
               }
               else
               {
                  // rewind and amplify
                  reader.Position = 0;
                  reader.Volume = 1f / max;

                  // write out to a new WAV file
                  //NAudio.Wave.WaveFileWriter.CreateWaveFile16(inputFile, reader);
                  NAudio.Wave.WaveFileWriter.CreateWaveFile16(tmpFile, reader);
               }
            }

            //System.IO.File.Delete(tmpFile);
         }
         catch (System.Exception ex)
         {
            Debug.LogError("Could not normalize audio file: " + ex);
         }
#else
            Debug.LogError("Can only normalize WAV audio files under Windows standalone!");
#endif
      }

      //TODO document!
      public static float GetMaxPeak(string inputFile)
      {
#if (UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN) && !UNITY_EDITOR_OSX && !UNITY_EDITOR_LINUX
         float max = 0;

         try
         {
            using (NAudio.Wave.AudioFileReader reader = new NAudio.Wave.AudioFileReader(inputFile))
            {
               // find the max peak
               float[] buffer = new float[reader.WaveFormat.SampleRate];
               int read;

               do
               {
                  read = reader.Read(buffer, 0, buffer.Length);
                  for (int ii = 0; ii < read; ii++)
                  {
                     float abs = Mathf.Abs(buffer[ii]);
                     if (abs > max) max = abs;
                  }
               } while (read > 0);
            }
         }
         catch (System.Exception ex)
         {
            Debug.LogError("Could not find the max peak in audio file: " + ex);
         }

         return max;
#else
            Debug.LogWarning("Can only find max peak from WAV audio files under Windows standalone!");
            return 1f;
#endif
      }

      private Model.Wrapper prepare(string[] args, string speech)
      {
         Model.Wrapper wrapper = new Model.Wrapper();

         wrapper.Text = args[0];

         if (FileInsideAssets)
         {
            wrapper.OutputFile = Application.dataPath + @"/" + args[1];
         }
         else
         {
            wrapper.OutputFile = args[1];
         }

         if (args.Length >= 3)
         {
            wrapper.Voice = Speaker.VoiceForName(args[2]);
         }

         if (args.Length >= 4)
         {
            float rate;
            if (!float.TryParse(args[3], out rate))
            {
               Debug.LogWarning("Rate was invalid: " + speech);
            }
            else
            {
               wrapper.Rate = rate;
            }
         }

         if (args.Length >= 5)
         {
            float pitch;
            if (!float.TryParse(args[4], out pitch))
            {
               Debug.LogWarning("Pitch was invalid: " + speech);
            }
            else
            {
               wrapper.Pitch = pitch;
            }
         }

         if (args.Length >= 6)
         {
            float volume;
            if (!float.TryParse(args[5], out volume))
            {
               Debug.LogWarning("Volume was invalid: " + speech);
            }
            else
            {
               wrapper.Volume = volume;
            }
         }

         return wrapper;
      }

      #endregion


      #region Callbacks

      private void onVoicesReady()
      {
         if (GenerateOnStart)
            Generate();
      }

      private void onSpeakAudioGenerationComplete(Model.Wrapper wrapper,AudioClip clip)
      {
         lastUid = wrapper.Uid;

         if (Util.Config.DEBUG)
            Debug.Log("Speech generated: " + wrapper);
      }

      #endregion


      #region Event-trigger methods

      private void onStart()
      {
         if (Util.Config.DEBUG)
            Debug.Log("onStart");

         if (_onStart != null) _onStart.Invoke();
      }

      private void onComplete()
      {
         if (Util.Config.DEBUG)
            Debug.Log("onComplete");

         if (_onComplete != null) _onComplete.Invoke();
      }

      #endregion


      #region Editor-only methods

#if UNITY_EDITOR
      private void generateInEditor()
      {
         foreach (TextAsset textFile in TextFiles)
         {
            if (textFile != null)
            {
               System.Collections.Generic.List<string> speeches = Util.Helper.SplitStringToLines(textFile.text);

               foreach (string speech in speeches)
               {
                  if (!speech.StartsWith("#"))
                  {
                     string[] args = speech.Split(splitChar, System.StringSplitOptions.RemoveEmptyEntries);

                     if (args.Length >= 2)
                     {
                        Model.Wrapper wrapper = prepare(args, speech);
                        Speaker.Generate(wrapper);

                        //TODO re-enable in a later release
                        /*
                        string uid = Speaker.Generate(wrapper);
                    
                        if (Util.Helper.isWindowsPlatform)
                        {
                            do
                            {
                                //Debug.Log("Wait...: " + uid + " - " + lastUid);
                                System.Threading.Thread.Sleep(50);
                            } while (!uid.Equals(lastUid));

                            Debug.Log(wrapper);
                            convert(wrapper.OutputFile);

                            //System.Threading.Thread.Sleep(100);

                            if (isNormalize)
                                Normalize(wrapper.OutputFile);
                        }
                        */
                     }
                     else
                     {
                        Debug.LogWarning("Invalid speech: " + speech);
                     }
                  }
               }
            }
         }

         if (Util.Config.DEBUG)
            Debug.Log("Generate finished!");

#if UNITY_EDITOR
         if (FileInsideAssets)
            UnityEditor.AssetDatabase.Refresh();
#endif

         isGenerate = false;
      }

#endif

      #endregion
   }
}
// © 2017-2020 crosstales LLC (https://www.crosstales.com)