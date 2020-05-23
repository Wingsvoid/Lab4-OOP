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
using System.Windows.Shapes;
using GMap.NET;
using GMap.NET.WindowsPresentation;

namespace Lab3_OOP
{
    public class Route : MapObject
    {
        private List<PointLatLng> points;

        public Route(string title, List<PointLatLng> points) : base(title)
        {
            this.points = new List<PointLatLng>();

            foreach (PointLatLng p in points)
            {
                this.points.Add(p);
            }
        }
                     
        public override double getDistance(PointLatLng point)
        {
            double minDistance = double.MaxValue;
            foreach(PointLatLng tpoint in points)
            {
                GeoCoordinate c1 = new GeoCoordinate(tpoint.Lat, tpoint.Lng);
                GeoCoordinate c2 = new GeoCoordinate(point.Lat, point.Lng);

                // вычисление расстояния между точками в метрах
                double distance = c1.GetDistanceTo(c2);
                if (distance < minDistance)
                    minDistance = distance;
            }

            return minDistance;

        }

        public override PointLatLng getFocus()
        {
            return points[0];
        }

        public override GMapMarker getMarker()
        {
            GMapMarker marker = new GMapRoute(points)
            {
                Shape = new Path()
                {
                    Stroke = Brushes.DarkRed, // цвет обводки
                    Fill = Brushes.DarkRed, // цвет заливки
                    StrokeThickness = 4, // толщина обводки
                    ToolTip = this.getTitle()
                }
            };

            return marker;
        }
    }
}
