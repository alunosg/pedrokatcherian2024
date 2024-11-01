using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
   public enum EnemyStates { Idle, Chase, Attack, Hit, Death }
   public EnemyStates state = EnemyStates.Idle;
   public NavMeshAgent nav;
   public Animator anim;
   public float chaseDistance = 50;
   public float attackDistance = 2;

   public int hp = 3;

   public float stunDuration = 1;
   public float DeathDuration = 5;
   public float attackDuration = 5;

   private bool locked = false;
   private bool dead = false;

   private PlayerController player;
   private float distance = 1000;

   public GameObject hitParticle; 
   void Start()
   {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
   }

    public void Update()
    {
        if (locked) return;
        distance = Vector3.Distance(transform.position, player.transform.position);
        switch (state)
        {
            case EnemyStates.Idle:
                IdleUpdate();
                break;
            case EnemyStates.Chase:
                ChaseUpdate();
                break;
            case EnemyStates.Attack:
                AttackUpdate();
                break;
            case EnemyStates.Hit:
                HitUpdate();
                break;
            case EnemyStates.Death:
                DeathUpdate();
                break;
        }
    }
   

   void IdleUpdate()
   {
        if (distance < chaseDistance)
        {
            if (distance < attackDistance)
            {
                EnterAttack();
            }
            else
            {
                state = EnemyStates.Chase;
                anim.SetBool("IsRunning", true);
            }
        }
        else
        {
            nav.isStopped = true;
        }
   }


    void ChaseUpdate()
    {
        if (distance < attackDistance)
        {
            EnterAttack();
        }
        else if (distance >= chaseDistance)
        {
            anim.SetBool("IsRunning", false);
            state = EnemyStates.Idle;
        }
        else
        {
            nav.isStopped = false;
            nav.SetDestination(player.transform.position);
        }
    }   

    void AttackUpdate()
    {
        if (distance >= attackDistance)
        {
            if (distance >= chaseDistance)
            {
                state = EnemyStates.Idle;
                anim.SetBool("IsRunning", false);
            }
            else
            {
                state = EnemyStates.Chase;
                anim.SetBool("IsRunning", true);
            }
        }
        else
        {
            EnterAttack();
        }
    }
    
    void GetHit(int damage)
    {
        if(!dead)
        {
            locked = true;
            hp -= damage;
            Instantiate(hitParticle, transform);
            nav.isStopped = true;
            CancelInvoke("DealDamage");
            CancelInvoke("Unlock");

            if (hp <= 0)
            {
                dead = true;
                anim.SetTrigger("Death");
                state = EnemyStates.Death;
                Invoke("AutoDestroy", DeathDuration);
            }
            else
            {
                anim.SetTrigger("Hit");
                state = EnemyStates.Hit;
                Invoke("Unlock", stunDuration);
            }
        }
    }
    
    void Unlock()
    {
        locked = false;
    }

    void AutoDestroy()
    {
        Destroy(gameObject);
    }

    void EnterAttack()
    {
        anim.SetTrigger("Attack");
        state = EnemyStates.Attack;
        locked = true;
        nav.isStopped = true;
        CancelInvoke("DealDamage");
        CancelInvoke("Unlock");
        Invoke("DealDamage", 0.5f);
        Invoke("Unlock", attackDuration);
    }

    void HitUpdate()
    {

    }

    void DeathUpdate()
    {

    }
    
    void DealDamage()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackDistance + 0.5f);
        foreach (var target in hitColliders)
        {
            if(target.CompareTag("Player"))
            {
                player.GetHit(1, Vector3.MoveTowards(target.transform.position, transform.position, 0.5f));
                break;
            }
        }
    }
}