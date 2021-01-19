using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AHP
{
    static class TabHandle
    {
        public static void MakeReadOnly(TabPage tab)
        {
            TextBox[] tboxes = FindTextBoxes(tab);
            foreach (TextBox tbox in tboxes)
                tbox.ReadOnly = true;
        }
        public static TextBox[] FindTextBoxes(TabPage tab)
        {
            List<TextBox> t = new List<TextBox>();
            foreach (Control ctrl in tab.Controls)
            {
                if ((ctrl as TextBox) != null)
                    t.Add(ctrl as TextBox);
                else if ((ctrl as TabControl) != null)
                    return FindTextBoxes((ctrl as TabControl).SelectedTab);
                else if ((ctrl as TableLayoutPanel) != null)
                    return FindTextBoxesOnTableLayoutPanel(ctrl as TableLayoutPanel);
            }
            return t.ToArray();
        }
        private static TextBox[] FindTextBoxesOnTableLayoutPanel(TableLayoutPanel p)
        {
            List<TextBox> t = new List<TextBox>();
            foreach (Control ctrl in p.Controls)
                if ((ctrl as TextBox) != null)
                    t.Add(ctrl as TextBox);
            return t.ToArray();
        }
        public static Button FindButton(TabPage tab)
        {
            Button btn = new Button();
            foreach (Control ctrl in tab.Controls)
            {
                if ((ctrl as TabControl) != null)
                    foreach (TabPage tabp in (ctrl as TabControl).TabPages)
                        btn = FindButton(tabp);
                if ((ctrl as Button) != null)
                    return ctrl as Button;
            }
            return btn;
        }
    }
}
