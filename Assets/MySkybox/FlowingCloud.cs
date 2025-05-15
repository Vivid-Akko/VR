using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public class FlowingCloud : MonoBehaviour
{
    public Volume globalVolume; // 你的全局 Volume
    private VolumetricClouds volumetricClouds;
    void Start()
    {
        if (globalVolume == null)
        {
            Debug.LogError("Global Volume is not assigned.");
            return;
        }

        // 从 Volume Profile 中获取你的自定义 VolumeComponent
        if (globalVolume.profile.TryGet(out volumetricClouds))
        {
            Debug.Log("VolumetricvolumetricClouds found!");
        }
        else
        {
            Debug.LogError("VolumetricvolumetricClouds not found in this Volume's Profile!");
        }
    }

    void Update()
    {
        if (volumetricClouds != null)
        {
            // 控制 shapeOffset 和 earthCurvature
            volumetricClouds.shapeOffset.value = new Vector3(
                0f,
                Time.time * 0.5f * 0.005f,
                0f
            );

            //volumetricClouds.earthCurvature.value = Mathf.PingPong(Time.time * 0.001f, 1f);
        }
    }
}
