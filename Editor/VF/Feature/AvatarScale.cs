using UnityEngine;
using VF.Builder;

namespace VF.Feature {

public class AvatarScale : BaseFeature<VF.Model.Feature.AvatarScale> {
    public override void Generate(VF.Model.Feature.AvatarScale config) {
        var paramScale = manager.NewFloat("Scale", synced: true, def: 0.5f);
        var scaleClip = manager.NewClip("Scale");
        var baseScale = avatarObject.transform.localScale.x;
        motions.Scale(scaleClip, avatarObject, ClipBuilder.FromFrames(
            new Keyframe(0, baseScale * 0.1f),
            new Keyframe(2, baseScale * 1),
            new Keyframe(3, baseScale * 2),
            new Keyframe(4, baseScale * 10)
        ));

        var layer = manager.NewLayer("Scale");
        var main = layer.NewState("Scale").WithAnimation(scaleClip).MotionTime(paramScale);
        
        manager.NewMenuSlider("Scale/Adjust", paramScale);
        manager.NewMenuToggle("Scale/40%", paramScale, 0.15f);
        manager.NewMenuToggle("Scale/60%", paramScale, 0.25f);
        manager.NewMenuToggle("Scale/80%", paramScale, 0.40f);
        manager.NewMenuToggle("Scale/100%", paramScale, 0.50f);
        manager.NewMenuToggle("Scale/125%", paramScale, 0.55f);
        manager.NewMenuToggle("Scale/150%", paramScale, 0.60f);
        manager.NewMenuToggle("Scale/200%", paramScale, 0.75f);
        
    }

    public override string GetEditorTitle() {
        return "Avatar Scale Slider";
    }

    public override bool AvailableOnProps() {
        return false;
    }
}

}
