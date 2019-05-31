// �X�L�j���O�p�R���s���[�g�V�F�[�_�[��������W���C���g�̍ő吔
#define MAX_JOINT    768

// �X�L�j���O�p�R���s���[�g�V�F�[�_�[�̓��́i�\�����o�b�t�@�j

StructuredBuffer<float3> g_VertexPositionBuffer : register( t0 );		// ���_�� POSITION �z��
StructuredBuffer<float3> g_VertexNormalBuffer : register( t1 );			// ���_�� NORMAL �z��
StructuredBuffer<float2> g_VertexTexCoord0Buffer : register( t2 );		// ���_�� TEXCOORD0 �z��
StructuredBuffer<float4> g_VertexWeights0Buffer : register( t3 );		// ���_�� WEIGHTS_0 �z��
StructuredBuffer<uint2> g_VertexJoints0Buffer : register( t4 );			// ���_�� JOINTS_0 �z��� Skin���̍s�񏇂ɒ������C���f�b�N�X�̔z��(ushort) 
StructuredBuffer<matrix> g_InvBindMatrixBuffer : register( t5 );		// �X�L���̋t�o�C���h�s��̔z��
StructuredBuffer<matrix> g_JointWorldMatrixBuffer : register( t6 );		// �W���C���g�̃��[���h�ϊ��s��̔z��

// �X�L�j���O�p�R���s���[�g�V�F�[�_�[�̏o�́i���_�o�b�t�@; RW�o�C�g�o�b�t�@�j
// �� ���̃o�b�t�@�́A���̂܂ܒ��_�V�F�[�_�̓��́i���_�o�b�t�@; VS_INPUT�j�Ƃ��Ďg�p�����B
RWByteAddressBuffer g_VSBuffer : register( u0 );

