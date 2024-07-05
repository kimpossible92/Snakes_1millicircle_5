using RPG.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemHealth_Slider_Scr : MonoBehaviour
{
    Slider enemySlider3D;
    [SerializeField]Health3 GetHealth3;
    EnemyStatsScript enemyStatsScript;

    // Start is called before the first frame update
    void Start()
    {
        //enemyStatsScript = GameObject.FindGameObjectWithTag("Enemy").GetComponent<EnemyStatsScript>(); // in multiplayer, check if owned as well

        //enemyStatsScript = GetComponentInParent<EnemyStatsScript>();
        GetHealth3 = GetComponentInParent<Health3>();

        enemySlider3D = GetComponentInChildren<Slider>();

        enemySlider3D.maxValue = GetHealth3.maxHealth;

    }

    // Update is called once per frame
    void Update()
    {
        if (GetHealth3 != null)
        {
            enemySlider3D.value = GetHealth3.currentHealth;
            enemySlider3D.maxValue = GetHealth3.maxHealth;
        }
    }
}
