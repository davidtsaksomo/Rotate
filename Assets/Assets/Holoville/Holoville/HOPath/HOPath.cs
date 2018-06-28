using Holoville.HOTween.Plugins;
using UnityEngine;
namespace Holoville.HOTween.Path {
	public class HOPath : MonoBehaviour {

		[SerializeField] private Vector3[] _points = new Vector3[2];
		[SerializeField] private PathType _pathType = PathType.Curved;
		[SerializeField] private bool _isClosed;
		[SerializeField] private bool _isRelative;


		private PathPreview _pathPreview; // Internal so that HOTween OnDrawGizmo can find it and draw the paths.

		private const int SUBDIVISIONS_MULTIPLIER = 16;
		private const float EPSILON = 0.001f; // Used for floating points comparison
		private Vector3 typedStartVal;
		private Vector3 diffChangeVal; // Used for incremental
		private Vector3[] _pathPointsInternal;
		private Transform _relativeTransform;
		private Vector3 _relativeOffset;
		public PlugVector3Path MakePlugVector3Path() {
			var p = new PlugVector3Path(Points, IsRelative, PathType);
			if(IsClosed) p.ClosePath();
			return p;
		}

		
		private void Awake() {
		}

		public void DrawPath() {
			SetChangeVal();
			_pathPreview.DrawPreview();
		}

		/// <summary>
		///     Adds the correct starting and ending point so the path can be reached from the property's actual position.
		/// </summary>
		private void SetChangeVal() {
			if(RelativeTransform == null) {
				RelativeTransform = transform;
			}

			// Create path.
			int indMod = 1;
			int pAdd = (_isClosed ? 1 : 0);
			int pointsLength = _points.Length;

			if(_isRelative) {
				_relativeOffset = _points[0] - RelativeTransform.position;
				// Path length is the same (plus control points).
				_pathPointsInternal = new Vector3[pointsLength + 2 + pAdd];
				for(int i = 0; i < pointsLength; ++i) _pathPointsInternal[i + indMod] = _points[i] - _relativeOffset;
			}
			else {
				Vector3 currVal = RelativeTransform.position;
				// Calculate if currVal and start point are equal,
				// managing floating point imprecision.
				Vector3 diff = currVal - _points[0];
				if(diff.x < 0) diff.x = -diff.x;
				if(diff.y < 0) diff.y = -diff.y;
				if(diff.z < 0) diff.z = -diff.z;
				if(diff.x < EPSILON && diff.y < EPSILON && diff.z < EPSILON) {
					// Path length is the same (plus control points).
					_pathPointsInternal = new Vector3[pointsLength + 2 + pAdd];
				}
				else {
					// Path needs additional point for current value as starting point (plus control points).
					_pathPointsInternal = new Vector3[pointsLength + 3 + pAdd];
					_pathPointsInternal[1] = currVal;
					indMod = 2;
				}
				for(int i = 0; i < pointsLength; ++i) {
					_pathPointsInternal[i + indMod] = _points[i];
				}
			}

			pointsLength = _pathPointsInternal.Length;

			if(_isClosed) {
				// Close path.
				_pathPointsInternal[pointsLength - 2] = _pathPointsInternal[1];
			}

			// Add control points.
			if(_isClosed) {
				_pathPointsInternal[0] = _pathPointsInternal[pointsLength - 3];
				_pathPointsInternal[pointsLength - 1] = _pathPointsInternal[2];
			}
			else {
				_pathPointsInternal[0] = _pathPointsInternal[1];
				Vector3 lastP = _pathPointsInternal[pointsLength - 2];
				Vector3 diffV = lastP - _pathPointsInternal[pointsLength - 3];
				_pathPointsInternal[pointsLength - 1] = lastP + diffV;
			}

			// Create the path.
			PathPreview = new PathPreview(_pathType, _pathPointsInternal);

			// Store arc lengths tables for constant speed.
			PathPreview.StoreTimeToLenTables(PathPreview.path.Length*SUBDIVISIONS_MULTIPLIER);

			if(!_isClosed) {
				// Store the changeVal used for Incremental loops
				diffChangeVal = _pathPointsInternal[pointsLength - 2] - _pathPointsInternal[1];
			}
		}

		/// <summary>
		///     Sets the correct values in case of Incremental loop type.
		/// </summary>
		/// <param name="p_diffIncr">
		///     The difference from the previous loop increment.
		/// </param>
		protected void SetIncremental(int p_diffIncr) {
			if(_isClosed) {
				return;
			}

			Vector3[] pathPs = PathPreview.path;
			int pathPsLength = pathPs.Length;
			for(int i = 0; i < pathPsLength; ++i) {
				pathPs[i] += (diffChangeVal*p_diffIncr);
			}
			PathPreview.changed = true;
		}

		public void AddPoint(int index) {
			var previous = Vector3.zero;
			var hasPrevious = index > 0;
			if(hasPrevious) {
				previous = Points[index - 1];
			}
			var next = Vector3.zero;
			var hasNext = index < Points.Length;
			if(hasNext) {
				next = Points[index];
			}
			var newNode = Vector3.zero;
			if(!hasPrevious) {
				newNode.Set(next.x - .5f, next.y - .5f, next.z);
			}
			else if(!hasNext) {
				newNode.Set(previous.x + .5f, previous.y + .5f, previous.z);
			}
			else {
				newNode = Vector3.Lerp(previous, next, .5f);
			}

			var old = Points;
			Points = new Vector3[old.Length + 1];
			// copy over the old array to the new array leaving a gap where the new element was insert
			System.Array.Copy(old, 0, Points, 0, index);
			System.Array.Copy(old, index, Points, index + 1, old.Length - index);
			Points[index] = newNode;
		}

		public void RemovePoint(int deleteIndex) {
			if(deleteIndex >= Points.Length || deleteIndex < 0) {
				Debug.LogError("Invalid material index");
				return;
			}
			if(Points.Length < 3) {
				Debug.LogError("Can't delete path point, min 2 points required.");
			}
			var old = Points;
			Points = new Vector3[old.Length - 1];
			// copy over the old array to the new array excepting the element at the delete index
			System.Array.Copy(old, 0, Points, 0, deleteIndex);
			System.Array.Copy(old, deleteIndex + 1, Points, deleteIndex, old.Length - deleteIndex - 1);
		}

		public PathType PathType {
			get { return _pathType; }
			set { _pathType = value; }
		}

		public bool IsClosed {
			get { return _isClosed; }
			set { _isClosed = value; }
		}

		public bool IsRelative {
			get { return _isRelative; }
			set { _isRelative = value; }
		}



		public Vector3[] Points {
			get { return _points; }
			private set { _points = value; }
		}

		private PathPreview PathPreview {
			get { return _pathPreview; }
			set { _pathPreview = value; }
		}

		public Vector3[] PathPointsInternal {
			get { return _pathPointsInternal; }
		}

		public Transform RelativeTransform {
			get { return _relativeTransform; }
			set { _relativeTransform = value; }
		}

		public Vector3 RelativeOffset {
			get { return _relativeOffset; }
		}
	}
}