using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AnimStateTeleport : AnimState
{
	private AgentActionTeleport Action;

	public AnimStateTeleport(Animation anims, AgentHuman owner)
		: base(anims, owner)
	{
	}

	public override void Release()
	{
		SetFinished(true);
	}

	public override void OnActivate(AgentAction action)
	{
		base.OnActivate(action);
	}

	public override void OnDeactivate()
	{
		Owner.BlackBoard.MotionType = E_MotionType.None;
		Owner.BlackBoard.MoveDir = Vector3.zero;
		Owner.BlackBoard.Speed = 0f;
		Action.SetSuccess();
		Action = null;
		base.OnDeactivate();
	}

	public override void Reset()
	{
		Action.SetSuccess();
		Action = null;
		base.Reset();
	}

	public override bool HandleNewAction(AgentAction action)
	{
		return false;
	}

	public override void Update()
	{
	}

	protected override void Initialize(AgentAction action)
	{
		base.Initialize(action);
		Action = action as AgentActionTeleport;
		Owner.BlackBoard.MotionType = E_MotionType.None;
		Owner.BlackBoard.MoveDir = Vector3.zero;
		Owner.BlackBoard.Speed = 0f;
		Owner.StartCoroutine(Teleport());
	}

	private IEnumerator Teleport()
	{
		Owner.Stop(true);
		Owner.BlackBoard.Invulnerable = true;
		Owner.BlackBoard.ReactOnHits = false;
		if ((bool)Owner.NavMeshAgent)
		{
			Owner.NavMeshAgent.enabled = false;
		}
		Owner.SensorsComponent.DeactivateAllSensors();
		string s = Owner.AnimSet.GetTeleportAnim(AnimSet.E_TeleportAnim.In);
		CrossFade(s, 0.2f, PlayMode.StopAll);
		yield return new WaitForSeconds(Animation[s].length - 2f);
		Owner.TeleportFadeOut();
		Owner.SoundPlay(Owner.TeleportSound);
		yield return new WaitForSeconds(0.7f);
		Owner.Teleport(Action.Destination - Vector3.up * 1000f, Action.Rotation);
		yield return new WaitForSeconds(Random.Range(1.1f, 3.5f));
		Owner.Teleport(Action.Destination, Action.Rotation);
		Owner.TeleportFadeIn();
		Owner.SoundPlay(Owner.TeleportSound);
		s = Owner.AnimSet.GetTeleportAnim(AnimSet.E_TeleportAnim.Out);
		CrossFade(s, 0f, PlayMode.StopAll);
		yield return new WaitForSeconds(1f);
		Owner.SensorsComponent.ActivateAllSensors();
		Owner.BlackBoard.Invulnerable = false;
		Owner.BlackBoard.ReactOnHits = true;
		Owner.Stop(false);
		if ((bool)Owner.NavMeshAgent)
		{
			Owner.NavMeshAgent.enabled = true;
		}
		Release();
	}

	private void SpawnTeleportFX()
	{
		Vector3 position = Owner.transform.position;
		float magnitude = (position - Camera.main.transform.position).magnitude;
		float t = Mathf.Min(magnitude / 30f, 1f);
		MFExplosionPostFX.S_WaveParams waveParams = default(MFExplosionPostFX.S_WaveParams);
		waveParams.m_Amplitude = 1f;
		waveParams.m_Duration = 1.5f;
		waveParams.m_Freq = 10f;
		waveParams.m_Speed = 1.3f;
		waveParams.m_Radius = Mathf.Lerp(0.025f, 0.2f, t);
		waveParams.m_Delay = 0f;
		CamExplosionFXMgr.Instance.SpawnExplosionWaveFX(position, waveParams);
	}
}
