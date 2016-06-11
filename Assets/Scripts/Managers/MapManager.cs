using UnityEngine;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
	[SerializeField]
	[Range(2,50)]
	private int mapSize = 25;

	[SerializeField]
	private GameObject tilePrefab; //set from Inspector

	public static MapManager Instance {
		get;
		protected set;
	}

	public Map map {
		get;
		protected set;
	}

	static public bool loadMap;

	public Dictionary<Tile, GameObject> spawnedBuildings; 

	BuildManager buildManager;

	void Awake ()
	{
		if (Instance != null)
			Debug.LogError ("There is an instance of this already");
		Instance = this;

		CreateEmptyMap ();
		buildManager = GameObject.FindObjectOfType<BuildManager> ();
		spawnedBuildings = new Dictionary<Tile, GameObject> ();


		if (loadMap) {
			loadMap = false;
			LoadSession ();
		}
		CreateTiles (); //create tiles after loaded map instatiated

	}

	public Tile GetTileAtWorldCoordinate (Vector3 coord)
	{
		int x = Mathf.FloorToInt (coord.x);
		int y = Mathf.FloorToInt (coord.y);

		return map.GetTile (x, y);

	}

	public void OnBuildingCreated (Building obj)
	{

		if (obj != null) {
			Debug.Log ("OnBuildingCreated");

			GameObject buildingPre = Instantiate (Resources.Load (obj.objectName, typeof(GameObject)), new Vector3 (obj.tile.X, obj.tile.Y, 0), Quaternion.identity)as GameObject;
			buildingPre.name = obj.objectName + obj.tile.X + "x" + obj.tile.Y;
			buildingPre.transform.SetParent (buildManager.transform, true);
			spawnedBuildings.Add (obj.tile, buildingPre);
			buildManager.ResetBuildingMode ();
		}

	}

	void CreateEmptyMap ()
	{
		map = new Map (mapSize, mapSize); 
		map.RegisterBuildingCreated (OnBuildingCreated);
	}

	public void CreateTiles ()
	{
		for (int x = 0; x < map.Width; x++) {
			for (int y = 0; y < map.Height; y++) {

				Tile tileObj = map.GetTile (x, y);

				GameObject tilePre = Instantiate (tilePrefab, new Vector3 (tileObj.X, tileObj.Y, 0), Quaternion.identity)as GameObject;
				tilePre.name = "Tile" + x + "x" + y;
				tilePre.transform.SetParent (this.transform, true);

				//Reset Camera Position and Zoom Level according to new map size
				Camera.main.transform.position = new Vector3 (map.Width / 2, map.Height / 2, Camera.main.transform.position.z);
				Camera.main.orthographicSize = map.Width / 2;
			}
		}

	}

	#region SAVING AND LOADING

	public void LoadMapFromSave ()
	{ 	// Load World button was clicked
		// Load Scene with saved data
		loadMap = true;
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}

	public void SaveSession ()
	{
		XmlSerializer serializer = new XmlSerializer (typeof(Map));
		TextWriter writer = new StringWriter ();
		serializer.Serialize (writer, map);
		writer.Close ();

		Debug.Log (writer.ToString ()); //show saved xml on console

		PlayerPrefs.SetString ("SavedGame", writer.ToString ());
	}

	public void LoadSession ()
	{
		XmlSerializer serializer = new XmlSerializer (typeof(Map));
		TextReader reader = new StringReader (PlayerPrefs.GetString ("SavedGame"));
		map = (Map)serializer.Deserialize (reader);
		map.RegisterBuildingCreated (OnBuildingCreated);
		Debug.Log (map);
		reader.Close ();
	}

	public void OnApplicationQuit ()
	{
		SaveSession ();
	}

	#endregion

}
