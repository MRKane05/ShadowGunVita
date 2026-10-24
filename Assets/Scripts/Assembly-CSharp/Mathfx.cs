using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Mathfx
{
	public static Vector3 InterpolateCatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
	{
		float num = t * t;
		float num2 = num * t;
		return 0.5f * (2f * p1 + (p2 - p0) * t + (2f * p0 - 5f * p1 + 4f * p2 - p3) * num + (3f * p1 - 3f * p2 + p3 - p0) * num2);
	}

	public static float Hermite(float start, float end, float value)
	{
		return Mathf.Lerp(start, end, value * value * (3f - 2f * value));
	}

	public static Vector3 Hermite(Vector3 start, Vector3 end, float value)
	{
		return new Vector3(Hermite(start.x, end.x, value), Hermite(start.y, end.y, value), Hermite(start.z, end.z, value));
	}

	public static float Sinerp(float start, float end, float value)
	{
		return Mathf.Lerp(start, end, Mathf.Sin(value * (float)Math.PI * 0.5f));
	}

	public static Vector3 Sinerp(Vector3 start, Vector3 end, float value)
	{
		return new Vector3(Sinerp(start.x, end.x, value), Sinerp(start.y, end.y, value), Sinerp(start.z, end.z, value));
	}

	public static float Coserp(float start, float end, float value)
	{
		return Mathf.Lerp(start, end, 1f - Mathf.Cos(value * (float)Math.PI * 0.5f));
	}

	public static Vector3 Coserp(Vector3 start, Vector3 end, float value)
	{
		return new Vector3(Coserp(start.x, end.x, value), Coserp(start.y, end.y, value), Coserp(start.z, end.z, value));
	}

	public static float Berp(float start, float end, float value)
	{
		value = Mathf.Clamp01(value);
		value = (Mathf.Sin(value * (float)Math.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + 1.2f * (1f - value));
		return start + (end - start) * value;
	}

	public static float SmoothStep(float x, float min, float max)
	{
		x = Mathf.Clamp(x, min, max);
		float num = (x - min) / (max - min);
		float num2 = (x - min) / (max - min);
		return -2f * num * num * num + 3f * num2 * num2;
	}

	public static float Lerp(float start, float end, float value)
	{
		return (1f - value) * start + value * end;
	}

	public static Vector3 NearestPoint(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
	{
		Vector3 vector = Vector3.Normalize(lineEnd - lineStart);
		float num = Vector3.Dot(lineStart - point, vector) / Vector3.Dot(vector, vector);
		return lineStart + num * vector;
	}

	public static float DistanceFromPointToVector(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
	{
		Vector3 vector = NearestPoint(lineStart, lineEnd, point);
		return (point - vector).magnitude;
	}

	public static Vector3 NearestPointStrict(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
	{
		Vector3 vector = lineEnd - lineStart;
		Vector3 vector2 = Vector3.Normalize(vector);
		float value = Vector3.Dot(point - lineStart, vector2) / Vector3.Dot(vector2, vector2);
		return lineStart + Mathf.Clamp(value, 0f, Vector3.Magnitude(vector)) * vector2;
	}

	public static float Bounce(float x)
	{
		return Mathf.Abs(Mathf.Sin(6.28f * (x + 1f) * (x + 1f)) * (1f - x));
	}

	public static bool Approx(float val, float about, float range)
	{
		return Mathf.Abs(val - about) < range;
	}

	public static bool Approx(Vector3 val, Vector3 about, float range)
	{
		return (val - about).sqrMagnitude < range * range;
	}

	public static float Clerp(float start, float end, float value)
	{
		float num = 0f;
		float num2 = 360f;
		float num3 = Mathf.Abs((num2 - num) / 2f);
		float num4 = 0f;
		float num5 = 0f;
		if (end - start < 0f - num3)
		{
			num5 = (num2 - start + end) * value;
			return start + num5;
		}
		if (end - start > num3)
		{
			num5 = (0f - (num2 - end + start)) * value;
			return start + num5;
		}
		return start + (end - start) * value;
	}

	public static Vector3 BezierSpline(Vector3[] pts, float pm)
	{
		float num = pm * (float)pts.Length;
		int num2 = Mathf.Clamp(Mathf.FloorToInt(num), 0, pts.Length - 1);
		float t = num - (float)num2;
		Vector3 vector;
		Vector3 to;
		if (num2 == 0)
		{
			vector = pts[0];
			to = (pts[1] + pts[0]) * 0.5f;
			return Vector3.Lerp(vector, to, t);
		}
		if (num2 == pts.Length - 1)
		{
			int num3 = pts.Length - 1;
			vector = (pts[num3 - 1] + pts[num3]) * 0.5f;
			to = pts[num3];
			return Vector3.Lerp(vector, to, t);
		}
		vector = (pts[num2 - 1] + pts[num2]) * 0.5f;
		to = (pts[num2 + 1] + pts[num2]) * 0.5f;
		Vector3 ctrl = pts[num2];
		return BezInterp(vector, to, ctrl, t);
	}

	public static Vector3 SmoothCurveDirection(Vector3[] pts, float pm)
	{
		float num = pm * (float)pts.Length;
		int num2 = Mathf.Clamp(Mathf.FloorToInt(num), 0, pts.Length - 1);
		float t = num - (float)num2;
		Vector3 vector;
		Vector3 vector2;
		if (num2 == 0)
		{
			vector = pts[0];
			vector2 = (pts[1] + pts[0]) * 0.5f;
			return vector2 - vector;
		}
		if (num2 == pts.Length - 1)
		{
			int num3 = pts.Length - 1;
			vector = (pts[num3 - 1] + pts[num3]) * 0.5f;
			vector2 = pts[num3];
			return vector2 - vector;
		}
		vector = (pts[num2 - 1] + pts[num2]) * 0.5f;
		vector2 = (pts[num2 + 1] + pts[num2]) * 0.5f;
		Vector3 ctrl = pts[num2];
		return BezDirection(vector, vector2, ctrl, t);
	}

	private static Vector3 BezInterp(Vector3 st, Vector3 en, Vector3 ctrl, float t)
	{
		float num = 1f - t;
		return num * num * st + 2f * num * t * ctrl + t * t * en;
	}

	private static Vector3 BezDirection(Vector3 st, Vector3 en, Vector3 ctrl, float t)
	{
		return (2f * st - 4f * ctrl + 2f * en) * t + 2f * ctrl - 2f * st;
	}

	public static E_Direction GetDirectionToVector(Transform transform, Vector3 dir)
	{
		float num = Vector3.Dot(transform.forward, dir);
		float num2 = Vector3.Dot(transform.right, dir);
		if (num > 0.5f)
		{
			return E_Direction.Forward;
		}
		if (num < -0.5f)
		{
			return E_Direction.Backward;
		}
		if (num2 > 0.5f)
		{
			return E_Direction.Right;
		}
		return E_Direction.Left;
	}

	public static Vector3 GetBestPositionFromPath(Vector3 start, Vector3[] path, int numberofCheckpoint, float minDistance, float maxDistance)
	{
		Vector3 result = start;
		Vector3 vector = start;
		float num = 0f;
		for (int i = 0; i < numberofCheckpoint; i++)
		{
			float magnitude = (path[i] - vector).magnitude;
			num += magnitude;
			if (num > maxDistance)
			{
				return vector + (path[i] - vector).normalized * Random.Range(0f, magnitude - (num - maxDistance));
			}
			result = vector;
			vector = path[i];
		}
		return result;
	}

	public static void Matrix_SetPos(ref Matrix4x4 inoutMatrix, Vector3 inPos)
	{
		inoutMatrix.m03 = inPos.x;
		inoutMatrix.m13 = inPos.y;
		inoutMatrix.m23 = inPos.z;
	}

	public static Vector3 Matrix_GetPos(Matrix4x4 inMatrix)
	{
		return new Vector3(inMatrix.m03, inMatrix.m13, inMatrix.m23);
	}

	public static void Matrix_SetEulerAngles(ref Matrix4x4 inoutMatrix, Vector3 inEulerAngles)
	{
		float num = Mathf.Cos(inEulerAngles.x);
		float num2 = Mathf.Sin(inEulerAngles.x);
		float num3 = Mathf.Cos(inEulerAngles.y);
		float num4 = Mathf.Sin(inEulerAngles.y);
		float num5 = Mathf.Cos(inEulerAngles.z);
		float num6 = Mathf.Sin(inEulerAngles.z);
		inoutMatrix.m00 = num3 * num5 + num2 * num4 * num6;
		inoutMatrix.m10 = num5 * num2 * num4 - num3 * num6;
		inoutMatrix.m20 = num * num4;
		inoutMatrix.m01 = num * num6;
		inoutMatrix.m11 = num * num5;
		inoutMatrix.m21 = 0f - num2;
		inoutMatrix.m02 = (0f - num5) * num4 + num3 * num2 * num6;
		inoutMatrix.m12 = num3 * num5 * num2 + num4 * num6;
		inoutMatrix.m22 = num * num3;
	}

	public static Vector3 Matrix_GetEulerAngles(Matrix4x4 inMatrix)
	{
		Vector3 euler = Vector3.zero;
		if (inMatrix.m21 < 0.999f)
		{
			if (inMatrix.m21 > -0.999f)
			{
				euler.x = Mathf.Asin(0f - inMatrix.m21);
				euler.y = Mathf.Atan2(inMatrix.m20, inMatrix.m22);
				euler.z = Mathf.Atan2(inMatrix.m01, inMatrix.m11);
				SanitizeEuler(ref euler);
				return euler;
			}
			euler.x = (float)Math.PI / 2f;
			euler.y = Mathf.Atan2(inMatrix.m10, inMatrix.m00);
			euler.z = 0f;
			SanitizeEuler(ref euler);
			return euler;
		}
		euler.x = -(float)Math.PI / 2f;
		euler.y = Mathf.Atan2(0f - inMatrix.m10, inMatrix.m00);
		euler.z = 0f;
		SanitizeEuler(ref euler);
		return euler;
	}

	internal static void SanitizeEuler(ref Vector3 euler)
	{
		float num = -0.0001f;
		float num2 = 6.2830853f;
		if (euler.x < num)
		{
			euler.x += (float)Math.PI * 2f;
		}
		else if (euler.x > num2)
		{
			euler.x -= (float)Math.PI * 2f;
		}
		if (euler.y < num)
		{
			euler.y += (float)Math.PI * 2f;
		}
		else if (euler.y > num2)
		{
			euler.y -= (float)Math.PI * 2f;
		}
		if (euler.z < num)
		{
			euler.z += (float)Math.PI * 2f;
		}
		else if (euler.z > num2)
		{
			euler.z -= (float)Math.PI * 2f;
		}
	}
}
