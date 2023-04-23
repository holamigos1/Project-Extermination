using UnityEngine;
using Weapons.SoundsSheet.Base;

namespace Weapons.SoundsSheet
{
    [CreateAssetMenu(fileName = "Rifle Sound Sheet", menuName = "Scriptable Data/Sound Sheets/Rifle", order = 1)]
    public class RifleSoundSheet : SoundClipsSheet
    {
        public AudioClip ShootSound_1;
        public AudioClip ShootSound_2;
        public AudioClip ShootSound_3;
        public AudioClip ShootEchoSound;
    }
}