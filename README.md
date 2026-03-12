# Unity 2D 1vs1 Shooting AI

Unityで作成した2D 1vs1シューティングゲームの敵AIです。

## AIの特徴

- 状態判定（距離 / プレイヤー向き / 危険弾 / HP）
- 行動木による候補生成
- 重み付きランダムによる行動選択
- 移動と射撃のレイヤー分離
- 回避行動とマップ端処理

## AI構造

- AIStateJudge  
  戦闘状況を評価し、AIの状態を決定

- AIMoveBehaviourTree / AIShootBehaviourTree  
  状況に応じた行動候補を生成

- AIMoveActionSelector / AIShootActionSelector  
  重み付きランダムで行動を選択

- AIMoveActuator / AIShootActuator  
  実際の移動や射撃を実行

- AIEnemyAIController  
  AI全体の処理を管理
