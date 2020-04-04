using UnityEngine;
using System.Collections;
using System;

public class DayCycle : MonoBehaviour
{

    public Light sun;
    public float secondsInFullDay = 86400f;
    [Range(0, 1)]
    public float currentTimeOfDay = 0;
    [HideInInspector]
    public float timeMultiplier = 1f;

    public AudioClip DayMusic;
    public AudioClip NightMusic;

    public AudioSource Music;

    float sunInitialIntensity;

    float TimeOffset = 0f;

    void Start()
    {
        sunInitialIntensity = sun.intensity;

        Music = FindObjectOfType<SaveManager>().GetComponent<AudioSource>();

        TimeOffset = Math.Abs((float)DateTime.Now.Subtract(NTP.GetNetworkTime("time.google.com")).TotalMilliseconds);
    }

    void Update()
    {
        UpdateSun();

        DateTime current = DateTime.Now.AddMilliseconds(-TimeOffset);

        int currentSecondsInRealDay = ((current.Hour * 3600) + (current.Minute * 60) + current.Second);

        currentTimeOfDay = (currentSecondsInRealDay / secondsInFullDay) * timeMultiplier;

        if (currentTimeOfDay >= 1)
        {
            currentTimeOfDay = 0;
        }
    }

    void UpdateSun()
    {
        sun.transform.localRotation = Quaternion.Euler((currentTimeOfDay * 360f) - 90, 170, 0);

        float intensityMultiplier = 1;
        if (currentTimeOfDay <= 0.23f || currentTimeOfDay >= 0.75f)
        {
            //intensityMultiplier = 0;
        }
        else if (currentTimeOfDay <= 0.25f)
        {
            if (Music.clip != DayMusic)
            {
                Music.clip = DayMusic;
                Music.Play();
            }

            intensityMultiplier = Mathf.Clamp01((currentTimeOfDay - 0.23f) * (1 / 0.02f));
        }
        else if (currentTimeOfDay >= 0.73f)
        {
            if(Music.clip != NightMusic)
            {
                Music.clip = NightMusic;
                Music.Play();
            }

            intensityMultiplier = Mathf.Clamp01(1 - ((currentTimeOfDay - 0.73f) * (1 / 0.02f)));
        }

        sun.intensity = sunInitialIntensity * intensityMultiplier;
    }
}