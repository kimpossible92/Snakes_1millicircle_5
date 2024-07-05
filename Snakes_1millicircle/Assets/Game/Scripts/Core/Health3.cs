
using RPG.Combat;
using RPG.Saving;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Core
{
    public class Health3: MonoBehaviour, ISaveable
    {
        public float maxHealth;
        public float currentHealth;
        private bool isDead = false;
        private bool removed = false;
        
        
        //Note: the way that this is currently written, the value of health when you load a new scene gets overriden by this method
        //because of script execution order
        void Start()
        {
            currentHealth = maxHealth;
        }
        
        void Update() 
        {
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        public void TakeDamage(float damage)
        {
            currentHealth = Mathf.Max(currentHealth - damage, 0);
            print("Health3 of " + currentHealth);
            if (currentHealth == 0)
            {
               Die();
            }
            if (tag == "Player") { }
        }

        private void Die()
        {
            //todo: stop enemy from attacking, change enemy to empty model?
            
            GetComponent<Animator>().SetTrigger("dead");
            isDead = true;
            GetComponent<ActionScheduler>().CancelAction();
            RemoveProjectiles();
            
            
            //dESTROY//deactivate STUFF
            if (!removed)
            {
                
                GetComponent<NavMeshAgent>().enabled = false;
                removed = true;
            }


            Destroy(this.gameObject, 2f);
           // Start();
        }

        public bool IsDead()
        {
            return isDead;
        }


        public object CaptureState()
        {
            return currentHealth;
        }

       
        //Note: the way that this is currently written, the value of health when you load a new scene gets overriden by this method
        //because of script execution order. change start to awake to to fix issue
        public void RestoreState(object state)
        {
            currentHealth = (float) state;
            if(currentHealth <= 0)
                Die();
        }

        public void RemoveProjectiles()
        {
            Projectile [] projectiles = GetComponentsInChildren<Projectile>();
            foreach (Projectile projectile in projectiles)
            {
                Destroy(projectile.gameObject);
                Debug.Log("hello");
            }
        }
    }
}