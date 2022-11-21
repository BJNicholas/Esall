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
        NextSong();

    }

    private void Update()
    {
        if (!GetComponent<AudioSource>().isPlaying) NextSong();
    }



    void NextSong()
    {
        int randomIndex = Random.Range(0, songs.Length);

        foreach(AudioClip song in songs)
        {
            if (songs[randomIndex] == song)
            {
                if (song == currentSong)
                {
                    NextSong();//repeat until not the same
                }
                else
                {
                    print("chose song: " + song);
                    GetComponent<AudioSource>().clip = song;
                    GetComponent<AudioSource>().Play();

                    currentSong = song;
                }
            }
        }
    }
}
