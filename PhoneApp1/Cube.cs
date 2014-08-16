using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using VirtualReality;

namespace PhoneApp1
{
	public class Cube
	{
		private GraphicsDevice device;
		private VertexBuffer cubeBuffer;

		private static readonly float[] cubeCoords = new float[] {
			// Front face
			-1.0f, 1.0f, 1.0f,
			-1.0f, -1.0f, 1.0f,
			1.0f, 1.0f, 1.0f,
			-1.0f, -1.0f, 1.0f,
			1.0f, -1.0f, 1.0f,
			1.0f, 1.0f, 1.0f,

			// Right face
			1.0f, 1.0f, 1.0f,
			1.0f, -1.0f, 1.0f,
			1.0f, 1.0f, -1.0f,
			1.0f, -1.0f, 1.0f,
			1.0f, -1.0f, -1.0f,
			1.0f, 1.0f, -1.0f,

			// Back face
			1.0f, 1.0f, -1.0f,
			1.0f, -1.0f, -1.0f,
			-1.0f, 1.0f, -1.0f,
			1.0f, -1.0f, -1.0f,
			-1.0f, -1.0f, -1.0f,
			-1.0f, 1.0f, -1.0f,

			// Left face
			-1.0f, 1.0f, -1.0f,
			-1.0f, -1.0f, -1.0f,
			-1.0f, 1.0f, 1.0f,
			-1.0f, -1.0f, -1.0f,
			-1.0f, -1.0f, 1.0f,
			-1.0f, 1.0f, 1.0f,

			// Top face
			-1.0f, 1.0f, -1.0f,
			-1.0f, 1.0f, 1.0f,
			1.0f, 1.0f, -1.0f,
			-1.0f, 1.0f, 1.0f,
			1.0f, 1.0f, 1.0f,
			1.0f, 1.0f, -1.0f,

			// Bottom face
			1.0f, -1.0f, -1.0f,
			1.0f, -1.0f, 1.0f,
			-1.0f, -1.0f, -1.0f,
			1.0f, -1.0f, 1.0f,
			-1.0f, -1.0f, 1.0f,
			-1.0f, -1.0f, -1.0f,
		};

		public Cube(GraphicsDevice device)
		{
			this.device = device;
			BuildCubeBuffer();
		}

		public Vector3 Position { get; set; }

		public Vector3 Rotation { get; set; }

		public void Draw(EyeTransform transform, BasicEffect effect)
		{
			//effect.VertexColorEnabled = true;
			//effect.View = HeadTransform.View;
			//effect.Projection = HeadTransform.Projection;
			//effect.World = Matrix.Identity;

			//foreach (EffectPass pass in effect.CurrentTechnique.Passes)
			//{
			//	pass.Apply();
			//	device.SetVertexBuffer(cubeBuffer);
			//	device.DrawPrimitives(PrimitiveType.TriangleList, 0, cubeBuffer.VertexCount / 3);
			//}
		}

		private void BuildCubeBuffer()
		{
			//List<VertexPositionColor> vertexList = new List<VertexPositionColor>();
			//bool isBlack = true;
			//for (int x = 0; x < floorWidth; x++)
			//{
			//	isBlack = !isBlack;
			//	for (int z = 0; z < floorHeight; z++)
			//	{
			//		vertexList.AddRange(FloorTile(x, z, isBlack ? Color.Black : Color.White));
			//		isBlack = !isBlack;
			//	}
			//}

			//floorBuffer = new VertexBuffer(device, VertexPositionColor.VertexDeclaration, vertexList.Count, BufferUsage.None);
			//floorBuffer.SetData(vertexList.ToArray());
		}

	}
}
