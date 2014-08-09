/*  1:   */ package com.google.vrtoolkit.cardboard;
/*  2:   */ 
/*  3:   */ import android.opengl.Matrix;
/*  4:   */ 
/*  5:   */ public class EyeTransform
/*  6:   */ {
/*  7:   */   private final EyeParams mEyeParams;
/*  8:   */   private final float[] mEyeView;
/*  9:   */   private final float[] mPerspective;
/* 10:   */   
/* 11:   */   public EyeTransform(EyeParams params)
/* 12:   */   {
/* 13:36 */     this.mEyeParams = params;
/* 14:37 */     this.mEyeView = new float[16];
/* 15:38 */     this.mPerspective = new float[16];
/* 16:   */     
/* 17:40 */     Matrix.setIdentityM(this.mEyeView, 0);
/* 18:41 */     Matrix.setIdentityM(this.mPerspective, 0);
/* 19:   */   }
/* 20:   */   
/* 21:   */   public float[] getEyeView()
/* 22:   */   {
/* 23:59 */     return this.mEyeView;
/* 24:   */   }
/* 25:   */   
/* 26:   */   public float[] getPerspective()
/* 27:   */   {
/* 28:68 */     return this.mPerspective;
/* 29:   */   }
/* 30:   */   
/* 31:   */   public EyeParams getParams()
/* 32:   */   {
/* 33:77 */     return this.mEyeParams;
/* 34:   */   }
/* 35:   */ }


/* Location:           C:\Users\mattl_000.SURFACE\Downloads\cardboard.jar
 * Qualified Name:     com.google.vrtoolkit.cardboard.EyeTransform
 * JD-Core Version:    0.7.0.1
 */