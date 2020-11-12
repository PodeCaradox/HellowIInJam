
using Microsoft.Xna.Framework;

namespace CastleSim.Json
{
    public struct MapData
    {
        public string Name { get; set; }
        public Point Size  { get; set; }
        public int[] MapTiles { get; set; }

    }
}
