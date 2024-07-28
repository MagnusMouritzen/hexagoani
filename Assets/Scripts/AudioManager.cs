using System;
using OK.Utility;
using UnityEngine;

public class AudioManager : MonoBehaviour {
    [SerializeField] private EnumDataContainer<Selection<Sound>, Movement> movementSounds;
    [SerializeField, Tooltip("Modifies chance of getting same option twice in a row. 1 is same chance, higher means higher chance.")] 
    private float lastChosenFactor = 1f;
    [SerializeField] private Sound victorySound;
    [SerializeField] private AudioSource audioSource;

    /// <summary>
    /// Plays a random sound for the chosen movement.
    /// </summary>
    /// <param name="movement">Movement direction.</param>
    public void PlayMovementSound(Movement movement) {
        Sound sound = movementSounds[movement].GetOne(lastChosenFactor);
        PlaySound(sound);
    }
    
    public void PlayVictorySound() {
        PlaySound(victorySound);
    }

    private void PlaySound(Sound sound) {
        audioSource.PlayOneShot(sound.clip, sound.volume);
    }

    [Serializable]
    private class Sound {
        public AudioClip clip;
        public float volume = 1f;
    }
}
