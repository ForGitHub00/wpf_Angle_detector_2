using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace wpf_Angle_detector_2 {
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            Data = new List<MyPoint>();
            /*
            Rectangle rec = new Rectangle() {
                Height = 50,
                Width = 70,
                Fill = new SolidColorBrush(Colors.Red)
            };
            Canvas.SetLeft(rec, 50);
            Canvas.SetTop(rec, 90);
            cnv.Children.Add(rec);
            */
            GetDataFromFile();
            Angle();
           // Data.Sort((s1,s2) => s1.X.CompareTo(s2.X));
            DrawData(Data);
            pol = new Polyline();
            slider.ValueChanged += Slider_ValueChanged;
        }



        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            lb_slider.Content = $"{slider.Value}";
            Dispatcher.Invoke(() => cnv.Children.Clear());
            Dispatcher.Invoke(() => DrawData(Data));
            //DrawData(Data);
            //DrawLines(Data, slider.Value);
            //DrawText();
            Dispatcher.Invoke(() => FindSpad3(Data, 1, 0));
        }

        public struct MyPoint {
            public double X;
            public double Z;
        }
        public List<MyPoint> Data;
        Polyline pol;
        public void GetDataFromFile() {
            Data.Clear();
            string path = "out6.txt";
            string line;
            string temp = "";
            int counter = 0;
            double tempX = 0;
            StreamReader file = new StreamReader(path);
            while ((line = file.ReadLine()) != null) {
                if (line != "") {
                    line = line.Substring(1);
                    //line = line.Substring(17);
                    line = line.Substring(3);
                    counter = 0;
                    temp = "";
                    while (line[counter] != ' ') {
                        temp += line[counter];
                        counter++;
                    }
                    temp = temp.Replace('.', ',');
                    tempX = Convert.ToDouble(temp);
                    line = line.Substring(counter);
                    counter = 0;
                    while (line[counter] == ' ') {
                        counter++;
                    }
                    line = line.Substring(counter + 4);
                    line = line.Replace('.', ',');
                    line = line.Remove(line.Length - 1, 1);
                    Data.Add(new MyPoint() {
                        X = tempX,
                        Z = Convert.ToDouble(line)
                    });
                }
            }
            /*
            foreach (var item in Data) {
                Console.WriteLine($"{item.X}    {item.Z}");
            }*/
        }
        public void DrawData(List<MyPoint> data) {
            Random rnd = new Random();
            int i = 0;
            foreach (var item in data) {
                Rectangle rec = new Rectangle() {
                    Height = 1,
                    Width = 1,
                    Fill = new SolidColorBrush(Color.FromRgb((byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255))),
                    Name = "C_" + i.ToString()
                };
                rec.MouseEnter += Rec_MouseEnter;
                Canvas.SetLeft(rec, i);
                //Canvas.SetTop(rec, (item.Z - 230) * 10);
                Canvas.SetTop(rec, item.Z);
                cnv.Children.Add(rec);
                i++;
            }
        }
        public void DrawLines(List<MyPoint> data, double raz) {
            Random rnd = new Random();

            Polyline pol = new Polyline() {
                Stroke = new SolidColorBrush(Colors.Red),
                StrokeThickness = 1,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
            };
            pol.Points = new PointCollection();



            int index = 0;
            for (int i = 1; i < data.Count; i++) {
                if (Math.Abs(data[i].Z - data[index].Z) > raz && data[i].Z != 0 && data[index].Z !=0) {
                    // pol.Points.Add(new Point(i, (data[i].Z - 230) * 10));
                    pol.Points.Add(new Point(i, data[i].Z));
                    index = i;
                }
            }

            //pol.Points.Add(new Point(data.Count - 1, (data[index].Z - 230) * 10));
           // pol.Points.Add(new Point(data.Count - 1, (data[data.Count - 1].Z - 230) * 10));

            pol.Points.Add(new Point(data.Count - 1, data[index].Z));
            pol.Points.Add(new Point(data.Count - 1, data[data.Count - 1].Z));

            for (int i = 1; i < pol.Points.Count; i++) {
                double radian = Math.Atan((pol.Points[i].X - pol.Points[i - 1].X) / (pol.Points[i].Y - pol.Points[i - 1].Y));
                double degris = radian * 360 / Math.PI;

                if (Math.Abs(degris) < 50) {
                    Ellipse r = new Ellipse() {
                        Height = 3,
                        Width = 3,
                        Fill = new SolidColorBrush(Colors.Blue),
                    };
                    Canvas.SetLeft(r, pol.Points[i].X);
                    //Canvas.SetTop(rec, (item.Z - 230) * 10);
                    Canvas.SetTop(r, pol.Points[i].Y);
                    cnv.Children.Add(r);
                }
                
                Console.WriteLine($"Count --- {pol.Points.Count} Degris ---- {degris}");
            }
            
            cnv.Children.Add(pol);
        }
        public void DrawText() {
            for (int i = 1; i < pol.Points.Count; i++) {
                double radian = Math.Atan((pol.Points[i].X - pol.Points[i - 1].X) / (pol.Points[i].Y - pol.Points[i - 1].Y));
                double degris = radian * 180 / Math.PI;

                Console.WriteLine($"Count --- {pol.Points.Count} Degris ---- {degris}");
            }
        }
        public void FindSpad(List<MyPoint> data, double raz) {
            int indexOfMax = 0;
            double maxValueZ = 0;

            maxValueZ = data.Max((s)=> s.Z);
            indexOfMax = data.FindIndex((s) => s.Z == maxValueZ);
            //Console.WriteLine($"Index = {indexOfMax}    Value = {maxValueZ}");
            /*
            int left = 0;
            int right = data.Count - 1;
            int index = 0;
            while (left < right) {
                while (data[left].Z == 0 && left < right - 1) {
                    left++;
                }
                while (data[right].Z == 0 && right - 1 > left) {
                    right--;
                }
                if (Math.Abs(data[left].Z - data[right].Z) >= raz) {
                    index = left;
                    break;
                }
                left++;
                right--;
            }
            Console.WriteLine($"Raz = {raz}    Index = {index}");
            */

            double sred = Math.Abs(data[0].Z - data[1].Z);
            double temp;
            int tempIndex;
            for (int i = 2; i < data.Count - 1; i++) {
                tempIndex = i + 1;
                while (data[tempIndex].Z == 0 && tempIndex < data.Count - 2) {
                    tempIndex++;
                }

                temp = Math.Abs(data[i].Z - data[tempIndex].Z);
                if (temp <= sred * 8) {
                    sred *= i - 1;
                    sred += temp;
                    sred /= i;
                }                
                else {
                    Console.WriteLine($"Sred = {sred} Index = {i} Z1 = {data[i].Z} Z2 = {data[tempIndex].Z} Raz = {temp}");
                }
                i = tempIndex - 1;
            }
            Console.WriteLine($"________________________________");

            for (int i = 0; i < data.Count - 1; i++) {
                tempIndex = i + 1;
                while (data[tempIndex].Z == 0 && tempIndex < data.Count - 2) {
                    tempIndex++;
                }

                temp = Math.Abs(data[i].Z - data[tempIndex].Z);
                if (temp >= sred * 19) {
                    Console.WriteLine($"Sred = {sred} Index = {i} Z1 = {data[i].Z} Z2 = {data[tempIndex].Z} Raz = {temp}");
                }
                i = tempIndex - 1;
            }
            Console.WriteLine($"***************************");
        }
        public void FindSpad2(List<MyPoint> data, double raz) {
            int indexL = 0;
            int indexR = 0;
            double z1 = 0;
            double z2 = 0;
            double maxRazn = 0;

            int tempIndex = 0;
            double temp = 0;
            for (int i = 0; i < data.Count - 1; i++) {
                tempIndex = i + 1;
                while (data[tempIndex].Z == 0 && tempIndex < data.Count - 2) {
                    tempIndex++;
                }

                temp = Math.Abs(data[i].Z - data[tempIndex].Z);
                if (temp > maxRazn) {
                    maxRazn = temp;
                    indexL = i;
                    indexR = tempIndex;
                    z1 = data[i].Z;
                    z2 = data[tempIndex].Z;
                    
                }
                i = tempIndex - 1;
            }
            Console.WriteLine($"Raz = {maxRazn} Left = {indexL} Z1 = {z1} Right = {indexR} Z2 = {z2}");
            Console.WriteLine($"Distance of Centre  Left= { Math.Abs(data[640].X - data[indexL].X)}");
            Console.WriteLine($"Distance of Centre  Right= { Math.Abs(data[640].X - data[indexR].X)}");
            Console.WriteLine($"\n");

            Line l1 = new Line() {
                Stroke = new SolidColorBrush(Colors.Red),
                StrokeThickness = 1,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
            };

            l1.X1 = 0;
            l1.Y1 = data[0].Z;
            l1.X2 = indexL;
            l1.Y2 = data[indexL].Z;

            Line l2 = new Line() {
                Stroke = new SolidColorBrush(Colors.Red),
                StrokeThickness = 1,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
            };

            l2.X1 = indexR;
            l2.Y1 = data[indexR].Z;
            l2.X2 = data.Count - 1;
            l2.Y2 = data[data.Count - 1].Z;

            cnv.Children.Add(l1);
            cnv.Children.Add(l2);
        }
        public double FindSpad3(List<MyPoint> data, int direction, int topBot) {
            int indexL = 0;
            int indexR = 0;
            double z1 = 0;
            double z2 = 0;
            double maxRazn = 0;

            int tempIndex = 0;
            double temp = 0;

            int startIndex = 0;
            int finishIndex = 0;

            if (direction == 0) {
                startIndex = 0;
                finishIndex = data.Count / 2;
            }
            else {
                startIndex = data.Count / 2;
                finishIndex = data.Count;
            }

            for (int i = startIndex; i < finishIndex - 1; i++) {
                tempIndex = i + 1;
                while (data[tempIndex].Z == 0 && tempIndex < data.Count - 2) {
                    tempIndex++;
                }

                temp = Math.Abs(data[i].Z - data[tempIndex].Z);
                if (temp > maxRazn) {
                    maxRazn = temp;
                    indexL = i;
                    indexR = tempIndex;
                    z1 = data[i].Z;
                    z2 = data[tempIndex].Z;
                }
                i = tempIndex - 1;
            }

            Console.WriteLine($"Raz = {maxRazn} Left = {indexL} Z1 = {z1} Right = {indexR} Z2 = {z2}");

            int resultIndex = 0;
            if (data[indexL].Z > data[indexR].Z) {
                if (topBot == 0)  resultIndex = indexR;
                else resultIndex = indexL;               
            }
            else {
                if (topBot == 0)  resultIndex = indexL;
                else resultIndex = indexR;       
            }

            double result = Math.Abs(data[640].X - data[resultIndex].X);
            return result;
        }



        public void Angle() {

            for (int i = 0; i < Data.Count; i++) {
                if (Data[i].Z != 0) {
                    MyPoint temp = new MyPoint() {
                        X = Data[i].X,
                        Z = Data[i].Z + i / 3
                    };
                    Data[i] = temp;
                }
            }
        }

        private void Rec_MouseEnter(object sender, MouseEventArgs e) {
            Rectangle rec = sender as Rectangle;
            int index = Convert.ToInt32(rec.Name.Substring(2));
            lb_name.Content = rec.Name;
            lb_z.Content = Data[index].Z.ToString();
            lb_x.Content = Data[index].X.ToString();
        }

        const double ScaleRate = 1.1;
        private void cnv_MouseWheel(object sender, MouseWheelEventArgs e) {
            if (e.Delta > 0) {
                st.ScaleX *= ScaleRate;
                st.ScaleY *= ScaleRate;
            }
            else {
                st.ScaleX /= ScaleRate;
                st.ScaleY /= ScaleRate;
            }
            e.Handled = true;
        }

        private void scrl_PreviewMouseUp(object sender, MouseButtonEventArgs e) {

        }
    }
}
