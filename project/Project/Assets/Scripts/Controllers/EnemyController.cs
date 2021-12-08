using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    bool objetoNaMao;
    public float lookRadius = 10f;
    GameObject[] itensArremessaveis;
    public Transform target;
    NavMeshAgent agent;
    bool EstaArremessando;
    int randomNumber;
    Animator animator;
    float distance;

    public bool TomandoDano;

    public Transform objetoSpawn;

    public float forca;

    int damping = 2;

    public Healthbar enemyLife;

    public float Damage;
    // Start is called before the first frame update
    void Start()
    {
        objetoNaMao = false;
        EstaArremessando = false;
        itensArremessaveis = GameObject.FindGameObjectsWithTag("Arremessaveis");
        //target = PlayerManager.instance.player.transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (TomandoDano)
        {
            enemyLife.TakeDamage(Damage*Time.deltaTime);
        }
        if(enemyLife.health <= 0)
        {
            animator.SetBool("Estunado", true);
            agent.isStopped = true;
        }

        PegarItem();
        if(agent.velocity.x > 0.1f || agent.velocity.x < -0.1f || agent.velocity.z > 0.1f || agent.velocity.z < -0.1f)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }

        if(itensArremessaveis[randomNumber] != null) 
        {
            distance = Vector3.Distance(itensArremessaveis[randomNumber].transform.position, transform.position);

            if(distance <= 2.3f && !objetoNaMao && !TomandoDano)
            {
                objetoNaMao = true;
                agent.isStopped = true;
                

                animator.SetTrigger("isThrowing");
            }
        }

        if (objetoNaMao)
        {
            Vector3 dir = target.position - itensArremessaveis[randomNumber].transform.position;


            Quaternion rotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
        }



    }

    void PegarItem()
    {
        
        if (!EstaArremessando)
        {
            randomNumber = Random.Range(0, itensArremessaveis.Length);
        }
        if (itensArremessaveis[randomNumber] != null )
        {
            
            EstaArremessando = true;
            if(distance > 2.5f && !objetoNaMao)
            {
                agent.SetDestination(itensArremessaveis[randomNumber].transform.position);
            }
            
        }
    }

    public void ArremessarItem()
    {
        
        itensArremessaveis[randomNumber].transform.SetParent(null,true);
        itensArremessaveis[randomNumber].GetComponent<Rigidbody>().isKinematic = false;
        Vector3 dir = target.position - itensArremessaveis[randomNumber].transform.position;
        dir = dir.normalized;
        itensArremessaveis[randomNumber].GetComponent<Rigidbody>().AddForce(dir * forca);

        StartCoroutine(Esperar());

    }
    public void LargarItem()
    {
        itensArremessaveis[randomNumber].transform.SetParent(null, true);
        itensArremessaveis[randomNumber].GetComponent<Rigidbody>().isKinematic = false;
        objetoNaMao = false;
    }

    IEnumerator Esperar()
    {
        yield return new WaitForSeconds(4f);

        objetoNaMao = false;
        EstaArremessando = false;
        
        agent.isStopped = false;
        


    }

    public void SegurarItem()
    {
        itensArremessaveis[randomNumber].transform.position = objetoSpawn.transform.position;
        itensArremessaveis[randomNumber].transform.parent = objetoSpawn;
        
    }

    public void TomarDano()
    {
        TomandoDano = true;
        if (objetoNaMao)
        {
            LargarItem();
        }
        EstaArremessando = false;
        agent.isStopped = true;
        animator.SetBool("BeingHit", true);
        

    }

    public void PararDeTomarDano()
    {
        TomandoDano = false;
        animator.SetBool("BeingHit", false);
        agent.isStopped = false;
    }
}
