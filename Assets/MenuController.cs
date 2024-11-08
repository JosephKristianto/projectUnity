using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{

    [System.Serializable]   
    public class MusicData
    {
        public string musicTitle;
        public string musicArtist;
        public string difficulty;
        public string time;
        public float noteSpeed;
        public float noteSpawnTime;
        public TimelineAsset musicAssets;
    }

    // Start is called before the first frame update

    [Header("Music Datas")]
    public List<MusicData> musicDatas;
    public GameObject musicRowTemplate;
    public Transform container;

    public Button playButton;
    public GameObject canvasMenu;

    public MusicData selectedMusicData = null;
    public List<GameObject> musicRows;

    private void Start()
    {
        playButton.onClick.AddListener(PlayGame);
        foreach (var item in musicDatas)
        {
            GameObject row = Instantiate(musicRowTemplate,container);
            var musicData = item;
            row.GetComponent<MusicRow>().Setup(musicData);
            row.GetComponent<Button>().onClick.AddListener(()=>SelectSong(musicData));
            musicRows.Add(row);
        }
    }


    private void Update()
    {
        playButton.interactable = selectedMusicData != null;
    }

    public void SelectSong(MusicData data)
    {
        foreach (var item in musicRows) 
        { 
            if(data == item.GetComponent<MusicRow>().musicData)
            {
                item.GetComponent<CanvasGroup>().alpha = 0.9f;
                selectedMusicData = data;
            }
            else
            {
                item.GetComponent<CanvasGroup>().alpha = 0.7f;
            }

        }
    }


    public void PlayGame()
    {
        if (selectedMusicData.musicAssets == null)
            return;
        canvasMenu.gameObject.SetActive(false);
        FindObjectOfType<BeatManager>().noteSpawnTime = selectedMusicData.noteSpawnTime;
        FindObjectOfType<BeatManager>().noteSpeed = selectedMusicData.noteSpeed;
        FindObjectOfType<BeatManager>().musicDirector.playableAsset = selectedMusicData.musicAssets;
        FindObjectOfType<BeatManager>().StartMusic();
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
