using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System.Net;
using Xamarin.Forms;

namespace conv
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            ViewModel viewModel = new ViewModel();

            BindingContext = viewModel;
        }
    }
}
