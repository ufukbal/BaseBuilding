using UnityEngine;
using System.Collections;
using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

public abstract class Building
{

	public Tile tile {
		get;
		protected set;
	}

	public Color color {
		get;
		protected set;
	}

	public string objectName {
		get;
		protected set;
	}

	public int Width { 
		get; 
		protected set;
	}

	public int Height { 
		get; 
		protected set;
	}

	protected Func<Tile, bool> placementValidation;

	public abstract bool isValidPlacement (Tile t);

	public abstract void ShowInfo ();


	#region SAVING AND LOADING

	public Building(){
	}
		
	public XmlSchema GetSchema(){
		return null;
	}

	public void WriteXml(XmlWriter writer){

		writer.WriteAttributeString ("X", tile.X.ToString());
		writer.WriteAttributeString ("Y", tile.Y.ToString());
		writer.WriteAttributeString ("objectName", objectName.ToString());


	}
	public void ReadXml(XmlReader reader){
		//Already read by Map class
	}

	#endregion
}
