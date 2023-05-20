namespace Afterimage
{
    using UnityEngine;
    using UnityEngine.Experimental.Rendering;

    public static class RenderConfig
    {
        public static RenderTexture CameraTexture { get; set; }
    }

    public class CameraImage : MonoBehaviour
    {
        private RenderTexture _renderTexture; // カメラ内容を表示したいRenderTexture
        private MaterialPropertyBlock _materialPropertyBlock; // マテリアルのパラメータ変更用のMaterialPropertyBlock
        private MeshRenderer _meshRenderer; // RenderTextureを表示用のMeshRenderer

        private void Start()
        {
            Setup();
        }

        private void OnDestroy()
        {
            Release();
        }

        /// <summary>
        /// セットアップ処理
        /// </summary>
        private void Setup()
        {
            _renderTexture = new RenderTexture(Screen.width, Screen.height, 0, GraphicsFormat.R32G32B32A32_SFloat);

            // ScriptableRenderPassから見える場所にRenderTextureを登録する
            RenderConfig.CameraTexture = _renderTexture;
        }

        /// <summary>
        /// 解放処理
        /// </summary>
        private void Release()
        {
            if (_renderTexture != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(_renderTexture);
                }
                else
                {
                    DestroyImmediate(_renderTexture);
                }

                _renderTexture = null;
            }
        }
    }
}