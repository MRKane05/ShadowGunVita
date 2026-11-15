public class CoverAIInfo
{
	public Cover Cover;

	public float LeftCoverValidity;

	public float RightCoverValidity;

	public float MiddleCoverValidity;

	public void Reset()
	{
		Cover = null;
		LeftCoverValidity = 0f;
		RightCoverValidity = 0f;
		MiddleCoverValidity = 0f;
	}
}
