using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coding4Fun.Toolkit.Audio;
using Coding4Fun.Toolkit.Audio.Helpers;
using System.IO;
using Microsoft.Xna.Framework.Audio;
using System.Threading;

namespace PianoPhone
{
    class PianoEngine
    {
         const int KeyCount = 12;
        List<MemoryStream> samples;
        List<SoundEffect> soundEffects;
        List<SoundEffectInstance> soundEffectInstances;
        
        public PianoEngine()
        {
            InitSoundEffects();
            
        }

        private void InitSamples()
        {
            samples = new List<MemoryStream>();
            for(int i = 0; i < KeyCount; i++)
            {
                string address = "Assets/Piano Keys/Piano" + i.ToString();
                using (FileStream fStream = new FileStream(address, FileMode.Open))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        byte[] buffer = new byte[fStream.Length];
                        fStream.ReadAsync(buffer,0, buffer.Length);
                        ms.WriteAsync(buffer,0,buffer.Length);
                        ms.Seek(0, SeekOrigin.Begin);
                        ms.Close();
                        samples.Add(ms);
                    }
                    fStream.Close();
                }
            }
        }

        private void InitSoundEffects()
        {
            soundEffects = new List<SoundEffect>();
            soundEffectInstances = new List<SoundEffectInstance>();
            for (int i = 1; i <= KeyCount; i++)
            {
                ReadFile(i);
                Thread.Sleep(100);
            }
        }

        private async void ReadFile(int i)
        {
            string address = "Assets/Piano Keys Mp3/Piano" + i.ToString() + ".wav";
            FileStream fStream = new FileStream(address, FileMode.Open);
            byte[] buffer = new byte[fStream.Length + 4 - fStream.Length % 4];
           await fStream.ReadAsync(buffer, 0, buffer.Length);
                
            soundEffects.Add(new SoundEffect(buffer, 44100, AudioChannels.Mono));
            fStream.Close();
            fStream.Dispose();
            soundEffectInstances.Add(soundEffects.Last().CreateInstance());
        }

        public void PlaySample(int i)
        {
            i--;
            if (soundEffectInstances[i].State == SoundState.Playing)
                soundEffectInstances[i].Stop();
            soundEffectInstances[i].Play();
        }
        
    }
}
