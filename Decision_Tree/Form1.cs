using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Decision_Tree
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public static double[,] predefinedProfit1 = new double[2,12] { {1.00, 2.00, 3.00, -1.00, 1.00, 2.00, -3.00, -3.00, 1.00, 3.00, -4.00, 3.00}, {-2.00, 0.00, -2.00, 0.00, 2.00, 5.00, 0.00, -2.00,  1.00, 5.00, 0.00, -2.00} };
        public static double[,] predefinedProfit2 = new double[2, 12] { { 3.00, 0.00, -3.00, 1.00, 1.00, 1.00, 4.00, 3.00, -2.00, 4.00, -3.00, 1.00 }, { 5.00, 0.00, 2.00, 0.00, 2.00, -3.00, 2.00, 1.00, 5.00, 1.00, 5.00, -4.00 } };
        public static double[,] Profit1 = new double[2, 12]; //Profit of the Player1 at the bottom step
        public static double[,] Profit2 = new double[2, 12]; //Profit of the Player2 at the bottom step
        public static double[,] firstProfit1 = new double[2, 3]; //Profit of the Player1 at the top step
        public static double[,] firstProfit2 = new double[2, 3]; //Profit of the Player2 at the top step
        public static bool[] dominance = new bool[6]; //Indicator of the dominant strategy
        public static bool[] usedDecisions = new bool[3]; //Indicator of decisions taken by Player2 at the top step
        public static int stageCounter; //Counter of the solving stages
        public static int count; //Counts number of variations of Player2 decisions at the top step
        public static int[] index = new int[2]; //Contains indexes of Player2 decisions at the top step
        public static double eu1; //Expected Utility for Player1
        public static double eu2; //Expected Utility for Player2

        private void button1_Click(object sender, EventArgs e)
        {
            groupBox1.Enabled = !groupBox1.Enabled;
            button1.Visible = !button1.Visible;
            button2.Text = "Next step";
            button2.Visible = !button2.Visible;

            //Displaying profit grid
            dataGridView1.ColumnCount = 12;
            dataGridView1.RowCount = 2;
            if (radioButton1.Checked)
            {
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 12; j++)
                    {
                        Profit1[i, j] = predefinedProfit1[i, j];
                        Profit2[i, j] = predefinedProfit2[i, j];
                        dataGridView1[j, i].Value = String.Format("{0:0.##}", Profit1[i, j]) + ", " + String.Format("{0:0.##}", Profit2[i, j]); //Displaying the values in the data grid
                    }
                }
            }
            else
            {
                Random rnd = new Random();
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 12; j++)
                    {
                        Profit1[i, j] = Convert.ToDouble((rnd.Next(-9, 9)));
                        Profit2[i, j] = Convert.ToDouble((rnd.Next(-9, 9)));
                        dataGridView1[j, i].Value = String.Format("{0:0.##}", Profit1[i, j]) + ", " + String.Format("{0:0.##}", Profit2[i, j]);
                    }
                }
            }
            dataGridView1.ClearSelection();

            //Initializing output matrix for bottom step
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    firstProfit1[i, j] = 0.00;
                    firstProfit2[i, j] = 0.00;
                }
            }

            //Initializing dominance and used decisions indicators
            for (int i = 0; i < 6; i++)
            {
                dominance[i] = false;
                usedDecisions[i / 2] = false;
            }

            stageCounter = 0;

        }

        //Dominant Strategy (bottom stage)
        void Dominance(int i, int j)
        {
            dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.Green;
            dominance[j / 2] = true;
            if (j < 6)
            {
                firstProfit1[0, j / 2] = Profit1[i, j];
                firstProfit2[0, j / 2] = Profit2[i, j];
            }
            else
            {
                firstProfit1[1, j / 2 - 3] = Profit1[i, j];
                firstProfit2[1, j / 2 - 3] = Profit2[i, j];
            }
        }

        //Expected Utility
        void EU(double LL1, double LR1, double RL1, double RR1, double LL2, double LR2, double RL2, double RR2)
        {
            eu1 = 0;
            eu2 = 0;
            double[] Probability1 = new double[2];
            double[] Probability2 = new double[2];
            Probability1[0] = (RR2 - RL2) / (LL2 - RL2 - LR2 + RR2); //profit values of player2
            Probability2[0] = (RR1 - LR1) / (LL1 - LR1 - RL1 + RR1); //profit values of player1
            Probability1[1] = 1 - Probability1[0];
            Probability2[1] = 1 - Probability2[0];
            eu1 = Probability1[0] * Probability2[0] * LL1 + Probability1[0] * Probability2[1] * LR1 + Probability1[1] * Probability2[0] * RL1 + Probability1[1] * Probability2[1] * RR1;
            eu2 = Probability1[0] * Probability2[0] * LL2 + Probability1[0] * Probability2[1] * LR2 + Probability1[1] * Probability2[0] * RL2 + Probability1[1] * Probability2[1] * RR2;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            switch (stageCounter)
            {
                case 0:

                        //Marking decisions of Player2 (bottom step)
                        for (int i = 0; i < 2; i++)
                        {
                            for (int j = 0; j < 12; j = j + 2)
                            {
                                if (Profit2[i, j] > Profit2[i, j + 1])
                                    dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.Yellow;
                                else
                                    dataGridView1.Rows[i].Cells[j + 1].Style.BackColor = Color.Yellow;

                            }
                        }

                        //Marking decisions of Player1 with regarding already marked decisions of Player2 (bottom step)
                        for (int j = 0; j < 12; j++)
                        {
                            if (Profit1[0, j] > Profit1[1, j])
                            {
                                if (dataGridView1.Rows[0].Cells[j].Style.BackColor != Color.Yellow)
                                    dataGridView1.Rows[0].Cells[j].Style.BackColor = Color.LightBlue;
                                else
                                    dataGridView1.Rows[0].Cells[j].Style.BackColor = Color.LightGreen;
                            }
                            else
                            {
                                if (dataGridView1.Rows[1].Cells[j].Style.BackColor != Color.Yellow)
                                    dataGridView1.Rows[1].Cells[j].Style.BackColor = Color.LightBlue;
                                else
                                    dataGridView1.Rows[1].Cells[j].Style.BackColor = Color.LightGreen;
                            }
                        }
                        dataGridView1.ClearSelection();

                        //Searching for Dominant Strategy (bottom stage)
                        //If there is a cell, marked with LightGreen (decision match),
                        //and there are no other matching cells in a quadrat
                        //this cell is a dominant solution
                        for (int j = 0; j < 12; j++)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                if (j % 2 == 0)
                                {
                                    if (dataGridView1.Rows[i].Cells[j].Style.BackColor == Color.LightGreen && dataGridView1.Rows[0].Cells[j + 1].Style.BackColor != Color.LightGreen && dataGridView1.Rows[1].Cells[j + 1].Style.BackColor != Color.LightGreen)
                                        Dominance(i, j);
                                }
                                else
                                {
                                    if (dataGridView1.Rows[i].Cells[j].Style.BackColor == Color.LightGreen && dataGridView1.Rows[0].Cells[j - 1].Style.BackColor != Color.LightGreen && dataGridView1.Rows[1].Cells[j - 1].Style.BackColor != Color.LightGreen)
                                        Dominance(i, j);
                                }
                            }
                        }

                        //Calculating Expected Utility for cases without Dominant Strategy (bottom stage)
                        for (int i = 0; i < 6; i++)
                        {
                            if (!dominance[i])
                            {
                                int j = i * 2;
                                EU(Profit1[0, j], Profit1[0, j + 1], Profit1[1, j], Profit1[1, j + 1], Profit2[0, j], Profit2[0, j + 1], Profit2[1, j], Profit2[1, j + 1]);
                                if (i < 3)
                                {
                                    firstProfit1[0, i] = eu1;
                                    firstProfit2[0, i] = eu2;
                                }
                                else
                                {
                                    firstProfit1[1, i - 3] = eu1;
                                    firstProfit2[1, i - 3] = eu2;
                                }
                            }
                        }
                        stageCounter++;
                        break;
                case 1:

                        dataGridView1.Visible = !dataGridView1.Visible;
                        dataGridView2.Visible = !dataGridView2.Visible;
                        pictureBox1.Visible = !pictureBox1.Visible;
                        textBox1.Text = String.Format("{0:0.##}", firstProfit1[0, 0]) + ", " + String.Format("{0:0.##}", firstProfit2[0, 0]);
                        textBox2.Text = String.Format("{0:0.##}", firstProfit1[0, 1]) + ", " + String.Format("{0:0.##}", firstProfit2[0, 1]);
                        textBox3.Text = String.Format("{0:0.##}", firstProfit1[0, 2]) + ", " + String.Format("{0:0.##}", firstProfit2[0, 2]);
                        textBox4.Text = String.Format("{0:0.##}", firstProfit1[1, 0]) + ", " + String.Format("{0:0.##}", firstProfit2[1, 0]);
                        textBox5.Text = String.Format("{0:0.##}", firstProfit1[1, 1]) + ", " + String.Format("{0:0.##}", firstProfit2[1, 1]);
                        textBox6.Text = String.Format("{0:0.##}", firstProfit1[1, 2]) + ", " + String.Format("{0:0.##}", firstProfit2[1, 2]);
                        textBox1.Visible = !textBox1.Visible;
                        textBox2.Visible = !textBox2.Visible;
                        textBox3.Visible = !textBox3.Visible;
                        textBox4.Visible = !textBox4.Visible;
                        textBox5.Visible = !textBox5.Visible;
                        textBox6.Visible = !textBox6.Visible;

                //Output of bottom step
                        dataGridView2.ColumnCount = 3;
                        dataGridView2.RowCount = 2;
                        for (int i = 0; i < 2; i++)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                dataGridView2[j, i].Value = String.Format("{0:0.##}", firstProfit1[i, j]) + ", " + String.Format("{0:0.##}", firstProfit2[i, j]);
                            }
                        }
                        dataGridView2.ClearSelection();
                        stageCounter++;
                        break;
                case 2:
                        //Marking decisions of Player2 (top step)
                        for (int i = 0; i < 2; i++)
                        {
                            int k = 0;
                            for (int j = 1; j < 3; j++)
                            {
                                if (firstProfit2[i, j] > firstProfit2[i, k])
                                    k = j;
                            }
                            dataGridView2.Rows[i].Cells[k].Style.BackColor = Color.Yellow;
                            usedDecisions[k] = true;
                        }

                        //Marking decisions of Player1 regarding already marked decisions of Player2 (top step)
                        for (int j = 0; j < 3; j++)
                        {
                            if (firstProfit1[0, j] > firstProfit1[1, j])
                            {
                                if (dataGridView2.Rows[0].Cells[j].Style.BackColor != Color.Yellow)
                                    dataGridView2.Rows[0].Cells[j].Style.BackColor = Color.LightBlue;
                                else
                                    dataGridView2.Rows[0].Cells[j].Style.BackColor = Color.LightGreen;
                            }
                            else
                            {
                                if (dataGridView2.Rows[1].Cells[j].Style.BackColor != Color.Yellow)
                                    dataGridView2.Rows[1].Cells[j].Style.BackColor = Color.LightBlue;
                                else
                                    dataGridView2.Rows[1].Cells[j].Style.BackColor = Color.LightGreen;
                            }
                        }
                        dataGridView2.ClearSelection();

                        //Searching for Dominant Strategy (top stage)
                        count = 0;
                        for (int i = 0; i < 3; i++)
                        {
                            if (usedDecisions[i])
                            {
                                index[count] = i;
                                count++;
                            }
                        }

                        if (count == 1) //Dominant strategy exists
                        {
                            if (dataGridView2.Rows[0].Cells[index[0]].Style.BackColor == Color.LightGreen)
                            {
                                dataGridView2.Rows[0].Cells[index[0]].Style.BackColor = Color.Green;
                                label1.Text = "Player 1: " + String.Format("{0:0.##}", firstProfit1[0, index[0]]) + ", Player 2: " + String.Format("{0:0.##}", firstProfit2[0, index[0]]);
                            }
                            else
                            {
                                dataGridView2.Rows[1].Cells[index[0]].Style.BackColor = Color.Green;
                                label1.Text = "Player 1: " + String.Format("{0:0.##}", firstProfit1[1, index[0]]) + ", Player 2: " + String.Format("{0:0.##}", firstProfit2[1, index[0]]);
                            }
                        }
                        else //No dominant strategy, calculate Expected utility
                        {
                            EU(firstProfit1[0, index[0]], firstProfit1[0, index[1]], firstProfit1[1, index[0]], firstProfit1[1, index[1]], firstProfit2[0, index[0]], firstProfit2[0, index[1]], firstProfit2[1, index[0]], firstProfit2[1, index[1]]);
                            label1.Text = "Player 1: " + String.Format("{0:0.##}", eu1) + ", Player 2: " + String.Format("{0:0.##}", eu2);
                        }
                        label1.Visible = !label1.Visible;
                        label8.Visible = !label8.Visible;
                        stageCounter++;
                        button2.Text = "Count again";
                        break;
                default:
                    groupBox1.Enabled = !groupBox1.Enabled;
                    button1.Visible = !button1.Visible;
                    button2.Visible = !button2.Visible;
                    dataGridView1.Visible = !dataGridView1.Visible;
                    dataGridView2.Visible = !dataGridView2.Visible;
                    pictureBox1.Visible = !pictureBox1.Visible;
                    dataGridView1.Rows.Clear();
                    dataGridView1.Refresh();
                    dataGridView2.Rows.Clear();
                    dataGridView2.Refresh();
                    label1.Visible = !label1.Visible;
                    label8.Visible = !label8.Visible;
                    textBox1.Visible = !textBox1.Visible;
                        textBox2.Visible = !textBox2.Visible;
                        textBox3.Visible = !textBox3.Visible;
                        textBox4.Visible = !textBox4.Visible;
                        textBox5.Visible = !textBox5.Visible;
                        textBox6.Visible = !textBox6.Visible;
                    break;
            }
        }
    }
}
