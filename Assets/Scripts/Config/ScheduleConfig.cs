using UnityEngine;

namespace Approximator
{
    [CreateAssetMenu(fileName = "ScheduleConfig", menuName = "Schedule config")]
    public class ScheduleConfig : ScriptableObject
    {
        [SerializeField]
        int xSteps = 7;
        [SerializeField]
        int ySteps = 11;
        [SerializeField]
        int drawingFrequency = 30;
        [SerializeField]
        int offset = 10;
        [SerializeField]
        float width = 3;
        [SerializeField]
        float dashLength = 4;
        [SerializeField]
        Material scheduleMaterial;
        [SerializeField]
        Material lagrangeMaterial;
        [SerializeField]
        Material olsMaterial;
        [SerializeField]
        Material pointsMaterial;
        [SerializeField]
        float pointRadius = .2f;

        public int XSteps { get { return xSteps; } }
        public int YSteps { get { return ySteps; } }
        public int DrawingFrequency { get { return drawingFrequency; } }
        public int Offset { get { return offset; } }
        public float Width { get { return width; } }
        public float DashLength { get { return dashLength; } }
        public Material ScheduleMaterial { get { return scheduleMaterial; } }
        public Material LagrangeMaterial { get { return lagrangeMaterial; } }
        public Material OLSMaterial { get { return olsMaterial; } }
        public Material PointsMaterial { get { return pointsMaterial; } }
        public float PointRadius { get { return pointRadius; } }
    }
}