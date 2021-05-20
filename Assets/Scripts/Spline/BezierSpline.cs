using System.Collections.Generic;
using UnityEngine;

public enum SplinePointMode
{
	Free,
	Aligned,
	Mirrored
}

public class BezierSpline : MonoBehaviour
{

	[SerializeField] private List<Vector3> points;
	[SerializeField] private List<SplinePointMode> modes;
	[SerializeField] private bool loop;
	[SerializeField] private float lengthCalculationTimeStep = 0.01f;
	[SerializeField, HideInInspector] private List<bool> editorFoldouts;
	[SerializeField, HideInInspector] private float splineLength;
	
	private void Reset()
	{

		points = new List<Vector3>
		{
			new Vector3(1f, 0f, 0f),
			new Vector3(2f, 0f, 0f),
			new Vector3(3f, 0f, 0f),
			new Vector3(4f, 0f, 0f)
		};

		modes = new List<SplinePointMode>
		{
			SplinePointMode.Free,
			SplinePointMode.Free
		};

		editorFoldouts = new List<bool>
		{
			true,
			true,
			true
		};

		ApproximateSplineLength();

	}

    public int GetPointCount()
    {

		return points.Count;

    }
	
	public Vector3 GetPointPosition(int pointIndex)
    {

		return points[pointIndex];

    }

	public void SetPointPosition(int pointIndex, Vector3 position)
    {

		if (pointIndex % 3 == 0)
		{

			Vector3 delta = position - points[pointIndex];
			
			if (loop)
			{

				if (pointIndex == 0)
				{

					points[1] += delta;
					points[points.Count - 2] += delta;
					points[points.Count - 1] = position;
				
				}
				else if (pointIndex == points.Count - 1)
				{

					points[0] = position;
					points[1] += delta;
					points[pointIndex - 1] += delta;
				
				}
				else
				{

					points[pointIndex - 1] += delta;
					points[pointIndex + 1] += delta;

				}

			}
			else
			{

				if (pointIndex > 0)
				{

					points[pointIndex - 1] += delta;
				
				}
				
				if (pointIndex + 1 < points.Count)
				{

					points[pointIndex + 1] += delta;
				
				}

			}

		}
		
		points[pointIndex] = position;

		EnforceMode(pointIndex);

		ApproximateSplineLength();

	}

	public SplinePointMode GetPointMode(int pointIndex)
	{

		return modes[(pointIndex + 1) / 3];

	}

	public void SetPointMode(int pointIndex, SplinePointMode mode)
    {

		int index = (pointIndex + 1) / 3;

		modes[index] = mode;

		if (loop)
		{

			if (index == 0)
			{

				modes[modes.Count - 1] = mode;

			}
			else if (index == modes.Count - 1)
			{

				modes[0] = mode;

			}

		}

		EnforceMode(pointIndex);

		ApproximateSplineLength();

	}

	public void AddPoint(int pointIndex)
	{

		if (pointIndex == -1)
        {

			Vector3 direction = GetBezierFirstDerivative(points[0], points[1], points[2], points[3], 0).normalized;

			Vector3 position = points[0];

			points.Insert(0, position - direction);
			points.Insert(0, position - (direction * 2));
			points.Insert(0, position - (direction * 3));

		}
		else if (pointIndex == points.Count - 1)
        {

			Vector3 direction = GetBezierFirstDerivative(points[pointIndex - 3], points[pointIndex - 2], points[pointIndex - 1], points[pointIndex], 1).normalized;

			Vector3 position = points[pointIndex];

			points.Add(position + direction);
			points.Add(position + (direction * 2));
			points.Add(position + (direction * 3));
			
		}
        else
        {

			Vector3 position = GetBezierPoint(points[pointIndex], points[pointIndex + 1], points[pointIndex + 2], points[pointIndex + 3], 0.5f);

			Vector3 direction = GetBezierFirstDerivative(points[pointIndex], points[pointIndex + 1], points[pointIndex + 2], points[pointIndex + 3], 0.5f).normalized;

			points.Insert(pointIndex + 2, position - direction);
			points.Insert(pointIndex + 3, position);
			points.Insert(pointIndex + 4, position + direction);

		}

		int modeIndex = pointIndex / 3;
		modes.Insert(modeIndex + 1, modes[modeIndex]);

		int foldoutIndex = (pointIndex / 3) + 1;
		editorFoldouts.Insert(foldoutIndex + 1, editorFoldouts[foldoutIndex]);

		if (pointIndex == -1)
		{

			EnforceMode(0);
			EnforceMode(3);

		}
		else if (pointIndex == points.Count - 4)
		{

			EnforceMode(pointIndex);
			EnforceMode(pointIndex + 3);

		}
		else
		{

			EnforceMode(pointIndex);
			EnforceMode(pointIndex + 3);
			EnforceMode(pointIndex + 6);

		}

		if (loop)
		{

			points[points.Count - 1] = points[0];

			modes[modes.Count - 1] = modes[0];

			EnforceMode(0);

		}

		ApproximateSplineLength();

	}

	public void RemovePoint(int pointIndex)
	{

		if (pointIndex == 0)
		{

			points.RemoveAt(pointIndex + 2);

			points.RemoveAt(pointIndex + 1);

			points.RemoveAt(pointIndex);

		}
		else if (pointIndex == points.Count - 1)
		{

			points.RemoveAt(pointIndex);

			points.RemoveAt(pointIndex - 1);

			points.RemoveAt(pointIndex - 2);

		}
		else
		{

			points.RemoveAt(pointIndex + 1);

			points.RemoveAt(pointIndex);

			points.RemoveAt(pointIndex - 1);

		}

		modes.RemoveAt(pointIndex / 3);

		editorFoldouts.RemoveAt((pointIndex / 3) + 1);

		ApproximateSplineLength();

	}

	public void SetPointCount(int count)
    {

		Reset();

		for (int i = 2; i < count; i++)
        {
			
			AddPoint(points.Count - 1);

        }

		ApproximateSplineLength();

	}

	public bool GetFoldout(int foldoutIndex)
    {

		return editorFoldouts[foldoutIndex];

    }
	
	public void SetFoldout(int foldoutIndex, bool value)
    {

		editorFoldouts[foldoutIndex] = value;

	}
	
	public void SetAllFoldouts(bool value)
    {

		for (int i = 0; i < editorFoldouts.Count; i++)
        {

			editorFoldouts[i] = value;

		}

	}

	private int GetCurveCount()
	{

		return (points.Count - 1) / 3;

	}

	public bool GetLoop()
    {

		return loop;

    }

	public void SetLoop(bool value)
    {

		loop = value;

		if (loop)
        {

			modes[modes.Count - 1] = modes[0];

			SetPointPosition(0, points[0]);

		}

		ApproximateSplineLength();

	}

	public Vector3 GetSplinePosition(float time)
	{

		int i;

		if (time >= 1f)
		{

			time = 1f;

			i = points.Count - 4;

		}
		else
		{

			time = Mathf.Clamp01(time) * GetCurveCount();

			i = (int)time;

			time -= i;

			i *= 3;

		}

		return transform.TransformPoint(GetBezierPoint(points[i], points[i + 1], points[i + 2], points[i + 3], time));

	}
	
	public Vector3 GetSplinePositionAtDistance(float distance)
	{

		if (loop && distance > splineLength)
        {

			distance %= splineLength;

		}

		return GetSplinePosition(distance / splineLength);

	}

	public Vector3 GetSplineDirection(float time)
	{

		int i;

		if (time >= 1f)
		{

			time = 1f;

			i = points.Count - 4;

		}
		else
		{

			time = Mathf.Clamp01(time) * GetCurveCount();

			i = (int)time;

			time -= i;

			i *= 3;

		}
		
		Vector3 velocity = transform.TransformPoint(GetBezierFirstDerivative(points[i], points[i + 1], points[i + 2], points[i + 3], time)) - transform.position;

		return velocity.normalized;

	}

	private void EnforceMode(int pointIndex)
	{

		int modeIndex = (pointIndex + 1) / 3;

		SplinePointMode mode = modes[modeIndex];
		
		if (mode != SplinePointMode.Free && (loop || (modeIndex != 0 && modeIndex != modes.Count - 1)))
		{

			int middleIndex = modeIndex * 3;

			int fixedIndex;

			int enforcedIndex;

			if (pointIndex <= middleIndex)
			{

				fixedIndex = middleIndex - 1;

				if (fixedIndex < 0)
				{

					fixedIndex = points.Count - 2;

				}

				enforcedIndex = middleIndex + 1;

				if (enforcedIndex >= points.Count)
				{

					enforcedIndex = 1;
				
				}
			
			}
			else
			{

				fixedIndex = middleIndex + 1;

				if (fixedIndex >= points.Count)
				{

					fixedIndex = 1;
				
				}
				
				enforcedIndex = middleIndex - 1;
				
				if (enforcedIndex < 0)
				{

					enforcedIndex = points.Count - 2;
				
				}
			
			}

			Vector3 middle = points[middleIndex];
			
			Vector3 enforcedTangent = middle - points[fixedIndex];
			
			if (mode == SplinePointMode.Aligned)
			{

				enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, points[enforcedIndex]);
			
			}

			points[enforcedIndex] = middle + enforcedTangent;

		}

	}

	private static Vector3 GetBezierPoint(Vector3 startPoint, Vector3 startTangent, Vector3 endTangent, Vector3 endPoint, float time)
	{

		time = Mathf.Clamp01(time);

		float oneMinusTime = 1f - time;

		return (oneMinusTime * oneMinusTime * oneMinusTime * startPoint) + (3f * oneMinusTime * oneMinusTime * time * startTangent) + (3f * oneMinusTime * time * time * endTangent) + (time * time * time * endPoint);

	}

	private static Vector3 GetBezierFirstDerivative(Vector3 startPoint, Vector3 startTangent, Vector3 endTangent, Vector3 endPoint, float time)
	{
		
		time = Mathf.Clamp01(time);

		float oneMinusTime = 1f - time;

		return (3f * oneMinusTime * oneMinusTime * (startTangent - startPoint)) + (6f * oneMinusTime * time * (endTangent - startTangent)) + (3f * time * time * (endPoint - endTangent));

	}

	private void ApproximateSplineLength()
    {

		if (lengthCalculationTimeStep <= 0)
        {

			lengthCalculationTimeStep = 0.01f;

		}

		float time = 0;

		splineLength = 0;

		Vector3 lastPoint = GetSplinePosition(0);

		while (time < 1)
        {

			time += lengthCalculationTimeStep;

			Vector3 nextPoint = GetSplinePosition(time);

			splineLength += Vector3.Distance(lastPoint, nextPoint);

			lastPoint = nextPoint;

        }

	}

	public float GetSplineLength()
    {

		return splineLength;

	}

	public float GetLengthCalculationTimeStep()
    {

		return lengthCalculationTimeStep;

	}

	public void SetLengthCalculationTimeStep(float value)
    {

		lengthCalculationTimeStep = value;

		ApproximateSplineLength();

	}

}
