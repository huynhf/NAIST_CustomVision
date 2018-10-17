using UnityEngine;
using System.Collections;

public class AudioPlay : MonoBehaviour
{
    /// <summary>
    /// Allows this class to behave like a singleton
    /// </summary>
    public static AudioPlay Instance;

    [SerializeField]
    public AudioClip[] audioClips;

    private AudioSource aSrc;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Add an AudioSource component and set up some defaults
        aSrc = gameObject.AddComponent<AudioSource>();
        aSrc.playOnAwake = false;
        aSrc.spatialize = true;
        aSrc.spatialBlend = 1.0f;
        aSrc.dopplerLevel = 0.0f;
        aSrc.rolloffMode = AudioRolloffMode.Logarithmic;
        aSrc.maxDistance = 20f;
    }

    public void Play(string audioName)
    {
        if (audioName == null)
            Debug.Log("No audio clip specified\n");

        else
        {
            for (int i=0;i<audioClips.Length;i++)
            {
                if(audioName == audioClips[i].name)
                {
                    // Load the Sphere sounds from the Resources folder
                    aSrc.clip = audioClips[i]; //Resources.Load<AudioClip>("Impact");
                    aSrc.Play();
                    return;
                }
            }
            //if we arrive here, no audio corresponds to the specified name
            Debug.Log("Audio clip loading Failed\n");
        }            
    }
}
