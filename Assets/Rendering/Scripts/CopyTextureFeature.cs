using UnityEngine.Rendering;

namespace Afterimage
{
    using UnityEngine;
    using UnityEngine.Rendering.Universal;

    /// <summary>
    /// テクスチャコピーを行うRendererFeature
    /// </summary>
    public class CopyTextureFeature : ScriptableRendererFeature
    {
        [SerializeField] private Settings settings = new Settings();
        private CopyTexturePass _copyTexturePass;
        private RTHandle _cameraTextureHandle;

        public enum CopyMode
        {
            CameraToTexture,
            TextureToCamera,
        }

        [System.Serializable]
        private class Settings
        {
            public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRendering;
            public CopyMode copyMode = CopyMode.CameraToTexture;
        }

        /// <inheritdoc/>
        public override void Create()
        {
            _copyTexturePass = new CopyTexturePass
            {
                renderPassEvent = settings.renderPassEvent
            };

            if (_cameraTextureHandle == null)
            {
                _cameraTextureHandle = RTHandles.Alloc(ShaderPropertyId.CameraTexture);
            }
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            
            if (_cameraTextureHandle != null)
            {
                _cameraTextureHandle.Release();
                _cameraTextureHandle = null;
            }
        }
        
        /// <inheritdoc/>
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            _copyTexturePass.Setup(settings.copyMode, _cameraTextureHandle);
            renderer.EnqueuePass(_copyTexturePass);
        }

    }
}