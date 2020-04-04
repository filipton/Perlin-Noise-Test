using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    public Slider MusicVolume;
    public float old_value;

    public AudioSource Music;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(MusicVolume.value != old_value)
        {
            Music.volume = MusicVolume.value;

            old_value = MusicVolume.value;
        }
    }
}