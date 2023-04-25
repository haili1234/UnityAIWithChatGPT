#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN //|| CT_DEVELOP
using UnityEngine;
using System.Collections;
using System.Linq;

namespace Crosstales.RTVoice.Provider
{
   /// <summary>Windows voice provider.</summary>
   public class VoiceProviderWindows : BaseVoiceProvider
   {
      #region Variables

#if ENABLE_IL2CPP
        private const bool useVisemesAndPhonemesIL2CPP = false;
#endif
      private readonly string dataPath;

      private const string idVoice = "@VOICE:";
      private const string idSpeak = "@SPEAK";
      private const string idWord = "@WORD";
      private const string idPhoneme = "@PHONEME:";
      private const string idViseme = "@VISEME:";
      private const string idStart = "@STARTED";

      private static readonly char[] splitChar = {':'};

#if ENABLE_IL2CPP
        private System.Collections.Generic.Dictionary<string, Common.Util.CTProcess> processCreators = new System.Collections.Generic.Dictionary<string, Common.Util.CTProcess>();
#endif

      #endregion


      #region Constructor

      /// <summary>
      /// Constructor for VoiceProviderWindowsLegacy.
      /// </summary>
      /// <param name="obj">Instance of the speaker</param>
      public VoiceProviderWindows(MonoBehaviour obj) : base(obj)
      {
         dataPath = Application.dataPath;

         //Debug.Log("APP: " + applicationName);

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
         get { return "Microsoft David Desktop"; }
      }

      public override bool isWorkingInEditor
      {
         get { return Util.Helper.isWindowsEditor; }
      }

      public override bool isWorkingInPlaymode
      {
         get { return Util.Helper.isWindowsEditor; }
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
         get { return Util.Helper.isWindowsPlatform; }
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
               Debug.LogWarning("'wrapper.Text' is null or empty!");
            }
            else
            {
               yield return null; //return to the main process (uid)

               if (System.IO.File.Exists(applicationName))
               {
                  string voiceName = getVoiceName(wrapper);
                  int calculatedRate = calculateRate(wrapper.Rate);
                  int calculatedVolume = calculateVolume(wrapper.Volume);

                  //string args = $"--speak -text \"{prepareText(wrapper)}\" -rate {calculatedRate} -volume {calculatedVolume} -voice \"{voiceName.Replace('"', '\'')}\"";
                  string args = "--speak " +
                                "-text \"" + prepareText(wrapper) + "\" " +
                                "-rate " + calculatedRate + " " +
                                "-volume " + calculatedVolume + " " +
                                "-voice \"" + voiceName.Replace('"', '\'') + '"';

                  if (Util.Config.DEBUG)
                     Debug.Log("Process arguments: " + args);
#if ENABLE_IL2CPP
                  Common.Util.CTProcess process = new Common.Util.CTProcess();
#else
                  System.Diagnostics.Process process = new System.Diagnostics.Process();
#endif
                  //speakProcess.StartInfo.FileName = System.IO.Path.GetFileName(application);
                  //speakProcess.StartInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(application);
                  process.StartInfo.FileName = applicationName;
                  process.StartInfo.Arguments = args;

                  string[] speechTextArray = Util.Helper.CleanText(wrapper.Text, false)
                     .Split(splitCharWords, System.StringSplitOptions.RemoveEmptyEntries);
                  int wordIndex = 0;
                  int wordIndexCompare = 0;
                  string phoneme = string.Empty;
                  string viseme = string.Empty;
                  bool start = false;
#if ENABLE_IL2CPP
                  System.Threading.Thread worker = new System.Threading.Thread(() => readSpeakNativeStream(ref process, ref speechTextArray, out wordIndex, out phoneme, out viseme, out start, useVisemesAndPhonemesIL2CPP, useVisemesAndPhonemesIL2CPP)) {Name = wrapper.Uid};
#else
                  System.Threading.Thread worker = new System.Threading.Thread(() =>
                     readSpeakNativeStream(ref process, ref speechTextArray, out wordIndex, out phoneme,
                        out viseme, out start)) {Name = wrapper.Uid};
#endif
                  worker.Start();

                  silence = false;
#if ENABLE_IL2CPP
                  processCreators.Add(wrapper.Uid, process);
#else
                  processes.Add(wrapper.Uid, process);
#endif
                  do
                  {
                     yield return null;

                     if (wordIndex != wordIndexCompare)
                     {
                        onSpeakCurrentWord(wrapper, speechTextArray, wordIndex - 1);

                        wordIndexCompare = wordIndex;
                     }

                     if (!string.IsNullOrEmpty(phoneme))
                     {
                        onSpeakCurrentPhoneme(wrapper, phoneme);

                        phoneme = string.Empty;
                     }

                     if (!string.IsNullOrEmpty(viseme))
                     {
                        onSpeakCurrentViseme(wrapper, viseme);

                        viseme = string.Empty;
                     }

                     if (start)
                     {
                        onSpeakStart(wrapper);

                        start = false;
                     }
                  } while (worker.IsAlive || !process.HasExited);

                  // clear output
                  onSpeakCurrentPhoneme(wrapper, string.Empty);
                  onSpeakCurrentViseme(wrapper, string.Empty);
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
                        string errorMessage = "Could not speak the text: " + wrapper +
                                              System.Environment.NewLine + "Exit code: " + process.ExitCode +
                                              System.Environment.NewLine + sr.ReadToEnd();
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
               else
               {
                  string errorMessage = "Could not find the TTS-wrapper: '" + applicationName + "'";
                  Debug.LogError(errorMessage);
                  onErrorInfo(wrapper, errorMessage);
               }
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

                  if (System.IO.File.Exists(applicationName))
                  {
                     string voiceName = getVoiceName(wrapper);
                     int calculatedRate = calculateRate(wrapper.Rate);
                     int calculatedVolume = calculateVolume(wrapper.Volume);

                     string outputFile = getOutputFile(wrapper.Uid);

                     //string args = $"--speakToFile -text \"{prepareText(wrapper)}\" -file \"{outputFile.Replace('"', '\'')}\" -rate {calculatedRate} -volume {calculatedVolume} -voice \"{voiceName.Replace('"', '\'')}\"";
                     string args = "--speakToFile " +
                                   "-text \"" + prepareText(wrapper) + "\" " +
                                   "-file \"" + outputFile.Replace('"', '\'') + "\" " +
                                   "-rate " + calculatedRate + " " +
                                   "-volume " + calculatedVolume + " " +
                                   "-voice \"" + voiceName.Replace('"', '\'') + '"';

                     if (Util.Config.DEBUG)
                        Debug.Log("Process arguments: " + args);
#if ENABLE_IL2CPP
                     Common.Util.CTProcess process = new Common.Util.CTProcess();
#else
                     System.Diagnostics.Process process = new System.Diagnostics.Process();
#endif
                     process.StartInfo.FileName = applicationName;
                     process.StartInfo.Arguments = args;
#if ENABLE_IL2CPP
                     System.Threading.Thread worker = new System.Threading.Thread(() => startProcess(ref process, 0, false, false, false, false)) {Name = wrapper.Uid};
#else
                     System.Threading.Thread worker =
                        new System.Threading.Thread(() => startProcess(ref process)) {Name = wrapper.Uid};
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
                        yield return playAudioFile(wrapper, Util.Constants.PREFIX_FILE + outputFile,
                           outputFile);
                     }
                     else
                     {
                        using (System.IO.StreamReader sr = process.StandardError)
                        {
                           string errorMessage =
                              "Could not speak the text: " + wrapper + System.Environment.NewLine +
                              "Exit code: " + process.ExitCode + System.Environment.NewLine + sr.ReadToEnd();
                           Debug.LogError(errorMessage);
                           onErrorInfo(wrapper, errorMessage);
                        }
                     }

                     process.Dispose();
                  }
                  else
                  {
                     string errorMessage = "Could not find the TTS-wrapper: '" + applicationName + "'";
                     Debug.LogError(errorMessage);
                     onErrorInfo(wrapper, errorMessage);
                  }
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

               if (System.IO.File.Exists(applicationName))
               {
                  string voiceName = getVoiceName(wrapper);
                  int calculatedRate = calculateRate(wrapper.Rate);
                  int calculatedVolume = calculateVolume(wrapper.Volume);

                  string outputFile = getOutputFile(wrapper.Uid);

                  string args = "--speakToFile " +
                                "-text \"" + prepareText(wrapper) + "\" " +
                                "-file \"" + outputFile.Replace('"', '\'') + "\" " +
                                "-rate " + calculatedRate + " " +
                                "-volume " + calculatedVolume + " " +
                                "-voice \"" + voiceName.Replace('"', '\'') + '"';

                  if (Util.Config.DEBUG)
                     Debug.Log("Process arguments: " + args);
#if ENABLE_IL2CPP
                        Common.Util.CTProcess process = new Common.Util.CTProcess();
#else
                  System.Diagnostics.Process process = new System.Diagnostics.Process();
#endif
                  process.StartInfo.FileName = applicationName;
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
               else
               {
                  string errorMessage = "Could not find the TTS-wrapper: '" + applicationName + "'";
                  Debug.LogError(errorMessage);
                  onErrorInfo(wrapper, errorMessage);
               }
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
         if (System.IO.File.Exists(applicationName))
         {
            System.Collections.Generic.List<Model.Voice>
               voices = new System.Collections.Generic.List<Model.Voice>();
#if ENABLE_IL2CPP
            Common.Util.CTProcess process = new Common.Util.CTProcess();
#else
            System.Diagnostics.Process process = new System.Diagnostics.Process();
#endif
            process.StartInfo.FileName = applicationName;
            process.StartInfo.Arguments = "--voices";

            System.Threading.Thread worker = new System.Threading.Thread(() =>
               startProcess(ref process, Util.Constants.DEFAULT_TTS_KILL_TIME));
            worker.Start();

            do
            {
               yield return null;
            } while (worker.IsAlive || !process.HasExited);

            if (process.ExitCode == 0)
            {
               using (System.IO.StreamReader streamReader = process.StandardOutput)
               {
                  while (!streamReader.EndOfStream)
                  {
                     var reply = streamReader.ReadLine();

                     if (Util.Config.DEBUG)
                        Debug.Log("reply: " + reply);

                     if (!string.IsNullOrEmpty(reply))
                     {
                        if (reply.StartsWith(idVoice))
                        {
                           string[] splittedString = reply.Split(splitChar,
                              System.StringSplitOptions.RemoveEmptyEntries);

                           if (splittedString.Length == 6)
                           {
                              //if (!splittedString[1].CTContains("espeak")) //ignore eSpeak voices
                              //{
                                 voices.Add(new Model.Voice(splittedString[1], splittedString[2],
                                    Util.Helper.StringToGender(splittedString[3]), splittedString[4],
                                    splittedString[5]));
                              //}
                           }
                           else
                           {
                              Debug.LogWarning("Voice is invalid: " + reply);
                           }
                        }
                     }
                  }
               }
            }
            else
            {
               using (System.IO.StreamReader sr = process.StandardError)
               {
                  string errorMessage = "Could not get any voices: " + process.ExitCode +
                                        System.Environment.NewLine + sr.ReadToEnd();
                  Debug.LogError(errorMessage);
                  onErrorInfo(null, errorMessage);
               }
            }

            process.Dispose();

            cachedVoices = voices.OrderBy(s => s.Name).ToList();

            if (Util.Constants.DEV_DEBUG)
               Debug.Log("Voices read: " + cachedVoices.CTDump());
         }
         else
         {
            string errorMessage = "Could not find the TTS-wrapper: '" + applicationName + "'";
            Debug.LogError(errorMessage);
            onErrorInfo(null, errorMessage);
         }

         onVoicesReady();
      }

#if ENABLE_IL2CPP
        private void readSpeakNativeStream(ref Common.Util.CTProcess process, ref string[] speechTextArray, out int wordIndex, out string phoneme, out string viseme, out bool start, bool redirectOutputData
 = true, bool redirectErrorData = true)
#else
      private static void readSpeakNativeStream(ref System.Diagnostics.Process process, ref string[] speechTextArray,
         out int wordIndex, out string phoneme, out string viseme, out bool start, bool redirectOutputData = true,
         bool redirectErrorData = true)
#endif
      {
         wordIndex = 0;
         phoneme = string.Empty;
         viseme = string.Empty;
         start = false;

         process.StartInfo.CreateNoWindow = true;
         process.StartInfo.RedirectStandardOutput = redirectOutputData;
         process.StartInfo.RedirectStandardError = redirectErrorData;
         process.StartInfo.UseShellExecute = false;
         process.StartInfo.StandardErrorEncoding =
            process.StartInfo.StandardOutputEncoding = System.Text.Encoding.UTF8;

         try
         {
            process.Start();

            using (System.IO.StreamReader streamReader = process.StandardOutput)
            {
               var reply = streamReader.ReadLine();
               if (!string.IsNullOrEmpty(reply) && idSpeak.Equals(reply))
               {
                  while (!process.HasExited)
                  {
                     reply = streamReader.ReadLine();

                     if (!string.IsNullOrEmpty(reply))
                     {
                        if (reply.StartsWith(idWord))
                        {
                           if (wordIndex < speechTextArray.Length)
                           {
                              if (speechTextArray[wordIndex].Equals("-"))
                              {
                                 wordIndex++;
                              }

                              wordIndex++;
                           }
                        }
                        else if (reply.StartsWith(idPhoneme))
                        {
                           string[] splittedString = reply.Split(splitChar,
                              System.StringSplitOptions.RemoveEmptyEntries);

                           if (splittedString.Length > 1)
                           {
                              phoneme = splittedString[1];
                           }
                        }
                        else if (reply.StartsWith(idViseme))
                        {
                           string[] splittedString = reply.Split(splitChar,
                              System.StringSplitOptions.RemoveEmptyEntries);

                           if (splittedString.Length > 1)
                           {
                              viseme = splittedString[1];
                           }
                        }
                        else if (reply.Equals(idStart))
                        {
                           start = true;
                        }
                     }
                  }
               }
               else
               {
                  if (process.StartInfo.RedirectStandardOutput)
                     Debug.LogError("Unexpected process output: " + reply + System.Environment.NewLine +
                                    streamReader.ReadToEnd());
               }
            }
         }
         catch (System.Exception ex)
         {
            Debug.LogError("Could not speak: " + ex);
         }
      }

      private string applicationName
      {
         get
         {
            string appName;

            if (Util.Helper.isEditor)
            {
               if (Util.Config.ENFORCE_32BIT_WINDOWS)
               {
                  appName = dataPath + Util.Config.TTS_WINDOWS_EDITOR_x86;
               }
               else
               {
                  appName = dataPath + Util.Config.TTS_WINDOWS_EDITOR;
               }
            }
            else
            {
               appName = dataPath + Util.Config.TTS_WINDOWS_BUILD;
            }

            if (appName.Contains("'"))
               Debug.LogError(
                  "The path to the application contains an apostrophe and the TTS-wrapper will therefore not work: " +
                  appName);

            return appName;
         }
      }

      private static string prepareText(Model.Wrapper wrapper)
      {
         //TEST
         //wrapper.ForceSSML = false;

         if (wrapper.ForceSSML && !Speaker.isAutoClearTags)
         {
            System.Text.StringBuilder sbXML = new System.Text.StringBuilder();

            sbXML.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
            sbXML.Append("<speak version=\"1.0\" xmlns=\"http://www.w3.org/2001/10/synthesis\" xml:lang=\"");
            sbXML.Append(wrapper.Voice == null ? "en-US" : wrapper.Voice.Culture);
            sbXML.Append("\">");

            float _pitch = wrapper.Pitch - 1f;

            if (Mathf.Abs(_pitch) > Common.Util.BaseConstants.FLOAT_TOLERANCE)
            {
               sbXML.Append("<prosody pitch='");

               sbXML.Append(_pitch > 0f
                  ? _pitch.ToString("+#0%", Util.Helper.BaseCulture)
                  : _pitch.ToString("#0%", Util.Helper.BaseCulture));

               sbXML.Append("'>");
            }

            sbXML.Append(wrapper.Text);

            if (Mathf.Abs(_pitch) > Common.Util.BaseConstants.FLOAT_TOLERANCE)
               sbXML.Append("</prosody>");

            sbXML.Append("</speak>");

            return getValidXML(sbXML.ToString().Replace('"', '\''));
         }

         return wrapper.Text.Replace('"', '\'');
      }

      private static int calculateVolume(float volume)
      {
         return Mathf.Clamp((int)(100 * volume), 0, 100);
      }

      private static int calculateRate(float rate)
      {
         //allowed range: 0 - 3f - all other values were cropped
         int result = 0;

         if (Mathf.Abs(rate - 1f) > Common.Util.BaseConstants.FLOAT_TOLERANCE)
         {
            //relevant?
            if (rate > 1f)
            {
               //larger than 1
               if (rate >= 2.75f)
               {
                  result = 10; //2.78
               }
               else if (rate >= 2.6f && rate < 2.75f)
               {
                  result = 9; //2.6
               }
               else if (rate >= 2.35f && rate < 2.6f)
               {
                  result = 8; //2.39
               }
               else if (rate >= 2.2f && rate < 2.35f)
               {
                  result = 7; //2.2
               }
               else if (rate >= 2f && rate < 2.2f)
               {
                  result = 6; //2
               }
               else if (rate >= 1.8f && rate < 2f)
               {
                  result = 5; //1.8
               }
               else if (rate >= 1.6f && rate < 1.8f)
               {
                  result = 4; //1.6
               }
               else if (rate >= 1.4f && rate < 1.6f)
               {
                  result = 3; //1.45
               }
               else if (rate >= 1.2f && rate < 1.4f)
               {
                  result = 2; //1.28
               }
               else if (rate > 1f && rate < 1.2f)
               {
                  result = 1; //1.14
               }
            }
            else
            {
               //smaller than 1
               if (rate <= 0.3f)
               {
                  result = -10; //0.33
               }
               else if (rate > 0.3 && rate <= 0.4f)
               {
                  result = -9; //0.375
               }
               else if (rate > 0.4 && rate <= 0.45f)
               {
                  result = -8; //0.42
               }
               else if (rate > 0.45 && rate <= 0.5f)
               {
                  result = -7; //0.47
               }
               else if (rate > 0.5 && rate <= 0.55f)
               {
                  result = -6; //0.525
               }
               else if (rate > 0.55 && rate <= 0.6f)
               {
                  result = -5; //0.585
               }
               else if (rate > 0.6 && rate <= 0.7f)
               {
                  result = -4; //0.655
               }
               else if (rate > 0.7 && rate <= 0.8f)
               {
                  result = -3; //0.732
               }
               else if (rate > 0.8 && rate <= 0.9f)
               {
                  result = -2; //0.82
               }
               else if (rate > 0.9 && rate < 1f)
               {
                  result = -1; //0.92
               }
            }
         }

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
               if (System.IO.File.Exists(applicationName))
               {
                  string voiceName = getVoiceName(wrapper);
                  int calculatedRate = calculateRate(wrapper.Rate);
                  int calculatedVolume = calculateVolume(wrapper.Volume);

                  string outputFile = getOutputFile(wrapper.Uid);

                  string args = "--speakToFile " +
                                "-text \"" + prepareText(wrapper) + "\" " +
                                "-file \"" + outputFile.Replace('"', '\'') + "\" " +
                                "-rate " + calculatedRate + " " +
                                "-volume " + calculatedVolume + " " +
                                "-voice \"" + voiceName.Replace('"', '\'') + '"';

                  if (Util.Config.DEBUG)
                     Debug.Log("Process arguments: " + args);
#if ENABLE_IL2CPP
                        Common.Util.CTProcess process = new Common.Util.CTProcess();
#else
                  System.Diagnostics.Process process = new System.Diagnostics.Process();
#endif
                  process.StartInfo.FileName = applicationName;
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

                  process.Dispose();
               }
               else
               {
                  string errorMessage = "Could not find the TTS-wrapper: '" + applicationName + "'";
                  Debug.LogError(errorMessage);
                  onErrorInfo(wrapper, errorMessage);
               }
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
               Debug.LogWarning("'wrapper.Text' is null or empty!");
            }
            else
            {
               if (System.IO.File.Exists(applicationName))
               {
                  string voiceName = getVoiceName(wrapper);
                  int calculatedRate = calculateRate(wrapper.Rate);
                  int calculatedVolume = calculateVolume(wrapper.Volume);

                  string args = "--speak " +
                                "-text \"" + prepareText(wrapper) + "\" " +
                                "-rate " + calculatedRate + " " +
                                "-volume " + calculatedVolume + " " +
                                "-voice \"" + voiceName.Replace('"', '\'') + '"';

                  if (Util.Config.DEBUG)
                     Debug.Log("Process arguments: " + args);
#if ENABLE_IL2CPP
                  Common.Util.CTProcess process = new Common.Util.CTProcess();
#else
                  System.Diagnostics.Process process = new System.Diagnostics.Process();
#endif
                  process.StartInfo.FileName = applicationName;
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
                        Debug.LogError("Could not speak the text: " + process.ExitCode +
                                       System.Environment.NewLine + sr.ReadToEnd());
                     }
                  }

                  process.Dispose();
               }
               else
               {
                  string errorMessage = "Could not find the TTS-wrapper: '" + applicationName + "'";
                  Debug.LogError(errorMessage);
                  onErrorInfo(wrapper, errorMessage);
               }
            }
         }
      }

      private void getVoicesInEditor()
      {
         if (System.IO.File.Exists(applicationName))
         {
            System.Collections.Generic.List<Model.Voice>
               voices = new System.Collections.Generic.List<Model.Voice>();
#if ENABLE_IL2CPP
            Common.Util.CTProcess process = new Common.Util.CTProcess();
#else
            System.Diagnostics.Process process = new System.Diagnostics.Process();
#endif
            process.StartInfo.FileName = applicationName;
            process.StartInfo.Arguments = "--voices";

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
                  using (System.IO.StreamReader streamReader = process.StandardOutput)
                  {
                     while (!streamReader.EndOfStream)
                     {
                        var reply = streamReader.ReadLine();

                        if (!string.IsNullOrEmpty(reply))
                        {
                           if (reply.StartsWith(idVoice))
                           {
                              string[] splittedString = reply.Split(splitChar,
                                 System.StringSplitOptions.RemoveEmptyEntries);

                              if (splittedString.Length == 6)
                              {
                                 //if (!splittedString[1].CTContains("espeak")) //ignore eSpeak voices
                                 //{
                                    voices.Add(new Model.Voice(splittedString[1], splittedString[2],
                                       Util.Helper.StringToGender(splittedString[3]), splittedString[4],
                                       splittedString[5]));
                                 //}
                              }
                              else
                              {
                                 Debug.LogWarning("Voice is invalid: " + reply);
                              }
                           }
                        }
                     }
                  }
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
         }
         else
         {
            string errorMessage = "Could not find the TTS-wrapper: '" + applicationName + "'";
            Debug.LogError(errorMessage);
         }

         onVoicesReady();
      }
#endif

      #endregion
   }
}
#endif
// © 2015-2020 crosstales LLC (https://www.crosstales.com)