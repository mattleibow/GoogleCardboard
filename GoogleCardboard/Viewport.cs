namespace com.google.vrtoolkit.cardboard
{
	using GLES20 = android.opengl.GLES20;
	public class Viewport
	{
		public int x;
		public int y;
		public int width;
		public int height;
		
		public virtual void setViewport(int x, int y, int width, int height)
		{	
			this.x = x;
			
			this.y = y;
			
			this.width = width;
			
			this.height = height;
			
		}	
		
		public virtual void setGLViewport()
		
		{	
			GLES20.glViewport(this.x, this.y, this.width, this.height);
			
		}	
		
		public virtual void setGLScissor()
		
		{	
			GLES20.glScissor(this.x, this.y, this.width, this.height);
			
		}	
		
		public virtual void getAsArray(int[] array, int offset)
		
		{	
			if (offset + 4 > array.Length)
			{
				
				throw new System.ArgumentException("Not enough space to write the result");
				
			}			 array[offset] = this.x;
			
			array[(offset + 1)] = this.y;
			
			array[(offset + 2)] = this.width;
			
			array[(offset + 3)] = this.height;
			
		}	
		
		public override string ToString()
		
		{	
			return "Viewport {x:" + this.x + " y:" + this.y + " width:" + this.width + " height:" + this.height + "}";
		}	
	}
}