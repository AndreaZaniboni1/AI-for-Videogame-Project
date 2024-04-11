using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour


{
    public bool open = false;
    public bool isLocked = true;
    public float doorOpenAngle = 90f;
    public float doorClosedAngle = 0f;
    public float smooth = 2f;

    private AudioSource audioSource;
    public AudioClip openingSound;
    public AudioClip lockedDoorSound;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void ChangeDoorState()
    {   
        if (isLocked == false)
        {
            open = !open;
            if(audioSource != null)
            {
                audioSource.PlayOneShot(openingSound);
            }
        }
        else
        {
            audioSource.PlayOneShot(lockedDoorSound);
        }

    }

    void Update()
    {
      if(open)
        {
            Quaternion targetRotationOpen = Quaternion.Euler(0, doorOpenAngle, 0);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotationOpen, smooth * Time.deltaTime);
        }
        else
        {
            Quaternion targetRotationClose = Quaternion.Euler(0, doorClosedAngle, 0);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotationClose, smooth * Time.deltaTime);
        }
    }
}
