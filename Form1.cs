﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using System.Reflection;
using System.IO;
using BLL = LotoStatistics.BusinessLogicLayer.BusinessLogicLayer;
using System.Globalization;

namespace LotoStatistics {

    public partial class Form1: Form {

        Dictionary<string, string> monthSeason = new Dictionary<string, string>();
        Dictionary<string, string> seasonLang = new Dictionary<string, string>();

        public Form1() {

            monthSeason.Add("Ocak", "Winter");
            monthSeason.Add("Şubat", "Winter");
            monthSeason.Add("Mart", "Spring");
            monthSeason.Add("Nisan", "Spring");
            monthSeason.Add("Mayıs", "Spring");
            monthSeason.Add("Haziran", "Summer");
            monthSeason.Add("Temmuz", "Summer");
            monthSeason.Add("Ağustos", "Summer");
            monthSeason.Add("Eylül", "Fall");
            monthSeason.Add("Ekim", "Fall");
            monthSeason.Add("Kasım", "Fall");
            monthSeason.Add("Aralık", "Winter");
            
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e) {
            ballsComboBox.SelectedIndex = 0;
            seasonsComboBox.SelectedIndex = 0;
            BLL bll = new BLL();
            dataGrid.DataSource = bll.Get();
        }

        private void ballButton_Click(object sender, EventArgs e) {
            BLL bll = new BLL();
            dataGrid.Columns.Clear();

            //Get selected balls statistics
            dataGrid.DataSource = bll.GetBallStats(ballsComboBox.SelectedItem.ToString());
            
            //Calculate total amount of draw for one ball
            int sum=0;
            for (int i = 0; i < dataGrid.Rows.Count - 1; i++) {
                sum += (int) dataGrid[dataGrid.Columns[ballsComboBox.SelectedItem.ToString() + "SAYI"].Index, i].Value;
            }

            //Add a new column with a name that depending on ball
            dataGrid.Columns.Add(ballsComboBox.SelectedItem.ToString() + "ratio", ballsComboBox.SelectedItem.ToString() + "%");
            //Add the show ratios for each number on the ball
            for (int i = 0; i < dataGrid.Rows.Count - 1; i++) {
                dataGrid[ballsComboBox.SelectedItem.ToString() + "ratio", i].Value = Convert.ToDouble(dataGrid[dataGrid.Columns[ballsComboBox.SelectedItem.ToString() + "ratio"].Index - 1, i].Value) * 100 / sum;
            }
        }

        private void dataGrid_CellContentClick(object sender, DataGridViewCellEventArgs e) {
        }

        private void ballsComboBox_KeyPress(object sender, KeyPressEventArgs e) {
            ballButton_Click(sender, e);
        }

        private void showSeasonButton_Click(object sender, EventArgs e) {
            ballButton_Click(sender, e);
            BLL bll = new BLL();
            dataGrid.Columns.Clear();

            dataGrid.DataSource = bll.GetSeasonStats(ballsComboBox.SelectedItem.ToString(), seasonsComboBox.SelectedItem.ToString());

            for (int i = 0; i < dataGrid.Rows.Count - 1; i++) {
                dataGrid[0, i].Value = dataGrid[0, i].Value.ToString().Substring(dataGrid[0, i].Value.ToString().IndexOf(" ") + 1);
                dataGrid[0, i].Value = dataGrid[0, i].Value.ToString().Split(' ').First();
            }

            for (int i = 0; i < dataGrid.Rows.Count - 1; i++) {
                dataGrid[0, i].Value = monthSeason[dataGrid[0, i].Value.ToString()];
            }

            dataGrid.Sort(dataGrid.Columns[0], ListSortDirection.Ascending);

            for (int i = 0; i < dataGrid.Rows.Count -1; i++) {
                if (dataGrid[0, i].Value.ToString() != seasonsComboBox.SelectedItem.ToString()) {
                    dataGrid.Rows.RemoveAt(i);
                    i--;
                }
            }
            int repeat = 1;
            int row = 0;
            dataGrid.Columns.Add("Repeat", "Repeat");
            for (int i = 0; i < dataGrid.Rows.Count - 2; i++) {
                if (dataGrid[1, i].Value.ToString() == dataGrid[1, i+1].Value.ToString()) {
                    repeat++;
                    dataGrid.Rows.RemoveAt(i + 1);
                    i--;
                } else {
                    dataGrid[2, i].Value = Convert.ToDouble(repeat);
                    repeat = 1;
                    row = i + 1;
                }
            }
        }
    }
}
