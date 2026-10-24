#define DEBUG
using UnityEngine;

[AddComponentMenu("Enemies/Mongo/WeaponGrenade")]
public class MongoWeaponGrenade : MongoWeaponBase
{
	private AgentHuman m_Target;

	private void Reset()
	{
		m_ProjectileType = E_ProjectileType.MongoGrenade;
		m_ProjectileSettingsEx.Speed = 20f;
	}

	private void Start()
	{
		m_Target = Player.Instance.Owner;
	}

	private void Update()
	{
	}

	public override void WeaponReset()
	{
		base.WeaponReset();
	}

	public override void Fire()
	{
		Vector3 chestPosition = m_Target.ChestPosition;
		if (m_ParentWeaponObject != null)
		{
			chestPosition += m_ParentWeaponObject.m_GrenadesSettings.groundVectorDispersion;
		}
		m_Transform.rotation = GetOptimalRotation(chestPosition - m_Transform.position, m_ProjectileSettingsEx.Speed, Physics.gravity.magnitude);
		base.Fire();
	}

	private float GetMinimalSpeed(Vector3 inDirToTarget, float inGravity)
	{
		Vector3 vector = inDirToTarget;
		vector.y = 0f;
		float magnitude = vector.magnitude;
		float y = inDirToTarget.y;
		DebugUtils.Assert(magnitude > y);
		return Mathf.Sqrt(inGravity * magnitude * magnitude / (magnitude - y));
	}

	private Quaternion GetOptimalRotation(Vector3 inDirToTarget, float inSpeed, float inGravity)
	{
		Vector3 forward = inDirToTarget;
		forward.y = 0f;
		float minimalSpeed = GetMinimalSpeed(inDirToTarget, inGravity);
		if (minimalSpeed > inSpeed)
		{
			inSpeed = minimalSpeed + 0.01f;
		}
		float num = inSpeed * inSpeed;
		float sqrMagnitude = forward.sqrMagnitude;
		float num2 = Mathf.Sqrt(sqrMagnitude);
		float y = inDirToTarget.y;
		float num3 = Mathf.Sqrt(num * num - inGravity * (inGravity * sqrMagnitude + 2f * y * num));
		float a = 57.29578f * Mathf.Atan((num + num3) / (inGravity * num2));
		float b = 57.29578f * Mathf.Atan((num - num3) / (inGravity * num2));
		float angle = Mathf.Max(a, b);
		return Quaternion.LookRotation(forward) * Quaternion.AngleAxis(angle, Vector3.left);
	}
}
