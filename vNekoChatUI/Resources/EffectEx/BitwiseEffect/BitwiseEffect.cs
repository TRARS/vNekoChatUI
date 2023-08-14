using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace vNekoChatUI.Resources.EffectEx.BitwiseEffect
{
    public class BitwiseEffect : ShaderEffect
    {
        string? AssemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

        public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(BitwiseEffect), 0);

        /// <summary>
        /// 反色
        /// </summary>
        public BitwiseEffect()
        {
            PixelShader = new PixelShader
            {
                UriSource = new Uri($"pack://application:,,,/{AssemblyName};component/Resources/EffectEx/BitwiseEffect/BitwiseEffect.ps"),
            };

            this.UpdateShaderValue(InputProperty);
        }

        public Brush Input
        {
            get => ((Brush)(this.GetValue(InputProperty)));
            set => this.SetValue(InputProperty, value);
        }
    }
}
