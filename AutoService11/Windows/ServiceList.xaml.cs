using AutoService11.Entity;
using AutoService11.Pages;
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
using System.Windows.Shapes;
using AutoService11.Classes;
using AutoService11.Entity;

namespace AutoService11.Windows
{
    /// <summary>
    /// Логика взаимодействия для ServiceList.xaml
    /// </summary>
    public partial class ServiceList : Window
    {
        public ServiceList(Client client, AutoServiseEntities db)
        {
            InitializeComponent();
            frame.Navigate(new ServiceClient(client,db));
        }

        private void frame_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            try
            {
                ServiceClient pg = (ServiceClient)e.Content;
            }
            catch { };
        }
    }
}
