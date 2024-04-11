using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    public Door myDoor;
    private AudioSource audioSource;
    public AudioClip pickingUp; 
    private FSMZombie enemy1;
    private FSMZombie enemy2;






    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        enemy1 = GameObject.Find("Enemy1").GetComponent<FSMZombie>();
        enemy2 = GameObject.Find("Enemy2").GetComponent<FSMZombie>();
    }

    IEnumerator DestroyItself()
    {
        yield return new WaitForSeconds(pickingUp.length);
        Destroy(gameObject);
    }
    public void UnlockDoor()
        {
            myDoor.isLocked = false;
            enemy1.keyTaken = true;
            enemy2.keyTaken = true;
            audioSource.PlayOneShot(pickingUp);
            StartCoroutine("DestroyItself");

        }

}
