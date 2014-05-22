using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;

namespace Supesu.SoundHandler
{
    class Sounds
    {
        protected static AudioEngine audioEngine;
        protected static WaveBank waveBank;
        protected static SoundBank soundBank;

        public static AudioEngine AudioEngine
        {
            get { return audioEngine; }
        }

        public static SoundBank SoundBank
        {
            get { return soundBank; }
        }

        public Sounds()
        {
            audioEngine = new AudioEngine("Content/GameSounds.xgs");
            waveBank = new WaveBank(audioEngine, "Content/WaveBank.xwb");
            soundBank = new SoundBank(audioEngine, "Content/SoundBank.xsb");
        }
    }
}
