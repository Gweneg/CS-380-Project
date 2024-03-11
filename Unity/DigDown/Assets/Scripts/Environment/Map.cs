using System;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

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
		/// The different types tiles could be.
		/// </summary>
		public static readonly TileType[] TileTypes =
		{
			new(typeID: 0, textureID: 0, durabilityMax: 1, durabilityHardness: 0.0f, isSolid: true),   // Grass (to Dirt).
			new(typeID: 1, textureID: 1, durabilityMax: 2, durabilityHardness: 0.1f, isSolid: true),   // Dirt.
			new(typeID: 2, textureID: 2, durabilityMax: 4, durabilityHardness: 0.25f, isSolid: true),  // Dirt to Dirter.
			new(typeID: 3, textureID: 3, durabilityMax: 6, durabilityHardness: 0.5f, isSolid: true),   // Dirter.
			new(typeID: 4, textureID: 4, durabilityMax: 8, durabilityHardness: 0.75f, isSolid: true),  // Dirter to Dirtest.
			new(typeID: 5, textureID: 5, durabilityMax: 12, durabilityHardness: 1.25f, isSolid: true), // Dirtest.
			new(typeID: 6, textureID: 6, durabilityMax: 14, durabilityHardness: 1.75f, isSolid: true), // Dirtest to Stone.
			new(typeID: 7, textureID: 7, durabilityMax: 20, durabilityHardness: 2.0f, isSolid: true),  // Stone.
			new(typeID: 8, textureID: 8, durabilityMax: 255, durabilityHardness: -1, isSolid: true)    // Void.
		};
		/// <summary>
		/// The instances of tiles for the current map.
		/// </summary>
		public TileInstance[,] TileInstances;
		/// <summary>
		/// The width of the map, measured in tiles.
		/// </summary>
		public static uint MapWidth
		{
			get;
			private set;
		}
		/// <summary>
		/// The height of the map, measured in tiles.
		/// </summary>
		public static uint MapHeight
		{
			get;
			private set;
		}

		// FIELDS - VIEWPORT
		/// <summary>
		/// The offset of the map from the world origin.
		/// </summary>
		private static Vector2 _mapOffset;
		/// <summary>
		/// The top-left tile of the player's screen.
		/// </summary>
		private (uint, uint) _renderTopLeftCoordinate;

		
		// FUNCTIONS - VIEWPORT
		/// <summary>
		/// Adjusts the tile GameObjects in-scene to reflect the underlying map.
		/// Pools ones that are no longer visible, allocates now-visible ones.
		/// </summary>
		/// <param name="renderCenter">The center of the viewport.</param>
		/// <param name="renderHalfSize">The half-width and half-height of the viewport.</param>
		public void AdjustViewportTiles(Vector2 renderCenter, Vector2 renderHalfSize)
		{
			// Compute the upper-left coordinates of the viewport.
			(uint newUpperLeftCornerX, uint newUpperLeftCornerY) = GetTileCoordinates(renderCenter - renderHalfSize);
			// Compute the lower-right coordinates of the viewport.
			(uint newLowerRightCornerX, uint newLowerRightCornerY) = GetTileCoordinates(renderCenter + renderHalfSize);
			
			// Cache the previous upper-left viewport coordinates.
			(uint oldUpperLeftCornerX, uint oldUpperLeftCornerY) = _renderTopLeftCoordinate;
			
			// Todo: Reallocate now-invisible TileObjects to the newly visible ones. Perhaps a circular-buffer/deque mix?
			// Check the new coordinates against the old ones.
			if (newUpperLeftCornerX > oldUpperLeftCornerX) // Checks for: load tiles right, unload left.
			{
				
			}
			else if (newUpperLeftCornerX < oldUpperLeftCornerX) // Checks for: load tiles left, unload right.
			{
				
			}
			
			if (newUpperLeftCornerY > oldUpperLeftCornerY) // Checks for: load tiles top, unload bottom.
			{
				
			}
			else if (newUpperLeftCornerY < oldUpperLeftCornerY) // Checks for: unload tiles top, load bottom.
			{
				
			}
		}

		// FUNCTIONS - MAP INFO
		/// <summary>
		/// Gets the grid coordinates of the tile at the given position.
		/// </summary>
		/// <param name="position">The world-space position to get the tile position at.</param>
		/// <returns>The (X,Y) indices/coordinates of the tile at the given position.</returns>
		public Tuple<uint, uint> GetTileCoordinates(Vector2 position)
		{
			// Compute the coordinate by getting the relative position from the bottom-left corner of the map, then converting into tiles (instead of distance). 
			Vector2 offset = (position - _mapOffset) / TileSize;
			// Round and clamp the coordinates to make them safe for use in indexing the map array.
			// Note: This means out-of-range coordinates will be clamped to the edge of the map. Add a warn?
			uint xCoordinate = (uint)Math.Clamp(offset.x, 0, MapWidth - 1);
			uint yCoordinate = (uint)Math.Clamp(offset.y, 0, MapHeight - 1);
			// Return the coordinates as a tuple.
			return new Tuple<uint, uint>(xCoordinate, yCoordinate);
		}

		// METHODS - SCENE INTERACTION
		/// <summary>
		/// Positions the map such that the middle of the bottom of it lies on the given point.
		/// </summary>
		/// <param name="position">The position (in world-space) to position the bottom-center of the map over.</param>
		public void MoveMap(Vector2 position)
		{
			// Cache the map's transform.
			Transform mapTransform = transform;
			// Set the map's position.
			mapTransform.position = position + Vector2.left * TileSize;
			// Update the offset.
			_mapOffset = mapTransform.position;
		}
		
		// METHODS - MAP INTERACTION
		/// <summary>
		/// Generates a map with the given dimensions.
		/// </summary>
		public void GenerateMap(uint width, uint height)
		{
			// Initialize copies of tile instances for faster instantiation.
			TileInstance[] tileInstanceCopies =
			{
				new(TileTypes[0]), new(TileTypes[1]), new(TileTypes[2]), new(TileTypes[3]), new(TileTypes[4]), new(TileTypes[5]), new(TileTypes[6]), new(TileTypes[7]), new(TileTypes[8]),
			};

			// Initialize the tile array.
			TileInstances = new TileInstance[width, height];
			// Update the stored map size.
			MapWidth = width;
			MapHeight = height;

			// Do basic world gen.
			for (int row = 0; row < height; row++)
			{
				// TEMP/DEBUG: Compute depth just to make sure each tile works.
				uint depth = (uint)row / (height / 8);

				for (int column = 0; column < width; column++)
				{
					TileInstances[column, row].Set(tileInstanceCopies[depth]);
				}
			}

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
		
		// METHODS - UNITY
		private void Awake()
		{
			GenerateMap(10, 10);
		}
	}

}