/*   1:    */ namespace com.google.vrtoolkit.cardboard.sensors.@internal
 {
	/*   2:    */ 
	/*   3:    */	 public class Matrix3x3d
	/*   4:    */
	 {	/*   5: 12 */	   public double[] m = new double[9];
	/*   6:    */   
	/*   7:    */	   public Matrix3x3d()
	   {
	   }
	/*   8:    */   
	/*   9:    */	   public Matrix3x3d(double m00, double m01, double m02, double m10, double m11, double m12, double m20, double m21, double m22)
	/*  10:    */
	   {	/*  11: 30 */		 this.m[0] = m00;
	/*  12: 31 */		 this.m[1] = m01;
	/*  13: 32 */		 this.m[2] = m02;
	/*  14: 33 */		 this.m[3] = m10;
	/*  15: 34 */		 this.m[4] = m11;
	/*  16: 35 */		 this.m[5] = m12;
	/*  17: 36 */		 this.m[6] = m20;
	/*  18: 37 */		 this.m[7] = m21;
	/*  19: 38 */		 this.m[8] = m22;
	/*  20:    */
	   }	/*  21:    */   
	/*  22:    */	   public Matrix3x3d(Matrix3x3d o)
	/*  23:    */
	   {	/*  24: 47 */		 this.m[0] = o.m[0];
	/*  25: 48 */		 this.m[1] = o.m[1];
	/*  26: 49 */		 this.m[2] = o.m[2];
	/*  27: 50 */		 this.m[3] = o.m[3];
	/*  28: 51 */		 this.m[4] = o.m[4];
	/*  29: 52 */		 this.m[5] = o.m[5];
	/*  30: 53 */		 this.m[6] = o.m[6];
	/*  31: 54 */		 this.m[7] = o.m[7];
	/*  32: 55 */		 this.m[8] = o.m[8];
	/*  33:    */
	   }	/*  34:    */   
	/*  35:    */	   public virtual void set(double m00, double m01, double m02, double m10, double m11, double m12, double m20, double m21, double m22)
	/*  36:    */
	   {	/*  37: 68 */		 this.m[0] = m00;
	/*  38: 69 */		 this.m[1] = m01;
	/*  39: 70 */		 this.m[2] = m02;
	/*  40: 71 */		 this.m[3] = m10;
	/*  41: 72 */		 this.m[4] = m11;
	/*  42: 73 */		 this.m[5] = m12;
	/*  43: 74 */		 this.m[6] = m20;
	/*  44: 75 */		 this.m[7] = m21;
	/*  45: 76 */		 this.m[8] = m22;
	/*  46:    */
	   }	/*  47:    */   
	/*  48:    */	   public virtual void set(Matrix3x3d o)
	/*  49:    */
	   {	/*  50: 85 */		 this.m[0] = o.m[0];
	/*  51: 86 */		 this.m[1] = o.m[1];
	/*  52: 87 */		 this.m[2] = o.m[2];
	/*  53: 88 */		 this.m[3] = o.m[3];
	/*  54: 89 */		 this.m[4] = o.m[4];
	/*  55: 90 */		 this.m[5] = o.m[5];
	/*  56: 91 */		 this.m[6] = o.m[6];
	/*  57: 92 */		 this.m[7] = o.m[7];
	/*  58: 93 */		 this.m[8] = o.m[8];
	/*  59:    */
	   }	/*  60:    */   
	/*  61:    */	   public virtual void setZero()
	/*  62:    */
	   {	/*  63:100 */		 double tmp63_62 = (this.m[2] = this.m[3] = this.m[4] = this.m[5] = this.m[6] = this.m[7] = this.m[8] = 0.0D);
		 this.m[1] = tmp63_62;
		 this.m[0] = tmp63_62;
	/*  64:    */
	   }	/*  65:    */   
	/*  66:    */	   public virtual void setIdentity()
	/*  67:    */
	   {	/*  68:107 */		 double tmp41_40 = (this.m[3] = this.m[5] = this.m[6] = this.m[7] = 0.0D);
		 this.m[2] = tmp41_40;
		 this.m[1] = tmp41_40;
		 double tmp63_62 = (this.m[8] = 1.0D);
	/*  69:108 */
		 this.m[4] = tmp63_62;
		 this.m[0] = tmp63_62;
	/*  70:    */
	   }	/*  71:    */   
	/*  72:    */
	   public virtual double SameDiagonal
	   {
		   set
		/*  73:    */
		   {		/*  74:117 */			 double tmp19_18 = (this.m[8] = value);
			 this.m[4] = tmp19_18;
			 this.m[0] = tmp19_18;
		/*  75:    */
		   }
	   }	/*  76:    */   
	/*  77:    */	   public virtual double get(int row, int col)
	/*  78:    */
	   {	/*  79:128 */		 return this.m[(3 * row + col)];
	/*  80:    */
	   }	/*  81:    */   
	/*  82:    */	   public virtual void set(int row, int col, double value)
	/*  83:    */
	   {	/*  84:139 */		 this.m[(3 * row + col)] = value;
	/*  85:    */
	   }	/*  86:    */   
	/*  87:    */	   public virtual void getColumn(int col, Vector3d v)
	/*  88:    */
	   {	/*  89:149 */		 v.x = this.m[col];
	/*  90:150 */		 v.y = this.m[(col + 3)];
	/*  91:151 */		 v.z = this.m[(col + 6)];
	/*  92:    */
	   }	/*  93:    */   
	/*  94:    */	   public virtual void setColumn(int col, Vector3d v)
	/*  95:    */
	   {	/*  96:161 */		 this.m[col] = v.x;
	/*  97:162 */		 this.m[(col + 3)] = v.y;
	/*  98:163 */		 this.m[(col + 6)] = v.z;
	/*  99:    */
	   }	/* 100:    */   
	/* 101:    */	   public virtual void scale(double s)
	/* 102:    */
	   {	/* 103:172 */		 this.m[0] *= s;
	/* 104:173 */		 this.m[1] *= s;
	/* 105:174 */		 this.m[2] *= s;
	/* 106:175 */		 this.m[3] *= s;
	/* 107:176 */		 this.m[4] *= s;
	/* 108:177 */		 this.m[5] *= s;
	/* 109:178 */		 this.m[6] *= s;
	/* 110:179 */		 this.m[7] *= s;
	/* 111:180 */		 this.m[8] *= s;
	/* 112:    */
	   }	/* 113:    */   
	/* 114:    */	   public virtual void plusEquals(Matrix3x3d b)
	/* 115:    */
	   {	/* 116:189 */		 this.m[0] += b.m[0];
	/* 117:190 */		 this.m[1] += b.m[1];
	/* 118:191 */		 this.m[2] += b.m[2];
	/* 119:192 */		 this.m[3] += b.m[3];
	/* 120:193 */		 this.m[4] += b.m[4];
	/* 121:194 */		 this.m[5] += b.m[5];
	/* 122:195 */		 this.m[6] += b.m[6];
	/* 123:196 */		 this.m[7] += b.m[7];
	/* 124:197 */		 this.m[8] += b.m[8];
	/* 125:    */
	   }	/* 126:    */   
	/* 127:    */	   public virtual void minusEquals(Matrix3x3d b)
	/* 128:    */
	   {	/* 129:206 */		 this.m[0] -= b.m[0];
	/* 130:207 */		 this.m[1] -= b.m[1];
	/* 131:208 */		 this.m[2] -= b.m[2];
	/* 132:209 */		 this.m[3] -= b.m[3];
	/* 133:210 */		 this.m[4] -= b.m[4];
	/* 134:211 */		 this.m[5] -= b.m[5];
	/* 135:212 */		 this.m[6] -= b.m[6];
	/* 136:213 */		 this.m[7] -= b.m[7];
	/* 137:214 */		 this.m[8] -= b.m[8];
	/* 138:    */
	   }	/* 139:    */   
	/* 140:    */	   public virtual void transpose()
	/* 141:    */
	   {	/* 142:221 */		 double tmp = this.m[1];
	/* 143:222 */		 this.m[1] = this.m[3];
	/* 144:223 */		 this.m[3] = tmp;
	/* 145:    */     
	/* 146:225 */		 tmp = this.m[2];
	/* 147:226 */		 this.m[2] = this.m[6];
	/* 148:227 */		 this.m[6] = tmp;
	/* 149:    */     
	/* 150:229 */		 tmp = this.m[5];
	/* 151:230 */		 this.m[5] = this.m[7];
	/* 152:231 */		 this.m[7] = tmp;
	/* 153:    */
	   }	/* 154:    */   
	/* 155:    */	   public virtual void transpose(Matrix3x3d result)
	/* 156:    */
	   {	/* 157:240 */		 double m1 = this.m[1];
	/* 158:241 */		 double m2 = this.m[2];
	/* 159:242 */		 double m5 = this.m[5];
	/* 160:243 */		 result.m[0] = this.m[0];
	/* 161:244 */		 result.m[1] = this.m[3];
	/* 162:245 */		 result.m[2] = this.m[6];
	/* 163:246 */		 result.m[3] = m1;
	/* 164:247 */		 result.m[4] = this.m[4];
	/* 165:248 */		 result.m[5] = this.m[7];
	/* 166:249 */		 result.m[6] = m2;
	/* 167:250 */		 result.m[7] = m5;
	/* 168:251 */		 result.m[8] = this.m[8];
	/* 169:    */
	   }	/* 170:    */   
	/* 171:    */	   public static void add(Matrix3x3d a, Matrix3x3d b, Matrix3x3d result)
	/* 172:    */
	   {	/* 173:262 */		 a.m[0] += b.m[0];
	/* 174:263 */		 a.m[1] += b.m[1];
	/* 175:264 */		 a.m[2] += b.m[2];
	/* 176:265 */		 a.m[3] += b.m[3];
	/* 177:266 */		 a.m[4] += b.m[4];
	/* 178:267 */		 a.m[5] += b.m[5];
	/* 179:268 */		 a.m[6] += b.m[6];
	/* 180:269 */		 a.m[7] += b.m[7];
	/* 181:270 */		 a.m[8] += b.m[8];
	/* 182:    */
	   }	/* 183:    */   
	/* 184:    */	   public static void mult(Matrix3x3d a, Matrix3x3d b, Matrix3x3d result)
	/* 185:    */
	   {	/* 186:281 */		 result.set(a.m[0] * b.m[0] + a.m[1] * b.m[3] + a.m[2] * b.m[6], a.m[0] * b.m[1] + a.m[1] * b.m[4] + a.m[2] * b.m[7], a.m[0] * b.m[2] + a.m[1] * b.m[5] + a.m[2] * b.m[8], a.m[3] * b.m[0] + a.m[4] * b.m[3] + a.m[5] * b.m[6], a.m[3] * b.m[1] + a.m[4] * b.m[4] + a.m[5] * b.m[7], a.m[3] * b.m[2] + a.m[4] * b.m[5] + a.m[5] * b.m[8], a.m[6] * b.m[0] + a.m[7] * b.m[3] + a.m[8] * b.m[6], a.m[6] * b.m[1] + a.m[7] * b.m[4] + a.m[8] * b.m[7], a.m[6] * b.m[2] + a.m[7] * b.m[5] + a.m[8] * b.m[8]);
	/* 187:    */
	   }	/* 188:    */   
	/* 189:    */	   public static void mult(Matrix3x3d a, Vector3d v, Vector3d result)
	/* 190:    */
	   {	/* 191:300 */		 double x = a.m[0] * v.x + a.m[1] * v.y + a.m[2] * v.z;
	/* 192:301 */		 double y = a.m[3] * v.x + a.m[4] * v.y + a.m[5] * v.z;
	/* 193:302 */		 double z = a.m[6] * v.x + a.m[7] * v.y + a.m[8] * v.z;
	/* 194:303 */		 result.x = x;
	/* 195:304 */		 result.y = y;
	/* 196:305 */		 result.z = z;
	/* 197:    */
	   }	/* 198:    */   
	/* 199:    */	   public virtual double determinant()
	/* 200:    */
	   {	/* 201:314 */		 return get(0, 0) * (get(1, 1) * get(2, 2) - get(2, 1) * get(1, 2)) - get(0, 1) * (get(1, 0) * get(2, 2) - get(1, 2) * get(2, 0)) + get(0, 2) * (get(1, 0) * get(2, 1) - get(1, 1) * get(2, 0));
	/* 202:    */
	   }	/* 203:    */   
	/* 204:    */	   public virtual bool invert(Matrix3x3d result)
	/* 205:    */
	   {	/* 206:325 */		 double d = determinant();
	/* 207:326 */		 if (d == 0.0D)
		 {
	/* 208:327 */		   return false;
	/* 209:    */
		 }	/* 210:330 */		 double invdet = 1.0D / d;
	/* 211:    */     
	/* 212:332 */		 result.set((this.m[4] * this.m[8] - this.m[7] * this.m[5]) * invdet, -(this.m[1] * this.m[8] - this.m[2] * this.m[7]) * invdet, (this.m[1] * this.m[5] - this.m[2] * this.m[4]) * invdet, -(this.m[3] * this.m[8] - this.m[5] * this.m[6]) * invdet, (this.m[0] * this.m[8] - this.m[2] * this.m[6]) * invdet, -(this.m[0] * this.m[5] - this.m[3] * this.m[2]) * invdet, (this.m[3] * this.m[7] - this.m[6] * this.m[4]) * invdet, -(this.m[0] * this.m[7] - this.m[6] * this.m[1]) * invdet, (this.m[0] * this.m[4] - this.m[3] * this.m[1]) * invdet);
	/* 213:    */     
	/* 214:    */ 
	/* 215:    */ 
	/* 216:    */ 
	/* 217:    */ 
	/* 218:    */ 
	/* 219:    */ 
	/* 220:    */ 
	/* 221:    */ 
	/* 222:    */ 
	/* 223:    */ 
	/* 224:    */ 
	/* 225:345 */		 return true;
	/* 226:    */
	   }	/* 227:    */
	 }

	/* Location:           C:\Users\mattl_000.SURFACE\Downloads\cardboard.jar
	 * Qualified Name:     com.google.vrtoolkit.cardboard.sensors.internal.Matrix3x3d
	 * JD-Core Version:    0.7.0.1
	 */
 }