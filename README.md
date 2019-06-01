# DirectVRM

DirectVRM は、.NET Framework で利用可能な [VRM](https://vrm.dev/) ライブラリです。  
C# などの .NET 言語を使って、VRM モデルを描画するアプリケーションを作ることができます。
  - .NET Framework で Direct3D を扱うために、[SharpDX](https://github.com/sharpdx/SharpDX) を使用します。
  - Unity, UnrealEngine などのゲームエンジンは使用しません。

## リリース

リリースは NuGet 上で行っています。

[NuGet Gallery | DirectVRM<br/>![Nuget](https://img.shields.io/nuget/vpre/DirectVRM.svg)](https://www.nuget.org/packages/DirectVRM/)

## 動作環境

+ Windows 10 (x64 only)
+ .NET Framework 4.7.1+
+ DirectX 11.0+

## 機能と実装

### glTF2.0 に準拠

- [x] glb ファイルの読み込み
- [x] Direct3D11 を使ったモデルの描画
- [x] シーン
- [x] メッシュ
- [x] モーフターゲット
- [x] スケルトン、スキニング（計算シェーダー利用）
- [ ] マテリアル（PBR）
  - [ ] メタリック-ラフネスマテリアル
  - [x] テクスチャ（画像、サンプラー）
- [x] アニメーション（モーフ、マテリアル）
- [x] カメラ（射影、正射影）
- [ ] 増分アクセサ(AccessorSparse)

### VRM に準拠

- [x] モデル情報
- [x] ブレンドシェイプ、表情プリセット
- [ ] 一人称
  - [x] 一人称情報
  - [ ] 一人称視点
  - [ ] 視線制御
- [x] 揺れモノ
  - [x] 揺れボーン(VRMStpringBone) 
  - [x] 衝突判定
- [ ] マテリアル（VRM）
  - [ ] Standard（テッセレーションあり）
  - [x] VRM/UnlitTexture
  - [ ] VRM/UnlitCutout
  - [ ] VRM/UnlitTransparent
  - [ ] VRM/UnlitTransparentZWrite
  - [ ] VRM/MToon
