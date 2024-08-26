using UnityEngine;
using UnityEditor;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class CustomLitShaderGUI : ShaderGUI
{
    private static bool _showMainSettings = true;
    private static bool _showAlphaCutoffSetting = true;
    private static bool _showNormalBumpMapSetting = true;
    private static bool _showMetallicSetting = true;
    private static bool _showSmoothnessSetting = true;
    private static bool _showEmissionSetting = true;
    private static bool _showOutLineSetting = true;

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        Material material = materialEditor.target as Material;

        EditorGUI.BeginChangeCheck();

        // Main Settings
        _showMainSettings = EditorGUILayout.Foldout(_showMainSettings, "Main Settings");
        if (_showMainSettings)
        {
            EditorGUILayout.BeginVertical("box");
            DrawProperties(materialEditor, properties, "_BaseMap", "_BaseColor", "_ShadowColor", "_ShadowStrength",
                "_DitherLevel");
            EditorGUILayout.EndVertical();
        }

        // Alpha Cutoff
        _showAlphaCutoffSetting = EditorGUILayout.Foldout(_showAlphaCutoffSetting, "Alpha Cutoff");
        if (_showAlphaCutoffSetting)
        {
            EditorGUILayout.BeginVertical("box");
            DrawProperties(materialEditor, properties, "_EnableAlphaTest", "_Cutoff");
            EditorGUILayout.EndVertical();
        }

        // Normal/Bump Map
        _showNormalBumpMapSetting = EditorGUILayout.Foldout(_showNormalBumpMapSetting, "Normal/Bump Map");
        if (_showNormalBumpMapSetting)
        {
            EditorGUILayout.BeginVertical("box");
            DrawProperties(materialEditor, properties, "_EnableBumpMap", "_BumpMap", "_BumpScale");
            EditorGUILayout.EndVertical();
        }

        // Metallic
        _showMetallicSetting = EditorGUILayout.Foldout(_showMetallicSetting, "Metallic");
        if (_showMetallicSetting)
        {
            EditorGUILayout.BeginVertical("box");
            DrawProperties(materialEditor, properties, "_EnableMetallic", "_MetallicGlossMap", "_Metallic");
            EditorGUILayout.EndVertical();
        }

        // Smoothness/Roughness
        _showSmoothnessSetting = EditorGUILayout.Foldout(_showSmoothnessSetting, "Smoothness/Roughness");
        if (_showSmoothnessSetting)
        {
            EditorGUILayout.BeginVertical("box");
            DrawProperties(materialEditor, properties, "_SmoothnessType", "_SmoothnessRoughnessMap", "_Smoothness");
            EditorGUILayout.EndVertical();
        }

        // Emission
        _showEmissionSetting = EditorGUILayout.Foldout(_showEmissionSetting, "Emission");
        if (_showEmissionSetting)
        {
            EditorGUILayout.BeginVertical("box");
            DrawProperties(materialEditor, properties, "_EnableEmission", "_EmissionMap", "_EmissionColor");
            EditorGUILayout.EndVertical();
        }

        // OutLine
        _showOutLineSetting = EditorGUILayout.Foldout(_showOutLineSetting, "OutLine");
        if (_showOutLineSetting)
        {
            EditorGUILayout.BeginVertical("box");
            DrawProperties(materialEditor, properties, "_EnableOutLine", "_OutLineColor", "_OutlineWidth");
            EditorGUILayout.EndVertical();
        }

        if (EditorGUI.EndChangeCheck())
        {
            foreach (var obj in materialEditor.targets)
            {
                MaterialChanged((Material)obj);
            }
        }
    }

    private void DrawProperties(MaterialEditor materialEditor, MaterialProperty[] properties,
        params string[] propertyNames)
    {
        foreach (var propertyName in propertyNames)
        {
            MaterialProperty property = FindProperty(propertyName, properties);
            materialEditor.ShaderProperty(property, property.displayName);
        }
    }

    private static void MaterialChanged(Material material)
    {
        // Implement any changes that need to happen when properties are modified
    }
}