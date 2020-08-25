﻿using System;
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
using System.Windows.Forms;
using System.IO;
using ChScript.Models;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;


namespace ChScript
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BindingList<Scripts> _scriptsData;
        private List<String> _nameFileList = new List<String>();
        private string _directoryFile;
        private SqlConnection _sqlConnection;

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// формирует списка имен файлов скриптов при нажатии ев кнопку "Папка"
        /// </summary>
        private void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _directoryFile = dialog.SelectedPath;
                AdressFolder.Text = _directoryFile;
                DirectoryInfo directoryInfo = new DirectoryInfo(dialog.SelectedPath);
                string name;
                foreach (var file in directoryInfo.GetFiles("*.sql"))
                {
                    name = file.Name.Remove(file.Name.Length - 4, 4);
                    _nameFileList.Add(name);
                }
            } 
        }
      
        /// <summary>
        /// подключение к базе данных
        /// </summary>
        public DataTable Select(string selectSQL)
        {
            DataTable dataTable = new DataTable("dataBase");                                                                                // подключаемся к базе данных
            _sqlConnection = new SqlConnection($@"server={TextServer.Text};Trusted_Connection=Yes;DataBase={TextDB.Text};");
            try
            {
                _sqlConnection.Open();
                SqlCommand sqlCommand = _sqlConnection.CreateCommand();
                sqlCommand.CommandText = selectSQL;
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                sqlDataAdapter.Fill(dataTable);
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("Отсутствует подключение к БД! Проверте подключение к БД", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            return dataTable;
        }
        /// <summary>
        /// формирует список скриптов, сравнивание их со списком файлов, вывод результата  в DataGrid
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DataTable scriptDB = Select("SELECT sc.Name FROM[dbo].[OSCRIPT_EXECUTED] as sc");
            var scriptDBList = new List<String>();
            for (int i = 0; i < scriptDB.Rows.Count; i++) scriptDBList.Add(scriptDB.Rows[i][0].ToString());
            string status = "не выполнен";
            _scriptsData = new BindingList<Scripts>();
            foreach (var file in _nameFileList)
            {
                foreach (var nameFile in scriptDBList) if (file == nameFile) status = "выполнен";
                _scriptsData.Add(new Scripts(file, status));
            }
            scriptsList.ItemsSource = _scriptsData;
        }
        
        /// <summary>
        /// фильтрация по статусу скрипта
        /// </summary>
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var filtrScript = new BindingList<Scripts>();
            try
            {
                switch (filterStatus.SelectedIndex)
                {
                    case (0):
                        //scriptsList.ItemsSource = _scriptsData.Where(x => x.StatusScript == "выполнен");
                        filtrScript.Clear();
                        foreach (var scrData in _scriptsData)
                        {
                            if (scrData.StatusScript == "выполнен")
                            {
                                filtrScript.Add(scrData);
                            }
                        }
                        scriptsList.ItemsSource = filtrScript;
                        break;
                    case (1):
                        //scriptsList.ItemsSource = _scriptsData.Where(x => x.StatusScript == "не выполнен");
                        filtrScript.Clear();
                        foreach (var scrData in _scriptsData)
                        {
                            if (scrData.StatusScript == "не выполнен")
                            {
                                filtrScript.Add(scrData);
                            }
                        }
                        scriptsList.ItemsSource = filtrScript;
                        break;
                    case (2):
                        scriptsList.ItemsSource = _scriptsData;
                        break;
                }
            }
            catch (Exception)
            {
                scriptsList.ItemsSource = _scriptsData;
            }   
        }
        /// <summary>
        /// по клику "Установить скрипт" , устанавливает скрипты, на которых стоит метка, в БД 
        /// </summary>
        private void usingScript_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var script in _scriptsData.Where(x => x.IsMark == true))
                {
                    
                    string connect = $@"server={TextServer.Text};Trusted_Connection=Yes;DataBase={TextDB.Text};";
                    SqlConnection con = new SqlConnection(connect);
                    string [] separatingString  = { "GO" };
                    string scripts  = File.ReadAllText(_directoryFile + "\\" + script.NameScript + ".sql");
                    string [] sepScripts = scripts.Split(separatingString, StringSplitOptions.RemoveEmptyEntries);
                    con.Open();
                    foreach (var i in sepScripts)
                    {
                        var sqlCmd = new SqlCommand(i, con);
                        sqlCmd.ExecuteNonQuery();
                    }
                }
                System.Windows.MessageBox.Show("Скрипты установлены", "Отчет", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception )
            { 
                System.Windows.MessageBox.Show("Отсутствует подключение к БД \n или не указана директория скриптов.", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }   
    }
}