using Project_4;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml.Linq;
using static Project_4.Program;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;
// panel1: 초기화면-환자
// panel1_1: 당일 예약 환자
// panel1_2: 환자 신규 등록
// panel1_3: 그래프

// panel2: 초기화면-근태
// panel3: 초기화면-재고
// panel4: 초기화면-관리자
// panel5: 대기자 화면
// panel6: 진료실1
// panel7: 진료실2

namespace Project_4
{


    public partial class Form_main : Form
    {

       

        public static Form_inv_add form_inv_add;
        private Form_login form_login;
        public static Form_man_add form_man_add;
        private Form_man_modify form_man_modify;
        private DateTime clickedDatetime;
        private UserControldays clickedpanel;
        public static outpatient outpatient;
        public static Form_pay form_pay;
        int month, year;
        string name;
        string workcount = "0";
        int totalworkcount = 0;
        int[] weeklyVisitors = new int[5];
        private Timer timer;
        public Form_main(Form_login form_login, bool auth, string Name, string Birth, string Rent)
        {


            InitializeComponent();


            //
            //간호사와 관리자 구분 [메인 사이드 왼쪽]
            this.name = Name;
            this.FormBorderStyle = FormBorderStyle.FixedSingle; // 화면 사이즈 변경 불가능
            MaximizeBox = false; // 최대화 불가능
            button4.Visible = auth;     // 관리자계정일때 보여주기
            if (auth == false)       // 간호사 계정일때
            {
                label1.Text = Name + " 간호사님 환영합니다.";
            }
            else
            {
                label1.Text = Name + " 님 환영합니다.";
            }
            this.form_login = form_login;


            //
            // 당일 예약 환자 [메인 가운데]
            List<List<string>> lines = Patient.Line();
            foreach (var patientData in lines)
            {
                string reservationDate = patientData.Last(); // 예약 날짜를 가져옴
                if (reservationDate == DateTime.Now.ToString("yyyy-MM-dd"))
                {
                    var patientInfo = string.Join("\t", patientData.Take(patientData.Count - 1)); // 마지막 예약 날짜를 제외하고 환자 정보를 가져오기
                   

                    // 데이터그리드뷰에 행을 추가하기 전에 초기화
                    dataGridView1.Rows.Clear();

                    // 예약 날짜가 오늘인 모든 환자에 대해 행 추가
                    foreach (var patient in lines.Where(p => p.Last() == DateTime.Now.ToString("yyyy-MM-dd")))
                    {
                        var patientList = string.Join("\t", patient.Take(patient.Count - 1)).Split('\t');
                        dataGridView1.Rows.Add(patientList[0], patientList[1], patientList[2], patientList[3], patientList[4]);
                    }

                    // 모든 환자를 처리했으므로 더 이상 반복할 필요가 없음
                    break;
                }
            }

            //
            //달력 나타내기
            DisplaDays();
            DisplaDays1();

            //
            // DateTimePicker에서 시간이 선택되었을 때 이벤트 핸들러 등록 -> 예약 시간
            dateTimePicker1.ValueChanged += DateTimePicker_ValueChanged;

        }

        //
        //달력 -> 근태관리 달력
        private void DisplaDays()
        {
            DateTime now = DateTime.Now;
            month = now.Month;
            year = now.Year;
            string monthname = DateTimeFormatInfo.CurrentInfo.GetMonthName(month);// 현재 월에 해당하는 달의 이름을 가져와서 라벨에 표시
            label13.Text = year + "년 " + monthname;

            DateTime startofthemonth = new DateTime(year, month, 1);// 현재 연도와 월에 해당하는 달의 시작 요일과 날짜 수를 계산
            int days = DateTime.DaysInMonth(year, month);
            int dayoftheweek = Convert.ToInt32(startofthemonth.DayOfWeek.ToString("d")) + 3;
            for (int i = 0; i < dayoftheweek; i++)  // 달력의 시작 부분에 공백을 추가
            {
                UserControl ucblank = new UserControlBlank();
                Daycontainer.Controls.Add(ucblank);
            }
            for (int i = 1; i <= days; i++)// 해당 월의 각 날짜를 패널에 추가
            {
                UserControldays ucdays = new UserControldays();
                ucdays.days(i);
                Daycontainer.Controls.Add(ucdays);
                ucdays.OnPanelClick += UserControlDays_OnPanelClick;// 클릭 이벤트 핸들러를 등록
            }

        }
        //
        // 달력 -> 예약관리 달력
        private void DisplaDays1()
        {
            DateTime now = DateTime.Now;
            month = now.Month;
            year = now.Year;
            string monthname = DateTimeFormatInfo.CurrentInfo.GetMonthName(month);// 현재 월에 해당하는 달의 이름을 가져와서 라벨에 표시
            label36.Text = year + "년 " + monthname;

            DateTime startofthemonth = new DateTime(year, month, 1);// 현재 연도와 월에 해당하는 달의 시작 요일과 날짜 수를 계산
            int days = DateTime.DaysInMonth(year, month);
            int dayoftheweek = Convert.ToInt32(startofthemonth.DayOfWeek.ToString("d")) + 3;
            for (int i = 0; i < dayoftheweek; i++)  // 달력의 시작 부분에 공백을 추가
            {
                UserControl ucblank = new UserControlBlank();
                Daycontainer1.Controls.Add(ucblank);
            }
            for (int i = 1; i <= days; i++)// 해당 월의 각 날짜를 패널에 추가
            {
                UserControldays ucdays = new UserControldays();
                ucdays.days(i);
                Daycontainer1.Controls.Add(ucdays);
                ucdays.OnPanelClick += UserControlDays_OnPanelClick;// 클릭 이벤트 핸들러를 등록
            }

        }

        //
        //
        public void UserControlDays_OnPanelClick(object sender, EventArgs e)
        {
            UserControldays clickedPanel = sender as UserControldays;//클릭한 패널 가져오기
            //string clickedDate = clickedPanel.DayLabelText; //클릭한 패널에서 날자 정보 추출
            DateTime today = DateTime.Now;

            DateTime clickedDateTime;

            if (clickedPanel != null)
            {
                string clickedDate = clickedPanel.DayLabelText; // 클릭한 패널에서 날짜 정보 추출
                clickedDateTime = new DateTime(year, month, int.Parse(clickedDate)); // 클릭된 패널의 날짜 설정
            }
            else
            {
                clickedDateTime = today; // 클릭한 패널이 없는 경우에는 오늘 날짜를 선택
            }
            clickedDatetime = clickedDateTime;
            List<List<string>> linez = Patient.Att();
            dataGridView6.Rows.Clear();
            dataGridView3.Rows.Clear();
            foreach (var patientData in linez)
            {
                string scheduleDate = patientData.Last();
                if (scheduleDate == clickedDateTime.ToString("yyyy-MM-dd"))
                {

                    dataGridView6.Rows.Add(patientData.ToArray()); // 클릭한 날짜와 스케줄 날짜가 일치하는 경우 DataGridView에 행 추가

                }

            }
            string ReserDate = "Reservation.txt";
            string[] Reser_line = File.ReadAllLines(ReserDate);
            for (int i = 0; i < Reser_line.Length; i++)
            {
                string[] Reser = Reser_line[i].Split('\t');
                if (Reser[5] == clickedDateTime.ToString("yyyy-MM-dd"))
                {
                    dataGridView3.Rows.Add(Reser[1], Reser[2], Reser[3], Reser[4], Reser[5]);
                }
            }


            //이윤서
            //
            // 달력 클릭시 예약 날짜가 자동으로 들어간다.
            dataGridView4.Rows.Clear();

            string medicalFilePath = "Reservation.txt";
            string[] medical_lines = File.ReadAllLines(medicalFilePath);

            foreach (string line in medical_lines)
            {
                string[] Pays = line.ToString().Split('\t');
                if (Pays[5] == clickedDateTime.ToString("yyyy-MM-dd"))
                {
                    dataGridView4.Rows.Add(Pays[0], Pays[1], Pays[2], Pays[3], Pays[4]);
                }
                textBox2.Text = clickedDateTime.ToString("yyyy-MM-dd");

            }

            //
            //
            dataGridView2.Rows.Clear(); // 그리드뷰의 모든 행 제거
            // DataGridView2 설정
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

        }

        //
        //
        public void SetDayLabel(string day)
        {

            label13.Text = $"{DateTime.Now.Year}년 {DateTime.Now.Month}월 {day}일";// UserControlDays로부터 전달된 날짜 값을 받아서 라벨에 설정
        }

        private void Form_main_Load(object sender, EventArgs e)
        {
            //진료실번호 기본값설정
            label54.Font = new System.Drawing.Font(label54.Font, label54.Font.Style | System.Drawing.FontStyle.Underline);
            label53.Font = new System.Drawing.Font(label53.Font, label53.Font.Style & ~System.Drawing.FontStyle.Underline);
            //재고 관리
            button3_1_3.Font = new Font("G마켓 산스 TTF Light", 16);
            button3_2_1.Font = new Font("G마켓 산스 TTF Light", 16);
            button3_2_2.Font = new Font("G마켓 산스 TTF Light", 16);
            button3_3_3.Font = new Font("G마켓 산스 TTF Light", 16);
            button3_3_1.BackgroundImage = Properties.Resources.minus3;
            button3_3_2.BackgroundImage = Properties.Resources.plus;
            button3_3_1.BackgroundImageLayout = ImageLayout.Stretch;
            button3_3_2.BackgroundImageLayout = ImageLayout.Stretch;
            dataGridView3_2_1.ColumnHeadersDefaultCellStyle.Font = new Font("G마켓 산스 TTF Light", 15);
            dataGridView3_3_1.ColumnHeadersDefaultCellStyle.Font = new Font("G마켓 산스 TTF Light", 15);
            dataGridView3_2_1.DefaultCellStyle.Font = new Font("G마켓 산스 TTF Light", 14);
            dataGridView3_3_1.DefaultCellStyle.Font = new Font("G마켓 산스 TTF Light", 14);
            dataGridView3_2_1.RowTemplate.Height = 25;
            dataGridView3_3_1.RowTemplate.Height = 25;
            label3_1_2.Font = new Font("G마켓 산스 TTF Light", 16);
            label3_1_3.Font = new Font("G마켓 산스 TTF Light", 16);
            label3_1_4.Font = new Font("G마켓 산스 TTF Light", 16);
            label3_1_5.Font = new Font("G마켓 산스 TTF Light", 16);
            label3_2_2.Font = new Font("G마켓 산스 TTF Light", 16);
            label3_3_2.Font = new Font("G마켓 산스 TTF Light", 16);
            // 관리자 폰트
            tabPage3.Font = new Font("G마켓 산스 TTF Light", 12);       // 적용안됨
            tabPage4.Font = new Font("G마켓 산스 TTF Light", 12);       // 적용안됨
            tabPage5.Font = new Font("G마켓 산스 TTF Light", 12);       // 적용안됨
            tabPage6.Font = new Font("G마켓 산스 TTF Light", 12);       // 적용안됨
            tabPage7.Font = new Font("G마켓 산스 TTF Light", 12);       // 적용안됨
            button4_1.Font = new Font("G마켓 산스 TTF Light", 16);
            button4_1_1.Font = new Font("G마켓 산스 TTF Light", 16);
            button4_3.Font = new Font("G마켓 산스 TTF Light", 16);
            button4_5.Font = new Font("G마켓 산스 TTF Light", 16);
            label4_1.Font = new Font("G마켓 산스 TTF Light", 16);
            label4_2.Font = new Font("G마켓 산스 TTF Light", 16);
            label4_3.Font = new Font("G마켓 산스 TTF Light", 16);
            label4_4.Font = new Font("G마켓 산스 TTF Light", 16);
            label4_5.Font = new Font("G마켓 산스 TTF Light", 16);
            label4_6.Font = new Font("G마켓 산스 TTF Light", 16);
            label4_1_1.Font = new Font("G마켓 산스 TTF Light", 14);
            label4_1_2.Font = new Font("G마켓 산스 TTF Light", 14);
            label4_1_3.Font = new Font("G마켓 산스 TTF Light", 14);
            label4_1_4.Font = new Font("G마켓 산스 TTF Light", 14);
            label4_1_5.Font = new Font("G마켓 산스 TTF Light", 14);
            label4_1_6.Font = new Font("G마켓 산스 TTF Light", 14);
            label29.Font = new Font("G마켓 산스 TTF Light", 16);
            label23.Font = new Font("G마켓 산스 TTF Light", 16);
            dataGridView4_1.ColumnHeadersDefaultCellStyle.Font = new Font("G마켓 산스 TTF Light", 15);
            dataGridView4_1.DefaultCellStyle.Font = new Font("G마켓 산스 TTF Light", 14);
            dataGridView4_2.ColumnHeadersDefaultCellStyle.Font = new Font("G마켓 산스 TTF Light", 15);
            dataGridView4_2.DefaultCellStyle.Font = new Font("G마켓 산스 TTF Light", 14);
            dataGridView4_6.ColumnHeadersDefaultCellStyle.Font = new Font("G마켓 산스 TTF Light", 15);
            dataGridView4_6.DefaultCellStyle.Font = new Font("G마켓 산스 TTF Light", 14);
            dataGridView4_7.ColumnHeadersDefaultCellStyle.Font = new Font("G마켓 산스 TTF Light", 15);
            dataGridView4_7.DefaultCellStyle.Font = new Font("G마켓 산스 TTF Light", 14);
            dataGridView4_1.RowTemplate.Height = 25;
            dataGridView4_2.RowTemplate.Height = 25;
            dataGridView4_6.RowTemplate.Height = 25;
            dataGridView4_7.RowTemplate.Height = 25;
            // 관리자 재직 증명서
            label48.Text = DateTime.Now.ToString("yyyy" + "년 " + "MM" + "월 " + "dd" + "일");
            label22.Font = new Font("G마켓 산스 TTF Light", 16);
            label24.Font = new Font("G마켓 산스 TTF Light", 16);
            label25.Font = new Font("G마켓 산스 TTF Light", 16);
            label26.Font = new Font("G마켓 산스 TTF Light", 16);
            label27.Font = new Font("G마켓 산스 TTF Light", 16);
            label28.Font = new Font("G마켓 산스 TTF Light", 16);
            label30.Font = new Font("G마켓 산스 TTF Light", 16);
            label71.Font = new Font("G마켓 산스 TTF Bold", 24);
            label31.Font = new Font("G마켓 산스 TTF Light", 16);
            label32.Font = new Font("G마켓 산스 TTF Light", 16);
            label33.Font = new Font("G마켓 산스 TTF Light", 16);
            label48.Font = new Font("G마켓 산스 TTF Light", 16);
            label50.Font = new Font("G마켓 산스 TTF Light", 16);
            label51.Font = new Font("G마켓 산스 TTF Light", 16);
            button4_2.Font = new Font("G마켓 산스 TTF Light", 16);
            //예약 패널
            panel57.Visible = false;

            //
            //데이터그리드 첫 번째 줄 안 눌려있게 -> 당일 예약 환자[가운데 중앙]
            dataGridView1.ClearSelection();
            dataGridView1.DefaultCellStyle.Font = new Font("G마켓 산스 TTF Light", 14); // 원하는 폰트 및 크기로 변경

            //데이터 그리드 폰트 변경
            dataGridView6.DefaultCellStyle.Font = new Font("G마켓 산스 TTF Light", 14); // 원하는 폰트 및 크기로 변경
            dataGridView3.DefaultCellStyle.Font = new Font("G마켓 산스 TTF Light", 14); // 원하는 폰트 및 크기로 변경
            dataGridView4_1.DefaultCellStyle.Font = new Font("G마켓 산스 TTF Light", 14); // 원하는 폰트 및 크기로 변경




            //
            // 로드 후 첫 환자버튼은 언더라인 처리
            Font ft2 = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
            button1.Font = ft2;

            //
            //타이머
            timer1.Interval = 100; // 타이머 간격 100ms
            timer1.Start();  // 타이머 시작  

            //
            //이윤서
            //차트 
            chart1.Series.Clear();
            chart1.Series.Add("이번주 방문객 수");

            //차트 추가
            chart1.Series.Add("연령별");
            chart1.Series[1].ChartArea = "ChartArea2";

            //Grid 없애기
            chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = false;

            chart1.ChartAreas[1].AxisX.MajorGrid.Enabled = false;
            chart1.ChartAreas[1].AxisY.MajorGrid.Enabled = false;


            // y축 설정
            this.chart1.ChartAreas[0].AxisY.Minimum = 0;
            this.chart1.ChartAreas[0].AxisY.Maximum = 30;

            this.chart1.ChartAreas[1].AxisY.Minimum = 0;
            this.chart1.ChartAreas[1].AxisY.Maximum = 30;

            //
            //차트 폰트

            // 축 레이블 폰트 변경
            chart1.ChartAreas[0].AxisX.LabelStyle.Font = new Font("G마켓 산스 TTF Light", 10);
            chart1.ChartAreas[0].AxisY.LabelStyle.Font = new Font("G마켓 산스 TTF Light", 10);
            chart1.ChartAreas[1].AxisX.LabelStyle.Font = new Font("G마켓 산스 TTF Light", 10);
            chart1.ChartAreas[1].AxisY.LabelStyle.Font = new Font("G마켓 산스 TTF Light", 10);


            // 범례 폰트 변경
            chart1.Legends[0].Font = new Font("G마켓 산스 TTF Light", 12);




            // 이번 주 월요일부터 금요일까지의 요일 설정
            DateTime thisMonday = GetThisWeekMonday();

            // 차트 초기화
            Form_login.form_main.chart1.Series[0].Points.Clear();

            chart1.ChartAreas[0].AxisX.Interval = 1; // 각 요일 간격
            chart1.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Auto; // 요일 간격 설정
            // x축 설정 ChartArea2
            chart1.ChartAreas[1].AxisX.Interval = 1; // 각 연령 간격
            chart1.ChartAreas[1].AxisX.IntervalType = DateTimeIntervalType.Auto; // 연령 간격 설정

            //MessageBox.Show(thisMonday.ToString());
            UpdateChart();
            chart();
            GetThisWeekTotalMedicalLinesCount();
        }

        private void DateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            // 시간이 선택되었을 때 수행할 작업을 여기에 추가
            DateTimePicker dateTimePicker = (DateTimePicker)sender;
            textBox11.Text = dateTimePicker.Value.ToString("HH:mm");

        }


        public void UpdateChart()//240310----각요일별 누적수 보고싶으면 chart1.Series[0].Points[0~4].Label= weeklyVisitors[0~4].ToString();
        {
            // 이번 주 월요일부터 금요일까지의 요일 설정
            DateTime thisMonday = GetThisWeekMonday();
            for (int i = 0; i < 5; i++)
            {
                DateTime currentDay = thisMonday.AddDays(i);

                // 해당 날짜의 누적 방문자 수를 가져옴
                int medicalLinesCount = GetMedicalLinesCountForDay(currentDay);

                // 해당 요일의 누적 방문자 수 업데이트
                weeklyVisitors[i] = medicalLinesCount;

                // 차트에 x축에 월요일부터 금요일까지 고정하면서 해당 요일의 누적 방문자 수를 표시
                chart1.Series[0].Points.AddXY(currentDay.ToString("ddd"), medicalLinesCount);
            }
            for (int i = 0; i < 5; i++)
            {
                if (weeklyVisitors[i].ToString() != "0")
                {
                    chart1.Series[0].Points[i].Label = weeklyVisitors[i].ToString();

                }
            }

        }
        public void chart()
        {
            var chart = Form_login.form_main.chart1;
            int[] cumulativeCounts = new int[6]; // 누적 수를 저장할 배열 생성

            // 각 연령 그룹에 대한 누적 수 계산 및 데이터 포인트 추가
            cumulativeCounts[0] = GetAgeGroupCount10();
            chart.Series[1].Points.AddXY("~19", cumulativeCounts[0]);

            cumulativeCounts[1] = GetAgeGroupCount20();
            chart.Series[1].Points.AddXY("~29", cumulativeCounts[1]);

            cumulativeCounts[2] = GetAgeGroupCount30();
            chart.Series[1].Points.AddXY("~39", cumulativeCounts[2]);

            cumulativeCounts[3] = GetAgeGroupCount40();
            chart.Series[1].Points.AddXY("~49", cumulativeCounts[3]);

            cumulativeCounts[4] = GetAgeGroupCount50();
            chart.Series[1].Points.AddXY("~59", cumulativeCounts[4]);

            cumulativeCounts[5] = GetAgeGroupCount60();
            chart.Series[1].Points.AddXY("60~", cumulativeCounts[5]);

            // 각 막대 위에 누적 수 라벨 표시
            for (int i = 0; i < chart.Series[1].Points.Count; i++)
            {
                if (cumulativeCounts[i].ToString() != "0")
                {
                    chart.Series[1].Points[i].Label = cumulativeCounts[i].ToString(); // 누적 수를 라벨로 설정

                }
            }
        }


        // 해당 날짜의 medical_lines 수를 가져오는 함수 정의
        public int GetMedicalLinesCountForDay(DateTime day)//240310--하루누적
        {
            string filePath = "medical.txt";
            string[] medical_lines = File.ReadAllLines(filePath);
            int count = 0;
            foreach (string line in medical_lines)
            {
                string[] medical = line.Split('\t');
                // 파일 데이터의 날짜와 비교하여 오늘 날짜와 일치하면 해당하는 누적 방문자 수를 반환
                if (medical[3] == day.ToString("yyyy-MM-dd"))
                {
                    count++;
                }
            }

            return count;
        }
        public int GetThisWeekTotalMedicalLinesCount()//240310----이번주 누적
        {
            DateTime thisMonday = GetThisWeekMonday();
            DateTime nextMonday = thisMonday.AddDays(7);

            string filePath = "medical.txt";
            string[] medical_lines = File.ReadAllLines(filePath);
            int count = 0;
            foreach (string line in medical_lines)
            {
                string[] medical = line.Split('\t');
                // 파일 데이터의 날짜와 비교하여 이번 주에 해당하는 날짜의 방문자 수를 누적
                DateTime medicalDate = DateTime.ParseExact(medical[3], "yyyy-MM-dd", CultureInfo.InvariantCulture);  // 문화권 독립적인 개체, DateTime.ParseExact은 문자열 부분으로 반환
                if (medicalDate >= thisMonday && medicalDate < nextMonday)
                {
                    count++;
                }
            }
            label35.Text = count.ToString();
            return count;
        }

            //연령별 인원 수 
        public static int GetAgeGroupCount10()
        {

            string filePath = "medical.txt"; // 실제 파일 경로로 변경

            string[] medical_lines = File.ReadAllLines(filePath);

            int count = 0;
                for (int i = 0; i < medical_lines.Length; i++)
                {
                    string[] medical = medical_lines[i].Split('\t');
                
                    if (Convert.ToInt32(medical[2]) < 20)
                    {
                        count++;
                    }
                }
            return count;
        }
        public static int GetAgeGroupCount20()
        {

            string filePath = "medical.txt"; // 실제 파일 경로로 변경

            string[] medical_lines = File.ReadAllLines(filePath);

            int count = 0;
            for (int i = 0; i < medical_lines.Length; i++)
            {
                string[] medical = medical_lines[i].Split('\t');

                if (20 <= Convert.ToInt32(medical[2]) && Convert.ToInt32(medical[2]) < 30)
                {
                    count++;
                }
            }
            return count;
        }
        public static int GetAgeGroupCount30()
        {

            string filePath = "medical.txt"; // 실제 파일 경로로 변경

            string[] medical_lines = File.ReadAllLines(filePath);

            int count = 0;
            for (int i = 0; i < medical_lines.Length; i++)
            {
                string[] medical = medical_lines[i].Split('\t');

                if (30 <= Convert.ToInt32(medical[2]) && Convert.ToInt32(medical[2]) < 40)
                {
                    count++;
                }
            }
            return count;
        }
        public static int GetAgeGroupCount40()
        {

            string filePath = "medical.txt"; // 실제 파일 경로로 변경

            string[] medical_lines = File.ReadAllLines(filePath);

            int count = 0;
            for (int i = 0; i < medical_lines.Length; i++)
            {
                string[] medical = medical_lines[i].Split('\t');

                if (40 <= Convert.ToInt32(medical[2]) && Convert.ToInt32(medical[2]) < 50)
                {
                    count++;
                }
            }
            return count;
        }
        public static int GetAgeGroupCount50()
        {

            string filePath = "medical.txt"; // 실제 파일 경로로 변경

            string[] medical_lines = File.ReadAllLines(filePath);

            int count = 0;
            for (int i = 0; i < medical_lines.Length; i++)
            {
                string[] medical = medical_lines[i].Split('\t');

                if (50 <= Convert.ToInt32(medical[2]) && Convert.ToInt32(medical[2]) < 60)
                {
                    count++;
                }
            }
            return count;
        }
        public static int GetAgeGroupCount60()
        {

            string filePath = "medical.txt"; // 실제 파일 경로로 변경

            string[] medical_lines = File.ReadAllLines(filePath);

            int count = 0;
            for (int i = 0; i < medical_lines.Length; i++)
            {
                string[] medical = medical_lines[i].Split('\t');

                if (60 <= Convert.ToInt32(medical[2]))
                {
                    count++;
                }
            }
            return count;
        }

        // 현재 주의 월요일을 가져오는 메서드
        private DateTime GetThisWeekMonday()
        {
            DateTime today = DateTime.Today;
            int diff = today.DayOfWeek - DayOfWeek.Monday;
            return today.AddDays(-diff);
        }



        private void button1_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;
            panel1_1.Visible = true;
            panel1_2.Visible = true;
            panel2.Visible = false;
            panel3.Visible = false;
            panel4.Visible = false;
            panel8.Visible = false;
            panel57.Visible = false;

            Font ft1 = new Font("G마켓 산스 TTF Light", 10);

            Font ft2 = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
            button1.Font = ft2;
            button2.Font = ft1;
            button3.Font = ft1;
            button4.Font = ft1;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            panel1_1.Visible = false;
            panel1_2.Visible = false;
            panel2.Visible = true;
            panel3.Visible = false;
            panel4.Visible = false;
            panel8.Visible = false;
            panel57.Visible = false;
            Font ft1 = new Font("G마켓 산스 TTF Light", 10);

            Font ft2 = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
            button2.Font = ft2;
            button1.Font = ft1;
            button3.Font = ft1;
            button4.Font = ft1;

            LoadDisplay();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            
            panel1.Visible = false;
            panel1_1.Visible = false;
            panel1_2.Visible = false;
            panel2.Visible = false;
            panel3.Visible = true;
            panel4.Visible = false;
            panel8.Visible = false;
            panel57.Visible = false;
            Form_login.form_main.dataGridView3_2_1.Rows.Clear();
            textBox3_1_1.Focus();
            Font ft1 = new Font("G마켓 산스 TTF Light", 10);

            Font ft2 = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
            button3.Font = ft2;
            button2.Font = ft1;
            button1.Font = ft1;
            button4.Font = ft1;
            Inventory.line_inv();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            panel1_1.Visible = false;
            panel1_2.Visible = false;
            panel2.Visible = false;
            panel3.Visible = false;
            panel4.Visible = true;
            panel8.Visible = false;
            panel57.Visible = false;
            Font ft1 = new Font("G마켓 산스 TTF Light", 10);

            Font ft2 = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
            button4.Font = ft2;
            button2.Font = ft1;
            button3.Font = ft1;
            button1.Font = ft1;
            //함수
            dataGridView4_5.Rows.Clear();
            dataGridView4_6.Rows.Clear();
            dataGridView4_7.Rows.Clear();
            Manage.Management();        // 간호사 이름과 직급
            Manage.m_app_inv();           // 결재올린 제품들
            Manage.vac_check();
            Manage.vac_check_mang();
            dataGridView4_6.ClearSelection();
            dataGridView4_7.ClearSelection();
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            label2.Text = DateTime.Now.ToString("F"); // label1에 현재날짜시간 표시, F:자세한 전체 날짜/시간
        }

       

        private void main_textBox_Click(object sender, EventArgs e)
        {
            panel8.Visible = true;
        }


  

        private void main_textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Patient.Psearch();
            }
        }

        private void button8_Click_2(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count > 0)
            {
                // 선택된 행의 데이터를 가져오기
                DataGridViewRow selectedRow = dataGridView2.SelectedRows[0];

                // 선택된 행의 데이터를 가져와서 문자열 배열로 변환
                string[] rowData = new string[selectedRow.Cells.Count];
                for (int i = 0; i < selectedRow.Cells.Count; i++)
                {
                    rowData[i] = selectedRow.Cells[i].Value.ToString();
                }

                // 폼2(outpatient)의 인스턴스 생성 및 데이터 전달
                outpatient formOutpatient = new outpatient(rowData);

                formOutpatient.Show();
            }
            else
            {
                MessageBox.Show("접수환자의 정보가 없습니다.");
            }
        }



        //주민번호 19991111이런식으로 입력하면 1999-11-11로 변환해서 저장되게 수정할것
        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            //숫자와 백스페이스와 '-'만 입력 형식에 맞지않게써도 뒤에 사용할 데이트타임변환에서 막혀서 메모장으로 안들어감
            if (!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back) || e.KeyChar == '-'))
            {
                e.Handled = true;
                label10.Visible = true;
            }
            else
            {
                label10.Visible = false;
            }
        }

        //한글입력시에도 경고문 뜨게 수정
        private void textBox6_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back) || e.KeyChar == '-'))
            {
                e.Handled = true;
                label10.Visible = true;
            }
            else
            {
                label10.Visible = false;
            }
        }

        private void textBox4_Click(object sender, EventArgs e)
        {
            label10.Visible = false;

        }

        private void textBox5_Click(object sender, EventArgs e)
        {
            label10.Visible = false;
        }

        private void textBox6_Click(object sender, EventArgs e)
        {
            label10.Visible = false;
        }

        private void textBox7_Click(object sender, EventArgs e)
        {
            label10.Visible = false;
        }

      

        private void textBox4_Enter(object sender, EventArgs e)
        {
            label3.Text = string.Empty;
        }

        private void textBox5_Enter(object sender, EventArgs e)
        {
            label4.Text = string.Empty;
        }

        private void textBox6_Enter(object sender, EventArgs e)
        {
            label11.Text = string.Empty;
        }

        private void textBox7_Enter(object sender, EventArgs e)
        {
            label12.Text = string.Empty;
        }

        private void textBox4_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button6_Click(sender, e);
            }

        }

        private void textBox5_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button6_Click(sender, e);
            }
        }

        private void textBox6_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button6_Click(sender, e);
            }
        }

        private void textBox7_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button6_Click(sender, e);
            }
        }

    


        private void button13_Click(object sender, EventArgs e)  // 수납버튼 클릭시 
        {
            Form_pay form_Pay = new Form_pay();
            form_Pay.Show();
        }



        private void button17_Click_1(object sender, EventArgs e)
        {
            Daycontainer.Controls.Clear();
            month++;
            if (month > 12)
            {
                month = 1;
                year++;
            }
            DateTime startofthemonth = new DateTime(year, month, 1);// 다음달 첫날의 정보를 가져옴
            int days = DateTime.DaysInMonth(year, month); //다음달 일 수 계산
            int dayoftheweek = Convert.ToInt32(startofthemonth.DayOfWeek.ToString("d")); // 다음 달의 시작 요일을 계산
            for (int i = 0; i < dayoftheweek; i++)  // 다음 달의 시작 요일까지 공백 패널을 추가
            {
                UserControl ucblank = new UserControlBlank(); //공백 패널 생성
                Daycontainer.Controls.Add(ucblank); //공백 패널 추가
            }
            for (int i = 1; i <= days; i++)// 다음 달의 각 날짜에 해당하는 패널을 추가
            {
                UserControldays ucdays = new UserControldays(); //날짜 패널 생성
                ucdays.days(i); //패널에 날짜 설정
                Daycontainer.Controls.Add(ucdays); //날짜 패널 추가
            }
            label13.Text = $"{year}년 {month}월"; // 라벨에 다음 달의 연도와 월을 표시
            foreach (Control control in Daycontainer.Controls) // 패널에 있는 UserControlDays의 클릭 이벤트 핸들러를 등록
            {
                if (control is UserControldays)
                {
                    (control as UserControldays).OnPanelClick += UserControlDays_OnPanelClick; // 해당 패널의 클릭 이벤트 핸들러 등록
                }
            }
        }

        private void button18_Click_1(object sender, EventArgs e)
        {
            Daycontainer.Controls.Clear();
            month--;
            if (month < 1)
            {
                month = 12;
                year--;
            }
            DateTime startofthemonth = new DateTime(year, month, 1); //지난달 첫날의 정보를 가져옴
            int days = DateTime.DaysInMonth(year, month);//지난달 일 수 계산
            int dayoftheweek = Convert.ToInt32(startofthemonth.DayOfWeek.ToString("d"));// 지난 달의 시작 요일을 계산
            for (int i = 0; i < dayoftheweek; i++)// 지난 달의 시작 요일까지 공백 패널을 추가
            {
                UserControl ucblank = new UserControlBlank();//공백 패널 생성
                Daycontainer.Controls.Add(ucblank); //공백 패널 추가
            }
            for (int i = 1; i <= days; i++)// 지난 달의 각 날짜에 해당하는 패널을 추가
            {
                UserControldays ucdays = new UserControldays();//날짜 패널 생성
                ucdays.days(i);//날짜 패널 설정
                Daycontainer.Controls.Add(ucdays);//날짜 패널 추가
            }
            label13.Text = $"{year}년 {month}월";// 라벨에 지난 달의 연도와 월을 표시
            foreach (Control control in Daycontainer.Controls) // 패널에 있는 UserControlDays의 클릭 이벤트 핸들러를 등록
            {
                if (control is UserControldays)
                {
                    (control as UserControldays).OnPanelClick += UserControlDays_OnPanelClick; // 해당 패널의 클릭 이벤트 핸들러 등록
                }
            }
        }

        private void button7_Click(object sender, EventArgs e) //출근버튼
        {
            string check_schedule = "schedule.txt";
            DateTime selectedDate = clickedDatetime; // 달력 날짜
            DateTime today = DateTime.Now; // 오늘의 날짜를 가져옴

            List<string> linz = File.ReadAllLines(check_schedule).ToList();
            bool check = true;

            // 모든 라인을 반복하면서 조건을 확인합니다.
            foreach (string line in linz)
            {
                string[] columns = line.Split('\t');

                // 이미 존재하는 데이터인 경우
                if (columns[0] == this.name && columns[8] == today.ToString("yyyy-MM-dd"))
                {
                    check = false; // 새로운 데이터가 필요하지 않음
                    break; // 반복문 종료
                }
            }
            // 새로운 데이터가 필요한 경우에만 추가합니다.
            if (check)
            {
                string check_work = this.name + "\t-\t-\tX\tX\t0\t0\t0\t" + today.ToString("yyyy-MM-dd");
                File.AppendAllText(check_schedule, check_work + Environment.NewLine);
            }
            if (clickedDatetime == DateTime.MinValue)
            {
                selectedDate = today;
            }
            else
            {
                selectedDate = clickedDatetime; // 판넬에서 선택한 날짜를 사용
            }
            if (selectedDate.ToString("yyyy-MM-dd") == today.ToString("yyyy-MM-dd")) // 선택한 날짜와 오늘 날짜를 비교
            {
                try
                {
                    List<string> lines = File.ReadAllLines(check_schedule).ToList();
                    for (int i = 1; i < lines.Count; i++)
                    {

                        string[] columns = lines[i].Split('\t');

                        if (columns[0] == this.name && columns[3] == "X" && columns[8] == today.ToString("yyyy-MM-dd"))
                        {
                            MessageBox.Show("출근 하셨습니다.");
                            columns[3] = "O";
                            columns[1] = today.ToString("HH:mm");
                            lines[i] = string.Join("\t", columns); // 변경된 값을 다시 합쳐서 해당 줄로 설정
                            File.WriteAllLines(check_schedule, lines); // 변경된 내용을 파일에 씀 
                            break;
                        }


                        if (columns[0] == this.name && columns[3] == "O" && columns[8] == today.ToString("yyyy-MM-dd"))
                        {
                            MessageBox.Show("출근을 취소 하셨습니다.");
                            columns[3] = "X";
                            columns[1] = "-";
                            lines[i] = string.Join("\t", columns); // 변경된 값을 다시 합쳐서 해당 줄로 설정
                            File.WriteAllLines(check_schedule, lines); // 변경된 내용을 파일에 씀
                            break;
                        }
                    }

                }
                catch (Exception Ex)
                {
                    MessageBox.Show(Ex.Message);
                }
            }
            else
            {
                MessageBox.Show("클릭한날은 " + DateTime.Now.ToString("yyyy년-MM월-dd일") + " 이아닙니다");

            }
            LoadDisplay();
        }
        private void button19_Click(object sender, EventArgs e) //월차 버튼
        {
            Form_vacation formVacation = new Form_vacation(form_login, form_login.Name, form_login.UserBirth, form_login.UserRent);


            // 생성된 인스턴스를 보여줌
            formVacation.Show();
        }
        
        private void button11_Click(object sender, EventArgs e) //퇴근
        {
            string check_schedule = "schedule.txt";
            DateTime selectedDate = clickedDatetime; // 달력 날짜
            TimeSpan worktimePlus = TimeSpan.Zero;
            DateTime today = DateTime.Now; // 오늘의 날짜를 가져옴
            int workcountPlus = 0;
            if (clickedDatetime == DateTime.MinValue)
            {
                selectedDate = today;
            }
            else
            {
                selectedDate = clickedDatetime; // 판넬에서 선택한 날짜를 사용
            }

            if (selectedDate.ToString("yyyy-MM-dd") == today.ToString("yyyy-MM-dd")) // 선택한 날짜와 오늘 날짜를 비교
            {

                try
                {
                    List<string> lines = File.ReadAllLines(check_schedule).ToList();
                    for (int i = 1; i < lines.Count; i++)
                    {
                        string[] columns = lines[i].Split('\t');
                        DateTime scheduleDate = DateTime.Parse(columns[8]);
                        if (this.name == columns[0] && DateTime.Parse(columns[8]) < DateTime.Today)
                        {
                            workcount = columns[5];
                        }
                        if (this.name == columns[0] && DateTime.Parse(columns[8]) <= DateTime.Today)
                        {
                            TimeSpan totalworkcount;
                            if (TimeSpan.TryParse(columns[6], out totalworkcount))
                            {
                                worktimePlus = worktimePlus + totalworkcount;
                            }
                        }
                        if (columns[0] == this.name && columns[4] == "X" && columns[8] == today.ToString("yyyy-MM-dd") && columns[3] == "O")
                        {
                            MessageBox.Show("퇴근 하셨습니다.");
                            columns[4] = "O";
                            columns[2] = today.ToString("HH:mm");
                            DateTime end = DateTime.ParseExact(columns[2], "HH:mm", CultureInfo.InvariantCulture);
                            DateTime start = DateTime.ParseExact(columns[1], "HH:mm", CultureInfo.InvariantCulture);
                            TimeSpan worktime = end - start;
                            workcountPlus = int.Parse(workcount);
                            workcountPlus++;
                            columns[5] = workcountPlus.ToString();
                            columns[6] = worktime.ToString();
                            TimeSpan previousWorktimePlus;
                            if (TimeSpan.TryParse(columns[7], out previousWorktimePlus))
                            {
                                // 현재 일의 근무 시간을 더해 새로운 누적 근무 시간 계산
                                worktimePlus = previousWorktimePlus + worktime;
                                columns[7] = worktimePlus.ToString();
                            }

                            //전날로하면안됨 전날 안나오는 경우도 있음 그것보다 열을 하나더 전으로? 3-5일 안나오면 3-4을날을 가져와야함 근데 3-4도 안나왓으면 3-3으로 이걸 거슬러 가야한다.
                            //어떻게 해야 전에 있는 columns[6]을 가져오는가 오늘이 아닌것을 가져온다? 가져올게 많지 않은가? 오늘이 30일이고 매일 출근했다면 1~29일까지 모두 가져오지 않을까
                            // 오늘보다 전날인 경우에만 처리
                            //workcount++;



                            columns[6] = worktime.ToString();

                            if (today.Day == 1)
                            {
                                columns[5] = "1";
                            }
                            else
                            {
                                columns[5] = workcountPlus.ToString();
                            }
                            lines[i] = string.Join("\t", columns); // 변경된 값을 다시 합쳐서 해당 줄로 설정
                            File.WriteAllLines(check_schedule, lines); // 변경된 내용을 파일에 씀
                            break;
                        }
                        if (columns[0] == this.name && columns[4] == "O" && columns[8] == today.ToString("yyyy-MM-dd"))
                        {
                            MessageBox.Show("퇴근을 취소 하셨습니다.");
                            columns[4] = "X";
                            columns[2] = "-";
                            workcount = columns[5];
                            workcountPlus = int.Parse(workcount);
                            workcountPlus--;
                            columns[5] = workcountPlus.ToString();
                            TimeSpan totalworkcount;
                            if (TimeSpan.TryParse(columns[6], out totalworkcount))
                            {
                                worktimePlus = worktimePlus - totalworkcount;
                            }
                            columns[7] = worktimePlus.ToString();
                            lines[i] = string.Join("\t", columns); // 변경된 값을 다시 합쳐서 해당 줄로 설정
                            File.WriteAllLines(check_schedule, lines); // 변경된 내용을 파일에 씀
                            break; // 이름이 일치하는 행을 찾았으므로 루프를 중지합니다.

                        }
                    }
                }
                catch (Exception Ex)
                {
                    MessageBox.Show(Ex.Message);
                }
            }

            else
            {
                MessageBox.Show("클릭한날은 " + DateTime.Now.ToString("yyyy년-MM월-dd일") + " 이아닙니다");

            }
            LoadDisplay();
        }
        public void LoadDisplay()
        {
            DateTime selectedDate = clickedDatetime; // 달력 날짜
            DateTime today = DateTime.Today;
            if (clickedDatetime == DateTime.MinValue)
            {
                selectedDate = today;
                //MessageBox.Show(clickedDatetime.ToString() + "김치전");
            }
            try
            {
                List<List<string>> Att_List = Patient.Att(); // 텍스트 파일의 내용을 읽어옴
                dataGridView6.Rows.Clear(); // DataGridView 초기화
                for (int k = 1; k < Att_List.Count; k++) // 읽어온 내용을 DataGridView에 표시
                {
                    List<string> Att = Att_List[k];
                    if (selectedDate.ToString("yyyy-MM-dd") == Att[8].ToString())
                    {
                        //MessageBox.Show("김치전2");
                        dataGridView6.Rows.Add(Att[0], Att[1], Att[2], Att[3], Att[4]);
                    }
                }
            }
            catch { }
        }

        private void button1_1_Click(object sender, EventArgs e)
        {
            
            form_login.textBox1.Text = null;
            form_login.textBox2.Text = null;
            
            form_login.label3.Text = "ID를 입력해주세요.";
            form_login.label4.Text = "PW를 입력해주세요.";
            form_login.textBox1.Focus();
            this.Hide();
            form_login.ShowDialog();
            
            this.Close();


        }



        //
        //환자 검색 후 환자 더블 클릭시

        private void button9_Click(object sender, EventArgs e)
        {
            Daycontainer1.Controls.Clear();
            month--;
            if (month < 1)
            {
                month = 12;
                year--;
            }
            DateTime startofthemonth = new DateTime(year, month, 1); //지난달 첫날의 정보를 가져옴
            int days = DateTime.DaysInMonth(year, month);//지난달 일 수 계산
            int dayoftheweek = Convert.ToInt32(startofthemonth.DayOfWeek.ToString("d"));// 지난 달의 시작 요일을 계산
            for (int i = 0; i < dayoftheweek; i++)// 지난 달의 시작 요일까지 공백 패널을 추가
            {
                UserControl ucblank = new UserControlBlank();//공백 패널 생성
                Daycontainer1.Controls.Add(ucblank); //공백 패널 추가
            }
            for (int i = 1; i <= days; i++)// 지난 달의 각 날짜에 해당하는 패널을 추가
            {
                UserControldays ucdays = new UserControldays();//날짜 패널 생성
                ucdays.days(i);//날짜 패널 설정
                Daycontainer1.Controls.Add(ucdays);//날짜 패널 추가
            }
            label13.Text = $"{year}년 {month}월";// 라벨에 지난 달의 연도와 월을 표시
            foreach (Control control in Daycontainer1.Controls) // 패널에 있는 UserControlDays의 클릭 이벤트 핸들러를 등록
            {
                if (control is UserControldays)
                {
                    (control as UserControldays).OnPanelClick += UserControlDays_OnPanelClick; // 해당 패널의 클릭 이벤트 핸들러 등록
                }
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Daycontainer1.Controls.Clear();
            month++;
            if (month > 12)
            {
                month = 1;
                year++;
            }
            DateTime startofthemonth = new DateTime(year, month, 1);// 다음달 첫날의 정보를 가져옴
            int days = DateTime.DaysInMonth(year, month); //다음달 일 수 계산
            int dayoftheweek = Convert.ToInt32(startofthemonth.DayOfWeek.ToString("d")); // 다음 달의 시작 요일을 계산
            for (int i = 0; i < dayoftheweek; i++)  // 다음 달의 시작 요일까지 공백 패널을 추가
            {
                UserControl ucblank = new UserControlBlank(); //공백 패널 생성
                Daycontainer1.Controls.Add(ucblank); //공백 패널 추가
            }
            for (int i = 1; i <= days; i++)// 다음 달의 각 날짜에 해당하는 패널을 추가
            {
                UserControldays ucdays = new UserControldays(); //날짜 패널 생성
                ucdays.days(i); //패널에 날짜 설정
                Daycontainer1.Controls.Add(ucdays); //날짜 패널 추가
            }
            label13.Text = $"{year}년 {month}월"; // 라벨에 다음 달의 연도와 월을 표시
            foreach (Control control in Daycontainer1.Controls) // 패널에 있는 UserControlDays의 클릭 이벤트 핸들러를 등록
            {
                if (control is UserControldays)
                {
                    (control as UserControldays).OnPanelClick += UserControlDays_OnPanelClick; // 해당 패널의 클릭 이벤트 핸들러 등록
                }
            }
        }

        //
        //환자 검색 더블 클릭 시
        //
        private void dataGridView2_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            panel1.Visible = false;
            panel2.Visible = false;
            panel3.Visible = false;
            panel4.Visible = false;
            panel8.Visible = false;
            panel1_1.Visible = false;
            panel1_2.Visible = false;
            panel57.Visible = true;
            panel8.BringToFront();
            DataGridViewRow selectedRow = dataGridView2.Rows[e.RowIndex];

            string name = selectedRow.Cells[0].Value.ToString();
            string birthday = selectedRow.Cells[2].Value.ToString();
            string phoneNumber = selectedRow.Cells[3].Value.ToString();
            string city = selectedRow.Cells[4].Value.ToString();

          
            textBox1.Text = name;   
            textBox8.Text = birthday;
            textBox9.Text = phoneNumber;
            textBox10.Text = city;

            string folderPath = "patientData"; // 폴더 경로 설정 

            string[] fileNames = Directory.GetFiles(folderPath);


            dataGridView5.Rows.Clear();

            //이후 기록이나 오늘 기록이면 안 뜨게
            foreach (string fileName in fileNames)
            {


                if (fileName.Contains(textBox1.Text) && fileName.Contains(textBox9.Text))
                {

                    if (fileName != "patientData\\" + textBox1.Text + " " + textBox9.Text + " " + DateTime.Now.ToString("yyMMdd") + ".txt")//240310
                    {
                        dataGridView5.Rows.Add(fileName);
                    }


                }

            }

    
        }

        //
        //이윤서
        //예약하기
        private void button20_Click(object sender, EventArgs e)
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            string medicalFilePath = "Reservation.txt"; // 접수 정보 파일 경로
            var medical_lines = File.ReadAllLines(medicalFilePath);

            string name = textBox1.Text;
            string birthday = textBox8.Text;
            string phoneNumber = textBox9.Text;
            string city = textBox10.Text;
            string day = textBox2.Text; // 날짜 입력
            string time = textBox11.Text; // 시간 입력

            // 입력된 날짜를 DateTime으로 변환
            DateTime previous = DateTime.Parse(day);

            // 예약 날짜가 오늘 날짜보다 미래인 경우
            if (previous > DateTime.Now)
            {
                bool isAppointmentAvailable = true; // 예약 가능 여부

                for (int i = 0; i < medical_lines.Length; i++)
                {
                    var parts = medical_lines[i].Split('\t');

                    // 동일한 날짜와 시간이 있는지 확인
                    if (parts[0] == time && parts[5] == day)
                    {
                        MessageBox.Show("동일한 날짜와 시간에 이미 예약이 있습니다.");
                        isAppointmentAvailable = false; // 예약 불가능으로 설정
                        break; // 반복문 종료
                    }
                }

                if (isAppointmentAvailable)
                {
                    if (textBox11.Text == "")
                    {
                        MessageBox.Show("입력을 확인해주세요.");
                    }
                    else
                    {
                        // 예약 가능한 경우, 예약 정보 추가
                        File.AppendAllText(medicalFilePath, time + "\t" + name + "\t" + birthday + "\t" + phoneNumber + "\t" + city + "\t" + day + Environment.NewLine);
                        MessageBox.Show("예약되었습니다.");
                    }
                }
            }
            else
            {
                MessageBox.Show("예약 할 수 없는 날짜입니다.");
            }

            // 입력 필드 초기화
            textBox2.Text = null;
            textBox11.Text = null;
        }

        //예약 삭제
        private void button16_Click(object sender, EventArgs e)
        {

            string medicalFilePath = "Reservation.txt"; // 접수 정보 파일 경로
            List<string> lines = new List<string>(File.ReadAllLines(medicalFilePath));

            // 데이터 그리드에서 선택된 행의 정보를 가져옵니다.   
            DataGridViewRow selectedRow = dataGridView4.SelectedRows[0];
            string time = selectedRow.Cells[0].Value.ToString();
            string name = selectedRow.Cells[1].Value.ToString();
            string birth = selectedRow.Cells[2].Value.ToString();
            string phone = selectedRow.Cells[3].Value.ToString();
            string city = selectedRow.Cells[4].Value.ToString();


            // 선택된 정보와 일치하는 줄을 찾아 삭제합니다.
            for (int i = 0; i < lines.Count; i++)
            {
                string[] parts = lines[i].Split('\t');

                if (parts.Length >= 2)
                {
                    string times = parts[0].Trim();
                    string names = parts[1].Trim();
                    string births = parts[2].Trim();
                    string phoneNumber = parts[3].Trim();
                    string citys = parts[4].Trim();


                    if (times == time && names == name && births == birth && phoneNumber == phone && citys == city)
                    {
                        lines.RemoveAt(i);
                        Form_login.form_main.dataGridView4.Refresh();
                        MessageBox.Show("예약이 삭제되었습니다.");

                        break; // 찾았으면 루프 종료
                    }
                }

            }

            File.WriteAllLines(medicalFilePath, lines);

        }

        //이전 진료 기록 더블 클릭시
        private void dataGridView5_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)//240310
        {
            if (dataGridView5.SelectedCells.Count > 0)
            {
                int selectedRowIndex = dataGridView5.SelectedCells[0].RowIndex;

                // 선택한 행의 메모장 정보 가져오기
                string selectedMemoInfo = dataGridView5.Rows[selectedRowIndex].Cells[0].Value.ToString();

                // 파일 이름 추출
                string fileName = Path.GetFileName(selectedMemoInfo);

                // 메모장 파일 경로 생성
                string folderPath = Path.Combine(System.Windows.Forms.Application.StartupPath, "patientData");
                string filePath = Path.Combine(folderPath, fileName);

                // 메모장 파일이 존재하는지 확인하고, 존재한다면 새 창에서 열기
                if (File.Exists(filePath))
                {
                    try
                    {
                        Process.Start("notepad.exe", filePath);//새창에서 텍스트파일 여는 코드(메모장,파일명)
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"파일을 열던 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show($"선택한 메모 파일 '{fileName}'이(가) 존재하지 않습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }



        //
        // 패널 테두리 색
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Color borderColor = Color.FromArgb(203, 216, 242);
            ControlPaint.DrawBorder(e.Graphics, this.panel1.ClientRectangle, borderColor, ButtonBorderStyle.Solid);
        }
        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            Color borderColor = Color.FromArgb(203, 216, 242);
            ControlPaint.DrawBorder(e.Graphics, this.panel1.ClientRectangle, borderColor, ButtonBorderStyle.Solid);
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {
            Color borderColor = Color.FromArgb(203, 216, 242);
            ControlPaint.DrawBorder(e.Graphics, this.panel1.ClientRectangle, borderColor, ButtonBorderStyle.Solid);
        }
        private void panel4_Paint(object sender, PaintEventArgs e)
        {
            Color borderColor = Color.FromArgb(203, 216, 242);
            ControlPaint.DrawBorder(e.Graphics, this.panel1.ClientRectangle, borderColor, ButtonBorderStyle.Solid);
        }

        private void panel57_Paint(object sender, PaintEventArgs e)
        {
            Color borderColor = Color.FromArgb(203, 216, 242);
            ControlPaint.DrawBorder(e.Graphics, this.panel1.ClientRectangle, borderColor, ButtonBorderStyle.Solid);
        }


        //
        //마우스 언더라인
        private void button1_1_MouseMove(object sender, MouseEventArgs e)
        {
            Font ft2 = new Font("G마켓 산스 TTF Light", 14, FontStyle.Underline);
            button1_1.Font = ft2;
        }
        private void button1_1_MouseLeave(object sender, EventArgs e)
        {
            Font ft2 = new Font("G마켓 산스 TTF Light", 14);
            button1_1.Font = ft2;
        }
        private void button6_MouseMove(object sender, MouseEventArgs e)
        {
            Font ft2 = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
            button6.Font = ft2;
        }
        private void button6_MouseLeave(object sender, EventArgs e)
        {
            Font ft2 = new Font("G마켓 산스 TTF Light", 16);
            button6.Font = ft2;
        }
        private void button13_MouseMove(object sender, MouseEventArgs e)
        {
            Font ft2 = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
            button13.Font = ft2;
        }
        private void button13_MouseLeave(object sender, EventArgs e)
        {
            Font ft2 = new Font("G마켓 산스 TTF Light", 16);
            button13.Font = ft2;
        }
        private void button8_MouseMove(object sender, MouseEventArgs e)
        {
            Font ft2 = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
            button8.Font = ft2;
        }
        private void button8_MouseLeave(object sender, EventArgs e)
        {
            Font ft2 = new Font("G마켓 산스 TTF Light", 16);
            button8.Font = ft2;
        }
        private void button12_MouseMove(object sender, MouseEventArgs e)
        {
            Font ft2 = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
            button12.Font = ft2;
        }
        private void button12_MouseLeave(object sender, EventArgs e)
        {
            button12.Font = new Font("G마켓 산스 TTF Light", 16);
        }
        private void button11_MouseMove(object sender, MouseEventArgs e)
        {
            button11.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
        }
        private void button11_MouseLeave(object sender, EventArgs e)
        {
            button11.Font = new Font("G마켓 산스 TTF Light", 16);
        }
        private void button7_MouseMove(object sender, MouseEventArgs e)
        {
            button7.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
        }
        private void button7_MouseLeave(object sender, EventArgs e)
        {
            button7.Font = new Font("G마켓 산스 TTF Light", 16);
        }
        private void button19_MouseMove(object sender, MouseEventArgs e)
        {
            button19.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
        }
        private void button19_MouseLeave(object sender, EventArgs e)
        {
            button19.Font = new Font("G마켓 산스 TTF Light", 16);
        }
        private void button17_MouseMove(object sender, MouseEventArgs e)
        {
            button17.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
        }
        private void button17_MouseLeave(object sender, EventArgs e)
        {
            button17.Font = new Font("G마켓 산스 TTF Light", 14);
        }
        private void button18_MouseMove(object sender, MouseEventArgs e)
        {
            button18.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
        }
        private void button18_MouseLeave(object sender, EventArgs e)
        {
            button18.Font = new Font("G마켓 산스 TTF Light", 14);
        }
        private void button9_MouseMove(object sender, MouseEventArgs e)
        {
            button9.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
        }
        private void button9_MouseLeave(object sender, EventArgs e)
        {
            button9.Font = new Font("G마켓 산스 TTF Light", 16);
        }
        private void button10_MouseMove(object sender, MouseEventArgs e)
        {
            button10.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
        }
        private void button10_MouseLeave(object sender, EventArgs e)
        {
            button10.Font = new Font("G마켓 산스 TTF Light", 16);
        }
        private void button21_MouseMove(object sender, MouseEventArgs e)
        {
            button21.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
        }
        private void button21_MouseLeave(object sender, EventArgs e)
        {
            button21.Font = new Font("G마켓 산스 TTF Light", 16);
        }
        private void button16_MouseMove(object sender, MouseEventArgs e)
        {
            button16.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
        }
        private void button16_MouseLeave(object sender, EventArgs e)
        {
            button16.Font = new Font("G마켓 산스 TTF Light", 16);
        }
        private void button20_MouseMove(object sender, MouseEventArgs e)
        {
            button20.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
        }
        private void button20_MouseLeave(object sender, EventArgs e)
        {
            button20.Font = new Font("G마켓 산스 TTF Light", 16);
        }
        private void button5_MouseMove(object sender, MouseEventArgs e)
        {
            button5.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
        }
        private void button5_MouseLeave(object sender, EventArgs e)
        {
            button5.Font = new Font("G마켓 산스 TTF Light", 16);
        }
        private void button14_MouseMove(object sender, MouseEventArgs e)
        {
            button14.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
        }
        private void button14_MouseLeave(object sender, EventArgs e)
        {
            button14.Font = new Font("G마켓 산스 TTF Light", 16);
        }
        private void button4_2_MouseMove(object sender, MouseEventArgs e)
        {
            button4_2.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
        }
        private void button4_2_MouseLeave(object sender, EventArgs e)
        {
            button4_2.Font = new Font("G마켓 산스 TTF Light", 16);
        }
        private void button4_3_MouseMove(object sender, MouseEventArgs e)
        {
            button4_3.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
        }
        private void button4_3_MouseLeave(object sender, EventArgs e)
        {
            button4_3.Font = new Font("G마켓 산스 TTF Light", 16);
        }
        private void button4_5_MouseMove(object sender, MouseEventArgs e)
        {
            button4_5.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
        }
        private void button4_5_MouseLeave(object sender, EventArgs e)
        {
            button4_5.Font = new Font("G마켓 산스 TTF Light", 16);
        }
        private void button4_1_1_MouseMove_1(object sender, MouseEventArgs e)
        {
            button4_1_1.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
        }

        private void button4_1_1_MouseLeave_1(object sender, EventArgs e)
        {
            button4_1_1.Font = new Font("G마켓 산스 TTF Light", 16);
        }
        private void button4_4_MouseMove(object sender, MouseEventArgs e)
        {
            button4_4.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);

        }
        private void button4_4_MouseLeave(object sender, EventArgs e)
        {
            button4_4.Font = new Font("G마켓 산스 TTF Light", 16);

        }

        private void button15_MouseMove(object sender, MouseEventArgs e)
        {
            button15.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
        }

        private void button15_MouseLeave(object sender, EventArgs e)
        {
            button15.Font = new Font("G마켓 산스 TTF Light", 16);
        }


        //신규 등록 Message 박스와 label
        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            label3.Text = textBox4.Text;
            char[] inputchars = textBox4.Text.ToCharArray();          //한글만 들어가게
            var sb = new StringBuilder();

            foreach (var item in inputchars)
            {
                if (char.GetUnicodeCategory(item) == UnicodeCategory.OtherLetter)
                {
                    sb.Append(item);
                    label10.Visible = false;
                }
                else
                {
                    label10.Visible = true;
                }
            }
            textBox4.Text = sb.ToString().Trim();

        }

        //
        // Message박스와 label
        private void label3_Click(object sender, EventArgs e)
        {
            textBox4.Focus();
            textBox4.MaxLength = 7;
            label3.Text = null;
            textBox4.Text = null;
        }
        private void label4_Click(object sender, EventArgs e)
        {
            textBox5.Focus();
            textBox5.MaxLength = 10;
            label4.Text = null;
            textBox5.Text = null;
        }
        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            label4.Text = textBox5.Text;
        }
        private void label11_Click(object sender, EventArgs e)
        {
            textBox6.Focus();
            textBox6.MaxLength = 13;
            label11.Text = null;
            textBox6.Text = null;
        }
        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            label11.Text = textBox6.Text;
        }
        private void label12_Click(object sender, EventArgs e)
        {
            textBox7.Focus();
            textBox7.MaxLength = 13;
            label12.Text = null;
            textBox7.Text = null;
        }
        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            label12.Text = textBox7.Text;
        }
        private void label34_Click(object sender, EventArgs e)
        {
            main_textBox.Focus();
            label34.Text = null;
        }
        private void main_textBox_TextChanged(object sender, EventArgs e)
        {
            Patient.Psearch();
            label34.Text = main_textBox.Text;
        }
        private void textBox4_1_1_TextChanged(object sender, EventArgs e)
        {
            label4_1_1.Text = textBox4_1_1.Text;
        }



        private void textBox4_1_2_TextChanged(object sender, EventArgs e)
        {
            label4_1_2.Text = textBox4_1_2.Text;
        }
        private void textBox4_1_3_TextChanged(object sender, EventArgs e)
        {
            label4_1_3.Text = textBox4_1_3.Text;
        }
        private void textBox4_1_4_TextChanged(object sender, EventArgs e)
        {
            label4_1_4.Text = textBox4_1_4.Text;
        }
        private void textBox4_1_5_TextChanged(object sender, EventArgs e)
        {
            label4_1_5.Text = textBox4_1_5.Text;
        }
        private void textBox4_1_6_TextChanged(object sender, EventArgs e)
        {
            label4_1_6.Text = textBox4_1_6.Text;
        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            label58.Text = textBox1.Text;
        }
        private void label58_Click(object sender, EventArgs e)
        {
            textBox1.Focus();
        }
        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            label60.Text = textBox10.Text;
        }
        private void label60_Click(object sender, EventArgs e)
        {
            textBox10.Focus();
        }
        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            label65.Text = textBox8.Text;
        }
        private void label65_Click(object sender, EventArgs e)
        {
            textBox8.Focus();
        }
        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            label66.Text = textBox9.Text;
        }
        private void label66_Click(object sender, EventArgs e)
        {
            textBox9.Focus();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            label45.Text = textBox2.Text;
        }
        private void label45_Click(object sender, EventArgs e)
        {
            textBox2.Focus();
        }
        private void label54_Click(object sender, EventArgs e)
        {
            panel106.Visible = true;
            panel107.Visible = false;
            label54.Font = new System.Drawing.Font(label54.Font, label54.Font.Style | System.Drawing.FontStyle.Underline);
            label53.Font = new System.Drawing.Font(label53.Font, label53.Font.Style & ~System.Drawing.FontStyle.Underline);
        }
        private void label53_Click(object sender, EventArgs e)
        {
            panel107.Visible = true;
            panel106.Visible = false;
            label53.Font = new System.Drawing.Font(label54.Font, label54.Font.Style | System.Drawing.FontStyle.Underline);
            label54.Font = new System.Drawing.Font(label53.Font, label53.Font.Style & ~System.Drawing.FontStyle.Underline);
        }
        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                Patient.Pnew();
                textBox4.Text = null;
                textBox5.Text = null;
                textBox6.Text = null;
                textBox7.Text = null;
            }
            catch
            {
                label10.Visible = true;
            }

        }
        private void button5_Click(object sender, EventArgs e)
        {
            Patient.Psearch();

            if (panel1.Visible == true)
            {
                panel1.Visible = true;
                panel2.Visible = false;
                panel3.Visible = false;
                panel4.Visible = false;
                panel57.Visible = false;

            }
            else if (panel2.Visible == true)
            {
                panel1.Visible = false;
                panel2.Visible = true;
                panel3.Visible = false;
                panel4.Visible = false;
                panel57.Visible = false;

            }
            else if (panel3.Visible == true)
            {
                panel1.Visible = false;
                panel2.Visible = false;
                panel3.Visible = true;
                panel4.Visible = false;
                panel57.Visible = false;

            }
            else if (panel3.Visible == true)
            {
                panel1.Visible = false;
                panel2.Visible = false;
                panel3.Visible = false;
                panel4.Visible = true;
                panel57.Visible = false;
            }
            else
            {
                panel1.Visible = false;
                panel2.Visible = false;
                panel3.Visible = false;
                panel4.Visible = false;
                panel57.Visible = true;

            }
            panel8.Visible = true;
            //dataGridView 처음에 셀 선택 안 되어있게
            dataGridView2.ClearSelection();

        }
        private void button12_Click(object sender, EventArgs e)
        {
            if (button12.Text == "수정")
            {
                button12.Text = "완료";
                dataGridView2.EditMode = DataGridViewEditMode.EditOnEnter; // 편집 모드 설정
                dataGridView2.ReadOnly = false;
            }
            else
            {
                try
                {
                    // DataGridView의 각 행에서 빈칸 체크 및 예외 처리
                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        if (row.IsNewRow) continue; // 새로운 행은 무시

                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (string.IsNullOrWhiteSpace(cell.Value?.ToString()))
                            {
                                MessageBox.Show("빈칸을 채워주세요.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }
                    button12.Text = "수정";
                    dataGridView2.ReadOnly = true;
                    Patient.SaveDataToTextFile();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"오류 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void panel8_MouseClick(object sender, MouseEventArgs e)
        {
            if (panel8.Visible && !panel8.Bounds.Contains(e.Location))
            {
                try
                {
                    // DataGridView의 각 행에서 빈칸 체크 및 예외 처리
                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        if (row.IsNewRow) continue; // 새로운 행은 무시

                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (string.IsNullOrWhiteSpace(cell.Value?.ToString()))
                            {
                                MessageBox.Show("빈칸을 채워주세요.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }
                    panel8.Visible = false;
                    button12.Text = "수정";
                    dataGridView2.ReadOnly = true;
                    Patient.SaveDataToTextFile();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"오류 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void Form_main_MouseClick(object sender, MouseEventArgs e)
        {
            if (panel8.Visible && !panel8.Bounds.Contains(e.Location))
            {
                try
                {
                    // DataGridView의 각 행에서 빈칸 체크 및 예외 처리
                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        if (row.IsNewRow) continue; // 새로운 행은 무시

                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (string.IsNullOrWhiteSpace(cell.Value?.ToString()))
                            {
                                MessageBox.Show("빈칸을 채워주세요.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }
                    panel8.Visible = false;
                    button12.Text = "수정";
                    dataGridView2.ReadOnly = true;
                    Patient.SaveDataToTextFile();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"오류 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

     
        private void tabControl1_MouseClick(object sender, MouseEventArgs e)
        {
            if (panel8.Visible && !panel8.Bounds.Contains(e.Location))
            {
                try
                {
                    // DataGridView의 각 행에서 빈칸 체크 및 예외 처리
                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        if (row.IsNewRow) continue; // 새로운 행은 무시

                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (string.IsNullOrWhiteSpace(cell.Value?.ToString()))
                            {
                                MessageBox.Show("빈칸을 채워주세요.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }
                    panel8.Visible = false;
                    button12.Text = "수정";
                    dataGridView2.ReadOnly = true;
                    Patient.SaveDataToTextFile();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"오류 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void panel5_MouseClick(object sender, MouseEventArgs e)
        {
            if (panel8.Visible && !panel8.Bounds.Contains(e.Location))
            {
                try
                {
                    // DataGridView의 각 행에서 빈칸 체크 및 예외 처리
                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        if (row.IsNewRow) continue; // 새로운 행은 무시

                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (string.IsNullOrWhiteSpace(cell.Value?.ToString()))
                            {
                                MessageBox.Show("빈칸을 채워주세요.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }
                    panel8.Visible = false;
                    button12.Text = "수정";
                    dataGridView2.ReadOnly = true;
                    Patient.SaveDataToTextFile();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"오류 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void panel13_MouseClick(object sender, MouseEventArgs e)
        {
            if (panel8.Visible && !panel8.Bounds.Contains(e.Location))
            {
                try
                {
                    // DataGridView의 각 행에서 빈칸 체크 및 예외 처리
                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        if (row.IsNewRow) continue; // 새로운 행은 무시

                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (string.IsNullOrWhiteSpace(cell.Value?.ToString()))
                            {
                                MessageBox.Show("빈칸을 채워주세요.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }
                    panel8.Visible = false;
                    button12.Text = "수정";
                    dataGridView2.ReadOnly = true;
                    Patient.SaveDataToTextFile();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"오류 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void panel3_MouseClick(object sender, MouseEventArgs e)
        {
            if (panel8.Visible && !panel8.Bounds.Contains(e.Location))
            {
                try
                {
                    // DataGridView의 각 행에서 빈칸 체크 및 예외 처리
                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        if (row.IsNewRow) continue; // 새로운 행은 무시

                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (string.IsNullOrWhiteSpace(cell.Value?.ToString()))
                            {
                                MessageBox.Show("빈칸을 채워주세요.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }
                    panel8.Visible = false;
                    button12.Text = "수정";
                    dataGridView2.ReadOnly = true;
                    Patient.SaveDataToTextFile();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"오류 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void panel4_MouseClick(object sender, MouseEventArgs e)
        {
            if (panel8.Visible && !panel8.Bounds.Contains(e.Location))
            {
                try
                {
                    // DataGridView의 각 행에서 빈칸 체크 및 예외 처리
                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        if (row.IsNewRow) continue; // 새로운 행은 무시

                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (string.IsNullOrWhiteSpace(cell.Value?.ToString()))
                            {
                                MessageBox.Show("빈칸을 채워주세요.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }
                    panel8.Visible = false;
                    button12.Text = "수정";
                    dataGridView2.ReadOnly = true;
                    Patient.SaveDataToTextFile();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"오류 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dataGridView4_1_MouseClick(object sender, MouseEventArgs e)
        {
            if (panel8.Visible && !panel8.Bounds.Contains(e.Location))
            {
                try
                {
                    // DataGridView의 각 행에서 빈칸 체크 및 예외 처리
                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        if (row.IsNewRow) continue; // 새로운 행은 무시

                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (string.IsNullOrWhiteSpace(cell.Value?.ToString()))
                            {
                                MessageBox.Show("빈칸을 채워주세요.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }
                    panel8.Visible = false;
                    button12.Text = "수정";
                    dataGridView2.ReadOnly = true;
                    Patient.SaveDataToTextFile();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"오류 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

     

        private void tabControl2_MouseClick(object sender, MouseEventArgs e)
        {
            if (panel8.Visible && !panel8.Bounds.Contains(e.Location))
            {
                try
                {
                    // DataGridView의 각 행에서 빈칸 체크 및 예외 처리
                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        if (row.IsNewRow) continue; // 새로운 행은 무시

                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (string.IsNullOrWhiteSpace(cell.Value?.ToString()))
                            {
                                MessageBox.Show("빈칸을 채워주세요.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }
                    panel8.Visible = false;
                    button12.Text = "수정";
                    dataGridView2.ReadOnly = true;
                    Patient.SaveDataToTextFile();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"오류 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dataGridView4_2_MouseClick(object sender, MouseEventArgs e)
        {
            if (panel8.Visible && !panel8.Bounds.Contains(e.Location))
            {
                try
                {
                    // DataGridView의 각 행에서 빈칸 체크 및 예외 처리
                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        if (row.IsNewRow) continue; // 새로운 행은 무시

                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (string.IsNullOrWhiteSpace(cell.Value?.ToString()))
                            {
                                MessageBox.Show("빈칸을 채워주세요.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }
                    panel8.Visible = false;
                    button12.Text = "수정";
                    dataGridView2.ReadOnly = true;
                    Patient.SaveDataToTextFile();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"오류 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void panel6_MouseClick(object sender, MouseEventArgs e)
        {
            if (panel8.Visible && !panel8.Bounds.Contains(e.Location))
            {
                try
                {
                    // DataGridView의 각 행에서 빈칸 체크 및 예외 처리
                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        if (row.IsNewRow) continue; // 새로운 행은 무시

                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (string.IsNullOrWhiteSpace(cell.Value?.ToString()))
                            {
                                MessageBox.Show("빈칸을 채워주세요.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }
                    panel8.Visible = false;
                    button12.Text = "수정";
                    dataGridView2.ReadOnly = true;
                    Patient.SaveDataToTextFile();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"오류 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void chart1_MouseClick(object sender, MouseEventArgs e)
        {
            if (panel8.Visible && !panel8.Bounds.Contains(e.Location))
            {
                try
                {
                    // DataGridView의 각 행에서 빈칸 체크 및 예외 처리
                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        if (row.IsNewRow) continue; // 새로운 행은 무시

                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (string.IsNullOrWhiteSpace(cell.Value?.ToString()))
                            {
                                MessageBox.Show("빈칸을 채워주세요.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }
                    panel8.Visible = false;
                    button12.Text = "수정";
                    dataGridView2.ReadOnly = true;
                    Patient.SaveDataToTextFile();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"오류 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void tabPage1_MouseClick(object sender, MouseEventArgs e)
        {
            if (panel8.Visible && !panel8.Bounds.Contains(e.Location))
            {
                try
                {
                    // DataGridView의 각 행에서 빈칸 체크 및 예외 처리
                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        if (row.IsNewRow) continue; // 새로운 행은 무시

                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (string.IsNullOrWhiteSpace(cell.Value?.ToString()))
                            {
                                MessageBox.Show("빈칸을 채워주세요.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }
                    panel8.Visible = false;
                    button12.Text = "수정";
                    dataGridView2.ReadOnly = true;
                    Patient.SaveDataToTextFile();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"오류 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void tabPage2_MouseClick(object sender, MouseEventArgs e)
        {
            if (panel8.Visible && !panel8.Bounds.Contains(e.Location))
            {
                try
                {
                    // DataGridView의 각 행에서 빈칸 체크 및 예외 처리
                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        if (row.IsNewRow) continue; // 새로운 행은 무시

                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (string.IsNullOrWhiteSpace(cell.Value?.ToString()))
                            {
                                MessageBox.Show("빈칸을 채워주세요.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }
                    panel8.Visible = false;
                    button12.Text = "수정";
                    dataGridView2.ReadOnly = true;
                    Patient.SaveDataToTextFile();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"오류 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void label2_MouseClick(object sender, MouseEventArgs e)
        {
            if (panel8.Visible && !panel8.Bounds.Contains(e.Location))
            {
                try
                {
                    // DataGridView의 각 행에서 빈칸 체크 및 예외 처리
                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        if (row.IsNewRow) continue; // 새로운 행은 무시

                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (string.IsNullOrWhiteSpace(cell.Value?.ToString()))
                            {
                                MessageBox.Show("빈칸을 채워주세요.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }
                    panel8.Visible = false;
                    button12.Text = "수정";
                    dataGridView2.ReadOnly = true;
                    Patient.SaveDataToTextFile();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"오류 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        //
        //이윤서
        // 환자 예약 수정
        private void button21_Click(object sender, EventArgs e)
        {
            if (button21.Text == "수정하기")
            {
                button21.Text = "수정완료";
                dataGridView4.EditMode = DataGridViewEditMode.EditOnEnter; // 편집 모드 설정
                dataGridView4.ReadOnly = false;
            }
            else
            {
                try
                {
                    // DataGridView의 각 행에서 빈칸 체크 및 예외 처리
                    foreach (DataGridViewRow row in dataGridView4.Rows)
                    {
                        if (row.IsNewRow) continue; // 새로운 행은 무시

                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (string.IsNullOrWhiteSpace(cell.Value?.ToString()))
                            {
                                MessageBox.Show("빈칸을 채워주세요.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }
                    button21.Text = "수정하기";
                    dataGridView4.ReadOnly = true;
                    Patient.SaveDataToTextFile();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"오류 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void button4_1_1_Click(object sender, EventArgs e)
        {
            form_man_modify = new Form_man_modify();
            form_man_modify.ShowDialog();
        }


        private void label1_MouseClick(object sender, MouseEventArgs e)
        {
            if (panel8.Visible && !panel8.Bounds.Contains(e.Location))
            {
                try
                {
                    // DataGridView의 각 행에서 빈칸 체크 및 예외 처리
                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {
                        if (row.IsNewRow) continue; // 새로운 행은 무시

                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (string.IsNullOrWhiteSpace(cell.Value?.ToString()))
                            {
                                MessageBox.Show("빈칸을 채워주세요.", "경고", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }
                    }
                    panel8.Visible = false;
                    button12.Text = "수정";
                    dataGridView2.ReadOnly = true;
                    Patient.SaveDataToTextFile();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"오류 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // 재고 이벤트 -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private static bool Isnumber(string input)
        {
            foreach (char c in input)
            {
                if (!char.IsDigit(c))
                {
                    return false;
                }
            }
            return true;
        }
        private void textBox3_1_1_Click(object sender, EventArgs e)             // 코드 입력창
        {
            textBox3_1_1.Text = null;
        }
        private void textBox3_1_1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button3_1_1_Click(sender, e);
            }
        }
        private void textBox3_1_1_TextChanged(object sender, EventArgs e)
        {
            label3_1_3.Text = textBox3_1_1.Text;
            dataGridView3_2_1.Rows.Clear();
            dataGridView3_3_1.Rows.Clear();
            Inventory.line_inv();
        }
        private void textBox3_1_2_Click(object sender, EventArgs e)             // 이름 입력창
        {
            textBox3_1_2.Text = null;
        }
        private void textBox3_1_2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button3_1_2_Click(sender, e);
            }
        }
        private void textBox3_1_2_TextChanged(object sender, EventArgs e)
        {
            label3_1_5.Text = textBox3_1_2.Text;
            dataGridView3_2_1.Rows.Clear();
            dataGridView3_3_1.Rows.Clear();
            Inventory.line_inv();
        }
        private void textBox3_2_1_Click(object sender, EventArgs e)             // 수량 입력창
        {
            textBox3_2_1.Text = null;
        }
        private void textBox3_2_1_TextChanged(object sender, EventArgs e)
        {
            label3_2_2.Text = textBox3_2_1.Text;
        }
        private void textBox3_3_1_Click(object sender, EventArgs e)             // 장바구니 수량 입력창
        {
            textBox3_3_1.Text = null;
        }
        private void textBox3_3_1_TextChanged(object sender, EventArgs e)
        {
            label3_3_2.Text = textBox3_3_1.Text;
        }
        private void button3_1_1_Click(object sender, EventArgs e)          // 제품코드 검색 버튼
        {
            dataGridView3_2_1.Rows.Clear();
            dataGridView3_3_1.Rows.Clear();
            Inventory.line_inv();
            textBox3_1_1.Focus();
        }
        private void button3_1_2_Click(object sender, EventArgs e)          // 제품코드 검색 버튼
        {
            dataGridView3_2_1.Rows.Clear();
            dataGridView3_3_1.Rows.Clear();
            Inventory.line_inv();
            textBox3_1_2.Focus();
            textBox3_2_1.Visible = false;
        }
        private void button3_1_3_Click(object sender, EventArgs e)        // 제품 등록 버튼
        {
            form_inv_add = new Form_inv_add();
            form_inv_add.ShowDialog();
        }
        private void button3_1_3_KeyDown(object sender, KeyEventArgs e)     // 작동 안됨
        {
            if (e.KeyCode == Keys.Tab)
            {
                textBox3_1_1.Focus();
            }
        }
        private void button3_1_3_MouseMove(object sender, MouseEventArgs e)
        {
            button3_1_3.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
        }

        private void button3_1_3_MouseLeave(object sender, EventArgs e)
        {
            button3_1_3.Font = new Font("G마켓 산스 TTF Light", 16);
        }
        private void button3_2_1_Click(object sender, EventArgs e)        // 사용버튼
        {
            Inventory.use_inv();
            textBox3_2_1.Text = null;
            textBox3_2_1.Focus();
        }
        
        private void button3_2_1_MouseMove(object sender, MouseEventArgs e)
        {
            button3_2_1.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
        }
        private void button3_2_1_MouseLeave(object sender, EventArgs e)
        {
            button3_2_1.Font = new Font("G마켓 산스 TTF Light", 16);

        }
        private void button3_2_2_Click(object sender, EventArgs e)          // 삭제 버튼
        {
            Inventory.del_inv();
        }
        private void button3_2_2_KeyDown(object sender, KeyEventArgs e)     
        {
            if (e.KeyCode == Keys.Tab)
            {
                textBox3_2_1.Focus();
            }
        }
        private void button3_2_2_MouseMove(object sender, MouseEventArgs e)
        {
            button3_2_2.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
        }
        private void button3_2_2_MouseLeave(object sender, EventArgs e)
        {
            button3_2_2.Font = new Font("G마켓 산스 TTF Light", 16);
        }
        private async void button3_3_1_Click(object sender, EventArgs e)          // 주문 마이너스 버튼
        {
            if (textBox3_3_1.Text != "" && Isnumber(textBox3_3_1.Text))
            {
                for (int i = 0; i < dataGridView3_3_1.Rows.Count; i++)
                {
                    if ((Convert.ToBoolean(dataGridView3_3_1.Rows[i].Cells[2].Value) == true))
                    {
                        int x = int.Parse(dataGridView3_3_1.Rows[i].Cells[1].Value.ToString());
                        if (x >= Int32.Parse(textBox3_3_1.Text))
                        {
                            dataGridView3_3_1.Rows[i].Cells[1].Value = (x - Int32.Parse(textBox3_3_1.Text)).ToString();
                        }
                        else
                        {
                            textBox3_3_1.Text = "잘못된 값입니다.";
                            break;
                        }
                    }
                }
            }
            else
            {
                textBox3_3_1.Text = null;
                textBox3_3_1.Text = "잘못된 값입니다.";
            }
            await Task.Delay(1000);                     // 일정시간후 에러메시지 삭제
            textBox3_3_1.Text = null;
            textBox3_3_1.Focus();
        }

        private async void button3_3_2_Click(object sender, EventArgs e)          // 주문 플러스 버튼
        {
            if (textBox3_3_1.Text != "" && Isnumber(textBox3_3_1.Text))
            {
                for (int i = 0; i < dataGridView3_3_1.Rows.Count; i++)
                {
                    if (Convert.ToBoolean(dataGridView3_3_1.Rows[i].Cells[2].Value) == true)
                    {
                        int x = int.Parse(dataGridView3_3_1.Rows[i].Cells[1].Value.ToString());
                        dataGridView3_3_1.Rows[i].Cells[1].Value = (x + Int32.Parse(textBox3_3_1.Text));
                    }
                }
                textBox3_3_1.Text = null;
            }
            else
            {
                textBox3_3_1.Text = "잘못된 값입니다.";
            }
            await Task.Delay(1000);                     // 일정시간후 에러메시지 삭제
            textBox3_3_1.Text = null;
            textBox3_3_1.Focus();
        }
        private void button3_3_3_Click(object sender, EventArgs e)         // 결재버튼
        {
            Inventory.app_inv();
        }
        private void button3_3_3_MouseMove(object sender, MouseEventArgs e)
        {
            button3_3_3.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
        }

        private void button3_3_3_MouseLeave(object sender, EventArgs e)
        {
            button3_3_3.Font = new Font("G마켓 산스 TTF Light", 16);
        }
        private void label3_1_3_Click(object sender, EventArgs e)
        {
            textBox3_1_1.Focus();
        }
        private void label3_1_5_Click(object sender, EventArgs e)
        {
            textBox3_1_2.Focus();
        }
        private void label3_2_2_Click(object sender, EventArgs e)
        {
            textBox3_2_1.Focus();
        }
        private void label3_3_2_Click(object sender, EventArgs e)
        {
            textBox3_3_1.Focus();
        }

        private void dataGridView3_2_1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int count = 0;
            string file = "Inventory Manager.txt";
            List<string> list = File.ReadAllLines(file).ToList();
            List<string> header = new List<string>();       // 카테고리 행을 담을 리스트
            List<string> lines = new List<string>();        // 내용을 담을 리스트

            if (e.RowIndex >= 0 && e.ColumnIndex == 3 && Convert.ToBoolean(dataGridView3_2_1.Rows[e.RowIndex].Cells[3].Value) == false)
            {
                dataGridView3_2_1.Rows[e.RowIndex].Cells[3].Value = true;       // 체크박스를 true
            }
            else if (e.RowIndex >= 0 && e.ColumnIndex == 3 && Convert.ToBoolean(dataGridView3_2_1.Rows[e.RowIndex].Cells[3].Value) == true)
            {
                dataGridView3_2_1.Rows[e.RowIndex].Cells[3].Value = false;
            }
            for (int i = 0; i < dataGridView3_2_1.Rows.Count; i++)
            {
                if (Convert.ToBoolean(dataGridView3_2_1.Rows[i].Cells[3].Value) == true)        // 선택되어있는 체크박스가 하나라도 있는 경우
                {
                    button3_2_1.Visible = true;
                    button3_2_2.Visible = true;
                    textBox3_2_1.Visible = true;
                    label3_2_2.Visible = true;
                    panel10.Visible = true;
                    textBox3_2_1.Text = "1";          // 기본 1로
                    textBox3_2_1.Focus();
                    count++;
                }
                else if (count == 0)         // 선택되어있는 체크박스가 하나도 없는 경우
                {
                    button3_2_1.Visible = false;
                    button3_2_2.Visible = false;
                    textBox3_2_1.Visible = false;
                    label3_2_2.Visible = false;
                    panel10.Visible = false;
                }
            }
        }
        private bool allselected1 = false;
        private void dataGridView3_2_1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)      // 모두 선택
        {
            if (dataGridView3_2_1.RowCount > 0 && e.ColumnIndex == 3)
            {
                allselected1 = !allselected1;
                for (int i = 0; i < dataGridView3_2_1.RowCount; i++)
                {
                    dataGridView3_2_1.Rows[i].Cells[3].Value = allselected1;
                }
            }
        }
        private void dataGridView3_3_1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int count = 0;
            if (e.RowIndex >= 0 && e.ColumnIndex == 2 && Convert.ToBoolean(dataGridView3_3_1.Rows[e.RowIndex].Cells[2].Value) == false)
            {
                dataGridView3_3_1.Rows[e.RowIndex].Cells[2].Value = true;       // 체크박스를 true
            }
            else if (e.RowIndex >= 0 && e.ColumnIndex == 2 && Convert.ToBoolean(dataGridView3_3_1.Rows[e.RowIndex].Cells[2].Value) == true)
            {
                dataGridView3_3_1.Rows[e.RowIndex].Cells[2].Value = false;
            }
            for (int i = 0; i < dataGridView3_3_1.Rows.Count; i++)
            {
                if (Convert.ToBoolean(dataGridView3_3_1.Rows[i].Cells[2].Value) == true)
                {
                    button3_3_1.Visible = true;
                    button3_3_2.Visible = true;
                    button3_3_3.Visible = true;
                    textBox3_3_1.Visible = true;
                    label3_3_2.Visible = true;
                    panel105.Visible = true;
                    textBox3_3_1.Text = "1";
                    textBox3_3_1.Focus();
                    count++;
                }
                else if (count == 0)         // 선택되어있는 체크박스가 하나도 없는 경우
                {
                    button3_3_1.Visible = false;
                    button3_3_2.Visible = false;
                    button3_3_3.Visible = false;
                    textBox3_3_1.Visible = false;
                    label3_3_2.Visible = false;
                    panel105.Visible = false;
                }
            }
        }
        private bool allselected2 = false;
        private void dataGridView3_3_1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridView3_3_1.RowCount > 0 && e.ColumnIndex == 2)
            {
                allselected2 = !allselected2;
                for (int i = 0; i < dataGridView3_3_1.RowCount; i++)
                {
                    dataGridView3_3_1.Rows[i].Cells[2].Value = allselected2;
                }
            }
        }

        // ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------재고 이벤트

        // 관리자 이벤트 -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            label52.Text = textBox3.Text;
        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {
            label57.Text = textBox12.Text;
        }

        private void textBox13_TextChanged(object sender, EventArgs e)
        {
            label59.Text = textBox13.Text;
        }

        private void textBox14_TextChanged(object sender, EventArgs e)
        {
            label61.Text = textBox14.Text;
        }

        private void textBox15_TextChanged(object sender, EventArgs e)
        {
            label62.Text = textBox15.Text;
        }

        private void textBox16_TextChanged(object sender, EventArgs e)
        {
            label64.Text = textBox16.Text;
        }

        private void textBox17_TextChanged(object sender, EventArgs e)
        {
            label68.Text = textBox17.Text;
        }

        private void textBox18_TextChanged(object sender, EventArgs e)
        {
            label69.Text = textBox18.Text;
        }

        private void textBox19_TextChanged(object sender, EventArgs e)
        {
            label70.Text = textBox19.Text;
        }

        private void label52_Click(object sender, EventArgs e)
        {
            textBox3.Focus();
        }

        private void label57_Click(object sender, EventArgs e)
        {
            textBox12.Focus();
        }

        private void label59_Click(object sender, EventArgs e)
        {
            textBox13.Focus();
        }

        private void label61_Click(object sender, EventArgs e)
        {
            textBox14.Focus();
        }

        private void label62_Click(object sender, EventArgs e)
        {
            textBox15.Focus();
        }

        private void label64_Click(object sender, EventArgs e)
        {
            textBox16.Focus();
        }

        private void label68_Click(object sender, EventArgs e)
        {
            textBox17.Focus();
        }

        private void label69_Click(object sender, EventArgs e)
        {
            textBox18.Focus();
        }

        private void label70_Click(object sender, EventArgs e)
        {
            textBox19.Focus();
        }
        private void button4_1_Click(object sender, EventArgs e)        // 신규 버튼
        {
            form_man_add = new Form_man_add();
            form_man_add.ShowDialog();
        }
        private void button4_1_MouseMove(object sender, MouseEventArgs e)
        {
            button4_1.Font = new Font("G마켓 산스 TTF Light", 16, FontStyle.Underline);
        }


        private void button4_1_MouseLeave(object sender, EventArgs e)
        {
            button4_1.Font = new Font("G마켓 산스 TTF Light", 16);
        }


        private void button4_3_Click(object sender, EventArgs e)        // 주문 버튼
        {
            int count = 0;
            for (int i = 0; i < dataGridView4_6.RowCount; i++)
            {
                if (Convert.ToBoolean(dataGridView4_6.Rows[i].Cells[3].Value))
                {
                    count++;
                }
            }
            if (count != 0)
            {
                Manage.pay_inv();
            }
        }
        private void button4_5_Click(object sender, EventArgs e)
        {
            int count = 0;
            for (int i = 0; i < dataGridView4_6.RowCount; i++)
            {
                if (Convert.ToBoolean(dataGridView4_6.Rows[i].Cells[3].Value))
                {
                    count++;
                }
            }
            if (count != 0)
            {
                Manage.cancel_inv();
            }
        }
        private void button4_4_Click(object sender, EventArgs e)
        {
            string file_vac_okay = "vac_Okay.txt";
            string file_vac_req = "vac_req.txt";
            List<string> newVacReqLines = new List<string>();
            for (int i = dataGridView4_5.Rows.Count - 1; i >= 0; i--)
            {
                string[] lineVacokay = File.ReadAllLines(file_vac_okay);
                if (Convert.ToBoolean(dataGridView4_5.Rows[i].Cells[5].Value))
                {
                    string Line = $"{dataGridView4_5.Rows[i].Cells[0].Value}\t{dataGridView4_5.Rows[i].Cells[1].Value}\t{dataGridView4_5.Rows[i].Cells[2].Value}\t{dataGridView4_5.Rows[i].Cells[3].Value}\t{dataGridView4_5.Rows[i].Cells[4].Value}";
                    bool check = false;
                    string[] vac_req = Line.Split('\t');
                    for (int j = 1; j < lineVacokay.Length; j++)
                    {
                        string[] check_vac = lineVacokay[j].Split('\t');
                        if (check_vac[0] == vac_req[0] && check_vac[4] == vac_req[4])
                        {
                            MessageBox.Show("중복된 휴가 신청입니다.");
                            string[] lines = File.ReadAllLines(file_vac_req);
                            List<string> newLines = new List<string>();

                            for (int l = 1; l < lines.Length; l++)
                            {
                                string[] vac_req_line = lines[l].Split('\t');
                                if (vac_req_line[0] != check_vac[0] || vac_req_line[4] != check_vac[4])
                                {
                                    newLines.Add(lines[l]);
                                }
                            }

                            File.WriteAllLines(file_vac_req, newLines);

                            dataGridView4_5.Rows.RemoveAt(i); 
                            string[] linz = File.ReadAllLines("info.txt");
                            List<string> newLinz = new List<string>();

                            foreach (string line in linz)
                            {
                                string[] splitLine = line.Split('\t');
                                if (splitLine[1] == check_vac[1]) // 주민번호를 기준으로 찾음
                                {
                                    int monthRent = int.Parse(splitLine[11]);
                                    if (check_vac[2] == "월차")
                                    {
                                        monthRent += 2;
                                    }
                                    if (check_vac[2] == "반차")
                                    {
                                        monthRent++;
                                    }
                                    splitLine[11] = monthRent.ToString();

                                    newLinz.Add(string.Join("\t", splitLine)); // 변경된 줄을 다시 조합하여 리스트에 추가
                                }
                                else
                                {
                                    newLinz.Add(line); // 변경이 필요 없는 경우 그대로 유지
                                }
                            }
                            File.WriteAllLines("info.txt", newLinz); // 변경된 내용을 파일에 씀
                            check = true;
                            break;

                        }

                    }
                    if (!check)
                    {
                        File.AppendAllText(file_vac_okay, Line + Environment.NewLine);

                        // vac_req 파일에서 삭제
                        string[] lines = File.ReadAllLines(file_vac_req);
                        List<string> newLines = new List<string>();

                        foreach (string vacLine in lines)
                        {
                            string[] splitLine = vacLine.Split('\t');
                            if (splitLine[0] != dataGridView4_5.Rows[i].Cells[0].Value.ToString() || splitLine[4] != dataGridView4_5.Rows[i].Cells[4].Value.ToString())
                            {
                                newLines.Add(vacLine);
                            }
                        }

                        File.WriteAllLines(file_vac_req, newLines);

                        dataGridView4_5.Rows.RemoveAt(i);// dataGridView7에서 행 삭제
                    }
                }
            }
        }
        private void dataGridView4_1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                Manage.p_info(e.RowIndex);
                Manage.p_salary(e.RowIndex);
                Manage.Att_mang(e.RowIndex);
                button4_1_1.Visible = true;
                label52.Text = label4_1_1.Text;
                label57.Text = label4_1_2.Text;
                label59.Text = label4_1_4.Text;
                label61.Text = label4_1_6.Text;
                label64.Text = dataGridView4_1.Rows[e.RowIndex].Cells[1].Value.ToString();
                label68.Text = label4_1_3.Text;
            }
        }
        

        private void dataGridView4_2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == 6 && dataGridView4_2.Rows[e.RowIndex].Cells[6].Value.ToString() == "N")
            {
                dataGridView4_2.Rows[e.RowIndex].Cells[6].Value = "Y";
            }
            else if (e.RowIndex >= 0 && e.ColumnIndex == 6 && dataGridView4_2.Rows[e.RowIndex].Cells[6].Value.ToString() == "Y")
            {
                dataGridView4_2.Rows[e.RowIndex].Cells[6].Value = "N";
            }
        }
        

        private void button15_Click(object sender, EventArgs e) //월차/반차 반려 버튼
        {
            string file_vac_okay = "vac_Okay.txt";
            string file_vac_req = "vac_req.txt";
            List<string> newVacReqLines = new List<string>();
            for (int i = dataGridView4_5.Rows.Count - 1; i >= 0; i--)
            {
                string[] lineVacokay = File.ReadAllLines(file_vac_okay);
                if (Convert.ToBoolean(dataGridView4_5.Rows[i].Cells[5].Value))
                {
                    string Line = $"{dataGridView4_5.Rows[i].Cells[0].Value}\t{dataGridView4_5.Rows[i].Cells[1].Value}\t{dataGridView4_5.Rows[i].Cells[2].Value}\t{dataGridView4_5.Rows[i].Cells[3].Value}\t{dataGridView4_5.Rows[i].Cells[4].Value}";
                    for (int j = 1; j < lineVacokay.Length; j++)
                    {
                        string[] check_vac = lineVacokay[j].Split('\t');
                        // vac_req 파일에서 해당 행 삭제
                        string[] lines = File.ReadAllLines(file_vac_req);
                        List<string> newLines = new List<string>();

                        foreach (string vacLine in lines)
                        {
                            string[] splitLine = vacLine.Split('\t');
                            if (splitLine[0] != check_vac[0] || splitLine[4] != check_vac[4])
                            {
                                newLines.Add(vacLine);
                            }
                        }

                        File.WriteAllLines(file_vac_req, newLines);

                        dataGridView4_5.Rows.RemoveAt(i);
                        string[] linz = File.ReadAllLines("info.txt");
                        List<string> newLinz = new List<string>();

                        foreach (string line in linz)
                        {
                            string[] splitLine = line.Split('\t');
                            if (splitLine[1] == check_vac[1]) // 주민번호를 기준으로 찾음
                            {
                                // 기존 monthrent 값을 가져옴
                                int monthRent = int.Parse(splitLine[11]);
                                if (check_vac[2] == "월차")
                                {
                                    monthRent += 2;
                                }
                                if (check_vac[2] == "반차")
                                {
                                    monthRent++;
                                }
                                splitLine[11] = monthRent.ToString();

                                // 변경된 줄을 다시 조합하여 리스트에 추가
                                newLinz.Add(string.Join("\t", splitLine));
                            }
                            else
                            {
                                // 변경이 필요 없는 경우 그대로 유지
                                newLinz.Add(line);
                            }
                        }

                        // 변경된 내용을 파일에 씀
                        File.WriteAllLines("info.txt", newLinz);
                        break;
                    }
                }
            }
        }

        private bool allselected3 = false;



        private void dataGridView4_6_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridView4_6.RowCount > 0 && e.ColumnIndex == 3)
            {
                allselected3 = !allselected3;
                for (int i = 0; i < dataGridView4_6.RowCount; i++)
                {
                    dataGridView4_6.Rows[i].Cells[3].Value = allselected3;
                }
            }
        }

        // ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ 관리자 이벤트
    }
}



class Patient
{

    public static List<string> text_ss()
    {
        string P_path = "Reservation.txt";      // 환자 정보 텍스트 파일
        string[] lines = File.ReadAllLines(P_path);       // 파일의 모든 줄을 읽어옴
        List<string> linesList = lines.ToList();
        return linesList;
    }


    public static List<string> Att_cal()
    {
        string schedule = "schedule.txt";
        string[] sch = File.ReadAllLines(schedule);
        List<string> schcel = sch.ToList();
        return schcel;
    }
    public static List<List<string>> Att()
    {
        List<string> AttData = new List<string>();
        List<List<string>> Att_list = new List<List<string>>();
        List<string> linez = Patient.Att_cal();
        for (int i = 0; i < linez.Count; i++)          // 파일 내용을 한 줄씩 읽어가며 처리
        {
            AttData = linez[i].Split('\t').ToList();
            Att_list.Add(AttData); // 각 환자 데이터 리스트를 전체 리스트에 추가
        }
        return Att_list;
    }

    public static List<List<string>> Line()
    {
        List<List<string>> patient_list = new List<List<string>>(); // 각 줄의 데이터를 저장한 리스트를 저장할 리스트
        List<string> lines = Patient.text_ss();
        for (int i = 0; i < lines.Count; i++)          // 파일 내용을 한 줄씩 읽어가며 처리
        {
            List<string> patientData = lines[i].Split('\t').ToList();
            patient_list.Add(patientData); // 각 환자 데이터 리스트를 전체 리스트에 추가
        }
        return patient_list;
    }


    public static void Pnew()
    {
        string text_name = Form_login.form_main.textBox4.Text;
        string text_ssnum = Form_login.form_main.textBox5.Text;
        string text_phone = Form_login.form_main.textBox6.Text;
        string text_address = Form_login.form_main.textBox7.Text;

        // 하나라도 비어있으면 데이터를 파일에 추가하지 않음
        if (string.IsNullOrEmpty(text_name) || string.IsNullOrEmpty(text_ssnum) || string.IsNullOrEmpty(text_phone) || string.IsNullOrEmpty(text_address))
        {
            Form_login.form_main.label10.Visible = true;
            return;
        }

        // 전화번호 형식 변환
        if (text_phone.Length == 11) // 입력된 전화번호가 11자리인 경우에만 변환
        {
            string formatted_phone = $"{text_phone.Substring(0, 3)}-{text_phone.Substring(3, 4)}-{text_phone.Substring(7)}";
            text_phone = formatted_phone;
        }

        DateTime Pnew_age;

        // 6자리의 연도를 받아와서 yyyy-MM-dd 형태로 변경
        string yearString = text_ssnum.Substring(0, 2);
        int yearPrefix = int.Parse(yearString);
        int currentYear = DateTime.Now.Year % 100;
        string fullYearString = yearPrefix > currentYear ? "19" + yearString : "20" + yearString;

        // 나머지 날짜 정보를 가져와서 날짜로 변경
        string dateString = fullYearString + text_ssnum.Substring(2);
        if (!DateTime.TryParseExact(dateString, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out Pnew_age))
        {
            Form_login.form_main.label10.Visible = true;
            return;
        }
        string formatted_ssnum = Pnew_age.ToString("yyyy-MM-dd");

        DateTime today = DateTime.Now;

        int age = today.Year - Pnew_age.Year + 1;
        string text_age = age < 10 ? "0" + age.ToString() : age.ToString();

        string Pnew_info = $"{text_name}\t{text_age}\t{formatted_ssnum}\t{text_phone}\t{text_address}";

        string patient = "Patient.txt";
        string ppp = "PPP.txt";

        if (File.ReadAllText(patient).Contains(Pnew_info))
        {
            MessageBox.Show("등록된 환자입니다.");
            return;
        }

        // 파일에 추가
        File.AppendAllText(patient, Pnew_info + Environment.NewLine);
        File.AppendAllText(ppp, Pnew_info + "\t" + "X" +"\t" + "X" + "\t" + "X" + "\t" + "X" + Environment.NewLine);

    }



    public static void Psearch()
    {
        try
        {
            string PatientFilePath = "Patient.txt"; // 환자 정보 파일 경로
            string P_search = Form_login.form_main.main_textBox.Text; // 환자 검색창에서 입력된 이름
            string[] lines = File.ReadAllLines(PatientFilePath); // 파일의 모든 줄 읽기

            // 데이터그리드뷰 초기화
            Form_login.form_main.dataGridView2.Rows.Clear();

            int maxColumns = 0; // 최대 열 수 초기화

            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(P_search) || lines[i].Contains(P_search))
                {
                    string[] columns = lines[i].Split('\t'); // 탭으로 구분된 열을 분리

                    DataGridViewRow row = new DataGridViewRow();
                    for (int j = 0; j < columns.Length; j++)
                    {
                        DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                        cell.Value = columns[j];
                        row.Cells.Add(cell);
                    }
                    Form_login.form_main.dataGridView2.Rows.Add(row);

                    // 최대 열 수 업데이트
                    if (columns.Length > maxColumns)
                        maxColumns = columns.Length;

                }
            }
           
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error: " + ex.Message);
        }
    }

    public static void SaveDataToTextFile()
    {
        using (StreamWriter writer = new StreamWriter("Patient.txt"))
        {
            writer.WriteLine("이름" + "\t" + "나이" + "\t" + "주민번호" + "\t" + "전화번호" + "\t" + "주소");

            foreach (DataGridViewRow row in Form_login.form_main.dataGridView2.Rows)
            {
                // DataGridView의 각 행에서 셀 값을 가져와서 텍스트 파일에 쓰기
                string line = string.Join("\t", row.Cells.Cast<DataGridViewCell>().Select(cell => cell.Value.ToString()));
                writer.WriteLine(line);
            }
        }
    }

    



}

//
//이윤서
//메모장 관리 클래스
public class MemoManager
{
    private const string DataFolderPath = "PatientData";

    public static void SaveMemo(string patientName, string memo)
    {
        string patientFilePath = Path.Combine(DataFolderPath, $"{patientName}.txt");

        if (!Directory.Exists(DataFolderPath))
        {
            Directory.CreateDirectory(DataFolderPath);
        }

        File.AppendAllText(patientFilePath,memo + Environment.NewLine);
    }

    public static List<string> GetMemos(string patientName)
    {
        string patientFilePath = Path.Combine(DataFolderPath, $"{patientName}.txt");

        if (File.Exists(patientFilePath))
        {
            return File.ReadAllLines(patientFilePath).ToList();
        }
        else
        {
            return new List<string>();
        }
    }
}



class Inventory
{
    private static bool Isnumber(string input)     // 숫자인지 확인
    {
        foreach (char c in input)
        {
            if (!char.IsDigit(c))
            {
                return false;
            }
        }
        return true;
    }
    public static List<string> text_inv()               // 제품
    {
        string I_path = "Inventory Manager.txt";      // 재품 정보 텍스트 파일
        List<string> lines = File.ReadAllLines(I_path).ToList();       // 파일의 모든 줄을 읽고 리스트화
        return lines;
    }
    public static void line_inv()         // 제품 검색
    {
        string Code = Form_login.form_main.textBox3_1_1.Text;           // 코드 검색
        string Name = Form_login.form_main.textBox3_1_2.Text;           // 이름 검색
        DataGridView Data1 = Form_login.form_main.dataGridView3_2_1;    // 수량 조절 데이터그리드
        DataGridView Data2 = Form_login.form_main.dataGridView3_3_1;    // 주문 데이터 그리드
        List<string> inventory_data = new List<string>(); // 각 줄의 데이터를 저장할 리스트 생성
        List<List<string>> inventory_list = new List<List<string>>(); // 각 줄의 데이터를 저장한 리스트를 저장할 리스트
        List<string> lines = Inventory.text_inv();
        for (int i = 1; i < lines.Count; i++)          // 파일 내용을 한 줄씩 읽어가며 처리
        {
            inventory_data = lines[i].Split('\t').ToList();      // Inventory Manager의 각 줄
            if (Code != "" && Name != "")           // 둘다 입력되어 있을때
            {
                if (inventory_data[0].Length >= Code.Length && inventory_data[0].Substring(0, Code.Length) == Code && inventory_data[1].Length >= Name.Length && inventory_data[1].Substring(0, Name.Length) == Name)
                {
                    Data1.Rows.Add(inventory_data[0], inventory_data[1], Int32.Parse(inventory_data[2]));
                    Data2.Rows.Add(inventory_data[1], Int32.Parse("0"));
                }
            }
            else if (Code != "" && Name == "")       // 코드에만 입력되어 있을때
            {
                if (inventory_data[0].Length >= Code.Length && inventory_data[0].Substring(0, Code.Length) == Code)
                {
                    Data1.Rows.Add(inventory_data[0], inventory_data[1], Int32.Parse(inventory_data[2]));
                    Data2.Rows.Add(inventory_data[1], Int32.Parse("0"));
                }
            }
            else if (Code == "" && Name != "")      // 이름에만 입력되어 있을때
            {
                if (inventory_data[1].Length >= Name.Length && inventory_data[1].Substring(0, Name.Length) == Name)
                {
                    Data1.Rows.Add(inventory_data[0], inventory_data[1], Int32.Parse(inventory_data[2]));
                    Data2.Rows.Add(inventory_data[1], Int32.Parse("0"));
                }
            }
            else
            {
                Data1.Rows.Add(inventory_data[0], inventory_data[1], Int32.Parse(inventory_data[2]));
                Data2.Rows.Add(inventory_data[1], Int32.Parse("0"));
            }
        }
        Data1.ClearSelection();
        Data2.ClearSelection();
    }


    public static async void use_inv()         // 사용버튼
    {
        DataGridView Data1 = Form_login.form_main.dataGridView3_2_1;                // 제품 데이터그리드
        System.Windows.Forms.TextBox Count = Form_login.form_main.textBox3_2_1;     // 수량입력창
        Label Error = Form_login.form_main.label3_2_2;
        List<string> inventory_data = new List<string>();                           // 각 줄의 데이터를 저장할 리스트 생성
        List<string> lines = Inventory.text_inv();
        string msg = "";
        if (Count.Text != "")
        {
            int x = 0;
            int y = 0;
            for (int i = 0; i < Data1.Rows.Count; i++)     // 체크박스가 선택되어있는 모든 행
            {
                if (Isnumber(Count.Text) && (Convert.ToBoolean(Data1.Rows[i].Cells[3].Value) == true))         // 수량입력창에 숫자가 오고 체크박스에 선택이 되어있으면
                {
                    x = Int32.Parse(Count.Text);                                        // x = 수량 입력
                    y = Int32.Parse(Data1.Rows[i].Cells[2].Value.ToString());           // y = 재고 수량
                    if (y >= x)         // 재고가 입력수량보다 크거나 같을경우
                    {
                        Data1.Rows[i].Cells[2].Value = y - x;       // 재고 - 입력한 값
                        msg += Data1.Rows[i].Cells[0].Value + "\t" + Data1.Rows[i].Cells[1].Value + "\t" + x + "개\n";
                    }
                    else                // 입력 수량이 재고보다 큰경우
                    {
                        Error.Text = "잘못된 값입니다.";
                    }
                }
                else if (!Isnumber(Count.Text))       // 숫자가 입력되지 않은경우
                {
                    Error.Text = "잘못된 값입니다.";
                }
            }

            if (Error.Text != "잘못된 값입니다.")
            {
                // Inventory Manager파일에 변화 적용
                int code = 0, name = 0, value = 0;
                for (int i = 0; i < lines.Count; i++)          // 파일 내용을 한 줄씩 읽어가며 처리
                {
                    inventory_data = lines[i].Split('\t').ToList();            // Inventory Manager텍스트 파일의 각 줄을 탭으로 구분
                    if (i == 0)                                                // 카테고리 행 일때
                    {
                        code = inventory_data.IndexOf("제품코드");                 // 코드의 열번호
                        name = inventory_data.IndexOf("제품명");               // 제품명의 열번호
                        value = inventory_data.IndexOf("재고");                // 재고의 열번호
                    }
                    else
                    {
                        if (Data1.Rows[Data1.CurrentRow.Index].Cells[0].Value.ToString() == inventory_data[code])    // 선택한 행의 제품코드와 Inventory Manager의 제품코드와 같다면
                        {
                            int result = Int32.Parse(inventory_data[value]);        // 현재 텍스트파일에 있는 재고값
                            result -= x;                                            // 재고값 - 사용값
                            lines[i] = lines[i].Replace(inventory_data[value], result.ToString()); // 재고 열 값 변경
                        }
                    }
                }
            }
            if (msg != "")
            {
                MessageBox.Show(msg + "사용했습니다.", "사용");
            }
            // 파일에 변경된 값을 적용
            File.WriteAllLines("Inventory Manager.txt", lines);
        }
        else        // 수량입력창이 비어있을경우
        {
            Error.Text = "잘못된 값입니다.";
        }
        await Task.Delay(1000);                     // 일정시간후 에러메시지 삭제
        Error.Text = null;
        Count.Focus();
    }
    public static void del_inv()                // 삭제
    {
        DataGridView Data1 = Form_login.form_main.dataGridView3_2_1;
        DataGridView Data2 = Form_login.form_main.dataGridView3_3_1;
        string file_a = "Inventory Manager.txt";
        List<string> list = File.ReadAllLines(file_a).ToList();
        List<string> header = new List<string>();       // 카테고리 행을 담을 리스트
        List<string> lines = new List<string>();        // 내용을 담을 리스트
        string msg = "";
        for (int i = 0; i < list.Count; i++)
        {
            // List<string> rows = list[i].Split('\t').ToList();
            if (i == 0)
            {
                header.Add(list[i]);
            }
            else
            {
                for (int j = Data1.RowCount - 1; j >= 0; j--)
                {
                    if (!Convert.ToBoolean(Data1.Rows[j].Cells[3].Value))
                    {
                        lines.Insert(0, Data1.Rows[j].Cells[0].Value + "\t" + Data1.Rows[j].Cells[1].Value + "\t" + Data1.Rows[j].Cells[2].Value);
                    }
                    else
                    {
                        msg += Data1.Rows[j].Cells[0].Value + "\t" + Data1.Rows[j].Cells[1].Value + "\t" + Data1.Rows[j].Cells[2].Value + "\n";
                    }
                }
                break;
            }
        }
        DialogResult result = MessageBox.Show(msg + "삭제하시겠습니까?", "삭제", MessageBoxButtons.YesNo);
        if (result == DialogResult.Yes)
        {
            Data1.Rows.Clear();
            Data2.Rows.Clear();
            File.WriteAllLines(file_a, header);
            File.AppendAllLines(file_a, lines);
            List<string> second = File.ReadAllLines(file_a).ToList();
            if (file_a.Length > 1)
            {
                for (int i = 1; i < second.Count; i++)
                {
                    List<string> data = second[i].Split('\t').ToList();
                    Data1.Rows.Add(data[0], data[1], data[2]);
                }
                for (int i = 1; i < second.Count; i++)
                {
                    List<string> data = second[i].Split('\t').ToList();
                    Data2.Rows.Add(data[1], "0");
                }
            }
            Form_login.form_main.button3_2_1.Visible = false;
            Form_login.form_main.button3_2_2.Visible = false;
            Form_login.form_main.textBox3_2_1.Visible = false;
            Form_login.form_main.label3_2_2.Visible = false;
            Form_login.form_main.panel10.Visible = false;
        }
        else
        {
            Form_login.form_main.button3_2_1.Visible = true;
            Form_login.form_main.button3_2_2.Visible = true;
            Form_login.form_main.textBox3_2_1.Visible = true;
            Form_login.form_main.label3_2_2.Visible = true;
            Form_login.form_main.panel10.Visible = true;
        }
    }
    public async static void app_inv()            // 결재버튼
    {
        DataGridView Data2 = Form_login.form_main.dataGridView3_3_1;
        string A_path = "approval.txt";
        List<string> app = File.ReadAllLines(A_path).ToList();
        List<string> lines = new List<string>();        // 제품
        string msg = "";
        if (app.Count == 0)
        {
            app.Add("제품명\t수량\t결재시간");
            File.WriteAllLines(A_path, app);
        }
        for (int i = 0; i < Data2.RowCount; i++)        // 장바구니에 있는 제품수 만큼 반복
        {
            if (Convert.ToBoolean(Data2.Rows[i].Cells[2].Value) == true && Data2.Rows[i].Cells[1].Value.ToString() != "0")        // 선택되어있고 수량이 0이 아닌 제품만
            {
                lines.Add(Data2.Rows[i].Cells[0].Value.ToString() + "\t" + Data2.Rows[i].Cells[1].Value.ToString() + "\t" + DateTime.Now.ToString("yyyy/MM/dd H:mm"));
                msg += Data2.Rows[i].Cells[0].Value.ToString() + "\t" + Data2.Rows[i].Cells[1].Value.ToString() + "개\n";
                File.AppendAllLines(A_path, lines);
                lines.Clear();
                Data2.Rows[i].Cells[1].Value = "0";
                Data2.Rows[i].Cells[2].Value = false;
            }
            else if (Convert.ToBoolean(Data2.Rows[i].Cells[2].Value) == true && Data2.Rows[i].Cells[1].Value.ToString() == "0")
            {
                Form_login.form_main.label3_3_2.Text = "수량이 0인 값이 있습니다.";
                await Task.Delay(1000);                     // 일정시간후 에러메시지 삭제
                Form_login.form_main.label3_3_2.Text = null;
                Form_login.form_main.textBox3_3_1.Focus();
            }
        }
        if (msg != "")
        {
            MessageBox.Show(msg + "주문이 완료되었습니다", "주문");
            Form_login.form_main.button3_3_1.Visible = false;
            Form_login.form_main.button3_3_2.Visible = false;
            Form_login.form_main.button3_3_3.Visible = false;
            Form_login.form_main.textBox3_3_1.Visible = false;
            Form_login.form_main.label3_3_2.Visible = false;
            Form_login.form_main.panel105.Visible = false;
            Form_login.form_main.label3_3_2.Text = null;
            Form_login.form_main.textBox3_3_1.Text = null;
            Form_login.form_main.textBox3_3_1.Focus();
        }
        
    }
}
class Manage
{
    public static void Management()             //직원 메모장 불러오기
    {
        try
        {
            string ManageFilePath = "Info.txt"; // 환자 정보 파일 경로
            string[] lines = File.ReadAllLines(ManageFilePath); // 파일의 모든 줄 읽기

            // 데이터그리드뷰 초기화
            Form_login.form_main.dataGridView4_1.Rows.Clear();

            for (int i = 0; i < lines.Length; i++)
            {
                string[] columns = lines[i].Split('\t'); // 탭으로 구분된 열을 분리
                if (i > 0 && columns[10] != "O")
                {
                    Form_login.form_main.dataGridView4_1.Rows.Add(columns[0], columns[12]);
                }
            }
            Form_login.form_main.dataGridView4_1.CurrentCell = null;
        }
        catch
        {

        }
    }
    public static void p_info(int RowIndex)             // 개인정보 함수
    {
        //Form_login form_Login = new Form_login(); 
        string path = "Info.txt";  // Info 파일 경로
        // AppDomain.CurrentDomain.BaseDirectory = 현재 실행중인 프로그램의 기본 디렉토리
        // C:/Users/301-08/Desktop/Project_4/Project_4/bin/Debug/
        DirectoryInfo debugFolder = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
        FileInfo[] files = debugFolder.GetFiles();      // AppDomain.CurrentDomain.BaseDirectory안에 있는 파일들을 불러오기
        int name = 0, birth = 0, address = 0, phone = 0, start = 0, email = 0, picture = 0; // 열번호를 저장할 변수
        try
        {
            List<string> lines = File.ReadAllLines(path).ToList();       // 파일의 모든 줄을 읽고 리스트화
            for (int i = 0; i < lines.Count; i++)          // 파일 내용을 한 줄씩 읽어가며 처리, lines.count는 info.txt의 행길이
            {
                List<string> rows = lines[i].Split('\t').ToList();    // lines[i]를 탭으로 나눠서 리스트에 배치, {"이름","주민번호",...}

                // textBox2.Text = Form_login.form_main.dataGridView4.SelectedRows[0].Cells[0].Value.ToString();
                //MessageBox.Show(rows[0].ToString());
                if (i == 0)                     // 카테고리 행
                {
                    name = rows.IndexOf("이름");
                    birth = rows.IndexOf("주민번호");
                    address = rows.IndexOf("주소");
                    phone = rows.IndexOf("전화번호");
                    start = rows.IndexOf("입사일");
                    email = rows.IndexOf("이메일");
                    picture = rows.IndexOf("사진");
                }
                else
                {
                    if (Form_login.form_main.dataGridView4_1.Rows[RowIndex].Cells[0].Value.ToString() == rows[0])       // datagridview의 이름열의 값과 info.txt의 이름열값과 같다면
                    {
                        Form_login.form_main.textBox4_1_1.Text = rows[name].ToString();
                        Form_login.form_main.textBox4_1_2.Text = rows[birth].ToString();
                        Form_login.form_main.textBox4_1_3.Text = rows[address].ToString();
                        Form_login.form_main.textBox4_1_4.Text = rows[phone].ToString();
                        Form_login.form_main.textBox4_1_5.Text = rows[start].ToString();
                        Form_login.form_main.textBox4_1_6.Text = rows[email].ToString();
                        for (int j = 0; j < files.Length; j++)          // debug폴더내에서 이미지 찾기
                        {
                            string file = files[j].ToString();      // file = 파일명.확장자
                            if (rows[picture] == file)
                            {
                                string debugFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, rows[picture]);    // 이미지 경로
                                System.Drawing.Image backgroundImage = System.Drawing.Image.FromFile(debugFolderPath);        // 이미지 불러오기
                                Form_login.form_main.panel4_1_1.BackgroundImage = backgroundImage;
                                Form_login.form_main.panel4_1_1.BackgroundImageLayout = ImageLayout.Stretch;
                                break;
                            }
                        }
                    }
                }
            }
        }
        catch { }
    }
    public static void p_salary(int RowIndex)               // 월급내역, 2년전까지 확인가능
    {
        DateTime today = DateTime.Now;
        Form_login.form_main.dataGridView4_2.Rows.Clear();
        string path = "Info.txt";  // Info 파일 경로
        string schedulepath = "schedule.txt";
        List<string> lines = File.ReadAllLines(path).ToList();
        List<string> linz = File.ReadAllLines(schedulepath).ToList();
        string salarypath = "salary.txt";
        List<string> salaries = new List<string>();
        foreach (string scheduleLine in linz)
        {
            string[] scheduleItems = scheduleLine.Split('\t'); // schedule 한 줄을 탭으로 분할하여 배열로 저장
            string date = scheduleItems[scheduleItems.Length - 1]; // 근무 날짜 가져오기

            foreach (string infoLine in lines)
            {
                string[] infoItems = infoLine.Split('\t'); // Info 한 줄을 탭으로 분할하여 배열로 저장

                // 필요한 정보 추출
                string Name = infoItems[0]; // 이름 가져오기
                string Bank = infoItems[4]; // 은행명 가져오기
                string accountNumber = infoItems[5]; // 계좌번호 가져오기
                string Names = Name; // 예금주는 이름으로 설정

                // Info 파일의 이름과 schedule 파일의 이름이 일치하고 근무 날짜가 같은 경우
                if (Name == scheduleItems[0] && date == scheduleItems[scheduleItems.Length - 1])
                {
                    string workingDays = scheduleItems[5]; // 근무일수 가져오기
                    string workingHours = scheduleItems[7]; // 근무시간 가져오기
                    string workdays = scheduleItems[8];

                    // salary 파일에 추가할 내용
                    string salaryLine = $"{workdays}\t{workingDays}\t{workingHours}\t{Bank}\t{accountNumber}\t{Names}";

                    salaries.Add(salaryLine); // salaries 리스트에 추가
                    break;
                }
            }
        }
        File.WriteAllLines(salarypath, salaries);
        List<string> salary_line = File.ReadAllLines(salarypath).ToList();
        int workday_count = 0, worktime = 0, bank = 0, account = 0, name = 0, workday = 0;
        for (int j = 0; j < salary_line.Count; j++)
        {
            List<string> rows = salary_line[j].Split('\t').ToList();
            if (j == 0)
            {
                bank = rows.IndexOf("은행");
                account = rows.IndexOf("계좌번호");
                name = rows.IndexOf("이름");
                workday_count = rows.IndexOf("근무일수");
                worktime = rows.IndexOf("누적근무시간");
                workday = rows.IndexOf("근무날짜");
            }
            else
            {
                string yearMonth = rows[workday].Substring(0, 7);
                if (rows[name] == Form_login.form_main.dataGridView4_1.Rows[RowIndex].Cells[0].Value.ToString() && yearMonth == today.ToString("yyyy-MM"))
                {
                    Form_login.form_main.dataGridView4_2.Rows.Add(yearMonth, rows[workday_count], rows[worktime], rows[bank], rows[account], rows[name], "N");
                }
            }
        }
    }
    public static void vac_check()
    {
        string vacation = "vac_req.txt";
        List<string> lines = File.ReadAllLines(vacation).ToList();
        int name = 0, check = 0, time = 0, identi = 0, etc = 0;
        for (int i = 0; i < lines.Count; i++)
        {
            List<string> rows = lines[i].Split('\t').ToList();
            if (i == 0)
            {
                time = rows.IndexOf("날짜");
                check = rows.IndexOf("월차/반차");
                name = rows.IndexOf("이름");
                identi = rows.IndexOf("주민번호");
                etc = rows.IndexOf("비고");
            }
            else
            {
                Form_login.form_main.dataGridView4_5.Rows.Add(rows[name], rows[identi], rows[check], rows[etc], rows[time]);
            }
        }
    }
    public static void m_app_inv()            // 결재 탭
    {
        DataGridView Data_app = Form_login.form_main.dataGridView4_6;
        DataGridView Data_done = Form_login.form_main.dataGridView4_7;
        Data_app.Rows.Clear();
        string path = "approval.txt";
        List<string> lines = File.ReadAllLines(path).ToList();
        string path_d = "approval_done.txt";
        List<string> lines_d = File.ReadAllLines(path_d).ToList();
        int name = 0, count = 0, time = 0;
        for (int i = 0; i < lines.Count; i++)
        {
            int temp = 0;
            List<string> rows = lines[i].Split('\t').ToList();
            if (i == 0)
            {
                name = rows.IndexOf("제품명");
                count = rows.IndexOf("수량");
                time = rows.IndexOf("결재시간");
            }
            else
            {
                if (Data_app.RowCount == 0)     // 데이터그리드에 아무것도 없으면
                {
                    Data_app.Rows.Add(rows[name], Int32.Parse(rows[count]), rows[time]);
                }
                else
                {
                    for (int j = 0; j < Data_app.RowCount; j++)     // 결재탭에 있는 제품수 만큼 반복
                    {
                        // 데이터그리드의 이름과 결재시간과 approval파일의 이름과 결재시간이 같으면 
                        if (rows[name] == Data_app.Rows[j].Cells[0].Value.ToString() && rows[time] == Data_app.Rows[j].Cells[2].Value.ToString())       // 이름과 결재시간이 같다면
                        {
                            int x = Int32.Parse(rows[count]);
                            Data_app.Rows[j].Cells[1].Value = x + Int32.Parse(Data_app.Rows[j].Cells[1].Value.ToString());      // 수량만 플러스
                            temp++;
                        }
                    }
                    if (temp == 0)
                    {
                        Data_app.Rows.Add(rows[name], Int32.Parse(rows[count]), rows[time]);
                    }
                }
            }
        }
        for (int i = 1; i < lines_d.Count; i++)        // 주문 완료 내역
        {
            List<string> rows = lines_d[i].Split('\t').ToList();
            Data_done.Rows.Add(rows[0], rows[1], rows[2]);
        }
    }
    public static void pay_inv()        // 주문 버튼
    {
        DataGridView Data_app = Form_login.form_main.dataGridView4_6;
        DataGridView Data_done = Form_login.form_main.dataGridView4_7;
        string file_a = "approval.txt";
        List<string> list_a = File.ReadAllLines(file_a).ToList();
        List<string> header_a = new List<string>();       // file_a의 카테고리 행을 담을 리스트
        List<string> lines_a = new List<string>();        // file_a의 내용을 담을 리스트
        string file_d = "approval_done.txt";            // 결제완료된 제품을 넣을 파일
        List<string> list_d = File.ReadAllLines(file_d).ToList();
        string file_inv = "Inventory Manager.txt";           // 결제완료된 제품의 수량을 기존의 수량에 플러스 하기 위해
        List<string> list_inv = File.ReadAllLines(file_inv).ToList();
        List<string> header_inv = new List<string>();
        List<string> lines_inv = new List<string>();
        int result = 0;
        for (int i = 0; i < list_a.Count; i++)
        {
            if (i == 0)
            {
                header_a.Add(list_a[i]);
            }
            else
            {
                for (int j = Data_app.RowCount - 1; j >= 0; j--)
                {
                    if (Convert.ToBoolean(Data_app.Rows[j].Cells[3].Value))
                    {
                        if (list_d.Count == 0)
                        {
                            list_d.Add("제품명\t수량\t주문시간");
                            File.WriteAllLines(file_d, list_d);
                        }
                        string done = Data_app.Rows[j].Cells[0].Value + "\t" + Data_app.Rows[j].Cells[1].Value + "\t" + DateTime.Now.ToString() + "\n";
                        File.AppendAllText(file_d, done);        // approval_done.txt 추가
                    }
                    else
                    {
                        lines_a.Insert(0, Data_app.Rows[j].Cells[0].Value + "\t" + Data_app.Rows[j].Cells[1].Value + "\t" + Data_app.Rows[j].Cells[2].Value);
                    }
                    header_inv.Clear();
                    lines_inv.Clear();
                    for (int k = 0; k < list_inv.Count; k++)
                    {
                        List<string> columns_inv = list_inv[k].Split('\t').ToList();
                        if (k == 0)
                        {
                            header_inv.Add(list_inv[k]);
                        }
                        else
                        {
                            if (Data_app.Rows[j].Cells[0].Value.ToString() == columns_inv[1]) // 데이터그리드의 제품명과 Inventory Manager의 제품명이 같다면
                            {
                                result = Int32.Parse(columns_inv[2]);
                                result += Int32.Parse(Data_app.Rows[j].Cells[1].Value.ToString());
                                list_inv[k] = list_inv[k].Replace(columns_inv[2], result.ToString());
                            }
                            lines_inv.Add(list_inv[k]);
                        }
                    }
                    Data_app.Rows.RemoveAt(j);       // 데이터그리드에서 삭제
                }
            }
        }
        File.WriteAllLines(file_a, header_a);
        File.AppendAllLines(file_a, lines_a);
        List<string> second = File.ReadAllLines(file_a).ToList();
        List<string> second_done = File.ReadAllLines(file_d).ToList();
        if (file_a.Length > 1)
        {
            for (int i = 1; i < second.Count; i++)
            {
                List<string> data = second[i].Split('\t').ToList();
                Data_app.Rows.Add(data[0], data[1], data[2]);
            }
        }
        if (file_d.Length > 1)
        {
            for (int i = 1; i < second_done.Count; i++)
            {
                List<string> data = second_done[i].Split('\t').ToList();
                Data_done.Rows.Add(data[0], data[1], data[2]);
            }
        }
        File.WriteAllLines(file_inv, header_inv);
        File.AppendAllLines(file_inv, lines_inv);
    }
    public static void cancel_inv()
    {
        DataGridView Data_app = Form_login.form_main.dataGridView4_6;
        string file_a = "approval.txt";
        List<string> list_a = File.ReadAllLines(file_a).ToList();
        List<string> header_a = new List<string>();       // file_a의 카테고리 행을 담을 리스트
        List<string> lines_a = new List<string>();        // file_a의 내용을 담을 리스트
        for (int i = 0; i < list_a.Count; i++)
        {
            if (i == 0)
            {
                header_a.Add(list_a[i]);
            }
            else
            {
                for (int j = Data_app.RowCount - 1; j >= 0; j--)
                {
                    if (!Convert.ToBoolean(Data_app.Rows[j].Cells[3].Value))
                    {
                        lines_a.Insert(0, Data_app.Rows[j].Cells[0].Value + "\t" + Data_app.Rows[j].Cells[1].Value + "\t" + Data_app.Rows[j].Cells[2].Value);
                    }
                    Data_app.Rows.RemoveAt(j);       // 데이터그리드에서 삭제
                }
            }
        }
        File.WriteAllLines(file_a, header_a);
        File.AppendAllLines(file_a, lines_a);
        List<string> second = File.ReadAllLines(file_a).ToList();
        if (file_a.Length > 1)
        {
            for (int i = 1; i < second.Count; i++)
            {
                List<string> data = second[i].Split('\t').ToList();
                Data_app.Rows.Add(data[0], data[1], data[2]);
            }
        }
    }
    public static void Att_mang(int RowIndex)
    {
        Form_login.form_main.dataGridView7.Rows.Clear();
        string schedule = "schedule.txt";
        List<string> lines = File.ReadAllLines(schedule).ToList();
        DateTime today = DateTime.Now;
        int date = 0, worktime = 0, workday = 0, name = 0, gowork_check = 0, outwork_check = 0;
        for (int i = 0; i < lines.Count; i++)
        {
            List<string> rows = lines[i].Split('\t').ToList();
            if (i == 0)
            {
                date = rows.IndexOf("근무날짜");
                gowork_check = rows.IndexOf("출근확인");
                outwork_check = rows.IndexOf("퇴근확인");
                workday = rows.IndexOf("근무일수");
                name = rows.IndexOf("이름");
                worktime = rows.IndexOf("근무시간");
            }
            else
            {
                string yearMonth = rows[date].Substring(0, 7); // 문자열의 처음부터 7번째 문자까지를 가져옴
                if (rows[name] == Form_login.form_main.dataGridView4_1.Rows[RowIndex].Cells[0].Value.ToString() && yearMonth == today.ToString("yyyy-MM"))
                {
                    Form_login.form_main.dataGridView7.Rows.Add(rows[date], rows[gowork_check], rows[outwork_check], rows[worktime], rows[workday]);
                }

            }
        }
    }
    public static void vac_check_mang()
    {
        Form_login.form_main.dataGridView8.Rows.Clear();
        string Vac_okay_check = "vac_okay.txt";
        List<string> lines = File.ReadAllLines(Vac_okay_check).ToList();
        int name = 0, check = 0, time = 0, identi = 0, etc = 0;
        for (int i = 0; i < lines.Count; i++)
        {
            List<string> rows = lines[i].Split('\t').ToList();
            if (i == 0)
            {
                time = rows.IndexOf("날짜");
                check = rows.IndexOf("월차/반차");
                name = rows.IndexOf("이름");
                identi = rows.IndexOf("주민번호");
                etc = rows.IndexOf("비고");
            }
            else
            {
                Form_login.form_main.dataGridView8.Rows.Add(rows[name], rows[identi], rows[check], rows[etc], rows[time]);
            }
        }
    }
}