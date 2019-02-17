using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile {

	private Vector2 cords;
	private string type;
	private bool created;

	public Tile(Vector2 c) {
		cords=c;
		type="grass";
		TileColor=new Color(Random.value,1,0,Random.value);
		created=true;
	}
	public Tile() {
		type="";
		created=false;
	}

	public Vector2 GetCords() {
		return cords;
	}

	public bool isCreated() {
		return created;
	}

	public string GetType() {
		return type;
	}

	public void SetWater() {
		type="water";
		TileColor=new Color(0,Random.value/3f+.2f,1,.6f);
	}

	public void SetCity() {
		type="city";
		TileColor=new Color(1,1,1,1);
	}

	public void SetCapital() {
		type="capital";
		TileColor=new Color(1,1,0,TileColor.a);
	}

	public Color TileColor {
		get; set;
	}

}
