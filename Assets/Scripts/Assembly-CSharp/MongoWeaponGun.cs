using UnityEngine;

[AddComponentMenu("Enemies/Mongo/WeaponGun")]
public class MongoWeaponGun : MongoWeaponBase
{
	private PredictPosition m_TargetPosPredictor = new PredictPosition(3);

	private AgentHuman m_Target;

	public float m_PredictionCoef = 1f;

	private void Reset()
	{
		m_ProjectileType = E_ProjectileType.MongoSMG;
		m_ProjectileSettingsEx.Speed = 100f;
	}

	private void Start()
	{
		m_Target = Player.Instance.Owner;
	}

	public override void WeaponReset()
	{
		base.WeaponReset();
	}

	private void Update()
	{
		Vector3 chestPosition = m_Target.ChestPosition;
		if (m_PredictionCoef > 0f)
		{
			float magnitude = (m_Target.ChestPosition - m_Transform.position).magnitude;
			float inDeltaTime = magnitude / m_ProjectileSettingsEx.Speed;
			m_TargetPosPredictor.AddSample(chestPosition);
			chestPosition += m_PredictionCoef * m_TargetPosPredictor.EstimatedMove(inDeltaTime);
		}
		LookAtTarget(chestPosition);
	}

	internal void LookAtTarget(Vector3 inTargetPos)
	{
		m_Transform.LookAt(inTargetPos);
		Debug.DrawLine(m_Transform.position, inTargetPos, Color.blue);
	}
}
