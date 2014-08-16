using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Devices.Sensors;
using Microsoft.Xna.Framework;
using Matrix = Microsoft.Xna.Framework.Matrix;

namespace Microsoft.Xna.Framework.Sensors.Motion
{
	using Motion = Microsoft.Devices.Sensors.Motion;

	public class MotionSensor
	{
		private readonly Motion motion;

		public MotionSensor()
		{
			try
			{
				motion = new Motion();
				motion.TimeBetweenUpdates = TimeSpan.FromMilliseconds(20);
				motion.Start();
			}
			catch (Exception ex)
			{
				throw new Exception("Unable to start motion sensor.", ex);
			}
		}

		public bool IsSupported { get { return Motion.IsSupported; } }

		public MotionSensorValues CurrentValue
		{
			get
			{
				if (!motion.IsDataValid)
					return null;

				MotionReading reading = motion.CurrentValue;
				AttitudeReading a = reading.Attitude;
				Matrix rm = a.RotationMatrix;
				Quaternion q = a.Quaternion;

				return new MotionSensorValues
				{
					RotationMatrix = new[]
					{
						rm.M11, rm.M12, rm.M13, rm.M14,
						rm.M21, rm.M22, rm.M23, rm.M24,
						rm.M31, rm.M32, rm.M33, rm.M34,
						rm.M41, rm.M42, rm.M43, rm.M44,
					},
					Pitch = a.Pitch,
					Yaw = a.Yaw,
					Roll = a.Roll,
					X = q.X,
					Y = q.Y,
					Z = q.Z,
				};
			}
		}
	}
}
