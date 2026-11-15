using UnityEngine;

public class CameraTargetAttributes : MonoBehaviour
{
	public enum E_HeightType
	{
		E_CALM = 0,
		E_ENEMIES_ARE_FAR = 1,
		E_ENEMIES_ARE_NEAR = 2
	}

	public float[] HeightOffset = new float[3] { 11f, 9f, 7f };

	public E_HeightType HeightType;

	public float DistanceModifier = 1f;

	public float VelocityLookAhead = 0.15f;

	public Vector2 MaxLookAhead = new Vector2(3f, 3f);

	public float HieghtSpeed = 1f;

	public float CurrentHeightOffset;

	private void Awake()
	{
	}

	private void Start()
	{
		CurrentHeightOffset = HeightOffset[0];
	}

	private void Update()
	{
		float num = HeightOffset[(int)HeightType];
		if (CurrentHeightOffset > num)
		{
			CurrentHeightOffset -= HieghtSpeed * Time.deltaTime;
			if (CurrentHeightOffset < num)
			{
				CurrentHeightOffset = num;
			}
		}
		else if (CurrentHeightOffset < num)
		{
			CurrentHeightOffset += HieghtSpeed * Time.deltaTime;
			if (CurrentHeightOffset > num)
			{
				CurrentHeightOffset = num;
			}
		}
	}

	private void ReSpawn(Transform spawnTransform)
	{
		HeightType = E_HeightType.E_CALM;
		CurrentHeightOffset = HeightOffset[0];
	}
}
