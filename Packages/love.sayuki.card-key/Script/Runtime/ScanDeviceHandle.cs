using UdonSharp;
using UnityEngine;

namespace __Sayuki.CardKey.Script.Runtime
{
    public class ScanDeviceHandle : UdonSharpBehaviour
    {
        public Animator animator;
        public AudioSource audioSource;
        public AudioClip passSound;
        public AudioClip warnSound;
        
        public void ToWarning()
        {
            animator.SetTrigger("Warn");
            audioSource.PlayOneShot(warnSound);
        }
        
        public void ToPass()
        {
            animator.SetTrigger("Pass");
            audioSource.PlayOneShot(passSound);
        }
    }
}