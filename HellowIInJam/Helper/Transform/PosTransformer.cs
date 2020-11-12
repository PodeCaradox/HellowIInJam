using DefaultEcs;
using CastleSim.Components;
using Microsoft.Xna.Framework;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using HellowIInJam.Components.Map;
using HellowIInJam.Components.Main;

namespace CastleSim.Systems.HelperClasses
{
    internal static class PosTransformer
    {
        private static Entity _map;
        private static Entity _gameConfig;
        private static Entity _camera;
        internal static void Init(World world)
        {
            _map = world.GetEntities().With<Map>().AsSet().GetEntities()[0];
            _gameConfig = world.GetEntities().With<GameConfig>().AsSet().GetEntities()[0];
            _camera = world.GetEntities().With<Camera>().AsSet().GetEntities()[0];
        }

        internal static Point MouseToScreenPos(Point mousePosition)
        {
            Vector2 worldPosition = Vector2.Transform(new Vector2(mousePosition.X, mousePosition.Y), Matrix.Invert(_camera.Get<Camera>().Transform));
            return new Point((int)worldPosition.X, (int)worldPosition.Y);
        }

        internal static Point ScreenToWorldPos(Point position)
        {
            ref var gameConfig = ref _gameConfig.Get<GameConfig>();
            Point point = new Point(position.X / gameConfig.TileSize.X, position.Y / gameConfig.TileSize.Y);
            return point;
        }

        internal static int CalculateWorldIndex(Point position)
        {
            ref var mapData = ref _map.Get<Map>();
            ref var gameConfig = ref _gameConfig.Get<GameConfig>();
            int index = position.X / gameConfig.TileSize.X + (position.Y / gameConfig.TileSize.Y) * mapData.Size.X;
            return index;
        }

        internal static int CalculateWorldIndex(Vector2 position)
        {
            ref var mapData = ref _map.Get<Map>();
            ref var gameConfig = ref _gameConfig.Get<GameConfig>();
            int index = (int)position.X / gameConfig.TileSize.X + ((int)position.Y / gameConfig.TileSize.Y) * mapData.Size.X;
            return index;
        }

        internal static Vector2 MapToScreenPos(Point position)
        {
            ref var gameConfig = ref _gameConfig.Get<GameConfig>();
            ref var mapData = ref _map.Get<Map>();
            Vector2 pos = new Vector2(position.X * gameConfig.TileSize.X + gameConfig.TileSizeHalf.X, position.Y * gameConfig.TileSize.Y + gameConfig.TileSizeHalf.Y);
            return pos;
        }

        internal static Vector2 MapToScreenPos(Vector2 position)
        {
            ref var gameConfig = ref _gameConfig.Get<GameConfig>();
            ref var mapData = ref _map.Get<Map>();
            Vector2 pos = new Vector2((int)position.X * gameConfig.TileSize.X + gameConfig.TileSizeHalf.X, (int)position.Y * gameConfig.TileSize.Y + gameConfig.TileSizeHalf.Y);
            return pos;
        }

      
 
    }
}
