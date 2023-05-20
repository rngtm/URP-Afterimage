using UnityEngine;

namespace Afterimage
{
    public static class RenderConfig
    {
        public static RenderTexture CameraTexture { get; set; }
    }

    public static class ShaderPropertyId
    {
        public static readonly int CameraTexture = Shader.PropertyToID("_CameraTexture");
        public static readonly int BlitTexture = Shader.PropertyToID("_BlitTexture");
    }
}