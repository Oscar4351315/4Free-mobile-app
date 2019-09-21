using Xamarin.Forms;

namespace IAB330
{
    public partial class App : Application
    {
        public App()
        {
            try
            {
                InitializeComponent();
                MainPage = new Views.MapPage();
            }
            catch
            {
                System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
            }
        }

        // Handle when your app starts
        protected override void OnStart()
        { }

        // Handle when your app sleeps
        protected override void OnSleep()
        { }

        // Handle when your app resumes
        protected override void OnResume()
        { }
    }
}
