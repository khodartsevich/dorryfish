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
using System.Data;
using System.Data.OleDb;
using System.ComponentModel;
using static Restourant.MainWindow;

namespace Restourant
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 
    public static class Globals
    {
        public static OleDbConnection conn = new OleDbConnection("Provider=Microsoft.Jet.OleDb.4.0;Data Source=RestDB.mdb;");
        public static string MainQuery = "SELECT dishes.dish_name, dishes.dish_price FROM(dishes INNER JOIN dish_classes ON dishes.dish_class = dish_classes.ID_dish_class)";
        public static Desk table1 = new Desk(1, false, 0, 0);
        public static Desk table2 = new Desk(2, false, 0, 0);
        public static Desk table3 = new Desk(3, false, 0, 0);
        public static Desk table4 = new Desk(4, false, 0, 0);
        public static Desk table5 = new Desk(5, false, 0, 0);
        public static Desk table6 = new Desk(6, false, 0, 0);
        public static Desk table7 = new Desk(7, false, 0, 0);
        public static Desk table8 = new Desk(8, false, 0, 0);
        public static Desk table9 = new Desk(9, false, 0, 0);
        public static Desk table10 = new Desk(10, false, 0, 0);
        public static Desk table11 = new Desk(11, false, 0, 0);
        public static Desk table12 = new Desk(12, false, 0, 0);
        public static Desk table13 = new Desk(13, false, 0, 0);
        public static int activeTable = 0;
        public static Desk activeDesk;
        public static List<Dish> AllDishes;
    }

    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            dishesGrid.Visibility = Visibility.Collapsed;
            DeskOrder.Visibility = Visibility.Collapsed;
            DeskTotalLabel.Visibility = Visibility.Collapsed;
            DeskTotalCost.Visibility = Visibility.Collapsed;
            Billbtn.Visibility = Visibility.Collapsed;
            Globals.conn.Open();
            var dtDishes = new DataTable();
            var adapter = new OleDbDataAdapter(Globals.MainQuery, Globals.conn);
            adapter.Fill(dtDishes);

            DishesTable.ItemsSource = dtDishes.DefaultView;
        }

        private void newOrder_Click(object sender, RoutedEventArgs e)
        {
            orderGrid.Visibility = Visibility.Collapsed;
            TableNumber.Text = DesksID.Content.ToString();
            dishesGrid.Visibility = Visibility.Visible;
        }

        private void backToRest_Click(object sender, RoutedEventArgs e)
        {
            var currentTable = Globals.activeDesk;
            MakeOrder(currentTable);
            OrderInfoVisible();


            result.Clear();
            BillDishesTable.Items.Refresh();
            ShowRestaurantWindow();

            //добавляем блюда с активного стола на вкладку "кухня"
            foreach (Dish item in Globals.activeDesk.TableDishes)
            {
                KitchenDataGrid.Items.Add(item);
            }
            KitchenDataGrid.Items.Refresh();
        }

        private void MakeOrder(Desk table)
        {
            table.TableDishes = new List<Dish>(result);
            table.TotalCost = Convert.ToInt32(BillTotalCost.Content);
            table.Order = true;

            DeskOrder.ItemsSource = table.TableDishes;
            DeskOrder.Items.Refresh();
            DeskTotalCost.Content = table.TotalCost;

        }

        public void ChangeDishCountAndCost(int count)
        {
            var totalCost = 0;

            foreach (Dish item in result)
            {
                if (item.billDishName.Contains(dName.Text))
                {
                    item.billDishCount = count.ToString();

                    BillDishesTable.ItemsSource = result;
                    BillDishesTable.Items.Refresh();
                }

                totalCost += Convert.ToInt32(item.billDishPrice) * Convert.ToInt32(item.billDishCount);
            }

            BillTotalCost.Content = totalCost;
        }

        private void BtnRed_Click(object sender, RoutedEventArgs e)
        {
            int dishRed = 1;
            
            dishRed = Convert.ToInt32(selDishCount.Text);
            if (dishRed >= 2)
            {
                dishRed -= 1;
            }
            selDishCount.Text = Convert.ToString(dishRed);

            ChangeDishCountAndCost(dishRed);
        }

        private void BtnInc_Click(object sender, RoutedEventArgs e)
        {
            int dishInc = 1;
            dishInc = Convert.ToInt32(selDishCount.Text);
            dishInc += 1;
            selDishCount.Text = Convert.ToString(dishInc);

            ChangeDishCountAndCost(dishInc);
        }

        private void AddToOrder_Click(object sender, RoutedEventArgs e)
        {
            orderGrid.Visibility = Visibility.Collapsed;
            dishesGrid.Visibility = Visibility.Visible;
        }

        public void IsTableHasOrder(Desk table)
        {
            if (table.Order)
            {
                OrderInfoVisible();
            }
            else
            {
                NewOrderVisible();
            }
        }

        public class Dish
        {
            public Dish(string billDishName, string billDishCount, string billDishPrice)
            {
                this.billDishName = billDishName;
                this.billDishCount = billDishCount;
                this.billDishPrice = billDishPrice;
            }
            public string billDishName { get; set; }
            public string billDishCount { get; set; }
            public string billDishPrice { get; set; }
        }

        public static List<Dish> result = new List<Dish>();

        void Row_Click(object sender, MouseButtonEventArgs e)
        {
            DataRowView selectedRow = (DataRowView)DishesTable.SelectedItem;
            dName.Text = selectedRow.Row["dish_name"].ToString();
            dPrice.Text = selectedRow.Row["dish_price"].ToString();

            Boolean isRowExist = false;

            foreach (Dish item in result)
            {
                if (item.billDishName.Contains(dName.Text))
                {
                    int count = Convert.ToInt32(item.billDishCount) + 1;
                    item.billDishCount = count.ToString();
                    selDishCount.Text = count.ToString();
                    BillTotalCost.Content = (Convert.ToInt32(BillTotalCost.Content)+Convert.ToInt32(item.billDishPrice)).ToString();
                    isRowExist = true;
                    BillDishesTable.ItemsSource = result;
                    BillDishesTable.Items.Refresh();
                    count = 0;
                }
            }

            if (!isRowExist)
            {
                selDishCount.Text = "1";
                result.Add(new Dish(dName.Text, "1", dPrice.Text));
                var selDish = result.Find(_ => _.billDishName.Equals(dName.Text));
                var totalCost = Convert.ToInt32(selDish.billDishCount) * Convert.ToInt32(selDish.billDishPrice);
                BillTotalCost.Content = (Convert.ToInt32(BillTotalCost.Content) + totalCost).ToString();
                BillDishesTable.ItemsSource = result;
                BillDishesTable.Items.Refresh();
            }
        }

        void BillRow_Click(object sender, MouseButtonEventArgs e)
        {
            Dish selectedRow = (Dish)BillDishesTable.SelectedItem;

            dName.Text = selectedRow.billDishName;
            dPrice.Text = selectedRow.billDishPrice;
            selDishCount.Text = selectedRow.billDishCount;
        }

        public void TableFilter(string filter)
        {
            string query = Globals.MainQuery + "WHERE(dish_classes.ID_dish_class = " + filter + ")";

            var dtDishes = new DataTable();
            var adapter = new OleDbDataAdapter(query, Globals.conn);
            adapter.Fill(dtDishes);

            DishesTable.ItemsSource = dtDishes.DefaultView;
        }

        public void ChangeDeskColor (object sender)
        {
            foreach (UIElement rectangle in DeskGrid.Children)
            {
                if (rectangle.GetType() == typeof(Rectangle))
                {
                    Rectangle rect = (Rectangle)rectangle;
                    rect.Fill = Brushes.White;
                }
            }

            Rectangle TheDesk = sender as Rectangle;
            TheDesk.Fill = Brushes.AliceBlue;
        }

        private void Table_1_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var currentTable = Globals.table1;
            TableClick(sender, currentTable);
        }

        private void Table_2_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var currentTable = Globals.table2;
            TableClick(sender, currentTable);
        }

        private void Table_3_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var currentTable = Globals.table3;
            TableClick(sender, currentTable);
        }

        private void Table_4_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var currentTable = Globals.table4;
            TableClick(sender, currentTable);
        }

        private void Table_5_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var currentTable = Globals.table5;
            TableClick(sender, currentTable);
        }

        private void Table_6_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var currentTable = Globals.table6;
            TableClick(sender, currentTable);
        }

        private void Table_7_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var currentTable = Globals.table7;
            TableClick(sender, currentTable);
        }

        private void Table_8_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var currentTable = Globals.table8;
            TableClick(sender, currentTable);
        }

        private void Table_9_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var currentTable = Globals.table9;
            TableClick(sender, currentTable);
        }

        private void Table_10_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var currentTable = Globals.table10;
            TableClick(sender, currentTable);
        }

        private void Table_11_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var currentTable = Globals.table11;
            TableClick(sender, currentTable);
        }

        private void Table_12_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var currentTable = Globals.table12;
            TableClick(sender, currentTable);
        }

        private void Table_13_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var currentTable = Globals.table13;
            TableClick(sender, currentTable);
        }

        public void ChangeFilterColor(object sender)
        {
            foreach (UIElement button in FilterButtonsGrid.Children)
            {
                if (button.GetType() == typeof(Button))
                {
                    Button btn = (Button)button;
                    btn.Foreground = btn.BorderBrush;
                    btn.Background = Brushes.White;
                }
            }

            Button TheBtn = sender as Button;
            TheBtn.Foreground = Brushes.White;
            TheBtn.Background = TheBtn.BorderBrush;
        }

        private void TableClick(object sender, Desk table)
        {
            IsTableHasOrder(table);
            Globals.activeTable = table.Id;
            DeskOrder.ItemsSource = table.TableDishes;
            DeskOrder.Items.Refresh();
            DeskTotalCost.Content = table.TotalCost;
            DesksID.Content = table.Id;

            ChangeDeskColor(sender);
            Globals.activeDesk = Globals.table1;

            ////определяем какой стол кликнут
            //Rectangle TheDesk = sender as Rectangle;
            //TheDesk.Fill = Brushes.AliceBlue;
            ////определяем текст с этого стола-кнопки 
            //String TblNum = TheDesk.Id.ToString();

            //MessageBox.Show(TblNum);
        }

        private void AllMenu_Click(object sender, RoutedEventArgs e)
        {
            string query = Globals.MainQuery;
            var dtDishes = new DataTable();
            var adapter = new OleDbDataAdapter(query, Globals.conn);
            adapter.Fill(dtDishes);

            DishesTable.ItemsSource = dtDishes.DefaultView;
            ChangeFilterColor(sender);
        }

        private void FilterHot_Click(object sender, RoutedEventArgs e)
        {
            TableFilter("1");
            ChangeFilterColor(sender);
        }

        private void FilterCold_Click(object sender, RoutedEventArgs e)
        {
            TableFilter("2");
            ChangeFilterColor(sender);
        }

        private void FilterSnack_Click(object sender, RoutedEventArgs e)
        {
            TableFilter("3");
            ChangeFilterColor(sender);
        }

        private void FilterDesserts_Click(object sender, RoutedEventArgs e)
        {
            TableFilter("4");
            ChangeFilterColor(sender);
        }

        private void FilterDrinks_Click(object sender, RoutedEventArgs e)
        {
            TableFilter("5");
            ChangeFilterColor(sender);
        }

        private void BillDishTableRemoveRow_Click(object sender, RoutedEventArgs e)
        {
            Dish selectedRow = (Dish)BillDishesTable.SelectedItem;
            BillTotalCost.Content = (Convert.ToInt32(BillTotalCost.Content) - Convert.ToInt32(selectedRow.billDishPrice)* Convert.ToInt32(selectedRow.billDishCount)).ToString();
            result.Remove(selectedRow);
            selDishCount.Text = "0";

            BillDishesTable.ItemsSource = result;
            BillDishesTable.Items.Refresh();
        }

        public class Desk
        {
            public int Id { get; set; }
            public Boolean Order { get; set; }
            public int Status { get; set; }
            public int TotalCost { get; set; }
            public List<Dish> TableDishes { get; set; }

            //public List<Dish> TableDishes = new List<Dish>();

            public Desk(int id, Boolean order, int status, int totalCost)
            {
                Id = id;
                Order = order;
                Status = status;
                TotalCost = totalCost;
            }
        }

        private void Billbtn_Click(object sender, RoutedEventArgs e)
        {
            Globals.activeDesk.TableDishes.Clear();
            NewOrderVisible();         
            Globals.activeDesk.TotalCost = 0;
            DeskTotalCost.Content = "0";

            DeskOrder.ItemsSource = Globals.activeDesk.TableDishes;
            DeskOrder.Items.Refresh();
        }

        private void NewOrderVisible()
        {
            AddToOrder.Content = "Новый заказ";
            DeskOrder.Visibility = Visibility.Hidden;
            DeskTotalCost.Visibility = Visibility.Hidden;
            DeskTotalLabel.Visibility = Visibility.Hidden;
            Billbtn.Visibility = Visibility.Hidden;
            CancelOrder.Visibility = Visibility.Hidden;

        }

        private void OrderInfoVisible()
        {
            AddToOrder.Content = "Добавить к заказу";
            DeskOrder.Visibility = Visibility.Visible;
            DeskTotalCost.Visibility = Visibility.Visible;
            DeskTotalLabel.Visibility = Visibility.Visible;
            Billbtn.Visibility = Visibility.Visible;
            CancelOrder.Visibility = Visibility.Visible;
        }

        private void ShowRestaurantWindow()
        {
            dishesGrid.Visibility = Visibility.Collapsed;
            orderGrid.Visibility = Visibility.Visible;
            DeskOrder.Visibility = Visibility.Visible;
            DeskTotalLabel.Visibility = Visibility.Visible;
            DeskTotalCost.Visibility = Visibility.Visible;
            Billbtn.Visibility = Visibility.Visible;
        }

        private void CancelOrder_Click(object sender, RoutedEventArgs e)
        {
            DeleteOrder();
            NewOrderVisible();
            DeskOrder.Items.Refresh();
        }

        private void DeleteOrder()
        {
            var currentDesc = Globals.activeDesk;
            currentDesc.TableDishes.Clear();
            currentDesc.Order = false;
            currentDesc.TotalCost = 0;
            DeskTotalCost.Content = 0;
            BillTotalCost.Content = 0;
        }
    }


    public class Dish
    {
        public string DishName { get; set; }
        public int DishCost { get; set; }
        public int DishCount { get; set; }
        public int TotalDishCount { get; set; }
        public int Status { get; set; }

        public Dish(string dishName, int dishCost, int dishCount)
        {
            DishName = dishName;
            DishCost = dishCost;
            DishCount = dishCount;
            TotalDishCount = dishCost * dishCount;
        }
    }
}
