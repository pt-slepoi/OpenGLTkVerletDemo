using System;
using System.Collections.Generic;
using System;
using System.Collections.Generic;

namespace MathTools
{
	public struct VertexConstraint{
		public int Vertex0 { get; set; }
		public int Vertex1 { get; set; }
		public int Vertex2 { get; set; }
		public float Distance01 { get; set; }
		public float Distance02 { get; set; }
		public float Distance12 { get; set; }
	}

	public class VerletMesh : Mesh
	{
		public float Mass { get; set; }
		public Vec4 Force { get; set; }

		public List<VertexConstraint> VertexConstatints{ get; private set; } //per gestire la mesh
		public List<Vec3> VertexOld { get; private set; }
		public List<Vec3> VertexNow { get { return VertexList; } set { VertexList = value; } } 

		void GenerateVertexConstraintsFromFaces(){
			foreach (var f in Faces) {
				var v0 = f.posIndex [0];
				var v1 = f.posIndex [1];
				var v2 = f.posIndex [2];

				var v01 = VertexList [v1] - VertexList [v0];
				var v02 = VertexList [v2] - VertexList [v0];
				var v12 = VertexList [v2] - VertexList [v1];

				VertexConstatints.Add(new VertexConstraint(){
					Vertex0 = v0,
					Vertex1 = v1,
					Vertex2 = v2,
					Distance01 = v01.Norm(),
					Distance02 = v02.Norm(),
					Distance12 = v12.Norm(),
				});


			}
		}

		public void ApplyForce(Vec4 force){
			Force = Force + force;
			Console.WriteLine ("Applied force" + Force.ToString ());
		}

		public void UpdatePosition(float dt){

			Vec3 acc = new Vec3 (Force.X, Force.Y, Force.Z);
			acc.Normalize();
			acc = acc.Mult(Force.W);
			acc = new Vec3(acc.Mult (Mass));

			var pNext = new List<Vec3> ();
			for (var i = 0; i<VertexNow.Count; i++) {
				var pNow = VertexNow [i];
				var pOld = VertexOld [i];
				pNext.Add (pNow * 2 - pOld + acc * dt*dt);
			}
			VertexOld = VertexNow;
			VertexNow = pNext;


		}

		public void FixCollisionWithSphere(Vec3 sphereCenter, float sphereRadius){
			for (var i = 0; i<VertexList.Count; i++) {
				var v = VertexList [i];
				var diff = (v - sphereCenter);
				var distance = diff.Norm();
				if (distance > sphereRadius)
					continue;

				VertexList [i] = v+diff;
			}
		}

		public void FixZCollision(float z){
			if (true)
				return;
			foreach (var v in VertexList)
				if (v.Z - z < 0)
					v.Z = z;
		}



		public void ApplyConstraints(float noise){
			foreach (var constraint in VertexConstatints) {
				var v0 = (Vec3)VertexList [constraint.Vertex0];
				var v1 = (Vec3)VertexList [constraint.Vertex1];
				var v2 = (Vec3)VertexList [constraint.Vertex2];

				var delta = v1 - v0;

				var currentDistance = delta.Norm();
				var diff = (currentDistance - constraint.Distance01) / currentDistance;

				v0 = v0 + delta * (0.5f * diff*noise);
				v1 = v1 - delta * (0.5f * diff*noise); 

				delta = v2 - v1;

				currentDistance = delta.Norm();
				diff = (currentDistance - constraint.Distance12) / currentDistance;

				v1 = v1 + delta * (0.5f * diff*noise);
				v2 = v2 - delta * (0.5f * diff*noise);

				delta = v2 - v0;

				currentDistance = delta.Norm();
				diff = (currentDistance - constraint.Distance02) / currentDistance;

				v0 = v0 + delta * (0.5f * diff*noise);
				v2 = v2 - delta * (0.5f * diff*noise);

				VertexList [constraint.Vertex0] = v0;
				VertexList [constraint.Vertex1] = v1;
				VertexList [constraint.Vertex2] = v2;

			}
		}
		public void ApplyConstraints(){
			ApplyConstraints (1.0f);
		}
		public VerletMesh ():base(){
			init ();
		}
		public VerletMesh(Mesh m){

			Faces = m.Faces;
			VertexList = m.VertexList;
			Normals = m.Normals;
			init ();
		}
		void init(){
			VertexConstatints = new List<VertexConstraint> ();
			VertexOld = new List<Vec3> ();
			Force = new Vec4 (0, 0, 0, 1);
			Mass = 0;
			foreach (var v in VertexNow)
				VertexOld.Add (v * 1);
			GenerateVertexConstraintsFromFaces ();
		}
	}
}


namespace MathTools
{
	public struct VertexConstraint{
		public int Vertex0 { get; set; }
		public int Vertex1 { get; set; }
		public float distance { get; set; }
	}

	public class VerletMesh : Mesh
	{
		public float Mass { get; set; }
		public Vec4 Force { get; set; }

		public List<VertexConstraint> VertexConstatints{ get; private set; } //per gestire la mesh
		public List<Vec3> VertexOld { get; private set; }
		public List<Vec3> VertexNow { get { return VertexList; } set { VertexList = value; } } 

		void GenerateVertexConstraintsFromFaces(){
			foreach (var f in Faces) {
				var v0 = f.posIndex [0];
				var v1 = f.posIndex [1];
				var v2 = f.posIndex [2];

				var v01 = VertexList [v1] - VertexList [v0];
				var v02 = VertexList [v2] - VertexList [v0];
				var v12 = VertexList [v2] - VertexList [v1];

				VertexConstatints.Add(new VertexConstraint(){Vertex0 = v0,Vertex1 = v1, distance = v01.Norm()});
				VertexConstatints.Add(new VertexConstraint(){Vertex0 = v0,Vertex1 = v2, distance = v02.Norm()});
				VertexConstatints.Add(new VertexConstraint(){Vertex0 = v1,Vertex1 = v2, distance = v12.Norm()});

			}
		}

		public void ApplyForce(Vec4 force){
			Force = Force + force;
			Console.WriteLine ("Applied force" + Force.ToString ());
		}

		public void UpdatePosition(float dt){

			Vec3 acc = new Vec3 (Force.X, Force.Y, Force.Z);
			acc.Normalize();
			acc = acc.Mult(Force.W);
			acc = new Vec3(acc.Mult (Mass));

			var pNext = new List<Vec3> ();
			for (var i = 0; i<VertexNow.Count; i++) {
				var pNow = VertexNow [i];
				var pOld = VertexOld [i];
				pNext.Add (pNow * 2 - pOld + acc * dt*dt);
			}
			VertexOld = VertexNow;
			VertexNow = pNext;


		}

		public void FixCollisionWithSphere(Vec3 sphereCenter, float sphereRadius){
			for (var i = 0; i<VertexList.Count; i++) {
				var v = VertexList [i];
				var diff = (v - sphereCenter);
				var distance = diff.Norm();
				if (distance > sphereRadius)
					continue;

				VertexList [i] = v+diff;
			}
		}

		public void FixZCollision(float z){
			if (true)
				return;
			foreach (var v in VertexList)
				if (v.Z - z < 0)
					v.Z = z;
		}



		public void ApplyConstraints(float noise){
			foreach (var constraint in VertexConstatints) {
				var v0 = (Vec3)VertexList [constraint.Vertex0];
				var v1 = (Vec3)VertexList [constraint.Vertex1];

				var delta = (v1 - v0);
				var currentDistance = delta.Norm();
				var diff = (currentDistance - constraint.distance) / currentDistance;

				VertexList [constraint.Vertex0] = v0 + delta * (0.5f * diff*noise);
				VertexList [constraint.Vertex1] = v1 - delta * (0.5f * diff*noise);  

			}
		}
		public void ApplyConstraints(){
			ApplyConstraints (1.0f);
		}
		public VerletMesh ():base(){
			init ();
		}
		public VerletMesh(Mesh m){

			Faces = m.Faces;
			VertexList = m.VertexList;
			Normals = m.Normals;
			init ();
		}
		void init(){
			VertexConstatints = new List<VertexConstraint> ();
			VertexOld = new List<Vec3> ();
			Force = new Vec4 (0, 0, 0, 1);
			Mass = 0;
			foreach (var v in VertexNow)
				VertexOld.Add (v * 1);
			GenerateVertexConstraintsFromFaces ();
		}
	}
}

