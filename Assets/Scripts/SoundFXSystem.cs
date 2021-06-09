using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFXSystem : MonoBehaviour
{

    [SerializeField]
    private AudioSource placementClip;
    [SerializeField]
    private AudioSource placeErrorClip;


    public void PlayPlacementAudio() {
        placementClip.Play();
    }

    public void PlayPlaceErrorAudio() {
        placeErrorClip.Play();
    }
}
