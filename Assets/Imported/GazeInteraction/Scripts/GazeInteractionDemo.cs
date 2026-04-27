using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TS.GazeInteraction;
using DG.Tweening;

public class GazeInteractionDemo : MonoBehaviour
{
    [Header("Wiggle Settings")]
    [Tooltip("Intensity of the wiggle animation")]
    [SerializeField] private float _wiggleIntensity = 0.1f;
    
    [Tooltip("Speed of the wiggle animation")]
    [SerializeField] private float _wiggleSpeed = 10f;

    [Header("Scale Settings")]
    [Tooltip("Scale multiplier when activated (e.g., 1.2 = 20% bigger)")]
    [SerializeField] private float _scaleMultiplier = 1.2f;
    
    [Tooltip("Duration of the scale animation")]
    [SerializeField] private float _scaleAnimationDuration = 0.3f;
    
    [Tooltip("Ease type for scaling up")]
    [SerializeField] private Ease _scaleUpEase = Ease.OutBack;
    
    [Tooltip("Ease type for scaling down")]
    [SerializeField] private Ease _scaleDownEase = Ease.InBack;

    [Header("UI Settings")]
    [Tooltip("UI Canvas to display when object is highlighted")]
    [SerializeField] private Canvas _highlightCanvas;
    
    [Tooltip("Offset position for UI next to the object (relative to object)")]
    [SerializeField] private Vector3 _uiOffset = new Vector3(1.5f, 0, 0);
    
    [Header("Slide Animation Settings")]
    [Tooltip("Destination position for the UI slide animation (relative to object, or leave zero to use offset)")]
    [SerializeField] private Vector3 _slideDestination = Vector3.zero;
    
    [Tooltip("Start position for the UI slide animation (relative to object, or leave zero to auto-calculate)")]
    [SerializeField] private Vector3 _slideStartPosition = Vector3.zero;
    
    [Tooltip("Duration of the slide-in animation")]
    [SerializeField] private float _slideAnimationDuration = 0.3f;
    
    [Tooltip("Ease type for sliding in")]
    [SerializeField] private Ease _slideInEase = Ease.OutBack;
    
    [Tooltip("Ease type for sliding out")]
    [SerializeField] private Ease _slideOutEase = Ease.InBack;

    private GazeInteractable _gazeInteractable;
    private Vector3 _originalPosition;
    private Quaternion _originalRotation;
    private Vector3 _originalScale;
    private Coroutine _wiggleCoroutine;
    private GameObject _uiInstance;
    private Vector3 _uiStartPosition;
    private Vector3 _uiTargetPosition;
    private Tween _uiSlideTween;
    private Tween _scaleTween;

    void Start()
    {
        // Get or add GazeInteractable component
        _gazeInteractable = GetComponent<GazeInteractable>();
        if (_gazeInteractable == null)
        {
            _gazeInteractable = gameObject.AddComponent<GazeInteractable>();
        }

        // Store original transform
        _originalPosition = transform.localPosition;
        _originalRotation = transform.localRotation;
        _originalScale = transform.localScale;

        // Subscribe to gaze events
        _gazeInteractable.OnGazeEnter.AddListener(OnGazeEnter);
        _gazeInteractable.OnGazeExit.AddListener(OnGazeExit);
        _gazeInteractable.OnGazeActivated.AddListener(OnGazeActivated);

        // Create UI if not assigned
        if (_highlightCanvas == null)
        {
            CreateDefaultUI();
        }
    }

    /// <summary>
    /// Called when gaze enters the object
    /// </summary>
    private void OnGazeEnter()
    {
        // UI will appear on activation, not on enter
    }

    /// <summary>
    /// Called when gaze activates the object - shows UI with animation and scales up
    /// </summary>
    private void OnGazeActivated()
    {
        ShowUI();
        ScaleUp();
    }

    /// <summary>
    /// Called when gaze exits the object - stops wiggle, hides UI, and scales down
    /// </summary>
    private void OnGazeExit()
    {
        StopWiggle();
        HideUI();
        ScaleDown();
    }

    /// <summary>
    /// Starts the wiggle animation
    /// </summary>
    public void StartWiggle()
    {
        if (_wiggleCoroutine != null)
        {
            StopCoroutine(_wiggleCoroutine);
        }
        _wiggleCoroutine = StartCoroutine(WiggleCoroutine());
    }

    /// <summary>
    /// Stops the wiggle animation and smoothly resets transform
    /// </summary>
    public void StopWiggle()
    {
        if (_wiggleCoroutine != null)
        {
            StopCoroutine(_wiggleCoroutine);
            _wiggleCoroutine = null;
        }
        // Smoothly return to original position
        StartCoroutine(ReturnToOriginalPosition());
    }

    /// <summary>
    /// Smoothly returns the object to its original position and rotation
    /// </summary>
    private IEnumerator ReturnToOriginalPosition()
    {
        float returnTime = 0f;
        float returnDuration = 0.2f;
        Vector3 currentPos = transform.localPosition;
        Quaternion currentRot = transform.localRotation;

        while (returnTime < returnDuration)
        {
            returnTime += Time.deltaTime;
            float t = returnTime / returnDuration;
            t = Mathf.SmoothStep(0, 1, t);

            transform.localPosition = Vector3.Lerp(currentPos, _originalPosition, t);
            transform.localRotation = Quaternion.Lerp(currentRot, _originalRotation, t);

            yield return null;
        }

        transform.localPosition = _originalPosition;
        transform.localRotation = _originalRotation;
    }

    /// <summary>
    /// Coroutine that performs the wiggle animation - loops continuously until stopped
    /// </summary>
    private IEnumerator WiggleCoroutine()
    {
        Vector3 startPos = _originalPosition;
        Quaternion startRot = _originalRotation;

        // Loop continuously until StopWiggle() is called
        while (true)
        {
            // Create wiggle effect using sine waves
            float x = Mathf.Sin(Time.time * _wiggleSpeed) * _wiggleIntensity;
            float y = Mathf.Cos(Time.time * _wiggleSpeed * 1.3f) * _wiggleIntensity;
            float z = Mathf.Sin(Time.time * _wiggleSpeed * 0.7f) * _wiggleIntensity;

            // Apply position wiggle
            transform.localPosition = startPos + new Vector3(x, y, z);

            // Apply rotation wiggle
            float rotX = Mathf.Sin(Time.time * _wiggleSpeed * 0.5f) * _wiggleIntensity * 10f;
            float rotY = Mathf.Cos(Time.time * _wiggleSpeed * 0.8f) * _wiggleIntensity * 10f;
            transform.localRotation = startRot * Quaternion.Euler(rotX, rotY, 0);

            yield return null;
        }
    }

    /// <summary>
    /// Shows UI next to the object with sliding animation
    /// </summary>
    public void ShowUI()
    {
        if (_highlightCanvas == null) return;

        // Kill any existing animation
        if (_uiSlideTween != null && _uiSlideTween.IsActive())
        {
            _uiSlideTween.Kill();
        }

        // Instantiate UI if it doesn't exist
        if (_uiInstance == null)
        {
            _uiInstance = Instantiate(_highlightCanvas.gameObject);
        }

        // Parent UI to object transform
        if (_uiInstance.transform.parent != transform)
        {
            _uiInstance.transform.SetParent(transform);
        }

        // Calculate destination position (relative/local space)
        if (_slideDestination != Vector3.zero)
        {
            // Use relative position directly (already in local space)
            _uiTargetPosition = _slideDestination;
        }
        else
        {
            // Use offset relative to object (already in local space)
            _uiTargetPosition = _uiOffset;
        }

        // Calculate start position (relative/local space)
        if (_slideStartPosition != Vector3.zero)
        {
            // Use relative position directly (already in local space)
            _uiStartPosition = _slideStartPosition;
        }
        else
        {
            // Auto-calculate start position (offset further in the same direction for slide-in effect)
            Vector3 slideDirection = _uiTargetPosition.normalized;
            float slideDistance = _uiTargetPosition.magnitude * 0.5f; // Slide from 50% further away
            _uiStartPosition = _uiTargetPosition + slideDirection * slideDistance;
        }

        // Set initial position to start position (local space)
        _uiInstance.transform.localPosition = _uiStartPosition;
        _uiInstance.transform.localRotation = Quaternion.identity;
        
        _uiInstance.SetActive(true);

        // Animate sliding in (using local position)
        _uiSlideTween = _uiInstance.transform.DOLocalMove(_uiTargetPosition, _slideAnimationDuration)
            .SetEase(_slideInEase);
    }

    /// <summary>
    /// Hides the UI with slide-out animation
    /// </summary>
    public void HideUI()
    {
        if (_uiInstance == null) return;

        // Kill any existing animation
        if (_uiSlideTween != null && _uiSlideTween.IsActive())
        {
            _uiSlideTween.Kill();
        }

        // Recalculate start position to ensure it's current
        Vector3 slideOutPosition;
        if (_slideStartPosition != Vector3.zero)
        {
            // Use explicitly set start position
            slideOutPosition = _slideStartPosition;
        }
        else
        {
            // Auto-calculate start position
            Vector3 slideDirection = _uiTargetPosition.normalized;
            float slideDistance = _uiTargetPosition.magnitude * 0.5f;
            slideOutPosition = _uiTargetPosition + slideDirection * slideDistance;
        }

        // Animate sliding out to start position, then deactivate (using local position)
        _uiSlideTween = _uiInstance.transform.DOLocalMove(slideOutPosition, _slideAnimationDuration)
            .SetEase(_slideOutEase)
            .OnComplete(() =>
            {
                if (_uiInstance != null)
                {
                    // Ensure UI is at start position before deactivating
                    _uiInstance.transform.localPosition = slideOutPosition;
                    _uiInstance.SetActive(false);
                }
            });
    }

    /// <summary>
    /// Scales the object up when activated
    /// </summary>
    public void ScaleUp()
    {
        // Kill any existing scale animation
        if (_scaleTween != null && _scaleTween.IsActive())
        {
            _scaleTween.Kill();
        }

        Vector3 targetScale = _originalScale * _scaleMultiplier;
        
        // Animate scale up
        _scaleTween = transform.DOScale(targetScale, _scaleAnimationDuration)
            .SetEase(_scaleUpEase);
    }

    /// <summary>
    /// Scales the object back to original size
    /// </summary>
    public void ScaleDown()
    {
        // Kill any existing scale animation
        if (_scaleTween != null && _scaleTween.IsActive())
        {
            _scaleTween.Kill();
        }

        // Animate scale down to original
        _scaleTween = transform.DOScale(_originalScale, _scaleAnimationDuration)
            .SetEase(_scaleDownEase);
    }

    /// <summary>
    /// Creates a default UI canvas if none is assigned
    /// </summary>
    private void CreateDefaultUI()
    {
        GameObject canvasObj = new GameObject("HighlightCanvas");
        canvasObj.transform.SetParent(transform);
        canvasObj.transform.localPosition = _uiOffset;
        canvasObj.transform.localRotation = Quaternion.identity;
        
        // Make UI face the camera if available
        if (Camera.main != null)
        {
            canvasObj.transform.LookAt(Camera.main.transform);
            canvasObj.transform.Rotate(0, 180, 0); // Flip to face camera properly
        }

        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.dynamicPixelsPerUnit = 100;

        // Create background panel
        GameObject panelObj = new GameObject("Panel");
        panelObj.transform.SetParent(canvasObj.transform, false);
        RectTransform panelRect = panelObj.AddComponent<RectTransform>();
        panelRect.sizeDelta = new Vector2(200, 50);
        panelRect.localPosition = Vector3.zero;

        Image panelImage = panelObj.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.7f);

        // Create text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(panelObj.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;

        Text text = textObj.AddComponent<Text>();
        text.text = gameObject.name;
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 20;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;

        _highlightCanvas = canvas;
    }

    void OnDestroy()
    {
        // Clean up event subscriptions
        if (_gazeInteractable != null)
        {
            _gazeInteractable.OnGazeEnter.RemoveListener(OnGazeEnter);
            _gazeInteractable.OnGazeExit.RemoveListener(OnGazeExit);
            _gazeInteractable.OnGazeActivated.RemoveListener(OnGazeActivated);
        }

        // Kill any active tweens
        if (_uiSlideTween != null && _uiSlideTween.IsActive())
        {
            _uiSlideTween.Kill();
        }
        
        if (_scaleTween != null && _scaleTween.IsActive())
        {
            _scaleTween.Kill();
        }

        // Clean up UI
        if (_uiInstance != null)
        {
            Destroy(_uiInstance);
        }
    }
}
