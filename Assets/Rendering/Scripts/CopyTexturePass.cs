namespace Afterimage
{
    using System;
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.Universal;

    /// <summary>
    /// テクスチャコピーを行うRenderPass
    /// </summary>
    public class CopyTexturePass : ScriptableRenderPass
    {
        private CopyTextureFeature.CopyMode _copyMode;
        private RTHandle _cameraTextureHandle;

        public void Setup(CopyTextureFeature.CopyMode copyMode, RTHandle cameraTextureHandle)
        {
            _copyMode = copyMode;
            _cameraTextureHandle = cameraTextureHandle;
        }

        public override void Configure(
            CommandBuffer cmd,
            RenderTextureDescriptor cameraTextureDescriptor)
        {
            base.Configure(cmd, cameraTextureDescriptor);
            ConfigureClear(ClearFlag.None, Color.clear);
            ConfigureInput(ScriptableRenderPassInput.Color);
            cmd.GetTemporaryRT(ShaderPropertyId.CameraTexture, cameraTextureDescriptor);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            base.OnCameraCleanup(cmd);
            cmd.ReleaseTemporaryRT(ShaderPropertyId.CameraTexture);
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            // 再生中のみ実行
            if (!Application.isPlaying)
            {
                return;
            }

            if (RenderConfig.CameraTexture == null)
            {
                return;
            }

            // Gameカメラのみ実行
            var camera = renderingData.cameraData.camera;
            bool isGameCamera = (camera.cameraType == CameraType.Game || camera.cameraType == CameraType.VR);
            if (!isGameCamera)
            {
                return;
            }

            // テクスチャをコピー
            var cmd = CommandBufferPool.Get("Copy Texture");
            var cameraColorTargetHandle = renderingData.cameraData.renderer.cameraColorTargetHandle;
            switch (_copyMode)
            {
                case CopyTextureFeature.CopyMode.CameraToTexture:
                    cmd.SetRenderTarget(ShaderPropertyId.BlitTexture);
                    cmd.SetGlobalTexture(ShaderPropertyId.BlitTexture, RenderConfig.CameraTexture);
                    Blitter.BlitTexture(
                        cmd,
                        cameraColorTargetHandle,
                        _cameraTextureHandle,
                        RenderBufferLoadAction.Load,
                        RenderBufferStoreAction.Store,
                        Blitter.GetBlitMaterial(TextureDimension.Tex2D),
                        0);
                    // cmd.Blit(cameraColorTargetHandle, RenderConfig.CameraTexture);
                    break;
                case CopyTextureFeature.CopyMode.TextureToCamera:
                    cmd.SetGlobalTexture(ShaderPropertyId.BlitTexture, RenderConfig.CameraTexture);
                    Blitter.BlitTexture(
                        cmd,
                        _cameraTextureHandle.nameID,
                        cameraColorTargetHandle.nameID,
                        RenderBufferLoadAction.Load,
                        RenderBufferStoreAction.Store,
                        Blitter.GetBlitMaterial(TextureDimension.Tex2D),
                        0);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}