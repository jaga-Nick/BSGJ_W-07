using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace InGame.NonMVP
{
    [CreateAssetMenu(fileName = "NoEntryTileData", menuName = "ScriptableObjects/NoEntryTileData")]
    public class NoEntryTiles : ScriptableObject
    {
        public List<TileBase> noEntryTiles;
    }
}