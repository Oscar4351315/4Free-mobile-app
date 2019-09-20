using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using IAB330.Models;
using IAB330.ViewModels;

namespace IAB330.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {
        public MapPage()
        {
            InitializeComponent();
            //CommandParameter = "{Binding Source={x:Reference CategoryEntry}, Path=Text}"

            

            //PostInfo Post = new PostInfo()
            //{
            //    CategoryEntry = CategoryEntry.Text
            //};
            //string name = OnPropertyChange(CategoryEntry.Text);          
            //GetFormBtn.CommandParameter = name;
            BindingContext = new MapViewModel();
        }
    }
}