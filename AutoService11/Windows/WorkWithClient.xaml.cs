﻿using AutoService11.Classes;
using AutoService11.Entity;
using AutoService11.Pages;
using Microsoft.Win32;
using Mono.Cecil;
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

namespace AutoService11.Windows
{
    /// <summary>
    /// Логика взаимодействия для WorkWithClient.xaml
    /// </summary>
    public partial class WorkWithClient : Window
    {
        Client client = new Client();
        private AutoServiseEntities db;
        Client currentClient = new Client();
        private ClientList clientList;
        public WorkWithClient(Client currentClient, AutoServiseEntities db, ClientList clientPage)
        {
            InitializeComponent();
            this.clientList = clientPage;
            this.db = db;
            cmbGender.ItemsSource = db.Gender.ToList();
            if (currentClient != null)
            {
                this.client = currentClient;
                LViewTags.ItemsSource = client.Tags;
                tbID.Text = client.ID.ToString();
                cmbGender.SelectedIndex = client.GenderCode - 1;
                dpBirthDate.SelectedDate = client.Birthday;
                this.Title = "Редактирование инфомрмации о клиенте";
            }
            else if (currentClient == null)
            {
                this.currentClient = currentClient;
                btnDeleteClient.IsEnabled = false;
                TagGrid.Visibility = Visibility.Collapsed;
                spID.Visibility = Visibility.Collapsed;
                this.Title = "Добавление инфомрмации о клиенте";
            }
            DataContext = client;
            
        }

        private void btnEnterImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var asmName = new AssemblyNameDefinition("DynamicAssembly", new Version(1, 0, 0, 0));
                var assembly = AssemblyDefinition.CreateAssembly(asmName, "<Module>", ModuleKind.Dll);
                OpenFileDialog GetImageDialog = new OpenFileDialog(); // Открытие диалогового окна
                string folderpath = System.IO.Directory.GetParent(Environment.CurrentDirectory).Parent.FullName + "\\Resourses\\Клиенты\\";
                GetImageDialog.Title = "Выберите изображение";
                GetImageDialog.Filter = "Файлы изображений: (*.png,*.jpg,*.jpeg)| *.png;*.jpg;*.jpeg"; // Фильтр типов файлов
                GetImageDialog.InitialDirectory = folderpath;
                if (GetImageDialog.ShowDialog() == true)
                    client.PhotoPath = GetImageDialog.SafeFileName;//Добавление наименования файла в БД
                var sourse = new BitmapImage(new Uri(GetImageDialog.FileName));
                imgPhoto.Source = sourse;
                /*
                string filePath = folderpath + System.IO.Path.GetFileName(GetImageDialog.FileName);
                File.Copy(GetImageDialog.FileName, filePath, true);
                byte[] imageData = File.ReadAllBytes(filePath);
                var imageResource = new EmbeddedResource(GetImageDialog.FileName, ManifestResourceAttributes.Private, imageData);
                assembly.MainModule.Resources.Add(imageResource);*/
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDelImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string defaultImagePath = (string)FindResource("defaultImage"); // Получаем путь к изображению из ресурсов
                if (!string.IsNullOrEmpty(defaultImagePath))
                {
                    BitmapImage defaultImage = new BitmapImage(new Uri(defaultImagePath)); // Создаем BitmapImage из пути
                    imgPhoto.Source = defaultImage; // Устанавливаем изображение по умолчанию
                }
                client.PhotoPath = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Phone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !ValidatorExtensions.IsValidPhone(e.Text);
        }

        private void tbFirstName_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !ValidatorExtensions.IsValidFIO(e.Text);
        }

        private void tbLastName_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !ValidatorExtensions.IsValidFIO(e.Text);
        }

        private void tbPatronymic_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !ValidatorExtensions.IsValidFIO(e.Text);
        }

        private void tbEmail_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !ValidatorExtensions.IsValidEmailAddress(e.Text);
        }

        private void tbColorTag_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //e.Handled = !ValidatorExtensions.IsValidColor(e.Text);
            if (tbColorTag.Text.Length >= 6 && !e.Text.Equals("\b"))
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = !ValidatorExtensions.IsValidColor(e.Text);
            }
        }

        private void btnDelTag_Click(object sender, RoutedEventArgs e)
        {
            int count = LViewTags.SelectedItems.Count;
            if (count > 0)
            {
                if (MessageBox.Show($"Вы действительно хотите удалить {count} запись(ей)?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    try
                    {
                        var selected = LViewTags.SelectedItems.Cast<Tag>().ToArray();
                        foreach (var item in selected)
                        {
                            db.Tag.Remove(item);
                        }
                        db.SaveChanges();
                        string about;
                        if (count == 1) about = "Тег удален!";
                        else about = "Теги удалены!";
                        MessageBox.Show(about, "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                        LViewTags.ItemsSource = client.Tags;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите теги для удаления", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnAddTag_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StringBuilder errors = new StringBuilder();
                Errors(tbColorTag.Text == "", errors, "Введите цвет!");
                Errors(tbTitleTag.Text == "", errors, "Заголовог тега!");
                if (errors.Length > 0)
                {
                    MessageBox.Show(errors.ToString());
                    return;
                }
                Tag tag = new Tag
                {
                    ID = db.Tag.Any() ? db.Tag.Max(t => t.ID) + 1 : 1,
                    Title = tbTitleTag.Text,
                    Color = tbColorTag.Text
                };
                client.Tag.Add(tag);
                db.SaveChanges();
                tbTitleTag.Text = ""; tbColorTag.Text = "";
                LViewTags.ItemsSource = client.Tags;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAddClient_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StringBuilder errors = new StringBuilder();
                Errors(cmbGender.SelectedItem == null, errors, "Выберите пол клиента!");
                Errors(tbFirstName.Text.Count() > 50, errors, "Фамилия не может быть длиннее 50 символов!");
                Errors(tbLastName.Text.Count() > 50, errors, "Имя не может быть длиннее 50 символов!");
                Errors(tbPatronymic.Text.Count() > 50, errors, "Отчество не может быть длиннее 50 символов!");
                Errors(ValidatorExtensions.IsValidEmailAddress(tbEmail.Text), errors, "Email адрес не действителен");
                Errors(tbFirstName.Text == ""
                    || tbLastName.Text == ""
                    || tbPhone.Text == "", errors, "Не заполнена важная ифнормация!");
                if (errors.Length > 0)
                {
                    MessageBox.Show(errors.ToString());
                    return;
                }
                if (currentClient == null)
                {
                    client.RegistrationDate = DateTime.Now;
                    RefrData();
                    db.Client.Add(client);
                    SaveInDB("Добавление информации о клиенте завершено");
                }
                else
                {
                    RefrData();
                    SaveInDB("Обновление информации о клиенте завершено");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefrData()
        {
            client.Birthday = (DateTime)dpBirthDate.SelectedDate;
            client.GenderCode = cmbGender.SelectedIndex + 1;
        }

        private void SaveInDB(string text)
        {
            db.SaveChanges();
            MessageBox.Show(text, "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private void Errors(bool expression, StringBuilder errors, string message)
        {
            if (expression)
            {
                errors.AppendLine(message);
            }
        }

        private void btnDeleteClient_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (client.ClientService.Count > 0)
                {
                    MessageBox.Show("Удаление не возможно, т.к. есть информация о посещении", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                foreach (Tag tag in client.Tag)
                {
                    db.Tag.Remove(tag);
                }
                db.Client.Remove(client);
                db.SaveChanges();
                MessageBox.Show("Удаление информации об клиенте завешено!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void AddAndEditClientWindow_Closed(object sender, EventArgs e)
        {
            try
            {
                if (clientList != null)
                {
                    clientList.Load(); // Вызываем метод на ClientList
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
