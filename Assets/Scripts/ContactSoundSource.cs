using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class ContactSoundSource : MonoBehaviour
{

    private AudioSource contactAudioSource;
    // Start is called before the first frame update
    void Start()
    {
        contactAudioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision) {

        bool isHittingDomino = collision.gameObject.CompareTag("DominoObj");
        if (isHittingDomino) {
            Debug.Log("[ContactSoundSource] Velocity: " + collision.relativeVelocity.magnitude + " impluse: " + collision.impulse.magnitude);
            contactAudioSource.Play();
        }
    }
}
