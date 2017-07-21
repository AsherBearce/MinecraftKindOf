using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaneGame2.Misc
{
    public static class TerrainGenerator
    {
        public static byte BlockGen(float x, float y, float z)//This method needs to be moved to it's own class
        {
            float yOff = 40 * Noise.GetOctaveNoise(x / 50f, 0, z / 50f, 1);

            float noise = 10 * Noise.GetOctaveNoise(x / 20, y / 20, z / 20, 1) - yOff - 10 + y;//Temporary generator

            return (byte)((noise < 6 && noise > 3.5) ? 2 : (noise < 3.5) ? 1 : 0);
        }
    }
}
