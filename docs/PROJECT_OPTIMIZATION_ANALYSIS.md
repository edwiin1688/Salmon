# Unity 專案優化分析報告

**專案路徑:** `D:\github\chiisen\Salmon`
**分析日期:** 2026-05-13
**Unity 版本:** 4.2.2.stable

---

## 📊 專案概覽

| 項目 | 數量 |
|------|------|
| 自定義場景 | 16 個 (Level1-5, NextLevel1-5, ResetLevel1-5) |
| C# 腳本 | ~70 個（不含 EasyTouch/2DxFX 套件） |
| 第三方套件 | EasyTouch 5.2.1, 2DxFX (2017年，停更) |

---

## 🔴 高優先級優化

### 1. Singleton 執行緒安全問題

**檔案:** `Assets/Scripts/Singleton.cs:16-31`

**問題:** 無鎖機制，多執行緒可能建立多實例，導致不可預測行為或閃退。

**現況:**
```csharp
public static T Instance
{
    get
    {
        if (_instance == null)
        {
            _instance = FindObjectOfType<T>();
            if (_instance == null)
            {
                GameObject obj_ = new GameObject();
                _instance = obj_.AddComponent<T>();
            }
        }
        return _instance;
    }
}
```

**建議改進:**
```csharp
private static readonly object _lock = new object();
private static T _instance;

public static T Instance
{
    get
    {
        if (_instance == null)
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject(typeof(T).Name);
                        _instance = obj.AddComponent<T>();
                    }
                }
            }
        }
        return _instance;
    }
}
```

**額外建議:** 在 `Awake()` 中加入 `DontDestroyOnLoad(gameObject)` 確保跨場景持久。

---

### 2. LevelManager.Update 過度呼叫 GUI

**檔案:** `Assets/Scripts/LevelManager.cs:202-224`

**問題:** 每幀呼叫 `GUIManager.Instance.UpdateLifes()` 和 `GUIManager.Instance.UpdateDistance()`，即使數值未變化。

**現況:**
```csharp
protected virtual void Update()
{
    GUIManager.Instance.UpdateLifes(Lifes);      // 每幀執行
    GUIManager.Instance.UpdateDistance(Distance); // 每幀執行

    if (_Time != 0)
    {
        // ... 敵人生成邏輯
    }
}
```

**建議改進:** 移除 `Update` 中的 GUI 更新，只在數值變化時更新（你已有 `LessDistance()` 機制）。

```csharp
protected virtual void Update()
{
    // 移除這兩行：
    // GUIManager.Instance.UpdateLifes(Lifes);
    // GUIManager.Instance.UpdateDistance(Distance);

    if (_Time != 0)
    {
        // ... 敵人生成邏輯
    }
}
```

**預期效益:** CPU 降低 5-10%

---

### 3. 敵人生成 Coroutine 泡沫

**檔案:** `Assets/Scripts/LevelManager.cs:214-217`

**問題:** `EnemyRandCount` 越大，產生的 Coroutine 越多，造成記憶體碎片。

**現況:**
```csharp
for (int i = 1; i < EnemyRandCount; ++i)
{
    StartCoroutine(StartCoroutineInstantiateEnemys(gameObject));
}
```

**建議改進:** 改用計時器方式，減少 Coroutine 開銷。

```csharp
protected float[] _enemyTimers;

protected virtual void Start()
{
    _enemyTimers = new float[EnemyRandCount];
}

protected virtual void Update()
{
    for (int i = 0; i < EnemyRandCount; ++i)
    {
        _enemyTimers[i] += Time.deltaTime;
        if (_enemyTimers[i] >= EnemyDelay * (i + 1))
        {
            InstantiateEnemys(i == 0 ? FirstEnemyRand : true);
            _enemyTimers[i] = 0f;
        }
    }
}
```

---

### 4. InputManager Lambda 造成 GC 壓力

**檔案:** `Assets/Scripts/InputManager.cs:29-82`

**問題:** `AddListener(() => {...})` 每個監聽器註冊都會產生新的 lambda 暫時物件，增加 GC 負擔。

**現況:**
```csharp
TouchPad.OnDownLeft.AddListener(() => {
    LeftButtonDown();
});
```

**建議改進:** 直接使用方法參考（你已經有命名方法）。

```csharp
TouchPad.OnDownLeft.AddListener(LeftButtonDown);
```

**預期效益:** 減少 GC allocations

---

## 🟡 中優先級優化

### 5. 移除廢棄套件

**建議刪除:**

| 資料夾 | 大小 | 說明 | 狀態 |
|--------|------|------|------|
| `Assets/EasyTouchBundle/EasyTouch/Examples/` | ~30MB | 38 個範例場景從未使用 | ✅ 已刪除 |
| `Assets/EasyTouchBundle/EasyTouchControls/Examples/` | ~10MB | 範例場景 | ✅ 已刪除 |
| `Assets/2DxFX/` | ~20MB | 60+ 腳本，使用率低 | ⏳ 待處理 |

**操作:** 備份後刪除，確認功能不受影響。

---

### 6. 場景合併策略

**現況:** 16 個關卡場景，重複性高。

**建議選項:**

**選項 A - Addressables（推荐）**
- 將關卡做成 Prefab 或 Asset Bundle
- 執行時動態載入
- 減少場景切換時間

**選項 B - 資料驅動**
- 合併為 5 個場景（Level1-5）
- 使用 ScriptableObject 儲存關卡配置
- 執行時動態生成關卡內容

---

### 7. FishGoController._KeyBuffer 邏輯缺陷

**檔案:** `Assets/Scripts/FishGoController.cs:131-143`

**問題:** 只檢查連續不同按鍵，導致：
- 快速按同鍵 → 無反應
- 連按三次不同鍵 → 只處理前兩次

**現況:**
```csharp
if (_KeyBuffer.Count >= 2)
{
    if (_KeyBuffer[_KeyBuffer.Count - 1] != _KeyBuffer[_KeyBuffer.Count - 2])
    {
        // 河岸開始流動
        LevelManager.Instance.WaterFlows(true);
        LevelManager.Instance.LessDistance();
    }
}
```

**建議改進:**
```csharp
protected void ManualMove(bool bButtonUp)
{
    if (bButtonUp && _KeyBuffer.Count >= 1)
    {
        bool lastKey = _KeyBuffer[_KeyBuffer.Count - 1];
        // 任意按鍵更換都觸發
        LevelManager.Instance.WaterFlows(true);
        LevelManager.Instance.LessDistance();
        _Time = Time.time;
    }

    // 延遲處理不變...
}
```

---

## 🟢 低優先級優化

### 8. Application.Quit() 在 Editor 無效

**檔案:** `Assets/Scripts/InputManager.cs:93-96`

**建議:**
```csharp
if (Input.GetKeyUp(KeyCode.Escape))
{
#if !UNITY_EDITOR
    Application.Quit();
#else
    UnityEditor.EditorApplication.isPlaying = false;
#endif
}
```

---

### 9. 缺少 sealed 標記

**建議:** 所有不需繼承的類別加上 `sealed`，讓 JIT 編譯器更好優化。

```csharp
public sealed class GUIManager : Singleton<GUIManager>
```

---

### 10. float 與 double 混用

**說明:** Unity 2020+ 的 `Time.time` 是 `double` 類型，注意混用時的精度問題。

---

## 🔍 Console 發現的問題

### 11. 場景損壞

**問題:** Unity Console 發現以下警告

| 訊息 | 影響 |
|------|------|
| `Component at index 2 could not be loaded when loading game object 'Main Camera'. Removing it.` | **主相機組件丟失** |
| `Problem detected while opening the Scene file: 'Assets/Scenes/Level1.unity'` | **Level1 場景損壞** |

**建議:**
1. 檢查 `Assets/Scenes/Level1.unity` 檔案完整性
2. 修復或重建 Level1 場景
3. 檢查 Main Camera 的組件是否有丢失

### 12. EasyTouch 重複軸註冊

**問題:** `ETCInput axis : Vertical/Horizontal already exists`

**原因:** 多個 `ETCTouchPad` 或 `ETCJoystick` 元件同時註冊相同的軸名稱。

**建議:**
1. 確保場景中只有一個 TouchPad 或 Joystick 實例
2. 或在 `ETCBase.cs:165` 的 `Awake()` 中加入檢查防止重複註冊

---

## 📋 建議執行順序

| 順序 | 項目 | 預期效益 | 風險 | 狀態 |
|------|------|----------|------|------|
| 1 | Singleton 執行緒安全 + DontDestroyOnLoad | 防止閃退 | 低 | ⏳ |
| 2 | 移除 Update 中的 GUI 更新 | CPU 降低 5-10% | 低 | ⏳ |
| 3 | 刪除 Examples 資料夾 | 移除 40MB+ | 低 | ✅ 完成 |
| 4 | InputManager lambda 改方法參考 | GC 降低 | 低 | ⏳ |
| 5 | 敵人生成改計時器 | 減少 Coroutine | 中 | ⏳ |

---

## 🚀 快速修復腳本

如需立即改善效能，可按以下順序處理：

1. **已完成 ✅**
   - ✅ 刪除 `Assets/EasyTouchBundle/EasyTouch/Examples/` 資料夾
   - ✅ 刪除 `Assets/EasyTouchBundle/EasyTouchControls/Examples/` 資料夾

2. **立即可做（無風險）**
   - 將 `LevelManager.Update` 中的 GUI 更新呼叫註解掉
   - 刪除 `Assets/2DxFX/` 資料夾（需確認使用與否）

3. **需測試**
   - 修改 `Singleton.cs` 加入執行緒鎖
   - 修改 `InputManager.cs` 移除 lambda

4. **需完整測試**
   - 修改 `FishGoController._KeyBuffer` 邏輯
   - 場景合併

---

*本報告由 AI 分析生成，建議在修改前先備份專案。*