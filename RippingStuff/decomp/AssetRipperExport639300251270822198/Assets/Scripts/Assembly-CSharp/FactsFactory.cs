public static class FactsFactory
{
	private static Queue<Fact> m_Unused = new Queue<Fact>();

	public static Fact Create(E_EventTypes eventType)
	{
		Fact fact;
		if (m_Unused.Count != 0)
		{
			fact = m_Unused.Dequeue();
			fact.Belief = 1f;
			fact.LiveTime = 0.2f;
			fact.Delay = 0f;
		}
		else
		{
			fact = new Fact();
		}
		fact.Type = eventType;
		return fact;
	}

	public static void Return(Fact f)
	{
		m_Unused.Enqueue(f);
	}

	public static void Clear()
	{
		m_Unused.Clear();
	}
}
