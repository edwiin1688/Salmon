# Unity 輸入問題診斷與解決指南

## 問題：Editor 測試模式下無法接收所有輸入

---

## 解決方案 1: 新增 EventSystem（最常見）

### 步驟

在 Unity Editor 中：

1. 開啟場景 `Assets/Scenes/Start.unity`

2. 在 Hierarchy 視窗中右鍵點擊

3. 選擇 `UI` → `Event System`

4. 儲存場景：`Ctrl + S`

### 驗證

- EventSystem 應該出現在 Hierarchy 中
- EventSystem GameObject 應該包含：
  - EventSystem 組件
  - Standalone Input Module 組件

---

## 解決方案 2: 檢查 EasyTouch 設定

EasyTouch 可能會攔截輸入。請檢查：

### 步驟

1. 在 Unity 中：`Tools` → `EasyTouch` → `Settings`

2. 檢查以下設定：
   - **Enable Unity Input**: 確保勾選
   - **Auto Select**: 視需要調整

3. 如果在 Editor 中測試：
   - EasyTouch 可能預期觸控輸入
   - 需要設定 Editor 模擬觸控

### EasyTouch Editor 模擬

EasyTouch 通常需要在 Editor 中模擬觸控輸入：

1. 找到 EasyTouch 的設定檔
2. 啟用 Editor 模擬模式
3. 或者暫時停用 EasyTouch 進行測試

---

## 解決方案 3: 檢查 Play Mode Settings

Unity Editor 的 Play Mode 可能影響輸入：

### 步驟

1. `Edit` → `Project Settings` → `Editor`

2. 在 **Enter Play Mode Settings** 中：
   - ✅ 勾選 `Enter Play Mode Options`
   - ✅ 勾選 `Reload Domain`
   - ✅ 勾選 `Reload Scene`

3. 重新啟動 Unity Editor

---

## 解決方案 4: 檢查輸入指令碼

### 常見問題

如果你的遊戲使用 `Input.GetKey()` 或 `Input.GetButton()`：

```csharp
// 確保使用正確的輸入方式
void Update()
{
    // 方式 1: 使用 Input Manager
    float horizontal = Input.GetAxis("Horizontal");
    float vertical = Input.GetAxis("Vertical");
    
    // 方式 2: 直接檢測按鍵
    if (Input.GetKeyDown(KeyCode.Space))
    {
        // 空白鍵被按下
    }
    
    // 方式 3: 檢測按鈕
    if (Input.GetButtonDown("Jump"))
    {
        // 跳躍按鈕被按下
    }
}
```

### 檢查清單

- ✅ 確認腳本已正確掛載到 GameObject
- ✅ 確認 GameObject 處於啟用狀態
- ✅ 確認腳本中的 `Update()` 方法正確

---

## 解決方案 5: Unity Editor 焦點問題

### 確認 Editor 有焦點

1. 點擊 Unity Editor 視窗確保它有焦點
2. 點擊 Game 視窗確保輸入被接收
3. 確保沒有其他應用程式攔截輸入（例如：輸入法、截圖工具）

### 檢查 Game Window 設定

1. 在 Game 視窗右上角
2. 確保 `Maximize On Play` 沒有影響輸入
3. 嘗試切換 `Stats` 按鈕

---

## 解決方案 6: 檢查 Console 錯誤

### 步驟

1. 開啟 Console 視窗：`Window` → `General` → `Console`

2. 查看是否有紅色錯誤訊息

3. 常見的輸入相關錯誤：
   - `Input System package not found`
   - `EventSystem missing`
   - `NullReferenceException` in input scripts

---

## 解決方案 7: 重新安裝 EasyTouch

如果 EasyTouch 造成問題：

### 步驟

1. 在 Project 視窗中找到 `Assets/EasyTouchBundle`

2. 右鍵 → `Reimport`

3. 或完全移除後重新匯入：
   - 刪除 `Assets/EasyTouchBundle` 資料夾
   - 重新下載並匯入 EasyTouch

---

## 解決方案 8: 簡化測試

建立一個簡單的測試腳本：

```csharp
using UnityEngine;

public class InputTest : MonoBehaviour
{
    void Update()
    {
        // 測試鍵盤輸入
        if (Input.anyKeyDown)
        {
            Debug.Log("檢測到按鍵：" + Input.inputString);
        }
        
        // 測試特定按鍵
        if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("W 鍵被按下");
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("空白鍵被按下");
        }
        
        // 測試滑鼠
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("滑鼠左鍵被按下");
        }
        
        // 測試軸向輸入
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        
        if (h != 0 || v != 0)
        {
            Debug.Log($"輸入: H={h}, V={v}");
        }
    }
}
```

### 使用方法

1. 建立新的 C# 腳本 `InputTest.cs`
2. 將腳本掛載到 Main Camera 或任何 GameObject
3. 進入 Play Mode
4. 查看 Console 是否有輸出

---

## 快速檢查清單

- [ ] EventSystem 存在於場景中
- [ ] EasyTouch 設定正確
- [ ] 沒有 Console 錯誤
- [ ] Unity Editor 有焦點
- [ ] Game Window 有焦點
- [ ] 輸入腳本正確掛載
- [ ] Play Mode Settings 正確

---

## 進階診斷

如果以上方法都無效，請提供：

1. Unity Console 的錯誤訊息
2. 正在測試的場景名稱
3. 預期應該響應輸入的 GameObject 名稱
4. 使用的輸入方式（鍵盤、滑鼠、觸控）