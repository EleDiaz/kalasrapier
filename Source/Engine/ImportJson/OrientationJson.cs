namespace Kalasrapier.Engine.ImportJson
{
    public class OrientationJson
    {
        public float[] axis { get; set; }
        public float angle { get; set; }

        public OrientationJson()
        {
            this.axis = new float[] { 0.0f, 1.0f, 0.0f };
            this.angle = 0f;
        }

        public OrientationJson(float[] axis, float angle)
        {
            this.axis = axis;
            this.angle = angle;
        }
    }
}

