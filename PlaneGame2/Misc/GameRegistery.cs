using System;
using System.Collections.Generic;
using System.Reflection;
using PlaneGame2.Misc;
using PlaneGame2.Blocks;
using PlaneGame2.Interfaces;

//There may be a better way of doing this, but right now this feels okay.

namespace PlaneGame2.Misc
{
    /// <summary>
    /// A class used for registering things into the game so they can be retrieved by the game engine later on.
    /// </summary>
    public class GameRegistery
    {
        protected static Dictionary<byte, object> BlockRegister = new Dictionary<byte, object>();

        public static object GetBlock(byte ID)
        {
            return BlockRegister[ID];
        }

        /// <summary>
        /// Register all blocks in the 'Blocks' namespace. 
        /// </summary>
        public static void init()
        {
            Type[] blocks = Assembly.GetExecutingAssembly().GetTypes();
            foreach (Type t in blocks)
            {
                if (t.Namespace == "PlaneGame2.Blocks" && t.Name != "Block")
                {
                    t.GetMethod("init", BindingFlags.Static | BindingFlags.Public).Invoke(null, null);
                }
            }
        }

        public static void RegisterBlock<T>(T newRegister) where T : IBlock
        {
            BlockRegister.Add(newRegister.ID, newRegister);
        }
    }
}
