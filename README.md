# Unity 2D 1vs1 Shooting AI

Unityで作成した2D 1vs1シューティングゲームの敵AIです。

## AIの特徴

- 状態判定（距離 / プレイヤー向き / 危険弾 / HP）
- 行動木による候補生成
- 重み付きランダムによる行動選択
- 移動と射撃のレイヤー分離
- 回避行動とマップ端処理

## AI構造

State → BehaviourTree → CandidateActions → WeightedSelection → Actuator
