using System;
using System.Collections.Generic;
using System.Linq;
using GameResources;
using UnityEngine;
using Random = UnityEngine.Random;
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
		/// A dictionary keying ore types to their respective overlay sprite IDs for display on tiles.
		/// </summary>
		public static readonly Dictionary<TileInstance.OverlayType, ushort> OreSpriteIDs = new()
		{
			{TileInstance.OverlayType.None, 0},
			{TileInstance.OverlayType.Iron, 12},
			{TileInstance.OverlayType.Copper, 13},
			{TileInstance.OverlayType.Silver, 14},
			{TileInstance.OverlayType.Gold, 15},
			{TileInstance.OverlayType.Diamond, 16},
			{TileInstance.OverlayType.Azurite, 17},
			{TileInstance.OverlayType.CloudOne, 18},
			{TileInstance.OverlayType.CloudTwo, 19},
			{TileInstance.OverlayType.CloudThree, 20},
		};
		/// <summary>
		/// A dictionary keying background types to their respective underlay sprite IDs for display under tiles.
		/// </summary>
		public static readonly Dictionary<TileInstance.UnderlayType, ushort> BackgroundSpriteIDs = new()
		{
			{TileInstance.UnderlayType.Sky, 21},
			{TileInstance.UnderlayType.Dirt, 22},
			{TileInstance.UnderlayType.Dirter, 23},
			{TileInstance.UnderlayType.Dirtest, 24},
			{TileInstance.UnderlayType.Stone, 25},
			{TileInstance.UnderlayType.DeepStone, 26},
			{TileInstance.UnderlayType.Lava, 27}
		};
		/// <summary>
		/// The different types tiles could be.
		/// </summary>
		public static readonly Dictionary<int, TileType> TileTypes =
			new()
		{
			{0, new TileType(typeID: 0, spriteID: 0, durabilityMax: 0, durabilityHardness: 1, isSolid: false)},		  // Air.
			{1, new TileType(typeID: 1, spriteID: 1, durabilityMax: 1, durabilityHardness: 1, isSolid: true)},        // Grass (to Dirt).
			{2, new TileType(typeID: 2, spriteID: 2, durabilityMax: 2, durabilityHardness: 255/20, isSolid: true)},   // Dirt.
			{3, new TileType(typeID: 3, spriteID: 3, durabilityMax: 4, durabilityHardness: 255/8, isSolid: true)},    // Dirt to Dirter.
			{4, new TileType(typeID: 4, spriteID: 4, durabilityMax: 6, durabilityHardness: 255/4, isSolid: true)},    // Dirter.
			{5, new TileType(typeID: 5, spriteID: 5, durabilityMax: 8, durabilityHardness: 255/8*3, isSolid: true)},  // Dirter to Dirtest.
			{6, new TileType(typeID: 6, spriteID: 6, durabilityMax: 12, durabilityHardness: 255/8*5, isSolid: true)}, // Dirtest.
			{7, new TileType(typeID: 7, spriteID: 7, durabilityMax: 14, durabilityHardness: 255/8*6, isSolid: true)}, // Dirtest to Stone.
			{8, new TileType(typeID: 8, spriteID: 8, durabilityMax: 20, durabilityHardness: 255, isSolid: true)},     // Stone.
			{9, new TileType(typeID: 9, spriteID: 9, durabilityMax: 10, durabilityHardness: 255, isSolid: true)},  	  // Molten Rock
		    {10, new TileType(typeID: 10, spriteID: 10, durabilityMax: 2, durabilityHardness: 255, isSolid: true)},	  // Lava
			{11, new TileType(typeID: 11, spriteID: 11, durabilityMax: 255, durabilityHardness: 1, isSolid: true)},   // Void.
			{12, new TileType(typeID: 12, spriteID: 12, durabilityMax: 20, durabilityHardness: 255, isSolid: true)},  // Iron
			{13, new TileType(typeID: 13, spriteID: 13, durabilityMax: 20, durabilityHardness: 255, isSolid: true)},  // Copper
			{14, new TileType(typeID: 14, spriteID: 14, durabilityMax: 30, durabilityHardness: 255, isSolid: true)},  // Silver
			{15, new TileType(typeID: 15, spriteID: 15, durabilityMax: 20, durabilityHardness: 255, isSolid: true)},  // Gold
			{16, new TileType(typeID: 16, spriteID: 16, durabilityMax: 80, durabilityHardness: 255, isSolid: true)},  // Diamond
			{17, new TileType(typeID: 17, spriteID: 17, durabilityMax: 40, durabilityHardness: 255, isSolid: true)}   // Azurite
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


		// METHODS - VIEWPORT
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
			uint newCoordinateX = (uint)Math.Clamp(offsetX + cornerX, 0, MapWidth - renderWidth);
			uint newCoordinateY = (uint)Math.Clamp(offsetY + cornerY, 0, MapHeight - renderHeight);

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
						markedRows.Add((uint)MathExtras.Operations.Modulo(value: oldCornerY - 1 - rowIndex,
																		   divisor: renderHeight));
					}
					break;
				case 1:
					for (long rowIndex = 0; rowIndex < Math.Min(Math.Abs(offsetY), renderHeight); rowIndex++)
					{
						// Compute the index of the current row that safely wraps around the TileObject pool.
						markedRows.Add((uint)MathExtras.Operations.Modulo(value: oldCornerY + rowIndex,
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
						markedColumns.Add((uint)MathExtras.Operations.Modulo(value: oldCornerX - 1 - columnIndex,
																			  divisor: renderWidth));
					}
					break;
				case 1:
					for (long columnIndex = 0; columnIndex < Math.Min(Math.Abs(offsetX), renderWidth); columnIndex++)
					{
						markedColumns.Add((uint)MathExtras.Operations.Modulo(value: oldCornerX + columnIndex,
																			  divisor: renderWidth));
					}
					break;
			}


			// Todo: Investigate other methods of iterating this (besides nested for loop).
			// Update each flagged TileObject by iterating over the entire 2D array and updating each one that's in either column/row.
			for (long renderRowIndex = 0; renderRowIndex < renderHeight; renderRowIndex++)
			{
				bool isWithinRow = markedRows.Contains((uint)renderRowIndex);

				for (long renderColumnIndex = 0; renderColumnIndex < renderWidth; renderColumnIndex++)
				{
					// Check if this tile type is within any of the
					bool isWithinColumn = markedColumns.Contains((uint)renderColumnIndex);

					// Do nothing if the tile does not need to be updated.
					if (!isWithinRow && !isWithinColumn)
					{
						continue;
					}

					// Compute the "chunk" the current render tile should sit within.
					// Chunk is found as the positively-clamped, rounded-up value of: (corner - index) / size.
					// E.g. Corner at 1, Index of render tile at 2, size at 3, means we have pos-ceil((1-1)/3) = pos-ceil(-2/3) = pos(-1) = 0
					long chunkCoordinateX = (long)Math.Max(Math.Ceiling((newCornerX - renderColumnIndex) / (double)renderWidth),
															0);
					long chunkCoordinateY = (long)Math.Max(Math.Ceiling((newCornerY - renderRowIndex) / (double)renderHeight),
															0);

					// Compute the new coordinate to place the tile at.
					uint newTileXCoordinate = (uint)(chunkCoordinateX * renderWidth + renderColumnIndex);
					uint newTileYCoordinate = (uint)(chunkCoordinateY * renderHeight + renderRowIndex);

					// Store the tile information that will be applied.
					TileInstance tileInstance = InstanceTiles[newTileYCoordinate,
															  newTileXCoordinate];
					TileType tileType = TileTypes[tileInstance.TypeID];
					Sprite tileSprite = ResourceManager.GetSprite(tileType.spriteID);
					Sprite tileOverlaySprite = ResourceManager.GetSprite(OreSpriteIDs[tileInstance.Overlay]);
					Sprite tileUnderlaySprite = ResourceManager.GetSprite(BackgroundSpriteIDs[tileInstance.Underlay]);
					bool isSolid = tileType.isSolid;

					// Compute the world-coordinates to place the tile at.
					Vector2 newTilePosition = GetWorldCoordinates((newTileXCoordinate, newTileYCoordinate));

					// Configure the tile object.
					TileObject tileObject = _renderTiles[renderRowIndex, renderColumnIndex];
					tileObject.Configure(newTilePosition, 
					                     sprite: tileSprite, 
					                     overlaySprite: tileOverlaySprite, 
					                     underlaySprite: tileUnderlaySprite, 
					                     isSolid);
				}
			}

			_renderCorner = ((uint)newCornerX, (uint)newCornerY);
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
					Sprite tileOverlaySprite = ResourceManager.GetSprite(OreSpriteIDs[tileInstance.Overlay]);
					Sprite tileUnderlaySprite = ResourceManager.GetSprite(BackgroundSpriteIDs[tileInstance.Underlay]);
					bool isSolid = tileType.isSolid;

					(uint xCoordinate, uint yCoordinate) = _renderCorner;

					// Compute the world-coordinates to place the tile at.
					Vector2 newTilePosition = GetWorldCoordinates((xCoordinate + (uint)columnIndex, yCoordinate + (uint)rowIndex));

					// Configure the tile object.
					TileObject tileObject = _renderTiles[rowIndex, columnIndex];
					tileObject.Configure(newTilePosition, 
					                     sprite: tileSprite, 
					                     overlaySprite: tileOverlaySprite,
					                     underlaySprite: tileUnderlaySprite,
					                     isSolid);
				}
			}
		}

		
		// METHODS - MAP INFO
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
		/// <summary>
		/// Checks whether or not the tile at the specified coordinates is an Air tile.
		/// </summary>
		/// <param name="x">The X-coordinate of the tile to check.</param>
		/// <param name="y">The Y-coordinate of the tile to check.</param>
		/// <returns>True when the tile at the given coordinates is Air; False when it is not.</returns>
		public bool IsAirTileAtCoordinates(uint x, uint y)
		{
			// Check if the coordinates are within the map bounds
			if (x >= MapWidth || y >= MapHeight)
			{
				Debug.LogError("Coordinates are out of map bounds!");
				return false;
			}

			// Get the tile instance at the specified coordinates
			TileInstance tileInstance = InstanceTiles[y, x];

			// Check if the tile type ID corresponds to an air tile
			return tileInstance.TypeID == 0;
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

			// Each layer needs similar amount of rows excluding transition tiles (0,2,4,6)
			//Sample Output: 9 9 9 9, 8, 7, 7, 7, 7, 6, 5, 5, 5, 5, 4, 3, 3, 3, 3, 2, 1, 1, 1, 1, 0
			//Note: When adding new environment tiles add them in pairs to avoid changing this code a lot
			const uint mapLayerSize = 20; // The size (in rows) of each layer.
			const uint mapLayerCount = 6; // The number of layers.
			for (uint row = 0; row < MapHeight; row++)
			{
				//Note: Must subtract from odd number or will fill map with transitional tiles instead
				uint distanceFromSurface = MapHeight - row - 1;
				// Compute the current layer.
				uint layerCurrent = distanceFromSurface / mapLayerSize;
				// Clamp the layer to the number of layers.
				layerCurrent = Math.Min(layerCurrent, mapLayerCount - 1);
				
				// Get the tile ID for the current layer.
				uint tileID = layerCurrent * 2;
				
				// Check if we're on the transition to the next layer.
				bool isLayerBoundary = (distanceFromSurface + 1) % mapLayerSize == 0;
				// Change the tile to a layer-transition tile if we're on the edge of two layers.
				if (isLayerBoundary)
				{
					tileID += 1;
				}

				// Configure the tile preset for the row (a little more optimized than doing it for each tile).
				TileInstance modifiedTile = tileInstanceCopies[(ushort)tileID];
				modifiedTile.Underlay = (TileInstance.UnderlayType) layerCurrent;
				tileInstanceCopies[(ushort) tileID] = modifiedTile;
				
				// Set the tile for each column in the row.
				for (int column = 0; column < MapWidth; column++)
				{
					// Set the instance for the current column/tile.
					InstanceTiles[row, column].Set(tileInstanceCopies[(ushort)tileID]);
				}
			}

			// Generate a long cave randomly in the map.
			const uint caveLength = 15;
			const uint caveHeight = 4;
			const uint caveCount = 6;
			Vector4 cavePadding = new Vector4(0, 1, 0.75f, 0f);
			
			// Catch bad map/cave dimensions.
			if (caveCount != 0 && (caveLength > MapWidth || caveHeight > MapHeight))
			{
				Debug.LogError("Cave dimensions are too large for the map!");
				return;
			}
			
			
			for (int i = 0; i < caveCount; i++)
			{
				// Compute the start column for the cave.
				int startColumn = UnityEngine.Random.Range(minInclusive: (int) (MapWidth * cavePadding.x), 
				                                           maxExclusive: (int) (MapWidth * cavePadding.y));
				startColumn = Math.Clamp(startColumn, 0, (int) MapWidth - (int) caveLength);
				
				// Compute the start Row for the cave.
				int startRow = UnityEngine.Random.Range(minInclusive: (int) (MapHeight * cavePadding.z), 
				                                        maxExclusive: (int) (MapHeight * cavePadding.w));
				startRow = Math.Clamp(startRow, 0, (int) MapHeight - (int) caveHeight);
				
				// Clear the cave area.
				for (int row = 0; row < caveHeight; row++)
				{
					for (int column = 0; column < caveLength; column++)
					{
						// Store the tile's background type before changing it.
						TileInstance.UnderlayType underlayType = InstanceTiles[row + startRow, column + startColumn].Underlay;
						// Get the tile instance corresponding to the new tile ID.
						TileInstance airTile = tileInstanceCopies[0];
						// Set the tile instance at the current coordinates to the new tile.
						InstanceTiles[row + startRow, column + startColumn].Set(airTile);
						// Set the tile's background type back to what it was.
						SetTileUnderlay(x: (uint) (column + startColumn), y: (uint) (row + startRow), underlayType);
					}
				}
			}
			
			Vector4 commonOrePadding = new Vector4(0, 1, 0.75f, 0f);
			SetRandomTiles(0, 150, commonOrePadding); // Set random air.
			SetRandomTiles(12, 30, commonOrePadding); // Set random iron.
			SetRandomTiles(13, 20, commonOrePadding); // Set random copper.
			
			Vector4 uncommonOrePadding = new Vector4(0, 1, 0.50f, 0f);
			SetRandomTiles(14, 16, uncommonOrePadding); // Set random silver.
			SetRandomTiles(15, 10, uncommonOrePadding); // Set random gold.
			
			Vector4 rareOrePadding = new Vector4(0, 1, 0.25f, 0f);
			SetRandomTiles(16, 5, rareOrePadding); // Set random diamond.
			SetRandomTiles(17, 5, rareOrePadding); // Set random azurite.
			
			Vector4 skyPadding = new Vector4(0, 1, 1f, 0.95f);
			SetRandomTiles(200, 25, skyPadding); // Set random clouds. // Todo: Figure out something for cloud block's ID.

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
								// Create the tile.
								GameObject newInstanceTile = new GameObject(name: $"Tile ({columnIndex}, {rowIndex})",
																 typeof(BoxCollider2D),
																 typeof(SpriteRenderer))
								{
									transform =
									{
										parent = MapHolder.transform,
										localScale = new Vector3(TileSize, TileSize, TileSize)
									}
								};
								
								// Create the tile sprite overlay.
								GameObject newInstanceTileOverlay = new GameObject(name: "Tile Sprite Overlay")
								{
									transform =
									{
										parent = newInstanceTile.transform,
										localScale = new Vector3(1, 1, 1),
										localPosition = new Vector3(0, 0, 0),
									}
								};
								SpriteRenderer spriteOverlayRenderer = newInstanceTileOverlay.AddComponent<SpriteRenderer>();
								spriteOverlayRenderer.sortingOrder = 1;
								
								// Create the tile sprite underlay.
								GameObject newInstanceTileUnderlay = new GameObject(name: "Tile Sprite Underlay")
								{
									transform =
									{
										parent = newInstanceTile.transform,
										localScale = new Vector3(1, 1, 1),
										localPosition = new Vector3(0, 0, 0),
									}
								};
								SpriteRenderer spriteUnderlayRenderer = newInstanceTileUnderlay.AddComponent<SpriteRenderer>();
								spriteUnderlayRenderer.sortingOrder = -1;
								
								// Add the tile component to the GameObject. Note: Hacky workaround to making the Awake() not brick itself.
								newInstanceTile.AddComponent<TileObject>();
								
								// Store the created tile.
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
		/// <summary>
		/// Changes a number of random tiles in the map into the provided type.
		/// </summary>
		/// <param name="id">The ID to set a random tile to.</param>
		/// <param name="repetitions">The number of times to repeat the random change.</param>
		/// <param name="padding">The padding (area excluded, in percent of map width and height) for the left (X), right (Y), top (Z), and bottom (W) sides respectively.</param>
		public void SetRandomTiles(ushort id, int repetitions, Vector4 padding = default)
		{
			uint minX = 0;
			uint maxX = MapWidth - 1;
			uint minY = 0;
			uint maxY = MapHeight - 1;
			
			if (padding != default)
			{
				// Clamp the padding to be within the range of 0 to 1.
				padding.x = Mathf.Clamp(padding.x, 0, 1);
				padding.y = Mathf.Clamp(padding.y, 0, 1);
				padding.z = Mathf.Clamp(padding.z, 0, 1);
				padding.w = Mathf.Clamp(padding.w, 0, 1);
				// Clamp the padding to not cross itself.
				padding.x = Mathf.Min(padding.x, padding.y);
				padding.y = Mathf.Max(padding.x, padding.y);
				padding.z = Mathf.Max(padding.z, padding.w);
				padding.w = Mathf.Min(padding.z, padding.w);
				// Calculate the bounds of the random lookup.
				minX = (uint)(MapWidth * padding.x);
				maxX = (uint)((MapWidth - 1) * padding.y);
				minY = (uint)(MapHeight * padding.z);
				maxY = (uint)((MapHeight - 1) * padding.w);
			}
			
			
			// Initialize bases of each tile instance type for faster instantiation. Linq for tidiness.
			Dictionary<ushort, TileInstance> tileInstanceCopies =
			TileTypes.Select(tileType => new TileInstance(tileType.Value)).ToDictionary(tileInstance => tileInstance.TypeID);

			// Track the number of failed repetitions.
			uint failedRepetitions = 0;
			const uint maxFailedRepetitions = 100;
			
			// Add random ore & air pockets to the map.
			for (int i = 0; i <= repetitions; i++)
			{
				// Get random coordinates from the Map
				ushort randomX = (ushort) UnityEngine.Random.Range(minX, maxX);
				ushort randomY = (ushort) UnityEngine.Random.Range(minY, maxY);
				
				// Check if the random tile is air.
				bool isAirTile = IsAirTileAtCoordinates(randomX, randomY);
				
				// Perform the tile change based on the ID.
				switch (id)
				{
					case 0: // Air
						// Save the background at the tile's location.
						TileInstance.UnderlayType currentUnderlay = InstanceTiles[randomY, randomX].Underlay;
						// Get & configure the tile instance that will replace the current tile.
						TileInstance airTile = tileInstanceCopies[id];
						// Set the tile instance at the current coordinates to the new tile.
						InstanceTiles[randomY, randomX].Set(airTile);
						// Set the underlay to the previous underlay.
						SetTileUnderlay(randomX, randomY, currentUnderlay);
						break;
					
					case 12: // Iron
						if (isAirTile == false)
						{
							SetTileOverlay(x: randomX, y: randomY, TileInstance.OverlayType.Iron);
						}
						else
						{
							// Attempt another random tile.
							i--;
							// Track the number of failed repetitions.
							failedRepetitions += 1;
							if (failedRepetitions > maxFailedRepetitions)
							{
								Debug.LogError("Failed to set random tiles after 100 attempts.");
								return;
							}
						}
						break;
					
					case 13: // Copper
						if (isAirTile == false)
						{
							SetTileOverlay(x: randomX, y: randomY, TileInstance.OverlayType.Copper);
						}
						else
						{
							// Attempt another random tile.
							i--;
							// Track the number of failed repetitions.
							failedRepetitions += 1;
							if (failedRepetitions > maxFailedRepetitions)
							{
								Debug.LogError("Failed to set random tiles after 100 attempts.");
								return;
							}
						}
						break;
					
					case 14: // Silver
						if (isAirTile == false)
						{
							SetTileOverlay(x: randomX, y: randomY, TileInstance.OverlayType.Silver);
						}
						else
						{
							// Attempt another random tile.
							i--;
							// Track the number of failed repetitions.
							failedRepetitions += 1;
							if (failedRepetitions > maxFailedRepetitions)
							{
								Debug.LogError("Failed to set random tiles after 100 attempts.");
								return;
							}
						}
						break;
					
					case 15: // Gold
						if(isAirTile == false)
						{
							SetTileOverlay(x: randomX, y: randomY, TileInstance.OverlayType.Gold);
						}
						else
						{
							// Attempt another random tile.
							i--;
							// Track the number of failed repetitions.
							failedRepetitions += 1;
							if (failedRepetitions > maxFailedRepetitions)
							{
								Debug.LogError("Failed to set random tiles after 100 attempts.");
								return;
							}
						}
						break;
					
					case 16: // Diamond
						if (isAirTile == false)
						{
							SetTileOverlay(x: randomX, y: randomY, TileInstance.OverlayType.Diamond);
						}
						else
						{
							// Attempt another random tile.
							i--;
							// Track the number of failed repetitions.
							failedRepetitions += 1;
							if (failedRepetitions > maxFailedRepetitions)
							{
								Debug.LogError("Failed to set random tiles after 100 attempts.");
								return;
							}
						}
						break;
					
					case 17: // Azurite
						if (isAirTile == false)
						{
							SetTileOverlay(x: randomX, y: randomY, TileInstance.OverlayType.Azurite);
						}
						else
						{
							// Attempt another random tile.
							i--;
							// Track the number of failed repetitions.
							failedRepetitions += 1;
							if (failedRepetitions > maxFailedRepetitions)
							{
								Debug.LogError("Failed to set random tiles after 100 attempts.");
								return;
							}
						}
						break;
					
					case 200: // Debug: Temp cloud
						if (isAirTile)
						{
							TileInstance.OverlayType cloudType = Random.Range(0, 4) switch
							{
								0 => TileInstance.OverlayType.CloudOne,
								1 => TileInstance.OverlayType.CloudTwo,
								2 => TileInstance.OverlayType.CloudThree,
								_ => TileInstance.OverlayType.CloudThree
							};
							SetTileOverlay(x: randomX, y: randomY, cloudType);
						}
						else
						{
							// Attempt another random tile.
							i--;
							// Track the number of failed repetitions.
							failedRepetitions += 1;
							if (failedRepetitions > maxFailedRepetitions)
							{
								Debug.LogError("Failed to set random tiles after 100 attempts.");
								return;
							}
						}
						break;
				}
			}
		}
		/// <summary>
		/// Sets the overlay on the tile at the given coordinates.
		/// </summary>
		/// <param name="x">The X-coordinate of the tile.</param>
		/// <param name="y">The Y-coordinate of the tile.</param>
		/// <param name="overlayType">The overlay type to set on the tile.</param>
		public void SetTileOverlay(uint x, uint y, TileInstance.OverlayType overlayType)
		{
			// Get the previous tile.
			TileInstance tile = InstanceTiles[y, x];
			// Update the tile's overlay type.
			tile.Overlay = overlayType;
			// Re-save the tile to the map.
			InstanceTiles[y, x].Set(tile);
		}
		/// <summary>
		/// Sets the underlay (background) on the tile at the given coordinates.
		/// </summary>
		/// <param name="x">The X-coordinate of the tile.</param>
		/// <param name="y">The Y-coordinate of the tile.</param>
		/// <param name="underlayType">The underlay (background) type to set on the tile.</param>
		public void SetTileUnderlay(uint x, uint y, TileInstance.UnderlayType underlayType)
		{
			// Get the previous tile.
			TileInstance tile = InstanceTiles[y, x];
			// Update the tile's underlay type.
			tile.Underlay = underlayType;
			// Re-save the tile to the map.
			InstanceTiles[y, x].Set(tile);
		}

		
		// METHODS - UNITY
		private void Start()
		{
			ConfigureMap(30, 120);
			ConfigureRenderTiles(25, 12);
			GenerateMap();
			MoveMap(Vector2.zero);
			RerenderObjectTiles();

			Debug.Log($"Map size: {MapWidth}, {MapHeight}");
		}

		private static Camera _mainCamera;
		private static bool _isPanning;
		private static Vector2 _panPosition;
		private void Update()
		{
			// TODO: DEBUG/DEMONSTRATION CODE, REMOVE
			if (Input.GetKeyDown(KeyCode.Mouse0))
			{
				_mainCamera = Camera.main;
				_panPosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
				_isPanning = true;
			}
			else if (Input.GetKeyUp(KeyCode.Mouse0))
			{
				_isPanning = false;
			}
			if (_isPanning)
			{
				// Compute the camera's new position.
				Vector3 newPosition = _mainCamera.transform.position + (Vector3) (_panPosition - (Vector2) _mainCamera.ScreenToWorldPoint(Input.mousePosition));
				Vector3 cameraCornerOffset = new Vector3(_mainCamera.orthographicSize * _mainCamera.aspect, _mainCamera.orthographicSize);
				Vector3 newPositionBLCorner = newPosition - cameraCornerOffset;
				
				// Compute the bounds of the map.
				Vector2 bottomleftCorner = GetWorldCoordinates((0, 0));
				Vector2 toprightCorner = GetWorldCoordinates((MapWidth - 1, MapHeight - 1));
				
				// Clamp the new position to within the bounds of the map.
				Vector3 clampedPosition = new Vector3(x: Math.Clamp(newPositionBLCorner.x, bottomleftCorner.x, toprightCorner.x), 
				                                      y: Math.Clamp(newPositionBLCorner.y, bottomleftCorner.y, toprightCorner.y), 
				                                      z: newPositionBLCorner.z);
				_mainCamera.transform.position = clampedPosition + cameraCornerOffset;
				// Update the corner.
				SetRenderArea(GetTileCoordinates(clampedPosition));
			}
			
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