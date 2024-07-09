using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project_4
{
    public partial class Form_man_modify : Form
    {
        public Form_man_modify()
        {
            InitializeComponent();
        }

        private void Form_man_modify_Load(object sender, EventArgs e)
        {
            button2.Font = new Font("G마켓 산스 TTF Light", 16);
            label1.Font = new Font("G마켓 산스 TTF Light", 15);
            label2.Font = new Font("G마켓 산스 TTF Light", 15);
            label3.Font = new Font("G마켓 산스 TTF Light", 15);
            label3.Font = new Font("G마켓 산스 TTF Light", 15);
            label4.Font = new Font("G마켓 산스 TTF Light", 15);
            label5.Font = new Font("G마켓 산스 TTF Light", 15);
            label6.Font = new Font("G마켓 산스 TTF Light", 15);
            label7.Font = new Font("G마켓 산스 TTF Light", 15);
            label8.Font = new Font("G마켓 산스 TTF Light", 15);
            label9.Font = new Font("G마켓 산스 TTF Light", 15);
            label10.Font = new Font("G마켓 산스 TTF Light", 15);
            textBox1.Font = new Font("G마켓 산스 TTF Light", 15);
            textBox2.Font = new Font("G마켓 산스 TTF Light", 15);
            textBox3.Font = new Font("G마켓 산스 TTF Light", 15);
            textBox4.Font = new Font("G마켓 산스 TTF Light", 15);
            textBox5.Font = new Font("G마켓 산스 TTF Light", 15);
            textBox6.Font = new Font("G마켓 산스 TTF Light", 15);
            textBox7.Font = new Font("G마켓 산스 TTF Light", 15);
            textBox8.Font = new Font("G마켓 산스 TTF Light", 15);
            comboBox1.Font = new Font("G마켓 산스 TTF Light", 15);
            comboBox1.Items.Add("농협");
            comboBox1.Items.Add("국민");
            comboBox1.Items.Add("신한");
            comboBox1.Items.Add("우리");
            comboBox1.Items.Add("카카오");
            button1.BackgroundImage = Properties.Resources.folder;
            button1.BackgroundImageLayout = ImageLayout.Stretch;

            string info = "Info.txt";
            List<string> lines = File.ReadAllLines(info).ToList();
            int name = 0, birth = 0, address = 0, phone = 0, bankname = 0, banknum = 0, id = 0, password = 0, picture = 0;
            for (int i = 0; i < lines.Count; i++)
            {
                List<string> line = lines[i].Split('\t').ToList();
                if (i == 0)
                {
                    name = line.IndexOf("이름");
                    birth = line.IndexOf("주민번호");
                    address = line.IndexOf("주소");
                    phone = line.IndexOf("전화번호");
                    bankname = line.IndexOf("은행");
                    banknum = line.IndexOf("계좌번호");
                    id = line.IndexOf("ID");
                    password = line.IndexOf("PW");
                    picture = line.IndexOf("사진");
                }
                else
                {
                    if (line[name] == Form_login.form_main.textBox4_1_1.Text && line[birth] == Form_login.form_main.textBox4_1_2.Text)
                    {
                        textBox1.Text = Form_login.form_main.textBox4_1_1.Text;
                        textBox2.Text = Form_login.form_main.textBox4_1_2.Text;
                        textBox3.Text = Form_login.form_main.textBox4_1_3.Text;
                        textBox4.Text = Form_login.form_main.textBox4_1_4.Text;
                        textBox5.Text = Form_login.form_main.textBox4_1_6.Text;
                        textBox6.Text = line[banknum];        // 계좌번호
                        textBox7.Text = line[id];        // 아이디
                        textBox8.Text = line[password];        // 비밀번호
                        comboBox1.SelectedItem = line[bankname];
                        label5.Text = line[picture];
                    }
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (open.ShowDialog() == DialogResult.OK)
            {
                string file = open.FileName;
                string name = Path.GetFileName(file);
                label5.Text = name;
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            string info = "Info.txt";
            List<string> lines = File.ReadAllLines(info).ToList();
            int name = 0, birth = 0, address = 0, phone = 0, bankname = 0, banknum = 0, id = 0, password = 0, picture = 0;
            for (int i = 0; i < lines.Count; i++)
            {
                List<string> line = lines[i].Split('\t').ToList();
                if (i == 0)
                {
                    name = line.IndexOf("이름");
                    birth = line.IndexOf("주민번호");
                    address = line.IndexOf("주소");
                    phone = line.IndexOf("전화번호");
                    bankname = line.IndexOf("은행");
                    banknum = line.IndexOf("계좌번호");
                    id = line.IndexOf("ID");
                    password = line.IndexOf("PW");
                    picture = line.IndexOf("사진");
                }
                else
                {
                    if (line[name] == Form_login.form_main.textBox4_1_1.Text && line[birth] == Form_login.form_main.textBox4_1_2.Text)
                    {
                        line[name] = textBox1.Text;
                        line[birth] = textBox2.Text;
                        line[address] = textBox3.Text;
                        line[phone] = textBox4.Text;
                        line[bankname] = comboBox1.SelectedItem.ToString();
                        line[banknum] = textBox6.Text;
                        line[id] = textBox7.Text;
                        line[password] = textBox8.Text;
                        line[picture] = label5.Text;
                        lines[i] = string.Join("\t", line);
                    }
                }
            }
            File.WriteAllLines(info, lines);
            MessageBox.Show("수정되었습니다.");
        }

        private void label1_1_Click(object sender, EventArgs e)
        {
            textBox1.Focus();
        }

        private void label1_2_Click(object sender, EventArgs e)
        {
            textBox2.Focus();
        }

        private void label1_3_Click(object sender, EventArgs e)
        {
            textBox3.Focus();
        }

        private void label1_4_Click(object sender, EventArgs e)
        {
            textBox4.Focus();
        }

        private void label1_5_Click(object sender, EventArgs e)
        {
            textBox5.Focus();
        }

        private void label1_6_Click(object sender, EventArgs e)
        {
            textBox6.Focus();
        }

        private void label1_7_Click(object sender, EventArgs e)
        {
            textBox7.Focus();
        }

        private void label1_8_Click(object sender, EventArgs e)
        {
            textBox8.Focus();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            label1_1.Text = textBox1.Text;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            label1_2.Text = textBox2.Text;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            label1_3.Text = textBox3.Text;
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            label1_4.Text = textBox4.Text;
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            label1_5.Text = textBox5.Text;
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            label1_6.Text = textBox6.Text;
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            label1_7.Text = textBox7.Text;
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            label1_8.Text = textBox8.Text;
        }

        private void button2_MouseMove(object sender, MouseEventArgs e)
        {
            button2.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
        }

        private void button2_MouseLeave(object sender, EventArgs e)
        {
            button2.Font = new Font("G마켓 산스 TTF Light", 16);
        }


    }
}
