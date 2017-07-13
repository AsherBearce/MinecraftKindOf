using Microsoft.Xna.Framework;

namespace PlaneGame2.Instances
{
    /// <summary> A class for all objects needing positional data </summary>
    public class GameObject : Instance
    {
        #region PRIVATE FIELDS
        /// <summary> The internal local transform </summary>
        private Matrix localTransform;

        /// <summary> The internal rotation matrix </summary>
        private Matrix rotation;
        private Vector3 position;
        private Vector3 scale;
        #endregion

        #region PUBLIC PROPERTIES
        /// <summary> The transform of this GameObject in local space </summary>
        public Matrix LocalTransform { get { return localTransform; } set { localTransform = value; } }

        /// <summary> The transform of this GameObject in global space </summary>
        public Matrix GlobalTransform { get { return ((Parent is GameObject p) ? p.GlobalTransform : Matrix.Identity) * LocalTransform; } }

        /// <summary> the rotation of this GameObject </summary>
        public Matrix Rotation
        {
            get { return rotation; }
            set
            {
                //Sets the value's scale and translation to default, so we just get the rotational part
                //value.Scale = new Vector3(1);
                //value.Translation = Vector3.Zero;//These things cause problems, don't use them.

                //Sets the rotation
                rotation = value;
                updateTransform();
                //Sets the rotation of localTransform
                //localTransform = Matrix.CreateTranslation(Position) * rotation;
            }
        }

        /// <summary> The position of this GameObject </summary>
        //public Vector3 Position { get { return localTransform.Translation; } set { localTransform.Translation = value; } }
        public Vector3 Position
        {
            get { return position; }
            set
            {
                position = value;
                //localTransform.Translation = value;
                updateTransform();
            }
        }

        /// <summary> The scale of this GameObject </summary>
        //public Vector3 Scale { get { return LocalTransform.Scale; } set { localTransform.Scale = value; } }
        public Vector3 Scale
        {
            get { return scale; }
            set
            {
                scale = value;
                updateTransform();
            }
        }

        /// <summary> The forward facing vector of this GameObject </summary>
        public Vector3 ForwardVector { get { return new Vector3(LocalTransform.M13, LocalTransform.M23, LocalTransform.M33); } }

        /// <summary> The rightward facing vector of this GameObject </summary>
        public Vector3 RightVector { get { return new Vector3(LocalTransform.M11, LocalTransform.M21, LocalTransform.M31); } }

        /// <summary> The upward facing vector of this GameObject </summary>
        public Vector3 UpVector { get { return new Vector3(LocalTransform.M12, LocalTransform.M22, LocalTransform.M32); } }
        #endregion

        #region PRIVATE PROPERTIES
        /// <summary> Updates the transform </summary>
        private void updateTransform()
        {
            localTransform = Matrix.CreateTranslation(position) * rotation * Matrix.CreateScale(scale);
        }
        #endregion

        #region PUBLIC CONSTRUCTORS
        /// <summary> Creates a blank GameObject </summary>
        public GameObject() :
            base("GameObject")
        {
            scale = new Vector3(1);
            rotation = Matrix.Identity;
            position = Vector3.Zero;
            LocalTransform = Matrix.Identity;
        }

        /// <summary> Creates a new GameObject with the given name and parent </summary>
        /// <param name="name"> The name of this GameObject </param> 
        /// <param name="parent"> The parent of this GameObject </param>
        public GameObject(string name, Instance parent)
            : base(name, parent)
        {
            position = Vector3.Zero;
            scale = new Vector3(1);
            rotation = Matrix.Identity;
            LocalTransform = Matrix.Identity;
        }

        /// <summary> Creates a new GameObject with the given rotation, position, and scale </summary>
        /// <param name="rotation"> The rotation of this GameObject </param>
        /// <param name="position"> The position of this GameObject </param>
        /// <param name="scale"> The scale of this GameObject </param>
        public GameObject(Matrix rotation, Vector3 position, Vector3 scale)
            : base("GameObject")
        {
            Rotation = rotation;
            Position = position;
            Scale = scale;
        }

        /// <summary> Creates a new GameObject with the given rotation, position, scale, and parent </summary>
        /// <param name="rotation"> The rotation of this GameObject </param>
        /// <param name="position"> The position of this GameObject </param>
        /// <param name="scale"> The scale of this GameObject </param>
        /// <param name="parent"> The parent of this GameObject </param>
        public GameObject(Matrix rotation, Vector3 position, Vector3 scale, Instance parent)
            : base("GameObject", parent)
        {
            Rotation = rotation;
            Position = position;
            Scale = scale;
        }

        /// <summary> Creates a new GameObject with the given rotation, position, scale, name, and parent </summary>
        /// <param name="rotation"> The rotation of this GameObject </param>
        /// <param name="position"> The position of this GameObject </param>
        /// <param name="scale"> The scale of this GameObject </param> 
        /// <param name="name"> The name of this GameObject </param>
        /// <param name="parent"> The parent of this GameObject </param>
        public GameObject(Matrix rotation, Vector3 position, Vector3 scale, string name, Instance parent)
            : base(name, parent)
        {
            Rotation = rotation;
            Position = position;
            Scale = scale;
        }
        #endregion
    }
}
