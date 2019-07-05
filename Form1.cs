using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Graphs
{
    public partial class Form1 : Form
    {
        Graph a = new Graph();
        Label[] labels;
        TextBox focusedTB;
        bool flag = true;

        public Form1()
        {
            InitializeComponent();
            foreach (TextBox textBox in Controls.OfType<TextBox>())
                textBox.GotFocus += new EventHandler(textBox_GotFocus);
        }

        void textBox_GotFocus(object sender, EventArgs e)
        {
            focusedTB = (sender as TextBox);
        }

        private void BlockEnter(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;

            if (!Char.IsDigit(number) && number != 8)
            {
                e.Handled = true;
                focusedTB.BackColor = Color.LightSalmon;
            }
            else focusedTB.BackColor = Color.White;
        }

        private void NoError()
        {
            textBox2.BackColor = Color.White;
            textBox3.BackColor = Color.White;
            textBox4.BackColor = Color.White;
            textBox5.BackColor = Color.White;
            textBox6.BackColor = Color.White;
        }

        private void DelLabels()
        {
            if(labels != null)
            foreach (Label l in labels)
                pictureBox1.Controls.Remove(l);
        }

        private bool Check_Root()
        {
            bool flag = false;
            for (int i = 0; i < a.Get_NumV(); i++)
            {
                if (a.Get_Coef(0,i) != 0)
                {
                    flag = true;
                }
            }
            return flag;
        }

        private bool Сheck_Connectivity(List<Points> PL)
        {
            int count = 0;

            for (int i = 1; i < a.Get_NumV(); i++)
                for (int j = i; j < a.Get_NumV(); j++)
                {
                    if (a.Get_Coef(i, j) == 1) count++;
                }

            return (PL.Count == count) ? true : false;
        }

        void ShowGraph()
        {
            Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics g = Graphics.FromImage(bmp);
            pictureBox1.Image = bmp;

            if (a.Get_NumV() == 0) return;

            Pen myPen = new Pen(Color.DarkSlateGray, 5);
            Pen myPen1 = new Pen(Color.DarkSlateGray, 1.5f);
            SolidBrush Brush = new SolidBrush(Color.DarkSlateGray);

            int R = 175;
            Point Centr = new Point
            {
                X = pictureBox1.Width / 2,
                Y = pictureBox1.Height / 2
            };

            Point P = new Point();

            int i = 0;
            labels = new Label[a.Get_NumV() + 1];

            while (i <= a.Get_NumV())
            {
                P.X = Centr.X + (int)(Math.Cos(i * 2 * Math.PI / a.Get_NumV()) * R);
                P.Y = Centr.Y - (int)(Math.Sin(i * 2 * Math.PI / a.Get_NumV()) * R);

                g.DrawEllipse(myPen, P.X, P.Y, 3, 3);
                g.FillEllipse(Brush, P.X, P.Y, 3, 3);

                labels[i] = new Label() { Text = (i+1).ToString(), Location = new Point(P.X + 5, P.Y + 5), ForeColor = Color.Firebrick,
                Size = new Size(25,25), Parent = pictureBox1, BackColor = Color.Transparent};

                labels[i].Font = new Font(labels[i].Font.Name, 10, labels[i].Font.Style);

                pictureBox1.Controls.Add(labels[i]);
                i++;
            }

            for (i = 0; i < a.Get_NumV(); i++)
            {
                for (int j = i + 1; j < a.Get_NumV(); j++)
                {
                    if (a.Get_Coef(i, j) == 1)
                    {
                        g.DrawLine(myPen1, labels[i].Location.X - 5, labels[i].Location.Y - 5,
                                          labels[j].Location.X - 5, labels[j].Location.Y - 5);
                    }
                }
            }
        }

        class Points
        {
            public int x { get; set; }
            public int y { get; set; }
            public int lvl { get; set; }
            public bool flag { get; set; }

            public void Swap(Points a)
            {
                Points buf = new Points() { x = a.x, y = a.y, lvl = a.lvl };
                a.x = x; a.y = y; a.lvl = lvl;
                x = buf.x; y = buf.y; lvl = buf.lvl;
            }
        }

        void ShowTree()
        {
            Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics g = Graphics.FromImage(bmp);
            pictureBox1.Image = bmp;

            if (a.Get_NumV() == 0) return;

            Pen myPen = new Pen(Color.DarkSlateGray, 5);
            Pen myPen1 = new Pen(Color.DarkSlateGray, 1.5f);
            SolidBrush Brush = new SolidBrush(Color.DarkSlateGray);

            List<Points> PL = new List<Points>();

            List<int> num = new List<int>();
            for (int c = 0; c < a.Get_NumV(); c++) num.Add(c);

            void func(int j)
            {
                List<int> L = a.Neighbors(j);

                num.Remove(j);
                for (int m = 0; m < a.Get_NumV(); m++)
                {
                    if (num.IndexOf(m) < 0)
                    {
                        if (L.IndexOf(m) >= 0)
                        {
                            L.Remove(m);
                        }
                    }
                }

                foreach (Points k in PL)
                {
                    List<int> N = a.Neighbors(k.x);
                    foreach (int t in N)
                    {
                        if (L.IndexOf(t) >= 0) L.Remove(t);
                    }
                }

                if (L.Count > 0)
                    foreach (int k in L)
                    {
                        if (num.IndexOf(k) >= 0)
                        {
                            PL.Add(new Points() { x = j, y = num[num.IndexOf(k)] });
                        }
                    }

                if (L.Count > 0)
                {
                    for (int m = 0; m < L.Count; m++)
                        func(L[m]);
                }
            }

            func(0);

            void SetLvls(int current, int lvl)
            {
                foreach (Points p in PL)
                {
                    if (p.x == current)
                    {
                        if (num.IndexOf(p.x) >= 0) lvl--;
                        num.Add(p.x);
                        p.lvl = lvl;
                       
                        f(p.y, ++lvl);
                    }
                }
                lvl--;
            }

            SetLvls(0, 0);

            int maxlvl = 0;
            foreach (Points k in PL)
                if (maxlvl < k.lvl) maxlvl = k.lvl;

            float heigh_lvl = pictureBox1.Height / (maxlvl + 2);

            int i = 0;
            int count = 0;
            int PLcount = 0;
            float width_lvl = 0;
            labels = new Label[a.Get_NumV() + 1];

            g.DrawEllipse(myPen, pictureBox1.Width / 2 - 2, 10 - 2, 4, 4);
            g.FillEllipse(Brush, pictureBox1.Width / 2 - 2, 10 - 2, 4, 4);

            labels[0] = new Label()
            {
                Text = 1.ToString(),
                Location = new Point(pictureBox1.Width / 2 + 5, 15),
                ForeColor = Color.Firebrick,
                Size = new Size(25, 25),
                Parent = pictureBox1,
                BackColor = Color.Transparent
            };
            labels[0].Font = new Font(labels[0].Font.Name, 10, labels[0].Font.Style);
            pictureBox1.Controls.Add(labels[0]);

            PL.Sort((a, b) => a.lvl.CompareTo(b.lvl));

            for (i = 0; i <= maxlvl; i++)
            {
                foreach (Points k in PL)
                {
                    if (k.lvl == i) count++;
                }
                width_lvl = pictureBox1.Width / (count + 1);

                for (int j = 1; j < count + 1; j++)
                {
                    g.DrawEllipse(myPen, width_lvl * j - 2, (i + 1) * heigh_lvl - 2, 4, 4);
                    g.FillEllipse(Brush, width_lvl * j - 2, (i + 1) * heigh_lvl - 2, 4, 4);

                    labels[PL[PLcount].y] = new Label()
                    {
                        Text = (PL[PLcount].y + 1).ToString(),
                        Location = new Point((int)width_lvl * j + 5, (i + 1) * (int)heigh_lvl + 5),
                        ForeColor = Color.Firebrick,
                        Size = new Size(25, 25),
                        Parent = pictureBox1,
                        BackColor = Color.Transparent
                    };

                    labels[PL[PLcount].y].Font = new Font(labels[PL[PLcount].y].Font.Name, 10, labels[PL[PLcount].y].Font.Style);
                    pictureBox1.Controls.Add(labels[PL[PLcount].y]);
                    
                    PLcount++;
                }
                count = 0;
            }

            for (int h = 0; h < PL.Count; h++)
            {
                g.DrawLine(myPen1, labels[PL[h].x].Location.X - 5, labels[PL[h].x].Location.Y - 5,
                labels[PL[h].y].Location.X - 5, labels[PL[h].y].Location.Y - 5);
            }

            if (Сheck_Connectivity(PL))
            {
                label1.ForeColor = Color.OrangeRed;
                label1.Text = "Несвязный граф\nОтображается одно дерево";
                return;
            }
        }

        private void считатьИзФайлаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DelLabels();
            label1.Text = "";
            a.ReadFromFile();
            if (flag) ShowGraph(); else ShowTree();
        }

        private void ввестиВФайлToolStripMenuItem_Click(object sender, EventArgs e)
        {
            a.WriteToFile();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DelLabels();
            label1.Text = "";
            a.AddVertex();
            if (flag) ShowGraph(); else ShowTree();
            NoError();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox2.Text == "" || textBox2.Text.Length > 2) { textBox2.BackColor = Color.LightSalmon; return; }
            DelLabels();
            label1.Text = "";
            a.DelVertex(Convert.ToInt32(textBox2.Text));
            if (flag) ShowGraph(); else ShowTree();
            textBox2.Clear();
            NoError();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(textBox3.Text == "" || textBox3.Text.Length > 2) { textBox3.BackColor = Color.LightSalmon; return; }
            if(textBox4.Text == "" || textBox4.Text.Length > 2) { textBox4.BackColor = Color.LightSalmon; return; }
            DelLabels();
            label1.Text = "";
            a.AddEdge(Convert.ToInt32(textBox3.Text) - 1, Convert.ToInt32(textBox4.Text) - 1);
            if (flag) ShowGraph(); else ShowTree();
            textBox3.Clear();
            textBox4.Clear();
            NoError();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox6.Text == "" || textBox6.Text.Length > 2) { textBox6.BackColor = Color.LightSalmon; return; }
            if (textBox5.Text == "" || textBox5.Text.Length > 2) { textBox5.BackColor = Color.LightSalmon; return; }
            DelLabels();
            label1.Text = "";
            a.DelEdge(Convert.ToInt32(textBox6.Text) - 1, Convert.ToInt32(textBox5.Text) - 1);
            if (flag) ShowGraph(); else ShowTree();
            textBox5.Clear();
            textBox6.Clear();
            NoError();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            DelLabels();
            if (Check_Root()) label1.Text = "";
            flag = true;
            ShowGraph();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (!Check_Root() && a.Get_NumV() != 0)
            {
                label1.ForeColor = Color.Red;
                label1.Text = "Степень вершины \"1\"\nдолжна превышать 0";
                radioButton2.Checked = false;
                radioButton1.Checked = true;
                return;
            }
            DelLabels();
            label1.Text = "";
            flag = false;
            ShowTree();
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            BlockEnter(sender, e);
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            BlockEnter(sender, e);
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            BlockEnter(sender, e);
        }

        private void textBox6_KeyPress(object sender, KeyPressEventArgs e)
        {
            BlockEnter(sender, e);
        }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            BlockEnter(sender, e);
        }
    }
}