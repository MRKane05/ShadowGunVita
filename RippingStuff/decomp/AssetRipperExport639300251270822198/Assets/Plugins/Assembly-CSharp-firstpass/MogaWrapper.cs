using System;
using UnityEngine;

internal class MogaWrapper
{
	private readonly AndroidJavaObject mController;

	public MogaWrapper(AndroidJavaObject controller)
	{
		mController = controller;
	}

	public static AndroidJavaObject getInstance(AndroidJavaObject activity)
	{
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.bda.controller.Controller"))
		{
			return androidJavaClass.CallStatic<AndroidJavaObject>("getInstance", new object[1] { activity });
		}
	}

	public bool init()
	{
		return mController.Call<bool>("init", new object[0]);
	}

	public void exit()
	{
		mController.Call("exit");
	}

	public float getAxisValue(int axis)
	{
		return mController.Call<float>("getAxisValue", new object[1] { axis });
	}

	public int getKeyCode(int keyCode)
	{
		return mController.Call<int>("getKeyCode", new object[1] { keyCode });
	}

	public int getInfo(int info)
	{
		return mController.Call<int>("getInfo", new object[1] { info });
	}

	public int getState(int state)
	{
		return mController.Call<int>("getState", new object[1] { state });
	}

	public void onPause()
	{
		mController.Call("onPause");
	}

	public void onResume()
	{
		mController.Call("onResume");
	}

	public void setListener(AndroidJavaObject listener, AndroidJavaObject handler)
	{
		IntPtr methodID = AndroidJNI.GetMethodID(mController.GetRawClass(), "setListener", "(Lcom/bda/controller/ControllerListener;Landroid/os/Handler;)V");
		object[] args = new object[2] { listener, handler };
		AndroidJNI.CallVoidMethod(mController.GetRawObject(), methodID, AndroidJNIHelper.CreateJNIArgArray(args));
	}
}
