using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SoundMusicManager : MonoBehaviour
{
    public static SoundMusicManager instance;
    private static bool isReady = false;

    [SerializeField] private SpawnClip spawnClip;
    [SerializeField] private AudioSource sourceSound;
    [SerializeField] private AudioSource sourceMusic;

    [SerializeField] private List<AudioClip> listMusicHome;
    [SerializeField] private List<AudioClip> listMusicGameplay;

    [SerializeField] private AudioClip clipClick;
    [SerializeField] private AudioClip clipTapPaintColor;
    [SerializeField] private AudioClip clipBooster;
    [SerializeField] private AudioClip clipCompletedLevel;
    [SerializeField] private AudioClip clipShowPopup;

    private StateGame stateGame;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            isReady = true;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (!isReady)
                Destroy(gameObject);
        }
        PlayMusic(StateGame.Home);
    }
    public bool GetIsSound()
    {
        return VariableSystem.Sound == 1;
    }
    public bool GetIsMusic()
    {
        return VariableSystem.Music == 1;
    }
    private void PlaySound(AudioClip clip, bool isLoop = false)
    {
        if (!GetIsSound()) return;
        if (clip != null)
        {
            if (!isLoop)
            {
                sourceSound.loop = false;
                sourceSound.PlayOneShot(clip, VariableSystem.Sound);
            }
            else
            {
                sourceSound.clip = clip;
                sourceSound.volume = VariableSystem.Sound;
                sourceSound.loop = true;
                sourceSound.Play();
            }
        }
    }

    public void PlayMusic(StateGame state)
    {
        stateGame = state;
        if (!GetIsMusic())
        {
            sourceMusic.Stop();
            return;
        }
        if (state == StateGame.Home)
            SetMusic(listMusicHome);
        else
            SetMusic(listMusicGameplay);

    }
    private void SetMusic(List<AudioClip> listMusic)
    {
        if (listMusic != null)
        {
            if (listMusic.Count <= 0) return;

            int rd = Random.Range(0, listMusic.Count);

            var clip = listMusic[rd];
            sourceMusic.clip = clip;
            sourceMusic.loop = false;
            sourceMusic.volume = 0.7f;// VariableSystem.Music;
            sourceMusic.Play();


            if (IE_MUSIC != null)
            {
                StopCoroutine(IE_MUSIC);
                IE_MUSIC = null;
            }

            IE_MUSIC = IE_Music(clip.length);
            StartCoroutine(IE_MUSIC);
        }
    }

    private IEnumerator IE_MUSIC;
    private IEnumerator IE_Music(float timeDelay)
    {
        yield return new WaitForSeconds(timeDelay);
        PlayMusic(stateGame);
    }

    #region Funct Sound
    public int maxSountTap = 10;
    public List<SpawnClip> listSpawnClip = new List<SpawnClip>();
    public void SoundTapPaintColor()
    {
        if (!GetIsSound()) return;
        if (listSpawnClip.Count >= 10) return;
        var cl = Instantiate(spawnClip, transform);
        listSpawnClip.Add(cl);
        float timeDelay = 0.001f * listSpawnClip.Count;
        cl.Spawn(clipTapPaintColor, timeDelay);
    }
    public void SoundUseBooster()
    {
        PlaySound(clipBooster);
    }

    public void SoundClickButton()
    {
        Debug.Log("sound click");
        PlaySound(clipClick);
    }
    public void SoundCompletedLevel()
    {
        PlaySound(clipCompletedLevel);
    }
    public void SoundShowPopup()
    {
        PlaySound(clipShowPopup);
    }

    #endregion
}
