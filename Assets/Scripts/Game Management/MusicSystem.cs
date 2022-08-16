using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSystem : MonoBehaviour
{
    public static MusicSystem instance;
    public AudioClip currentSong;
    public AudioClip[] songs; // add peace and war time later

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        GetComponent<AudioSource>().Stop();
        GetComponent<AudioSource>().clip = songs[Random.Range(0, songs.Length)];
        GetComponent<AudioSource>().Play();

        currentSong = GetComponent<AudioSource>().clip;

    }

    private void Update()
    {
        if (!GetComponent<AudioSource>().isPlaying) NextSong();
    }



    void NextSong()
    {
        foreach(AudioClip song in songs)
        {
            if (song != currentSong)
            {
                GetComponent<AudioSource>().clip = song;
                GetComponent<AudioSource>().Play();

                currentSong = song;

            }
        }
    }
}
