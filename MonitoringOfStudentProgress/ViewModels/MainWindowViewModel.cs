using MonitoringOfStudentProgress.Models;
using MonitoringOfStudentProgress.Views;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reactive;
using System.Text;

namespace MonitoringOfStudentProgress.ViewModels {
    public class MainWindowViewModel: ViewModelBase {
        // private TextBox studentName;
        // private ComboBox cb_1, cb_2, cb_3, cb_4;
        //private readonly MainWindow mw;
        public static MainWindowViewModel? inst;
        private readonly int[] cb_data = { -1, -1, -1, -1 };

        //private ForcePropertyChange force = new ForcePropertyChange();



        private String cb_avg = "NaN", status = "Есть пустые КС", student_name = "";
        private bool active_add = false, active_grind = false;
        private string last_student_name = "";
        public String CbAvg {
            get => cb_avg;
            set => this.RaiseAndSetIfChanged(ref cb_avg, value);
        }
        public String Status {
            get => status;
            set => this.RaiseAndSetIfChanged(ref status, value);
        }
        public String StudentName {
            get => student_name;
            set { student_name = value; Cb_calc(); }
        }
        public bool ActiveAdd {
            get => active_add;
            set => this.RaiseAndSetIfChanged(ref active_add, value);
        }
        public bool ActiveGrind {
            get => active_grind;
            set => this.RaiseAndSetIfChanged(ref active_grind, value);
        }

        private void Cb_calc() {
            int S = 0;
            foreach (int i in cb_data) {
                if (i == -1) {
                    CbAvg = "NaN";
                    Status = "Есть пустые КС";
                    ActiveAdd = false;
                    ActiveGrind = false;
                    return;
                }
                S += i;
            }
            CbAvg = String.Format("{0:F2}", (float) S / 4);
            String name = NormalizeStr.NormalizeTitleStr(student_name);
            if (name.Split(" ").Length != 3) {
                Status = "Невалидное ФИО";
                ActiveAdd = false;
                ActiveGrind = false;
                return;
            }
            int new_pos = FindPosList(name);
            bool repeat_fio = new_pos == -1;
            Status = repeat_fio ? "Такое ФИО уже есть" : new_pos + " | " + name;
            ActiveAdd = !repeat_fio;
            ActiveGrind = repeat_fio;
            last_student_name = name;
        }



        private readonly List<Student> studentList = new() {
            new Student("А А А", new int[] {2, 0, 1, 1}),
            new Student("В В В", new int[] {1, 2, 0, 2}),
            new Student("И И И", new int[] {1, 2, 0, 2})
        };
        public Student[] StudentList {
            get => studentList.ToArray();
            set {
                List<Student> lol = new();
                this.RaiseAndSetIfChanged(ref lol, studentList);
                // Заставляю силой обновить список после каждого добавления/удаления студента...
                // Даже force.UpdProperty("studentList") не помог, только через RaiseAndSetIfChanged :///
            }
        }
        private void Upd_stud_list() {
            //force.UpdProperty("studentList");
            StudentList = Array.Empty<Student>();
            Cb_calc();
            GlobalUpdate();
        }
        private int FindPosList(String name, bool eq = false) {
            int L = 0, R = studentList.Count;
            while (L < R) {
                int M = (L + R) / 2;
                int cmp = string.Compare(name, studentList[M].Name, StringComparison.Ordinal);
                if (cmp == 0) return eq ? M : -1;
                if (cmp < 0) R = M;
                else L = M + 1;
            }
            return eq ? -1 : L;
        }



        private string glob_1 = "", glob_2 = "", glob_3 = "", glob_4 = "", glob_avg = "";
        public string GlobA { get => glob_1; set => this.RaiseAndSetIfChanged(ref glob_1, value); }
        public string GlobB { get => glob_2; set => this.RaiseAndSetIfChanged(ref glob_2, value); }
        public string GlobC { get => glob_3; set => this.RaiseAndSetIfChanged(ref glob_3, value); }
        public string GlobD { get => glob_4; set => this.RaiseAndSetIfChanged(ref glob_4, value); }
        public string GlobAVG { get => glob_avg; set => this.RaiseAndSetIfChanged(ref glob_avg, value); }
        public void GlobalUpdate() {
            int a = 0, b = 0, c = 0, d = 0, L = studentList.Count;
            if (L == 0) {
                GlobA = GlobB = GlobC = GlobD = GlobAVG = "NaN";
                ColorA = ColorB = ColorC = ColorD = ColorAVG = "Aqua";
                return;
            }
            foreach (Student student in studentList) {
                a += student.ScoreA;
                b += student.ScoreB;
                c += student.ScoreC;
                d += student.ScoreD;
            }
            float a_val = (float) a / L;
            float b_val = (float) b / L;
            float c_val = (float) c / L;
            float d_val = (float) d / L;
            float avg_val = (float) (a + b + c + d) / (4 * L);

            GlobA = String.Format("{0:F3}", a_val);
            GlobB = String.Format("{0:F3}", b_val);
            GlobC = String.Format("{0:F3}", c_val);
            GlobD = String.Format("{0:F3}", d_val);
            GlobAVG = String.Format("{0:F4}", avg_val);

            ColorA = a_val < 1 ? "Red" : a_val < 1.5 ? "Yellow" : "Green";
            ColorB = b_val < 1 ? "Red" : b_val < 1.5 ? "Yellow" : "Green";
            ColorC = c_val < 1 ? "Red" : c_val < 1.5 ? "Yellow" : "Green";
            ColorD = d_val < 1 ? "Red" : d_val < 1.5 ? "Yellow" : "Green";
            ColorAVG = avg_val < 1 ? "Red" : avg_val < 1.5 ? "Yellow" : "Green";
        }



        private string color_1 = "", color_2 = "", color_3 = "", color_4 = "", color_avg = "";
        public string ColorA { get => color_1; set => this.RaiseAndSetIfChanged(ref color_1, value); }
        public string ColorB { get => color_2; set => this.RaiseAndSetIfChanged(ref color_2, value); }
        public string ColorC { get => color_3; set => this.RaiseAndSetIfChanged(ref color_3, value); }
        public string ColorD { get => color_4; set => this.RaiseAndSetIfChanged(ref color_4, value); }
        public string ColorAVG { get => color_avg; set => this.RaiseAndSetIfChanged(ref color_avg, value); }



        public int Cb_1 {
            get => cb_data[0] + 1;
            set { cb_data[0] = value - 1; Cb_calc(); }
        }
        public int Cb_2 {
            get => cb_data[1] + 1;
            set { cb_data[1] = value - 1; Cb_calc(); }
        }
        public int Cb_3 {
            get => cb_data[2] + 1;
            set { cb_data[2] = value - 1; Cb_calc(); }
        }
        public int Cb_4 {
            get => cb_data[3] + 1;
            set { cb_data[3] = value - 1; Cb_calc(); }
        }



        private void FuncAddStudent() {
            int pos = FindPosList(last_student_name);
            if (pos == -1) return;
            studentList.Insert(pos, new Student(last_student_name, cb_data));
            Upd_stud_list();
        }
        private void FuncGrindStudent() {
            int pos = FindPosList(last_student_name, true);
            if (pos == -1) return;
            studentList.RemoveAt(pos);
            Upd_stud_list();
        }



        public static byte[] Compress(byte[] input) {
            using var result = new MemoryStream();
            var lengthBytes = BitConverter.GetBytes(input.Length);
            result.Write(lengthBytes, 0, 4);
            using (var compressionStream = new GZipStream(result, CompressionMode.Compress)) {
                compressionStream.Write(input, 0, input.Length);
                compressionStream.Flush();
            }
            return result.ToArray();
        }
        public static byte[] Decompress(byte[] input) {
            using var source = new MemoryStream(input);
            byte[] lengthBytes = new byte[4];
            source.Read(lengthBytes, 0, 4);
            var length = BitConverter.ToInt32(lengthBytes, 0);
            using var decompressionStream = new GZipStream(source, CompressionMode.Decompress);
            var result = new byte[length];
            decompressionStream.Read(result, 0, length);
            return result;
        }
        String base_path = "../../../../MainStudBase.asd"; // Папка с решением lab2.
        public void Set_debug_mode() {
            base_path = "../../../../TestStudBase.asd";
        }
        private void FuncExport() {
            StringBuilder sb = new();
            bool first = true;
            foreach (Student student in studentList) {
                if (first) first = false;
                else sb.Append('\n');
                sb.Append(student.Pack());
            }
            byte[] encoded = Encoding.UTF8.GetBytes(sb.ToString());
            byte[] compressed = Compress(encoded);
            File.WriteAllBytes(base_path, compressed);
            Status = "База удачно сохранена";
        }
        private void FuncImport() {
            if (!File.Exists(base_path)) {
                Status = "Файл базы не найден";
                return;
            }
            byte[] compressed = File.ReadAllBytes(base_path);
            byte[] decompressed = Decompress(compressed);
            String data = Encoding.UTF8.GetString(decompressed);

            studentList.Clear();
            foreach (String pack in data.Split("\n")) studentList.Add(new Student(pack));
            Upd_stud_list();
            Status = "База удачно загружена";
        }


        public MainWindowViewModel(MainWindow _mw) {
            // mw = _mw;
            // studentName = mw.Find<TextBox>("studentName");
            // cb_1 = mw.Find<ComboBox>("cb_1");
            // cb_2 = mw.Find<ComboBox>("cb_2");
            // cb_3 = mw.Find<ComboBox>("cb_3");
            // cb_4 = mw.Find<ComboBox>("cb_4");
            // studentList = mw.Find<ListBox>("studentList");
            // cbAvg = (TextBlock) mw.Find<TextBlock>("cbAvg");

            inst = this;
            GlobalUpdate();

            AddStudent = ReactiveCommand.Create<Unit, Unit>(_ => { FuncAddStudent(); return new Unit(); });
            GrindStudent = ReactiveCommand.Create<Unit, Unit>(_ => { FuncGrindStudent(); return new Unit(); });
            Export = ReactiveCommand.Create<Unit, Unit>(_ => { FuncExport(); return new Unit(); });
            Import = ReactiveCommand.Create<Unit, Unit>(_ => { FuncImport(); return new Unit(); });
        }
        public ReactiveCommand<Unit, Unit> AddStudent { get; }
        // Хотел назвать метод DestroyBurnCutGrindAnnihilatePulverizeRuinExterminateStudent,
        // но половина из них применима к технической обработке сырья, а не студентов.
        // Шутка про длинное название метода, хотя всё это синонимы одного и того же слова Remove с разной степенью масштаба...
        public ReactiveCommand<Unit, Unit> GrindStudent { get; }
        public ReactiveCommand<Unit, Unit> Export { get; }
        public ReactiveCommand<Unit, Unit> Import { get; }
    }
}
