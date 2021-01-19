using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace AHP
{
    public partial class Form1 : Form
    {
        TextBox[] t;
        bool editing = false;
        public Form1()
        {
            InitializeComponent();
            SetDefaultValues();
        }
        private void SetDefaultValues()
        {
            tabPage1.Text = "Начало";
            tabPage7.Text = "Начало";
            tabPage2.Text = "Критерии";
            tabPage3.Text = "Альтернативы";
            tabPage4.Text = "Сравнение критериев";
            tabPage5.Text = "Сравнение альтернатив";
            tabPage6.Text = "Результаты";
            foreach (TabPage tab in tabControl1.TabPages)
                tab.Leave += Tab_Leave;
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Exit();
        }

        private void Tab_Leave(object sender, EventArgs e)
        {
            if (editing)
            {
                DialogResult confirm = MessageBox.Show("Сохранить изменённые данные?", "Внимание!", MessageBoxButtons.YesNo);
                if (confirm == DialogResult.Yes)
                    saveDataToolStripMenuItem.PerformClick();
                else
                {
                    TabHandle.MakeReadOnly(tabControl1.SelectedTab);
                    editing = false;
                    editDataToolStripMenuItem.Enabled = true;
                    saveDataToolStripMenuItem.Enabled = false;
                }
            }
        }
        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;
            Brush _textBrush;
            // Get the item from the collection.
            TabPage _tabPage = tabControl1.TabPages[e.Index];
            // Get the real bounds for the tab rectangle.
            Rectangle _tabBounds = tabControl1.GetTabRect(e.Index);
            if (e.State == DrawItemState.Selected)
            {
                // Draw a different background color, and don't paint a focus rectangle.
                _textBrush = new SolidBrush(Color.Yellow);
                g.FillRectangle(Brushes.Blue, e.Bounds);
            }
            else
            {
                _textBrush = new System.Drawing.SolidBrush(e.ForeColor);
                e.DrawBackground();
            }
            // Use our own font.
            Font _tabFont = new Font("Tahoma", 12.0f, FontStyle.Bold, GraphicsUnit.Pixel);
            // Draw string. Center the text.
            StringFormat _stringFlags = new StringFormat();
            _stringFlags.Alignment = StringAlignment.Center;
            _stringFlags.LineAlignment = StringAlignment.Center;
            g.DrawString(_tabPage.Text, _tabFont, _textBrush, _tabBounds, new StringFormat(_stringFlags));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.tb_problemName.Text == "" || this.tb_levels.Text == "" || this.textBox1.Text == "" || (this.textBox2.Visible && this.textBox2.Text == ""))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }
            if (Convert.ToInt32(tb_levels.Text) < 3 || Convert.ToInt32(tb_levels.Text) > 9 || Convert.ToInt32(textBox1.Text) < 2 || Convert.ToInt32(textBox1.Text) > 10 || (textBox2.Visible && (Convert.ToInt32(textBox2.Text) < 2 || Convert.ToInt32(textBox2.Text) > 10)))
            {
                MessageBox.Show("Проверьте правильность заполнения полей!");
                return;
            }
            AHP.problemName = this.tb_problemName.Text;
            AHP.levels = Convert.ToInt32(this.tb_levels.Text) - 1;
            AHP.alternatives_q = Convert.ToInt32(this.textBox1.Text);
            AHP.criterias_q = new int[AHP.levels - 1];
            if (AHP.levels == 2)
            {
                AHP.criterias_q[0] = Convert.ToInt32(textBox2.Text);
                TabCriteriasShow();
                tabControl1.SelectedTab = tabPage2;
            }
            else if (AHP.levels >= 3)
            {
                TabPage crL = new TabPage("Уровни критериев");
                crL.Name = "tabCrL";
                tabControl2.TabPages.Add(crL);
                TabCriteriaLevelsShow(crL);
                tabControl2.SelectedTab = crL;
            }
            tb_problemName.ReadOnly = tb_levels.ReadOnly = textBox1.ReadOnly = textBox2.ReadOnly = true;
            editProblemToolStripMenuItem.Enabled = true;
            (sender as Button).Visible = false;
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((sender as TextBox).Text == "" && e.KeyChar == '0')
            {
                e.Handled = true;
                return;
            }
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
                return;
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((sender as TextBox).Text == "" && e.KeyChar == '0')
            {
                e.Handled = true;
                return;
            }
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
                return;
            }
        }

        private void tb_levels_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((sender as TextBox).Text == "" && (e.KeyChar < '3' || e.KeyChar > '9'))
            {
                e.Handled = true;
                return;
            }
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
                return;
            }
        }

        private void tb_levels_TextChanged(object sender, EventArgs e)
        {
            if ((sender as TextBox).Text == "3")
            {
                label4.Visible = true;
                textBox2.Visible = true;
            }
            else
            {
                label4.Visible = false;
                textBox2.Visible = false;
            }
        }
        
        private void NewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (AHP.problemName != null)
            {
                DialogResult confirm = MessageBox.Show("Сохранить состояние?", "Новая задача", MessageBoxButtons.YesNoCancel);
                if (confirm == DialogResult.Yes)
                    saveStateToolStripMenuItem.PerformClick();
                else if (confirm == DialogResult.Cancel)
                    return;
            }
            AHP.Reset();
            this.Hide();
            this.Controls.Clear();
            Form1 f = new Form1();
            f.NormalizeSize();
            f.Size = this.Size;
            f.Show();
            f.Location = this.Location;
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult confirm = MessageBox.Show("Выйти из программы?", "Выход", MessageBoxButtons.YesNo);
            if (confirm == DialogResult.Yes)
                Application.Exit();
        }
        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WelcomeForm f = new WelcomeForm();
            f.Text = "Справка";
            f.Show();
            f.VerticalScroll.Value = 0;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            tabControl1.Size = new Size(this.Width - 20, this.Height - 60);
            foreach (TabPage tab in tabControl1.TabPages)
                tab.Size = new Size(tabControl1.Width, tabControl1.Height);
            tabControl2.Size = new Size(tabControl2.Parent.Width, tabControl2.Parent.Height);
            tabControl3.Size = new Size(tabControl3.Parent.Width, tabControl3.Parent.Height);
            tabControl4.Size = new Size(tabControl4.Parent.Width, tabControl4.Parent.Height);
            sort.Location = new Point((sort.Parent.Width - sort.Width - radioButton1.Width - radioButton2.Width) / 2, sort.Location.Y);
            radioButton1.Location = new Point(sort.Right + 10, radioButton1.Location.Y);
            radioButton2.Location = new Point(radioButton1.Right + 10, radioButton2.Location.Y);
            if (tabControl1.SelectedTab == tabPage4 && tabControl3.TabPages.Count > 0)
            {
                ResizeControls(tabControl3.SelectedTab);
            }
            else if (tabControl1.SelectedTab == tabPage5 && tabControl4.TabPages.Count > 0)
            {
                ResizeControls(tabControl4.SelectedTab);
            }
        }
        private void ResizeControls(TabPage tab)
        {
            TableLayoutPanel tabl = new TableLayoutPanel();
            foreach (Control c in tab.Controls)
            {
                if ((c as TableLayoutPanel) != null)
                    tabl = c as TableLayoutPanel;
            }
            if (tab.Width < tabl.Width)
            {
                tab.HorizontalScroll.Value = 0;
                tabl.Location = new Point(20, tabl.Location.Y);
            }
            else
                tabl.Location = new Point((tab.Width - tabl.Width) / 2, tabl.Location.Y);
            foreach (Control c in tab.Controls)
                if ((c as Label) != null || (c as Button) != null)
                    c.Location = new Point((tabl.Width + tabl.Left * 2 - c.Width) / 2, c.Location.Y);
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 3 && tabControl3.TabPages.Count > 0)
            {
                editDataToolStripMenuItem.Enabled = true;
                ResizeControls(tabControl3.SelectedTab);
            }
            else if (tabControl1.SelectedIndex == 4 && tabControl4.TabPages.Count > 0)
            {
                editDataToolStripMenuItem.Enabled = true;
                ResizeControls(tabControl4.SelectedTab);
            }
            else
                editDataToolStripMenuItem.Enabled = false;
        }
        private void tabControl34_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResizeControls((sender as TabControl).SelectedTab);
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            NormalizeSize();
        }
        private void NormalizeSize()
        {
            if (this.Size == MinimumSize)
            {
                this.Size = new Size(this.Width + 1, this.Height + 1);
                this.Size = new Size(this.Width - 1, this.Height - 1);
            }
            else
            {
                this.Size = new Size(this.Width - 1, this.Height - 1);
                this.Size = new Size(this.Width + 1, this.Height + 1);
            }
        }
    }
}
