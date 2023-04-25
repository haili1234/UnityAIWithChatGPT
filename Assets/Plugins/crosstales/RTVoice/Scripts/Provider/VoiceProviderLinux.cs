#if UNITY_STANDALONE || UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Linq;

namespace Crosstales.RTVoice.Provider
{
   /// <summary>
   /// Linux voice provider.
   /// NOTE: needs eSpeak to work: http://espeak.sourceforge.net/
   /// </summary>
   public class VoiceProviderLinux : BaseVoiceProvider
   {
      #region Variables

      private const int defaultRate = 160;
      private const int defaultVolume = 100;
      private const int defaultPitch = 50;

      private readonly System.Collections.Generic.List<Model.Voice> voices =
         new System.Collections.Generic.List<Model.Voice>(100);
#if ENABLE_IL2CPP
        private System.Collections.Generic.Dictionary<string, Common.Util.CTProcess> processCreators =
 new System.Collections.Generic.Dictionary<string, Common.Util.CTProcess>();
#endif

      #endregion


      #region Constructor

      /// <summary>
      /// Constructor for VoiceProviderLinux.
      /// </summary>
      /// <param name="obj">Instance of the speaker</param>
      public VoiceProviderLinux(MonoBehaviour obj) : base(obj)
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
         get { return ".wav"; }
      }

      public override AudioType AudioFileType
      {
         get { return AudioType.WAV; }
      }

      public override string DefaultVoiceName
      {
         get { return "en"; }
      }

      public override bool isWorkingInEditor
      {
         get { return true; }
      }

      public override bool isWorkingInPlaymode
      {
         get { return true; }
      }

      public override int MaxTextLength
      {
         get { return 32000; }
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
         get { return isSupported; }
      }

      public static bool isSupported
      {
         get { return Util.Helper.isWindowsPlatform || Util.Helper.isMacOSPlatform || Util.Helper.isLinuxPlatform; }
      }

      public override bool isSSMLSupported
      {
         get { return true; }
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
               int calculatedVolume = calculateVolume(wrapper.Volume);
               int calculatedPitch = calculatePitch(wrapper.Pitch);

               string args = (string.IsNullOrEmpty(voiceName)
                                ? string.Empty
                                : "-v \"" + voiceName.Replace('"', '\'') + '"') +
                             (calculatedRate != defaultRate ? " -s " + calculatedRate + " " : string.Empty) +
                             (calculatedVolume != defaultVolume ? " -a " + calculatedVolume + " " : string.Empty) +
                             (calculatedPitch != defaultPitch ? " -p " + calculatedPitch + " " : string.Empty) +
                             " -z " +
                             " -m \"" +
                             wrapper.Text.Replace('"', '\'') + '"' +
                             (string.IsNullOrEmpty(Util.Config.TTS_LINUX_DATA)
                                ? string.Empty
                                : " --path=\"" + Util.Config.TTS_LINUX_DATA + '"');

               if (Util.Config.DEBUG)
                  Debug.Log("Process arguments: " + args);
#if ENABLE_IL2CPP
                    Common.Util.CTProcess process = new Common.Util.CTProcess();
#else
               System.Diagnostics.Process process = new System.Diagnostics.Process();
#endif
               process.StartInfo.FileName = Util.Config.TTS_LINUX;
               process.StartInfo.Arguments = args;

#if ENABLE_IL2CPP
                    System.Threading.Thread worker =
 new System.Threading.Thread(() => startProcess(ref process, 0, false, false, false, false)) { Name = wrapper.Uid };
#else
               System.Threading.Thread worker = new System.Threading.Thread(() => startProcess(ref process))
                  {Name = wrapper.Uid};
#endif
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
               if (process.ExitCode == 0 || process.ExitCode == -1) //-1 = Killed
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
                  int calculatedVolume = calculateVolume(wrapper.Volume);
                  int calculatedPitch = calculatePitch(wrapper.Pitch);
                  string outputFile = getOutputFile(wrapper.Uid);

                  string args = (string.IsNullOrEmpty(voiceName)
                                   ? string.Empty
                                   : "-v \"" + voiceName.Replace('"', '\'') + '"') +
                                (calculatedRate != defaultRate ? " -s " + calculatedRate + " " : string.Empty) +
                                (calculatedVolume != defaultVolume
                                   ? " -a " + calculatedVolume + " "
                                   : string.Empty) +
                                (calculatedPitch != defaultPitch
                                   ? " -p " + calculatedPitch + " "
                                   : string.Empty) +
                                " -w \"" + outputFile.Replace('"', '\'') + '"' +
                                " -z " +
                                " -m \"" +
                                wrapper.Text.Replace('"', '\'') + '"' +
                                (string.IsNullOrEmpty(Util.Config.TTS_LINUX_DATA)
                                   ? string.Empty
                                   : " --path=\"" + Util.Config.TTS_LINUX_DATA + '"');

                  if (Util.Config.DEBUG)
                     Debug.Log("Process arguments: " + args);
#if ENABLE_IL2CPP
                        Common.Util.CTProcess process = new Common.Util.CTProcess();
#else
                  System.Diagnostics.Process process = new System.Diagnostics.Process();
#endif
                  process.StartInfo.FileName = Util.Config.TTS_LINUX;
                  process.StartInfo.Arguments = args;
#if ENABLE_IL2CPP
                        System.Threading.Thread worker =
 new System.Threading.Thread(() => startProcess(ref process, 0, false, false, false, false)) {Name = wrapper.Uid};
#else
                  System.Threading.Thread worker = new System.Threading.Thread(() => startProcess(ref process))
                     {Name = wrapper.Uid};
#endif
                  worker.Start();

                  silence = false;
                  onSpeakAudioGenerationStart(wrapper);

                  do
                  {
                     yield return null;
                  } while (worker.IsAlive || !process.HasExited);

                  if (process.ExitCode == 0)
                  {
                     yield return playAudioFile(wrapper, Util.Constants.PREFIX_FILE + outputFile, outputFile);
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
               int calculatedVolume = calculateVolume(wrapper.Volume);
               int calculatedPitch = calculatePitch(wrapper.Pitch);
               string outputFile = getOutputFile(wrapper.Uid);

               string args = (string.IsNullOrEmpty(voiceName)
                                ? string.Empty
                                : "-v \"" + voiceName.Replace('"', '\'') + '"') +
                             (calculatedRate != defaultRate ? " -s " + calculatedRate + " " : string.Empty) +
                             (calculatedVolume != defaultVolume ? " -a " + calculatedVolume + " " : string.Empty) +
                             (calculatedPitch != defaultPitch ? " -p " + calculatedPitch + " " : string.Empty) +
                             " -w \"" + outputFile.Replace('"', '\'') + '"' +
                             " -z " +
                             " -m \"" +
                             wrapper.Text.Replace('"', '\'') + '"' +
                             (string.IsNullOrEmpty(Util.Config.TTS_LINUX_DATA)
                                ? string.Empty
                                : " --path=\"" + Util.Config.TTS_LINUX_DATA + '"');

               if (Util.Config.DEBUG)
                  Debug.Log("Process arguments: " + args);
#if ENABLE_IL2CPP
                    Common.Util.CTProcess process = new Common.Util.CTProcess();
#else
               System.Diagnostics.Process process = new System.Diagnostics.Process();
#endif
               process.StartInfo.FileName = Util.Config.TTS_LINUX;
               process.StartInfo.Arguments = args;
#if ENABLE_IL2CPP
                    System.Threading.Thread worker =
 new System.Threading.Thread(() => startProcess(ref process, 0, false, false, false, false)) {Name = wrapper.Uid};
#else
               System.Threading.Thread worker = new System.Threading.Thread(() => startProcess(ref process))
                  {Name = wrapper.Uid};
#endif
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
         foreach (System.Collections.Generic.KeyValuePair<string, Common.Util.CTProcess> kvp in processCreators)
         {
             if (kvp.Value.isBusy)
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

      protected override string getVoiceName(Model.Wrapper wrapper)
      {
         if (wrapper != null && (wrapper.Voice == null || string.IsNullOrEmpty(wrapper.Voice.Name)))
         {
            if (Util.Config.DEBUG)
               Debug.LogWarning("'wrapper.Voice' or 'wrapper.Voice.Name' is null! Using the OS 'default' voice.");

            return DefaultVoiceName;
         }

         if (wrapper == null)
            return DefaultVoiceName;

         if (Speaker.ESpeakMod == Model.Enum.ESpeakModifiers.none)
         {
            if (wrapper.Voice.Gender == Model.Enum.Gender.FEMALE)
            {
               return wrapper.Voice.Name + Util.Constants.ESPEAK_FEMALE_MODIFIER;
            }

            return wrapper.Voice.Name;
         }

         return wrapper.Voice.Name + "+" + Speaker.ESpeakMod;
      }

      private IEnumerator getVoices()
      {
         voices.Clear();

         string args = "--voices" + (string.IsNullOrEmpty(Util.Config.TTS_LINUX_DATA)
                          ? string.Empty
                          : " --path=\"" + Util.Config.TTS_LINUX_DATA + '"');
#if ENABLE_IL2CPP
         Common.Util.CTProcess process = new Common.Util.CTProcess();
#else
         System.Diagnostics.Process process = new System.Diagnostics.Process();
#endif
         process.StartInfo.FileName = Util.Config.TTS_LINUX;
         process.StartInfo.Arguments = args;
         process.OutputDataReceived += process_OutputDataReceived;

         System.Threading.Thread worker = new System.Threading.Thread(() =>
            startProcess(ref process, Util.Constants.DEFAULT_TTS_KILL_TIME, true));
         worker.Start();

         do
         {
            yield return null;
         } while (worker.IsAlive || !process.HasExited);

         if (process.ExitCode == 0)
         {
            //do nothing
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

         cachedVoices = voices.OrderBy(s => s.Name).ToList();

         if (Util.Constants.DEV_DEBUG)
            Debug.Log("Voices read: " + cachedVoices.CTDump());

         onVoicesReady();
      }

      private void process_OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
      {
         //Debug.Log(e.Data);

         string reply = e.Data;

         if (!string.IsNullOrEmpty(reply))
         {
            if (!reply.StartsWith("Pty")) //ignore header
            {
               voices.Add(Util.Config.TTS_LINUX.CTContains("espeak-ng")
                  ? new Model.Voice(reply.Substring(30, 19).Trim().Replace("_", " "), reply.Substring(50).Trim(),
                     Util.Helper.StringToGender(reply.Substring(23, 1)), "unknown",
                     reply.Substring(4, 15).Trim(), "", "espeak-ng")
                  : new Model.Voice(reply.Substring(22, 20).Trim(), reply.Substring(43).Trim(),
                     Util.Helper.StringToGender(reply.Substring(19, 1)), "unknown",
                     reply.Substring(4, 15).Trim(), "", "espeak"));
            }
         }
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

      private static int calculateVolume(float volume)
      {
         return Mathf.Clamp((int)(defaultVolume * volume), 0, 200);
      }

      private static int calculatePitch(float pitch)
      {
         return Mathf.Clamp((int)(defaultPitch * pitch), 0, 99);
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
               int calculatedVolume = calculateVolume(wrapper.Volume);
               int calculatedPitch = calculatePitch(wrapper.Pitch);
               string outputFile = getOutputFile(wrapper.Uid);

               string args = (string.IsNullOrEmpty(voiceName)
                                ? string.Empty
                                : "-v \"" + voiceName.Replace('"', '\'') + '"') +
                             (calculatedRate != defaultRate ? " -s " + calculatedRate + " " : string.Empty) +
                             (calculatedVolume != defaultVolume ? " -a " + calculatedVolume + " " : string.Empty) +
                             (calculatedPitch != defaultPitch ? " -p " + calculatedPitch + " " : string.Empty) +
                             " -w \"" + outputFile.Replace('"', '\'') + '"' +
                             " -z " +
                             " -m \"" +
                             wrapper.Text.Replace('"', '\'') + '"' +
                             (string.IsNullOrEmpty(Util.Config.TTS_LINUX_DATA)
                                ? string.Empty
                                : " --path=\"" + Util.Config.TTS_LINUX_DATA + '"');

               if (Util.Config.DEBUG)
                  Debug.Log("Process arguments: " + args);

#if ENABLE_IL2CPP
                    Common.Util.CTProcess process = new Common.Util.CTProcess();
#else
               System.Diagnostics.Process process = new System.Diagnostics.Process();
#endif
               process.StartInfo.FileName = Util.Config.TTS_LINUX;
               process.StartInfo.Arguments = args;
#if ENABLE_IL2CPP
               System.Threading.Thread worker = new System.Threading.Thread(() => startProcess(ref process, 0, false, false, false, false)) {Name = wrapper.Uid};
#else
               System.Threading.Thread worker = new System.Threading.Thread(() => startProcess(ref process))
                  {Name = wrapper.Uid};
#endif
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

               onSpeakAudioGenerationComplete(wrapper);

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
               int calculatedVolume = calculateVolume(wrapper.Volume);
               int calculatedPitch = calculatePitch(wrapper.Pitch);

               string args = (string.IsNullOrEmpty(voiceName)
                                ? string.Empty
                                : "-v \"" + voiceName.Replace('"', '\'') + '"') +
                             (calculatedRate != defaultRate ? " -s " + calculatedRate + " " : string.Empty) +
                             (calculatedVolume != defaultVolume ? " -a " + calculatedVolume + " " : string.Empty) +
                             (calculatedPitch != defaultPitch ? " -p " + calculatedPitch + " " : string.Empty) +
                             " -z " +
                             " -m \"" +
                             wrapper.Text.Replace('"', '\'') + '"' +
                             (string.IsNullOrEmpty(Util.Config.TTS_LINUX_DATA)
                                ? string.Empty
                                : " --path=\"" + Util.Config.TTS_LINUX_DATA + '"');

               if (Util.Config.DEBUG)
                  Debug.Log("Process arguments: " + args);

#if ENABLE_IL2CPP
               Common.Util.CTProcess process = new Common.Util.CTProcess();
#else
               System.Diagnostics.Process process = new System.Diagnostics.Process();
#endif
               process.StartInfo.FileName = Util.Config.TTS_LINUX;
               process.StartInfo.Arguments = args;
#if ENABLE_IL2CPP
               System.Threading.Thread worker = new System.Threading.Thread(() => startProcess(ref process, 0, false, false, false, false)) {Name = wrapper.Uid};
#else
               System.Threading.Thread worker = new System.Threading.Thread(() => startProcess(ref process))
                  {Name = wrapper.Uid};
#endif
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
               if (process.ExitCode == 0 || process.ExitCode == -1) //-1 = Killed
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
         cachedVoices.Clear();
         string args = "--voices" + (string.IsNullOrEmpty(Util.Config.TTS_LINUX_DATA)
                          ? string.Empty
                          : " --path=\"" + Util.Config.TTS_LINUX_DATA + '"');

#if ENABLE_IL2CPP
            Common.Util.CTProcess process = new Common.Util.CTProcess();
#else
         System.Diagnostics.Process process = new System.Diagnostics.Process();
#endif
         process.StartInfo.FileName = Util.Config.TTS_LINUX;
         process.StartInfo.Arguments = args;
         process.OutputDataReceived += process_OutputDataReceived;

         try
         {
            System.Threading.Thread worker = new System.Threading.Thread(() =>
               startProcess(ref process, Util.Constants.DEFAULT_TTS_KILL_TIME, true));
            worker.Start();

            do
            {
               System.Threading.Thread.Sleep(50);
            } while (worker.IsAlive || !process.HasExited);

            if (Util.Constants.DEV_DEBUG)
               Debug.Log("Finished after: " + (process.ExitTime - process.StartTime).Seconds);

            if (process.ExitCode == 0)
            {
               //do nothing
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

         cachedVoices = voices.OrderBy(s => s.Name).ToList();

         if (Util.Constants.DEV_DEBUG)
            Debug.Log("Voices read: " + cachedVoices.CTDump());

         onVoicesReady();
      }
#endif

      #endregion
   }
}
#endif
// © 2018-2020 crosstales LLC (https://www.crosstales.com)