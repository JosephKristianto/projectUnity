using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using VRBeats;

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
        public Side side;
    }

    public class NoteTimeWrapper
    {
        public List<NoteTime> data;  
    }

    public GameObject notePrefab;  // Prefab for the notes
    public Transform[] spawnPoints; // The spawn positions for notes (e.g. 5 lanes)
    public float noteSpeed = 5f;  // Speed at which the note moves
    public List<NoteTime> noteTimes;  // List of beat times (seconds) when notes should appear

    private int noteIndex = 0;

    public TMP_Text musicTimeTMP;

    [TextArea(4,5)]
    public string noteJsonConvert; 
    [TextArea(4,5)]
    public string noteJsonToConvert; 

    PlayableDirector musicDirector;
    void Start()
    {
        musicDirector = GetComponent<PlayableDirector>();

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
                                noteTimes.Find(x => x.time == (float)clip.time).blocks.Add(new Block() { lane = laneCounter, side= marker.spawInfo.colorSide == ColorSide.Left ? Block.Side.Left: Block.Side.Right});
                            }
                            else
                            {
                                noteTimes.Add(new NoteTime() { time = (float)clip.time, blocks = new List<Block>() { new Block() { lane = laneCounter , side = marker.spawInfo.colorSide == ColorSide.Left ? Block.Side.Left : Block.Side.Right } } });
                            }
                        }
                   

                    }

                    laneCounter++;

                }
            }

            noteTimes = noteTimes.OrderBy(x => x.time).ToList();
        }

      



    }



    void Update()
    {
        musicTimeTMP.text = musicDirector.time.ToString("0.00");
        if (noteIndex < noteTimes.Count)
        {
            // Get the distance the note needs to travel along the z-axis
            float distance = spawnPoints[0].position.z;  // Assuming notes move along the z-axis

            // Calculate the time it will take to travel that distance
            float timeToTravel = distance / noteSpeed;

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
            GameObject note = Instantiate(notePrefab, spawnPoints[block.lane].position, Quaternion.identity);
            note.GetComponent<Note>().Initialize(noteSpeed);
            note.GetComponent<VRNoteBlock>().InitializeBlock(block.side == Block.Side.Left);
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
