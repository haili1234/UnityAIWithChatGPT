using UnityEngine;

namespace Crosstales.RTVoice.Tool
{
   /// <summary>Loudspeaker for an AudioSource.</summary>
   [RequireComponent(typeof(AudioSource))]
   [HelpURL("https://www.crosstales.com/media/data/assets/rtvoice/api/class_crosstales_1_1_r_t_voice_1_1_tool_1_1_loudspeaker.html")]
   public class Loudspeaker : MonoBehaviour
   {
      #region Variables

      /// <summary>Origin AudioSource.</summary>
      [Tooltip("Origin AudioSource.")] public AudioSource Source;

      /// <summary>Synchronize with the origin (default: false).</summary>
      [Tooltip("Synchronize with the origin (default: false).")] public bool Synchronized = false;

      /// <summary>Silence the origin (default: true).</summary>
      [Tooltip("Silence the origin (default: true).")] public bool SilenceSource = true;

      private AudioSource audioSource;

      private bool stopped = true;

      #endregion


      #region Properties

      /// <summary>Synchronize with the origin (main use is for UI).</summary>
      public bool isSynchronized
      {
         get { return Synchronized; }

         set { Synchronized = value; }
      }

      /// <summary>Silence the origin (main use is for UI).</summary>
      public bool isSilenceSource
      {
         get { return SilenceSource; }

         set { SilenceSource = value; }
      }

      #endregion


      #region MonoBehaviour methods

      public void Awake()
      {
         audioSource = GetComponent<AudioSource>();
         audioSource.playOnAwake = false;
         audioSource.loop = false;
         audioSource.Stop(); //always stop the AudioSource at startup
      }

      public void Start()
      {
         if (Source == null)
         {
            Debug.LogWarning("No 'Source' added to the Loudspeaker!");
         }
      }

      public void Update()
      {
         if (Util.Helper.hasActiveClip(Source))
         {
            if (stopped)
            {
               audioSource.loop = Source.loop;
               audioSource.clip = Source.clip;

               audioSource.Play();

               stopped = false;

               if (SilenceSource)
               {
                  Source.volume = 0f;
               }
            }
         }
         else
         {
            if (!stopped)
            {
               audioSource.Stop();
               audioSource.clip = null;
               stopped = true;
            }
         }
      }

      public void FixedUpdate()
      {
         if (Synchronized && Util.Helper.hasActiveClip(Source))
         {
            audioSource.timeSamples = Source.timeSamples;
         }
      }

      public void OnDisable()
      {
         audioSource.Stop();
         audioSource.clip = null;
         stopped = true;
      }

      #endregion
   }
}
// © 2016-2020 crosstales LLC (https://www.crosstales.com)