using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace Cpp2IL_Gui
{
    public partial class MainWindow : Window
    {
        private string outputDir;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BrowseApk_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                Filter = "APK files (*.apk)|*.apk",
                Title = "Select an APK File"
            };

            if (dlg.ShowDialog() == true)
            {
                ApkFileTextBox.Text = dlg.FileName;
            }
        }

        private async void RunCpp2Il_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ApkOutputTextBox.Clear();

                string apkPath = ApkFileTextBox.Text;
                if (string.IsNullOrWhiteSpace(apkPath) || !File.Exists(apkPath))
                {
                    AppendApkOutput("Please select a valid APK file first.");
                    return;
                }

                string cpp2ilExe = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Cpp2IL.exe");
                if (!File.Exists(cpp2ilExe))
                {
                    AppendApkOutput("Cpp2IL.exe not found in the application directory!");
                    return;
                }

                if (string.IsNullOrWhiteSpace(outputDir))
                    outputDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cpp2il_out");
                if (!Directory.Exists(outputDir))
                    Directory.CreateDirectory(outputDir);

                AppendApkOutput("Running Cpp2IL...");
                AppendApkOutput("Loading files, please wait...");

                string args = $"--game-path=\"{apkPath}\" --output-as dummydll --output-to \"{outputDir}\" --use-processor attributeinjector --processor-config key=value";

                var startInfo = new ProcessStartInfo
                {
                    FileName = cpp2ilExe,
                    Arguments = args,
                    WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                var process = new Process { StartInfo = startInfo, EnableRaisingEvents = true };

                process.Start();


                AppendApkOutput("Cpp2IL finished! (However if it isnt there straight away please Wait!)");
            }
            catch (Exception ex)
            {
                AppendApkOutput($"Error running Cpp2IL: {ex.Message}");
            }
        }


        private void AppendApkOutput(string message)
        {
            ApkOutputTextBox.AppendText(message + Environment.NewLine);
            ApkOutputTextBox.ScrollToEnd();
        }

        private void Close_Click(object sender, RoutedEventArgs e) => this.Close();

        private void TopBar_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
                this.DragMove();
        }

        private void MainTabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (MainTabControl.SelectedContent is UIElement content)
            {
                content.Opacity = 0;
                content.BeginAnimation(UIElement.OpacityProperty, new System.Windows.Media.Animation.DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300)));
            }
        }
    }
}
