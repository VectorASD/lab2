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
            int S = 0;
            foreach (int i in scores) S += i;
            float avg = (float) S / 4;
            Avg = String.Format("{0:F2}", avg);

            var main = MainWindowViewModel.inst;
            if (main != null) main.GlobalUpdate();
        }



        private string name;
        public string Name {
            get => name;
            set => SetProperty(ref name, value);
        }



        private readonly int[] scores;
        public int ScoreA { get => scores[0]; set { SetProperty(ref scores[0], value); Update(); } }
        public int ScoreB { get => scores[1]; set { SetProperty(ref scores[1], value); Update(); } }
        public int ScoreC { get => scores[2]; set { SetProperty(ref scores[2], value); Update(); } }
        public int ScoreD { get => scores[3]; set { SetProperty(ref scores[3], value); Update(); } }



        private string avg = "";
        public string Avg { get => avg; private set => SetProperty(ref avg, value); }



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
