using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Chalice : MonoBehaviour
{

    private AudioSource audioSource;
    public AudioClip broke;
    public GameObject[] zombies;
    float distance;
    float radius;
    public FSMZombie zombie;
    public NavMeshAgent agent;
    public ChaliceTrigger myTrigger;




    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        zombies = GameObject.FindGameObjectsWithTag("Enemy");

    }

    // Update is called once per frame
    void Update()
    {
        float distance1 = Vector3.Distance(this.transform.position, zombies[0].transform.position);
        float distance2 = Vector3.Distance(this.transform.position, zombies[1].transform.position);
        if (distance1 <= distance2)
        {
            zombie = zombies[0].GetComponent<FSMZombie>();
        }
        else
        {
            zombie = zombies[1].GetComponent<FSMZombie>();
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {

            agent = zombie.GetComponent<NavMeshAgent>();
            audioSource.PlayOneShot(broke);
            myTrigger.broken = true;
            TauntEnemy();

        }
    }
    IEnumerator DestroyItself()
    {

        yield return new WaitForSeconds(broke.length);
        Destroy(gameObject);

    }
    public void TauntEnemy()
    {

        audioSource.PlayOneShot(broke);
        zombie.isTaunted = true;
        zombie.chalicePosition = transform.position;

        StartCoroutine("DestroyItself");

    }

}
