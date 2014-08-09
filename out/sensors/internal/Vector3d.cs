using System;

/*   1:    */ namespace com.google.vrtoolkit.cardboard.sensors.@internal
 {
	/*   2:    */ 
	/*   3:    */	 public class Vector3d
	/*   4:    */
	 {	/*   5:    */	   public double x;
	/*   6:    */	   public double y;
	/*   7:    */	   public double z;
	/*   8:    */   
	/*   9:    */	   public Vector3d()
	   {
	   }
	/*  10:    */   
	/*  11:    */	   public Vector3d(double xx, double yy, double zz)
	/*  12:    */
	   {	/*  13: 29 */		 set(xx, yy, zz);
	/*  14:    */
	   }	/*  15:    */   
	/*  16:    */	   public virtual void set(double xx, double yy, double zz)
	/*  17:    */
	   {	/*  18: 40 */		 this.x = xx;
	/*  19: 41 */		 this.y = yy;
	/*  20: 42 */		 this.z = zz;
	/*  21:    */
	   }	/*  22:    */   
	/*  23:    */	   public virtual void setComponent(int i, double val)
	/*  24:    */
	   {	/*  25: 53 */		 if (i == 0)
		 {
	/*  26: 54 */		   this.x = val;
	/*  27: 55 */
		 }		 else if (i == 1)
		 {
	/*  28: 56 */		   this.y = val;
	/*  29:    */
		 }		 else
		 {
	/*  30: 58 */		   this.z = val;
	/*  31:    */
		 }	/*  32:    */
	   }	/*  33:    */   
	/*  34:    */	   public virtual void setZero()
	/*  35:    */
	   {	/*  36: 66 */		 this.x = (this.y = this.z = 0.0D);
	/*  37:    */
	   }	/*  38:    */   
	/*  39:    */	   public virtual void set(Vector3d other)
	/*  40:    */
	   {	/*  41: 75 */		 this.x = other.x;
	/*  42: 76 */		 this.y = other.y;
	/*  43: 77 */		 this.z = other.z;
	/*  44:    */
	   }	/*  45:    */   
	/*  46:    */	   public virtual void scale(double s)
	/*  47:    */
	   {	/*  48: 86 */		 this.x *= s;
	/*  49: 87 */		 this.y *= s;
	/*  50: 88 */		 this.z *= s;
	/*  51:    */
	   }	/*  52:    */   
	/*  53:    */	   public virtual void normalize()
	/*  54:    */
	   {	/*  55: 95 */		 double d = length();
	/*  56: 96 */		 if (d != 0.0D)
		 {
	/*  57: 97 */		   scale(1.0D / d);
	/*  58:    */
		 }	/*  59:    */
	   }	/*  60:    */   
	/*  61:    */	   public static double dot(Vector3d a, Vector3d b)
	/*  62:    */
	   {	/*  63:109 */		 return a.x * b.x + a.y * b.y + a.z * b.z;
	/*  64:    */
	   }	/*  65:    */   
	/*  66:    */	   public virtual double length()
	/*  67:    */
	   {	/*  68:118 */		 return Math.Sqrt(this.x * this.x + this.y * this.y + this.z * this.z);
	/*  69:    */
	   }	/*  70:    */   
	/*  71:    */	   public virtual bool sameValues(Vector3d other)
	/*  72:    */
	   {	/*  73:127 */		 return (this.x == other.x) && (this.y == other.y) && (this.z == other.z);
	/*  74:    */
	   }	/*  75:    */   
	/*  76:    */	   public static void sub(Vector3d a, Vector3d b, Vector3d result)
	/*  77:    */
	   {	/*  78:138 */		 result.set(a.x - b.x, a.y - b.y, a.z - b.z);
	/*  79:    */
	   }	/*  80:    */   
	/*  81:    */	   public static void cross(Vector3d a, Vector3d b, Vector3d result)
	/*  82:    */
	   {	/*  83:149 */		 result.set(a.y * b.z - a.z * b.y, a.z * b.x - a.x * b.z, a.x * b.y - a.y * b.x);
	/*  84:    */
	   }	/*  85:    */   
	/*  86:    */	   public static void ortho(Vector3d v, Vector3d result)
	/*  87:    */
	   {	/*  88:159 */		 int k = largestAbsComponent(v) - 1;
	/*  89:160 */		 if (k < 0)
		 {
	/*  90:161 */		   k = 2;
	/*  91:    */
		 }	/*  92:163 */		 result.setZero();
	/*  93:164 */		 result.setComponent(k, 1.0D);
	/*  94:    */     
	/*  95:166 */		 cross(v, result, result);
	/*  96:167 */		 result.normalize();
	/*  97:    */
	   }	/*  98:    */   
	/*  99:    */	   public static int largestAbsComponent(Vector3d v)
	/* 100:    */
	   {	/* 101:176 */		 double xAbs = Math.Abs(v.x);
	/* 102:177 */		 double yAbs = Math.Abs(v.y);
	/* 103:178 */		 double zAbs = Math.Abs(v.z);
	/* 104:180 */		 if (xAbs > yAbs)
	/* 105:    */
		 {	/* 106:181 */		   if (xAbs > zAbs)
		   {
	/* 107:182 */			 return 0;
	/* 108:    */
		   }	/* 109:184 */		   return 2;
	/* 110:    */
		 }	/* 111:187 */		 if (yAbs > zAbs)
		 {
	/* 112:188 */		   return 1;
	/* 113:    */
		 }	/* 114:190 */		 return 2;
	/* 115:    */
	   }	/* 116:    */
	 }

	/* Location:           C:\Users\mattl_000.SURFACE\Downloads\cardboard.jar
	 * Qualified Name:     com.google.vrtoolkit.cardboard.sensors.internal.Vector3d
	 * JD-Core Version:    0.7.0.1
	 */
 }