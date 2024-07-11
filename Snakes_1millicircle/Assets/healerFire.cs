using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class healerFire : MonoBehaviour
{
    [SerializeField]public bool isMainPlayer = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isMainPlayer) {
         
        }
        else 
        {
            if (FindObjectOfType<HeroClass>() != null)
            {
                if (Vector3.Distance(transform.position, FindObjectOfType<HeroClass>().transform.position) <= 4.5f)
                {
                    FindObjectOfType<HeroClass>().setMaxHealth();
                }
            }
        }
    }
}
