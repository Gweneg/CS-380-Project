using System;
using System.Collections;
using System.Collections.Generic;
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
		public TileType(ushort typeID, ushort spriteID, byte durabilityMax, byte durabilityHardness, bool isSolid)
		{
			this.typeID = typeID;
			this.spriteID = spriteID;
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
		public ushort spriteID;
		
		/// <summary>
		/// The maximum/initial tile durability (health).
		/// </summary>
		public byte durabilityMax;
		/// <summary>
		/// The hardness modifier of this tile's durability.
		/// </summary>
		public byte durabilityHardness;

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
			this.IsSolid = type.isSolid;
		}
		/// <summary>
		/// Creates a tile instance using the given instance.
		/// </summary>
		/// <param name="instance">The instance to create from.</param>
		public TileInstance(TileInstance instance)
		{
			this = instance;
		}
		
		/// <summary>
		/// The type-ID of this tile; used for looking up the immutable data of the tile.
		/// </summary>
		public ushort TypeID;
		
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
			localTransform.position = new Vector3(position.x, position.y, localTransform.position.z);
			// Update the sprite.
			renderer.sprite = sprite;
			// Update the collider.
			collider.enabled = isSolid;
		}

		private void Awake()
		{
			renderer = GetComponent<SpriteRenderer>();
			collider = GetComponent<Collider2D>();
		}
	}
}

