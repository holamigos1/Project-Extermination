using UnityEngine;

namespace Artificial_Intelligence
{
    public class NPCAttackSoundManager : MonoBehaviour
    {
        AudioSource audioSource;
        public AudioClip [] clips;
    
        void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

   
        void Update()
        {
        
        }

        public void Attack()
        {
            if(!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(clips[Random.Range(0,clips.Length)]);
            }
        }
    }
}
