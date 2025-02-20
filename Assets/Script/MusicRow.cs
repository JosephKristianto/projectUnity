using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MusicRow : MonoBehaviour
{
    // Start is called before the first frame update
    public TMP_Text titleText;
    public TMP_Text artistText;
    public TMP_Text difficultyText;
    public TMP_Text durationText;

    public GameObject tmpHighscore;
    public TMP_Text highScore;
    public MenuController.MusicData musicData;
    public void Setup(MenuController.MusicData musicData)
    {
        this.musicData = musicData; 
        titleText.text = musicData.musicTitle;
        artistText.text = musicData.musicArtist;
        difficultyText.text = musicData.difficulty;
        durationText.text = musicData.time;

        if (PlayerPrefs.HasKey(musicData.musicTitle))
        {
            tmpHighscore.gameObject.SetActive(true);
            highScore.text = PlayerPrefs.GetInt(musicData.musicTitle).ToString();
            highScore.gameObject.SetActive(true);

        }
        else
        {
            tmpHighscore.gameObject.SetActive(false);
            highScore.gameObject.SetActive(false);

        }
    }

    public void ReloadData()
    {
        if (PlayerPrefs.HasKey(musicData.musicTitle))
        {
            tmpHighscore.gameObject.SetActive(true);
            highScore.text = PlayerPrefs.GetInt(musicData.musicTitle).ToString();
            highScore.gameObject.SetActive(true);

        }
        else
        {
            tmpHighscore.gameObject.SetActive(false);
            highScore.gameObject.SetActive(false);

        }
    }
}
