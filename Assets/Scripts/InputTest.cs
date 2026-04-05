using UnityEngine;

public class InputTest : MonoBehaviour
{
    void Update()
    {
        // 測試任意按鍵
        if (Input.anyKeyDown)
        {
            Debug.Log($"[InputTest] 檢測到按鍵: {Input.inputString}");
        }
        
        // 測試特定按鍵
        if (Input.GetKeyDown(KeyCode.W))
            Debug.Log("[InputTest] W 鍵被按下");
        
        if (Input.GetKeyDown(KeyCode.A))
            Debug.Log("[InputTest] A 鍵被按下");
        
        if (Input.GetKeyDown(KeyCode.S))
            Debug.Log("[InputTest] S 鍵被按下");
        
        if (Input.GetKeyDown(KeyCode.D))
            Debug.Log("[InputTest] D 鍵被按下");
        
        if (Input.GetKeyDown(KeyCode.Space))
            Debug.Log("[InputTest] 空白鍵被按下");
        
        if (Input.GetKeyDown(KeyCode.Return))
            Debug.Log("[InputTest] Enter 鍵被按下");
        
        if (Input.GetKeyDown(KeyCode.Escape))
            Debug.Log("[InputTest] ESC 鍵被按下");
        
        // 測試滑鼠
        if (Input.GetMouseButtonDown(0))
            Debug.Log("[InputTest] 滑鼠左鍵被按下");
        
        if (Input.GetMouseButtonDown(1))
            Debug.Log("[InputTest] 滑鼠右鍵被按下");
        
        // 測試軸向輸入
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        
        if (h != 0 || v != 0)
            Debug.Log($"[InputTest] 軸向輸入: H={h:F2}, V={v:F2}");
    }
    
    void OnGUI()
    {
        // 在畫面上顯示提示
        GUI.color = Color.white;
        GUI.Label(new Rect(10, 10, 300, 100), 
            "Input Test Active\n" +
            "按下任意鍵查看 Console 輸出\n" +
            "WASD: 移動\n" +
            "Space: 跳躍\n" +
            "Mouse: 點擊");
    }
}