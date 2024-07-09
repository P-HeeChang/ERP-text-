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
using System.Reflection.Emit;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;
namespace Project_4
{
    public partial class Form_vacation : Form
    {
        private Timer timer;
        private Form_login form_login;

        string name;
        string birth;
        string Rent;
        public Form_vacation(Form_login form_login, string name, string birth, string Rent)
        {
            InitializeComponent();
            this.form_login = form_login;
            this.name = form_login.UserName;
            this.birth = birth;
            this.Rent = Rent;
            LoadDisplay(); // 월차/반차 신청을 새로고침 
            Vac_count(); //월차/반차 남은 갯수를 새로고침
            Check_vac();
        }

        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {
            if (monthCalendar1.SelectionRange.Start == monthCalendar1.SelectionRange.End) //달력에서 선택된 날을 텍스트박스에 삽입
                textBox1.Text = monthCalendar1.SelectionRange.Start.ToString("yyyy-MM-dd");
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedItem = comboBox1.SelectedItem.ToString(); //콤보박스에서 선택한것

            // 만약 특정 항목을 선택한 경우에만 두 번째 콤보 박스를 보이게 합니다.
            if (selectedItem == "반차")
            {
                comboBox2.Visible = true;
            }
            else
            {
                comboBox2.Visible = false;
                comboBox2.Text = "";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string comboText = comboBox1.SelectedItem.ToString();
            string selectedDate = textBox1.Text;
            string filePath = "vac_req.txt";
            string fileVac = "Info.txt";
            string comboText2 = " ";
            DateTime selecteDate = monthCalendar1.SelectionStart;//달력에서 선택한 날을 삽입
            if (comboText == "월차")
            {
                if (selecteDate > DateTime.Now) //선택한 날짜가 오늘날보다 크면
                {
                    bool vac_check = false; //중복 체크할것
                    List<List<string>> vacList = vacation.Line(); // 월차 신청 목록을 가져옴
                    foreach (List<string> vacInfo in vacList)
                    {
                        try
                        {
                            if (DateTime.Parse(vacInfo[4]) == selecteDate && vacInfo[0] == name)
                            {
                                MessageBox.Show("이미 있는 날짜입니다.");
                                vac_check = true;
                                break;
                            }
                        }
                        catch { }

                    }
                    if (vac_check == false && int.Parse(Rent) >= 2)
                    {
                        string vac_reqText = $"{name}\t{birth}\t{comboText}\t{"."}\t{selectedDate}"; // 파일에 쓸 내용
                        try
                        {
                            File.AppendAllText(filePath, vac_reqText + Environment.NewLine); // 파일에 내용 추가

                            int rentValue = int.Parse(Rent);
                            rentValue--; // rent 값을 2 감소
                            rentValue--; // rent 값을 2 감소
                            Rent = rentValue.ToString(); // 감소된 rent 값을 문자열로 변환하여 저장
                            //Vac_count();// rent 값이 변경되었으므로 다시 화면에 표시
                            List<string> lines = File.ReadAllLines(fileVac).ToList();//fileVac을 읽어옴
                            if (lines.Count > 0) //휴가의 갯수가 0보다 크면
                            {
                                for (int i = 0; i < lines.Count; i++)
                                {
                                    string[] columns = lines[i].Split('\t');
                                    if (columns[0] == name) // 이름이 일치하는 경우 
                                    {
                                        columns[11] = Rent; // monthrent 값을 변경
                                        lines[i] = string.Join("\t", columns); // 변경된 값을 다시 합쳐서 해당 줄로 설정
                                        File.WriteAllLines(fileVac, lines); // 변경된 내용을 파일에 씀
                                        break; // 루프 중단
                                    }
                                }
                            }
                        }
                        catch
                        {

                        }
                        MessageBox.Show("신청 되었습니다.");// 작업 완료 메시지 출력

                        LoadDisplay(); // 새로고침 메서드 호출
                        Vac_count();
                    }


                    if (int.Parse(Rent) < 0)
                    {
                        MessageBox.Show("사용할수있는 갯수가 모자랍니다");
                    }
                }
            }

            if (comboText == "반차") //선택한 날짜가 오늘날보다 크면
            {
                if (selecteDate > DateTime.Now)
                {
                    bool vac_check = false;
                    List<List<string>> vacList = vacation.Line(); // 월차 신청 목록을 가져옴
                    foreach (List<string> vacInfo in vacList)
                    {
                        try
                        {
                            if (DateTime.Parse(vacInfo[4]) == selecteDate && vacInfo[0] == name)
                            {
                                MessageBox.Show("이미 있는 날짜입니다.");
                                vac_check = true;
                                break;
                            }
                        }
                        catch { }
                    }
                    if (vac_check == false && int.Parse(Rent) > 0)
                    {
                        string vac_reqText = $"{name}\t{birth}\t{comboText}\t{comboBox2.Text}\t{selectedDate}"; // 파일에 쓸 내용
                        try
                        {
                            File.AppendAllText(filePath, vac_reqText + Environment.NewLine); // 파일에 내용 추가

                            double rentValue = double.Parse(Rent);
                            rentValue--; // rent 값을 1 감소
                            Rent = rentValue.ToString(); // 감소된 rent 값을 문자열로 변환하여 저장
                            //Vac_count();// rent 값이 변경되었으므로 다시 화면에 표시
                            List<string> lines = File.ReadAllLines(fileVac).ToList();//fileVac을 읽어옴
                            if (lines.Count > 0) //휴가의 갯수가 0보다 크면
                            {
                                for (int i = 0; i < lines.Count; i++)
                                {
                                    string[] columns = lines[i].Split('\t');
                                    if (columns[0] == name) // 이름이 일치하는 경우 
                                    {
                                        columns[11] = Rent; // monthrent 값을 변경
                                        lines[i] = string.Join("\t", columns); // 변경된 값을 다시 합쳐서 해당 줄로 설정
                                        File.WriteAllLines(fileVac, lines); // 변경된 내용을 파일에 씀
                                        break; // 루프 중단
                                    }
                                }
                            }
                        }
                        catch
                        {

                        }
                        MessageBox.Show("신청 되었습니다.");// 작업 완료 메시지 출력

                        LoadDisplay(); // 새로고침 메서드 호출
                        Vac_count();
                    }

                    if (int.Parse(Rent) < 0)
                    {
                        MessageBox.Show("사용할수있는 갯수가 모자랍니다");
                    }

                }
                else
                {
                    MessageBox.Show("날짜가 지났습니다");
                }
            }

        }
        private void Check_vac()
        {
            try
            {
                List<List<string>> vac_req_List = vacation.Line_vac(); // 텍스트 파일의 내용을 읽어옴
                dataGridView3.Rows.Clear(); // DataGridView 초기화
                for (int k = 1; k < vac_req_List.Count; k++) // 읽어온 내용을 DataGridView에 표시
                {
                    List<string> Vac = vac_req_List[k];
                    dataGridView3.Rows.Add(Vac[0], Vac[1], Vac[2], Vac[3], Vac[4]);
                }
            }
            catch { }
        }
        private void LoadDisplay()
        {
            try
            {
                List<List<string>> vac_req_List = vacation.Line(); // 텍스트 파일의 내용을 읽어옴
                dataGridView1.Rows.Clear(); // DataGridView 초기화
                for (int k = 1; k < vac_req_List.Count; k++) // 읽어온 내용을 DataGridView에 표시
                {
                    List<string> Vac = vac_req_List[k];
                    dataGridView1.Rows.Add(Vac[0], Vac[1], Vac[2], Vac[3], Vac[4]);
                }
            }
            catch { }

        }
        public void Vac_count()
        {
            try
            {
                List<List<string>> vac_req_List = vacation.Linz(); // 텍스트 파일의 내용을 읽어옴
                dataGridView2.Rows.Clear(); // DataGridView 초기화
                for (int k = 1; k < vac_req_List.Count; k++) // 읽어온 내용을 DataGridView에 표시
                {
                    List<string> Vac = vac_req_List[k];
                    if (name == Vac[0])
                    {
                        dataGridView2.Rows.Add(Vac[11]);
                    }
                }
            }
            catch { }
            /*dataGridView2.Rows.Clear(); // DataGridView 초기화
            int rent;
            rent = int.Parse(Rent);
            try
            {
                int rowIndex = dataGridView2.Rows.Add(); //행을 추가
                dataGridView2.Rows[rowIndex].Cells[0].Value = rent; //새로운 행에 값을 대체함

            }
            catch { }*/
        }


        private void label58_Click(object sender, EventArgs e)
        {
            textBox1.Focus();

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            label58.Text = textBox1.Text;
        }

        private void button1_MouseMove(object sender, MouseEventArgs e)
        {
            button1.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button1.Font = new Font("G마켓 산스 TTF Light", 16);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
    class vacation
    {
        public static List<string> vac_ok_info()
        {
            string P_path = "Info.txt";      // 환자 정보 텍스트 파일
            string[] lines = File.ReadAllLines(P_path);       // 파일의 모든 줄을 읽어옴
            List<string> vacList = lines.ToList();
            return vacList;
        }
        public static List<List<string>> Line_vac_info()
        {
            List<List<string>> lines = new List<List<string>>();

            // 파일의 각 줄을 읽어와 리스트 형태로 변환
            foreach (string line in vac_ok())
            {
                List<string> data = line.Split('\t').ToList(); // 각 줄을 탭으로 구분하여 리스트로 변환
                lines.Add(data);
            }

            return lines;
        }
        public static List<string> vac_ok()
        {
            string P_path = "Vac_Okay.txt";      // 환자 정보 텍스트 파일
            string[] lines = File.ReadAllLines(P_path);       // 파일의 모든 줄을 읽어옴
            List<string> vacList = lines.ToList();
            return vacList;
        }
        public static List<List<string>> Line_vac()
        {
            List<List<string>> lines = new List<List<string>>();

            // 파일의 각 줄을 읽어와 리스트 형태로 변환
            foreach (string line in vac_ok())
            {
                List<string> data = line.Split('\t').ToList(); // 각 줄을 탭으로 구분하여 리스트로 변환
                lines.Add(data);
            }

            return lines;
        }
        public static List<string> text_vac()
        {
            string P_path = "vac_req.txt";      // 환자 정보 텍스트 파일
            string[] lines = File.ReadAllLines(P_path);       // 파일의 모든 줄을 읽어옴
            List<string> vacList = lines.ToList();
            return vacList;
        }

        public static List<List<string>> Line()
        {
            List<List<string>> lines = new List<List<string>>();

            // 파일의 각 줄을 읽어와 리스트 형태로 변환
            foreach (string line in text_vac())
            {
                List<string> data = line.Split('\t').ToList(); // 각 줄을 탭으로 구분하여 리스트로 변환
                lines.Add(data);
            }

            return lines;
        }
        public static List<List<string>> Linz()
        {
            List<List<string>> lines = new List<List<string>>();

            // 파일의 각 줄을 읽어와 리스트 형태로 변환
            foreach (string line in vac_ok_info())
            {
                List<string> data = line.Split('\t').ToList(); // 각 줄을 탭으로 구분하여 리스트로 변환
                lines.Add(data);
            }

            return lines;
        }
    }
}

