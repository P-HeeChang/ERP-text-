using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;
using System.Reflection.Emit;
using System.Globalization;

namespace Project_4
{
    public partial class Form_inv_add : Form
    {
        public Form_inv_add()
        {
            InitializeComponent();
        }

        private void Form_inv_add_Load(object sender, EventArgs e)
        {
            label1.Font = new Font("G마켓 산스 TTF Light", 16);
            label2.Font = new Font("G마켓 산스 TTF Light", 16);
            label3.Font = new Font("G마켓 산스 TTF Light", 16);
            label4.Font = new Font("G마켓 산스 TTF Light", 14);
            label5.Font = new Font("G마켓 산스 TTF Light", 16);
            label6.Font = new Font("G마켓 산스 TTF Light", 16);
            label7.Font = new Font("G마켓 산스 TTF Light", 16);
            button1.Font = new Font("G마켓 산스 TTF Light", 16);
            textBox1.Text = null;
            textBox2.Text = null;
            textBox3.Text = null;
        }
        
        private async void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && textBox2.Text != "" && textBox3.Text != "") 
            {
                Inv_add.Inew();
                textBox1.Text = null;
                textBox2.Text = null;
                textBox3.Text = null;
            }
            else
            {
                label4.Text = "잘못된 값입니다.";
                await Task.Delay(1000);                     // 일정시간후 에러메시지 삭제
                label4.Text = null;
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1_Click(sender, e);
            }

        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1_Click(sender, e);
            }

        }

        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1_Click(sender, e);
            }
        }

        private void button1_MouseMove(object sender, MouseEventArgs e)
        {
            button1.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);

        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button1.Font = new Font("G마켓 산스 TTF Light", 14);

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            label5.Text = textBox1.Text;
        }

        private void label5_Click(object sender, EventArgs e)
        {
            label5.Text = null;
            textBox1.Focus();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            label6.Text = textBox2.Text;
        }

        private void label6_Click(object sender, EventArgs e)
        {
            label6.Text = null;
            textBox2.Focus();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            label7.Text = textBox3.Text;
        }

        private void label7_Click(object sender, EventArgs e)
        {
            label7.Text = null;
            textBox3.Focus();
        }
    }
    class Inv_add
    {
        private static bool Isnumber(string input)
        {
            foreach (char c in input)
            {
                if (char.IsDigit(c))
                {
                    return true;
                }
            }
            return false;
        }
        private static bool Isupper(string input)
        {
            foreach (char c in input)
            {
                if (char.IsUpper(input[0]))     // 제품 코드의 첫 글자가 대문자가 아닐때
                {
                    return true;
                }
            }
            return false;
        }
        private static bool Iskorean(string input)
        {
            foreach (char c in input)
            {
                if (char.GetUnicodeCategory(c) == UnicodeCategory.OtherLetter)     // 제품 명이 한글이 아닐때
                {
                    return true;
                }
            }
            return false;
        }
        public async static void Inew()
        {
            string Icode = Form_main.form_inv_add.textBox1.Text;          // 신규제품 코드
            string Iname = Form_main.form_inv_add.textBox2.Text;          // 신규제품 이름
            string Icount = Form_main.form_inv_add.textBox3.Text;         // 신규제품 재고
            if (Isupper(Icode) && Iskorean(Iname) && Isnumber(Icount))
            {
                string inventory = "Inventory Manager.txt";
                List<string> lines = File.ReadAllLines(inventory).ToList();
                string Inew_info = $"{Icode}\t{Iname}\t{Icount}";
                int count = 0;                          // 중복 값 확인

                if (lines.Count == 0)        // 파일이 비어있다면
                {
                    lines.Add("제품코드\t제품명\t재고");
                    File.WriteAllLines(inventory, lines);
                }
                for (int i = 0; i < lines.Count; i++)                         // 제품 텍스트파일 행만큼 반복
                {
                    List<string> columns = lines[i].Split('\t').ToList();
                    if (Icode == columns[0])                            // 제품 코드가 같다면
                    {
                        count++;
                        break;
                    }
                }
                if (count == 0)             // 중복이 없다면
                {
                    File.AppendAllText(inventory, Inew_info + Environment.NewLine);         // 제품 추가
                    List<string> sort = File.ReadAllLines(inventory).ToList();
                    string firstLine = sort.FirstOrDefault();               // 첫번째행인 카테고리행
                    List<string> linesToSort = sort.Skip(1).ToList();       // 첫번째행을 제외하고 리스트화
                    linesToSort.Sort();
                    List<string> sortedLines = new List<string>();
                    sortedLines.Add(firstLine);                     // 카테고리 행먼저 추가
                    sortedLines.AddRange(linesToSort);              // 정렬할 제품행
                    File.WriteAllLines(inventory, sortedLines);
                }
                else
                {
                    Form_main.form_inv_add.label4.Text = "이미 포함된 제품입니다.";
                }
            }
            else if (!Isupper(Icode))
            {
                Form_main.form_inv_add.label4.Text = "제품코드 형식이 잘못되었습니다.";
            }
            else if (!Iskorean(Iname))
            {
                Form_main.form_inv_add.label4.Text = "제품명 형식이 잘못되었습니다.";
            }
            else if (!Isnumber(Icount))
            {
                Form_main.form_inv_add.label4.Text = "수량 형식이 잘못되었습니다.";
            }
            await Task.Delay(1000);                     // 일정시간후 에러메시지 삭제
            Form_main.form_inv_add.label4.Text = null;
        }
    }
}