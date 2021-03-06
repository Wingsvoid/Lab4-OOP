﻿using System;
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
    class Human : MapObject
    {
        private PointLatLng point;
        private PointLatLng destinationPoint;
        public event EventHandler Seated;
        private bool inTaxi;
        private GMapMarker marker;

        public PointLatLng Destination
        {
            set { this.destinationPoint = value; }
        }
        

        public Human(string title, PointLatLng point) : base(title)
        {
            this.point = point;
            this.inTaxi = false;
            marker = new GMapMarker(point)
            {
                Shape = new Image
                {
                    Width = 42, // ширина маркера
                    Height = 42, // высота маркера
                    ToolTip = this.getTitle(), // всплывающая подсказка
                    Source = new BitmapImage(new Uri("pack://application:,,,/imgs/human.png")), // картинка
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
        public void moveTo(PointLatLng point)
        {
            this.point = point;
        }

        public void carArrived(object sender, EventArgs e)
        {
            if (sender is Car taxi && !destinationPoint.IsEmpty)
            {
                if (inTaxi)
                {
                    inTaxi = false;
                    destinationPoint = PointLatLng.Empty;
                }
                else
                {
                    inTaxi = true;
                    Seated?.Invoke(this, null);
                    taxi.moveTo(destinationPoint);
                }
            }
            //сесть в машину
        }
    }
}

