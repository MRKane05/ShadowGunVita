using UnityEngine;

public class Mine : MonoBehaviour, IGameZoneControledObject
{
	private static float s_MaxAudioPitchMod = 0.1f;

	private AgentHuman m_Target;

	public Explosion m_Explosion;

	public Vector3 m_ExplosionOffset = Vector3.zero;

	public float m_MaxDamage;

	public float m_DamageRadius;

	private bool m_CountDownRunning;

	public AudioClip m_PipSound;

	public float m_DetectionDistance = 5f;

	private MeshRenderer m_SonarPlane;

	private float m_DefaultStartDelay = 0.5f;

	public float m_Period = 1f;

	public float m_MaxScale = 1.5f;

	public float m_ActivationDelay = 2f;

	public float m_NextDelay = 2f;

	public GameObject[] m_ShowObjectsOnDestroy;

	private float m_StartDetectionTime = -1f;

	private void Awake()
	{
		m_SonarPlane = base.transform.Find("mine_plane").GetComponent<MeshRenderer>();
		m_SonarPlane.transform.localScale = new Vector3(2f * m_DetectionDistance, 2f * m_DetectionDistance, 1f);
		base.GetComponent<AudioSource>().pitch = base.GetComponent<AudioSource>().pitch + Random.Range(0f - s_MaxAudioPitchMod, s_MaxAudioPitchMod);
		GameZone firstComponentUpward = base.gameObject.GetFirstComponentUpward<GameZone>();
		firstComponentUpward.RegisterControllableObject(this);
	}

	private void OnDestroy()
	{
		m_Explosion = null;
	}

	private void Start()
	{
		m_Target = ((!(m_Target == null)) ? m_Target : Player.Instance.Owner);
	}

	private void Update()
	{
		if (!m_CountDownRunning && IsEnemyInRange())
		{
			CancelInvoke();
			m_CountDownRunning = true;
			base.GetComponent<Animation>().Play();
			base.GetComponent<AudioSource>().Play();
		}
		else if (m_CountDownRunning && !base.GetComponent<Animation>().isPlaying)
		{
			Explode();
		}
	}

	private void PlayPipSound()
	{
		base.GetComponent<AudioSource>().PlayOneShot(m_PipSound);
		m_StartDetectionTime = Time.time;
	}

	public void Enable()
	{
		m_CountDownRunning = false;
		m_StartDetectionTime = -1f;
		base.gameObject.SetActiveRecursively(true);
		GameObject[] showObjectsOnDestroy = m_ShowObjectsOnDestroy;
		foreach (GameObject gameObject in showObjectsOnDestroy)
		{
			gameObject.active = false;
		}
		m_SonarPlane.material.SetVector("_Params", new Vector4(m_Period, m_MaxScale, 0f - Time.timeSinceLevelLoad - m_ActivationDelay - m_DefaultStartDelay, m_NextDelay));
		InvokeRepeating("PlayPipSound", m_ActivationDelay + m_DefaultStartDelay, m_Period + m_NextDelay);
		base.GetComponent<Animation>().clip.SampleAnimation(base.gameObject, 0f);
	}

	public void Reset()
	{
		Disable();
		Enable();
	}

	public void Disable()
	{
		m_CountDownRunning = false;
		m_StartDetectionTime = -1f;
		CancelInvoke();
		base.GetComponent<AudioSource>().Stop();
		base.GetComponent<Animation>().Stop();
		base.gameObject.SetActiveRecursively(false);
	}

	public bool IsEnemyInRange()
	{
		if (m_Target == null || m_StartDetectionTime < 0f)
		{
			return false;
		}
		float num = (Time.time - m_StartDetectionTime) / m_Period;
		if (num > 1f)
		{
			m_StartDetectionTime = -1f;
			return false;
		}
		float num2 = Mathf.Clamp((Time.time - m_StartDetectionTime) / m_Period, 0f, 1f) * m_DetectionDistance;
		num2 *= num2;
		Vector3 chestPosition = m_Target.ChestPosition;
		Vector3 vector = chestPosition - base.transform.position;
		if (vector.y < 0.5f || vector.y > 2.5f)
		{
			return false;
		}
		vector.y = 0f;
		float sqrMagnitude = vector.sqrMagnitude;
		return sqrMagnitude < num2;
	}

	public void Explode()
	{
		if (m_Explosion != null)
		{
			Explosion explosion = Mission.Instance.ExplosionCache.Get(m_Explosion, base.transform.position + m_ExplosionOffset, base.transform.rotation);
			if (explosion != null)
			{
				explosion.damage = m_MaxDamage;
				explosion.damageRadius = m_DamageRadius;
			}
		}
		base.gameObject.SetActiveRecursively(false);
		GameObject[] showObjectsOnDestroy = m_ShowObjectsOnDestroy;
		foreach (GameObject gameObject in showObjectsOnDestroy)
		{
			gameObject.active = true;
		}
	}
}
