using System.Windows;
using System.Windows.Input;
using Leo.WindowModels;

namespace Leo.PageModels;

public partial class Skills
{
    public Skills()
    {
        InitializeComponent();
    }

    private void back(object sender, RoutedEventArgs e)
    {
        MainWindow.backAboutPage();
    }
}