using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Left4Spawn : MonoBehaviour
{
    [SerializeField]
    private GameObject _object;
    int level = 0;
    [SerializeField]
    private Transform _parent;

    [SerializeField]
    private Vector2 _spawnPeriodRange;

    [SerializeField]
    private Vector2 _spawnDelayRange;

    [SerializeField]
    private bool _autoStart = true;
    [SerializeField] private bool _autoSpawn22 = false;
    [SerializeField]
    private Sprite[] Objects;
    [SerializeField] private int anothermovement = 0;
    public Slider PlayerHealthHUD;
    private int count = 0;
    public void lvlplus()
    {
        level++;
    }
    private void Start()
    {
        //if (_autoStart)
        //{
        //    startspawn0();
        //    StartSpawn();
        //}
    }
    private void Update()
    {
        if (Vector3.Distance(transform.position, FindObjectOfType<HeroClass>().transform.position)<=10f)
        {
            StartSpawn();
        }
    }
    public void NoSpawnStart() { _autoStart = false; }

    public void StartSpawn()
    {
        if (count < 1||count!=1)
        {
            var enem = Instantiate(_object, transform.position, transform.rotation, _parent);
            count++;
        }
    }
    public void startspawn0()
    {
        //var enem = Instantiate(_object, transform.position, transform.rotation, _parent);
        //enem.GetComponent<Enemy_Combat_Script>().SetSliders(PlayerHealthHUD);
        //enem.GetComponent<EnemySp>().GetSpawner = this;
    }
    public void StopSpawn()
    {
        StopAllCoroutines();
    }
    public bool setSpawnBool()
    {
        bool isFind = false;
        foreach (var obj123 in FindObjectsOfType<EnemySp>())
        {
            if (obj123.GetSpawner == this) isFind = true;
        }
        return isFind;
    }
    private IEnumerator Spawn22()
    {
        yield return new WaitForSeconds(Random.Range(_spawnDelayRange.x, _spawnDelayRange.y));

        while (true)
        {
            if (!setSpawnBool() && count < 1)
            {
                count++;
                var enem = Instantiate(_object, transform.position, transform.rotation, _parent);
                //enem.GetComponent<enemScr>().SetSliders(PlayerHealthHUD);
                //enem.GetComponent<enemScr>().GetSpawner = this;
            }
            yield return new WaitForSeconds(Random.Range(_spawnPeriodRange.x, _spawnPeriodRange.y));
        }
    }

    private IEnumerator Spawn()
    {
        yield return new WaitForSeconds(Random.Range(_spawnDelayRange.x, _spawnDelayRange.y));

        while (true)
        {

            var enem = Instantiate(_object, transform.position, transform.rotation, _parent);
            if (23 >= level)
            {
                enem.transform.Find("Hull").GetComponent<SpriteRenderer>().sprite = Objects[level];
                if (level >= 8) {  }
                else if (level >= 16) { }
                else if (level >= 21) { }
                else {  }
            }
            else { level = 0; }
            //enem.GetComponent<EnemySp>().GetSpawner = this;
            if (anothermovement == 1) { enem.GetComponent<EnemyShipController>().setAnotherMovement(1); }
            if (anothermovement == -1) { enem.GetComponent<EnemyShipController>().setAnotherMovement(-1); }
            else { }
            yield return new WaitForSeconds(Random.Range(_spawnPeriodRange.x, _spawnPeriodRange.y));
        }
    }
}
