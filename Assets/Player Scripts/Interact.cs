using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interact : MonoBehaviour
{
    public string interactButton;
    public float interactDistance = 3f;
    public LayerMask interactLayer;
    public Image interactIcon;
    public bool isInteracting;
    RaycastHit hit;



    void Start()
    {
        if (interactIcon != null)
        {
            interactIcon.enabled = false;
}
        
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        
        if (Physics.Raycast(ray, out hit, interactDistance, interactLayer))
        {
            if (isInteracting == false)
            {
                
                if(interactIcon != false)
                {
                    interactIcon.enabled = true;
                }

                if(Input.GetButtonDown(interactButton))
                {
                    if (hit.collider.CompareTag("Door"))
                    {
                        hit.collider.GetComponent<Door>().ChangeDoorState();
                         
                    }
                    else if (hit.collider.CompareTag("Key"))
                    {
                        hit.collider.GetComponent<Key>().UnlockDoor();
                    }
                }
            }
        }
        else
        {
            interactIcon.enabled = false;
        }


    }
}
