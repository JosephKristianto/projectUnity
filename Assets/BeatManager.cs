using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;
using UnityEngine.UI;
using VRBeats;
using static BeatManager;
using static MenuController;

public class BeatManager : MonoBehaviour
{

    [System.Serializable]
    public class NoteTime
    {
        public float time;
        public List<Block> blocks;
    }

    [System.Serializable]
    public class Block
    {
        public enum Side
        {
            Left, Right
        }
        public int lane;
        public ColorSide side;
        public int twinLane;
        public int gripLong;
    }

    public class NoteTimeWrapper
    {
        public List<NoteTime> data;  
    }

    public GameObject notePrefab;  // Prefab for the notes
    public Transform[] spawnPoints; // The spawn positions for notes (e.g. 5 lanes)
    public float noteSpeed = 5f;  // Speed at which the note moves
    public float noteSpawnTime = 5f;  // Speed at which the note moves
    public List<NoteTime> noteTimes;  // List of beat times (seconds) when notes should appear

    private int noteIndex = 0;

    public TMP_Text musicTimeTMP;

    [TextArea(4,5)]
    public string noteJsonConvert; 
    [TextArea(4,5)]
    public string noteJsonToConvert; 

    public PlayableDirector musicDirector;

    private void Awake()
    {
        restartButton.onClick.AddListener(Restart);
        backToMenuButton.onClick.AddListener(BackToMenu);
        musicDirector = GetComponent<PlayableDirector>();

    }
    public async void StartMusic()
    {
        noteTimes.Clear();
        noteIndex = 0;
        GameManager.Instance.score = 0;
        GameManager.Instance.TMP_Score.text = "0";
        GameManager.Instance.TMP_Combo.text = "";
        normalScoreBoard.gameObject.SetActive(true);
        endingPanel.gameObject.SetActive(false);
        FindObjectsOfType<RayTracker>(true).ToList().ForEach(x => x.gameObject.SetActive(false));

        TimelineAsset timelineAsset = musicDirector.playableAsset as TimelineAsset;

        if (timelineAsset != null)
        {
            // Loop through the tracks in the timeline
            int laneCounter = 0;

            foreach (var track in timelineAsset.GetOutputTracks())
            {
                // Check if the track is an ActivationTrack
                if (track is ActivationTrack activationTrack)
                {
                    // Create a list to store keyframe times for this specific ActivationTrack
     
                    // Loop through all the clips in the ActivationTrack
                    foreach (var clip in activationTrack.GetMarkers())
                    {
                        // Add the start time of the clip (keyframe time) to the list
                        Debug.Log("cliptime : " + (float)clip.time);
                        if(clip is VR_BeatSpawnMarker marker)
                        {
                            if (noteTimes.Find(x => x.time == (float)clip.time) != null)
                            {
                                noteTimes.Find(x => x.time == (float)clip.time).blocks.Add(new Block() { lane = laneCounter, side = marker.spawInfo.colorSide , twinLane = marker.spawInfo.twinLane ,gripLong = marker.spawInfo.gripLong  });
                            }
                            else
                            {
                                noteTimes.Add(new NoteTime() { time = (float)clip.time, blocks = new List<Block>() { new Block() { lane = laneCounter, side = marker.spawInfo.colorSide, twinLane = marker.spawInfo.twinLane , gripLong = marker.spawInfo.gripLong } } });

                                
                            }
                        }
                   

                    }

                    laneCounter++;

                }
            }



            noteTimes = noteTimes.OrderBy(x => x.time).ToList();
        }

        await Task.Delay(3000);
        musicDirector.Play();
        IsPlaying = true;
        StartCoroutine(IE_WaitForEndOfSong());
    }

    [Header("Ending")]
    public GameObject endingPanel;
    public TMP_Text finalScorePanel;
    public GameObject normalScoreBoard;

    public Button restartButton;
    public Button backToMenuButton;

    public GameObject newHighScoreText;

    private IEnumerator IE_WaitForEndOfSong()

    {
        float time = 0;
        while (time < musicDirector.duration)
        {
            time += Time.deltaTime;
            yield return null;
        }
        MusicFinished(musicDirector) ;

    }
        
    private async void MusicFinished(PlayableDirector obj)
    {
        if (PlayerPrefs.HasKey(FindObjectOfType<MenuController>(true).selectedMusicData.musicTitle))
        {
            if (GameManager.Instance.score >  PlayerPrefs.GetInt(FindObjectOfType<MenuController>(true).selectedMusicData.musicTitle))
            {
                PlayerPrefs.SetInt(FindObjectOfType<MenuController>(true).selectedMusicData.musicTitle, GameManager.Instance.score);
                newHighScoreText.gameObject.SetActive(true);
            }
            else
            {
                newHighScoreText.gameObject.SetActive(false);

            }
        }
        else
        {
            PlayerPrefs.SetInt(FindObjectOfType<MenuController>(true).selectedMusicData.musicTitle, GameManager.Instance.score);

        }

        IsPlaying = false;
        await Task.Delay(2000);

        finalScorePanel.text = GameManager.Instance.score.ToString();
        normalScoreBoard.gameObject.SetActive(false);
        endingPanel.gameObject.SetActive(true);

        FindObjectsOfType<RayTracker>(true).ToList().ForEach(x => x.gameObject.SetActive(true));
        
    }

    private void OnDestroy()
    {
    }
    public void Restart()
    {

        
        StartMusic();
        FindObjectsOfType<RayTracker>(true).ToList().ForEach(x => x.gameObject.SetActive(false));

    }

    private void OnApplicationQuit()
    {
        noteTimes.Clear();
        noteIndex = 0;
        GameManager.Instance.score = 0;
        GameManager.Instance.TMP_Score.text = "0";
        GameManager.Instance.TMP_Combo.text = "";
        normalScoreBoard.gameObject.SetActive(true);
        endingPanel.gameObject.SetActive(false);
        FindObjectsOfType<RayTracker>(true).ToList().ForEach(x => x.gameObject.SetActive(false));
    }
    public void BackToMenu()
    {
        normalScoreBoard.gameObject.SetActive(false);
        endingPanel.gameObject.SetActive(false);
        FindObjectOfType<MenuController>(true).canvasMenu.SetActive(true);
        FindObjectsOfType<MusicRow>(true).ToList().ForEach(x => x.ReloadData());
    }


    bool flag_finished = false;
    public PlayState playState;
    public bool IsPlaying;
    void Update()
    {
        if (IsPlaying == false)
            return;
        musicTimeTMP.text = musicDirector.time.ToString("0.00");

        if (noteIndex < noteTimes.Count)
        {
            // Get the distance the note needs to travel along the z-axis
            float distance = spawnPoints[0].position.z;  // Assuming notes move along the z-axis

            // Calculate the time it will take to travel that distance
            float timeToTravel = distance / noteSpawnTime;

            // Check if the current time is greater than the note time minus travel time
            if (musicDirector.time >= noteTimes[noteIndex].time - timeToTravel)
            {
                SpawnNote();
                noteIndex++;
            }
        }
    }

    void SpawnNote()
    {
        // Get the list of lanes for the current note
        List<int> lanes = new List<int>();
        foreach(var block in noteTimes[noteIndex].blocks)
        {
            lanes.Add(block.lane);
        }

        // Spawn a note in each specified lane

        foreach(var block in noteTimes[noteIndex].blocks)
        {
            if(block.side == ColorSide.Twin)
            {
                GameObject note1 = Instantiate(notePrefab, spawnPoints[block.lane].position, Quaternion.identity);
                GameObject note2 = Instantiate(notePrefab, spawnPoints[block.twinLane].position, Quaternion.identity);

                note1.GetComponent<Note>().Initialize(noteSpeed);
                note1.GetComponent<VRNoteBlock>().InitializeBlock(block.side , note2); 
                
                note2.GetComponent<Note>().Initialize(noteSpeed);
                note2.GetComponent<VRNoteBlock>().InitializeBlock(block.side , note1);
            }
            else
            {
                GameObject note = Instantiate(notePrefab, spawnPoints[block.lane].position, Quaternion.identity);
                note.GetComponent<Note>().Initialize(noteSpeed);
                note.GetComponent<VRNoteBlock>().InitializeBlock(block.side , gripLong: block.gripLong);
            }
            
        }
    
    }

    [ContextMenu("Convert To Json")]
    public void SetAsJson()
    {
        NoteTimeWrapper wrapper = new NoteTimeWrapper();
        wrapper.data = noteTimes;
        noteJsonConvert = JsonUtility.ToJson(wrapper);
    }    
    [ContextMenu("Convert From Json")]
    public void SetToJson()
    {
        NoteTimeWrapper wrapper = JsonUtility.FromJson<NoteTimeWrapper>(noteJsonToConvert);
        noteTimes = wrapper.data;
    }
}
