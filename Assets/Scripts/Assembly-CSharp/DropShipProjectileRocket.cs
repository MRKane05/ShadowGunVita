using UnityEngine;

[AddComponentMenu("Enemies/DropShip/ProjectileRocket")]
public class DropShipProjectileRocket : ProjectileRocket
{
	private Transform m_Parent;

	private bool m_Initialzied;

	private new void Awake()
	{
		base.Awake();
		base.GetComponent<Rigidbody>().detectCollisions = false;
		m_Parent = base.transform.parent;
	}

	public override void ProjectileInit(Vector3 pos, Vector3 dir, InitSettings inSettings)
	{
		if (m_Parent == null)
		{
			m_Parent = base.transform.parent;
		}
		Vector3 position = base.transform.position;
		Vector3 forward = base.transform.forward;
		base.transform.parent = null;
		base.ProjectileInit(position, forward, inSettings);
		if (base.GetComponent<AudioSource>() != null)
		{
			base.GetComponent<AudioSource>().Play();
		}
		foreach (Transform item in base.transform)
		{
			item.gameObject.SetActiveRecursively(true);
		}
		m_Initialzied = true;
	}

	public override void ProjectileDeinit()
	{
		if (!m_Initialzied)
		{
			return;
		}
		base.transform.parent = m_Parent;
		base.transform.localPosition = Vector3.zero;
		base.transform.localRotation = Quaternion.identity;
		if (base.GetComponent<AudioSource>() != null)
		{
			base.GetComponent<AudioSource>().Stop();
		}
		base.GetComponent<Rigidbody>().detectCollisions = false;
		base.gameObject.active = true;
		foreach (Transform item in base.transform)
		{
			item.gameObject.SetActiveRecursively(false);
		}
		m_Initialzied = false;
	}
}
