using UnityEngine;
using System.Collections.Generic;
using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

public enum TileType
{
	Empty,
	Floor,
	Full}
;

public class Tile : IXmlSerializable
{
	private TileType type = TileType.Floor;

	public TileType Type {
		get { return type; }
		set {
			type = value;
		}
	}

	public Building building {
		get;
		protected set;
	}

	public Map map {
		get;
		protected set;
	}

	public int X {
		get;
		protected set;
	}

	public int Y {
		get;
		protected set;
	}

	public Tile (Map map, int x, int y)
	{
		this.map = map;
		this.X = x;
		this.Y = y;
	}

	public bool PlaceObject (Building obj)
	{
		if (obj.isValidPlacement (this) == false) {
			Debug.LogError ("Tile isn't valid. -Place Object-");
			return false;
		}

		for (int xOffset = X; xOffset < X + obj.Width; xOffset++) {

			for (int yOffset = Y; yOffset < Y + obj.Height; yOffset++) {
				Tile tile = map.GetTile (xOffset, yOffset);
				tile.building = obj;
				tile.Type = TileType.Full;
			}
			
		}
		this.building = obj;
		return true;	
	}

	public void DestroyObject (Tile t)
	{
		Tile masterTile = t.building.tile;
		int buildingWidth = t.building.Width;
		int	buildingHeight = t.building.Height;
		MapManager.Instance.map.buildings.Remove (t.building);

		for (int xOffset = masterTile.X; xOffset < masterTile.X + buildingWidth; xOffset++) {

			for (int yOffset = masterTile.Y; yOffset < masterTile.Y + buildingHeight; yOffset++) {
				Tile t1 = MapManager.Instance.map.GetTile (xOffset, yOffset);
				t1.building = null;
				t1.Type = TileType.Floor;
			}

		}

		GameObject destroyingObject = MapManager.Instance.spawnedBuildings [masterTile];
		MapManager.Instance.spawnedBuildings.Remove (masterTile);
		Debug.Log ("Destroyed: " + destroyingObject);
		GameObject.Destroy (destroyingObject);
	}

	#region SAVING AND LOADING

	public XmlSchema GetSchema ()
	{
		return null;
	}

	public void WriteXml (XmlWriter writer)
	{

		writer.WriteAttributeString ("Type", ((int)Type).ToString ());
		writer.WriteAttributeString ("X", X.ToString ());
		writer.WriteAttributeString ("Y", Y.ToString ());

	}

	public void ReadXml (XmlReader reader)
	{
		Type = (TileType)int.Parse (reader.GetAttribute ("Type"));
		// x and y already read by map class
	}

	#endregion

}
