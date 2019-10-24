using Game;
using UnityEngine;

namespace Game
{
    public class SoundTrigger : MonoBehaviour
    {
        public void PlaySound(AudioClip clip)
        {
            Finder.SoundManager.PlaySingle(clip);
        }
    }
}