using System;
using System.Collections.Generic;
using System.IO;
using OpenTK.Audio.OpenAL;

namespace LudumDare49.OpenAL
{
    public static class AudioMaster
    {
        private static readonly List<int> Sources = new();
        private static readonly List<int> Buffers = new();

        static AudioMaster()
        {
            string deviceName = ALC.GetString(ALDevice.Null, AlcGetString.DefaultDeviceSpecifier);
            var device = ALC.OpenDevice(deviceName);
            var context = ALC.CreateContext(device, (int[])null);
            ALC.MakeContextCurrent(context);
        }

        public static void PlayAmbient()
        {
            StartSound("ambient1.wav");
            StartSound("ambient2.wav");
        }

        public static unsafe void StartSound(string path)
        {
            int buffer = AL.GenBuffer();
            Buffers.Add(buffer);
            int source = AL.GenSource();
            Sources.Add(source);
            int state;

            int channels, bitsPerSample, sampleRate;
            byte[] soundData = LoadWave(Assets.LoadAsset(path), out channels, out bitsPerSample, out sampleRate);
            fixed (void* ptr = soundData)
            {
                AL.BufferData(buffer, GetSoundFormat(channels, bitsPerSample), ptr, soundData.Length, sampleRate);
            }

            AL.Source(source, ALSourcei.Buffer, buffer);
            AL.Source(source, ALSourceb.Looping, true);
            AL.SourcePlay(source);
        }

        public static void Stop()
        {
            foreach (int source in Sources)
            {
                AL.SourceStop(source);
                AL.DeleteSource(source);
            }
            foreach (int buffer in Buffers)
            {
                AL.DeleteBuffer(buffer);
            }
        }
        
        // Kindly provided by https://github.com/mono/opentk/blob/main/Source/Examples/OpenAL/1.1/Playback.cs
        public static byte[] LoadWave(Stream stream, out int channels, out int bits, out int rate)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (BinaryReader reader = new BinaryReader(stream))
            {
                // RIFF header
                string signature = new string(reader.ReadChars(4));
                if (signature != "RIFF")
                    throw new NotSupportedException("Specified stream is not a wave file.");

                int riffChunckSize = reader.ReadInt32();

                string format = new string(reader.ReadChars(4));
                if (format != "WAVE")
                    throw new NotSupportedException("Specified stream is not a wave file.");

                // WAVE header
                string formatSignature = new string(reader.ReadChars(4));
                if (formatSignature != "fmt ")
                    throw new NotSupportedException("Specified wave file is not supported.");

                int formatChunkSize = reader.ReadInt32();
                int audioFormat = reader.ReadInt16();
                int numChannels = reader.ReadInt16();
                int sampleRate = reader.ReadInt32();
                int byteRate = reader.ReadInt32();
                int blockAlign = reader.ReadInt16();
                int bitsPerSample = reader.ReadInt16();

                string dataSignature;
                while((dataSignature = new string(reader.ReadChars(4))) != "data") {
                    int chunkSize = reader.ReadInt32();
                    reader.BaseStream.Seek(chunkSize, SeekOrigin.Current);
                }

                int dataChunkSize = reader.ReadInt32();

                channels = numChannels;
                bits = bitsPerSample;
                rate = sampleRate;

                return reader.ReadBytes(dataChunkSize);
            }
        }

        public static ALFormat GetSoundFormat(int channels, int bits)
        {
            switch (channels)
            {
                case 1: return bits == 8 ? ALFormat.Mono8 : ALFormat.Mono16;
                case 2: return bits == 8 ? ALFormat.Stereo8 : ALFormat.Stereo16;
                default: throw new NotSupportedException("The specified sound format is not supported.");
            }
        }
    }
}