using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class ReplaceRuleTileSheet : MonoBehaviour
{
    public RuleTile ruleTileToUpdate;
    public Texture replaceWith; // choose the default sprite & it will replace all of the sprites with the new sheet

    [Header("Tap to Replace")]
    public bool replace = false;

    private void Update()
    {

        if (replace) ReplaceRT();

    }

    public void ReplaceRT()
    {
        List<RuleTile.TilingRule> trList = ruleTileToUpdate.m_TilingRules; // Load all Tiling Rules from the RuleTile

        // Get the sprites associated with this texture
        string path = AssetDatabase.GetAssetPath(replaceWith); // Get the path of the new texture
        //path = path.Replace("Assets/Resources/", ""); // Remove "Resources" path
        //path = path.Replace(".png", ""); // Remove file extension
        Object[] objects = AssetDatabase.LoadAllAssetsAtPath(path);
        Sprite[] sprites = objects.Where(x => x is Sprite).Cast<Sprite>().ToArray();

        string defaultSpriteName = ruleTileToUpdate.m_DefaultSprite.name;
        int defaultSpriteNumber = int.Parse(defaultSpriteName.Substring(defaultSpriteName.LastIndexOf('_') + 1));
        
        // Get the name & replace with the numbered tile of the "replaceWith" sprite's name. Set the Replaced Default tile to the same number as the original tilesheet
        foreach (RuleTile.TilingRule tr in trList)
        {
            string tileSpriteName = tr.m_Sprites[0].name;
            int tileSpriteNumber = int.Parse(tileSpriteName.Substring(tileSpriteName.LastIndexOf('_') + 1));

            // Replace with the new sprite
            if (tileSpriteNumber < sprites.Length)
            {
                tr.m_Sprites[0] = sprites[tileSpriteNumber];
            }

            // If default sprite, update default
            if (defaultSpriteNumber == tileSpriteNumber)
            {
                ruleTileToUpdate.m_DefaultSprite = sprites[tileSpriteNumber];
            }
        }
        EditorUtility.SetDirty(ruleTileToUpdate);
        replace = false;
    }
}
