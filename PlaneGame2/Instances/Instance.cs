using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

namespace PlaneGame2.Instances
{
    /// <summary> A base class for all game objects </summary>
    public abstract class Instance : IDisposable
    {
        #region PRIVATE FIELDS
        /// <summary> The internal parent object </summary>
        private Instance parent;

        /// <summary> The node version of this object </summary>
        protected LinkedListNode<Instance> ContainerNode;
        #endregion

        #region PUBLIC PROPERTIES
        /// <summary> The name of this Instance </summary>
        public string Name { get; set; }
        public bool Visible = true;

        /// <summary> The children of this Instance </summary>
        public LinkedList<Instance> Children { get; private set; }
        
        /// <summary> The parent of this Instance </summary>
        public Instance Parent
        {
            get { return parent; }
            set
            {
                //If the parent is being set to nothing, itself, or is redundantly being set, do nothing
                if (value == parent || value == this) return;

                //If this object has a parent, tell it to remove this from its list
                if (parent != null) parent.RemoveChild(this);

                //Sets the new parent
                parent = value;

                //If the new parent isn't set to null, adds this object as a child to the new parent object
                if (parent != null) parent.AddChild(this);
            }
        }
        #endregion

        #region PROTECTED FUNCTIONS
        /// <summary> Adds an Instance to this Instance's children </summary>
        /// <param name="child"> The child to be added </param>
        protected void AddChild(Instance child)
        {
            Children.AddFirst(child.ContainerNode);
        }

        /// <summary> Removes an Instance from this Instance's children </summary>
        /// <param name="child"> The child to be removed </param>
        protected void RemoveChild(Instance child)//Removes this object from the list.
        {
            if (child.ContainerNode.List == Children && child.Parent == this)
                Children.Remove(child.ContainerNode);
                child.Dispose();
        }
        #endregion

        #region PUBLIC METHODS
        /// <summary> Draws this Instance and all of its children </summary>
        /// <param name="graphicsDevice"> The graphics device to draw to </param>
        /// <param name="activeCamera"> The camera which is drawing this Instance </param>
        public virtual void Draw(GraphicsDevice graphicsDevice, Camera activeCamera)
        {
            if (Visible)
            {
                foreach (Instance i in Children) i.Draw(graphicsDevice, activeCamera);
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        /// <summary> Updates this Instance and all of its children </summary>
        /// <param name="gameTime"> The current game time </param>
        public virtual void Update(GameTime gameTime)
        {
            foreach (Instance i in Children) i.Update(gameTime);
        }
        #endregion

        #region PUBLIC CONSTRUCTORS
        /// <summary> Creates a new Instance with the given name </summary>
        /// <param name="name"> The name of this Instance </param>
        public Instance(string name)
        {
            Name = name;
            Children = new LinkedList<Instance>();
            ContainerNode = new LinkedListNode<Instance>(this);
        }

        /// <summary> Creates a new Instance with the given name and parent </summary>
        /// <param name="name"> The name of this Instance </param>
        /// <param name="parent"> The parent of this instance </param>
        public Instance(string name, Instance parent)
        {
            Name = name;
            Children = new LinkedList<Instance>();
            ContainerNode = new LinkedListNode<Instance>(this);
            Parent = parent;
        }
        #endregion
    }
}
