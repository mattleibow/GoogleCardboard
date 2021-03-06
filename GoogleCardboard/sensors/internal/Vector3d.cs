﻿using System;

 namespace com.google.vrtoolkit.cardboard.sensors.@internal
 {
	 
		 public class Vector3d
	
	 {		   public double x;
		   public double y;
		   public double z;
	   
		   public Vector3d()
	   {
	   }
	   
		   public Vector3d(double xx, double yy, double zz)
	
	   {			 set(xx, yy, zz);
	
	   }	   
		   public virtual void set(double xx, double yy, double zz)
	
	   {			 this.x = xx;
			 this.y = yy;
			 this.z = zz;
	
	   }	   
		   public virtual void setComponent(int i, double val)
	
	   {			 if (i == 0)
		 {
			   this.x = val;
	
		 }		 else if (i == 1)
		 {
			   this.y = val;
	
		 }		 else
		 {
			   this.z = val;
	
		 }	
	   }	   
		   public virtual void setZero()
	
	   {			 this.x = (this.y = this.z = 0.0D);
	
	   }	   
		   public virtual void set(Vector3d other)
	
	   {			 this.x = other.x;
			 this.y = other.y;
			 this.z = other.z;
	
	   }	   
		   public virtual void scale(double s)
	
	   {			 this.x *= s;
			 this.y *= s;
			 this.z *= s;
	
	   }	   
		   public virtual void normalize()
	
	   {			 double d = length();
			 if (d != 0.0D)
		 {
			   scale(1.0D / d);
	
		 }	
	   }	   
		   public static double dot(Vector3d a, Vector3d b)
	
	   {			 return a.x * b.x + a.y * b.y + a.z * b.z;
	
	   }	   
		   public virtual double length()
	
	   {			 return Math.Sqrt(this.x * this.x + this.y * this.y + this.z * this.z);
	
	   }	   
		   public virtual bool sameValues(Vector3d other)
	
	   {			 return (this.x == other.x) && (this.y == other.y) && (this.z == other.z);
	
	   }	   
		   public static void sub(Vector3d a, Vector3d b, Vector3d result)
	
	   {			 result.set(a.x - b.x, a.y - b.y, a.z - b.z);
	
	   }	   
		   public static void cross(Vector3d a, Vector3d b, Vector3d result)
	
	   {			 result.set(a.y * b.z - a.z * b.y, a.z * b.x - a.x * b.z, a.x * b.y - a.y * b.x);
	
	   }	   
		   public static void ortho(Vector3d v, Vector3d result)
	
	   {			 int k = largestAbsComponent(v) - 1;
			 if (k < 0)
		 {
			   k = 2;
	
		 }			 result.setZero();
			 result.setComponent(k, 1.0D);
	     
			 cross(v, result, result);
			 result.normalize();
	
	   }	   
		   public static int largestAbsComponent(Vector3d v)
	
	   {			 double xAbs = Math.Abs(v.x);
			 double yAbs = Math.Abs(v.y);
			 double zAbs = Math.Abs(v.z);
			 if (xAbs > yAbs)
	
		 {			   if (xAbs > zAbs)
		   {
				 return 0;
	
		   }			   return 2;
	
		 }			 if (yAbs > zAbs)
		 {
			   return 1;
	
		 }			 return 2;
	
	   }	
	 }

	/* Location:           C:\Users\mattl_000.SURFACE\Downloads\cardboard.jar
	 * Qualified Name:     com.google.vrtoolkit.cardboard.sensors.internal.Vector3d
	 * JD-Core Version:    0.7.0.1
	 */
 }