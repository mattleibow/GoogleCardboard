using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsFormsApplication
{
	public class Floor
	{
		private int floorWidth;
		private int floorHeight;
		private VertexBuffer floorBuffer;
		private GraphicsDevice device;

		public Floor(GraphicsDevice device, int width, int height)
		{
			this.device = device;
			this.floorWidth = width;
			this.floorHeight = height;
			BuildFloorBuffer();
		}

		public Texture2D Texture { get; set; }

		public void Draw(Camera camera, BasicEffect effect)
		{
			effect.View = camera.ViewMatrix;
			effect.Projection = camera.ProjectionMatrix;
			effect.World = Matrix.Identity;
			
			effect.TextureEnabled = true;
			effect.Texture = Texture;

			foreach (EffectPass pass in effect.CurrentTechnique.Passes)
			{
				pass.Apply();

				//device.SamplerStates[0] = SamplerState.AnisotropicWrap;

				device.SetVertexBuffer(floorBuffer);
				device.DrawPrimitives(PrimitiveType.TriangleList, 0, floorBuffer.VertexCount / 3);
			}
		}

		private void BuildFloorBuffer()
		{
			List<VertexPositionTexture> vertexList = FloorTile(floorWidth, floorHeight);

			floorBuffer = new VertexBuffer(device, VertexPositionTexture.VertexDeclaration, vertexList.Count, BufferUsage.None);
			floorBuffer.SetData(vertexList.ToArray());
		}

		private List<VertexPositionTexture> FloorTile(int x, int z)
		{
			List<VertexPositionTexture> vList = new List<VertexPositionTexture>(6);
			vList.Add(new VertexPositionTexture(new Vector3(0, 0, 0), new Vector2(0, 0)));
			vList.Add(new VertexPositionTexture(new Vector3(x, 0, 0), new Vector2(x, 0)));
			vList.Add(new VertexPositionTexture(new Vector3(0, 0, z), new Vector2(0, z)));
			vList.Add(new VertexPositionTexture(new Vector3(x, 0, 0), new Vector2(x, 0)));
			vList.Add(new VertexPositionTexture(new Vector3(x, 0, z), new Vector2(x, z)));
			vList.Add(new VertexPositionTexture(new Vector3(0, 0, z), new Vector2(0, z)));
			return vList;
		}
	}
}
