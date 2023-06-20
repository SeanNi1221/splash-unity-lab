using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using Unity.Profiling;
using Unity.Profiling.LowLevel.Unsafe;

public class ProfilerController : MonoBehaviour
{
    /************************************************************************************************************
    * Source: https://docs.unity3d.com/2020.2/Documentation/ScriptReference/Unity.Profiling.ProfilerRecorder.html
    *************************************************************************************************************/

    /************************************************************************************************************
    * Mod: Modified by Sean in 2023
    *************************************************************************************************************/

    string statsText;

    //Overview
    ProfilerRecorder mainThreadTimeRecorder;
    ProfilerRecorder totalUsedMemoryRecorder;
    ProfilerRecorder totalReservedMemoryRecorder;
    ProfilerRecorder gcMemoryRecorder;

    //Graphics
    ProfilerRecorder verticesCountRecorder;
    // ProfilerRecorder meshMemoryRecorder; //Not supported by Web GL
    // ProfilerRecorder textureMemoryRecorder; //Not supported by Web GL
    ProfilerRecorder renderTexturesRecorder;
    ProfilerRecorder drawCallsCountRecorder;

    //Physics
    // ProfilerRecorder physicsMemoryRecorder;
    // ProfilerRecorder activeDynamicBodiesRecorder;

    //Management
    public int infoAreaHeight;

    void OnEnable()
    {
        //Overview
        mainThreadTimeRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "Main Thread", 15);
        totalUsedMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Total Used Memory");
        totalReservedMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Total Reserved Memory");
        gcMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Reserved Memory");

        //Graphics
        // meshMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Mesh Memory"); //Not supported by Web GL
        verticesCountRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Vertices Count");
        // textureMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Texture Memory"); //Not supported by Web GL
        renderTexturesRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Render Textures Count");
        drawCallsCountRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count");

        //Physics
        // physicsMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Physics, "Physics Used Memory");
        // activeDynamicBodiesRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Physics, "Active Dynamic Bodies");

    }

    void OnDisable()
    {
        //Overview
        mainThreadTimeRecorder.Dispose();
        totalUsedMemoryRecorder.Dispose();
        totalReservedMemoryRecorder.Dispose();
        gcMemoryRecorder.Dispose();

        //Graphics
        // meshMemoryRecorder.Dispose(); 
        verticesCountRecorder.Dispose();
        // textureMemoryRecorder.Dispose();
        renderTexturesRecorder.Dispose();
        drawCallsCountRecorder.Dispose();

        //Physics
        // physicsMemoryRecorder.Dispose();
        // activeDynamicBodiesRecorder.Dispose();
    }

    void Update()
    {
        //Data Processing for the ease of read
        var frameTimeNano = GetRecorderFrameAverage(mainThreadTimeRecorder);
        // var physicsMemory = GetRecorderFrameAverage(physicsMemoryRecorder);

        var sb = new StringBuilder(1000);

        //Overall
        sb.AppendLine("----OVERALL----");
        sb.AppendLine($"Frame Rate: {(1e9f)/frameTimeNano:F0} FPS");
        sb.AppendLine($"Used Memory: {totalUsedMemoryRecorder.LastValue / (1024 * 1024)} MB");
        sb.AppendLine($"Reserved Memory: {totalReservedMemoryRecorder.LastValue / (1024 * 1024)} MB");
        sb.AppendLine($"GC Memory: {gcMemoryRecorder.LastValue / (1024 * 1024)} MB");

        //Graphics
        sb.AppendLine(string.Empty);
        sb.AppendLine("----GRAPHICS----");
        // sb.AppendLine($"Mesh Memory: {meshMemoryRecorder.LastValue / (1024 * 1024)} MB"); //Not supported by Web GL
        sb.AppendLine($"Vertices: {(verticesCountRecorder.LastValue * 1e-6f):F2} M");
        // sb.AppendLine($"Texture Memory: {textureMemoryRecorder.LastValue / (1024 * 1024)} MB"); //Not supported by Web GL
        sb.AppendLine($"Render Textures: {renderTexturesRecorder.LastValue}");
        sb.AppendLine($"Draw Calls: {drawCallsCountRecorder.LastValue}");

        //Physics
        sb.AppendLine(string.Empty);
        sb.AppendLine("----PHYSICS----");
        sb.AppendLine("Coming soon...");

        // sb.AppendLine($"Physics Memory: {(physicsMemory/(1024 * 1024)):F1} MB");
        // sb.AppendLine($"Active Dynamic Bodies: {activeDynamicBodiesRecorder.LastValue}");


        statsText = sb.ToString();
        sb.AppendLine(string.Empty);
    }

    void OnGUI()
    {
        GUI.TextArea(new Rect(10, 30, 250, 260), statsText);
    }

    static double GetRecorderFrameAverage(ProfilerRecorder recorder)
    {
        var samplesCount = recorder.Capacity;
        if (samplesCount == 0)
            return 0;

        double r = 0;
        var samples = new List<ProfilerRecorderSample>(samplesCount);
        recorder.CopyTo(samples);
        for (var i = 0; i < samples.Count; ++i)
            r += samples[i].Value;
        r /= samplesCount;

        return r;
    }
}
