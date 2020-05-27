using System;
using System.Collections.Generic;
using System.Device.Location;
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
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;


namespace Lab3_OOP
{
    public class Car : MapObject
    {
        private PointLatLng point;
        private List<PointLatLng> route;
        private List<Human> passengers;
        public event EventHandler Arrived;
        private GMapMarker marker;

        public Car(string title, PointLatLng point) : base(title)
        {
            this.point = point;
            this.passengers = new List<Human>();
            //this.route = new List<PointLatLng>();
            marker = new GMapMarker(point)
            {
                Shape = new Image
                {
                    Width = 42, // ширина маркера
                    Height = 42, // высота маркера
                    ToolTip = this.getTitle(), // всплывающая подсказка
                    Source = new BitmapImage(new Uri("pack://application:,,,/imgs/car.png")), // картинка
                    RenderTransform = new TranslateTransform { X = -14, Y = -14 }
                }
            };
        }

        public override double getDistance(PointLatLng point)
        {
            // точки в формате System.Device.Location
            GeoCoordinate c1 = new GeoCoordinate(this.point.Lat, this.point.Lng);
            GeoCoordinate c2 = new GeoCoordinate(point.Lat, point.Lng);

            // вычисление расстояния между точками в метрах
            double distance = c1.GetDistanceTo(c2);
            return distance;
        }

        public override PointLatLng getFocus()
        {
            return point;
        }

        public override GMapMarker getMarker()
        {
            return marker;
        }

        public void moveTo(PointLatLng endPoint)
        {
            //построить маршрут до указанной точки и сохранить в route 
            // провайдер навигации
            RoutingProvider routingProvider = GMapProviders.OpenStreetMap;
            // определение маршрута
            MapRoute route = routingProvider.GetRoute(
             this.point, // начальная точка маршрута
             endPoint, // конечная точка маршрута
             false, // поиск по шоссе (false - включен)
             false, // режим пешехода (false - выключен)
             15);
            // получение точек маршрута
            this.route = route.Points;
            //начать движение
            Thread newThread = new Thread(new ThreadStart(moveByRoute));
            newThread.Start();
        }

        public void passengerSeated(object sender, EventArgs e)
        {
            //посадить пассажира в машину и начать движение по маршруту
            if (sender is Human passenger)
            {
                this.passengers.Add(passenger);
            }
        }

        private void moveByRoute()
        {
            foreach (var point in route)
            {
                // вычисление разницы между двумя соседними точками по широте и долготе
                double latDiff = point.Lat - this.point.Lat;
                double lngDiff = point.Lng - this.point.Lng;
                // вычисление угла направления движения
                //latDiff и lngDiff - катеты прямоугольного треугольника
                double angle = Math.Atan2(lngDiff, latDiff) * 180.0 / Math.PI;

                this.point = point;
                Application.Current.Dispatcher.Invoke(delegate
                {
                    // установка угла поворота маркера
                    TransformGroup transform = new TransformGroup();
                    transform.Children.Add(new RotateTransform { Angle = angle - 90.0, CenterX = 14, CenterY = 14 });
                    transform.Children.Add(new TranslateTransform { X = -14, Y = -14 });
                    MainWindow.marker_taxiCar.Shape.RenderTransform = transform;
                    //MainWindow.marker_taxiCar.Shape.RenderTransform = new RotateTransform { Angle = angle - 90.0, CenterX = 14, CenterY = 14 };
                    //MainWindow.marker_taxiCar.Shape.RenderTransform = new TranslateTransform { X = -14, Y = -14 };
                    MainWindow.marker_taxiCar.Position = this.point;      
                    MainWindow.map.Position = this.point; 
                    //добавить перемещение пассажиров
                    foreach (Human passenger in passengers)
                    {
                        passenger.moveTo(this.point);
                        MainWindow.marker_taxiClient.Position = passenger.getFocus();
                    }
                    
                });
                Thread.Sleep(250);
            }

            if (passengers.Count() > 0)
            {
                MessageBox.Show("Такси достигло пункта назначения");
            }

            else
            {
                MessageBox.Show("Пассажир сел в такси");
            }
          
            passengers.Clear();
            route.Clear();
            // отправка события о прибытии после достижения последней точки маршрута
            Arrived?.Invoke(this, null);
            
        }
    }
}
