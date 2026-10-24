using System.Collections.Generic;
using UnityEngine;

public class InputComponent : MonoBehaviour
{
	private static int MaxTouches = 4;

	private List<TouchEvent> TouchEvents = new List<TouchEvent>();

	private List<InputInterface> Receivers = new List<InputInterface>();

	public static InputComponent Instance;

	private void Awake()
	{
		Instance = this;
	}

	private void Update()
	{
		if (Input.touchCount != 0)
		{
			Touch touch = Input.GetTouch(0);
			if (touch.phase == TouchPhase.Began)
			{
				TouchBegin(touch);
			}
			else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
			{
				TouchUpdate(touch);
			}
			else if (touch.phase == TouchPhase.Ended)
			{
				TouchEnd(touch);
			}
		}
	}

	private void TouchBegin(Touch touch)
	{
		if (TouchEvents.Count != MaxTouches)
		{
			TouchEvent touchEvent = TouchEvent.Create(touch);
			TouchEvents.Add(touchEvent);
			SendToReceivers(touchEvent);
		}
	}

	private void TouchEnd(Touch touch)
	{
		for (int i = 0; i < TouchEvents.Count; i++)
		{
			if (TouchEvents[i].Id == touch.fingerId)
			{
				TouchEvent touchEvent = TouchEvents[i];
				touchEvent.Update(touch);
				SendToReceivers(touchEvent);
				TouchEvents.RemoveAt(i);
				TouchEvent.Return(touchEvent);
				break;
			}
		}
	}

	private void TouchUpdate(Touch touch)
	{
		for (int i = 0; i < TouchEvents.Count; i++)
		{
			if (TouchEvents[i].Id == touch.fingerId)
			{
				TouchEvent touchEvent = TouchEvents[i];
				touchEvent.Update(touch);
				SendToReceivers(touchEvent);
				break;
			}
		}
	}

	private void SendToReceivers(TouchEvent touch)
	{
		for (int i = 0; i < Receivers.Count; i++)
		{
			Receivers[i].ReceiveInput(touch);
		}
	}

	public void AddReceiver(InputInterface inputReceiver)
	{
		Receivers.Add(inputReceiver);
	}
}
