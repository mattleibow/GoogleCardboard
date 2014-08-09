using System;

/*   1:    */ namespace com.google.vrtoolkit.cardboard
 {
	/*   2:    */ 
	/*   3:    */	 public class Distortion
	/*   4:    */
	 {	/*   5: 31 */	   private static readonly float[] DEFAULT_COEFFICIENTS = new float[] {250.0F, 50000.0F};
	/*   6:    */	   private float[] mCoefficients;
	/*   7:    */   
	/*   8:    */	   public Distortion()
	/*   9:    */
	   {	/*  10: 46 */		 this.mCoefficients = new float[2];
	/*  11: 47 */		 this.mCoefficients[0] = DEFAULT_COEFFICIENTS[0];
	/*  12: 48 */		 this.mCoefficients[1] = DEFAULT_COEFFICIENTS[1];
	/*  13:    */
	   }	/*  14:    */   
	/*  15:    */	   public Distortion(Distortion other)
	/*  16:    */
	   {	/*  17: 57 */		 this.mCoefficients = new float[2];
	/*  18: 58 */		 this.mCoefficients[0] = other.mCoefficients[0];
	/*  19: 59 */		 this.mCoefficients[1] = other.mCoefficients[1];
	/*  20:    */
	   }	/*  21:    */   
	/*  22:    */
	   public virtual float[] Coefficients
	   {
		   set
		/*  23:    */
		   {		/*  24: 76 */			 this.mCoefficients[0] = value[0];
		/*  25: 77 */			 this.mCoefficients[1] = value[1];
		/*  26:    */
		   }		   get
		/*  29:    */
		   {		/*  30: 86 */			 return this.mCoefficients;
		/*  31:    */
		   }
	   }	/*  32:    */   
	/*  33:    */	   public virtual float distortionFactor(float radius)
	/*  34:    */
	   {	/*  35: 96 */		 float rSq = radius * radius;
	/*  36: 97 */		 return 1.0F + this.mCoefficients[0] * rSq + this.mCoefficients[1] * rSq * rSq;
	/*  37:    */
	   }	/*  38:    */   
	/*  39:    */	   public virtual float distort(float radius)
	/*  40:    */
	   {	/*  41:107 */		 return radius * distortionFactor(radius);
	/*  42:    */
	   }	/*  43:    */   
	/*  44:    */	   public virtual float distortInverse(float radius)
	/*  45:    */
	   {	/*  46:120 */		 float r0 = radius / 0.9F;
	/*  47:121 */		 float r1 = radius * 0.9F;
	/*  48:    */     
	/*  49:123 */		 float dr0 = radius - distort(r0);
	/*  50:125 */		 while (Math.Abs(r1 - r0) > 0.0001D)
	/*  51:    */
		 {	/*  52:126 */		   float dr1 = radius - distort(r1);
	/*  53:127 */		   float r2 = r1 - dr1 * ((r1 - r0) / (dr1 - dr0));
	/*  54:128 */		   r0 = r1;
	/*  55:129 */		   r1 = r2;
	/*  56:130 */		   dr0 = dr1;
	/*  57:    */
		 }	/*  58:132 */		 return r1;
	/*  59:    */
	   }	/*  60:    */   
	/*  61:    */	   public override bool Equals(object other)
	/*  62:    */
	   {	/*  63:143 */		 if (other == null)
		 {
	/*  64:144 */		   return false;
	/*  65:    */
		 }	/*  66:147 */		 if (other == this)
		 {
	/*  67:148 */		   return true;
	/*  68:    */
		 }	/*  69:151 */		 if (!(other is Distortion))
		 {
	/*  70:152 */		   return false;
	/*  71:    */
		 }	/*  72:155 */		 Distortion o = (Distortion)other;
	/*  73:156 */		 return (this.mCoefficients[0] == o.mCoefficients[0]) && (this.mCoefficients[1] == o.mCoefficients[1]);
	/*  74:    */
	   }	/*  75:    */   
	/*  76:    */	   public override string ToString()
	/*  77:    */
	   {	/*  78:167 */		 return "Distortion {" + this.mCoefficients[0] + ", " + this.mCoefficients[1] + "}";
	/*  79:    */
	   }	/*  80:    */
	 }

	/* Location:           C:\Users\mattl_000.SURFACE\Downloads\cardboard.jar
	 * Qualified Name:     com.google.vrtoolkit.cardboard.Distortion
	 * JD-Core Version:    0.7.0.1
	 */
 }