using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Project_4
{
    public partial class Form_pay : Form
    {
        static Random random = new Random();   // 랜덤 수 생성


        public Form_pay()
        {

            InitializeComponent();

            
            string medicalFilePath = "medical.txt";


            if (File.Exists(medicalFilePath))
            {


                DataGridViewCheckBoxColumn checkBoxColumn = new DataGridViewCheckBoxColumn();

                try
                {

                    checkBoxColumn.HeaderText = "여부";


                    dataGridView1.Columns.Add(checkBoxColumn);
                    string[] pay_lines = File.ReadAllLines(medicalFilePath);

                    for (int i = 0; i < pay_lines.Length; i++)
                    {
                        string[] Pays = pay_lines[i].Split('\t');
                        dataGridView1.Rows.Add(Pays[0], Pays[1], Pays[4], false);

                    }

                    dataGridView1.CurrentCellDirtyStateChanged += new EventHandler(delegate (Object o, EventArgs a)
                    {

                        for (int i = 0; i < dataGridView1.Rows.Count; i++)
                        {

                            if (i == dataGridView1.CurrentCell.RowIndex)
                            {
                                if (Convert.ToBoolean(dataGridView1.CurrentCell.Value))
                                {
                                    // 전체 파일을 읽어옵니다.
                                    string[] allLines = File.ReadAllLines(medicalFilePath);

                                    // 현재 선택한 행의 X를 O로 변경합니다.
                                    string[] selectedRow = allLines[dataGridView1.CurrentCell.RowIndex].Split('\t');
                                    selectedRow[5] = "O";

                                    // 변경된 내용을 메모리 상에서 수정한 뒤 다시 파일에 씁니다.
                                    allLines[dataGridView1.CurrentCell.RowIndex] = string.Join("\t", selectedRow);

                                    // 전체 내용을 파일에 다시 씁니다.
                                    File.WriteAllLines(medicalFilePath, allLines);
                                }
                                else
                                {
                                    // 전체 파일을 읽어옵니다.
                                    string[] allLines = File.ReadAllLines(medicalFilePath);

                                    // 현재 선택한 행의 X를 O로 변경합니다.
                                    string[] selectedRow = allLines[dataGridView1.CurrentCell.RowIndex].Split('\t');
                                    selectedRow[5] = "X";

                                    // 변경된 내용을 메모리 상에서 수정한 뒤 다시 파일에 씁니다.
                                    allLines[dataGridView1.CurrentCell.RowIndex] = string.Join("\t", selectedRow);

                                    // 전체 내용을 파일에 다시 씁니다.
                                    File.WriteAllLines(medicalFilePath, allLines);
                                }
                            }


                        }

                    });

                    string[] payline = File.ReadAllLines(medicalFilePath);
                    for (int k = 0; k < payline.Length; k++)
                    {

                        string[] Pays_line = payline[k].Split('\t');
                        if (Pays_line[5] == "O")
                        {

                            dataGridView1.Rows[k].Cells[3].Value = true;
                        }
                        else
                        {
                            dataGridView1.Rows[k].Cells[3].Value = false;
                        }

                    }


                }
                catch (Exception ex)
                {
                    MessageBox.Show("예외가 발생했습니다: " + ex.Message);
                }
            }
            else
            {
                try
                {
                    // 새로운 메모장 파일 생성
                    using (StreamWriter writer = File.CreateText(medicalFilePath))
                    {

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("예외가 발생했습니다: " + ex.Message);
                }

            }
            

        }
    }
}

