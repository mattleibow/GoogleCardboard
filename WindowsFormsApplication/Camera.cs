using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace WindowsFormsApplication
{
	public class Camera : GameComponent
	{
		private Vector3 cameraPosition;
		private Vector3 cameraRotation;
		private float cameraSpeed;
		private Vector3 cameraLookAt;
		private Vector3 mouseRotationBuffer;
		private MouseState currentMouseState;
		private MouseState previousMouseState;

		public Matrix ProjectionMatrix { get; protected set; }

		public Matrix AttitudeMatrix { get; set; }

		public Matrix ViewMatrix
		{
			get { return Matrix.Identity * LookAtMatrix * AttitudeMatrix; }
		}

		private Matrix LookAtMatrix
		{
			get { return Matrix.CreateLookAt(Position, cameraLookAt, Vector3.Up); }
		}

		public Vector3 Position
		{
			get { return cameraPosition; }
			set
			{
				cameraPosition = value; 
				UpdateLookAt();
			}
		}

		public Vector3 Rotation
		{
			get { return cameraRotation; }
			set
			{
				cameraRotation = value;
				UpdateLookAt();
			}
		}


		public Camera(Game game, Vector3 position, Vector3 rotation, float speed)
			: base(game)
		{
			cameraSpeed = speed;

			ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(
				MathHelper.PiOver4,
				Game.GraphicsDevice.Viewport.AspectRatio,
				0.05f,
				1000.0f);
			AttitudeMatrix = Matrix.Identity;

			MoveTo(position, rotation);

			previousMouseState = Mouse.GetState();
		}

		public override void Update(GameTime gameTime)
		{
			float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

			currentMouseState = Mouse.GetState();

			KeyboardState ks = Keyboard.GetState();
			
			Vector3 moveVector = Vector3.Zero;

			if (ks.IsKeyDown(Keys.W))
				moveVector.Z = 1;
			if (ks.IsKeyDown(Keys.S))
				moveVector.Z = -1;
			if (ks.IsKeyDown(Keys.A))
				moveVector.X = 1;
			if (ks.IsKeyDown(Keys.D))
				moveVector.X = -1;

			if (moveVector != Vector3.Zero)
			{
				moveVector.Normalize();
				moveVector *= dt*cameraSpeed;

				Move(moveVector);
			}

			if (currentMouseState != previousMouseState)
			{
				float deltaX = currentMouseState.X - (Game.GraphicsDevice.Viewport.Width / 2);
				float deltaY = currentMouseState.Y - (Game.GraphicsDevice.Viewport.Height / 2);

				mouseRotationBuffer.X -= 0.01f * deltaX * dt;
				mouseRotationBuffer.Y -= 0.01f * deltaY * dt;

				mouseRotationBuffer.Y = MathHelper.Clamp(mouseRotationBuffer.Y, MathHelper.ToRadians(-75.0f), MathHelper.ToRadians(75.0f));
				mouseRotationBuffer.X = MathHelper.WrapAngle(mouseRotationBuffer.X);

				Rotation = new Vector3(-mouseRotationBuffer.Y, mouseRotationBuffer.X, 0);
			}

			Mouse.SetPosition(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2);

			previousMouseState = currentMouseState;

			base.Update(gameTime);
		}

		private Vector3 PreviewMove(Vector3 amount)
		{
			Matrix rotate = Matrix.CreateRotationY(Rotation.Y);
			Vector3 movement = new Vector3(amount.X, amount.Y, amount.Z);
			movement = Vector3.Transform(movement, rotate);
			return cameraPosition + movement;
		}

		private void Move(Vector3 scale)
		{
			MoveTo(PreviewMove(scale), Rotation);
		}

		private void MoveTo(Vector3 pos, Vector3 rot)
		{
			Position = pos;
			Rotation = rot;
		}

		private void UpdateLookAt()
		{
			Matrix rotationMatrix = Matrix.CreateRotationX(Rotation.X) * Matrix.CreateRotationY(Rotation.Y);
			Vector3 lookAtOffset = Vector3.Transform(Vector3.UnitZ, rotationMatrix);
			cameraLookAt = Position + lookAtOffset;
		}
	}
}
