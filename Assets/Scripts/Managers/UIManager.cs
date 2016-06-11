using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

 public class UIManager : MonoBehaviour {
	public static UIManager Instance;

	public GameObject popUpMenu;

	void Start () {
	
		if (Instance != null)
			return;
		Instance = this;

	}
	
	public void ShowBuildingInfo(Building building){
		building.ShowInfo ();
	}

	public void OpenMenu( string name, Color color){
		popUpMenu.GetComponentInChildren<Text> ().text = name;
		popUpMenu.GetComponent<Image> ().color = color;
		popUpMenu.SetActive (true);
	}

	public void CloseMenu(GameObject menu){
		menu.SetActive (false);
	}
}
