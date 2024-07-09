using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Project_4
{

    public partial class Form_login : Form
    {
        public static Form_main form_main;
        public static Form_pay form_pay;
        public string UserName { get; private set; }
        public string UserBirth { get; private set; }
        public string UserRent { get; private set; }



        public Form_login()
        {
            InitializeComponent();
            
            this.BackgroundImage = Properties.Resources.kk;

            this.FormBorderStyle = FormBorderStyle.FixedSingle; // 화면 사이즈 변경 불가능
        }


        private void Form_login_Load(object sender, EventArgs e)
        {
            label1.BackColor = Color.Transparent; // label1 배경화면 투명
            label2.BackColor = Color.Transparent; // label2 배경화면 투명
            this.BackgroundImageLayout = ImageLayout.Stretch;
            button1.BackgroundImage = Image.FromFile("login.png");
            this.button1.BackgroundImageLayout = ImageLayout.Stretch;
            this.button3.BackgroundImageLayout = ImageLayout.Stretch;           // button사이즈에 맞춰 이미지 추가
            this.button4.BackgroundImageLayout = ImageLayout.Stretch;
            MaximizeBox = false;                                                // 최대화 불가능
            // this.ActiveControl = null;          // 포커스 해제
        }

/*        private void label1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
        }*/

        private void button1_Click(object sender, EventArgs e)
        {
            string path = "Info.txt";  // Info 파일 경로
            //int name, birth, address, phone, bank, start, blood, id, password, admin = 0;     에러
            int name = 0, birth = 0, address = 0, phone = 0, bank = 0, start = 0, blood = 0, id = 0, password = 0, admin = 0,monthrent=0; // 열번호를 저장할 변수
            try
            {
                List<string> lines = File.ReadAllLines(path).ToList();       // 파일의 모든 줄을 읽고 리스트화
                for (int i = 0; i < lines.Count; i++)          // 파일 내용을 한 줄씩 읽어가며 처리, lines.count는 info.txt의 행길이
                {
                    List<string> columns = lines[i].Split('\t').ToList();    // lines[i]를 탭으로 나눠서 리스트에 배치, {"이름","주민번호",...}
                    if (i == 0)
                    {
                        name = columns.IndexOf("이름");                 // "이름" 열번호
                        birth = columns.IndexOf("주민번호");            // "주민번호" 열번호
                        address = columns.IndexOf("주소");              // "주소" 열번호
                        phone = columns.IndexOf("전화번호");            // "전화번호" 열번호
                        bank = columns.IndexOf("계좌번호");             // "계좌번호" 열번호
                        start = columns.IndexOf("입사일");              // "입사일" 열번호
                        blood = columns.IndexOf("혈액형");              // "혈액형" 열번호
                        id = columns.IndexOf("ID");                     // "ID" 열번호
                        password = columns.IndexOf("PW");               // "PW" 열번호
                        admin = columns.IndexOf("admin");               // "admin" 열번호
                        monthrent = columns.IndexOf("monthrent");
                    }
                    else                                // 행 번호가 1 이상인 경우
                    {
                        if (columns[id] == textBox1.Text && columns[password] == textBox2.Text)            // 첫 번째 비교할 열의 값과 textBox1의 값이 같은 지 확인, 두 번째 비교할 열의 값과 textBox2의 값이 같은지 확인.
                        {
                            bool isAdmin = columns[admin] == "O";  // 권한이 O일때 관리자
                            string Name = columns[name];
                            UserName = columns[name];
                            UserBirth = columns[birth];
                            UserRent = columns[monthrent];
                            form_main = new Form_main(this, isAdmin,UserName, UserBirth, UserRent);
                            this.Hide();

                            //form_main.Visible = true;
                            label3.Text = null;
                            label4.Text = null;
                            form_main.ShowDialog();

                            this.Close();  //form_main 창이 없어지면 login창도 없어진다. 
                            return;
                        }
                    
                    }
                    
                }
                textBox1.Text = null;
                textBox2.Text = null;
                textBox1.Focus();
            }
            catch
            {
               
            }
        }


        private void label3_Click(object sender, EventArgs e)
        {
            textBox1.Focus();
            textBox1.MaxLength = 15;
            label3.Text = null;
            textBox1.Text = null;


        }

        private void label4_Click(object sender, EventArgs e)
        {
            textBox2.Focus();
            textBox2.MaxLength = 15;
            label4.Text = null;
            textBox2.Text = null;


        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            label3.Text = textBox1.Text;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            label4.Text = textBox2.Text;
            label4.Text = new string('*', textBox2.Text.Length);
        }

       


        private void button1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1_Click(sender, e);
            }


        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            label4.Text = string.Empty;

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
    }
}