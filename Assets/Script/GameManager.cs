using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static GameManager Instance;
    public int combo;
    public int score = 0;

    public UnityEvent<int> OnCorrectAnswer;
    public UnityEvent OnWrongAnswer;


    public TMP_Text TMP_Score;
    public TMP_Text TMP_Combo;
    private void Awake()
    {
        Instance = this;
    }

    public void SetAnswer(bool answer)
    {
        if (answer)
        {
            score += combo > 200 ? 320 : combo > 100 ? 310 : 300;
            combo++;
        }
        else
        {
            combo = 0;
        }

        TMP_Score.text = score.ToString();
        TMP_Combo.text = combo == 0? "" : combo.ToString() + "x Combo!";

    }
}
