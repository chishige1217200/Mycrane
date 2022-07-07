# MyCrane
自分でクレーンゲームの設定を作りたい！遊びたい！を形にするために，このプロジェクトは始動しました．
現在は固定設定になっていますが，将来はクレーンゲームの設定をいじってファイルに保存できるようになる予定です．

# プログラム構成（MyCrane独自）
- ArmControllerSupport.cs : 下降制限・確率調整用のサポート
- ArmNail.cs : 床への衝突の検知
- ArmUnitLifter : Ropeを使用しないユニットの上昇下降
- BGMPlayer.cs : BGM再生（Unity組み込みのやつが使いにくかったため）
- CameraChanger.cs : カメラの切り替え
- CraneBox.cs : クレーンユニットの移動制御
- CraneManager.cs：各マシンマネージャーの親クラス
- CraneUnitRoter.cs : Type4のアームユニット回転
- CreditSystem.cs : クレジット情報と確率機能
- GameManager.cs : ゲームの終了
- GetPoint.cs : 獲得口通過検知機能とPrizePanelクラスに情報を転送
- Lever.cs : UIレバーの制御
- MachineHost : 操作対象のマシンに指定されているかどうかを管理
- Prize.cs : 景品名の保持
- PrizePanel.cs : Canvasの獲得表示を制御
- RayCaster.cs : 光線を飛ばして操作対象マシンや景品を探す
- Pusher.cs : プッシャーの制御
- RopeManager.cs : 各RopePointインスタンスに上昇・下降指示を送信
- RopePoint.cs : ロープの質点の物理演算情報管理・実際の上昇・下降処理を行う
- SceneManager.cs : シーン遷移
- SEPlayer.cs : SE再生（Unity組み込みのやつが使いにくかったため）
- Timer.cs : 共通のタイマー機能
- Tube.cs : 筒の部分の伸び縮みを制御
- Type1ArmController.cs : Type1のアーム開閉制御（他タイプも同様）
- Type1Manager.cs : Type1の統括制御（他タイプも同様）
- Type1Selector.cs : 1P・2PのType1Manager情報を他のクラスに送る（他タイプも同様）
- Type4VideoManager.cs：ビデオを再生する


## .gitignoreについて
膨大なファイルになるため，音声ファイルと認証関係のファイルを除外しています．

## ライセンスについて
MIT Licenseが適用されますが，適用対象はソースコードのみです．
"DESG"のライセンスは，DSEG-LICENSE.txtの内容が適用されます．
Font "DSEG" by けしかん

# 製作
プログラム制御・モデリング・UI設計・音声 : chishige1217200

モデリング・マッピング : ゐづる

MYCRANE PROJECT
