# CHANGELOG

本專案所有重要變動都將記錄于此文件。

格式基於 [Keep a Changelog](https://keepachangelog.com/zh-TW/1.0.0/)，
並遵循 [Semantic Versioning](https://semver.org/lang/zh-TW/) 版本號規則。

## [Unreleased]

### 新增
- 新增 `Assets/Scenes/StartSettings.lighting` 場景光照設定檔案
- 新增 `ProjectSettings/SceneTemplateSettings.json` 場景模板設定

### 變動
- 升級 `Assets/Scenes/Start.unity` 場景渲染與光照設定以支援 Unity 6000.0.46f1
  - RenderSettings serializedVersion 升級至 10
  - LightmapSettings serializedVersion 升級至 13
- 更新 `opencode.json` OpenCode 配置檔案

## [2021.0.3.45f1] - 2025-04-05

### 新增
- 初始化 Unity MCP Server 整合
- 新增 Unity MCP Plugin (com.ivanmurzak.unity.mcp v0.63.3)
- 新增專案配置檔案與套件依賴
- 新增 Unity MCP Skills 文檔
- 新增優化指南、套件清單與 GUILayer 移除工具

### 變動
- 更新 .gitignore 配置
- 指定 Python 版本 3.11.9
- 升級 Unity 版本至 6000.0.46f1

---

[Unreleased]: https://github.com/chiisen/Salmon/compare/v2021.0.3.45f1...HEAD
[2021.0.3.45f1]: https://github.com/chiisen/Salmon/releases/tag/v2021.0.3.45f1