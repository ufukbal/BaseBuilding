using UnityEngine;
using System.Collections.Generic;
using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

public class Map : IXmlSerializable
{

	Tile[,] tiles;

	public Dictionary<string, Building> buildingPrototypes;
	//building prototypes for buttons

	public List<Building> buildings;
	//list of spawned building

	int width;

	public int Width {
		get {
			return width;
		}
		protected set{ width = value; }
	}

	int height;

	public int Height {
		get {
			return height;
		}
		protected set{ height = value; }
	}

	Action <Building> buildingCreated;

	//empty constructor for xml serialization.
	private Map ()
	{
	}

	public Map (int width, int height)
	{		
		SetUpMap (width, height);
	}

	void SetUpMap (int width, int height)
	{
		Width = width;
		Height = height;

		tiles = new Tile[width, height];

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				tiles [x, y] = new Tile (this, x, y);
			}
		}

		Debug.Log ("Map Created. Size:" + (width) + "x" + (height));

		buildings = new List<Building> ();
		CreateBuildingPrototypes ();

	}
		
	void CreateBuildingPrototypes ()
	{
		//TODO: read building types from json or xml?
		
		buildingPrototypes = new Dictionary<string, Building> ();

		//predefined common buildings
		buildingPrototypes.Add ("Flowers", new CommonBuilding ("Flowers"));
		buildingPrototypes.Add ("Library", new CommonBuilding ("Library"));
		buildingPrototypes.Add ("FirePlace", new CommonBuilding ("FirePlace"));
		buildingPrototypes.Add ("Baricade", new CommonBuilding ("Baricade"));
		buildingPrototypes.Add ("SmallFlag", new CommonBuilding ("SmallFlag"));

		buildingPrototypes.Add ("Bush", new CommonBuilding ("Bush"));
		buildingPrototypes.Add ("SmallTree", new CommonBuilding ("SmallTree"));
		buildingPrototypes.Add ("Rock", new CommonBuilding ("Rock"));

		//predefined unique buildings
		buildingPrototypes.Add ("Tent", new UniqueBuilding ("Tent", Color.yellow));
		buildingPrototypes.Add ("WhiteTent", new UniqueBuilding ("WhiteTent", Color.magenta));
		buildingPrototypes.Add ("Flag", new UniqueBuilding ("Flag", Color.red));
		buildingPrototypes.Add ("Poma", new UniqueBuilding ("Poma", Color.cyan));
		buildingPrototypes.Add ("Tree", new UniqueBuilding ("Tree", Color.green));
	}


	public Tile GetTile (int x, int y)
	{

		if (x >= width || x < 0 || y >= height || y < 0) {
			Debug.Log ("Out of Range");
			return null;
		}
		return tiles [x, y];
	}

	public void PlaceBuilding (string objectName, Tile t)
	{

		if (buildingPrototypes.ContainsKey (objectName) == false) {
			Debug.LogError ("Wrong object type: " + objectName);
			return;
		}

		Building building = buildingPrototypes [objectName];

		if (building is CommonBuilding)
			building = CommonBuilding.SpawnObject (buildingPrototypes [objectName] as CommonBuilding, t);
		else {
			building = UniqueBuilding.SpawnObject (buildingPrototypes [objectName] as UniqueBuilding, t);
		}
		
		Debug.Log ("Built object name: " + objectName);	

		if (buildingCreated != null) {
			buildingCreated (building);

		} else { //FIXME Event is null after loading. All info loaded except OnBuildingCreated Method 
			Debug.LogError ("BuildingCreatedEvent is Null");
			MapManager.Instance.OnBuildingCreated (building);
		}

	}

	public bool IsBuildingPlacementValid (string buildingType, Tile t)
	{
		Building obj = buildingPrototypes [buildingType];
		return buildingPrototypes [buildingType].isValidPlacement (t);
	}

	public void RegisterBuildingCreated (Action <Building> callback)
	{		
		buildingCreated = callback;
	}


	#region SAVING AND LOADING

	public XmlSchema GetSchema ()
	{ //unused method from interface 
		return null;
	}

	public void WriteXml (XmlWriter writer)
	{
		
		writer.WriteAttributeString ("Width", Width.ToString ());
		writer.WriteAttributeString ("Height", Height.ToString ());

		writer.WriteStartElement ("Buildings");
		foreach (var item in buildings) {
			writer.WriteStartElement ("Building");
			item.WriteXml (writer);
			writer.WriteEndElement ();
		}
		writer.WriteEndElement ();


		writer.WriteStartElement ("Tiles");
		for (int x = 0; x < Width; x++) {
			for (int y = 0; y < Height; y++) {
				
				writer.WriteStartElement ("Tile");
				tiles [x, y].WriteXml (writer);
				writer.WriteEndElement ();
			}
		}
		writer.WriteEndElement ();

	}

	public void ReadXml (XmlReader reader)
	{

		Width = int.Parse (reader.GetAttribute ("Width"));
		Height = int.Parse (reader.GetAttribute ("Height"));

		SetUpMap (Width, Height);

		while (reader.Read ()) {
			switch (reader.Name) {
			case "Buildings":
				ReadXml_Buildings (reader);
				break;
			case "Tiles":
				ReadXml_Tiles (reader);
				break;
			}
		}

	

	}

	void ReadXml_Tiles (XmlReader reader)
	{

		while (reader.Read ()) {
			if (reader.Name != "Tile")
				return;
			
			int x = int.Parse (reader.GetAttribute ("X"));
			int y = int.Parse (reader.GetAttribute ("Y"));
			tiles [x, y].ReadXml (reader);

		}


	}


	void ReadXml_Buildings (XmlReader reader)
	{

		while (reader.Read ()) {
			if (reader.Name != "Building")
				return;

			int x = int.Parse (reader.GetAttribute ("X"));
			int y = int.Parse (reader.GetAttribute ("Y"));
			PlaceBuilding (reader.GetAttribute ("objectName"), tiles [x, y]);

			buildings.Add (tiles [x, y].building); //add to read building to list that holds spawned buildings
		}


	}

	#endregion

}
