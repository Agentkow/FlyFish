using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMaker : MonoBehaviour
{
   public AudioSource PlayClipAt(AudioClip clip, Vector3 pos)
    {
        var tempGO = new GameObject("TempAudio"); // create the temp object

        tempGO.transform.position = pos; // set its position

        var aSource = tempGO.AddComponent<AudioSource>(); // add an audio source

        aSource.clip = clip; // define the clip
                             // set other aSource properties here, if desired

        aSource.Play(); // start the sound

        Destroy(tempGO, clip.length); // destroy object after clip duration

        return aSource; // return the AudioSource reference
    }

    public void SoundMake(AudioClip clip, int choice)
    {
        var sfx = PlayClipAt(clip, Camera.main.transform.position);
        switch (choice)
        {
            case 0:
                float rand = Random.Range(0.5f, 1.5f);
                sfx.pitch = rand;
                break;
            case 1:
                sfx.volume = 0.5f;
                break;
            default:
                break;
        }
        
        
    }
}
