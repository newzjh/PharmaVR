Shader "Scene/Rim"
{
//-------------------------------�����ԡ�-----------------------------------------
Properties
{
_MainTex ("������Texture", 2D) = "white" {}
_BumpMap ("����͹����Bumpmap", 2D) = "bump" {}
_RimColor ("����Ե��ɫ��Rim Color", Color) = (0.17,0.36,0.81,0.0)
_RimPower ("����Ե��ɫǿ�ȡ�Rim Power", Range(0.6,9.0)) = 1.0
}
 
//----------------------------����ʼһ������ɫ����---------------------------
SubShader
{
//��Ⱦ����ΪOpaque����͸��
Tags { "RenderType" = "Opaque" }
 
//-------------------��ʼCG��ɫ��������Զ�-----------------
CGPROGRAM
 
//ʹ�������ع���ģʽ
#pragma surface surf Lambert
 
//����ṹ
struct Input
{
float2 uv_MainTex;//������ͼ
float2 uv_BumpMap;//������ͼ
float3 viewDir;//�۲췽��
};
 
//��������
sampler2D _MainTex;//������
sampler2D _BumpMap;//��͹����
float4 _RimColor;//��Ե��ɫ
float _RimPower;//��Ե��ɫǿ��
 
//������ɫ�����ı�д
void surf (Input IN, inout SurfaceOutput o)
{
//���淴����ɫΪ������ɫ
o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
//���淨��Ϊ��͹�������ɫ
o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
//��Ե��ɫ
half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
//��Ե��ɫǿ��
o.Emission = _RimColor.rgb * pow (rim, _RimPower);
}
 
//-------------------����CG��ɫ��������Զ�------------------
ENDCG
}
 
//����̥��Ϊ��ͨ������
Fallback "Diffuse"
}