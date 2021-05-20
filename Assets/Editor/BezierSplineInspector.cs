using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BezierSpline))]
public class SplineInspector : Editor
{

	private const float handleSize = 0.04f;
	private const float pickSize = 0.06f;

	private int selectedIndex = -1;
	private BezierSpline spline;

	private static Color[] modeColors = new Color[]
	{
		Color.white,
		Color.yellow,
		Color.cyan
	};

	public override void OnInspectorGUI()
	{

		spline = target as BezierSpline;

		EditorGUI.BeginChangeCheck();

		float lengthTimeStep = EditorGUILayout.FloatField("Length Calculation Time Step", spline.GetLengthCalculationTimeStep());

		if (EditorGUI.EndChangeCheck())
		{

			Undo.RecordObject(spline, "LengthCalculationTimeStepChange");

			spline.SetLengthCalculationTimeStep(lengthTimeStep);

			EditorUtility.SetDirty(spline);

		}

		EditorGUILayout.LabelField("Spline Length", spline.GetSplineLength().ToString());

		EditorGUI.BeginChangeCheck();

		bool loop = EditorGUILayout.Toggle("Loop Spline", spline.GetLoop());

		if (EditorGUI.EndChangeCheck())
		{

			Undo.RecordObject(spline, "ToggleLoopSpline");

			spline.SetLoop(loop);

			EditorUtility.SetDirty(spline);

		}

		bool foldoutAll = Event.current.type == EventType.MouseUp && Event.current.modifiers == EventModifiers.Alt;

		EditorGUILayout.BeginHorizontal();

		EditorGUI.BeginChangeCheck();

		bool foldoutPoints = EditorGUILayout.Foldout(spline.GetFoldout(0), "Points", true);

		if (EditorGUI.EndChangeCheck() && foldoutAll)
        {

			spline.SetAllFoldouts(foldoutPoints);

        }

		GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
		{
			fixedWidth = 20,
			fixedHeight = 20,
			fontStyle = FontStyle.Bold,
			fontSize = 20,
			clipping = TextClipping.Overflow,
			contentOffset = new Vector2(1, -1)
		};

		DrawAddButton(-1, buttonStyle);

		EditorGUILayout.EndHorizontal();

		if (foldoutPoints)
        {

			EditorGUI.indentLevel++;

			for (int i = 0; i < spline.GetPointCount(); i += 3)
			{

				if (i != spline.GetPointCount() - 1 || !loop)
                {

					EditorGUILayout.BeginHorizontal();

					int foldoutNumber = (i / 3) + 1;

					bool foldoutPoint = EditorGUILayout.Foldout(spline.GetFoldout(foldoutNumber), "Point " + foldoutNumber, true);

					if (spline.GetPointCount() > 4 && (!loop || spline.GetPointCount() > 7))
                    {

						if (GUILayout.Button("-", buttonStyle))
						{

							Undo.RecordObject(spline, "RemoveSplineCurve");

							spline.RemovePoint(i);

							EditorUtility.SetDirty(spline);

							break;

						}

					}

					DrawAddButton(i, buttonStyle);

					EditorGUILayout.EndHorizontal();

					if (foldoutPoint)
					{

						EditorGUI.indentLevel++;

						DrawPointPositionField(i, "Local Position");

						if (i == 0)
						{

							if (loop)
							{

								DrawPointPositionField(i + 1, "Handle 1");

								DrawPointPositionField(spline.GetPointCount() - 2, "Handle 2");

							}
							else
							{

								DrawPointPositionField(i + 1, "Handle");

							}

						}
						else if (i == spline.GetPointCount() - 1)
						{

							DrawPointPositionField(i - 1, "Handle");

						}
						else
						{

							DrawPointPositionField(i - 1, "Handle 1");

							DrawPointPositionField(i + 1, "Handle 2");

						}

						DrawPointModeField(i, "Mode");

						EditorGUI.indentLevel--;

					}

					spline.SetFoldout(foldoutNumber, foldoutPoint);

				}

			}

			EditorGUI.indentLevel--;

		}

		spline.SetFoldout(0, foldoutPoints);

		if (selectedIndex >= 0 && selectedIndex < spline.GetPointCount())
		{

			GUILayout.Label("Selected Point", EditorStyles.boldLabel);

			EditorGUI.indentLevel++;

			EditorGUI.BeginChangeCheck();

			DrawPointPositionField(selectedIndex, "Local Position");

			DrawPointModeField(selectedIndex, "Mode");

		}

	}

	private void DrawPointPositionField(int pointIndex, string label)
    {

		EditorGUI.BeginChangeCheck();

		Vector3 pointPosition = EditorGUILayout.Vector3Field(label, spline.GetPointPosition(pointIndex));

		if (EditorGUI.EndChangeCheck())
		{

			Undo.RecordObject(spline, "SetSplinePoint" + label + pointIndex);

			spline.SetPointPosition(pointIndex, pointPosition);

			EditorUtility.SetDirty(spline);

		}

	}
	
	private void DrawPointModeField(int pointIndex, string label)
    {

		EditorGUI.BeginChangeCheck();

		SplinePointMode pointMode = (SplinePointMode)EditorGUILayout.EnumPopup("Mode", spline.GetPointMode(pointIndex));

		if (EditorGUI.EndChangeCheck())
		{

			Undo.RecordObject(spline, "SetSplineMode" + label + pointIndex);

			spline.SetPointMode(pointIndex, pointMode);

			EditorUtility.SetDirty(spline);

		}

	}

	private void DrawAddButton(int pointIndex, GUIStyle buttonStyle)
    {

		if (GUILayout.Button("+", buttonStyle))
		{

			Undo.RecordObject(spline, "AddSplineCurve");

			spline.AddPoint(pointIndex);

			EditorUtility.SetDirty(spline);

		}

	}

	private void OnSceneGUI()
	{

		spline = target as BezierSpline;

		Transform handleTransform = spline.transform;

		Quaternion handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;

		Vector3 point0 = handleTransform.TransformPoint(spline.GetPointPosition(0));

		for (int i = 1; i < spline.GetPointCount(); i += 3)
		{

			Vector3 handle1 = handleTransform.TransformPoint(spline.GetPointPosition(i));
			Vector3 handle2 = handleTransform.TransformPoint(spline.GetPointPosition(i + 1));
			Vector3 point3 = handleTransform.TransformPoint(spline.GetPointPosition(i + 2));

			Handles.color = Color.gray;

			Handles.DrawLine(point0, handle1);
			Handles.DrawLine(handle2, point3);

			Handles.DrawBezier(point0, point3, handle1, handle2, Color.white, null, 2f);

			point0 = point3;

			DrawHandle(i, handleTransform, handleRotation);
			DrawHandle(i + 1, handleTransform, handleRotation);
			DrawHandle(i + 2, handleTransform, handleRotation);

		}

		DrawHandle(0, handleTransform, handleRotation);

	}

	private void DrawHandle(int pointIndex, Transform handleTransform, Quaternion handleRotation)
	{

		Vector3 handlePosition = handleTransform.TransformPoint(spline.GetPointPosition(pointIndex));

		float size = HandleUtility.GetHandleSize(handlePosition);

		if (pointIndex == 0)
		{

			size *= 2f;

		}

		Handles.color = modeColors[(int)spline.GetPointMode(pointIndex)];

		if (Handles.Button(handlePosition, handleRotation, size * handleSize, size * pickSize, Handles.DotHandleCap))
		{

			selectedIndex = pointIndex;

			Repaint();

		}

		if (selectedIndex == pointIndex)
		{

			EditorGUI.BeginChangeCheck();

			handlePosition = Handles.DoPositionHandle(handlePosition, handleRotation);

			if (EditorGUI.EndChangeCheck())
			{

				Undo.RecordObject(spline, "MoveSplinePoint");

				spline.SetPointPosition(pointIndex, handleTransform.InverseTransformPoint(handlePosition));

				EditorUtility.SetDirty(spline);

			}

		}

	}

}
