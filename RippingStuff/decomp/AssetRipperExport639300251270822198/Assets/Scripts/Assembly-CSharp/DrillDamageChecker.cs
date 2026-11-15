using System;
using UnityEngine;

[AddComponentMenu("Enemies/Drill/DrillDamageChecker")]
public class DrillDamageChecker : MonoBehaviour
{
	[Serializable]
	public class DamageZone
	{
		public ObjectDamage m_DamageZone;

		internal bool m_Trigered;
	}

	private Drill m_Drill;

	public DamageZone[] m_DamageZones = new DamageZone[3];

	private void Awake()
	{
		m_Drill = GameObjectUtils.GetFirstComponentUpward<Drill>(base.gameObject);
		if (m_Drill == null)
		{
			Debug.LogWarning("Neccessary script missing - Drill");
		}
	}

	private void OnEnable()
	{
		InvokeRepeating("DamageZoneCheck", 0.5f, 0.5f);
	}

	private void OnDisable()
	{
		CancelInvoke();
	}

	public void Reset()
	{
		DamageZone[] damageZones = m_DamageZones;
		foreach (DamageZone damageZone in damageZones)
		{
			damageZone.m_Trigered = false;
		}
	}

	private void DamageZoneCheck()
	{
		Vector3 position = base.transform.position;
		DamageZone[] damageZones = m_DamageZones;
		foreach (DamageZone damageZone in damageZones)
		{
			if (!damageZone.m_Trigered && damageZone.m_DamageZone.IsEnabled() && damageZone.m_DamageZone.PointInsideObject(position))
			{
				damageZone.m_Trigered = true;
				m_Drill.OnObjectDamageHit_HACK(damageZone.m_DamageZone, m_DamageZones.Length);
			}
		}
	}
}
