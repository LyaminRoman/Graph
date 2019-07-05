using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Collections;

namespace Graphs
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

        }
    }


    class Graph
    {
        int num_v;
        int[,] matrix;

        public void ReadFromFile()
        {
            if (!File.Exists("Matrix.txt")) return;

            using (TextReader Reader = File.OpenText("Matrix.txt"))
            {
                num_v = int.Parse(Reader.ReadLine());

                matrix = new int[num_v, num_v];

                for (int i = 0; i < num_v; i++)
                {
                    string s = Reader.ReadLine();
                    double si = int.Parse(s.Replace(" ", string.Empty));

                    for (int j = 0; j < num_v; j++)
                    {
                        matrix[i, j] = (int)(si / Math.Pow(10, num_v - j - 1));
                        si %= Math.Pow(10, num_v - j - 1);
                    }
                }
            }
        }

        public void WriteToFile()
        {
            using (StreamWriter sw = new StreamWriter(@"Matrix.txt", false, System.Text.Encoding.Default))
            {
                string[] s = new string[num_v + 1];
                s[0] = Convert.ToString(num_v);

                for (int i = 0; i < num_v; i++)
                {
                    for (int j = 0; j < num_v; j++)
                        s[i + 1] += Convert.ToString(matrix[i, j]) + " ";
                }

                for (int i = 0; i < num_v + 1; i++)
                    sw.WriteLine(s[i]);
            }
        }

        public void ResizeArrayPlus()
        {
            int[,] newArray = new int[num_v, num_v];

            for (int i = 0; i < num_v; i++)
                for (int j = 0; j < num_v; j++)
                {
                    if (i== num_v - 1 || j == num_v - 1)
                    {
                        newArray[i, j] = 0;
                        continue;
                    }
                    newArray[i, j] = matrix[i, j];
                }
                matrix = newArray;
        }

        public void ResizeArrayMinus(int v)
        {
            int[,] newArray = new int[num_v, num_v];
            int i2 = 0, j2 = 0;
            v--;

            for (int i = 0; i < num_v + 1; i++)
            {
                for (int j = 0; j < num_v + 1; j++)
                {
                    if (i != v && j != v)
                    {
                        newArray[i2, j2] = matrix[i, j];
                        j2++;
                    }
                }
                if(j2 > 0) i2++;
                j2 = 0;
            }
            matrix = newArray;
        }

        public int Get_NumV() => num_v;

        public int Get_Coef(int i, int j) => matrix[i, j];

        public void AddVertex()
        {
            num_v++;
            ResizeArrayPlus();
        }

        public void DelVertex(int v)
        {
            if (v > num_v || v < 1) return;
            num_v--;
            ResizeArrayMinus(v);
        }

        public  void AddEdge(int i, int j)
        {
            if (i > num_v - 1 || i < 0 || j > num_v - 1 || j < 0) return;
            matrix[i, j] = 1;
            matrix[j, i] = 1;
        }

        public void DelEdge(int i, int j)
        {
            if (i > num_v - 1 || i < 0 || j > num_v - 1 || j < 0) return;
            matrix[i, j] = 0;
            matrix[j, i] = 0;
        }

        public List<int> Neighbors(int v)
        {
            List<int> L = new List<int>();

            for (int i = 0; i < num_v; i++)
            {
                if (matrix[i, v] == 1)
                    L.Add(i);
            }
            return L;
        }
    }
}