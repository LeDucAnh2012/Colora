using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnClip : PanelBase
{
    [SerializeField] private AudioSource audiSource;
    public void Spawn(AudioClip clip,float timeDelay)
    {
        DelayCallback(() =>
        {
            audiSource.clip = clip;
            audiSource.Play();
            DelayCallback(() =>
            {
                SoundMusicManager.instance.listSpawnClip.Remove(this);
                Destroy(gameObject);
            },0.093f);
        }, timeDelay);


    }
}


