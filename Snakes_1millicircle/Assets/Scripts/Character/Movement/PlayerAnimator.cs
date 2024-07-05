using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerAnimator : MonoBehaviour
{
    NavMeshAgent agent;
    [SerializeField] private Animator anim;
    [SerializeField] private bool setViewNewInput = false;
    float motionSmoothTime = .075f;

    // Start is called before the first frame update
    void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (setViewNewInput)
        {
            float speed = GetComponent<TopDownCharacterMover>().MoveSpeed();
            anim.SetFloat("Speed", speed, motionSmoothTime, Time.deltaTime);
        }
        else
        {
            float speed = agent.velocity.magnitude / agent.speed;
            anim.SetFloat("Speed", speed, motionSmoothTime, Time.deltaTime);
        }
    }
}
