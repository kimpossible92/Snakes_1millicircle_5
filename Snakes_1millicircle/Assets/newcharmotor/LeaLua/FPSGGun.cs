using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FPSGGun : MonoBehaviour
{
    public Animator GetAnimation1;
    public int index = 1;//Weapon Slot
    public Vector3 offset = new Vector3(0, 0, 0);//
    public Vector3 rotation = new Vector3(0, 0, 0);
    public int clipsize = 6;
    public int ammo = 200;//int "Ammunition"

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void Holster()
    {
        if (GetAnimation1.GetInteger("unholster") > -1)
        { //then
          //--self.animationmanager:SetAnimationSequence("holster", self.reloadspeed,0,1)--,self,self.EndHolster,25)		
          //self.entity:PlayAnimation("holster", self.reloadspeed,0,1)

        }
    }

    void Unholster() {
        if (GetAnimation1.GetInteger("unholster") > -1) {
            //self.animationmanager:SetAnimationSequence("unholster", self.reloadspeed,0,1)
            //self.entity:PlayAnimation("unholster", self.reloadspeed,0,1)
            //self.currentaction="unholster"
        }
    }
    public void HideGun()
    {
        //this.gameObject.
    }

}
