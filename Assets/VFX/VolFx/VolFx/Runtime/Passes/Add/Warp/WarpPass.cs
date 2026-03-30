using UnityEngine;

//  VolFx © NullTale - https://x.com/NullTale
namespace VolFx
{
    [ShaderName("Hidden/VolFx/Warp")]
    public class WarpPass : VolFx.Pass
    {
		private static readonly int s_Color        = Shader.PropertyToID("_Color");
		private static readonly int s_Tiling       = Shader.PropertyToID("_Tiling");
		private static readonly int s_RadialScale  = Shader.PropertyToID("_RadialScale");
		private static readonly int s_Power        = Shader.PropertyToID("_Power");
		private static readonly int s_Remap        = Shader.PropertyToID("_Remap");
		private static readonly int s_Animation    = Shader.PropertyToID("_Animation");
		private static readonly int s_MaskScale    = Shader.PropertyToID("_MaskScale");
		private static readonly int s_MaskHardness = Shader.PropertyToID("_MaskHardness");
		private static readonly int s_MaskPower    = Shader.PropertyToID("_MaskPower");
		
		public override string ShaderName => string.Empty;

		public Vector2 _count = new Vector2(0, 300f);
		public float _power    = 2f;
		[Range(-1, 1)]
		public float _hardness = 0.583f;

		protected override bool Invert => true;
		
        // =======================================================================
        public override bool Validate(Material mat)
        {
            var settings = Stack.GetComponent<WarpVol>();

            if (settings.IsActive() == false)
                return false;
            
            mat.SetColor(s_Color, settings._color.value);
            mat.SetFloat(s_Tiling, _count.x + _count.y * settings._count.value);
            mat.SetFloat(s_RadialScale, settings._size.value);
            mat.SetFloat(s_Power, settings._density.value);
            mat.SetFloat(s_Remap, 1f - settings._intensity.value);
            mat.SetFloat(s_Animation, settings._speed.value);
            mat.SetFloat(s_MaskScale, settings._depth.value);
			mat.SetFloat(s_MaskHardness, _hardness);
			mat.SetFloat(s_MaskPower, _power);

            return true;
        }
    }
}