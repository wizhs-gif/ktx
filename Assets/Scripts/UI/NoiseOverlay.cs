using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 程序化噪点覆盖效果
/// 挂载在噪点 Image 物体上，通过 SetIntensity 控制噪点强度
/// </summary>
[RequireComponent(typeof(Image))]
public class NoiseOverlay : MonoBehaviour
{
    [Header("噪点配置")]
    [SerializeField] private float noiseSpeed = 8f;
    [SerializeField] private float noiseScale = 50f;

    private Material noiseMaterial;
    private Image image;
    private static readonly int IntensityProp = Shader.PropertyToID("_NoiseIntensity");
    private static readonly int SpeedProp = Shader.PropertyToID("_NoiseSpeed");
    private static readonly int ScaleProp = Shader.PropertyToID("_NoiseScale");

    private void Awake()
    {
        image = GetComponent<Image>();

        // 加载噪点着色器并创建材质
        Shader shader = Shader.Find("UI/NoiseGrain");
        if (shader != null)
        {
            noiseMaterial = new Material(shader);
            noiseMaterial.SetFloat(SpeedProp, noiseSpeed);
            noiseMaterial.SetFloat(ScaleProp, noiseScale);
            noiseMaterial.SetFloat(IntensityProp, 0f);
            image.material = noiseMaterial;
        }
        else
        {
            Debug.LogWarning("[NoiseOverlay] 未找到 UI/NoiseGrain 着色器，噪点效果不可用");
        }
    }

    /// <summary>
    /// 设置噪点强度（0 = 无噪点, 1 = 最大噪点）
    /// </summary>
    public void SetIntensity(float intensity)
    {
        if (noiseMaterial != null)
        {
            noiseMaterial.SetFloat(IntensityProp, Mathf.Clamp01(intensity));
        }
    }

    private void OnDestroy()
    {
        if (noiseMaterial != null)
        {
            Destroy(noiseMaterial);
        }
    }
}
