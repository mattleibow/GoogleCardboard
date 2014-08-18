using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Sensors.Motion;

namespace VirtualReality
{
	public class HeadTransform
	{
		private readonly MotionSensor motion;

		private Viewport viewport;
		private float interpupillaryDistance;
		private DisplayOrientation displayOrientation;
		private bool forceLandscape;

		public HeadTransform(Viewport viewport)
		{
			LeftEyeTransform = new EyeTransform(this);
			RightEyeTransform = new EyeTransform(this);
			ForceLandscape = true;
			Viewport = viewport; // this will update the eye viewports

			InterpupillaryDistance = 0.06F;
			VerticalDistanceToLensCenter = 0.035F;
			LensDiameter = 0.025F;
			ScreenToLensDistance = 0.037F;
			EyeToLensDistance = 0.011F;

			motion = new MotionSensor();
		}

		// eye transformations
		public float InterpupillaryDistance
		{
			get { return interpupillaryDistance; }
			set
			{
				interpupillaryDistance = value;

				float half = interpupillaryDistance * 0.5F;
				LeftEyeTransform.Transform = Matrix.CreateTranslation(-half, 0.0f, 0.0f);
				RightEyeTransform.Transform = Matrix.CreateTranslation(half, 0.0f, 0.0f);
			}
		}

		// lens transformations
		public float VerticalDistanceToLensCenter { get; set; }
		public float LensDiameter { get; set; }
		public float ScreenToLensDistance { get; set; }
		public float EyeToLensDistance { get; set; }

		public EyeTransform LeftEyeTransform { get; private set; }
		public EyeTransform RightEyeTransform { get; private set; }

		// view transformations

		public Viewport Viewport
		{
			get { return viewport; }
			set
			{
				viewport = value;
				UpdateViewport();
			}
		}

		public DisplayOrientation DisplayOrientation
		{
			get { return displayOrientation; }
			set
			{
				displayOrientation = value;
				UpdateViewport();
			}
		}

		public bool ForceLandscape
		{
			get { return forceLandscape; }
			set
			{
				forceLandscape = value;
				UpdateViewport();
			}
		}

		private void UpdateViewport()
		{
			Viewport left = Viewport;
			Viewport right = Viewport;

			var isLandscape = 
				DisplayOrientation == DisplayOrientation.LandscapeLeft || 
				DisplayOrientation == DisplayOrientation.LandscapeRight;

			if (ForceLandscape && !isLandscape)
			{
				left.Height = left.Height/2;
				right.Height = right.Height/2;
				right.Y = right.Height;
			}
			else
			{
				left.Width = left.Width/2;
				right.Width = right.Width/2;
				right.X = right.Width;
			}

			LeftEyeTransform.Viewport = left;
			RightEyeTransform.Viewport = right;
		}

		public Matrix Projection
		{
			get { return GetProjection(Viewport); }
		}

		public Matrix GetProjection(Viewport projectionViewport)
		{
			Matrix projection = Matrix.CreatePerspectiveFieldOfView(
				MathHelper.PiOver4,
				projectionViewport.AspectRatio,
				0.05f,
				1000.0f);

			Matrix rotation = Matrix.Identity;
			switch (DisplayOrientation)
			{
				case DisplayOrientation.LandscapeLeft:
					rotation = Matrix.CreateRotationZ(MathHelper.PiOver2);
					break;
				case DisplayOrientation.LandscapeRight:
					rotation = Matrix.CreateRotationZ(-MathHelper.PiOver2);
					break;
				case DisplayOrientation.Portrait:
					rotation = Matrix.Identity;
					break;
				case DisplayOrientation.PortraitDown:
					rotation = Matrix.CreateRotationZ(MathHelper.Pi);
					break;
				case DisplayOrientation.Default:
				case DisplayOrientation.Unknown:
				default:
					rotation = Matrix.Identity;
					break;
			}

			return rotation*projection;
		}

		public Matrix Rotation
		{
			get
			{
				var m = motion.CurrentValue.RotationMatrix;
				Matrix matrix = new Matrix(
					m[0], m[1], m[2], m[3],
					m[4], m[5], m[6], m[7],
					m[8], m[9], m[10], m[11],
					m[12], m[13], m[14], m[15]);

				return Matrix.CreateRotationX(MathHelper.PiOver2) * matrix;
			}
		}

		public Matrix View
		{
			get { return Matrix.Identity * LookAt * Rotation; }
		}

		public Matrix LookAt
		{
			get
			{
				return Matrix.CreateLookAt(Position, Position + Vector3.UnitZ, Vector3.Up);
			}
		}

		public Vector3 Position { get; set; }
	}
}
