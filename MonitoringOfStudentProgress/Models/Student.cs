using Avalonia.Media;
using MonitoringOfStudentProgress.ViewModels;
using ReactiveUI;
using System;
using System.ComponentModel;

namespace MonitoringOfStudentProgress.Models {
    public class Student: OnNotifyPropertyChanged {
        // OnNotifyPropertyChanged - Единтсвенный адекватный вариант, ибо даже RaiseAndSetIfChanged не работает,
        // хоть я и протащил экземпляр MainWindowViewModel в класс Student. Всё же SetProperty делает тоже самое,
        // что и RaiseAndSetIfChanged, только без требования экземпляра MainWindowViewModel, так ещё и работает!!!

        public Student(string fio, int[] arr) {
            name = fio;
            scores = (int[]) arr.Clone(); // целые 5 минут ломал голову чё за магия с global avg, а на деле клонирования не хватало ;'-}
            Update();
        }
        private void Update() {
            ColorA = scores[0] < 1 ? "Red" : scores[0] < 1.5 ? "Yellow" : "Green";
            ColorB = scores[1] < 1 ? "Red" : scores[1] < 1.5 ? "Yellow" : "Green";
            ColorC = scores[2] < 1 ? "Red" : scores[2] < 1.5 ? "Yellow" : "Green";
            ColorD = scores[3] < 1 ? "Red" : scores[3] < 1.5 ? "Yellow" : "Green";

            int S = 0;
            foreach (int i in scores) S += i;
            float avg = (float) S / 4;
            Avg = String.Format("{0:F2}", avg);

            ColorAVG = avg < 1 ? "Red" : avg < 1.5 ? "Yellow" : "Green";

            var main = MainWindowViewModel.inst;
            if (main != null) main.GlobalUpdate();
        }



        private string name;
        public string Name {
            get => name;
            set => SetProperty(ref name, value);
        }



        private readonly int[] scores;
        public int ScoreA { get => scores[0]; set { scores[0] = value; Update(); } }
        public int ScoreB { get => scores[1]; set { scores[1] = value; Update(); } }
        public int ScoreC { get => scores[2]; set { scores[2] = value; Update(); } }
        public int ScoreD { get => scores[3]; set { scores[3] = value; Update(); } }



        private string colorA = "", colorB = "", colorC = "", colorD = "";
        public string ColorA { get => colorA; set => SetProperty(ref colorA, value); }
        public string ColorB { get => colorB; set => SetProperty(ref colorB, value); }
        public string ColorC { get => colorC; set => SetProperty(ref colorC, value); }
        public string ColorD { get => colorD; set => SetProperty(ref colorD, value); }



        private string avg = "", colorAVG = "";
        public string Avg { get => avg; private set => SetProperty(ref avg, value); }
        public string ColorAVG { get => colorAVG; set => SetProperty(ref colorAVG, value); }



        public Student(string packed) { // Unpack
            String[] data = packed.Split("|");
            name = data[0];
            String[] s = data[1].Split(",");
            scores = new int[] { int.Parse(s[0]), int.Parse(s[1]), int.Parse(s[2]), int.Parse(s[3]) };
            Update();
        }
        public String Pack() {
            return string.Format("{0}|{1},{2},{3},{4}", name, scores[0], scores[1], scores[2], scores[3]);
        }
    }
}
