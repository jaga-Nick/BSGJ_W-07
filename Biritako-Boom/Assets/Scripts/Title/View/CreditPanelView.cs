using UnityEngine;

namespace Title.View
{
    public class CreditPanelView : MonoBehaviour
    {
        private GameObject _panelObject;
        
        /// <summary>
        /// Panelを開く。
        /// </summary>
        public void OpenPanel()
        {
            _panelObject.SetActive(true);
            Debug.Log(_panelObject.name + " Opened");
        }

        /// <summary>
        /// Panelを閉じる。
        /// </summary>
        public void ClosePanel()
        {
            _panelObject.SetActive(false);
            Debug.Log(_panelObject.name + " Closed");
        }
        
        /// <summary>
        /// Panelの表示・非表示を切り替えます。
        /// </summary>
        public void TogglePanel()
        {
            if (!_panelObject) return;
            if (_panelObject.activeSelf)
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
        public bool IsOpen => _panelObject && _panelObject.activeSelf;
    }
}