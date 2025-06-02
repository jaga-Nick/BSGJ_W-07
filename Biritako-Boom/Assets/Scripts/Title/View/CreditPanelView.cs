using UnityEngine;

namespace Title.View
{
    public class CreditPanelView : MonoBehaviour
    {
        private GameObject panelObject;
        
        /// <summary>
        /// Panelを開く。
        /// </summary>
        public void OpenPanel()
        {
            panelObject.SetActive(true);
            Debug.Log(panelObject.name + " Opened");
        }

        /// <summary>
        /// Panelを閉じる。
        /// </summary>
        public void ClosePanel()
        {
            panelObject.SetActive(false);
            Debug.Log(panelObject.name + " Closed");
        }
        
        /// <summary>
        /// Panelの表示・非表示を切り替えます。
        /// </summary>
        public void TogglePanel()
        {
            if (!panelObject) return;
            if (panelObject.activeSelf)
            {
                ClosePanel();
            }
            else
            {
                OpenPanel();
            }
        }

        /// <summary>
        /// Panelが現在開いているかどうかを返します。
        /// </summary>
        public bool IsOpen => panelObject && panelObject.activeSelf;
    }
}