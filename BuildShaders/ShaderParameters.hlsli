// �S�V�F�[�_���ʒ萔�o�b�t�@�B
// DirectVRM.D3D.ShaderParameters.cs �ɑΉ�����B

cbuffer ShaderParameters : register( b0 )
{
	// ���[���h�ϊ��s��B
	matrix g_WorldMatrix;
	
	// �r���[�ϊ��s��B
	matrix g_ViewMatrix;
	
	// �ˉe�ϊ��s��B
	matrix g_ProjectionMatrix;

	// �e�b�Z���[�V�����W���B
	float g_TessellationFactor;
};