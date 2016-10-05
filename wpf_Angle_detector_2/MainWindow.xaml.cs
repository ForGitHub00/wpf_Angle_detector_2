using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            Data.Sort((s1,s2) => s1.X.CompareTo(s2.X));
            DrawData(Data);
            pol = new Polyline();
            slider.ValueChanged += Slider_ValueChanged;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            lb_slider.Content = $"{slider.Value}";
            cnv.Children.Clear();
            DrawData(Data);
            DrawLines(Data, slider.Value);
            DrawText();
        }

        public struct MyPoint {
            public double X;
            public double Z;
        }
        public List<MyPoint> Data;
        Polyline pol;
        public void GetDataFromFile() {
            Data.Clear();
            string path = "out.txt";
            string line;
            string temp = "";
            int counter = 0;
            double tempX = 0;
            StreamReader file = new StreamReader(path);
            while ((line = file.ReadLine()) != null) {
                if (line != "") {
                    line = line.Substring(1);
                    line = line.Substring(17);
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
                if (Math.Abs(data[i].Z - data[index].Z) > raz) {
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
            
            //cnv.Children.Add(pol);
        }
        public void DrawText() {
            for (int i = 1; i < pol.Points.Count; i++) {
                double radian = Math.Atan((pol.Points[i].X - pol.Points[i - 1].X) / (pol.Points[i].Y - pol.Points[i - 1].Y));
                double degris = radian * 180 / Math.PI;

                Console.WriteLine($"Count --- {pol.Points.Count} Degris ---- {degris}");
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
        }
    }
}
