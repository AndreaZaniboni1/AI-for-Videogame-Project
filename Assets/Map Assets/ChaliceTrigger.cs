using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChaliceTrigger : MonoBehaviour
{
    private GameObject enemy1;
    private float distance1;
    private float distance2;
    private GameObject enemy2;
    public FSMZombie zombie;
    public bool broken;


    private void Update()
    {
        enemy1 = GameObject.Find("Enemy1");
        distance1 = Vector3.Distance(this.transform.position, enemy1.transform.position);
        enemy2 = GameObject.Find("Enemy2");
        distance2 = Vector3.Distance(this.transform.position, enemy2.transform.position);

        if (distance1 < distance2)
        {
            zombie = enemy1.GetComponent<FSMZombie>();
        }
        else
        {
            zombie = enemy2.GetComponent<FSMZombie>();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy" && broken==true)
        {


            zombie.isTaunted = false;

            Destroy(gameObject);

        }
    }

}
