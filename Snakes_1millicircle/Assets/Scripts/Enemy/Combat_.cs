using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Combat_ : MonoBehaviour
{
    // Enemy Init
    public enum EnemyAttackType { Melee, Ranged };
    public EnemyAttackType enemyAttackType;
    NavMeshAgent agent;

    // Combat targeting and range
    public GameObject targetedEnemy;
    [SerializeField] private float enemyAttackRange;
    [SerializeField] private float enemyRotateSpeedForAttack;

    [SerializeField] private float enemyAggroRange;

    // Combat range visuals
    [SerializeField] private UnityEngine.UI.Image attackRange_Indicator;
    [SerializeField] private Canvas attackRange_Canvas;
    private float offset;
    [SerializeField] private KeyCode checkAttackRange;
    private float radius;
    UnityEngine.UI.Image attackRange_Indicator_Image;

    // Character Animator
    private Move_PinkLeaf enemyMoveScript;
    private EnemyStatsScript enemyStatsScript;
    private Animator anim;
    // Combat Variables
    public bool basicAtkIdle = false;
    public bool isEnemyAlive;
    public bool performMeleeAttack = true;
    public bool enemyWithinAttackRange = false;

    // Not sure why this is needed
    [SerializeField] UnityEngine.UI.Slider PlayerHealthHUD;
    HealthSlider2D_Script healthCallRef;
    [SerializeField] private bool isMiss=true;
    public void SetSliders(UnityEngine.UI.Slider slider1)
    {
        PlayerHealthHUD = slider1;
        healthCallRef = PlayerHealthHUD.GetComponent<HealthSlider2D_Script>();
    }
    void Start()
    {
        isMiss = true;
        attackRange_Indicator_Image = attackRange_Indicator.GetComponent<UnityEngine.UI.Image>();
        agent = GetComponent<NavMeshAgent>();
        enemyStatsScript = gameObject.GetComponent<EnemyStatsScript>();
        anim = GetComponent<Animator>();
        enemyMoveScript = GetComponent<Move_PinkLeaf>();

        // This is defunct code to scale the image of the attack range properly
        //radius = enemyAttackRange / 2.0f / 2.0f / 2.0f / 1.25f;

        if (PlayerHealthHUD != null) healthCallRef = PlayerHealthHUD.GetComponent<HealthSlider2D_Script>();
    }
    //Used for debugging range indicator in game
    void CheckAttackRange()
    {
        if (Input.GetKeyDown(checkAttackRange) && attackRange_Indicator.GetComponent<UnityEngine.UI.Image>().enabled == false)
            attackRange_Indicator_Image.enabled = true;
        else if (Input.GetKeyDown(checkAttackRange) && attackRange_Indicator.GetComponent<UnityEngine.UI.Image>().enabled == true)
            attackRange_Indicator_Image.enabled = false;
    }

    void Update()
    {
        CheckAttackRange();
        attackRange_Indicator.transform.localScale = new Vector3(radius + offset, radius + offset, radius + offset);

        //Only check for targets when none
        if (targetedEnemy == null && isEnemyAlive)
        {
            CheckAggroRadius();
        }

        AliveFunc();

        CheckDead();
    }
    [SerializeField] private GameObject Bomb1;
    private void AliveFunc()
    {
        if (isEnemyAlive)
        {
            if (targetedEnemy != null)
            {
                speed = Mathf.Lerp(speed, agent.velocity.magnitude, Time.deltaTime * 10);
               // anim.SetFloat("Speed", speed);
                if (Vector3.Distance(gameObject.transform.position, targetedEnemy.transform.position) < enemyAggroRange)
                {
                    agent.SetDestination(targetedEnemy.transform.position);
                    agent.stoppingDistance = enemyAttackRange - 0.55f;

                    if (Vector3.Distance(gameObject.transform.position, targetedEnemy.transform.position) < enemyAttackRange && targetedEnemy != null && enemyWithinAttackRange == false)
                    {
                        //Debug.Log("Distance Enemy entered Attack Range");
                        agent.SetDestination(targetedEnemy.transform.position);
                        agent.stoppingDistance = enemyAttackRange;
                        enemyWithinAttackRange = true;
                    }
                    else if (enemyWithinAttackRange == true)
                    {
                        //Debug.Log("Enemy entered Attack Range");
                        //anim.SetTrigger("Attack");
                        
                        agent.isStopped = true;
                        Invoke("InvBomb", 2.5f);
                    }
                }
            }
            CheckEnemyLeaveRadius2();
        }
    }
    private void InvBomb()
    {
        if (Bomb1 != null) Bomb1.SetActive(true);
        CheckCombat();
        Invoke("InvDead", 0.3f);
    }
    private void InvDead()
    {
        enemyStatsScript.enemyHealth = 0;
    }
    IEnumerator CallDeadAnim()
    {
        //anim.SetBool("isDead", true);
        yield return new WaitForSeconds(0.01f);
        //anim.SetBool("isDead", true);
        OnDeadAnimEnd();
    }

    void CheckDead()
    {
        if (enemyStatsScript.enemyHealth <= 0 && isEnemyAlive == true)
        {
            StartCoroutine(CallDeadAnim());
            isEnemyAlive = false;
            if (enemyMoveScript != null)
                enemyMoveScript.isEnemyAliveRef = false;
        }
    }
    public void OnDeadAnimEnd()
    {
        Destroy(gameObject, 2.3f);
    }
    void CheckEnemyLeaveRadius2()
    {
        if (targetedEnemy != null)
        {
            if (Vector3.Distance(gameObject.transform.position, targetedEnemy.transform.position) > enemyAggroRange)
            {
                targetedEnemy = null;
                transforms.Clear();
                enemyWithinAttackRange = false;
                if (Bomb1 != null) Bomb1.SetActive(false);
            }
            else
            if (Vector3.Distance(gameObject.transform.position, targetedEnemy.transform.position) > enemyAttackRange)
            {
                enemyWithinAttackRange = false;
            }
        }
        else
        {
        }
    }
    void CheckEnemyLeaveRadius()
    {
        if (targetedEnemy != null)
        {
            if (Vector3.Distance(gameObject.transform.position, targetedEnemy.transform.position) > enemyAggroRange)
            {
                targetedEnemy = null;
                transforms.Clear();
                enemyWithinAttackRange = false;
            }
            else
            if (Vector3.Distance(gameObject.transform.position, targetedEnemy.transform.position) > enemyAttackRange)
            {
                enemyWithinAttackRange = false;
                ResetAutoAttack();
            }
        }
        else
        {
            ResetAutoAttack();
        }
    }
    private GameObject findClosestResourceWithTag(string tagtoCheck)
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag(tagtoCheck);
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
    }
    private void everyhealth()
    {
        if (Vector3.Distance(transform.position, findClosestResourceWithTag("Player").transform.position) <= 1.6f)
        {
            anim.SetBool("Slash", true);
        }
        else { anim.SetBool("Slash", false); anim.SetBool("walk3", true); }
    }

    public void ResetAutoAttack()
    {
        //anim.SetBool("Basic Attack", false);
        GetComponent<Animator>().SetTrigger("Attack");
        performMeleeAttack = true;
    }
    IEnumerator MeleeAttackInterval()
    {
        performMeleeAttack = false;

        yield return new WaitForSeconds(enemyStatsScript.enemyAttackTime / ((100 + enemyStatsScript.enemyAttackTime) * 0.01f));

        if (targetedEnemy == null)
        {
            performMeleeAttack = true;
        }
    }

    public void MeleeAttack()
    {
        if (targetedEnemy != null && targetedEnemy.GetComponent<HeroClass>()!=null)
        {
            float damageCalc = enemyStatsScript.enemyAttackDmg - (targetedEnemy.GetComponent<HeroClass>().heroDef * 0.1f);
            
            damageCalc = Mathf.Round(damageCalc);
            if (damageCalc <= 1f)
            {
                targetedEnemy.GetComponent<HeroClass>().heroHealth -= 1f;
                healthCallRef.CallHealthTrigger(targetedEnemy);
            }
            else
            {
                targetedEnemy.GetComponent<HeroClass>().heroHealth -= damageCalc;
                healthCallRef.CallHealthTrigger(targetedEnemy);
            }
        }
        performMeleeAttack = true;
    }
    public Transform[] targets;
    public List<Transform> transforms;
    private float speed;

    void CheckAggroRadius()
    {
        Collider[] hitCollider = Physics.OverlapSphere(gameObject.transform.position, enemyAggroRange);

        foreach (Collider col in hitCollider)
        {
            if (col.gameObject.GetComponent<TargetableScript>() != null && transforms.Contains(col.transform) == false && col.gameObject != this.gameObject && col.gameObject.tag != "Enemy")
                transforms.Add(col.transform);
        }

        transforms.AddRange(targets);

        Transform closestTarget = null;
        float closestTargetDistance = float.MaxValue;
        NavMeshPath path = new NavMeshPath();

        for (int i = 0; i < transforms.Count; i++)
        {
            if (transforms[i] == null)
                continue;

            if (NavMesh.CalculatePath(transform.position, transforms[i].position, agent.areaMask, path))
            {
                float distance = Vector3.Distance(transform.position, path.corners[0]);

                for (int j = 1; j < path.corners.Length; j++)
                {
                    distance += Vector3.Distance(path.corners[j - 1], path.corners[j]);
                }

                if (distance < closestTargetDistance)
                {
                    closestTargetDistance = distance;
                    closestTarget = transforms[i];
                }
            }
        }

        if (transforms.Count != 0)
        {
            targetedEnemy = closestTarget.gameObject;
            if (enemyMoveScript != null)
                enemyMoveScript.GetNewTargetedEnemyRef(targetedEnemy);
        }
    }
    void CheckCombat()
    {
        if (targetedEnemy != null)
        {
            if (enemyAttackType == EnemyAttackType.Melee)
            {
                FaceTarget();

                if (performMeleeAttack && enemyWithinAttackRange)
                {
                    //Debug.Log("Attack the player");
                    MeleeAttack();
                    // Start Courotine
                    //if (targetedEnemy.GetComponent<Enemy_Combat_Script>().isEnemyAlive)
                    StartCoroutine(MeleeAttackInterval());
                }
            }
        }
        else if (targetedEnemy == null)
        {
            performMeleeAttack = true;
        }
    }
    void MoveToEnemy()
    {
        if (targetedEnemy != null)
        {
            if (Vector3.Distance(gameObject.transform.position, targetedEnemy.transform.position) < enemyAggroRange)
            {
                agent.SetDestination(targetedEnemy.transform.position);
                agent.stoppingDistance = enemyAttackRange - 0.55f;

                if (Vector3.Distance(gameObject.transform.position, targetedEnemy.transform.position) < enemyAttackRange && targetedEnemy != null && enemyWithinAttackRange == false)
                {
                    //Debug.Log("Distance Enemy entered Attack Range");
                    agent.SetDestination(targetedEnemy.transform.position);
                    agent.stoppingDistance = enemyAttackRange;
                    enemyWithinAttackRange = true;
                }
                else if (enemyWithinAttackRange == true)
                {
                    //Debug.Log("Enemy entered Attack Range");
                    CheckCombat();
                    agent.isStopped = true;
                }
            }
        }
    }
    void FaceTarget()
    {
        // Rotation ??
        Quaternion rotationToLookAt = Quaternion.LookRotation(targetedEnemy.transform.position - transform.position);
        float rotationQ = Mathf.SmoothDampAngle(transform.eulerAngles.y,
            rotationToLookAt.eulerAngles.y,
            ref enemyMoveScript.rotateVelocity,
            enemyRotateSpeedForAttack * (Time.deltaTime * 5));

        transform.eulerAngles = new Vector3(0, rotationQ, 0);
    }
}
