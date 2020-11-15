
using Microsoft.Xna.Framework;

namespace CastleSim.Json
{
    public struct MapData
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public Point Size  { get; set; }
        public int[] MapTiles { get; set; }
        public int[] ObjectLayer { get; set; }

        public int[] Enemys { get; set; }




    }
}
