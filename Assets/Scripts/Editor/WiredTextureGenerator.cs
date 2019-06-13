using System;
using System.IO;
using UnityEngine;
using UnityEditor;

public class WiredTextureGenerator : EditorWindow
{
    #region Public Attributes
    public const string assetFolder = "Assets7Textures";
    #endregion

    #region Private Attributes
    private static string _name = "Wired_Texture";
    private static int _width = 512;
    private static int _heigth = 512;
    private static int _totalWires = 20;
    private static int _wireWidth = 5;
    private static Color _wireColor = new Color(1.0f, 1.0f, 1.0f);
    private static float _textureSpeed;
    #endregion

    #region Editor
    [MenuItem("Texture Generator")]
    private static void ShowMenu()
    {
        WiredTextureGenerator window = EditorWindow.GetWindow<WiredTextureGenerator>("Wired Texture Generator");
        window.minSize = new Vector2(250.0f, 100.0f);
        window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.Separator();
        GUILayout.Label("Name of your Texture", EditorStyles.boldLabel);
        _name = EditorGUILayout.TextField("Wired Texture Name", _name);

        EditorGUILayout.Separator();
        GUILayout.Label("Texture info", EditorStyles.boldLabel);
        _width = EditorGUILayout.

        if (GUILayout.Button("Generate Fancy Sphere"))
        {
            if (CreateWiredTexture())
            {
                Debug.Log("You successfully created a wired texture of size " + _width + "x" + _heigth + " and " + _totalWires + " wires");
            }
            else
            {
                Debug.LogWarning("Error creating the wired texture.");
            }
        }
    }
    #endregion

    #region Methods
    private bool CreateWiredTexture()
    {
        bool created = true;



        return created;
    }
    #endregion
}
