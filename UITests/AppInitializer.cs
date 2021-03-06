﻿using Xamarin.UITest;

namespace UITests
{
    public class AppInitializer
    {
        public static IApp StartApp(Platform platform)
        {
            if (platform == Platform.Android)
            {
                return ConfigureApp.Android.InstalledApp("com.rmjcs.simonish").StartApp();
            }

            return ConfigureApp.iOS.StartApp();
        }
    }
}