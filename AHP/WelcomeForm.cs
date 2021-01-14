using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AHP
{
    public partial class WelcomeForm : Form
    {
        public WelcomeForm()
        {
            InitializeComponent();
            panel1.Width = this.Width - 10;
            label1.MaximumSize = new Size(panel1.Width - 30, 0);
            label1.Text = "Метод анализа иерархий — математический инструмент системного подхода " +
                            "к сложным проблемам принятия решений. Разработан американским математиком Томасом Саати.\r\n" +
                            "Анализ проблемы принятия решений в МАИ начинается с построения иерархической структуры, " +
                            "которая включает цель, критерии, альтернативы и другие рассматриваемые факторы, влияющие на выбор. " +
                            "Эта структура отражает понимание проблемы лицом, принимающим решение.\r\n" +
                            "Следующим этапом анализа является определение приоритетов, представляющих относительную важность или предпочтительность элементов построенной иерархической структуры, с помощью процедуры парных сравнений. На заключительном этапе анализа выполняется синтез (линейная свертка) приоритетов на иерархии, в результате которой вычисляются приоритеты альтернативных решений относительно главной цели. Лучшей считается альтернатива с максимальным значением приоритета.";
            pictureBox1.Width = label1.Width;
            pictureBox1.Location = new Point(pictureBox1.Location.X, label1.Bottom + 15);
            label2.Location = new Point((panel1.Width - label2.Width) / 2, pictureBox1.Bottom + 15);
            Label howToUse = new Label();
            howToUse.Location = new Point(0, label2.Bottom + 25);
            howToUse.AutoSize = true;
            howToUse.Font = new Font("Microsoft Sans Serif", 10.0f);
            howToUse.Text = "1. В первой вкладке введите цель, количество уровней иерархии, альтернатив и критериев." +
                            "Простые задачи состоят из 3-х уровней (1 уровень критериев), но если потребуется больше, то в следующей подвкладке надо ввести количество критериев на каждом своём уровне.\r\n\n" +
                            "2. В следующих двух вкладках введите названия критериев и альтернатив соответственно.\r\n\n" +
                            "3. Далее оцениваются критерии, начиная с высшего уровня, по шкале Саати. Так же в предпоследней вкладке оцениваются альтернативы по критериям нижнего уровня.\r\n" +
                            "    Шкала Саати:\r\n" +
                            "     1 - Равная важность\r\n" +
                            "     3 - Умеренное превосходство одного над другим\r\n" +
                            "     5 - Существенное или сильное превосходство\r\n" +
                            "     7 - Значительное превосходство\r\n" +
                            "     9 - Очень сильное превосходство\r\n" +
                            "     2, 4, 6, 8 - Промежуточные значения\r\n" +
                            "При необходимости можно изменить введённые оценки на текущей вкладке (Правка - Изменить данные).\r\n\n" +
                            "4. На вкладке с результатами будут показаны вычисленные приоритеты каждой альтернативы.";
            howToUse.MaximumSize = new Size(panel1.Width - 30, 0);
            panel1.Controls.Add(howToUse);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void WelcomeForm_Shown(object sender, EventArgs e)
        {
            VerticalScroll.Value = 0;
        }
    }
}
