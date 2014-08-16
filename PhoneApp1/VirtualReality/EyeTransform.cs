using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VirtualReality
{
	public class EyeTransform
	{
		public EyeTransform(HeadTransform headTransform)
		{
			HeadTransform = headTransform;
		}

		public HeadTransform HeadTransform { get; private set; }

		public Viewport Viewport { get; set; }

		public Matrix Transform { get; set; }

		public Matrix Projection { get { return HeadTransform.GetProjection(Viewport); } }

		public Matrix View
		{
			get { return Transform*HeadTransform.View; }
		}
	}
}
