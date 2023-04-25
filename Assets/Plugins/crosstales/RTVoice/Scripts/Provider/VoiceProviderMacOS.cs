#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || CT_DEVELOP
using System;
using UnityEngine;
using System.Collections;
using System.Linq;

namespace Crosstales.RTVoice.Provider
{
   /// <summary>MacOS voice provider.</summary>
   public class VoiceProviderMacOS : BaseVoiceProvider
   {
      #region Variables

      private static readonly System.Text.RegularExpressions.Regex sayRegex =
         new System.Text.RegularExpressions.Regex(@"^([^#]+?)\s*([^ ]+)\s*# (.*?)$");

      private const int defaultRate = 175;

#if ENABLE_IL2CPP
        private System.Collections.Generic.Dictionary<string, Common.Util.CTProcess> processCreators = new System.Collections.Generic.Dictionary<string, Common.Util.CTProcess>();
#endif

      #endregion


      #region Constructor

      /// <summary>
      /// Constructor for VoiceProviderMacOS.
      /// </summary>
      /// <param name="obj">Instance of the speaker</param>
      public VoiceProviderMacOS(MonoBehaviour obj) : base(obj)
      {
         if (Util.Helper.isEditorMode)
         {
#if UNITY_EDITOR
            getVoicesInEditor();
#endif
         }
         else
         {
            speakerObj.StartCoroutine(getVoices());
         }
      }

      #endregion


      #region Implemented methods

      public override string AudioFileExtension
      {
         get { return ".aiff"; }
      }

      public override AudioType AudioFileType
      {
         get { return AudioType.AIFF; }
      }

      public override string DefaultVoiceName
      {
         get { return "Alex"; }
      }

      public override bool isWorkingInEditor
      {
         get { return Util.Helper.isMacOSEditor; }
      }

      public override bool isWorkingInPlaymode
      {
         get { return Util.Helper.isMacOSEditor; }
      }

      public override int MaxTextLength
      {
         get { return 256000; }
      }

      public override bool isSpeakNativeSupported
      {
         get { return true; }
      }

      public override bool isSpeakSupported
      {
         get { return true; }
      }

      public override bool isPlatformSupported
      {
         get { return Util.Helper.isMacOSPlatform; }
      }

      public override bool isSSMLSupported
      {
         get { return false; }
      }

      public override bool isOnlineService
      {
         get { return false; }
      }

      public override bool hasCoRoutines
      {
         get { return true; }
      }

      public override bool isIL2CPPSupported
      {
         get { return true; }
      }

      public override bool hasVoicesInEditor
      {
         get { return true; }
      }

      public override IEnumerator SpeakNative(Model.Wrapper wrapper)
      {
         if (wrapper == null)
         {
            Debug.LogWarning("'wrapper' is null!");
         }
         else
         {
            if (string.IsNullOrEmpty(wrapper.Text))
            {
               Debug.LogWarning("'wrapper.Text' is null or empty: " + wrapper);
            }
            else
            {
               yield return null; //return to the main process (uid)

               string voiceName = getVoiceName(wrapper);
               int calculatedRate = calculateRate(wrapper.Rate);
#if ENABLE_IL2CPP
               Common.Util.CTProcess process = new Common.Util.CTProcess();
#else
               System.Diagnostics.Process process = new System.Diagnostics.Process();
#endif
               string args = (string.IsNullOrEmpty(voiceName)
                                ? string.Empty
                                : " -v \"" + voiceName.Replace('"', '\'') + '"') +
                             (calculatedRate != defaultRate ? " -r " + calculatedRate : string.Empty) + " \"" +
                             wrapper.Text.Replace('"', '\'') + '"';

               if (Util.Config.DEBUG)
                  Debug.Log("Process arguments: " + args);

               process.StartInfo.FileName = Util.Config.TTS_MACOS;
               process.StartInfo.Arguments = args;

               System.Threading.Thread worker = new System.Threading.Thread(() => startProcess(ref process))
                  {Name = wrapper.Uid};
               worker.Start();

               silence = false;
#if ENABLE_IL2CPP
               processCreators.Add(wrapper.Uid, process);
#else
               processes.Add(wrapper.Uid, process);
#endif
               onSpeakStart(wrapper);

               do
               {
                  yield return null;
               } while (worker.IsAlive || !process.HasExited);
#if ENABLE_IL2CPP
               if (process.ExitCode == 0 || process.ExitCode == 123456) //123456 = Killed
#else
               if (process.ExitCode == 0 || process.ExitCode == -1 || process.ExitCode == 137
                  ) //0 = normal ended, -1/137 = killed
#endif
               {
                  if (Util.Config.DEBUG)
                     Debug.Log("Text spoken: " + wrapper.Text);

                  onSpeakComplete(wrapper);
               }
               else
               {
                  using (System.IO.StreamReader sr = process.StandardError)
                  {
                     string errorMessage = "Could not speak the text: " + wrapper + System.Environment.NewLine +
                                           "Exit code: " + process.ExitCode + System.Environment.NewLine +
                                           sr.ReadToEnd();
                     Debug.LogError(errorMessage);
                     onErrorInfo(wrapper, errorMessage);
                  }
               }
#if ENABLE_IL2CPP
               processCreators.Remove(wrapper.Uid);
#else
               processes.Remove(wrapper.Uid);
#endif
               process.Dispose();
            }
         }
      }

      public Action<AudioClip>  audioClipCallBack = null;
      public override IEnumerator Speak(Model.Wrapper wrapper)
      {
         if (wrapper == null)
         {
            Debug.LogWarning("'wrapper' is null!");
         }
         else
         {
            if (string.IsNullOrEmpty(wrapper.Text))
            {
               Debug.LogWarning("'wrapper.Text' is null or empty: " + wrapper);
            }
            else
            {
               if (wrapper.Source == null)
               {
                  Debug.LogWarning("'wrapper.Source' is null: " + wrapper);
               }
               else
               {
                  yield return null; //return to the main process (uid)

                  string voiceName = getVoiceName(wrapper);
                  int calculatedRate = calculateRate(wrapper.Rate);
                  string outputFile = getOutputFile(wrapper.Uid);
#if ENABLE_IL2CPP
                  Common.Util.CTProcess process = new Common.Util.CTProcess();
#else
                  System.Diagnostics.Process process = new System.Diagnostics.Process();
#endif
                  string args = (string.IsNullOrEmpty(voiceName)
                                   ? string.Empty
                                   : " -v \"" + voiceName.Replace('"', '\'') + '"') +
                                (calculatedRate != defaultRate ? " -r " + calculatedRate : string.Empty) +
                                " -o \"" +
                                outputFile.Replace('"', '\'') + '"' +
                                " --file-format=AIFFLE" + " \"" +
                                wrapper.Text.Replace('"', '\'') + '"';

                  if (Util.Config.DEBUG)
                     Debug.Log("Process arguments: " + args);

                  process.StartInfo.FileName = Util.Config.TTS_MACOS;
                  process.StartInfo.Arguments = args;

                  System.Threading.Thread worker = new System.Threading.Thread(() => startProcess(ref process))
                     {Name = wrapper.Uid};
                  worker.Start();

                  silence = false;
                  onSpeakAudioGenerationStart(wrapper);

                  do
                  {
                     yield return null;
                  } while (worker.IsAlive || !process.HasExited);

                  if (process.ExitCode == 0)
                  {
                     yield return playAudioFile(wrapper, Util.Constants.PREFIX_FILE + outputFile, outputFile,
                        AudioFileType,false,true,null,audioClipCallBack);
                  }
                  else
                  {
                     using (System.IO.StreamReader sr = process.StandardError)
                     {
                        string errorMessage = "Could not speak the text: " + wrapper +
                                              System.Environment.NewLine + "Exit code: " + process.ExitCode +
                                              System.Environment.NewLine + sr.ReadToEnd();
                        Debug.LogError(errorMessage);
                        onErrorInfo(wrapper, errorMessage);
                     }
                  }

                  process.Dispose();
               }
            }
         }
      }

      public override IEnumerator Generate(Model.Wrapper wrapper)
      {
         if (wrapper == null)
         {
            Debug.LogWarning("'wrapper' is null!");
         }
         else
         {
            if (string.IsNullOrEmpty(wrapper.Text))
            {
               Debug.LogWarning("'wrapper.Text' is null or empty: " + wrapper);
            }
            else
            {
               yield return null; //return to the main process (uid)

               string voiceName = getVoiceName(wrapper);
               int calculatedRate = calculateRate(wrapper.Rate);
               string outputFile = getOutputFile(wrapper.Uid);
#if ENABLE_IL2CPP
               Common.Util.CTProcess process = new Common.Util.CTProcess();
#else
               System.Diagnostics.Process process = new System.Diagnostics.Process();
#endif
               string args = (string.IsNullOrEmpty(voiceName)
                                ? string.Empty
                                : " -v \"" + voiceName.Replace('"', '\'') + '"') +
                             (calculatedRate != defaultRate ? " -r " + calculatedRate : string.Empty) + " -o \"" +
                             outputFile.Replace('"', '\'') + '"' +
                             " --file-format=AIFFLE" + " \"" +
                             wrapper.Text.Replace('"', '\'') + '"';

               if (Util.Config.DEBUG)
                  Debug.Log("Process arguments: " + args);

               process.StartInfo.FileName = Util.Config.TTS_MACOS;
               process.StartInfo.Arguments = args;

               System.Threading.Thread worker = new System.Threading.Thread(() => startProcess(ref process))
                  {Name = wrapper.Uid};
               worker.Start();

               silence = false;
               onSpeakAudioGenerationStart(wrapper);

               do
               {
                  yield return null;
               } while (worker.IsAlive || !process.HasExited);

               if (process.ExitCode == 0)
               {
                  processAudioFile(wrapper, outputFile);
               }
               else
               {
                  using (System.IO.StreamReader sr = process.StandardError)
                  {
                     string errorMessage = "Could not generate the text: " + wrapper +
                                           System.Environment.NewLine + "Exit code: " + process.ExitCode +
                                           System.Environment.NewLine + sr.ReadToEnd();
                     Debug.LogError(errorMessage);
                     onErrorInfo(wrapper, errorMessage);
                  }
               }

               process.Dispose();
            }
         }
      }

      public override void Silence()
      {
         base.Silence();
#if ENABLE_IL2CPP
         foreach (var kvp in processCreators.Where(kvp => kvp.Value.isBusy))
         {
             kvp.Value.Kill();
         }

         processCreators.Clear();
#endif
      }

      public override void Silence(string uid)
      {
         base.Silence(uid);
#if ENABLE_IL2CPP
         if (!string.IsNullOrEmpty(uid))
         {
             if (processCreators.ContainsKey(uid))
             {
                 if (processCreators[uid].isBusy)
                     processCreators[uid].Kill();

                 processCreators.Remove(uid);
             }
         }
#endif
      }

      #endregion


      #region Private methods

      private IEnumerator getVoices()
      {
#if ENABLE_IL2CPP
         Common.Util.CTProcess process = new Common.Util.CTProcess();
#else
         System.Diagnostics.Process process = new System.Diagnostics.Process();
#endif

         process.StartInfo.FileName = Util.Config.TTS_MACOS;
         process.StartInfo.Arguments = "-v '?'";

         process.Start();

         System.Threading.Thread worker =
            new System.Threading.Thread(() => startProcess(ref process, Util.Constants.DEFAULT_TTS_KILL_TIME));
         worker.Start();

         do
         {
            yield return null;
         } while (worker.IsAlive || !process.HasExited);

         if (process.ExitCode == 0)
         {
            System.Collections.Generic.List<Model.Voice> voices =
               new System.Collections.Generic.List<Model.Voice>(60);

            using (System.IO.StreamReader streamReader = process.StandardOutput)
            {
               while (!streamReader.EndOfStream)
               {
                  var reply = streamReader.ReadLine();

                  if (!string.IsNullOrEmpty(reply))
                  {
                     System.Text.RegularExpressions.Match match = sayRegex.Match(reply);

                     if (match.Success)
                     {
                        var name = match.Groups[1].ToString();
                        voices.Add(new Model.Voice(name, match.Groups[3].ToString(),
                           Util.Helper.AppleVoiceNameToGender(name), "unknown",
                           match.Groups[2].ToString().Replace('_', '-'), string.Empty, "Apple"));
                     }
                  }
               }
            }

            cachedVoices = voices.OrderBy(s => s.Name).ToList();

            if (Util.Constants.DEV_DEBUG)
               Debug.Log("Voices read: " + cachedVoices.CTDump());
         }
         else
         {
            using (System.IO.StreamReader sr = process.StandardError)
            {
               string errorMessage = "Could not get any voices: " + process.ExitCode + System.Environment.NewLine +
                                     sr.ReadToEnd();
               Debug.LogError(errorMessage);
               onErrorInfo(null, errorMessage);
            }
         }

         process.Dispose();

         onVoicesReady();
      }

      private static int calculateRate(float rate)
      {
         int result =
            Mathf.Clamp(
               Mathf.Abs(rate - 1f) > Common.Util.BaseConstants.FLOAT_TOLERANCE
                  ? (int)(defaultRate * rate)
                  : defaultRate, 1, 3 * defaultRate);

         if (Util.Constants.DEV_DEBUG)
            Debug.Log("calculateRate: " + result + " - " + rate);

         return result;
      }

      #endregion


      #region Editor-only methods

#if UNITY_EDITOR
      public override void GenerateInEditor(Model.Wrapper wrapper)
      {
         if (wrapper == null)
         {
            Debug.LogWarning("'wrapper' is null!");
         }
         else
         {
            if (string.IsNullOrEmpty(wrapper.Text))
            {
               Debug.LogWarning("'wrapper.Text' is null or empty: " + wrapper);
            }
            else
            {
               string voiceName = getVoiceName(wrapper);
               int calculatedRate = calculateRate(wrapper.Rate);
               string outputFile = getOutputFile(wrapper.Uid);
#if ENABLE_IL2CPP
                    Common.Util.CTProcess process = new Common.Util.CTProcess();
#else
               System.Diagnostics.Process process = new System.Diagnostics.Process();
#endif
               string args = (string.IsNullOrEmpty(voiceName)
                                ? string.Empty
                                : " -v \"" + voiceName.Replace('"', '\'') + '"') +
                             (calculatedRate != defaultRate ? " -r " + calculatedRate : string.Empty) + " -o \"" +
                             outputFile.Replace('"', '\'') + '"' +
                             " --file-format=AIFFLE" + " \"" +
                             wrapper.Text.Replace('"', '\'') + '"';

               if (Util.Config.DEBUG)
                  Debug.Log("Process arguments: " + args);

               process.StartInfo.FileName = Util.Config.TTS_MACOS;
               process.StartInfo.Arguments = args;

               System.Threading.Thread worker = new System.Threading.Thread(() => startProcess(ref process))
                  {Name = wrapper.Uid};
               worker.Start();

               silence = false;
               onSpeakAudioGenerationStart(wrapper);

               do
               {
                  System.Threading.Thread.Sleep(50);
               } while (worker.IsAlive || !process.HasExited);

               if (process.ExitCode == 0)
               {
                  processAudioFile(wrapper, outputFile);
               }
               else
               {
                  using (System.IO.StreamReader sr = process.StandardError)
                  {
                     string errorMessage = "Could not generate the text: " + wrapper +
                                           System.Environment.NewLine + "Exit code: " + process.ExitCode +
                                           System.Environment.NewLine + sr.ReadToEnd();
                     Debug.LogError(errorMessage);
                     onErrorInfo(wrapper, errorMessage);
                  }
               }

               process.Dispose();
            }
         }
      }

      public override void SpeakNativeInEditor(Model.Wrapper wrapper)
      {
         if (wrapper == null)
         {
            Debug.LogWarning("'wrapper' is null!");
         }
         else
         {
            if (string.IsNullOrEmpty(wrapper.Text))
            {
               Debug.LogWarning("'wrapper.Text' is null or empty: " + wrapper);
            }
            else
            {
               string voiceName = getVoiceName(wrapper);
               int calculatedRate = calculateRate(wrapper.Rate);
#if ENABLE_IL2CPP
               Common.Util.CTProcess process = new Common.Util.CTProcess();
#else
               System.Diagnostics.Process process = new System.Diagnostics.Process();
#endif
               string args = (string.IsNullOrEmpty(voiceName)
                                ? string.Empty
                                : " -v \"" + voiceName.Replace('"', '\'') + '"') +
                             (calculatedRate != defaultRate ? " -r " + calculatedRate : string.Empty) + " \"" +
                             wrapper.Text.Replace('"', '\'') + '"';

               if (Util.Config.DEBUG)
                  Debug.Log("Process arguments: " + args);

               process.StartInfo.FileName = Util.Config.TTS_MACOS;
               process.StartInfo.Arguments = args;

               System.Threading.Thread worker = new System.Threading.Thread(() => startProcess(ref process))
                  {Name = wrapper.Uid};
               worker.Start();

               silence = false;
               onSpeakStart(wrapper);

               do
               {
                  System.Threading.Thread.Sleep(50);

                  if (silence && !process.HasExited)
                  {
                     process.Kill();
                  }
               } while (worker.IsAlive || !process.HasExited);

#if ENABLE_IL2CPP
               if (process.ExitCode == 0 || process.ExitCode == 123456) //123456 = Killed
#else
               if (process.ExitCode == 0 || process.ExitCode == -1 || process.ExitCode == 137
                  ) //0 = normal ended, -1/137 = killed
#endif
               {
                  if (Util.Config.DEBUG)
                     Debug.Log("Text spoken: " + wrapper.Text);

                  onSpeakComplete(wrapper);
               }
               else
               {
                  using (System.IO.StreamReader sr = process.StandardError)
                  {
                     string errorMessage = "Could not speak the text: " + wrapper + System.Environment.NewLine +
                                           "Exit code: " + process.ExitCode + System.Environment.NewLine +
                                           sr.ReadToEnd();
                     Debug.LogError(errorMessage);
                     onErrorInfo(wrapper, errorMessage);
                  }
               }

               process.Dispose();
            }
         }
      }

      private void getVoicesInEditor()
      {
#if ENABLE_IL2CPP
         Common.Util.CTProcess process = new Common.Util.CTProcess();
#else
         System.Diagnostics.Process process = new System.Diagnostics.Process();
#endif
         process.StartInfo.FileName = Util.Config.TTS_MACOS;
         process.StartInfo.Arguments = "-v '?'";

         try
         {
            System.Threading.Thread worker = new System.Threading.Thread(() =>
               startProcess(ref process, Util.Constants.DEFAULT_TTS_KILL_TIME));
            worker.Start();

            do
            {
               System.Threading.Thread.Sleep(50);
            } while (worker.IsAlive || !process.HasExited);

            if (Util.Constants.DEV_DEBUG)
               Debug.Log("Finished after: " + (process.ExitTime - process.StartTime).Seconds);

            if (process.ExitCode == 0)
            {
               System.Collections.Generic.List<Model.Voice> voices =
                  new System.Collections.Generic.List<Model.Voice>(100);

               using (System.IO.StreamReader streamReader = process.StandardOutput)
               {
                  while (!streamReader.EndOfStream)
                  {
                     var reply = streamReader.ReadLine();

                     if (!string.IsNullOrEmpty(reply))
                     {
                        System.Text.RegularExpressions.Match match = sayRegex.Match(reply);

                        if (match.Success)
                        {
                           var name = match.Groups[1].ToString();
                           voices.Add(new Model.Voice(match.Groups[1].ToString(), match.Groups[3].ToString(),
                              Util.Helper.AppleVoiceNameToGender(name), "unknown",
                              match.Groups[2].ToString().Replace('_', '-')));
                        }
                     }
                  }
               }

               cachedVoices = voices.OrderBy(s => s.Name).ToList();

               if (Util.Constants.DEV_DEBUG)
                  Debug.Log("Voices read: " + cachedVoices.CTDump());
            }
            else
            {
               using (System.IO.StreamReader sr = process.StandardError)
               {
                  string errorMessage = "Could not get any voices: " + process.ExitCode +
                                        System.Environment.NewLine + sr.ReadToEnd();
                  Debug.LogError(errorMessage);
               }
            }
         }
         catch (System.Exception ex)
         {
            string errorMessage = "Could not get any voices!" + System.Environment.NewLine + ex;
            Debug.LogError(errorMessage);
         }

         process.Dispose();
         onVoicesReady();
      }
#endif

      #endregion
   }
}
#endif
// © 2015-2020 crosstales LLC (https://www.crosstales.com)