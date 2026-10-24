using UnityEngine;

public class UVAnimation
{
	protected Vector2[] frames;

	protected int curFrame;

	protected int stepDir = 1;

	protected int numLoops;

	public string name;

	public int loopCycles;

	public bool loopReverse;

	public float framerate;

	public void Reset()
	{
		curFrame = 0;
		stepDir = 1;
		numLoops = 0;
	}

	public void PlayInReverse()
	{
		stepDir = -1;
		curFrame = frames.Length - 1;
	}

	public bool GetNextFrame(ref Vector2 uv)
	{
		if (curFrame + stepDir >= frames.Length || curFrame + stepDir < 0)
		{
			if (stepDir > 0 && loopReverse)
			{
				stepDir = -1;
				curFrame += stepDir;
				uv = frames[curFrame];
			}
			else
			{
				if (numLoops + 1 > loopCycles && loopCycles != -1)
				{
					return false;
				}
				numLoops++;
				if (loopReverse)
				{
					stepDir *= -1;
					curFrame += stepDir;
				}
				else
				{
					curFrame = 0;
				}
				uv = frames[curFrame];
			}
		}
		else
		{
			curFrame += stepDir;
			uv = frames[curFrame];
		}
		return true;
	}

	public Vector2[] BuildUVAnim(Vector2 start, Vector2 cellSize, int cols, int rows, int totalCells, float fps)
	{
		int num = 0;
		frames = new Vector2[totalCells];
		framerate = fps;
		frames[0] = start;
		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < cols; j++)
			{
				if (num >= totalCells)
				{
					break;
				}
				frames[num].x = start.x + cellSize.x * (float)j;
				frames[num].y = start.y + cellSize.y * (float)i;
				num++;
			}
		}
		return frames;
	}

	public void SetAnim(Vector2[] anim)
	{
		frames = anim;
	}

	public void AppendAnim(Vector2[] anim)
	{
		Vector2[] array = frames;
		frames = new Vector2[frames.Length + anim.Length];
		array.CopyTo(frames, 0);
		anim.CopyTo(frames, array.Length);
	}
}
