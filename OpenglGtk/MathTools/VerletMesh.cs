using System;
using System.Collections.Generic;

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
		public Vec3 Force { get; set; }

		public List<VertexConstraint> VertexConstraints{ get; private set; } //per gestire la mesh
		public List<Vec3> VertexOld { get; private set; }
		public List<Vec3> VertexNow { get { return VertexList; } set { VertexList = value; } }
		public IMatrix Model { get; set; }

		void GenerateVertexConstraintsFromFaces(){
			foreach (var f in Faces) {
				var v0 = f.posIndex [0];
				var v1 = f.posIndex [1];
				var v2 = f.posIndex [2];

				var v01 = VertexList [v1] - VertexList [v0];
				var v02 = VertexList [v2] - VertexList [v0];
				var v12 = VertexList [v2] - VertexList [v1];

				VertexConstraints.Add(new VertexConstraint(){Vertex0 = v0,Vertex1 = v1, distance = v01.Norm()});
				VertexConstraints.Add(new VertexConstraint(){Vertex0 = v0,Vertex1 = v2, distance = v02.Norm()});
				VertexConstraints.Add(new VertexConstraint(){Vertex0 = v1,Vertex1 = v2, distance = v12.Norm()});

			}
		}

		public void ApplyForce(Vec3 force){
			Force = Force + force;
		}
	
		public void PhysicStep(float dt){
		

			Vec3 acc = Force.Mult(1/Mass);

			Force.W = Force.W * 1.1f;
			var pNext = new List<Vec3> ();
			for (var i = 0; i<VertexNow.Count; i++) {

				var pNow = VertexNow [i];
				var pOld = VertexOld [i];
				pNext.Add (	pNow.IsColliding? pNow : pNow * 2 - pOld + acc * dt*dt);
			}
			VertexOld = VertexNow;
			VertexNow = pNext;


		}

		public void FixCollisionWithSphere(Vec3 sphereCenter, float sphereRadius){
			var offset = +0.25f;
			for (var i = 0; i<VertexList.Count; i++) {
			
				var v = new Vec3(Model.Direct.Dot(VertexList [i]));
				var diff = (v - sphereCenter);
				var distance = diff.Norm();
				if (distance > sphereRadius+offset) {
					VertexList [i].IsColliding = false;
					continue;
				}
				v = sphereCenter+(diff.Normalized () * (sphereRadius+offset));
				VertexList[i] = new Vec3 (Model.Inverse.Dot (v));
				VertexList [i].IsColliding = true;
			}

		}

		public void ApplyConstraints(float noise){
			foreach (var constraint in VertexConstraints) {
				var a = (Vec3)VertexList [constraint.Vertex0];
				var b = (Vec3)VertexList [constraint.Vertex1];

				var delta = (b - a);
				var currentDistance = delta.Norm();
				var diff = (currentDistance - constraint.distance) / currentDistance;



				VertexList [constraint.Vertex0] = a + delta * (0.5f * diff*noise);
				VertexList [constraint.Vertex1] = b - delta * (0.5f * diff*noise);

				VertexList [constraint.Vertex0].IsColliding = a.IsColliding;
				VertexList [constraint.Vertex1].IsColliding = b.IsColliding;


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
			VertexConstraints = new List<VertexConstraint> ();
			VertexOld = new List<Vec3> ();
			Force = new Vec3 (0, 0, 0);
			Mass = 0;
			Model = IMatrix.Identity ();
			foreach (var v in VertexNow)
				VertexOld.Add (v * 1);
			GenerateVertexConstraintsFromFaces ();
		}
	}
}

