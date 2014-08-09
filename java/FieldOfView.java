/*   1:    */ package com.google.vrtoolkit.cardboard;
/*   2:    */ 
/*   3:    */ import android.opengl.Matrix;
/*   4:    */ 
/*   5:    */ public class FieldOfView
/*   6:    */ {
/*   7:    */   private float mLeft;
/*   8:    */   private float mRight;
/*   9:    */   private float mBottom;
/*  10:    */   private float mTop;
/*  11:    */   
/*  12:    */   public FieldOfView() {}
/*  13:    */   
/*  14:    */   public FieldOfView(float left, float right, float bottom, float top)
/*  15:    */   {
/*  16: 43 */     this.mLeft = left;
/*  17: 44 */     this.mRight = right;
/*  18: 45 */     this.mBottom = bottom;
/*  19: 46 */     this.mTop = top;
/*  20:    */   }
/*  21:    */   
/*  22:    */   public FieldOfView(FieldOfView other)
/*  23:    */   {
/*  24: 55 */     this.mLeft = other.mLeft;
/*  25: 56 */     this.mRight = other.mRight;
/*  26: 57 */     this.mBottom = other.mBottom;
/*  27: 58 */     this.mTop = other.mTop;
/*  28:    */   }
/*  29:    */   
/*  30:    */   public void setLeft(float left)
/*  31:    */   {
/*  32: 67 */     this.mLeft = left;
/*  33:    */   }
/*  34:    */   
/*  35:    */   public float getLeft()
/*  36:    */   {
/*  37: 76 */     return this.mLeft;
/*  38:    */   }
/*  39:    */   
/*  40:    */   public void setRight(float right)
/*  41:    */   {
/*  42: 85 */     this.mRight = right;
/*  43:    */   }
/*  44:    */   
/*  45:    */   public float getRight()
/*  46:    */   {
/*  47: 94 */     return this.mRight;
/*  48:    */   }
/*  49:    */   
/*  50:    */   public void setBottom(float bottom)
/*  51:    */   {
/*  52:103 */     this.mBottom = bottom;
/*  53:    */   }
/*  54:    */   
/*  55:    */   public float getBottom()
/*  56:    */   {
/*  57:112 */     return this.mBottom;
/*  58:    */   }
/*  59:    */   
/*  60:    */   public void setTop(float top)
/*  61:    */   {
/*  62:121 */     this.mTop = top;
/*  63:    */   }
/*  64:    */   
/*  65:    */   public float getTop()
/*  66:    */   {
/*  67:130 */     return this.mTop;
/*  68:    */   }
/*  69:    */   
/*  70:    */   public void toPerspectiveMatrix(float near, float far, float[] perspective, int offset)
/*  71:    */   {
/*  72:144 */     if (offset + 16 > perspective.length) {
/*  73:145 */       throw new IllegalArgumentException("Not enough space to write the result");
/*  74:    */     }
/*  75:148 */     float l = (float)-Math.tan(Math.toRadians(this.mLeft)) * near;
/*  76:149 */     float r = (float)Math.tan(Math.toRadians(this.mRight)) * near;
/*  77:150 */     float b = (float)-Math.tan(Math.toRadians(this.mBottom)) * near;
/*  78:151 */     float t = (float)Math.tan(Math.toRadians(this.mTop)) * near;
/*  79:152 */     Matrix.frustumM(perspective, offset, l, r, b, t, near, far);
/*  80:    */   }
/*  81:    */   
/*  82:    */   public boolean equals(Object other)
/*  83:    */   {
/*  84:163 */     if (other == null) {
/*  85:164 */       return false;
/*  86:    */     }
/*  87:167 */     if (other == this) {
/*  88:168 */       return true;
/*  89:    */     }
/*  90:171 */     if (!(other instanceof FieldOfView)) {
/*  91:172 */       return false;
/*  92:    */     }
/*  93:175 */     FieldOfView o = (FieldOfView)other;
/*  94:176 */     return (this.mLeft == o.mLeft) && (this.mRight == o.mRight) && (this.mBottom == o.mBottom) && (this.mTop == o.mTop);
/*  95:    */   }
/*  96:    */   
/*  97:    */   public String toString()
/*  98:    */   {
/*  99:186 */     return "FieldOfView {left:" + this.mLeft + " right:" + this.mRight + " bottom:" + this.mBottom + " top:" + this.mTop + "}";
/* 100:    */   }
/* 101:    */ }


/* Location:           C:\Users\mattl_000.SURFACE\Downloads\cardboard.jar
 * Qualified Name:     com.google.vrtoolkit.cardboard.FieldOfView
 * JD-Core Version:    0.7.0.1
 */