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

namespace PhoneClassLibrary1
{
	public class MotionSensor
	{
		private Motion motion;

		public MotionSensor()
		{
			if (!Motion.IsSupported)
			{
				MessageBox.Show("the Motion API is not supported on this device.");
				return;
			}

			// If the Motion object is null, initialize it and add a CurrentValueChanged
			// event handler.
			if (motion == null)
			{
				motion = new Motion();
				motion.TimeBetweenUpdates = TimeSpan.FromMilliseconds(20);
				motion.CurrentValueChanged += (sender, args) =>
				{
					MotionReading reading = args.SensorReading;
					AttitudeReading a = reading.Attitude;
					Matrix rm = a.RotationMatrix;
					Quaternion q = a.Quaternion;
					OnCurrentValueChanged(new MotionSensorEventArgs
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
					});
				};
			}

			// Try to start the Motion API.
			try
			{
				motion.Start();
			}
			catch (Exception ex)
			{
				MessageBox.Show("unable to start the Motion API.");
			}
		}

		public event EventHandler<MotionSensorEventArgs> CurrentValueChanged;

		protected virtual void OnCurrentValueChanged(MotionSensorEventArgs e)
		{
			EventHandler<MotionSensorEventArgs> handler = CurrentValueChanged;
			if (handler != null) handler(this, e);
		}
	}

	public class MotionSensorEventArgs : EventArgs
	{
		public float[] RotationMatrix { get; set; }
		public float Pitch { get; set; }
		public float Yaw { get; set; }
		public float Roll { get; set; }
		public float X { get; set; }
		public float Y { get; set; }
		public float Z { get; set; }
	}
}
