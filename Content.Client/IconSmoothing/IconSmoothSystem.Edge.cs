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

        var xform = Transform(uid);

        var directions = DirectionFlag.None;
        
        if (xform.GridUid is EntityUid gridUid && TryComp<MapGridComponent>(gridUid, out var grid))
        {
            var pos = _map.TileIndicesFor(gridUid, grid, xform.Coordinates);

            if (MatchingEntity(smooth, _map.GetAnchoredEntitiesEnumerator(gridUid, grid, pos.Offset(Direction.North))))
                directions |= DirectionFlag.North;
            if (MatchingEntity(smooth, _map.GetAnchoredEntitiesEnumerator(gridUid, grid, pos.Offset(Direction.South))))
                directions |= DirectionFlag.South;
            if (MatchingEntity(smooth, _map.GetAnchoredEntitiesEnumerator(gridUid, grid, pos.Offset(Direction.East))))
                directions |= DirectionFlag.East;
            if (MatchingEntity(smooth, _map.GetAnchoredEntitiesEnumerator(gridUid, grid, pos.Offset(Direction.West))))
                directions |= DirectionFlag.West;
        }

        UpdateEdge(uid, directions, sprite, smooth);
    }

    private void UpdateEdge(EntityUid uid, DirectionFlag directions, SpriteComponent? sprite = null, IconSmoothComponent? smooth = null)
    {
        if (!Resolve(uid, ref sprite, ref smooth, false))
            return;

        if (component.DrawDepth.HasValue)
            sprite.DrawDepth = component.DrawDepth.Value;

            _sprite.LayerSetVisible((uid, sprite), edge, (dir & directions) == 0x0);
        }
    }

    // WWDP edit start
    private void HideAllEdgeLayers(SpriteComponent sprite)
    {
        var allEdgeLayers = Enum.GetValues<EdgeLayer>();
        foreach (var edgeLayer in allEdgeLayers)
        {
            if (sprite.LayerMapTryGet(edgeLayer, out var layerIndex))
            {
                sprite.LayerSetVisible(layerIndex, false);
            }
        }
    }
    // WWDP edit end

    private bool MatchesEdgeCriteria(SmoothEdgeComponent edge, IconSmoothComponent neighbor)
    {
        if (!edge.RequireMatchingKey)
            return true; // legacy: always show edge

        if (neighbor.SmoothKey == null)
            return false;

        return edge.EdgeAdditionalKeys.Contains(neighbor.SmoothKey);
    }

    private Vector2i DirectionToOffset(DirectionFlag direction)
    {
        return direction switch
        {
            DirectionFlag.North => new Vector2i(0, 1),
            DirectionFlag.South => new Vector2i(0, -1),
            DirectionFlag.East => new Vector2i(1, 0),
            DirectionFlag.West => new Vector2i(-1, 0),
            DirectionFlag.NorthEast => new Vector2i(1, 1),
            DirectionFlag.NorthWest => new Vector2i(-1, 1),
            DirectionFlag.SouthEast => new Vector2i(1, -1),
            DirectionFlag.SouthWest => new Vector2i(-1, -1),
            _ => Vector2i.Zero
        };
    }

    private EdgeLayer GetEdge(DirectionFlag direction)
    {
        return direction switch
        {
            DirectionFlag.South => EdgeLayer.South,
            DirectionFlag.East => EdgeLayer.East,
            DirectionFlag.North => EdgeLayer.North,
            DirectionFlag.West => EdgeLayer.West,
            DirectionFlag.SouthEast => EdgeLayer.SouthEast,
            DirectionFlag.NorthEast => EdgeLayer.NorthEast,
            DirectionFlag.NorthWest => EdgeLayer.NorthWest,
            DirectionFlag.SouthWest => EdgeLayer.SouthWest,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }

    private enum EdgeLayer : byte
    {
        South,
        East,
        North,
        West,
        SouthEast,
        NorthEast,
        NorthWest,
        SouthWest
    }
}
