using System;
using System.Collections;
using System.Collections.Generic;
using Resources;
using UnityEngine;
using UnityEngine.Serialization;

namespace Environment
{
	/// <summary>
	/// A container of a tile's fixed/characteristic information.
	/// </summary>
	[Serializable]
	public struct TileType
	{
		public TileType(ushort typeID, ushort textureID, byte durabilityMax, float durabilityHardness, bool isSolid)
		{
			this.typeID = typeID;
			this.textureID = textureID;
			this.durabilityMax = durabilityMax;
			this.durabilityHardness = durabilityHardness;
			this.isSolid = isSolid;
		}
		
		/// <summary>
		/// The unique ID of this tile type.
		/// </summary>
		public ushort typeID;
		
		/// <summary>
		/// The ID of the tile's texture.
		/// </summary>
		public ushort textureID;
		
		/// <summary>
		/// The maximum/initial tile durability (health).
		/// </summary>
		public byte durabilityMax;
		/// <summary>
		/// The hardness modifier of this tile's durability.
		/// </summary>
		public float durabilityHardness;

		/// <summary>
		/// Whether the tile can be stood on or not.
		/// </summary>
		public bool isSolid;
	}
	/// <summary>
	/// A granular instance of a block in the world; the type ID and state of a tile.
	/// </summary>
	public struct TileInstance
	{
		/// <summary>
		/// Creates a tile of the given type.
		/// Not optimized for repeated use!
		/// </summary>
		/// <param name="type">The type to initialize this type to.</param>
		public TileInstance(TileType type)
		{
			this.TypeID = type.typeID;
			this.DurabilityRemaining = type.durabilityMax;
			this.DurabilityHardness = type.durabilityHardness;
			this.IsSolid = type.isSolid;
			this.PositionX = 0;
			this.PositionY = 0;
		}
		/// <summary>
		/// Creates a tile instance using the given instance.
		/// </summary>
		/// <param name="instance">The instance to create from.</param>
		/// <param name="x">The x-coordinate to create this tile at.</param>
		/// <param name="y">The y-coordinate to create tis tile at.</param>
		public TileInstance(TileInstance instance, ushort x, ushort y)
		{
			this = instance;
			this.PositionX = x;
			this.PositionY = y;
		}
		
		/// <summary>
		/// The type-ID of this tile; used for looking up the immutable data of the tile.
		/// </summary>
		public ushort TypeID;
		
		/// <summary>
		/// The X-position (coordinate) of this tile. Relative to the map array, such that 0 is the left of the map.
		/// </summary>
		public ushort PositionX;
		/// <summary>
		/// The Y-position (coordinate) of this tile. Relative to the map array, such that 0 is the bottom of the map.
		/// </summary>
		public ushort PositionY;
		
		/// <summary>
		/// The remaining durability of this tile.
		/// </summary>
		public byte DurabilityRemaining;
		/// <summary>
		/// The damage reduction modifier of this tile.
		/// Final damage equals tool damage divided by the tile hardness minus the tool hardness.
		/// </summary>
		public float DurabilityHardness;
		/// <summary>
		/// Whether this tile is solid or not (can be passed through).
		/// </summary>
		public bool IsSolid;

		/// <summary>
		/// Sets this tile to be as the given one.
		/// </summary>
		/// <param name="newValue">The new tile to set this to.</param>
		public void Set(TileInstance newValue)
		{
			this = newValue;
		}
	}
	/// <summary>
	/// A GameObject representing a TileInstance in the Scene.
	/// Used as the interface for tile data to interact with the Scene.
	/// </summary>
	public class TileObject : MonoBehaviour
	{
		/// <summary>
		/// The renderer of this tile's primary graphic.
		/// </summary>
		public new SpriteRenderer renderer;
		/// <summary>
		/// The scene collider for this tile. 
		/// </summary>
		public new Collider2D collider;

		/// <summary>
		/// Configure this tile GameObject to the given settings.
		/// </summary>
		/// <param name="position">The position to move to, in world coordinates.</param>
		/// <param name="sprite">The sprite to set.</param>
		/// <param name="isSolid">Whether this tile should collide or not.</param>
		public void Configure(Vector2 position, Sprite sprite, bool isSolid)
		{
			// Cache the transform for tiny performance gains.
			Transform localTransform = transform;
			// Update the size/scale of the transform.
			localTransform.localPosition = localTransform.InverseTransformPoint(position);
			// Update the sprite.
			renderer.sprite = sprite;
			// Update the collider.
			collider.enabled = isSolid;
		}
	}
}

