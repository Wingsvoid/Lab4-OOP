using System;
using System.Collections.Generic;
using System.Device.Location;
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
using GMap.NET;
using GMap.NET.WindowsPresentation;
namespace Lab3_OOP
{
    class Location : MapObject
    {
        private PointLatLng point;
        private GMapMarker marker;

        public Location(string title, PointLatLng point) : base(title)
        {
            this.point = point;
            marker = new GMapMarker(point)
            {
                Shape = new Image
                {
                    Width = 42, // ширина маркера
                    Height = 42, // высота маркера
                    ToolTip = this.getTitle(), // всплывающая подсказка
                    Source = new BitmapImage(new Uri("pack://application:,,,/imgs/location.png")), // картинка
                    RenderTransform = new TranslateTransform { X = -14, Y = -14 } // картинка
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
    }
}
