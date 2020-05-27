using System;
using System.Collections.Generic;
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
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using System.Device.Location;

namespace Lab3_OOP
{
    public partial class MainWindow : Window
    {
        List<MapObject> objs = new List<MapObject>();
        List<MapObject> objsInList = new List<MapObject>();

        List<PointLatLng> pts = new List<PointLatLng>();

        Human taxiClient;
        Car taxiCar;
        Location taxiClientDestination;

        public MainWindow()
        {
            InitializeComponent();
            cb1.IsChecked = true;
        }

        private void MapLoaded(object sender, RoutedEventArgs e)
        {
            // настройка доступа к данным
            GMaps.Instance.Mode = AccessMode.ServerAndCache;

            // установка провайдера карт
            Map.MapProvider = OpenStreetMapProvider.Instance;

            // установка зума карты
            Map.MinZoom = 2;
            Map.MaxZoom = 17;
            Map.Zoom = 15;

            // установка фокуса карты
            Map.Position = new PointLatLng(55.012823, 82.950359);

            // настройка взаимодействия с картой
            Map.MouseWheelZoomType = MouseWheelZoomType.MousePositionAndCenter;
            Map.CanDragMap = true;
            Map.DragButton = MouseButton.Left;

            PointLatLng point = new PointLatLng(55.016511, 82.946152);
        }

        private void Map_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PointLatLng point = Map.FromLocalToLatLng((int)e.GetPosition(Map).X, (int)e.GetPosition(Map).Y);
            if (cb1.IsChecked == true)
                pts.Add(point);

            if (cb2.IsChecked == true)
            {
                objectList.Items.Clear();
                objsInList.Clear();
                //лямбда-функция для сортировки списка по указанному критерию
                objs.Sort((obj1, obj2) => obj1.getDistance(point).CompareTo(obj2.getDistance(point)));
                foreach (MapObject mapObject in objs)
                {
                    objsInList.Add(mapObject);
                    objectList.Items.Add("Расстояние до " + mapObject.getTitle() + " - " + mapObject.getDistance(point).ToString("0.00") + " метров");
                }
            }
        }

        private void addObj_Click(object sender, RoutedEventArgs e)
        {
            //проверка на пустое имя
            bool isNewTitle = true;
            foreach (MapObject mapObject in objs)
            {
                if (mapObject.getTitle() == objTitle.Text) isNewTitle = false;
            }

            if (objType.SelectedIndex > -1 && objTitle.Text != "" && isNewTitle)
            {
                if (objType.SelectedIndex == 0)
                {
                    if (pts.Count == 1)
                    {
                        Car car = new Car(objTitle.Text, pts[0]);
                        objs.Add(car);
                    }

                    else
                    {
                        MessageBox.Show("Нужна одна точка");
                    }
                }

                if (objType.SelectedIndex == 1)
                {
                    if (pts.Count == 1)
                    {
                        Human human = new Human(objTitle.Text, pts[0]);
                        objs.Add(human);
                    }

                    else
                    {
                        MessageBox.Show("Нужна одна точка");
                    }

                }

                if (objType.SelectedIndex == 2)
                {
                    if (pts.Count == 1)
                    {
                        Location location = new Location(objTitle.Text, pts[0]);
                        objs.Add(location);
                    }

                    else
                    {
                        MessageBox.Show("Нужна одна точка");
                    }
                }

                if (objType.SelectedIndex == 3)
                {
                    if (pts.Count > 2)
                    {
                        Area area = new Area(objTitle.Text, pts);
                        objs.Add(area);
                    }

                    else
                    {
                        MessageBox.Show("Нужно более трёх точек");
                    }
                    
                }

                if (objType.SelectedIndex == 4)
                {
                    if (pts.Count > 1)
                    {
                        Route route = new Route(objTitle.Text, pts);
                        objs.Add(route);
                    }

                    else
                    {
                        MessageBox.Show("Нужно более двух точек");
                    }
                }
            }

            Map.Markers.Clear();
            objectList.Items.Clear();
            objsInList.Clear();

            foreach (MapObject mapObject in objs)
            {
                Map.Markers.Add(mapObject.getMarker());
                objsInList.Add(mapObject);
                objectList.Items.Add(mapObject.getTitle());
            }

            pts.Clear();
        }

        private void cb1_Checked(object sender, RoutedEventArgs e)
        {
            if (cb1.IsChecked == true)
            {
                cb2.IsChecked = false;

                objType.IsEnabled = true;
                objTitle.IsEnabled = true;
                addObj.IsEnabled = true;
                clearObj.IsEnabled = true;

                pts.Clear();
            }
        }

        private void clearObj_Click(object sender, RoutedEventArgs e)
        {
            Map.Markers.Clear();
            objectList.Items.Clear();
            objs.Clear();
        }

        private void cb2_Checked(object sender, RoutedEventArgs e)
        {
            if (cb2.IsChecked == true)
            {
                cb1.IsChecked = false;

                objType.IsEnabled = false;
                objTitle.IsEnabled = false;
                addObj.IsEnabled = false;
                clearObj.IsEnabled = false;

                pts.Clear();
            }
        }

        private void objectList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (objectList.SelectedIndex >= 0)
            {
                PointLatLng p = objsInList[objectList.SelectedIndex].getFocus();
                Map.Position = p;
                objectList.UnselectAll();
            }                
        }

        private void findObj_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Map.Markers.Clear();
            objectList.UnselectAll();
            objectList.Items.Clear();
            objsInList.Clear();

            foreach (MapObject mapObject in objs)
            {
                if (mapObject.getTitle().Contains(findObj.Text))
                {
                    //Map.Markers.Add(mapObject.getMarker());
                    objsInList.Add(mapObject);
                    objectList.Items.Add(mapObject.getTitle());
                }
            }
        }

        private void Map_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            PointLatLng point = Map.FromLocalToLatLng((int)e.GetPosition(Map).X, (int)e.GetPosition(Map).Y);
            Map.Markers.Clear();
            foreach (MapObject mapObject in objs)
            {
                Map.Markers.Add(mapObject.getMarker());
            }
            if (btn_SetDestination.IsCancel && taxiClient != null)
            {
                taxiClientDestination = new Location("DestinationPoint", point);
                taxiClient.Destination = point;
                Map.Markers.Add(taxiClient.getMarker());
                Map.Markers.Add(taxiClientDestination.getMarker());
            }
            else
            {
                taxiClient = new Human("TaxiClient", point);
                taxiClientDestination = null;
                Map.Markers.Add(taxiClient.getMarker());
            }


        }

        private void Btn_SetDestination_Click(object sender, RoutedEventArgs e)
        {
            if (btn_SetDestination.IsCancel)
            {
                btn_SetDestination.Content = "Указать точку назначения";
                btn_SetDestination.IsCancel = false;
            }
            else
            {
                btn_SetDestination.Content = "Отмена";
                btn_SetDestination.IsCancel = true;
            }
        }

        private void Btn_CallTaxi_Click(object sender, RoutedEventArgs e)
        {
            if (taxiClient == null || taxiClientDestination == null)
                return;
            int distanceToNearestCar = int.MaxValue;
            foreach (MapObject mapObject in objs)
            {
                if (mapObject is Car someCar && someCar.getDistance(taxiClient.getFocus()) < distanceToNearestCar)
                {
                    taxiCar = someCar;             
                }
            }
            if (taxiCar != null)
            {
                Map.Markers.Clear();
                Map.Markers.Add(taxiClient.getMarker());
                Map.Markers.Add(taxiClientDestination.getMarker());
                Map.Markers.Add(taxiCar.getMarker());
                taxiCar.Arrived += taxiClient.carArrived;
                taxiClient.Seated += taxiCar.passengerSeated;
                taxiCar.Moved += this.focusMapOn;
                taxiCar.moveTo(taxiClient.getFocus());
            }
        }
        public void focusMapOn(object sender, EventArgs e)
        {
            if (sender is Car movedCar)
            {
                Map.Position = movedCar.getFocus();
            }
            //Map.Position = point;
        }
    }
}
