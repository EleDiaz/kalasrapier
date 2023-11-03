namespace Kalasrapier
{
    public class Angles2D
    {
        public double Yaw { get; set; }
        public double Pitch { get; set; }


        public Angles2D()
        {
            Yaw = 0.0;
            Pitch = 0.0;
        }
        public Angles2D(double y, double p)
        {
            Yaw = y % 360.0;
            if (Yaw > 180)
                Yaw = -(360 - Yaw);
            Pitch = clampPitch(p % 360.0);

        }

        public Angles2D(Angles2D ang)
        {
            Yaw = ang.Yaw;
            Pitch = ang.Pitch;
        }


        public static Angles2D operator +(Angles2D bA1, Angles2D bA2)
        {
            double yaw, pitch;
            yaw = bA1.Yaw + bA2.Yaw;

            yaw = yaw % 360.0;
            if (yaw > 180)
                yaw = -(360.0 - yaw);
            pitch = bA1.Pitch + bA2.Pitch;
            pitch = clampPitch(pitch % 360.0);

            return new Angles2D(yaw, pitch);
        }
        public static Angles2D operator -(Angles2D bA1, Angles2D bA2)
        {
            double yaw, pitch;
            yaw = bA1.Yaw - bA2.Yaw;
            yaw = yaw % 360.0;
            pitch = bA1.Pitch - bA2.Pitch;
            pitch = clampPitch(pitch % 360.0);
            return new Angles2D(yaw, pitch);
        }

        public static Angles2D operator *(double f, Angles2D bA)
            => new Angles2D(f * bA.Yaw, f * bA.Pitch);
        public static Angles2D operator *(Angles2D bA, double f)
            => new Angles2D(f * bA.Yaw, f * bA.Pitch);


        private static double clampPitch(double p)
        {
            double lim = 89.9;
            if (p < -lim)
                return -lim;
            if (p > lim)
                return lim;
            return p;
        }


    }
}
