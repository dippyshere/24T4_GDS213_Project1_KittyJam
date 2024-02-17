using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using MyUILibrary;
using System.Collections;

// Adapted from https://gist.github.com/AdPetrou/31ee05e6df40791c638004a42901af91
namespace UnityEngine.UIElements
{
    public class CircleUI : MonoBehaviour
    {
        [Tooltip("Width of the panel in pixels. The RenderTexture used to render the panel will have this width.")]
        [SerializeField] protected int _panelWidth = 1280;
        [Tooltip("Height of the panel in pixels. The RenderTexture used to render the panel will have this height.")]
        [SerializeField] protected int _panelHeight = 720;
        [Tooltip("Scale of the panel. It is like the zoom in a browser.")]
        [SerializeField] protected float _panelScale = 1.0f;
        [Tooltip("Pixels per world units, it will the termine the real panel size in the world based on panel pixel width and height.")]
        [SerializeField] protected float _pixelsPerUnit = 1280.0f;
        [Tooltip("Visual tree element object of this panel.")]
        [SerializeField] protected VisualTreeAsset _visualTreeAsset;
        [Tooltip("PanelSettings that will be used to create a new instance for this panel.")]
        [SerializeField] protected PanelSettings _panelSettingsPrefab;
        [Tooltip("RenderTexture that will be used to create a new instance for this panel.")]
        [SerializeField] protected RenderTexture _renderTexturePrefab;
        [SerializeField] Material material;
        private RadialProgress m_RadialProgress;
        private double timeInstantiated;

        public VisualTreeAsset VisualTreeAsset
        {
            get => _visualTreeAsset;
            set
            {
                _visualTreeAsset = value;

                if (_uiDocument != null)
                    _uiDocument.visualTreeAsset = value;
            }
        }

        public VisualElement UIWidget
        { get { return _uiDocument.rootVisualElement[0]; } }

        public float PixelsPerUnit { get => _pixelsPerUnit; set { _pixelsPerUnit = value; RefreshPanelSize(); } }

        protected MeshRenderer _meshRenderer;
        protected SpriteRenderer _spriteRenderer;

        // runtime rebuildable stuff
        protected UIDocument _uiDocument;
        protected PanelSettings _panelSettings;
        protected RenderTexture _renderTexture;
        protected Material _material;

        void Awake()
        {
            PixelsPerUnit = _pixelsPerUnit;

            // dynamically a MeshFilter, MeshRenderer and BoxCollider
            MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();

            _meshRenderer = gameObject.AddComponent<MeshRenderer>();
            _meshRenderer.sharedMaterial = null;
            _meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
            _meshRenderer.receiveShadows = false;
            _meshRenderer.allowOcclusionWhenDynamic = false;
            _meshRenderer.lightProbeUsage = LightProbeUsage.Off;
            _meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
            _meshRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;

            // set the primitive quad mesh to the mesh filter
            GameObject quadGo = GameObject.CreatePrimitive(PrimitiveType.Quad);
            meshFilter.sharedMesh = quadGo.GetComponent<MeshFilter>().sharedMesh;
            Destroy(quadGo);

            _spriteRenderer = new GameObject("Sprite", typeof(SpriteRenderer)).GetComponent<SpriteRenderer>();
            _spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            _spriteRenderer.sortingOrder = 5;
            _spriteRenderer.transform.SetParent(transform);
            _spriteRenderer.material = new Material(Shader.Find("Universal Render Pipeline/2D/Sprite-Lit-Default"));
            _spriteRenderer.transform.localPosition = new Vector3(0, 0, 0);
            _spriteRenderer.color = new Color(0.0745098039f, 0.0745098039f, 0.0745098039f, 0.235294118f);
        }

        private bool _isSet = false;
        void Start()
        {
            /* 
             For some reason, this is getting called multiple times, _isSet stops that from happening, idk why
             If this isn't here then it attempts to add the UiDocument to the gameobject multiple times
             which results in no return value from adding the component which throws an object reference error 
             because it can't add the panel to the UiDocument
            */

            if (!_isSet)
                RebuildPanel();
            m_RadialProgress = UIWidget as RadialProgress;
            timeInstantiated = SongManager.GetAudioSourceTime();
            StartCoroutine(UpdateSilhouette());
        }

        private void Update()
        {

            double timeSinceInstantiated = SongManager.GetAudioSourceTime() - timeInstantiated;
            float t = (float)(timeSinceInstantiated / (SongManager.Instance.noteTime * 2));
            m_RadialProgress.progress = Mathf.Clamp(t * 200, 0, 100);
            if (t > 0.5)
            {
                _material.color = new Color(1.0f, 1.0f, 1.0f, Mathf.Lerp(1, 0, (t - 0.5f) / 0.0585f));
                _spriteRenderer.color = new Color(0.0745098039f, 0.0745098039f, 0.0745098039f, Mathf.Lerp(0.235294118f, 0, (t - 0.5f) / 0.0585f));
            }
            else
            {
                _material.color = new Color(1.0f, 1.0f, 1.0f, Mathf.Clamp01(t * 2));
                _spriteRenderer.color = new Color(0.0745098039f, 0.0745098039f, 0.0745098039f, Mathf.Clamp01(t * 2) * 0.235294118f);
            }
        }

        private IEnumerator UpdateSilhouette()
        {
            while (true)
            {
                Texture2D texture2D = new Texture2D(_renderTexture.width, _renderTexture.height);
                RenderTexture.active = _renderTexture;
                texture2D.ReadPixels(new Rect(0, 0, _renderTexture.width, _renderTexture.height), 0, 0);
                texture2D.Apply();
                RenderTexture.active = null;
                _spriteRenderer.sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), _pixelsPerUnit);
                yield return new WaitForSeconds(0.05f);
            }
        }

        /// <summary>
        /// Use this method to initialise the panel without triggering a rebuild (i.e.: when instantiating it from scripts). Start method
        /// will always trigger RebuildPanel(), but if you are calling this after the GameObject started you must call RebuildPanel() so the
        /// changes take effect.
        /// </summary>
        public void InitPanel(int panelWidth, int panelHeight, float panelScale, float pixelsPerUnit, VisualTreeAsset visualTreeAsset, PanelSettings panelSettingsPrefab, RenderTexture renderTexturePrefab)
        {
            _panelWidth = panelWidth;
            _panelHeight = panelHeight;
            _panelScale = panelScale;
            _pixelsPerUnit = pixelsPerUnit;
            _visualTreeAsset = visualTreeAsset;
            _panelSettingsPrefab = panelSettingsPrefab;
            _renderTexturePrefab = renderTexturePrefab;
        }

        /// <summary>
        /// Rebuilds the panel by destroy current assets and generating new ones based on the configuration.
        /// </summary>
        public void RebuildPanel()
        {
            _isSet = true;
            DestroyGeneratedAssets();

            // generate render texture
            RenderTextureDescriptor textureDescriptor = _renderTexturePrefab.descriptor;
            textureDescriptor.width = _panelWidth;
            textureDescriptor.height = _panelHeight;
            _renderTexture = new RenderTexture(textureDescriptor);

            // generate panel settings
            _panelSettings = Instantiate(_panelSettingsPrefab);
            _panelSettings.targetTexture = _renderTexture;
            _panelSettings.clearColor = true; // ConstantPixelSize and clearColor are mandatory configs
            _panelSettings.scaleMode = PanelScaleMode.ConstantPixelSize;
            _panelSettings.scale = _panelScale;
            _renderTexture.name = $"{name} - RenderTexture";
            _panelSettings.name = $"{name} - PanelSettings";

            // generate UIDocument
            _uiDocument = gameObject.AddComponent<UIDocument>();
            _uiDocument.panelSettings = _panelSettings;
            _uiDocument.visualTreeAsset = _visualTreeAsset;

            _material = new Material(material);

            _material.SetTexture("_BaseMap", _renderTexture);
            _meshRenderer.sharedMaterial = _material;
            RefreshPanelSize();
        }

        protected void RefreshPanelSize()
        {
            if (_renderTexture != null && (_renderTexture.width != _panelWidth || _renderTexture.height != _panelHeight))
            {
                _renderTexture.Release();
                _renderTexture.width = _panelWidth;
                _renderTexture.height = _panelHeight;
                _renderTexture.Create();

                if (_uiDocument != null)
                    _uiDocument.rootVisualElement?.MarkDirtyRepaint();
            }

            transform.localScale = new Vector3(_panelWidth / _pixelsPerUnit, _panelHeight / _pixelsPerUnit, 1.0f);
        }

        protected void DestroyGeneratedAssets()
        {
            if (_uiDocument) Destroy(_uiDocument);
            if (_renderTexture) Destroy(_renderTexture);
            if (_panelSettings) Destroy(_panelSettings);
            if (_material) Destroy(_material);
        }

        void OnDestroy()
        {
            StopAllCoroutines();
            DestroyGeneratedAssets();
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            if (Application.isPlaying && _material != null && _uiDocument != null)
            {
                if (_uiDocument.visualTreeAsset != _visualTreeAsset)
                    VisualTreeAsset = _visualTreeAsset;
                if (_panelScale != _panelSettings.scale)
                    _panelSettings.scale = _panelScale;

                RefreshPanelSize();
            }
        }
#endif
    }
}