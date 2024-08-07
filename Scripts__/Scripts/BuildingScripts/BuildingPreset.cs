﻿using UnityEngine;

[CreateAssetMenu(fileName = "Building Preset", menuName = "New Building Preset")]
public class BuildingPreset : ScriptableObject
{
	public string displayName;
	public int cost;
	public int costPerTurn;
	public GameObject prefab;

	public int population;
	public int jobs;
	public int food;

	public bool allowMultiple;
}