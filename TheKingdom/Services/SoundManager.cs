using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Audio;

namespace TheKingdom
{
    public class SoundManager
    {

        Music bgMusic = new Music("./Music/bgmusic.ogg");
        public int MusicVolume = 100;
        public int SoundVolume = 100;
        
        public void Play()
        {
            bgMusic.Volume = MusicVolume;            
            bgMusic.Play();
            bgMusic.Loop = false;            
        }

        public bool MusicPlaying()
        {
            if (bgMusic.Status == SoundStatus.Playing)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Stop()
        {
            bgMusic.Stop();
        }
    }
}
