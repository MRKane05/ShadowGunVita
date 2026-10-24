using System.Collections;
using UnityEngine;

[AddComponentMenu("Enemies/Drill/Drill")]
public class Drill : MonoBehaviour, IGameZoneControledObject, IBossHealth
{
	private Animation m_Animations;

	private AnimationState m_Moving;

	private AnimationState m_Shaking;

	public bool m_Testing;

	public float m_ShakingSpeedPct = 0.1f;

	public float m_StartIgnoreDalay = 5f;

	public float m_MinPlayerDistance = 20f;

	public float m_MaxPlayerDistance = 35f;

	public float m_MaxAnimSpeedScale = 2f;

	private AgentHuman m_Target;

	private Transform m_CabineTransform;

	private float m_ActivationTime;

	public AnimationClip m_RunAnim;

	public AnimationClip m_ShakingAnim;

	public AudioSource m_Audio;

	public AudioClip m_ShakingSound;

	public AudioClip m_DestroySound;

	public float m_HitPoints = 1000f;

	private float m_DefaultHitPoints;

	private DrillDamageChecker m_DrillDamageChecker;

	public GameEvent m_DeathEvent;

	public float HealthPercent
	{
		get
		{
			return m_HitPoints / m_DefaultHitPoints;
		}
	}

	private void Awake()
	{
		m_DefaultHitPoints = m_HitPoints;
		m_Animations = base.GetComponent<Animation>();
		m_Moving = m_Animations[(!(m_RunAnim != null)) ? "RUN" : m_RunAnim.name];
		m_Shaking = m_Animations[(!(m_ShakingAnim != null)) ? "SHAKING_copy" : m_ShakingAnim.name];
		m_Shaking.blendMode = AnimationBlendMode.Additive;
		m_Shaking.wrapMode = WrapMode.Default;
		m_ShakingSpeedPct = Mathf.Clamp(m_ShakingSpeedPct, 0f, 1f);
		m_DrillDamageChecker = GetComponentInChildren<DrillDamageChecker>();
		if (m_DrillDamageChecker == null)
		{
			Debug.LogWarning("Neccessary script missing - DrillDamageChecker");
		}
		GameZone firstComponentUpward = base.gameObject.GetFirstComponentUpward<GameZone>();
		if (firstComponentUpward == null)
		{
			Debug.LogWarning("This object must be a child of GameZone.");
		}
		firstComponentUpward.RegisterControllableObject(this);
		m_CabineTransform = base.transform.Find("CABIN");
		if (m_CabineTransform == null)
		{
			Debug.LogError("DRILL - Can't find child object with name CABIN");
		}
		DeactivateAllWithoutObjectDamage(base.gameObject);
	}

	private void Start()
	{
		if (m_DeathEvent.Name != string.Empty && !GameBlackboard.Instance.GameEvents.Exist(m_DeathEvent.Name))
		{
			Debug.LogError("Drill :: Death event is not registered in GlobalBlackboard");
			Debug.LogError("Drill :: Please add event " + m_DeathEvent.Name + " into Mission.GlobalBlackboard");
			m_DeathEvent = null;
		}
		m_Target = ((!(m_Target == null)) ? m_Target : Player.Instance.Owner);
		if (m_Testing)
		{
			StartCoroutine(PlayShakingLoop_Test());
		}
	}

	private void Update()
	{
		if (!(Time.timeSinceLevelLoad - m_ActivationTime < m_StartIgnoreDalay) && Time.time > float.Epsilon && !m_Shaking.enabled && m_Moving.enabled)
		{
			float targetMoveSpeed = GetTargetMoveSpeed();
			m_Moving.speed = Mathf.MoveTowards(m_Moving.speed, targetMoveSpeed, 20f * Time.time);
		}
	}

	private void OnEnable()
	{
		m_Moving.speed = 1f;
		m_Animations.Play(m_Moving.name);
		if ((bool)m_Audio)
		{
			m_Audio.Play();
		}
		m_ActivationTime = Time.timeSinceLevelLoad;
	}

	private void OnDisable()
	{
		if ((bool)m_Audio)
		{
			m_Audio.Stop();
		}
		m_Animations.Stop();
		m_Animations[m_Moving.name].time = 0f;
		m_Animations[m_Moving.name].enabled = true;
		m_Animations.Sample();
		m_Animations[m_Moving.name].enabled = false;
		m_Animations.Stop();
		DeactivateAllWithoutObjectDamage(base.gameObject);
	}

	public void Enable()
	{
		m_HitPoints = m_DefaultHitPoints;
	}

	public void Disable()
	{
		if ((bool)m_Audio)
		{
			m_Audio.Stop();
		}
		m_Animations.Stop();
		DeactivateAllWithoutObjectDamage(base.gameObject);
	}

	public void Reset()
	{
		Disable();
		Enable();
		m_DrillDamageChecker.Reset();
	}

	public void OnObjectDamageHit_HACK(ObjectDamage inObject, int inTotalNumberOfZoness)
	{
		m_HitPoints -= m_DefaultHitPoints / ((float)inTotalNumberOfZoness - 0.1f);
		m_HitPoints = Mathf.Max(0f, m_HitPoints);
		if (m_HitPoints > 0f)
		{
			StartCoroutine(PlayShaking());
			return;
		}
		m_Moving.enabled = false;
		if (m_DeathEvent != null)
		{
			Mission.Instance.SendGameEvent(m_DeathEvent);
		}
		base.gameObject.SetActiveRecursively(false);
	}

	private float GetTargetMoveSpeed()
	{
		float magnitude = (m_Target.ChestPosition - m_CabineTransform.position).magnitude;
		return GetTargetMoveSpeed(magnitude);
	}

	private float GetTargetMoveSpeed(float inDistance)
	{
		float num = (m_MaxAnimSpeedScale - 1f) / (m_MaxPlayerDistance - m_MinPlayerDistance);
		float value = num * (inDistance - m_MinPlayerDistance) + 1f;
		return Mathf.Clamp(value, 1f, m_MaxAnimSpeedScale);
	}

	private IEnumerator PlayShaking()
	{
		float shakingTargetWeight = 1f;
		float shakingSpeedPct = m_ShakingSpeedPct;
		m_Animations.Blend("SHAKING_copy", shakingTargetWeight);
		if ((bool)m_Audio)
		{
			if (m_HitPoints > 0f)
			{
				m_Audio.PlayOneShot(m_ShakingSound);
			}
			else
			{
				m_Audio.Stop();
				m_Audio.PlayOneShot(m_DestroySound);
			}
		}
		float moveSpeed = 1f;
		while (m_Shaking.enabled)
		{
			float speed = GetTargetMoveSpeed();
			float k = 1f - (1f - shakingSpeedPct) * (m_Shaking.weight / shakingTargetWeight);
			moveSpeed = k * speed;
			m_Moving.speed = moveSpeed;
			yield return 0;
		}
	}

	public IEnumerator PlayShakingLoop_Test()
	{
		while (true)
		{
			if (m_Animations.isPlaying)
			{
				yield return new WaitForSeconds(2f);
				yield return StartCoroutine(PlayShaking());
			}
			else
			{
				yield return 0;
			}
		}
	}

	internal void DeactivateAllWithoutObjectDamage(GameObject inGameObject)
	{
		if (inGameObject.GetComponent<ObjectDamage>() != null)
		{
			return;
		}
		inGameObject.active = false;
		foreach (Transform item in inGameObject.transform)
		{
			DeactivateAllWithoutObjectDamage(item.gameObject);
		}
	}
}
