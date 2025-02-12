using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using NVorbis;
using UnityEngine;

namespace MadCore.API.Utils
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static class AssetUtils
    {
        public static AudioClip LoadAudioClip(string clipName, string filePath)
        {
            var stream = GetEmbeddedAsset(filePath);
            using (var reader = new VorbisReader(stream))
            {
                var sampleCount = (int)reader.TotalSamples * reader.Channels;
                var audioData = new float[sampleCount];
                reader.ReadSamples(audioData, 0, sampleCount);
                return LoadClipInternal(clipName, audioData, reader.Channels, reader.SampleRate);
            }
        }

        private static AudioClip LoadClipInternal(string name, float[] data, int channels, int sampleRate)
        {
            var clip = AudioClip.Create(name, data.Length/channels, channels, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }
        
        public static Sprite LoadSprite(string filePath, float pixelsPerUnit = 100.0f) {
            var texture = LoadTexture(filePath);
            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),new Vector2(0.5f,0.5f), pixelsPerUnit);
            return sprite;
        }

        public static Texture2D LoadTexture(string filePath)
        {
            var stream = GetEmbeddedAsset(filePath);
            var fileData = ReadAllBytes(stream);
            var tex2D = new Texture2D(2, 2);       
            return tex2D.LoadImage(fileData) ? tex2D : null;
        }

        public static byte[] ReadAllBytes(Stream stream)
        {
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }
        
        public static Stream GetEmbeddedAsset(string filePath)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(filePath);
        }
        
        public static string[] GetEmbeddedAssetsNames()
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceNames();
        }
    }
}