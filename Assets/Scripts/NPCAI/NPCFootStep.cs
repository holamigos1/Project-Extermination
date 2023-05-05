using UnityEngine;

namespace NPCAI
{
    public class NPCFootStep : MonoBehaviour
    {
        AudioSource audioSource;
        public AudioClip[] footStepSounds;
        
        void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        
        void Update()
        {
            
        }

        public void FootStep()
        {
            int randomIndex = (int) Random.Range(0, footStepSounds.Length);
            audioSource.PlayOneShot(footStepSounds[randomIndex]);
        }
    }
}
