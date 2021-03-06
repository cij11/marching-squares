﻿using UnityEngine;
using System.Collections;

public enum OreTypes
{
	Empty, Rock, Iron, Mud, Desert, Snow, Grass
}

public class OreGrid {

	private int width;
	private int height;

	int numTerrainTypes = 4;

	private int randomFillPercent = 45;

	int[,] bitMap;	//Initially fill with cellular automata. 0 squares end up being flood filled with terrain types.
	int[,] terrainMap;



	public void GenerateMap(int width, int height) {
		this.width = width;
		this.height = height;
		bitMap = new int[width,height];
		RandomFillMap();

		for (int i = 0; i < 1; i ++) {
			SmoothMap();
		}

		terrainMap = new int[width, height];
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				terrainMap [x, y] = (int)OreTypes.Mud;
			}
		}
		//Now fill in the generated contiguous areas with random terrain types
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				int terrainCol = (int)UnityEngine.Random.Range (1, numTerrainTypes);
				if (bitMap [x, y] == 1)
					FloodFill (x, y, terrainCol);
			}
		}
	}

	private void FloodFill(int x, int y, int terrainCol){
		if (!(x < 0) && !(y < 0) && !(x > width - 1) && !(y > height - 1)) {
			if (bitMap [x, y] == 1) {
				bitMap [x, y] = 0;
				terrainMap [x, y] = terrainCol;
				FloodFill (x - 1, y, terrainCol);
				FloodFill (x + 1, y, terrainCol);
				FloodFill (x, y + 1, terrainCol);
				FloodFill (x, y - 1, terrainCol);
			}
		}
	}


	void RandomFillMap() {
		for (int x = 0; x < width; x ++) {
			for (int y = 0; y < height; y ++) {
				if (x == 0 || x == width-1 || y == 0 || y == height -1) {
					bitMap[x,y] = 1;
				}
				else {
					bitMap[x,y] = (Random.Range(0f,100f) < randomFillPercent)? 1: 0;
				}
			}
		}
	}

	void SmoothMap() {
		for (int x = 0; x < width; x ++) {
			for (int y = 0; y < height; y ++) {
				int neighbourWallTiles = GetSurroundingWallCount(x,y);

				if (neighbourWallTiles > 4)
					bitMap[x,y] = 1;
				else if (neighbourWallTiles < 4)
					bitMap[x,y] = 0;

			}
		}
	}

	int GetSurroundingWallCount(int gridX, int gridY) {
		int wallCount = 0;
		for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX ++) {
			for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY ++) {
				if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height) {
					if (neighbourX != gridX || neighbourY != gridY) {
						wallCount += bitMap[neighbourX,neighbourY];
					}
				}
				else {
					wallCount ++;
				}
			}
		}

		return wallCount;
	}

	public int GetTile(int x, int y){
		return terrainMap [x, y];
	}

	public int GetTileByIndex(int index, int x, int y){
		switch (index) {
		case 0:
			return terrainMap [x, y];
		case 2:
			return terrainMap [x, y+1];
		case 4:
			return terrainMap [x+1, y+1];
		case 6:
			return terrainMap [x+1, y];
		default:
			return 0;

		}
	}

	public Color32 GetOreColor(OreTypes ore){
		switch (ore) {
		case OreTypes.Empty:
			return new Color32 (0, 0, 0, 255);
		case OreTypes.Desert:
			return new Color32 (255, 0, 0, 255);
		case OreTypes.Grass:
			return new Color32 (0, 255, 0, 255);
		case OreTypes.Mud:
			return new Color32 (230, 173, 113, 255);
		case OreTypes.Rock:
			return new Color32 (100, 100, 100, 255);
		case OreTypes.Snow:
			return new Color32 (250, 250, 250, 255);
		case OreTypes.Iron:
			return new Color32 (200, 200, 200, 255);
		default:
			return new Color32 (0, 255, 255, 255);
		}
	}

	public Color32 GetOreColor(int ore)
	{
		return GetOreColor ((OreTypes)ore);
	}

	public Color32 GetColorOfTile(int x, int y){
		return GetOreColor (GetTile (x, y));
	}
}
