using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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

namespace WH_APP_GUI.Dock
{
    public partial class EditDockPage : Page
    {
        private DataRow dock;
        public EditDockPage(DataRow dock)
        {
            InitializeComponent();
            IniPicture();
            IniWarehouses();
            IsInUseDock((bool)dock["free"]);
            name.ValueDataType = typeof(string);
            name.Text = dock["name"].ToString();
            if (dock["warehouse_id"] != DBNull.Value)
            {
                warehouse_id.SelectedItem = Tables.docks.getWarehouse(dock)["name"];
            }
            this.dock = dock;
        }
        #region Display
        private void IniPicture()
        {
            string targetDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../Images");
            if (Directory.Exists(targetDirectory))
            {
                string imageFileName = "DockDefaultImage.png";
                string imagePath = Path.Combine(targetDirectory, imageFileName);
                if (File.Exists(imagePath))
                {
                    string fileName = Path.GetFileName(imagePath);
                    string targetFilePath = Path.Combine(targetDirectory, fileName);

                    BitmapImage bitmap = new BitmapImage(new Uri(targetFilePath));
                    image.Source = bitmap;
                }
            }
        }
        private Dictionary<string, DataRow> Warehouses = new Dictionary<string, DataRow>();
        private void IniWarehouses()
        {
            Warehouses.Clear();
            warehouse_id.Items.Clear();

            if (User.currentUser.Table.TableName != "employees")
            {
                foreach (DataRow warehouse in Tables.warehouses.database.Rows)
                {
                    Warehouses.Add(warehouse["name"].ToString(), warehouse);
                    warehouse_id.Items.Add(warehouse["name"].ToString());
                }
            }
            else
            {
                DataRow warehouse = Tables.employees.getWarehouse(User.currentUser);
                Warehouses.Add(warehouse["name"].ToString(), warehouse);
                warehouse_id.Items.Add(warehouse["name"].ToString());
                warehouse_id.SelectedItem = warehouse["name"].ToString();
            }
        }
        private void EditDockPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (var child in alapgrid.Children)
            {
                FontSize = e.NewSize.Height * 0.03;
            }
            image.Width = image.Height;
        }
        #endregion
        #region Methods
        private void IsInUseDock(bool is_in_use)
        {
            InUse.Content = is_in_use ? "Free" : "In use";
            InUse.Background = is_in_use ? Brushes.Blue : Brushes.Red;
        }
        private void InUse_Click(object sender, RoutedEventArgs e)
        {
            bool IsInUse = bool.Parse(dock["free"].ToString()) ? false : true;
            dock["free"] = IsInUse;
            Tables.docks.updateChanges();
            IsInUseDock(IsInUse);
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Navigation.OpenPage(Navigation.PreviousPage.GetType());
        }
        private void Done_Click(object sender, RoutedEventArgs e)
        {
            if (!Validation.ValidateTextbox(name, dock) && !Validation.ValidateCombobox(warehouse_id, dock))
            {
                dock["name"] = name.Text;
                dock["warehouse_id"] = Warehouses[warehouse_id.SelectedItem.ToString()]["id"];
                Tables.docks.updateChanges();
                Navigation.OpenPage(Navigation.PreviousPage.GetType());
            }
        }
        #endregion
    }
}
