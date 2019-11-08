using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AudioSystem : MonoBehaviour {
	
	[System.Serializable]
	public struct AudioNode {
		public string		alias;      // Name by which the audio clip is known as
		public AudioClip	audio;		// Audio clip to be loaded into an audio source
	}

    [System.Serializable]
    public struct PlayInfo {
        // Input
        public string alias;
        public float volume;
        
        public bool b_varyPitch;
        public float minPitch;
        public float maxPitch;

        // Output
        public AudioClip    o_clip;
        public float        o_pitch;
    }

	public List<AudioNode> audioFiles;

	AudioSource targetSource;			// Which audio source to play the clips from

	private void Awake() {
		// Set the target audio source to the first found one in the children of the entity wrapper object
		targetSource = GetComponentInChildren<AudioSource>();

		Debug.Assert(targetSource, "Object with attached AudioSystem does not have any AudioSources");
	}

    public PlayInfo InterpretPlayString(string a_str, bool b_randClip = false) {
        /// Interpret audio string into play information with JSON
        PlayInfo playInfo = default(PlayInfo);

        try {
            playInfo = JsonUtility.FromJson<PlayInfo>(a_str);
        }
        catch (System.Exception e) { Debug.LogError(e.Message + " " + "'" + a_str + "'" + "\n" +
            "NOTE: Audio string must be formatted like: {\"alias\":\"ALIAS\",\"volume\":1}"); };
        
        // Error checking
        if (playInfo.volume == 0) Debug.LogWarning("AUDIO_SYSTEM::Audio clip will be played with a volume of 0: " + a_str);

        // Determine pitch
        playInfo.o_pitch = (playInfo.b_varyPitch) ? Random.Range(playInfo.minPitch, playInfo.maxPitch) : 1f;

        // Determine audio clip
        if (b_randClip) {
            // Find and hold onto all audio clips containing the given alias
            List<AudioNode> randClips = audioFiles.FindAll(clip => clip.alias.Contains(playInfo.alias));

            // Get random index and use it to play a random clip sound with the alias from list
            playInfo.o_clip = randClips[Random.Range(0, randClips.Count())].audio;
        }
        else {

            playInfo.o_clip = (audioFiles.Find(node => node.alias == playInfo.alias)).audio;
            Debug.Assert(playInfo.o_clip, "Attempted to play sound with alias not found in audio files of the AudioSystem: " + playInfo.alias);
        }

        return playInfo;
    }

	/**
	 * @brief Play a sound in audio files from the specified audio source only once.
	 * @param a_audioString is the string containing the audio clip name with an optional volume.
	 * @return void.
	 * */
	public void PlaySoundOnce(string a_audioString) {

        PlayInfo playInfo = InterpretPlayString(a_audioString);

        targetSource.pitch = playInfo.o_pitch;
		targetSource.PlayOneShot(playInfo.o_clip, playInfo.volume);
	}

	/**
	 * @brief Play an audio clip on the attached audio source.
	 * @param a_audioString is the string containing the audio clip name with volume and pitch variance.
     * @param a_altAudioSource is the alternative audio source to play the sound from.
	 * @return void.
	 * */
	 public void PlayClip(string a_audioString, AudioSource a_altAudioSource = null) {
        PlayInfo playInfo = InterpretPlayString(a_audioString);

        AudioSource source = a_altAudioSource;

        // No alternate audio source provided, use set target
        if (!source) { source = targetSource; }

        // Set audio source clip in order to loop sound constantly
        source.pitch = playInfo.o_pitch;
        source.clip  = playInfo.o_clip; 

		source.Play();
	}

	/**
	 * @brief Find and play a random clip from a list of clips that contain the given alias.
	 * @param a_audioString is the string containing the alias to search for audio clips with, and an optional volume.
	 * @return void.
	 * */
	public void PlayRandClip(string a_audioString) {
        PlayInfo playInfo = InterpretPlayString(a_audioString, true);

        targetSource.pitch = playInfo.o_pitch;
        targetSource.PlayOneShot(playInfo.o_clip, playInfo.volume);
    }

    public void StopCurrentClip() {
	    targetSource.Stop();
	}

    public void SetLooping(int b_loop) {
        targetSource.loop = b_loop == 0 ? false : true;
    }
}
