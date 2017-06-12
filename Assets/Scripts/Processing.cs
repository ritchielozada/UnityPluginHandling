#define WRCCPP

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

#if !UNITY_EDITOR && UNITY_WSA
using UniversalCSharp;
using UniversalWRCCSharp;
#endif

public class Processing : MonoBehaviour
{
    public TextMesh StatusText;
    public TextMesh StatusText2;
    public Renderer RenderTexture;
    public float SampleDuration = 5f;
        
    private int counter;
    private float endTime;
    private float fpsMeasure;
    private int taskCount;

#if !UNITY_EDITOR && UNITY_WSA
    private bool isImageFrameReady = false;
    private const int textureWidth = 1000;
    private const int textureHeight = 1000;
    private float TextureScale = 1f;
    private int PluginMode = 2;
    
    private UniversalCSharp.PluginTask pluginTask;
    private UniversalWRCCSharp.PluginTask pluginTaskWRC;

#if WRCCPP
    [DllImport("UniversalWRCCpp")]
#else
    [DllImport("UniversalCpp")]
#endif
    private static extern void SetTextureFromUnity(System.IntPtr texture, int w, int h);

#if WRCCPP
    [DllImport("UniversalWRCCpp")]
#else
    [DllImport("UniversalCpp")]
#endif
    private static extern void ProcessBuffer(IntPtr data, uint dataSize);

#if WRCCPP
    [DllImport("UniversalWRCCpp")]
#else
    [DllImport("UniversalCpp")]
#endif
    private static extern IntPtr GetRenderEventFunc();

#if WRCCPP
    [DllImport("UniversalWRCCpp")]
#else
    [DllImport("UniversalCpp")]
#endif
    private static extern void SetPluginMode(int mode);
#endif

    void Start()
    {
        StatusText.text = "Computing...";
        counter = 0;
        taskCount = 0;
        endTime = Time.time + SampleDuration;

#if !UNITY_EDITOR && UNITY_WSA
        pluginTask = new UniversalCSharp.PluginTask();
        pluginTaskWRC = new UniversalWRCCSharp.PluginTask();
        StatusText2.text = string.Format(
            "{0}\n{1}", 
            pluginTask.GetInfo(),
            pluginTaskWRC.GetInfo());

        CreateTextureAndPassToPlugin();
        SetPluginMode(PluginMode);
        StartCoroutine(CallPluginAtEndOfFrames());
#endif

        // Slow Coroutine
        //StartCoroutine(SlowCoroutine());



    }

    void Update()
    {
        counter++;
        if (Time.time > endTime)
        {
            endTime = Time.time + SampleDuration;
            fpsMeasure = counter / SampleDuration;
            counter = 0;
            StatusText.text = string.Format("FPS: {0:0.00} - {1}", fpsMeasure, taskCount);
        }
    }

    IEnumerator SlowCoroutine()
    {
        while (true)
        {
            for (int x = 0; x < 3840; x++)
            {
                for (int y = 0; y < 2160; y++)
                {
                    taskCount++;                    
                }
            }
            yield return null;
        }        
    }

#if !UNITY_EDITOR && UNITY_WSA
    private void CreateTextureAndPassToPlugin()
    {
        RenderTexture.transform.localScale = new Vector3(-TextureScale, (float)textureHeight / textureWidth * TextureScale, 1f);
        Texture2D tex = new Texture2D(textureWidth, textureHeight, TextureFormat.ARGB32, false);
        tex.filterMode = FilterMode.Point;
        tex.Apply();
        RenderTexture.material.mainTexture = tex;
        SetTextureFromUnity(tex.GetNativeTexturePtr(), tex.width, tex.height);
    }

    private IEnumerator CallPluginAtEndOfFrames()
    {
        while (true)
        {
            // Wait until all frame rendering is done
            yield return new WaitForEndOfFrame();

            // Issue a plugin event with arbitrary integer identifier.
            // The plugin can distinguish between different
            // things it needs to do based on this ID.
            // For our simple plugin, it does not matter which ID we pass here.
            switch (PluginMode)
            {
                case 0:
                    if (!isImageFrameReady)
                    {
                        GL.IssuePluginEvent(GetRenderEventFunc(), 1);
                        isImageFrameReady = true;
                    }
                    break;
                default:
                    GL.IssuePluginEvent(GetRenderEventFunc(), 1);
                    break;
            }
        }
    }
#endif
}
