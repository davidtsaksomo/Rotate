using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Holoville.HOTween.Path {

	/// <summary>
	///     Used to manage movement on a Cardinal spline (of Catmull-Rom type).
	///     Contains code from Andeeee's CRSpline
	///     (http://forum.unity3d.com/threads/32954-Waypoints-and-constant-variable-speed-problems).
	/// </summary>
	internal class PathPreview {
		// VARS ///////////////////////////////////////////////////

		public float pathLength; // Stored when storing time and length tables.
		public float[] waypointsLength; // Length of each waypoint, excluding control points

		public float[] timesTable; // Connected to lengthsTable, used for constant speed calculations
		private float[] lengthsTable; // Connected to timesTable, used for constant speed calculations

		internal Vector3[] path;
		internal bool changed; // Used by incremental loops to tell that drawPs should be recalculated.

		private Vector3[] drawPs; // Used by GizmoDraw to store point only once.
		private readonly PathType pathType;


		// ***********************************************************************************
		// CONSTRUCTOR
		// ***********************************************************************************

		/// <summary>
		///     Creates a new <see cref="PathPreview" /> based on the given array of <see cref="Vector3" /> points.
		/// </summary>
		/// <param name="p_type">Type of path</param>
		/// <param name="p_path">
		///     The <see cref="Vector3" /> array used to create the path.
		/// </param>
		public PathPreview(PathType p_type, params Vector3[] p_path) {
			pathType = p_type;
			path = new Vector3[p_path.Length];
			Array.Copy(p_path, path, path.Length);
		}

		// ===================================================================================
		// METHODS ---------------------------------------------------------------------------

		/// <summary>
		///     Gets the point on the path at the given percentage (0 to 1).
		/// </summary>
		/// <param name="t">
		///     The percentage (0 to 1) at which to get the point.
		/// </param>
		private Vector3 GetPoint(float t) {
			int tmp;
			return GetPoint(t, out tmp);
		}

		/// <summary>
		///     Gets the point on the path at the given percentage (0 to 1).
		/// </summary>
		/// <param name="t">
		///     The percentage (0 to 1) at which to get the point.
		/// </param>
		/// <param name="out_waypointIndex">
		///     Index of waypoint we're moving to (or where we are). Only used for Linear paths.
		/// </param>
		private Vector3 GetPoint(float t, out int out_waypointIndex) {
			switch(pathType) {
				case PathType.Linear:
					if(t <= 0) {
						out_waypointIndex = 1;
						return path[1];
					}
					int startPIndex = 0;
					int endPIndex = 0;
					int len = timesTable.Length;
					for(int i = 1; i < len; i++) {
						if(timesTable[i] >= t) {
							startPIndex = i - 1;
							endPIndex = i;
							break;
						}
					}
					float startPPerc = timesTable[startPIndex];
					float partialPerc = timesTable[endPIndex] - timesTable[startPIndex];
					partialPerc = t - startPPerc;
					float partialLen = pathLength*partialPerc;
					Vector3 wp0 = path[startPIndex];
					Vector3 wp1 = path[endPIndex];
					out_waypointIndex = endPIndex;
					return wp0 + Vector3.ClampMagnitude(wp1 - wp0, partialLen);
				default: // Curved
					int numSections = path.Length - 3;
					int tSec = (int) Math.Floor(t*numSections);
					int currPt = numSections - 1;
					if(currPt > tSec) {
						currPt = tSec;
					}
					float u = t*numSections - currPt;

					Vector3 a = path[currPt];
					Vector3 b = path[currPt + 1];
					Vector3 c = path[currPt + 2];
					Vector3 d = path[currPt + 3];

//                out_waypointIndex = -1;
					out_waypointIndex = 0 + currPt; // -- ak mod so that we always know the index point
					return .5f*(
						(-a + 3f*b - 3f*c + d)*(u*u*u)
						+ (2f*a - 5f*b + 4f*c - d)*(u*u)
						+ (-a + c)*u
						+ 2f*b
						);
			}
		}


		/// <summary>
		///     Draws the full path.
		/// </summary>
		/// <param name="lineRenderer"></param>
		public void DrawPreview() {
			DrawPreview(-1, false);
		}

		/// <summary>
		///     Draws the full path, and if <c>t</c> is not -1 also draws the velocity at <c>t</c>.
		/// </summary>
		/// <param name="t">
		///     The point where to calculate velocity and eventual additional trigonometry.
		/// </param>
		/// <param name="p_drawTrig">
		///     If <c>true</c> also draws the normal, tangent, and binormal of t.
		/// </param>
		/// <param name="lineRenderer"></param>
		private void DrawPreview(float t, bool p_drawTrig) {
			//Gizmos.color = new Color(1, 0.3f, 0.3f, 0.6f);

			Vector3 currPt;
			if(changed || pathType == PathType.Curved && drawPs == null) {
				changed = false;
				if(pathType == PathType.Curved) {
					// Store draw points.
					int subdivisions = path.Length*10;
					drawPs = new Vector3[subdivisions + 1];
					for(int i = 0; i <= subdivisions; ++i) {
						float pm = i/(float) subdivisions;
						currPt = GetPoint(pm);
						drawPs[i] = currPt;
					}
				}
			}
			// Draw path.
#if UNITY_EDITOR
			Vector3 prevPt;
			switch(pathType) {
				case PathType.Linear:
					prevPt = path[1];
					int len = path.Length;
					for(int i = 1; i < len - 1; ++i) {
						currPt = path[i];
						Handles.DrawLine(currPt, prevPt);
						prevPt = currPt;
					}
					break;
				default: // Curved
					prevPt = drawPs[0];
					int drawPsLength = drawPs.Length;
					for(int i = 1; i < drawPsLength; ++i) {
						currPt = drawPs[i];
						Handles.DrawLine(currPt, prevPt);
						prevPt = currPt;
					}
					break;
			}
#endif
			// Draw path control points.
			//Gizmos.color = Color.white;
			int pathLength = path.Length - 1;
			for(int i = 1; i < pathLength; ++i) {
				//Gizmos.DrawSphere(path[i], 0.1f);
			}

			if(p_drawTrig && t != -1) {
				Vector3 pos = GetPoint(t);
				Vector3 prevP;
				Vector3 p = pos;
				Vector3 nextP;
				float nextT = t + 0.0001f;
				if(nextT > 1) {
					nextP = pos;
					p = GetPoint(t - 0.0001f);
					prevP = GetPoint(t - 0.0002f);
				}
				else {
					float prevT = t - 0.0001f;
					if(prevT < 0) {
						prevP = pos;
						p = GetPoint(t + 0.0001f);
						nextP = GetPoint(t + 0.0002f);
					}
					else {
						prevP = GetPoint(prevT);
						nextP = GetPoint(nextT);
					}
				}
				Vector3 tangent = nextP - p;
				tangent.Normalize();
				Vector3 tangent2 = p - prevP;
				tangent2.Normalize();
				Vector3 normal = Vector3.Cross(tangent, tangent2);
				normal.Normalize();
				Vector3 binormal = Vector3.Cross(tangent, normal);
				binormal.Normalize();
				// Draw normal.
				//Gizmos.color = Color.black;
				//Gizmos.DrawLine(pos, pos + tangent);
				//Gizmos.color = Color.blue;
				//Gizmos.DrawLine(pos, pos + normal);
				//Gizmos.color = Color.red;
				//Gizmos.DrawLine(pos, pos + binormal);
			}
		}


		// If path is linear, p_subdivisions is ignored,
		// and waypointsLength are stored here instead than when calling StoreWaypointsLengths
		public void StoreTimeToLenTables(int p_subdivisions) {
			Vector3 prevP;
			Vector3 currP;
			float incr;
			switch(pathType) {
				case PathType.Linear:
					pathLength = 0;
					int pathCount = path.Length;
					waypointsLength = new float[pathCount];
					prevP = path[1];
					for(int i = 1; i < pathCount; i++) {
						currP = path[i];
						float dist = Vector3.Distance(currP, prevP);
						if(i < pathCount - 1) pathLength += dist;
						prevP = currP;
						waypointsLength[i] = dist;
					}
					timesTable = new float[pathCount];
					float tmpLen = 0;
					for(int i = 2; i < pathCount; i++) {
						tmpLen += waypointsLength[i];
						timesTable[i] = tmpLen/pathLength;
					}
					break;
				default: // Curved
					pathLength = 0;
					incr = 1f/p_subdivisions;
					timesTable = new float[p_subdivisions];
					lengthsTable = new float[p_subdivisions];
					prevP = GetPoint(0);
					for(int i = 1; i < p_subdivisions + 1; ++i) {
						float perc = incr*i;
						currP = GetPoint(perc);
						pathLength += Vector3.Distance(currP, prevP);
						prevP = currP;
						timesTable[i - 1] = perc;
						lengthsTable[i - 1] = pathLength;
					}
					break;
			}
		}
	}
}