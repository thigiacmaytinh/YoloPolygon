using NAudio.Wave;
using System;
using System.IO;
using System.Media;
using System.Threading;

namespace TGMTcs
{
    public class TGMTsound
    {
        static WaveOut waveOut;
        static SoundPlayer player;

        public static void PlaySoundAsync(string filepath)
        {
            Thread t = new Thread(() => PlaySound(filepath));
            t.Start();
        }

        private static bool PlaySound(string filepath)
        {
            if (!File.Exists(filepath))
                return false;

            if (waveOut != null && waveOut.PlaybackState == PlaybackState.Playing)
                return false;

            if(player != null)
                return false;

            try
            {
                FileInfo fi = new FileInfo(filepath);
                if(fi.Extension == ".wav")
                {
                    player = new SoundPlayer(filepath);
                    player.PlaySync();
                    player = null;
                }
                else
                {
                    if (waveOut != null)
                    {
                        waveOut.Stop();
                    }
                    var reader = new Mp3FileReader(filepath);
                    waveOut = new WaveOut();
                    waveOut.Init(reader);
                    waveOut.Play();
                }

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
    }
}

    

