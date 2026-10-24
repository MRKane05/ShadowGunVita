using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ProjectileHitDispatcher
{
    static readonly List<IProjectileHitReceiver> s_Receivers = new List<IProjectileHitReceiver>(8);

    public static void Send(Transform target, Projectile projectile)
    {
        target.GetComponents(s_Receivers);
        for (int i = 0; i < s_Receivers.Count; i++)
        {
            s_Receivers[i].OnProjectileHit(projectile);
        }
        s_Receivers.Clear();
    }
}