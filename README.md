# BSGJ_W-07
## 概要
BitSummit2025のチームW07のリポジトリです。

## ルール
### ブランチ名
| ブランチ名 | 特徴 |
| --- | --- |
| main | メインブランチ。基本いじりません。<br> developブランチがある程度完成したらマージします。 |
| develop | 基本マージする親ブランチです。ここからブランチを切って作業をします。 |
| feature_task_20YYMMDD | developブランチから切って作業するブランチです。<br> feature_<タスク名>_<日付>で書きます。<br>例：feature_gamewindow_20250430 |

### コミットメッセージ
- chore：タスクファイルなどプロダクションに影響のない修正
- docs：ドキュメントの更新
- feat：ユーザ向けの機能の追加や変更
- fix：ユーザ向けの不具合の調整
- ref：リファクタリングを目的とした修正
- style：フォーマットなどのスタイルに関する修正
- test：テストコードの追加や修正

## 環境
- ゲームエンジン：Unity6000.0.47f1
- バージョン管理：github
- アセット管理・進捗管理：GoogleDrive
- サウンドアセット管理：AutioStock

## 技術スタック・ライブラリ
- Unity
- UniTask

## デザインパターン
- 未定

## ファイル構成
<pre> 
BSGJ_W-07 // リポジトリ
├── README.md
└── Biritako-Boom // Unityプロジェクト
    ├── asset
        ├── AddressableAssetsData
        ├── Audio
        ├── Scenes
        │   ├──InGame //Scene
        │   ├──Result //Scene
        │   ├──Title //Scene
        ├── Scripts
        │   ├── Common
        │   ├── InGame
        │   │   ├── Model
        │   │   ├── Presenter
        │   │   └── View
        │   ├── Result
        │   │   ├── Model
        │   │   ├── Presenter
        │   │   └── View
        │   └── Title
        │       ├── Model
        │       ├── Presenter
        │       └── View
        └── UI
</pre>
