using AutoService11.Classes;
using AutoService11.Entity;
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

namespace AutoService11.Pages
{
    /// <summary>
    /// Логика взаимодействия для ServiceClient.xaml
    /// </summary>
    public partial class ServiceClient : Page
    {
        Client client = new Client();
        AutoServiseEntities db = new AutoServiseEntities();
        public ServiceClient(Client client, AutoServiseEntities db)
        {
            InitializeComponent();
            this.client = client;
            this.db = db;
            DataContext = this.client;
            TbClientInfo.Text = $"{client.FirstName} {client.LastName} {client.Patronymic}({client.ID})";
            if (client.ServiceList.Count > 0)
            {
                LViewService.ItemsSource = this.client.ServiceList;
            }
            else
            {
                LViewService.Visibility = Visibility.Collapsed;
                spServiceInfo.Children.Clear();
                TextBlock tb = new TextBlock();
                tb.Text = "У данного клиента нет посещений";
                tb.FontSize = 22;
                tb.HorizontalAlignment = HorizontalAlignment.Center;
                tb.VerticalAlignment = VerticalAlignment.Center;
                spServiceInfo.Children.Add(tb);
            }
        }
    }
}
