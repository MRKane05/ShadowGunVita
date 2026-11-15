using System;
using UnityEngine;

[Serializable]
internal class ProjectileCacheEx : ResourceCache
{
	private E_ProjectileType m_ProjectileType;

	public ProjectileCacheEx(string inName, E_ProjectileType inProjectileType, int inInitialCacheSize)
		: base(inName, inInitialCacheSize)
	{
		m_ProjectileType = inProjectileType;
	}

	public new Projectile Get()
	{
		GameObject gameObject = base.Get();
		gameObject.transform.position = new Vector3(0f, 0f, 10000f);
		gameObject.SetActiveRecursively(true);
		Projectile component = gameObject.GetComponent<Projectile>();
		component.ProjectileType = m_ProjectileType;
		return component;
	}

	public void Return(Projectile projectile)
	{
		projectile.gameObject.SetActiveRecursively(false);
		Return(projectile.gameObject);
	}
}
