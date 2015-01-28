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
        public void Play()
        {            
            bgMusic.Volume = 100;            
            bgMusic.Play();
            bgMusic.Loop = false;            
        }

        public void Stop()
        {
            bgMusic.Stop();
        }
    }
}
