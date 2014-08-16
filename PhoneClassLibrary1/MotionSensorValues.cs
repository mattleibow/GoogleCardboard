using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Xna.Framework.Sensors.Motion
{
	using Motion = Microsoft.Devices.Sensors.Motion;

	public class MotionSensorValues
	{
		public float[] RotationMatrix { get; set; }

		public float Pitch { get;  set; }
		public float Yaw { get;  set; }
		public float Roll { get;  set; }

		public float X { get;  set; }
		public float Y { get;  set; }
		public float Z { get;  set; }
	}
}
