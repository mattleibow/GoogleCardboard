/*  1:   */ package com.google.vrtoolkit.cardboard;
/*  2:   */ 
/*  3:   */ import android.opengl.GLES20;
/*  4:   */ 
/*  5:   */ public class Viewport
/*  6:   */ {
/*  7:   */   public int x;
/*  8:   */   public int y;
/*  9:   */   public int width;
/* 10:   */   public int height;
/* 11:   */   
/* 12:   */   public void setViewport(int x, int y, int width, int height)
/* 13:   */   {
/* 14:39 */     this.x = x;
/* 15:40 */     this.y = y;
/* 16:41 */     this.width = width;
/* 17:42 */     this.height = height;
/* 18:   */   }
/* 19:   */   
/* 20:   */   public void setGLViewport()
/* 21:   */   {
/* 22:47 */     GLES20.glViewport(this.x, this.y, this.width, this.height);
/* 23:   */   }
/* 24:   */   
/* 25:   */   public void setGLScissor()
/* 26:   */   {
/* 27:52 */     GLES20.glScissor(this.x, this.y, this.width, this.height);
/* 28:   */   }
/* 29:   */   
/* 30:   */   public void getAsArray(int[] array, int offset)
/* 31:   */   {
/* 32:64 */     if (offset + 4 > array.length) {
/* 33:65 */       throw new IllegalArgumentException("Not enough space to write the result");
/* 34:   */     }
/* 35:68 */     array[offset] = this.x;
/* 36:69 */     array[(offset + 1)] = this.y;
/* 37:70 */     array[(offset + 2)] = this.width;
/* 38:71 */     array[(offset + 3)] = this.height;
/* 39:   */   }
/* 40:   */   
/* 41:   */   public String toString()
/* 42:   */   {
/* 43:81 */     return "Viewport {x:" + this.x + " y:" + this.y + " width:" + this.width + " height:" + this.height + "}";
/* 44:   */   }
/* 45:   */ }


/* Location:           C:\Users\mattl_000.SURFACE\Downloads\cardboard.jar
 * Qualified Name:     com.google.vrtoolkit.cardboard.Viewport
 * JD-Core Version:    0.7.0.1
 */