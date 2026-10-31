using Content.Shared.Storage;
using Content.Shared.Lock;
using Robust.Client.GameObjects;

namespace Content.Client.Lock.Visualizers;

public sealed class LockVisualizerSystem : VisualizerSystem<LockVisualsComponent>
{
    protected override void OnAppearanceChange(EntityUid uid, LockVisualsComponent comp, ref AppearanceChangeEvent args)
    {
        if (args.Sprite == null)
            return;

        if (!AppearanceSystem.TryGetData<bool>(uid, LockVisuals.Locked, out var locked, args.Component))
            locked = true;

        var hasHighlight = args.Sprite.LayerMapTryGet(LockVisualLayers.Highlight, out _);
        var unlockedStateExist = args.Sprite.BaseRSI?.TryGetState(comp.StateUnlocked, out _) == true;
        var hasOpenState = AppearanceSystem.TryGetData<bool>(uid, StorageVisuals.Open, out var open, args.Component);

        if (hasOpenState && open)
        {
            args.Sprite.LayerSetVisible(LockVisualLayers.Lock, false);
            if (hasHighlight)
                args.Sprite.LayerSetVisible(LockVisualLayers.Highlight, false);
            return;
        }


        args.Sprite.LayerSetVisible(LockVisualLayers.Lock, unlockedStateExist || locked);
        if (unlockedStateExist)
            args.Sprite.LayerSetState(LockVisualLayers.Lock, locked ? comp.StateLocked : comp.StateUnlocked);

        if (hasHighlight)
            args.Sprite.LayerSetVisible(LockVisualLayers.Highlight, locked);
    }
}

public enum LockVisualLayers : byte
{
    Lock,
    Highlight
}
