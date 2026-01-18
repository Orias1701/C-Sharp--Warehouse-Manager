using System.Drawing;
using System.Windows.Forms;
using WarehouseManagement.UI;
using WarehouseManagement.UI.Components;

namespace WarehouseManagement.Views.Commons
{
    /// <summary>
    /// Content component - Vùng hiển thị nội dung chính (tables, panels)
    /// </summary>
    public class Content : CustomPanel
    {
        public Content()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Panel configuration
            BackColor = ThemeManager.Instance.BackgroundDefault;
            ShowBorder = true;
            BorderRadius = UIConstants.Borders.RadiusMedium;
            Padding = new Padding(0);
        }
    }
}
