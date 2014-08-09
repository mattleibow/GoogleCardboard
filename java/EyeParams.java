/*  1:   */ package com.google.vrtoolkit.cardboard;
/*  2:   */ 
/*  3:   */ public class EyeParams
/*  4:   */ {
/*  5:   */   private final int mEye;
/*  6:   */   private final Viewport mViewport;
/*  7:   */   private final FieldOfView mFov;
/*  8:   */   private final EyeTransform mEyeTransform;
/*  9:   */   
/* 10:   */   public EyeParams(int eye)
/* 11:   */   {
/* 12:52 */     this.mEye = eye;
/* 13:53 */     this.mViewport = new Viewport();
/* 14:54 */     this.mFov = new FieldOfView();
/* 15:55 */     this.mEyeTransform = new EyeTransform(this);
/* 16:   */   }
/* 17:   */   
/* 18:   */   public int getEye()
/* 19:   */   {
/* 20:64 */     return this.mEye;
/* 21:   */   }
/* 22:   */   
/* 23:   */   public Viewport getViewport()
/* 24:   */   {
/* 25:73 */     return this.mViewport;
/* 26:   */   }
/* 27:   */   
/* 28:   */   public FieldOfView getFov()
/* 29:   */   {
/* 30:82 */     return this.mFov;
/* 31:   */   }
/* 32:   */   
/* 33:   */   public EyeTransform getTransform()
/* 34:   */   {
/* 35:91 */     return this.mEyeTransform;
/* 36:   */   }
/* 37:   */   
/* 38:   */   public static class Eye
/* 39:   */   {
/* 40:   */     public static final int MONOCULAR = 0;
/* 41:   */     public static final int LEFT = 1;
/* 42:   */     public static final int RIGHT = 2;
/* 43:   */   }
/* 44:   */ }


/* Location:           C:\Users\mattl_000.SURFACE\Downloads\cardboard.jar
 * Qualified Name:     com.google.vrtoolkit.cardboard.EyeParams
 * JD-Core Version:    0.7.0.1
 */