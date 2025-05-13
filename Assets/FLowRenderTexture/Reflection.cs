using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GenerateWaterReflect : MonoBehaviour
{
    public Camera mainCamera = null;

    public Camera reflectionCamera = null;

    public bool isReflectionCameraRendering = false;

    public RenderTexture reflectionRT = null;
    // Start is called before the first frame update
    void Start()
    {
        // reflectionCamera = CreateReflectCamera();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // private void OnWillRenderObject()
    // {
    //     if (!isReflectionCameraRendering)
    //         return;
    //     // 实时更新相机的参数
    //     UpdateCameraParams(mainCamera, reflectionCamera);
    //
    //     // 修改view 矩阵
    //     Matrix4x4 reflectM = CaculateReflectMatrix();
    //     reflectionCamera.worldToCameraMatrix = mainCamera.worldToCameraMatrix * reflectM;
    //     // 更改远近平面
    //     // 1 用四维vec表示平面
    //     Vector3 normal = transform.up;
    //     float d = -Vector3.Dot(normal, transform.position);
    //     Vector4 planeVec = new Vector4(normal.x, normal.y, normal.z, d);
    //     // 2 将平面从世界坐标 转换到 view坐标下 平面的转换与点的转换不一样
    //     Vector4 planeVecView = reflectionCamera.worldToCameraMatrix.inverse.transpose * planeVec;
    //     // 3 用unity封装的api 得到 以新平面为near的新透视矩阵
    //     Matrix4x4 newProMatrix = reflectionCamera.CalculateObliqueMatrix(planeVecView);
    //     reflectionCamera.projectionMatrix = newProMatrix;
    //
    //     // 渲染
    //     // reflectionCamera.targetDisplay = 2;
    //     reflectionCamera.targetTexture = reflectionRT;
    //     // Debug.Log("Test");
    //     GL.invertCulling = true;
    //     // reflectionCamera.Render();
    //     GL.invertCulling = false;
    // }


    Matrix4x4 CaculateReflectMatrix()
    {
        var normal = transform.up;
        var d = -Vector3.Dot(normal, transform.position);
        var reflectM = new Matrix4x4();
        reflectM.m00 = 1 - 2 * normal.x * normal.x;
        reflectM.m01 = -2 * normal.x * normal.y;
        reflectM.m02 = -2 * normal.x * normal.z;
        reflectM.m03 = -2 * d * normal.x;

        reflectM.m10 = -2 * normal.x * normal.y;
        reflectM.m11 = 1 - 2 * normal.y * normal.y;
        reflectM.m12 = -2 * normal.y * normal.z;
        reflectM.m13 = -2 * d * normal.y;

        reflectM.m20 = -2 * normal.x * normal.z;
        reflectM.m21 = -2 * normal.y * normal.z;
        reflectM.m22 = 1 - 2 * normal.z * normal.z;
        reflectM.m23 = -2 * d * normal.z;

        reflectM.m30 = 0;
        reflectM.m31 = 0;
        reflectM.m32 = 0;
        reflectM.m33 = 1;
        return reflectM;
    }
    void UpdateCameraParams(Camera srcCamera, Camera destCamera)
    {
        destCamera.transform.position = srcCamera.transform.position;
        destCamera.cameraType = srcCamera.cameraType;
        destCamera.clearFlags = srcCamera.clearFlags;
        destCamera.backgroundColor = srcCamera.backgroundColor;
        destCamera.farClipPlane = srcCamera.farClipPlane;
        destCamera.nearClipPlane = srcCamera.nearClipPlane;
        destCamera.focalLength = srcCamera.focalLength;
        destCamera.orthographic = srcCamera.orthographic;
        destCamera.fieldOfView = srcCamera.fieldOfView;
        destCamera.aspect = srcCamera.aspect;
        destCamera.orthographicSize = srcCamera.orthographicSize;
    }



    private void OnEnable()
    {
        // RenderPipelineManager.beginCameraRendering += UpdateCamera;
        RenderPipelineManager.beginCameraRendering += UpdateCameraTest;
        RenderPipelineManager.endCameraRendering += UpdateCameraTestEnd;
    }

    private void OnDisable()
    {
        // RenderPipelineManager.beginCameraRendering -= UpdateCamera;
        RenderPipelineManager.beginCameraRendering -= UpdateCameraTest;
        RenderPipelineManager.beginCameraRendering -= UpdateCameraTestEnd;
    }

    private void UpdateCamera(ScriptableRenderContext src, Camera camera)
    {
        if (camera == reflectionCamera) // 我们只自定义渲染指定的相机即可
            return;
        Debug.Log($"Beginning rendering the camera: {camera.name}");

        // 开始自定义渲染流程
        if (!isReflectionCameraRendering)
            return;

        // reflectionCamera.enabled = false;
        reflectionCamera.depth = -11;
        // 实时更新相机的参数
        UpdateCameraParams(camera, reflectionCamera);

        // 修改view 矩阵
        Matrix4x4 reflectM = CaculateReflectMatrix();
        reflectionCamera.worldToCameraMatrix = camera.worldToCameraMatrix * reflectM;
        // 更改远近平面
        // 1 用四维vec表示平面
        Vector3 normal = transform.up;
        float d = -Vector3.Dot(normal, transform.position);
        Vector4 planeVec = new Vector4(normal.x, normal.y, normal.z, d);
        // 2 将平面从世界坐标 转换到 view坐标下 平面的转换与点的转换不一样
        Vector4 planeVecView = reflectionCamera.worldToCameraMatrix.inverse.transpose * planeVec;
        // 3 用unity封装的api 得到 以新平面为near的新透视矩阵
        Matrix4x4 newProMatrix = reflectionCamera.CalculateObliqueMatrix(planeVecView);
        reflectionCamera.projectionMatrix = newProMatrix;

        // 渲染
        // camera.targetDisplay = 2;
        reflectionCamera.targetTexture = reflectionRT;

        GL.invertCulling = true;
        UniversalRenderPipeline.RenderSingleCamera(src, reflectionCamera);
        GL.invertCulling = false;
    }

    private void UpdateCameraTest(ScriptableRenderContext src, Camera camera)
    {
        if (camera != reflectionCamera) // 我们只自定义渲染指定的相机即可
            return;
        Debug.Log($"Beginning rendering the camera: {camera.name}");

        // 开始自定义渲染流程
        if (!isReflectionCameraRendering)
            return;

        // reflectionCamera.enabled = false;
        reflectionCamera.depth = -11;
        // 实时更新相机的参数
        UpdateCameraParams(mainCamera, reflectionCamera);

        // 修改view 矩阵
        Matrix4x4 reflectM = CaculateReflectMatrix();
        reflectionCamera.worldToCameraMatrix = mainCamera.worldToCameraMatrix * reflectM;
        // 更改远近平面
        // 1 用四维vec表示平面
        Vector3 normal = transform.up;
        float d = -Vector3.Dot(normal, transform.position);
        Vector4 planeVec = new Vector4(normal.x, normal.y, normal.z, d);
        // 2 将平面从世界坐标 转换到 view坐标下 平面的转换与点的转换不一样
        Vector4 planeVecView = reflectionCamera.worldToCameraMatrix.inverse.transpose * planeVec;
        // 3 用unity封装的api 得到 以新平面为near的新透视矩阵
        Matrix4x4 newProMatrix = reflectionCamera.CalculateObliqueMatrix(planeVecView);
        reflectionCamera.projectionMatrix = newProMatrix;

        // 渲染
        // camera.targetDisplay = 2;
        reflectionCamera.targetTexture = reflectionRT;

        GL.invertCulling = true;
        // UniversalRenderPipeline.RenderSingleCamera(src, reflectionCamera);
        // GL.invertCulling = false;
    }

    private void UpdateCameraTestEnd(ScriptableRenderContext src, Camera camera)
    {
        GL.invertCulling = false;
    }

    private Camera CreateReflectCamera()
    {
        // 首先创建camera对象+给定名字
        var go = new GameObject(gameObject.name + " Planar Reflection Camera", typeof(Camera));
        // 需要给上固定需要的cameraData组件
        var cameraData = go.AddComponent(typeof(UniversalAdditionalCameraData)) as UniversalAdditionalCameraData;
        // 一些属性设置 后面那个setrender 应该是rendering data 这里选择我们的0
        cameraData.requiresColorOption = CameraOverrideOption.Off;
        cameraData.requiresDepthOption = CameraOverrideOption.Off;
        cameraData.renderShadows = false;
        cameraData.SetRenderer(0);  // 根据 render list 的索引选择 render TODO

        // 位置应该是无所谓的 后面会重新指定
        var t = transform;
        var reflectionCamera = go.GetComponent<Camera>();
        reflectionCamera.transform.SetPositionAndRotation(transform.position, t.rotation);  // 相机初始位置设为当前 gameobject 位置
        // 优先级需要比main camera的小 也就是更优先渲染
        reflectionCamera.depth = -10;  // 渲染优先级 [-100, 100]
        // 定位false 就无法被管线自动渲染，只能脚本控制自定义渲染 我是这么理解的
        reflectionCamera.enabled = false;
        go.hideFlags = HideFlags.HideAndDontSave;

        return reflectionCamera;
    }
}