using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CaptionedAudioCreator : EditorWindow
{
    [MenuItem("Tools/CaptionedAudio Creator")]
    public static void ShowWindow()
    {
        GetWindow<CaptionedAudioCreator>("CaptionedAudio Creator");
    }

    private string folderPath = "Assets/Audio";
    private string speakerName = "";

    private void OnGUI()
    {
        GUILayout.Label("CaptionedAudio Creator", EditorStyles.boldLabel);

        folderPath = EditorGUILayout.TextField("Folder Path", folderPath);
        speakerName = EditorGUILayout.TextField("Speaker Name", speakerName);

        if (GUILayout.Button("Create CaptionedAudio"))
        {
            CreateCaptionedAudioObjects();
        }
    }

    private void CreateCaptionedAudioObjects()
    {
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            Debug.LogError($"Folder path '{folderPath}' is invalid. Please create the folder first.");
            return;
        }


        var subFolders = AssetDatabase.GetSubFolders(folderPath);
        var allFolders = subFolders.Prepend(folderPath).ToArray();

        var audioFiles = AssetDatabase.FindAssets("t:AudioClip", allFolders);
        var srtFiles = AssetDatabase.FindAssets("t:TextAsset", allFolders);

        var audioClipsByName = audioFiles.Select(id => AssetDatabase.GUIDToAssetPath(id)).ToDictionary(
            path => Path.Join(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path)),
            path => AssetDatabase.LoadAssetAtPath(path, typeof(AudioClip)) as AudioClip);
        var srtFilesByName = srtFiles.Select(id => AssetDatabase.GUIDToAssetPath(id))
            .Where(path => Path.GetExtension(path).EndsWith("srt", System.StringComparison.OrdinalIgnoreCase))
            .ToDictionary(
                path => Path.Join(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path)),
                path => AssetDatabase.LoadAssetAtPath(path, typeof(TextAsset)) as TextAsset);

        var commonPaths = audioClipsByName.Keys.Intersect(srtFilesByName.Keys).ToList();
        var audioWithoutMatch = audioClipsByName.Keys.Except(srtFilesByName.Keys).ToList();
        var srtWithoutMatch = srtFilesByName.Keys.Except(audioClipsByName.Keys).ToList();

        if (audioWithoutMatch.Count > 0)
        {
            audioWithoutMatch.Sort();
            Debug.LogError($"Found audio files without a matching srt file:[{string.Join(", ", audioWithoutMatch)}]");
        }

        if (srtWithoutMatch.Count > 0)
        {
            srtWithoutMatch.Sort();
            Debug.LogError($"Found srt files without a matching audio file:[{string.Join(", ", srtWithoutMatch)}]");
        }

        if (commonPaths.Count > 0)
        {
            foreach (var commonPath in commonPaths)
            {
                // Check if asset already exists
                var assetPath = commonPath + ".asset";
                bool exists = AssetDatabase.AssetPathExists(assetPath);
                CaptionedAudio captionedAudio = null;
                if (exists)
                {
                    // Check if it's not of type captioned audio
                    var type = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
                    captionedAudio = AssetDatabase.LoadAssetAtPath(assetPath, typeof(CaptionedAudio)) as CaptionedAudio;
                    if (captionedAudio == null)
                    {
                        Debug.Log($"Found existing asset at path:{assetPath} of unexpected type:{type}, skipping this file.");
                        continue;
                    }

                    // Check if it's already created
                    // Debug.Log($"Found existing asset at path:{assetPath}, updating existing file's clip, srt, and speaker.");
                }
                else
                {
                    // Create new asset
                    captionedAudio = ScriptableObject.CreateInstance<CaptionedAudio>();
                }

                captionedAudio.audioClip = audioClipsByName[commonPath];
                captionedAudio.captionsSrt = srtFilesByName[commonPath];
                if (!string.IsNullOrEmpty(speakerName))
                {
                    captionedAudio.speakerName = speakerName;
                }

                if (!exists)
                {
                    AssetDatabase.CreateAsset(captionedAudio, assetPath);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
