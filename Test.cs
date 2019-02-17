using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

	public float spacing = .88f;
	public int rows=100,columns=100;
	public float weight = .2f;
	public int numCities=50;
	public int numCivs=10;
	public float offset = Mathf.Sqrt(3)/2;
	public int waterAmt = 10;
	public int messyEdge = 3;
	Tile[,] tiles;
	int[,,] directions = {
		{{ 1, 0},{ 0,-1},{-1,-1},
		 {-1, 0},{-1, 1},{ 0, 1}},
		{{ 1, 0},{ 1,-1},{ 0,-1},
		 {-1, 0},{ 0, 1},{ 1, 1}}
	};

	void Start () {
		GenerateMap();
	}
	
	void Update () {
		
	}

	void GenerateMap() {
		tiles = new Tile[rows,columns];
		for(int i=0;i<columns;i++) {
			for(int j=0;j<rows;j++) {
				tiles[i,j]=new Tile();
			}
		}

		// Generate Tiles
		for(int i=0;i<columns;i++) {
			for(int j=0;j<rows;j++) {
				tiles[i,j] = new Tile(new Vector2(i,j));
				float r=weight*tiles[i,j].TileColor.r,a=weight*tiles[i,j].TileColor.a;
				float c=1*weight;
				for(int k=0;k<6;k++) {
					int x = i + directions[j%2,k,0];
					int y = j + directions[j%2,k,1];
					if(i>0 && j>0 && i<columns-1 && j<rows-1) { 
						if(tiles[x,y].isCreated()) {
							r+=tiles[x,y].TileColor.r;
							a+=tiles[x,y].TileColor.a;
							c++;
						}
					}
				}
				r/=c;
				a/=c;
				tiles[i,j].TileColor=new Color(r,1,0,a);
			}
		}
		
		// Generate Water 
		for(int k=0;k<waterAmt;k++) {
			int rand = Random.Range(0,4);
			int i=Random.Range(0,columns);
			int j=Random.Range(0,rows);
			int length=0;
			while(length<5 || (i!=0 && i!=columns-1 && j!=0 && j!=rows-1)) {
				tiles[i,j].SetWater();
				int a=i,b=j;
				do {
					i=a+directions[b%2,Random.Range(0,6),0];
					j=b+directions[b%2,Random.Range(0,6),1];
				} while(i<0 || i>columns-1 || j<0 || j>rows-1); 
				length++;
			}
		}

		// Fill Outside with water
		for(int i=0;i<columns;i++) {
			for(int j=0;j<rows;j++) {
				if(Vector3.Distance(new Vector3(spacing*(i+j%2*.5f-columns/2),spacing*(j*offset-offset*rows/2),10), new Vector3(0,0,10)) > spacing*(rows+columns)/5f-Random.value*messyEdge) {
					tiles[i,j].SetWater();
				}
			}
		}

		// Change 1-tile islands to water
		for(int i=0;i<columns;i++) {
			for(int j=0;j<rows;j++) {
				bool hasGrassNeighbor=false;
				if(tiles[i,j].GetType()=="grass") {
					for(int k=0;k<6;k++) {
						int x = i + directions[j%2,k,0];
						int y = j + directions[j%2,k,1];
						if(tiles[x,y].GetType()=="grass") {
							hasGrassNeighbor=true;
						}
					}
					if(!hasGrassNeighbor) {
						tiles[i,j].SetWater();
					}
				}
			}
		}

		//Generate Cities
		int cityCount=0;
		int overload=0;
		while(cityCount<numCities || overload>10000) {
			for(int i=0;i<columns;i++) {
				for(int j=0;j<rows;j++) {
					if(tiles[i,j].GetType()=="grass") {
						bool eligible=true;
						for(int k=0;k<6;k++) {
							int x = i + directions[j%2,k,0];
							int y = j + directions[j%2,k,1];
							if(tiles[x,y].GetType()!="grass") {
								eligible=false;
							}
						}
						if(eligible) {
							float rand = Random.value;
							if(rand>.99f) {
								if(cityCount<numCities) {
									tiles[i,j].SetCity();
									cityCount++;
								}
							}
						}
					}
				}
			}
			overload++;
		}

		//Generate Civs
		int civCount=0;
		overload=0;
		while(civCount<numCivs || overload>10000) {
			for(int i=0;i<columns;i++) {
				for(int j=0;j<rows;j++) {
					if(tiles[i,j].GetType()=="city") {
						float rand = Random.value;
						if(rand>.9 && civCount<numCivs) {
							tiles[i,j].SetCapital();
							Color civColor = new Color(Random.value,Random.value,Random.value);
							for(int k=0;k<6;k++) {
								int x = i + directions[j%2,k,0];
								int y = j + directions[j%2,k,1];
								if(tiles[x,y].GetType()=="grass") {
									tiles[x,y].TileColor=civColor;
								}
							}
							civCount++;
						}
					}
				}
			}
			overload++;
		}

		for(int i=0;i<columns;i++) {
			for(int j=0;j<rows;j++) {
				GameObject tileGO = Instantiate(Resources.Load("Hexagon"),new Vector3(spacing*(i+j%2*.5f-columns/2),spacing*(j*offset-offset*rows/2),10),transform.rotation) as GameObject;
				if(tiles[i,j].GetType()=="water") {
					float g=weight*tiles[i,j].TileColor.g;
					float c=1*weight;
					for(int k=0;k<6;k++) {
						int x = i + directions[j%2,k,0];
						int y = j + directions[j%2,k,1];
						if(i>0 && j>0 && i<columns-1 && j<rows-1) { 
							if(tiles[x,y].isCreated() && tiles[x,y].GetType()=="water") {
								g+=tiles[x,y].TileColor.g;
								c++;
							}
						}
					}
					g/=c;
					tiles[i,j].TileColor=new Color(0,g,1,.6f);
				}
				tileGO.GetComponent<SpriteRenderer>().color = tiles[i,j].TileColor;
			}
		}

	}

}
