# MyCrane
自分でクレーンゲームの設定を作りたい！遊びたい！を形にするために，このプロジェクトは始動しました．
現在は固定設定になっていますが，将来はクレーンゲームの設定をいじってファイルに保存できるようになる予定です．

# プログラム構成
- ArmControllerSupport.cs : 下降制限・確率調整用のサポート
- ArmNail.cs : 床への衝突の検知
- BGMPlayer.cs : BGM再生（Unity組み込みのやつが使いにくかったため）
- CameraChanger.cs : カメラの切り替え
- CraneBox.cs : クレーンユニットの移動制御
- CreditSystem.cs : クレジット情報と確率機能
- GameManager.cs : シーンの切り替え
- GetPoint.cs : 獲得口通過検知機能とPrizePanelクラスに情報を転送
- Lever.cs : レバーモーションの制御
- Prize.cs : 景品名の保持
- PrizePanel.cs : Canvasの獲得表示を制御
- Rope.cs : ロープ表示用（未使用）
- RopeManager.cs : 各RopePointインスタンスに上昇・下降指示を送信
- RopePoint.cs : ロープの質点の物理演算情報管理・実際の上昇・下降処理を行う
- SEPlayer.cs : SE再生（Unity組み込みのやつが使いにくかったため）
- Timer.cs : 共通のタイマー機能
- Tube.cs : 筒の部分の伸び縮みを制御
- Type1ArmController.cs : Type1のアーム開閉制御
- Type1Manager.cs : Type1の統括制御
- Type1Selector.cs : 1P・2PのType1Manager情報を他のクラスに送る
- Type2ArmController.cs : Type2のアーム開閉制御
- Type2Manager.cs : Type2の統括制御
- Type3ArmController.cs : Type3のアーム開閉制御
- Type3Manager.cs : Type3の統括制御
- Type4ArmUnitRoter.cs : Type4のアームユニット回転
- Type4ArmController.cs : Type4のアーム開閉制御
- Type4Manager.cs : Type4の統括制御
- Type4Selector : 1P・2PのType4Manager情報を他のクラスに送る
- Type5ArmController.cs : Type5のアーム開閉制御
- Type5Manager.cs : Type5の統括制御
- Type7Manager : Type7の統括制御
- VideoPlay.cs：ビデオを再生する

## .gitignoreについて
膨大なファイルになるため，音声ファイルと認証関係のファイルを除外しています．

# 製作
プログラム制御・モデリング・UI設計・音声 : chishige1217200

モデリング・マッピング : ゐづる

MYCRANE PROJECT