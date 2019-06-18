using System;
using System.IO;
using UnityEngine;
using UnityEditor;

public class WiredTextureGenerator : EditorWindow
{
    #region Public Attributes
    public const string assetsFolder = "Assets/Textures";
    #endregion

    #region Private Attributes
    private static string _name = "Wired_Texture";
    private static int _width = 512;
    private static int _height = 512;
    private static int _totalWires = 20;
    private static int _wireThickness = 3;
    private static Color _wireColor = new Color(1.0f, 1.0f, 1.0f);
    private static float _textureSpeed;
    private static Color[] _pixels;
    #endregion

    #region Properties
    public int PixelsPerDivision
    {
        get { return _width / _totalWires; }
    }

    public int Transparents
    {
        get { return PixelsPerDivision - _wireThickness; }
    }
    #endregion

    #region Editor
    [MenuItem("Texture Generator/Wired Texture")]
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
        GUILayout.Label("Texture Info", EditorStyles.boldLabel);
        _width = EditorGUILayout.IntField("Texture Width", _width);
        _height = EditorGUILayout.IntField("Texture Heigth", _height);

        EditorGUILayout.Separator();
        GUILayout.Label("Wires");
        _totalWires = EditorGUILayout.IntSlider("Total Wires", _totalWires, 1, 100);
        _wireThickness = EditorGUILayout.IntSlider("Wire Thickness (px)", _wireThickness, 1, 256);
        _wireColor = EditorGUILayout.ColorField("Wire color", _wireColor);

        if (GUILayout.Button("Generate Wired Texture"))
        {
            if (CreateWiredTexture())
            {
                Debug.Log("You successfully created a wired texture of size " + _width + "x" + _height + " and " + _totalWires + " wires");
            }
            else
            {
                Debug.LogWarning("Error creating the wired texture.");
            }
        }
    }
    #endregion

    #region Methods
    /// <summary>
    /// This method handles all the process to create the wired texture for the fancy sphere
    /// </summary>
    /// <returns></returns>
    private bool CreateWiredTexture()
    {
        bool created = true;
        int pixelsPerDivision = _height / _totalWires;
        int transparentPixelsPerDivision = pixelsPerDivision - _wireThickness;

        try
        {
            _pixels = new Color[_width * _height];

            // Generate the wired Texture
            GenerateWiredTexture();

            // Save the texture
            SaveTexture();

            // Refresh the assets database
            AssetDatabase.Refresh();
        }
        catch (Exception e)
        {
            Debug.LogWarningFormat("The Wire Texture could not be successfully generated. Here is why: {0}", e.ToString());
            created = false;
        }

        return created;
    }

    /// <summary>
    /// This methos tries to save the texture we generated
    /// </summary>
    /// <returns>
    /// Returns true if the texture was successfuly saved, false otherwise
    /// </returns>
    private static bool SaveTexture()
    {
        bool saved = true;
        string path = Path.Combine(assetsFolder, _name + ".png");

        try
        {
            // Create or look for the directory we will store our texture in
            if (!Directory.Exists(assetsFolder))
            {
                Directory.CreateDirectory(assetsFolder);
            }

            // Create the texture
            Texture2D texture = new Texture2D(_width, _height, TextureFormat.ARGB32, false);
            texture.SetPixels(_pixels);

            // Store the mesh
            byte[] pngBytes = texture.EncodeToPNG();

            // Save all these bytes to a file
            File.WriteAllBytes(path, pngBytes);
        }
        catch (Exception e)
        {
            Debug.LogWarningFormat("Error creating texture [{1}]: {0}", e.ToString(), path);
            saved = false;
        }

        return saved;
    }

    /// <summary>
    /// Thies method sets the color for each pixel of our texture
    /// </summary>
    private void GenerateWiredTexture()
    {
        bool paint = true;
        int counter = 0;

        for (int h = 0; h < _height; h++)
        {
            // Paint coloured lines
            if (paint)
            {
                for (int w = 0; w < _width; w++)
                {
                    _pixels[(h * _width) + w] = _wireColor;
                }

                if (counter < _wireThickness)
                {
                    counter++;
                }
                else
                {
                    paint = false;
                    counter = 0;
                }
            }
            // Paint blanks
            else
            {
                for (int w = 0; w < _width; w++)
                {
                    _pixels[(h * _width) + w] = new Color(0.0f, 0.0f, 0.0f, 0.0f);
                }

                if (counter < Transparents)
                {
                    counter++;
                }
                else
                {
                    paint = true;
                    counter = 0;
                }
            }
        }
    }
    #endregion
}
