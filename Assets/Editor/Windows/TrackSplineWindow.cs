using UnityEngine;
using UnityEditor;

public class TrackSplineWindow : EditorWindow
{

    private TrackData trackData;
    private BezierSpline spline;
    
    [MenuItem("Window/Custom/Track Spline")]
    private static void Init()
    {

        TrackSplineWindow window = GetWindow<TrackSplineWindow>();

        window.titleContent = new GUIContent("Track Spline");

        window.Show();

    }

    private void OnGUI()
    {

        trackData = EditorGUILayout.ObjectField("Track Data", trackData, typeof(TrackData), true) as TrackData;
        
        spline = EditorGUILayout.ObjectField("Spline", spline, typeof(BezierSpline), true) as BezierSpline;

        if (GUILayout.Button("Set") && spline != null && trackData != null)
        {

            spline.SetPointCount(trackData.checkpoints.Count + 1);

            spline.SetLoop(true);

            int splinePointIndex = 0;

            for (int i = 0; i < trackData.checkpoints.Count; i++)
            {

                spline.SetPointMode(splinePointIndex, SplinePointMode.Mirrored);

                Vector3 checkpointPosition = trackData.checkpoints[i].position;

                Vector3 from;

                Vector3 to;

                if (i == 0)
                {

                    from = checkpointPosition - trackData.checkpoints[trackData.checkpoints.Count - 1].position;

                    to = trackData.checkpoints[i + 1].position - checkpointPosition;

                }
                else if (i == trackData.checkpoints.Count - 1)
                {

                    from = checkpointPosition - trackData.checkpoints[i - 1].position;

                    to = trackData.checkpoints[0].position - checkpointPosition;

                }
                else
                {

                    from = checkpointPosition - trackData.checkpoints[i - 1].position;

                    to = trackData.checkpoints[i + 1].position - checkpointPosition;

                }

                spline.SetPointPosition(splinePointIndex, checkpointPosition);

                Vector3 handlePosition = checkpointPosition + ((from + to) / 5);

                spline.SetPointPosition(splinePointIndex + 1, handlePosition);

                splinePointIndex += 3;

            }
            
        }
        
    }

}
