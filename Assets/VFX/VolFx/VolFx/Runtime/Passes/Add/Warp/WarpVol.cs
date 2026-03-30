using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

//  VolFx © NullTale - https://x.com/NullTale
namespace VolFx
{
    [Serializable, VolumeComponentMenu("VolFx/Warp")]
    public sealed class WarpVol : VolumeComponent, IPostProcessComponent
    {
		public ClampedFloatParameter _intensity = new ClampedFloatParameter(0f, 0f, 1f);
		public ClampedFloatParameter _depth     = new ClampedFloatParameter(2f, 0f, 2f);
        public ColorParameter        _color     = new ColorParameter(Color.white);
		public NoInterpClampedFloatParameter _count     = new NoInterpClampedFloatParameter(.5f, 0f, 1f);
		public NoInterpClampedFloatParameter _size      = new NoInterpClampedFloatParameter(.7f, 0f, 10f);
		public NoInterpClampedFloatParameter _density   = new NoInterpClampedFloatParameter(2.3f, 0f, 5f);
		public NoInterpClampedFloatParameter _speed     = new NoInterpClampedFloatParameter(3f, -7f, 7f);

        // =======================================================================
        public bool IsActive() => active && _intensity.value > 0f;

        public bool IsTileCompatible() => true;
    }
}