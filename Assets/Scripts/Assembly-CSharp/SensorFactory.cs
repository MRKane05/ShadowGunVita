internal class SensorFactory
{
	public static SensorBase Create(E_SensorType sensorType, AgentHuman owner)
	{
		SensorBase result = null;
		switch (sensorType)
		{
		case E_SensorType.CoverPlayer:
			result = new SensorCoverPlayer(owner);
			break;
		case E_SensorType.EyesAi:
			result = new SensorEyesAI(owner);
			break;
		case E_SensorType.EyesPlayer:
			result = new SensorEyesAI(owner);
			break;
		case E_SensorType.CoverAI:
			result = new SensorCoverAI(owner);
			break;
		case E_SensorType.Positions:
			result = new SensorPosition(owner);
			break;
		}
		return result;
	}
}
