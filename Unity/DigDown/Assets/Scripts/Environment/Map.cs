using System;
using System.Collections.Generic;
using System.Linq;
using Resources;
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
		public const float TileSize = 1f;
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
		/// The GameObject which parents all the map tiles.
		/// </summary>
		public static GameObject MapHolder;
		/// <summary>
		/// The different types tiles could be.
		/// </summary>
		public static readonly TileType[] TileTypes =
		{
			new(typeID: 0, spriteID: 0, durabilityMax: 1, durabilityHardness: 0.0f, isSolid: true),   // Grass (to Dirt).
			new(typeID: 1, spriteID: 1, durabilityMax: 2, durabilityHardness: 0.1f, isSolid: true),   // Dirt.
			new(typeID: 2, spriteID: 2, durabilityMax: 4, durabilityHardness: 0.25f, isSolid: true),  // Dirt to Dirter.
			new(typeID: 3, spriteID: 3, durabilityMax: 6, durabilityHardness: 0.5f, isSolid: true),   // Dirter.
			new(typeID: 4, spriteID: 4, durabilityMax: 8, durabilityHardness: 0.75f, isSolid: true),  // Dirter to Dirtest.
			new(typeID: 5, spriteID: 5, durabilityMax: 12, durabilityHardness: 1.25f, isSolid: true), // Dirtest.
			new(typeID: 6, spriteID: 6, durabilityMax: 14, durabilityHardness: 1.75f, isSolid: true), // Dirtest to Stone.
			new(typeID: 7, spriteID: 7, durabilityMax: 20, durabilityHardness: 2.0f, isSolid: true),  // Stone.
			new(typeID: 8, spriteID: 8, durabilityMax: 255, durabilityHardness: -1, isSolid: true)    // Void.
		};
		/// <summary>
		/// The instances of tiles for the current map.
		/// </summary>
		public TileInstance[,] InstanceTiles;
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
		/// The bottom-left coordinate of the window in which tiles are rendered to TileObjects.
		/// </summary>
		private (uint x, uint y) _renderCorner;
		/// <summary>
		/// The width and height (in tiles) of the window in which tiles are rendered to TileObjects.
		/// Ideally equal to the current viewport size divided by TileSize plus a couple tile padding.
		/// </summary>
		private (ushort width, ushort height) _renderSize;
		/// <summary>
		/// The array of GameObjects used to represent tileInstance's within the scene (around the player).
		/// </summary>
		private TileObject[,] _renderTiles;

		
		// FUNCTIONS - VIEWPORT

		
		/// <summary>
		/// Adjusts the tile GameObjects in-scene to reflect the underlying map.
		/// Pools ones that are no longer visible, allocates now-visible ones.
		/// </summary>
		/// <param name="newBottomLeftCorner">The bottom-left coordinate of the render window.</param>
		public void AdjustObjectTiles((uint, uint) newBottomLeftCorner)
		{
			// Note: Arrays are ordered [row, column], aka [y, x].
			// Note: Use of 'long' due to uints having huge range.
			
			// Cache the old lower-left coordinates of the render window.
			(long oldCoordinateX, long oldCoordinateY) = _renderCorner;
			// Compute the new lower-left coordinates of the render window.
			(long newXCoordinate, long newYCoordinate) = newBottomLeftCorner;
			// Compute the change in the (X,Y) coordinates.
			(long offsetX, long offsetY) = (newXCoordinate - oldCoordinateX, newYCoordinate - oldCoordinateY);
			// Cache the grid width and height of the render window.
			(long renderWidth, long renderHeight) = _renderSize;
			
			// Clamp the new coordinates to be within the map bounds.
			newXCoordinate = Math.Min(newXCoordinate, MapWidth - 1 - renderWidth);
			newYCoordinate = Math.Min(newYCoordinate, MapHeight - 1 - renderHeight);
			
			// Todo: Profile for-loop efficiency (swap inner/outer loops?).
			// Note: Row = Y = [rowIndex, 0]
			// Create an array with a write-index to store which rows of TileObjects need moving.
			HashSet<uint> markedRows = new();
			// Store the sign of the movement to allow movement either direction to be accounted for.
			long offsetYSign = Math.Sign(offsetY);
			// Compute which TileObject rows need to be moved, up to a maximum of all rows.
			for (long rowIndex = 0; rowIndex < Math.Min(Math.Abs(offsetY), renderHeight); rowIndex++)
			{
				// Compute the index of the current row that safely wraps around the TileObject pool.
				// Max clamps negatives to 0 to prevent uint overflows on cast.
				markedRows.Add((uint) Math.Max(0, MathExtras.Operations.Modulo(value: rowIndex * offsetYSign + oldCoordinateY, 
				                                                           divisor: renderHeight)));
			}
			
			// Note: Column = X = [0, rowIndex]
			// Create an array with a write-index to store which columns of TileObjects need moving.
			HashSet<uint> markedColumns = new();
			// Store the sign of the movement to allow movement either direction to be accounted for.
			long offsetXSign = Math.Sign(offsetX);
			// Compute which TileObject columns need to be moved, up to a maximum of all columns.
			for (long columnIndex = 0; columnIndex < Math.Min(Math.Abs(offsetX), renderWidth); columnIndex++)
			{
				// Compute the index of the current columns that safely wraps around the TileObject pool.
				// Max clamps negatives to 0 to prevent uint overflows on cast.
				markedColumns.Add((uint) Math.Max(0, MathExtras.Operations.Modulo(value: columnIndex * offsetXSign + oldCoordinateX, 
				                                                                       divisor: renderWidth)));
			}
			
			// Todo: Investigate other methods of iterating this (besides nested for loop).
			// Update each flagged TileObject by iterating over the entire 2D array and updating each one that's in either column/row.
			for (long rowIndex = 0; rowIndex < renderHeight; rowIndex++)
			{
				bool isWithinRow = markedRows.Contains((uint) rowIndex);
				
				for (long columnIndex = 0; columnIndex < renderWidth; columnIndex++)
				{
					// Check if this tile type is within any of the
					bool isWithinColumn = markedColumns.Contains((uint) rowIndex);

					// Do nothing if the tile does not need to be updated.
					if (!isWithinRow && !isWithinColumn)
					{
						continue;	
					}
					
					// Store the tile information that will be applied.
					TileInstance tileInstance = InstanceTiles[rowIndex, columnIndex];
					TileType tileType = TileTypes[tileInstance.TypeID];
					Sprite tileSprite = ResourceManager.GetSprite(tileType.spriteID);
					bool isSolid = tileType.isSolid;
					
					// Compute the new coordinate to place the tile at.
					uint newTileXCoordinate = (uint) Math.Ceiling((double)(newXCoordinate - columnIndex) / renderWidth);
					uint newTileYCoordinate = (uint) Math.Ceiling((double)(newYCoordinate - rowIndex) / renderHeight); 
					
					// Compute the world-coordinates to place the tile at.
					Vector2 newTilePosition = GetWorldCoordinates((newTileXCoordinate, newTileYCoordinate));
					
					// Configure the tile object.
					TileObject tileObject = _renderTiles[rowIndex, columnIndex];
					tileObject.Configure(newTilePosition, tileSprite, isSolid);
				}
			}
		}

		public void RerenderObjectTiles()
		{
			// Cache the grid width and height of the render window.
			(long renderWidth, long renderHeight) = _renderSize;
			
			// Update each tile.
			for (long rowIndex = 0; rowIndex < renderHeight; rowIndex++)
			{
				for (long columnIndex = 0; columnIndex < renderWidth; columnIndex++)
				{
					// Store the tile information that will be applied.
					TileInstance tileInstance = InstanceTiles[rowIndex, columnIndex];
					TileType tileType = TileTypes[tileInstance.TypeID];
					Sprite tileSprite = ResourceManager.GetSprite(tileType.spriteID);
					bool isSolid = tileType.isSolid;

					(uint xCoordinate, uint yCoordinate) = _renderCorner;
					
					// Compute the world-coordinates to place the tile at.
					Vector2 newTilePosition = GetWorldCoordinates((xCoordinate + (uint) columnIndex, yCoordinate + (uint) rowIndex));
					
					// Configure the tile object.
					TileObject tileObject = _renderTiles[rowIndex, columnIndex];
					tileObject.Configure(newTilePosition, tileSprite, isSolid);
				}
			}
		}

		// FUNCTIONS - MAP INFO
		/// <summary>
		/// Gets the grid-coordinates of the tile at the given position.
		/// </summary>
		/// <param name="position">The world-space position to get the tile at.</param>
		/// <returns>The (X,Y) indices/grid-coordinates of the tile at the given position.</returns>
		public static (uint xCoordinate, uint yCoordinate) GetTileCoordinates(Vector2 position)
		{
			// Compute the coordinate by getting the relative position from the bottom-left corner of the map, then converting into tiles (instead of distance). 
			Vector2 offset = (position - _mapOffset) / TileSize;
			// Round and clamp the coordinates to make them safe for use in indexing the map array.
			// Note: This means out-of-range coordinates will be clamped to the edge of the map. Add a warn?
			uint xCoordinate = (uint)Math.Clamp(offset.x, 0, MapWidth - 1);
			uint yCoordinate = (uint)Math.Clamp(offset.y, 0, MapHeight - 1);
			// Return the coordinates as a tuple.
			return (xCoordinate, yCoordinate);
		}
		
		/// <summary>
		/// Gets the world-space position of the tile at the given grid-coordinates.
		/// </summary>
		/// <param name="coordinates">The (X,Y) grid-coordinates of the tile.</param>
		/// <returns>The world-space position of hte tile at the given coordinates.</returns>
		public static Vector2 GetWorldCoordinates((uint, uint) coordinates)
		{
			// Unbox the x and y components.
			(uint xCoordinate, uint yCoordinate) = coordinates;
			return _mapOffset + new Vector2(xCoordinate, yCoordinate) * TileSize;
		}

		// METHODS - SCENE INTERACTION
		/// <summary>
		/// Positions the map such that the middle of the bottom of it lies on the given point.
		/// </summary>
		/// <param name="position">The position (in world-space) to position the bottom-center of the map over.</param>
		public static void MoveMap(Vector2 position)
		{
			// Cache the map's transform.
			Transform mapTransform = MapHolder.transform;
			// Set the map's position.
			mapTransform.position = position + Vector2.left * TileSize;
			// Update the offset.
			_mapOffset = mapTransform.position;
		}
		
		// METHODS - MAP INTERACTION
		/// <summary>
		/// Generates a map with the given dimensions.
		/// </summary>
		public void GenerateMap()
		{
			// Initialize bases of each tile instance type for faster instantiation. Linq for tidiness.
			TileInstance[] tileInstanceCopies = TileTypes.Select(tileType => new TileInstance(tileType)).ToArray();

			// Do basic world gen.
			for (int row = 0; row < MapHeight; row++)
			{
				// TEMP/DEBUG: Compute depth just to make sure each tile works.
				uint depth = (uint) row;

				for (int column = 0; column < MapWidth; column++)
				{
					InstanceTiles[row, column].Set(tileInstanceCopies[7 - depth]);
				}
			}

			// Todo: Perform generation
			// Todo: Perform point-of-interest (caves, loot, etc.) generation
			// Todo: Perform world-gen validation
			// Todo: Perform navigation graph generation.
		}
		public void ConfigureMap(uint width, uint height)
		{
			// Check if we need to create the MapHolder.
			if (MapHolder == null)
			{
				MapHolder = new GameObject("MapHolder");
			}
			// Initialize the tile array.
			InstanceTiles = new TileInstance[height, width];
			// Update the stored map size.
			MapWidth = width;
			MapHeight = height;
		}
		public void ConfigureRenderTiles(ushort width, ushort height)
		{
			// Cache the old render size.
			(ushort oldWidth, ushort oldHeight) = _renderSize;

			// Remake the array if it's a different size than before.
			if (width != oldWidth || height != oldHeight)
			{
				TileObject[,] oldRenderTiles = _renderTiles;
				TileObject[,] newRenderTiles = new TileObject[height, width];

				for (int rowIndex = 0; rowIndex < height; rowIndex++)
				{
					for (int columnIndex = 0; columnIndex < width; columnIndex++)
					{
						bool isWithinOldWidth = columnIndex < oldWidth;
						bool isWithinOldHeight = rowIndex < oldHeight;

						switch (isWithinOldWidth)
						{
							// Try to copy data from the old array.
							case true when isWithinOldHeight: // Copy successful.
								newRenderTiles[rowIndex, columnIndex] = oldRenderTiles[rowIndex, columnIndex];
								break;
							case false when !isWithinOldHeight: // Copy unsuccessful- must create new!
								GameObject newInstanceTile = new($"Tile ({columnIndex}, {rowIndex})", 
								                                 typeof(BoxCollider2D),
								                                 typeof(SpriteRenderer),
								                                 typeof(TileObject));
								newInstanceTile.transform.parent = MapHolder.transform;
								newRenderTiles[rowIndex, columnIndex] = newInstanceTile.GetComponent<TileObject>();
								break;
						}
					}
				}
				
				// Check if we need to destroy any truncated render tiles.
				if (width > oldWidth || height > oldHeight)
				{
					// Todo: Iterate over and destroy each tile outside the new range.
				}

				// Assign the new render tile array.
				_renderTiles = newRenderTiles;
			}
			
			// Update the render size.
			_renderSize = (width, height);
		}
		
		// METHODS - UNITY
		private void Awake()
		{
			ConfigureMap(10, 8);
			ConfigureRenderTiles(10, 8);
			GenerateMap();
			MoveMap(Vector2.zero);
			RerenderObjectTiles();
			
			
			Debug.Log($"Map size: {MapWidth}, {MapHeight}");
		}

		private void Update()
		{
			
			// TODO: DEBUG/DEMONSTRATION CODE, REMOVE
			(uint oldXCoordinate, uint oldYCoordinate) = _renderCorner;
			if (Input.GetKeyDown(KeyCode.UpArrow))
			{
				AdjustObjectTiles((oldXCoordinate, Math.Clamp(oldYCoordinate + 1, 0, 100)));
			}
			else if (Input.GetKeyDown(KeyCode.DownArrow))
			{
				AdjustObjectTiles((oldXCoordinate, Math.Clamp(oldYCoordinate - 1, 0, 100)));
			}
			else if (Input.GetKeyDown(KeyCode.LeftArrow))
			{
				AdjustObjectTiles((Math.Clamp(oldXCoordinate - 1, 0, 1000), oldYCoordinate));
			}
			else if (Input.GetKeyDown(KeyCode.RightArrow))
			{
				AdjustObjectTiles((Math.Clamp(oldXCoordinate + 1, 0, 1000), oldYCoordinate));
			}
		}
	}
}