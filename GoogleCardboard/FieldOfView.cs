using System;

 namespace com.google.vrtoolkit.cardboard
 {
	 
		 using Matrix = android.opengl.Matrix;
	 
		 public class FieldOfView
	
	 {		   private float mLeft;
		   private float mRight;
		   private float mBottom;
		   private float mTop;
	   
		   public FieldOfView()
	   {
	   }
	   
		   public FieldOfView(float left, float right, float bottom, float top)
	
	   {			 this.mLeft = left;
			 this.mRight = right;
			 this.mBottom = bottom;
			 this.mTop = top;
	
	   }	   
		   public FieldOfView(FieldOfView other)
	
	   {			 this.mLeft = other.mLeft;
			 this.mRight = other.mRight;
			 this.mBottom = other.mBottom;
			 this.mTop = other.mTop;
	
	   }	   
	
	   public virtual float Left
	   {
		   set
		
		   {					 this.mLeft = value;
		
		   }		   get
		
		   {					 return this.mLeft;
		
		   }
	   }	   
	
	   public virtual float Right
	   {
		   set
		
		   {					 this.mRight = value;
		
		   }		   get
		
		   {					 return this.mRight;
		
		   }
	   }	   
	
	   public virtual float Bottom
	   {
		   set
		
		   {					 this.mBottom = value;
		
		   }		   get
		
		   {					 return this.mBottom;
		
		   }
	   }	   
	
	   public virtual float Top
	   {
		   set
		
		   {					 this.mTop = value;
		
		   }		   get
		
		   {					 return this.mTop;
		
		   }
	   }	   
		   public virtual void toPerspectiveMatrix(float near, float far, float[] perspective, int offset)
	
	   {			 if (offset + 16 > perspective.Length)
		 {
			   throw new System.ArgumentException("Not enough space to write the result");
	
		 }			 float l = (float) - Math.Tan(Math.toRadians(this.mLeft)) * near;
			 float r = (float)Math.Tan(Math.toRadians(this.mRight)) * near;
			 float b = (float) - Math.Tan(Math.toRadians(this.mBottom)) * near;
			 float t = (float)Math.Tan(Math.toRadians(this.mTop)) * near;
			 Matrix.frustumM(perspective, offset, l, r, b, t, near, far);
	
	   }	   
		   public override bool Equals(object other)
	
	   {			 if (other == null)
		 {
			   return false;
	
		 }			 if (other == this)
		 {
			   return true;
	
		 }			 if (!(other is FieldOfView))
		 {
			   return false;
	
		 }			 FieldOfView o = (FieldOfView)other;
			 return (this.mLeft == o.mLeft) && (this.mRight == o.mRight) && (this.mBottom == o.mBottom) && (this.mTop == o.mTop);
	
	   }	   
		   public override string ToString()
	
	   {			 return "FieldOfView {left:" + this.mLeft + " right:" + this.mRight + " bottom:" + this.mBottom + " top:" + this.mTop + "}";
	
	   }	
	 }

	/* Location:           C:\Users\mattl_000.SURFACE\Downloads\cardboard.jar
	 * Qualified Name:     com.google.vrtoolkit.cardboard.FieldOfView
	 * JD-Core Version:    0.7.0.1
	 */
 }