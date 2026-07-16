using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class AudioTester : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip clip;
    public InputActionReference jumpAction;


    private void OnEnable()
    {
        jumpAction.action.Enable();
        jumpAction.action.performed += HandleJumpAction;
    }

    private void OnDisable()
    {
        jumpAction.action.Disable();
        jumpAction.action.performed -= HandleJumpAction;
    }
    private void HandleJumpAction(InputAction.CallbackContext context)
    {
        //AudioSource.PlayClipAtPoint(clip, transform.position); // mereu creaza cate un GameObject cu un AudioSource nou (ineficient)

        //audioSource.clip = clip;
        //audioSource.Play();             // va intrerupe sunetul de dinainte (daca nu s-a terminat)

        audioSource.PlayOneShot(clip);  // nu intrerupe sunetul de dinainte
    }
}
