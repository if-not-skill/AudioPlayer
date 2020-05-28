using System;
using System.Net.Mime;
using Xamarin.Forms;

namespace KillerAIMP
{
    public class SplashPage : ContentPage
    {
        private Image splashImage;

        public SplashPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            
            var sub = new AbsoluteLayout();
            splashImage = new Image()
            {
                Source = "icon.png",
                WidthRequest = 100,
                HeightRequest = 100
            };
            
            AbsoluteLayout.SetLayoutFlags(splashImage, AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(splashImage, 
                new Rectangle(0.5, 0.5, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));

            sub.Children.Add(splashImage);

            this.BackgroundColor = Color.FromHex("#181818");
            this.Content = sub;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await splashImage.ScaleTo(1, 2000);
            await splashImage.ScaleTo(0.9, 1500, Easing.Linear);
            await splashImage.ScaleTo(150, 1200, Easing.Linear);
            Application.Current.MainPage = new MainPage();
        }
    }
}