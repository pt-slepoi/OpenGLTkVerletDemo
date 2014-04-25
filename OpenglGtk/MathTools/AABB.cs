using System;
namespace MathTools
{
	public class AABB
	{

		private Vec3 min, max;

		public AABB(){
			IsEmpty = true;
		}

		public bool IsEmpty{ get; set; }

		public void Add(Vec3 p){
			if(IsEmpty){
				max = new Vec3(p.X,p.Y,p.Z);
				min =new Vec3(p.X,p.Y,p.Z); 
			}

			else{
				if(p.X < min.X) min.X=p.X;
				if(p.Y < min.Y) min.Y=p.Y;
				if(p.Z < min.Z) min.Z=p.Z;

				if(p.X > max.X) max.X=p.X;
				if(p.Y > max.Y) max.Y=p.Y;
				if(p.Z > max.Z) max.Z=p.Z;
			}
			IsEmpty = false;
		}

		public void Clear(){
			IsEmpty = true;
		}

		/**
	 * @return punto centrale dell'AABB
	 */
		public Vec3 Center(){
			Vec3 res = min.Add(max);
			res = res.Mult(0.5f);
			return res;
		}

		/**
	 * @return raggio dell'AABB 
	 */
		public float Radius(){
			Vec3 res = min.Sub(max);
			return 0.5f * res.Norm(); 
		}

		/**
	 * @return Vec3 con max x,y,z
	 */
		public Vec3 Max(){
			return new Vec3(max);
		}

		/**
	 * @return Vec3 con min x,y,z
	 */
		public Vec3 Min(){
			return new Vec3(min);
		}

	}
}

