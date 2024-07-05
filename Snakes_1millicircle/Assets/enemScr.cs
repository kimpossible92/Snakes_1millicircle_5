using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class enemScr : MonoBehaviour
{
    public Left4Spawn GetSpawner;
    [SerializeField] UnityEngine.UI.Slider PlayerHealthHUD;
    private HealthSlider2D_Script healthCallRef;

    internal void SetSliders(UnityEngine.UI.Slider playerHealthHUD)
    {
        PlayerHealthHUD = playerHealthHUD;
        healthCallRef = PlayerHealthHUD.GetComponent<HealthSlider2D_Script>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerHealthHUD != null) healthCallRef = PlayerHealthHUD.GetComponent<HealthSlider2D_Script>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
