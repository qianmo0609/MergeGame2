// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "Custom/UI/UVAni"
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Total ("total", Int) = 1	//总数
		_Rows ("rows", Int) = 1 //行数
		_Cols ("cols", Int) = 1	//列数
		_Speed ("speed", Int) = 1	//速度		
		[HideInInspector]
		_StencilComp ("Stencil Comparison", Float) = 8
		[HideInInspector]
		_Stencil ("Stencil ID", Float) = 0
		[HideInInspector]
		_StencilOp ("Stencil Operation", Float) = 0
		[HideInInspector]
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		[HideInInspector]
		_StencilReadMask ("Stencil Read Mask", Float) = 255		
	}

	SubShader 
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType" = "Transparent" }
		//Tags { "Queue"="Geometry" "IgnoreProjector"="True" "RenderType"="Opaque"}
		

		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}

		ColorMask RGB

		Lighting OFF
		Cull OFF 
		ZWrite On
		Blend SrcAlpha OneMinusSrcAlpha
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			sampler2D _MainTex;

			int _Total;
			int _Rows;
			int _Cols;
			float _Speed;		

			float xStart;
			float yStart;			

			int col;
			int row;
			int num;

			struct v2f {
				float4  pos : POSITION;
				float2  uv : TEXCOORD0;
			} ;

			v2f vert(appdata_base v)
			{			
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex); //顶点3D坐标转换为屏幕2D坐标				
				
				num = (int)(_Time.y * 15 * _Speed)  % _Total; //用时间遍历				

				col =  num % _Cols;
				row = _Rows - 1 - num / _Cols; //v是从下到上，所以需要倒置

				xStart = col * (1 / (float)_Cols);
				yStart = row * (1 / (float)_Rows);
				
				o.uv = float2(xStart + v.texcoord.x / (float)_Cols,yStart + v.texcoord.y / (float)_Rows);				

				return o;
			}
			float4 frag(v2f i) : COLOR
			{				
				float4 texCol = tex2D(_MainTex,i.uv);		
				clip(texCol.a-0.1);		
				return texCol;
			}
			ENDCG	
		}				
	} 

	Fallback "Mobile/VertexLit"
}

