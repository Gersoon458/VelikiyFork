using System.Numerics;
using Content.Shared.IconSmoothing;
using Robust.Client.GameObjects;
using Robust.Shared.Map.Components;

namespace Content.Client.IconSmoothing;

public sealed partial class IconSmoothSystem
{
//    private void OnEdgeShutdown(EntityUid uid, SmoothEdgeComponent component, ComponentShutdown args)
//    {
//        if (!TryComp<SpriteComponent>(uid, out var sprite))
//            return;
//
//        sprite.LayerMapRemove(EdgeLayer.South);
//        sprite.LayerMapRemove(EdgeLayer.East);
//        sprite.LayerMapRemove(EdgeLayer.North);
//        sprite.LayerMapRemove(EdgeLayer.West);
//    }

    private void CalculateEdge(EntityUid uid, SpriteComponent? sprite = null, IconSmoothComponent? smooth = null)
    {
        if (!Resolve(uid, ref sprite, ref smooth, false))
            return;
        
        if (smooth.SmoothEdgeLayers.Length == 0)
            return;

        var xform = Transform(uid);

        var directions = DirectionFlag.None;

        if (xform.GridUid is EntityUid gridUid && TryComp<MapGridComponent>(gridUid, out var grid))
        {
            var pos = _map.TileIndicesFor(gridUid, grid, xform.Coordinates);

            if (MatchingEntity(smooth, grid, pos, Direction.North, xform.LocalRotation))
                directions |= DirectionFlag.North;
            if (MatchingEntity(smooth, grid, pos, Direction.South, xform.LocalRotation))
                directions |= DirectionFlag.South;
            if (MatchingEntity(smooth, grid, pos, Direction.East, xform.LocalRotation))
                directions |= DirectionFlag.East;
            if (MatchingEntity(smooth, grid, pos, Direction.West, xform.LocalRotation))
                directions |= DirectionFlag.West;
            if (MatchingEntity(smooth, grid, pos, Direction.NorthEast, xform.LocalRotation))
                directions |= DirectionFlag.NorthEast;
            if (MatchingEntity(smooth, grid, pos, Direction.NorthWest, xform.LocalRotation))
                directions |= DirectionFlag.NorthWest;
            if (MatchingEntity(smooth, grid, pos, Direction.SouthEast, xform.LocalRotation))
                directions |= DirectionFlag.SouthEast;
            if (MatchingEntity(smooth, grid, pos, Direction.SouthWest, xform.LocalRotation))
                directions |= DirectionFlag.SouthWest;
        }

        UpdateEdge(uid, directions, sprite, smooth);
    }

    private void UpdateEdge(EntityUid uid, DirectionFlag directions, SpriteComponent? sprite = null, IconSmoothComponent? smooth = null)
    {
        if (!Resolve(uid, ref sprite, ref smooth, false))
            return;

        if (smooth.SmoothEdgeLayers.Length == 0)
            return;

        foreach (var edge in smooth.SmoothEdgeLayers)
        {
            var dir = GetDir(edge);
            var visible = (dir & directions) == 0x0;

            _sprite.LayerSetVisible((uid, sprite), edge, visible ^ smooth.ShowEdgeIfMatching);
        }
    }

    private DirectionFlag GetDir(EdgeLayer direction)
    {
        return direction switch
        {
            EdgeLayer.North => DirectionFlag.North,
            EdgeLayer.South => DirectionFlag.South,
            EdgeLayer.East => DirectionFlag.East,
            EdgeLayer.West => DirectionFlag.West,
            EdgeLayer.NorthEast => DirectionFlag.NorthEast,
            EdgeLayer.NorthWest => DirectionFlag.NorthWest,
            EdgeLayer.SouthEast => DirectionFlag.SouthEast,
            EdgeLayer.SouthWest => DirectionFlag.SouthWest,
            _ => throw new ArgumentOutOfRangeException(),
        };
    }
}
