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
    public partial class CreateDockPage : Page
    {
        public CreateDockPage()
        {
            InitializeComponent();
            IniPicture();
            IniWarehouses();

            name.ValueDataType = typeof(string);
        }

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
            foreach (DataRow warehouse in Tables.warehouses.database.Rows)
            {
                Warehouses.Add(warehouse["name"].ToString(), warehouse);
                warehouse_id.Items.Add(warehouse["name"].ToString());
            }
        }

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            DataRow dock = Tables.docks.database.NewRow();
            if (! Validation.ValidateTextbox(name, dock) && ! Validation.ValidateCombobox(warehouse_id, dock))
            {
                dock["name"] = name.Text;
                dock["warehouse_id"] = Warehouses[warehouse_id.SelectedItem.ToString()]["id"];
                dock["free"] = true;

                Tables.docks.database.Rows.Add(dock);
                Tables.docks.updateChanges();

                Controller.LogWrite(User.currentUser["email"].ToString(), $"{User.currentUser["name"]} has created {dock["name"]} dock.");
                MessageBox.Show("Dock has successfully created!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                Navigation.OpenPage(Navigation.PreviousPage.GetType());
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (Navigation.PreviousPage != null)
            {
                Navigation.OpenPage(Navigation.PreviousPage.GetType());
            }
            else
            {
                Navigation.OpenPage(Navigation.GetTypeByName("DockPage"));
            }
        }
    }
}
