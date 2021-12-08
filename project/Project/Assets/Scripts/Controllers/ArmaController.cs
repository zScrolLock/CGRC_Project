using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmaController : MonoBehaviour
{
    public ParticleSystem armaRaios;
    public float damage;
    public Camera cam;
    Transform inimigo;
    RaycastHit hit;
    public Healthbar energyBar;

    bool usando;
    public float energyBarVelocity = 40;

    public AudioClip somAtirar;
    AudioSource audioSr;
    // Start is called before the first frame update
    void Start()
    {
        audioSr = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

        if (usando)
        {
            energyBar.TakeDamage(energyBarVelocity * Time.deltaTime);

            if (!audioSr.isPlaying)
            {
                audioSr.PlayOneShot(somAtirar);
            }
            
        }
        if (Input.GetButton("Fire1") && energyBar.health > 0)
        {
            
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit))
            {
                if (hit.transform.CompareTag("Inimigo"))
                {
                    usando = true;
                    hit.transform.GetComponent<EnemyController>().TomarDano();
                    inimigo = hit.transform;
                    if (armaRaios.isStopped)
                    {
                        armaRaios.Play();
                    }
                }
            }

            
        }
        if(hit.transform == null)
        {
            audioSr.Stop();
            usando = false;
            armaRaios.Stop();
            if(inimigo != null)
            {
                inimigo.GetComponent<EnemyController>().PararDeTomarDano();
            }
            
        }
        else if (Input.GetButtonUp("Fire1") || hit.transform != inimigo)
        {
            audioSr.Stop();
            usando = false;
            armaRaios.Stop();
            inimigo.GetComponent<EnemyController>().PararDeTomarDano();
        }
        else if(energyBar.health <= 0)
        {
            audioSr.Stop();
            usando = false;
            armaRaios.Stop();
            inimigo.GetComponent<EnemyController>().PararDeTomarDano();
        }

        
    }
}
