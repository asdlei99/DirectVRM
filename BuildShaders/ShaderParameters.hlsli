// 全シェーダ共通定数バッファ。
// DirectVRM.D3D.ShaderParameters.cs に対応する。

cbuffer ShaderParameters : register( b0 )
{
	// ワールド変換行列。
	matrix g_WorldMatrix;
	
	// ビュー変換行列。
	matrix g_ViewMatrix;
	
	// 射影変換行列。
	matrix g_ProjectionMatrix;

	// テッセレーション係数。
	float g_TessellationFactor;
};