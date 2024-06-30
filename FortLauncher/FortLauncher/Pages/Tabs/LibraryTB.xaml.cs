using System;
using System.Collections.Generic;
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

namespace FortLauncher.Pages.Tabs
{
    /// <summary>
    /// Interaction logic for HomeTB.xaml
    /// </summary>
    public partial class LibraryTB : Page
    {
        public class BorderInfo
        {
            public Border Border { get; set; }
            public int PlacementId { get; set; }
            public string BuildId { get; set; }
        }
        List<BorderInfo> borderInfoList = new List<BorderInfo>();

        public LibraryTB()
        {
            InitializeComponent();
        }

        public async Task LoadBuilds()
        {
            string BaseFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string DataFolder = Path.Combine(BaseFolder, "FortLauncher");
            string FilePath = Path.Combine(DataFolder, "builds.json");


            try
            {
                if (!(borderInfoList.Count > 0))
                {
                    while (LaunchBuilds.Children.Count > 1)
                    {
                        LaunchBuilds.Children.RemoveAt(0);
                    }

                    await Task.Run(async () =>
                    {

                        if (File.Exists(FilePath))
                        {

                        }

                    });
                }
            }
            catch { }
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            LoadBuilds(); // Yes!
        }
    }
}
