using System.Collections;
using System.Collections.Generic;
using Environment;
using UnityEngine;

namespace Environment
{
	/// <summary>
	/// Manages the map; exposes interactions with tiles.
	/// </summary>
	public class Map : MonoBehaviour
	{
		// CONSTANTS
		/// <summary>
		/// The full-size (size measured between opposite edges) of tiles in the scene (width & height).
		/// </summary>
		public const float TileSize = 0.1f;
		/// <summary>
		/// The half-size (size measured from the center to the edge) of tiles in the scene (width & height).
		/// </summary>
		public const float TileHalfSize = TileSize / 2;
		/// <summary>
		/// The offset to apply to the position of any given tile.
		/// </summary>
		public static readonly Vector2 TileOffset = new(TileSize, TileSize);
		
		// FIELDS - TILE
		/// <summary>
		/// The types of tiles, indexed by typeID.
		/// </summary>
		public TileType[] TileTypes;
		/// <summary>
		/// The instances of tiles for the current map.
		/// </summary>
		public TileInstance[,] TileInstances;
		
		// FIELDS - VIEWPORT
		/// <summary>
		/// The top-left tile of the player's screen.
		/// </summary>
		private uint _cornerTopLeft;
		/// <summary>
		/// The bottom-right tile of the player's screen.
		/// </summary>
		private uint _cornerBottomRight;
		
		// FUNCTIONS - VIEWPORT
		/// <summary>
		/// Adjusts the tile GameObjects in-scene to reflect the underlying map.
		/// Pools ones that are no longer visible, allocates now-visible ones.
		/// </summary>
		/// <param name="viewportCenter">The center of the viewport.</param>
		/// <param name="viewportHalfSize">The half-width and half-height of the viewport.</param>
		public void AdjustViewportTiles(Vector2 viewportCenter, Vector2 viewportHalfSize)
		{
			
		}

		// FUNCTIONS - MAP DATA
		/// <summary>
		/// Generates a map with the given dimensions.
		/// </summary>
		public void GenerateMap(uint width, uint height)
		{
			// Initialize the tile array.
			TileInstances = new TileInstance[width, height];
			
			// Todo: Perform generation
			// Todo: Perform point-of-interest (caves, loot, etc.) generation
			// Todo: Perform world-gen validation
			// Todo: Perform navigation graph generation.
		}
		/// <summary>
		/// Damage the given tiles, lowering their durability and breaking them if they have no durability remaining.
		/// </summary>
		/// <param name="x">The x-coordinate of the tile to be damaged.</param>
		/// <param name="y">The y-coordinate of the tile to be damaged.</param>
		/// <param name="damage">The amount of damage to deal to the tile.</param>
		public void DamageTile(uint x, uint y, uint damage)
		{
			
		}
		/// <summary>
		/// Break the given tile, removing it from the map and rolling on its loot table.
		/// </summary>
		/// <param name="x">The x-coordinate of the tile to be broken.</param>
		/// <param name="y">The y-coordinate of the tile to be broken.</param>
		public void BreakTile(uint x, uint y)
		{
			// Todo: Remove/mark the tile as destroyed.
			// Todo: Update the navigation graph.
		}
	}

}