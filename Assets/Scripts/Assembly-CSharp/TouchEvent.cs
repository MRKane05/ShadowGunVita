using System.Collections.Generic;
using UnityEngine;

public class TouchEvent
{
	private static int _id;

	public int Id;

	private List<Vector2> Positions = new List<Vector2>();

	private static Queue<TouchEvent> UnusedEvents = new Queue<TouchEvent>();

	public TouchPhase CurrentPhase;

	public float StartTime;

	public int CountOfPositions
	{
		get
		{
			return Positions.Count;
		}
	}

	public static TouchEvent Create(Touch touch)
	{
		TouchEvent touchEvent = null;
		touchEvent = ((UnusedEvents.Count == 0) ? new TouchEvent() : UnusedEvents.Dequeue());
		touchEvent.Id = touch.fingerId;
		touchEvent.CurrentPhase = TouchPhase.Began;
		touchEvent.StartTime = Time.timeSinceLevelLoad;
		touchEvent.Positions.Add(touch.position);
		return touchEvent;
	}

	public static void Return(TouchEvent iEvent)
	{
		iEvent.Id = -1;
		iEvent.Positions.Clear();
		iEvent.CurrentPhase = TouchPhase.Canceled;
		UnusedEvents.Enqueue(iEvent);
	}

	public void Update(Touch touch)
	{
		CurrentPhase = touch.phase;
		Positions.Add(touch.position);
	}

	public Vector2 GetStartPos()
	{
		return Positions[0];
	}

	public Vector2 GetEndPos()
	{
		return Positions[Positions.Count - 1];
	}

	public float GetTouchTime()
	{
		return Time.timeSinceLevelLoad - StartTime;
	}

	public Vector2 GetPos(int index)
	{
		return Positions[index];
	}
}
