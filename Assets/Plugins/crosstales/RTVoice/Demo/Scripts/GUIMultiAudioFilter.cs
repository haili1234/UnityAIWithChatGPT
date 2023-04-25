using UnityEngine;
using UnityEngine.UI;

namespace Crosstales.RTVoice.Demo
{
   /// <summary>Simple GUI for audio filters on multiple objects.</summary>
   [HelpURL("https://www.crosstales.com/media/data/assets/rtvoice/api/class_crosstales_1_1_r_t_voice_1_1_demo_1_1_g_u_i_multi_audio_filter.html")]
   public class GUIMultiAudioFilter : MonoBehaviour
   {
      #region Variables

      [Header("Audio Sources")] public System.Collections.Generic.List<AudioSource> Sources = new System.Collections.Generic.List<AudioSource>();

      [Header("Filters")] public System.Collections.Generic.List<AudioReverbFilter> ReverbFilters = new System.Collections.Generic.List<AudioReverbFilter>();
      public System.Collections.Generic.List<AudioChorusFilter> ChorusFilters = new System.Collections.Generic.List<AudioChorusFilter>();
      public System.Collections.Generic.List<AudioEchoFilter> EchoFilters = new System.Collections.Generic.List<AudioEchoFilter>();
      public System.Collections.Generic.List<AudioDistortionFilter> DistortionFilters = new System.Collections.Generic.List<AudioDistortionFilter>();
      public System.Collections.Generic.List<AudioLowPassFilter> LowPassFilters = new System.Collections.Generic.List<AudioLowPassFilter>();
      public System.Collections.Generic.List<AudioHighPassFilter> HighPassFilters = new System.Collections.Generic.List<AudioHighPassFilter>();

      [Header("UI Objects")] public Text Distortion;
      public Text Lowpass;
      public Text Highpass;
      public Text Volume;
      public Text Pitch;

      public Dropdown ReverbFilterDropdown;

      private readonly System.Collections.Generic.List<AudioReverbPreset> reverbPresets = new System.Collections.Generic.List<AudioReverbPreset>();

      #endregion


      #region MonoBehaviour methods

      public void Start()
      {
         System.Collections.Generic.List<Dropdown.OptionData> options = new System.Collections.Generic.List<Dropdown.OptionData>();

         foreach (AudioReverbPreset arp in System.Enum.GetValues(typeof(AudioReverbPreset)))
         {
            options.Add(new Dropdown.OptionData(arp.ToString()));

            reverbPresets.Add(arp);
         }

         if (ReverbFilterDropdown != null)
         {
            ReverbFilterDropdown.ClearOptions();
            ReverbFilterDropdown.AddOptions(options);
         }
      }

      #endregion


      #region Public methods

      public void ResetFilters()
      {
         foreach (AudioSource source in Sources)
         {
            source.volume = 1f;
            source.pitch = 1f;
         }

         foreach (AudioReverbFilter reverbFilter in ReverbFilters)
         {
            reverbFilter.reverbPreset = reverbPresets[0];
         }

         foreach (AudioChorusFilter chorusFilter in ChorusFilters)
         {
            chorusFilter.enabled = false;
         }

         foreach (AudioEchoFilter echoFilter in EchoFilters)
         {
            echoFilter.enabled = false;
         }

         foreach (AudioDistortionFilter distortionFilter in DistortionFilters)
         {
            distortionFilter.distortionLevel = 0.5f;
            distortionFilter.enabled = false;
         }

         foreach (AudioLowPassFilter lowPassFilter in LowPassFilters)
         {
            lowPassFilter.cutoffFrequency = 5000;
            lowPassFilter.enabled = false;
         }

         foreach (AudioHighPassFilter highPassFilter in HighPassFilters)
         {
            highPassFilter.cutoffFrequency = 5000;
            highPassFilter.enabled = false;
         }
      }

      public void ClearFilters()
      {
         Sources.Clear();
         ReverbFilters.Clear();
         ChorusFilters.Clear();
         EchoFilters.Clear();
         DistortionFilters.Clear();
         LowPassFilters.Clear();
         HighPassFilters.Clear();
      }

      public void ReverbFilterDropdownChanged(System.Int32 index)
      {
         foreach (AudioReverbFilter reverbFilter in ReverbFilters)
         {
            reverbFilter.reverbPreset = reverbPresets[index];
         }
      }

      public void ChorusFilterEnabled(bool isEnabled)
      {
         foreach (AudioChorusFilter chorusFilter in ChorusFilters)
         {
            chorusFilter.enabled = isEnabled;
         }
      }

      public void EchoFilterEnabled(bool isEnabled)
      {
         foreach (AudioEchoFilter echoFilter in EchoFilters)
         {
            echoFilter.enabled = isEnabled;
         }
      }

      public void DistortionFilterEnabled(bool isEnabled)
      {
         foreach (AudioDistortionFilter distortionFilter in DistortionFilters)
         {
            distortionFilter.enabled = isEnabled;
         }
      }

      public void DistortionFilterChanged(float value)
      {
         foreach (AudioDistortionFilter distortionFilter in DistortionFilters)
         {
            distortionFilter.distortionLevel = value;
         }

         Distortion.text = value.ToString("0.00");
      }

      public void LowPassFilterEnabled(bool isEnabled)
      {
         foreach (AudioLowPassFilter lowPassFilter in LowPassFilters)
         {
            lowPassFilter.enabled = isEnabled;
         }
      }

      public void LowPassFilterChanged(float value)
      {
         foreach (AudioLowPassFilter lowPassFilter in LowPassFilters)
         {
            lowPassFilter.cutoffFrequency = value;
         }

         Lowpass.text = value.ToString();
      }

      public void HighPassFilterEnabled(bool isEnabled)
      {
         foreach (AudioHighPassFilter highPassFilter in HighPassFilters)
         {
            highPassFilter.enabled = isEnabled;
         }
      }

      public void HighPassFilterChanged(float value)
      {
         foreach (AudioHighPassFilter highPassFilter in HighPassFilters)
         {
            highPassFilter.cutoffFrequency = value;
         }

         Highpass.text = value.ToString();
      }

      public void VolumeChanged(float value)
      {
         foreach (AudioSource source in Sources)
         {
            source.volume = value;
         }

         Volume.text = value.ToString("0.00");
      }

      public void PitchChanged(float value)
      {
         foreach (AudioSource source in Sources)
         {
            source.pitch = value;
         }

         Pitch.text = value.ToString("0.00");
      }

      #endregion
   }
}
// © 2016-2020 crosstales LLC (https://www.crosstales.com)