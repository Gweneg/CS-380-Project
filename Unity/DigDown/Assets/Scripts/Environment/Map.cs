using System;
using System.Collections.Generic;
using System.Linq;
using GameResources;
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
		/// 
		public static readonly Dictionary<int, TileType> TileTypes =
			new()
		{
			{255, new TileType(typeID: 255, spriteID: 0, durabilityMax: 0, durabilityHardness: 1, isSolid: false)},	  // Air.
			{0, new TileType(typeID: 0, spriteID: 1, durabilityMax: 1, durabilityHardness: 1, isSolid: true)},        // Grass (to Dirt).
			{1, new TileType(typeID: 1, spriteID: 2, durabilityMax: 2, durabilityHardness: 255/20, isSolid: true)},   // Dirt.
			{2, new TileType(typeID: 2, spriteID: 3, durabilityMax: 4, durabilityHardness: 255/8, isSolid: true)},    // Dirt to Dirter.
			{3, new TileType(typeID: 3, spriteID: 4, durabilityMax: 6, durabilityHardness: 255/4, isSolid: true)},    // Dirter.
			{4, new TileType(typeID: 4, spriteID: 5, durabilityMax: 8, durabilityHardness: 255/8*3, isSolid: true)},  // Dirter to Dirtest.
			{5, new TileType(typeID: 5, spriteID: 6, durabilityMax: 12, durabilityHardness: 255/8*5, isSolid: true)}, // Dirtest.
			{6, new TileType(typeID: 6, spriteID: 7, durabilityMax: 14, durabilityHardness: 255/8*6, isSolid: true)}, // Dirtest to Stone.
			{7, new TileType(typeID: 7, spriteID: 8, durabilityMax: 20, durabilityHardness: 255, isSolid: true)},     // Stone.
			{8, new TileType(typeID: 8, spriteID: 9, durabilityMax: 255, durabilityHardness: 1, isSolid: true)},      // Void.
			{9, new TileType(typeID: 9, spriteID: 10, durabilityMax: 20, durabilityHardness: 255, isSolid: true)},	  // Iron
			{10, new TileType(typeID: 10, spriteID: 11, durabilityMax: 20, durabilityHardness: 255, isSolid: true)},  // Copper
			{11, new TileType(typeID: 11, spriteID: 12, durabilityMax: 30, durabilityHardness: 255, isSolid: true)},  // Silver
			{12, new TileType(typeID: 12, spriteID: 13, durabilityMax: 20, durabilityHardness: 255, isSolid: true)},  // Gold
			{13, new TileType(typeID: 13, spriteID: 14, durabilityMax: 80, durabilityHardness: 255, isSolid: true)}  // Diamond
			
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
		/// Note: The coordinates of the new corner position will be clamped to be within the true map dimensions.
		/// </summary>
		/// <param name="offset">Direction/amount to move the render window.</param>
		public void MoveRenderArea((long, long) offset)
		{
			// Unbox the offset X & Y.
			(long offsetX, long offsetY) = offset;
			// Cache the old lower-left coordinates of the render window.
			(long cornerX, long cornerY) = _renderCorner;
			// Cache the render dimensions
			(long renderWidth, long renderHeight) = _renderSize;
			
			// Clamp the offset to not over/underflow.
			uint newCoordinateX = (uint) Math.Clamp(offsetX + cornerX, 0, MapWidth - renderWidth);
			uint newCoordinateY = (uint) Math.Clamp(offsetY + cornerY, 0, MapHeight - renderHeight);
			
			// Set as normal.
			SetRenderArea((newCoordinateX, newCoordinateY));
		}
		
		/// <summary>
		/// Adjusts the tile GameObjects in-scene to reflect the underlying map.
		/// Note: The coordinates of the new corner position will be clamped to be within the true map dimensions.
		/// </summary>
		/// <param name="newBottomLeftCorner">The bottom-left coordinate to move the render window to.</param>
		public void SetRenderArea((uint, uint) newBottomLeftCorner)
		{
			// Note: Arrays are ordered [row, column], aka [y, x].
			// Note: Use of 'long' due to uints having huge range.
			
			// Cache the old lower-left coordinates of the render window.
			(long oldCornerX, long oldCornerY) = _renderCorner;
			// Compute the new lower-left coordinates of the render window.
			(long newCornerX, long newCornerY) = newBottomLeftCorner;
			// Compute the change in the (X,Y) coordinates.
			(long offsetX, long offsetY) = (newCornerX - oldCornerX, newCornerY - oldCornerY);
			// Cache the grid width and height of the render window.
			(long renderWidth, long renderHeight) = _renderSize;
			
			// Clamp the new coordinates to be within the map bounds.
			newCornerX = Math.Min(newCornerX, MapWidth - renderWidth);
			newCornerY = Math.Min(newCornerY, MapHeight - renderHeight);
			
			// Todo: Profile for-loop efficiency (swap inner/outer loops?).
			// Note: Row = Y = [rowIndex, 0]
			// Create an array with a write-index to store which rows of TileObjects need moving.
			HashSet<uint> markedRows = new();
			// Store the sign of the movement to allow movement either direction to be accounted for.
			long offsetYSign = Math.Sign(offsetY);
			// Compute which TileObject rows need to be moved, up to a maximum of all rows.
			switch (offsetYSign)
			{
				case -1:
					for (long rowIndex = 0; rowIndex < Math.Min(Math.Abs(offsetY), renderHeight); rowIndex++)
					{
						// Compute the index of the current row that safely wraps around the TileObject pool.
						markedRows.Add((uint) MathExtras.Operations.Modulo(value: oldCornerY - 1 - rowIndex,
						                                                   divisor: renderHeight));
					}
					break;
				case 1:
					for (long rowIndex = 0; rowIndex < Math.Min(Math.Abs(offsetY), renderHeight); rowIndex++)
					{
						// Compute the index of the current row that safely wraps around the TileObject pool.
						markedRows.Add((uint) MathExtras.Operations.Modulo(value: oldCornerY + rowIndex,
						                                                   divisor: renderHeight));
					}
					break;
			}
			
			
			// Note: Column = X = [0, rowIndex]
			// Create an array with a write-index to store which columns of TileObjects need moving.
			HashSet<uint> markedColumns = new();
			// Store the sign of the movement to allow movement either direction to be accounted for.
			long offsetXSign = Math.Sign(offsetX);
			// Compute which TileObject columns need to be moved, up to a maximum of all columns.
			// Each iteration: Modulo allows a clean way of wrapping around the list
			switch (offsetXSign)
			{
				case -1:
					for (long columnIndex = 0; columnIndex < Math.Min(Math.Abs(offsetX), renderWidth); columnIndex++)
					{
						markedColumns.Add((uint) MathExtras.Operations.Modulo(value: oldCornerX - 1 - columnIndex, 
						                                                      divisor: renderWidth));
					}
					break;
				case 1:
					for (long columnIndex = 0; columnIndex < Math.Min(Math.Abs(offsetX), renderWidth); columnIndex++)
					{
						markedColumns.Add((uint) MathExtras.Operations.Modulo(value: oldCornerX + columnIndex, 
						                                                      divisor: renderWidth));
					}
					break;
			}
			
			
			// Todo: Investigate other methods of iterating this (besides nested for loop).
			// Update each flagged TileObject by iterating over the entire 2D array and updating each one that's in either column/row.
			for (long renderRowIndex = 0; renderRowIndex < renderHeight; renderRowIndex++)
			{
				bool isWithinRow = markedRows.Contains((uint) renderRowIndex);
				
				for (long renderColumnIndex = 0; renderColumnIndex < renderWidth; renderColumnIndex++)
				{
					// Check if this tile type is within any of the
					bool isWithinColumn = markedColumns.Contains((uint) renderColumnIndex);

					// Do nothing if the tile does not need to be updated.
					if (!isWithinRow && !isWithinColumn)
					{
						continue;	
					}

					// Compute the "chunk" the current render tile should sit within.
					// Chunk is found as the positively-clamped, rounded-up value of: (corner - index) / size.
					// E.g. Corner at 1, Index of render tile at 2, size at 3, means we have pos-ceil((1-1)/3) = pos-ceil(-2/3) = pos(-1) = 0
					long chunkCoordinateX = (long) Math.Max(Math.Ceiling((newCornerX - renderColumnIndex) / (double) renderWidth),
					                                        0);
					long chunkCoordinateY = (long) Math.Max(Math.Ceiling((newCornerY - renderRowIndex) / (double) renderHeight),
					                                        0);
					
					// Compute the new coordinate to place the tile at.
					uint newTileXCoordinate = (uint) (chunkCoordinateX * renderWidth + renderColumnIndex);
					uint newTileYCoordinate = (uint) (chunkCoordinateY * renderHeight + renderRowIndex);
					
					// Store the tile information that will be applied.
					TileInstance tileInstance = InstanceTiles[newTileYCoordinate, 
					                                          newTileXCoordinate];
					TileType tileType = TileTypes[tileInstance.TypeID];
					Sprite tileSprite = ResourceManager.GetSprite(tileType.spriteID);
					bool isSolid = tileType.isSolid;
					
					// Compute the world-coordinates to place the tile at.
					Vector2 newTilePosition = GetWorldCoordinates((newTileXCoordinate, newTileYCoordinate));
					
					// Configure the tile object.
					TileObject tileObject = _renderTiles[renderRowIndex, renderColumnIndex];
					tileObject.Configure(newTilePosition, tileSprite, isSolid);
				}
			}

			_renderCorner = ((uint) newCornerX, (uint) newCornerY);
		}

		/// <summary>
		/// Re-renders all render tiles.
		/// </summary>
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

		//Changes a random tile from the map into a tile of another type
		//
		public void ChangeTile(ushort Id, int repetitions)
		{
			// Initialize bases of each tile instance type for faster instantiation. Linq for tidiness.
			Dictionary<ushort, TileInstance> tileInstanceCopies = 	
			TileTypes.Select(tileType => new TileInstance(tileType.Value)).ToDictionary(tileInstance => tileInstance.TypeID);

			//Get random coordinates from the Map
			int randomX = 0;
			int randomY = 0;
			for (int i = 0; i <= repetitions; i++)
			{
				randomX = UnityEngine.Random.Range(0, (int)MapWidth);
				randomY = UnityEngine.Random.Range(0, (int)MapHeight - (int)MapHeight / 4);
				switch (Id)
				{
					case 255: //air
						
						TileInstance airTile = tileInstanceCopies[Id];
						InstanceTiles[randomY, randomX].Set(airTile);
						break;
					case 9://iron

						TileInstance ironTile = tileInstanceCopies[Id]; 
						InstanceTiles[randomY, randomX].Set(ironTile);
						break;
					case 10://copper

						TileInstance copperTile = tileInstanceCopies[Id]; 
						InstanceTiles[randomY, randomX].Set(copperTile);
						break;
					case 11://Silver

						TileInstance silverTile = tileInstanceCopies[Id]; 
						InstanceTiles[randomY, randomX].Set(silverTile);
						break;
					case 12://Gold

						TileInstance goldTile = tileInstanceCopies[Id]; 
						InstanceTiles[randomY, randomX].Set(goldTile);
						break;
					case 13://Gold

						TileInstance diamondTile = tileInstanceCopies[Id]; 
						InstanceTiles[randomY, randomX].Set(diamondTile);
						break;
				}
			}
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
		/// 
		public void GenerateMap()
		{
			// Initialize bases of each tile instance type for faster instantiation. Linq for tidiness.
			Dictionary<ushort, TileInstance> tileInstanceCopies = TileTypes
			                                                     .Select(tileType => new TileInstance(tileType.Value))
			                                                     .ToDictionary(tileInstance => tileInstance.TypeID);
			// // 1. Get the type from the ID
			// TileType newTilesType = TileTypes[1];
			// // 2. Create the tile from the type
			// TileInstance newTileA = new TileInstance(newTilesType);

			// (1 + 2 Combined)
			TileInstance newTileB = tileInstanceCopies[1];

			// 3. Set the tile in the world
			InstanceTiles[0, 0].Set(newTileB);


			// Todo: Tidy this up
			// Loops over the entire world (starting at bottom left, sweeping all the way to the right, then moving up one block)
			for (int row = 0; row < MapHeight; row++)
			{
				// Choose tile type here
				long tileID = 3;

				for (int column = 0; column < MapWidth; column++)
				{
					// Sets the tile to the type chosen above
					InstanceTiles[row, column].Set(tileInstanceCopies[(ushort) tileID]);
				}
			}


			// Todo: Placeholder, replace?
			long mapLayerSize = MapHeight / 4;
			for (int row = 0; row < MapHeight; row++)
			{
				long tileID = 7 - row / mapLayerSize * 2;
				// If we're not the boundary of a layer, make it not a transition tile.
				if ((row + 1) % mapLayerSize == 0)
				{
					tileID -= 1;
				}

				for (int column = 0; column < MapWidth; column++)
				{
					InstanceTiles[row, column].Set(tileInstanceCopies[(ushort) tileID]);
				}
			}
				ChangeTile(255,80);
				ChangeTile(9,30);
				ChangeTile(10,20);
				ChangeTile(11,16);
				ChangeTile(12,10);
				ChangeTile(13,5);
			

			// Todo: Perform generation
			// Todo: Perform point-of-interest (caves, loot, etc.) generation
			// Todo: Perform world-gen validation
			// Todo: Perform navigation graph generation.
		}
		/// <summary>
		/// Configures the map and sets the width & height.
		/// </summary>
		/// <param name="width">The width (in tiles) of the map.</param>
		/// <param name="height">The height (in tiles) of the map.</param>
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
		/// <summary>
		/// Configures the render tiles objects of the map.
		/// </summary>
		/// <param name="width">The width (in tiles) of the render view.</param>
		/// <param name="height">The height (in tiles) of the render view.</param>
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
								                                 typeof(TileObject))
								{
									transform =
									{
										parent = MapHolder.transform,
										localScale = new Vector3(TileSize, TileSize, TileSize)
									}
								};
								newRenderTiles[rowIndex, columnIndex] = newInstanceTile.GetComponent<TileObject>();
								break;
						}
					}
				}
				
				// Clean up old tiles that will be out-of-range of the new.
				if (height < oldHeight)
				{
					// Destroy each GameObject in each unused row.
					for (int rowIndex = height; rowIndex < oldHeight; rowIndex++)
					{
						for (int columnIndex = 0; columnIndex < oldWidth; columnIndex++)
						{
							Destroy(oldRenderTiles[rowIndex, columnIndex]);
						}
					}
				}
				if (width < oldWidth)
				{
					// Destroy each GameObject in each unused row.
					for (int rowIndex = 0; rowIndex < oldHeight; rowIndex++)
					{
						for (int columnIndex = width; columnIndex < oldWidth; columnIndex++)
						{
							Destroy(oldRenderTiles[rowIndex, columnIndex]);
						}
					}
				}

				// Assign the new render tile array.
				_renderTiles = newRenderTiles;
			}
			
			// Update the render size.
			_renderSize = (width, height);
		}
		
		// METHODS - UNITY
		private void Start()
		{
			ConfigureMap(200, 16);
			ConfigureRenderTiles(25, 10);
			GenerateMap();
			MoveMap(Vector2.zero);
			RerenderObjectTiles();
			
			
			
			Debug.Log($"Map size: {MapWidth}, {MapHeight}");
		}

		private void Update()
		{
			
			// TODO: DEBUG/DEMONSTRATION CODE, REMOVE
			if (Input.GetKey(KeyCode.UpArrow))
			{
				MoveRenderArea((0, 1));
			}
			if (Input.GetKey(KeyCode.DownArrow))
			{
				MoveRenderArea((0, -1));
			}
			if (Input.GetKey(KeyCode.LeftArrow))
			{
				MoveRenderArea((-1, 0));
			}
			if (Input.GetKey(KeyCode.RightArrow))
			{
				MoveRenderArea((1, 0));
			}
		}
	}
}