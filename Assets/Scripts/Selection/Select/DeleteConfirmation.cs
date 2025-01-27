using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArcCreate.Storage;
using ArcCreate.Storage.Data;
using ArcCreate.Utility.Animation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ArcCreate.Selection.Select
{
    public class DeleteConfirmation : MonoBehaviour
    {
        [SerializeField] private StorageData storageData;
        [SerializeField] private TMP_Text listText;
        [SerializeField] private Button deleteButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private ScriptedAnimator animator;

        private List<IStorageUnit> storageUnits;

        public void PromptUser(IEnumerable<IStorageUnit> units)
        {
            storageUnits = units.ToList();
            StringBuilder str = new StringBuilder();
            foreach (var st in storageUnits)
            {
                if (!st.IsDefaultAsset)
                {
                    str.Append($"{st.Type}: {st.Identifier}");
                }
            }

            animator.Show();
        }

        private void Awake()
        {
            deleteButton.onClick.AddListener(Delete);
            cancelButton.onClick.AddListener(Cancel);
        }

        private void OnDestroy()
        {
            deleteButton.onClick.RemoveListener(Delete);
            cancelButton.onClick.RemoveListener(Cancel);
        }

        private void Cancel()
        {
            animator.Hide();
        }

        private void Delete()
        {
            foreach (var st in storageUnits)
            {
                if (!st.IsDefaultAsset)
                {
                    st.Delete();
                }
            }

            storageUnits.Clear();
            animator.Hide();
            Services.Select.ClearSelection();
            storageData.NotifyStorageChange();
        }
    }
}