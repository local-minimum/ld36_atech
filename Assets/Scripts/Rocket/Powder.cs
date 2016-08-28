using UnityEngine;

[System.Serializable]
public class Powder : RocketComponent {

	public Material particleMaterial;
	public Color startColor;
	public Color endColor;
	public double burnTime;
	public string behaviour;

}
