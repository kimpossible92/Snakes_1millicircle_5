using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityLevelHUDVisualController_Script : MonoBehaviour
{
    [SerializeField] private Image[] QRanks;
    [SerializeField] private Image[] WRanks;
    [SerializeField] private Image[] ERanks;
    [SerializeField] private Image[] RRanks;

    int Q_Level, W_Level, E_Level, R_Level;

    [SerializeField] private Sprite emptyRankSprite;
    [SerializeField] private Sprite filledRankSprite;
    public Image W_Ability_Indicator_Image;
    public Image Q_Ability_Indicator_Image;
    public Image Q_Ability_Indicator;

    public Image W_Ability_Range;
    public GameObject W_Ability_Canvas;
    [SerializeField] private Text ShiftText;
    public void LevelAbilityHUD(string ability, int level)
    {
        switch (ability)
        {
            case ("Q"):
                Q_Level += level;
                break;
            case ("W"):
                W_Level += level;
                break;
            case ("E"):
                E_Level += level;
                break;
            case ("R"):
                R_Level += level;
                break;
        }
    }
    Vector3 W_pos0 = Vector3.zero;
    Vector3 Q_pos0 = Vector3.zero;
    [SerializeField] private bool isabilityReturn = false; 
    private void AbilityReturn()
    {
        W_Ability_Indicator_Image.rectTransform.anchoredPosition3D = W_pos0;
        Q_Ability_Indicator_Image.rectTransform.anchoredPosition3D = Q_pos0;
       //
        isabilityReturn = false;
    }
    private void Awake()
    {
        W_pos0 = W_Ability_Indicator_Image.rectTransform.anchoredPosition3D;
        Q_pos0 = Q_Ability_Indicator_Image.rectTransform.anchoredPosition3D;
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            ShiftText.color = Color.blue;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            ShiftText.color = Color.yellow;
        }
        if (!isabilityReturn)
        {
            if (W_Ability_Indicator_Image.rectTransform.anchoredPosition3D != W_pos0 ||
                Q_Ability_Indicator_Image.rectTransform.anchoredPosition3D != Q_pos0)
            {
                Invoke("AbilityReturn", 1.5f);
                isabilityReturn = true;
            }
        }
        Q_Level_Check();
        W_Level_Check();
        E_Level_Check();
        R_Level_Check();
    }

    void Q_Level_Check()
    {
        for (int i = 0; i < QRanks.Length; i++)
        {
            if (i < Q_Level)
            {
                QRanks[i].sprite = filledRankSprite;
            }
            else
                QRanks[i].sprite = emptyRankSprite;
        }
    }

    void W_Level_Check()
    {
        for (int i = 0; i < WRanks.Length; i++)
        {
            if (i < W_Level)
            {
                WRanks[i].sprite = filledRankSprite;
            }
            else
                WRanks[i].sprite = emptyRankSprite;
        }
    }

    void E_Level_Check()
    {
        for (int i = 0; i < ERanks.Length; i++)
        {
            if (i < E_Level)
            {
                ERanks[i].sprite = filledRankSprite;
            }
            else
                ERanks[i].sprite = emptyRankSprite;
        }
    }

    void R_Level_Check()
    {
        for (int i = 0; i < RRanks.Length; i++)
        {
            if (i < R_Level)
            {
                RRanks[i].sprite = filledRankSprite;
            }
            else
                RRanks[i].sprite = emptyRankSprite;
        }
    }
}
