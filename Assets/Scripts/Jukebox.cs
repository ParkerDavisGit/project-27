using UnityEngine;

public class Jukebox : MonoBehaviour
{
    public AudioSource[] tracks;
    public int activeTrack = -1;

    public void playTrack(int _track)
    {
        foreach (AudioSource track in tracks)
        {
            track.Pause();
        }

        activeTrack = _track;
        tracks[activeTrack].Play();
    }

    public void Start()
    {
        playTrack(0);
    }
}
