using Drastic.UI;
using Drastic.UI.Platform.WPF;

namespace SampleApp.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : FormsApplicationPage
    {
        public MainWindow()
        {
            InitializeComponent();
            UI.Init();
            LoadApplication(new SampleApp.App());
        }
    }
}
